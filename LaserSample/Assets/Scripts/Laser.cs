/*
 * レーザークラス.
 * 
 * @file	Laser.cs
 * @author	Lotos
 * @date	2018-5-06
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {

	#region Inspector
	[Header("--LineRenderer")]
	[SerializeField]
	private LineRenderer m_Laser;

	[Header("--レーザー当たり判定")]
	[SerializeField]
	private BoxCollider2D m_LaserCollider;

	[Header("--レーザーレイキャストルート")]
	[SerializeField]
	private Transform m_RaycastRoot;

	[Header("--レーザーの太さ")]
	[SerializeField]
	private float m_LaserWidth = 2f;

	[Header("--レーザーの最大の長さ")]
	[SerializeField]
	private float m_LaserMaxLength = 15f;

	[Header("--レーザーの速度")]
	[SerializeField]
	private float m_LaserSpeed = 60f;

	[Header("--ピボット")]
	[SerializeField]
	private Transform m_Pivot;

	[Header("--衝突Offset")]
	[SerializeField]
	private float m_HitOffset = 1.5f;

	[Header("--コライダ管理")]
	[SerializeField]
	private ColliderManager m_ColMg;
	#endregion

	#region Param
	// レーザー衝突レイキャスト.
	private RaycastHit2D m_LaserHitRaycast;

	// レーザーの長さ.
	private float m_LaserLength;

	// レーザーレイキャストの長さ.
	private float m_LaserRaycastLength;

	// レーザー終了ポジション.
	private Vector3 m_LaserEndPos;

	// レーザーコライダ終了サイズ.
	private Vector3 m_LaserColliderEndSize;

	// 再生するか.
	private bool m_isPlay = false;

	// 衝突コールバック.
	private System.Action<List<GameObject>> m_AttackCallback = null;
	#endregion

	#region Property
	// コライダ管理クラス.
	public ColliderManager ColMg{
		get { return m_ColMg; }
	}
	#endregion

	/// <summary> 初期化. </summary>
	/// <param name="_attackCallback"> 衝突コールバック. </param>
	public void Init(System.Action<List<GameObject>> _attackCallback){
		// レーザーコライダのサイズ.
		m_LaserCollider.size = new Vector2(m_LaserWidth * 0.5f, 0);
		m_LaserCollider.offset = Vector2.zero;

		// レーザーの太さ.
		m_Laser.widthMultiplier = m_LaserWidth;

		// レーザーの長さを初期化.
		m_Laser.SetPosition(1, Vector3.zero);

		// レーザーの当たり判定をONに.
		m_LaserCollider.enabled = true;

		// レーザー終了ポジション.
		m_LaserEndPos = new Vector3(0, m_LaserMaxLength, 0);

		// レーザーコライダ終了ポジション.
		m_LaserColliderEndSize = new Vector2(m_LaserCollider.size.x, m_LaserMaxLength);

		m_AttackCallback = _attackCallback;

		// レーザー再生.
		m_isPlay = true;
	}

	private void FixedUpdate() {
		
		if(!m_isPlay) return;

		// レーザー最大ポジション.
		var laserMaxPos = m_LaserEndPos;

		// レーザーコライダの最大サイズ.
		var laserColliderMaxSize = m_LaserColliderEndSize;

		// レーザーレイキャストの衝突情報を取得.
		m_LaserHitRaycast = Physics2D.Raycast(m_RaycastRoot.position, m_Pivot.up, m_LaserRaycastLength);

#if UNITY_EDITOR
		Debug.DrawRay(m_RaycastRoot.position, m_Pivot.up * m_LaserRaycastLength, Color.green);
#endif
		// レイキャストに衝突したとき.
		if(m_LaserHitRaycast.collider != null){

			// レーザーの位置を衝突位置に戻す.

			var laserLineDistance = m_LaserHitRaycast.distance;
			laserLineDistance += m_HitOffset;
			
			// レーザーとコライダのサイズを衝突した位置に.
			laserMaxPos = new Vector3(0f, Mathf.Clamp(laserLineDistance, 0, 10000), 0f);
			laserColliderMaxSize = new Vector2(m_LaserCollider.size.x, Mathf.Clamp(laserLineDistance, 0, 10000));

			//　レイキャストの長さを衝突した位置に.
			m_LaserRaycastLength = laserLineDistance;

			// レーザーの位置を変更.
			m_Laser.SetPosition(1, laserMaxPos);

			// レーザーコライダのサイズを変更.
			m_LaserCollider.size = laserColliderMaxSize;

		}else{
			
			// レーザーの位置を伸ばす.

			//　レイキャストの長さ変更.
			m_LaserRaycastLength = Mathf.MoveTowards(m_LaserRaycastLength, m_LaserMaxLength, (m_LaserSpeed * Time.deltaTime));

			// レーザーの位置を変更.
			m_Laser.SetPosition(1, Vector3.MoveTowards(m_Laser.GetPosition(1), laserMaxPos, m_LaserSpeed * Time.deltaTime));

			// レーザーコライダのサイズを変更.
			m_LaserCollider.size = Vector2.MoveTowards(m_LaserCollider.size, laserColliderMaxSize, m_LaserSpeed * Time.deltaTime);
		}

		// 衝突コールバック実行.
		if(m_AttackCallback != null){
			m_AttackCallback(m_ColMg.ColList);
		}

		// レーザーコライダの位置調整.
		var offSet = m_LaserCollider.offset;
		offSet.y = m_LaserCollider.size.y * 0.5f;
		m_LaserCollider.offset = offSet;
	}
}
