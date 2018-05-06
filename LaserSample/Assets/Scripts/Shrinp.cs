/*
 * 海老クラス.
 * 
 * @file	Shrinp.cs
 * @author	Lotos
 * @date	2018-5-06
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrinp : MonoBehaviour {

	#region Inspector
	[Header("--HP")]
	[SerializeField]
	private float m_Hp = 60;

	[Header("--ターゲット")]
	[SerializeField]
	private Transform m_Target;

	[Header("--コライダ")]
	[SerializeField]
	private Collider2D m_Col2D;

	[Header("--間隔")]
	[SerializeField]
	private float m_Interval = 1.5f;

	[Header("--始まりの座標")]
	[SerializeField]
	private Vector3 m_BeforePos = new Vector3(0, -1, 0);

	[Header("--終わりの座標")]
	[SerializeField]
	private Vector3 m_AfterPos = new Vector3(0, 1, 0);

	[Header("--炎エフェクト")]
	[SerializeField]
	private GameObject m_FireEffect;

	[Header("--エビ画像")]
	[SerializeField]
	private SpriteRenderer m_ShrinpSprite;

	[Header("--エビ影画像")]
	[SerializeField]
	private SpriteRenderer m_ShrinpShadowSprite;

	[Header("--エビ焼ける音")]
	[SerializeField]
	private AudioSource m_ShrinpBurnSe;
	#endregion
	
	#region Param
	// 最大HP.
	private float m_MaxHp;

	// 経過時間.
	private float m_Time;

	// 変化した値.
	private Vector3 m_ChangePos;
	#endregion

	#region Property
	// 死亡したか.
	public bool IsDead { get; private set; }
	#endregion

	#region Const
	// 死亡変化ポジション.
	private static readonly float DEAD_POS_CHANGE_VALUE = 0.3f;
	#endregion

	void Start(){
		m_MaxHp = m_Hp;
	}

	void Update(){
		if(m_Target == null || IsDead){
			return;
		}
		ShrinpMove();
	}

	/// <summary> 海老の上下移動. </summary>
	private void ShrinpMove(){
		m_Time = Mathf.PingPong(Time.time, m_Interval);
		m_Time = m_Time / m_Interval;
		m_ChangePos = Vector3.Slerp(m_BeforePos, m_AfterPos, m_Time);
		m_ChangePos.z = 0f;
		m_Target.localPosition = m_ChangePos;
	}

	/// <summary> ダメージ. </summary>
	/// <param name="_damage"> ダメージ. </param>
	public void OnDamage(float _damage){
		if(IsDead){
			return;
		}

		// ダメージエフェクト表示.
		m_FireEffect.SetActive(true);

		// ダメージHp.
		DamageHp(_damage);
	}

	/// <summary> HPダメージ. </summary>
	/// <param name="_damage"></param>
	private void DamageHp(float _damage){
		var hp = m_Hp;
		hp -= _damage;
		m_Hp = Mathf.Clamp(hp, 0, m_MaxHp);
		if(m_Hp <= 0){
			Dead();
			IsDead = true;
		}
	}

	/// <summary> ノーダメージ. </summary>
	public void OnNotDamage(){
		// ダメージエフェクト非表示.
		m_FireEffect.SetActive(false);
	}
	
	/// <summary> 死亡. </summary>
	private void Dead(){
		// ダメージエフェクト非表示.
		m_FireEffect.SetActive(false);

		// 当たり判定を停止.
		m_Col2D.enabled = false;

		// 死亡アニメーション開始.
		StartCoroutine(DeadAnimation());
	}

	/// <summary> 死亡アニメーション. </summary>
	/// <returns></returns>
	private IEnumerator DeadAnimation(){
		var isDone1 = false;
		var isDone2 = false;
		var isDone3 = false;

		// 海老焼ける音再生.
		m_ShrinpBurnSe.Play();

		// 海老変化色.
		var shrinpChangeColor = m_ShrinpSprite.color;
		shrinpChangeColor.a = 0f;

		// 海老影変化色.
		var shrinpShadowChangeColor = m_ShrinpShadowSprite.color;
		shrinpShadowChangeColor.a = 0f;

		// 海老変化ポジション.
		var shrinpChangePos = transform.localPosition;
		shrinpChangePos.y -= DEAD_POS_CHANGE_VALUE;

		// 海老色、海老影色アルファ変更.
		StartCoroutine(ColorChange(m_ShrinpSprite, m_ShrinpSprite.color, shrinpChangeColor, 0.5f, (isDone) => isDone1 = isDone));
		StartCoroutine(ColorChange(m_ShrinpShadowSprite, m_ShrinpShadowSprite.color, shrinpShadowChangeColor, 0.5f, (isDone) => isDone2 = isDone));

		// ポジション変更.
		StartCoroutine(PosChange(2.0f, transform, shrinpChangePos, (isDone) => isDone3 = isDone));

		yield return new WaitWhile(() => !isDone1);
		yield return new WaitWhile(() => !isDone2);
		yield return new WaitWhile(() => !isDone3);
	}

	/// <summary> 色の変更. </summary>
	/// <param name="_sp"> スプライト. </param>
	/// <param name="_beforeColor"> 変更前色. </param>
	/// <param name="_targetColor"> 変更後色. </param>
	/// <param name="_changeTime"> 変更までの時間. </param>
	/// <param name="_endCallback"> 終了コールバック. </param>
	private IEnumerator ColorChange(SpriteRenderer _sp, Color _beforeColor, Color _targetColor, float _changeTime, System.Action<bool> _endCallback = null){
		var elapsedTime = 0f;
		var totalTime = _changeTime;
		while (elapsedTime < totalTime){
			elapsedTime += Time.deltaTime;
			_sp.color = Color.Lerp(_beforeColor, _targetColor, (elapsedTime / totalTime));
			yield return null;
		}
		if (_endCallback != null){
			_endCallback(true);
		}
		yield break;
	}

	/// <summary> ポジションの変更. </summary>
	/// <param name="_endTime"> 終了時間. </param>
	/// <param name="_targetTra"> 変更対象. </param>
	/// <param name="ToPos"> 変更後のポジション. </param>
	/// <param name="_endCalback"> 終了後のコールバック. </param>
	private IEnumerator PosChange(float _endTime, Transform _targetTra, Vector3 ToPos, System.Action<bool> _endCalback){
		// 開始時間.
		var startTime = Time.timeSinceLevelLoad;

		// 経過時間.
		var diffTime = 0f;

		var isFinish = false;
		do{
			diffTime = Time.timeSinceLevelLoad - startTime;
			var rate = diffTime / _endTime;
			_targetTra.localPosition = Vector3.Lerp(_targetTra.localPosition, ToPos, rate);
			if (diffTime > _endTime){
				isFinish = true;
			}
			yield return null;
		} while (isFinish == false);
		if (_endCalback != null){
			_endCalback(true);
		}
		yield break;
	}
}
