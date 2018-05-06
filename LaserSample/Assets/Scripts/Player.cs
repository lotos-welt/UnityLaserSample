/*
 * プレイヤークラス.
 * 
 * @file	Player.cs
 * @author	Lotos
 * @date	2018-5-06
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	#region Inspector
	[Header("--レーザー")]
	[SerializeField]
	private Laser m_Laser;

	[Header("--ピボット")]
	[SerializeField]
	private Transform m_Pivot;

	[Header("--レーザーダメージ間隔時間")]
	[SerializeField]
	private float m_laserDamageIntervalTime = 0.5f;

	[Header("--大砲操作速度")]
	[SerializeField]
	private float m_CannonControlSpeed = 30f;

	[Header("--レーザー攻撃力")]
	[SerializeField]
	private float m_LaserAttackValue = 10f;
	#endregion

	#region Param
	// レーザーダメージ間隔計算時間.
	private float m_LaserDamageRepeatTime = 0f;

	// 回転値.
	private float m_Rotate;

	// 前回衝突した海老リスト.
	private List<Shrinp> m_BeforeAttackShrinpList = new List<Shrinp>();
	#endregion
	
	#region Const
	// レーザー最小制御角度.
	private static readonly float CANNON_CONTROL_MIN_ANGLE = -90;

	// レーザー最大制御角度.
	private static readonly float CANNON_CONTROL_MAX_ANGLE = 90;
	#endregion

	void Start(){
		m_Laser.Init(LaserAttack);
	}

	void Update(){
		// 砲台入力受付.
		var axis = Input.GetAxis("Horizontal");
		if(axis > 0){
			m_Rotate -= Time.deltaTime * m_CannonControlSpeed;
		}else if(axis < 0){
			m_Rotate += Time.deltaTime * m_CannonControlSpeed;
		}

		// 砲台の回転.
		m_Rotate = Mathf.Clamp(m_Rotate, CANNON_CONTROL_MIN_ANGLE, CANNON_CONTROL_MAX_ANGLE);
		m_Pivot.localRotation = Quaternion.Euler(0, 0, m_Rotate);
	}

	/// <summary> レーザー攻撃. </summary>
	/// <param name="_colList"> 当たり判定一覧. </param>
	private void LaserAttack(List<GameObject> _colList){
		if(_colList.Count > 0){

			// 衝突した海老を一時保持.
			for(int idx = 0; idx < _colList.Count; ++idx){
				var shrinp = _colList[idx].GetComponent<Shrinp>();
				if(!m_BeforeAttackShrinpList.Contains(shrinp)){
					m_BeforeAttackShrinpList.Add(shrinp);
				}
			}

			// ダメージ間隔によってダメージを与える.
			m_LaserDamageRepeatTime -= Time.deltaTime;
			if(m_LaserDamageRepeatTime <= 0f){
				m_LaserDamageRepeatTime = m_laserDamageIntervalTime;
				for(int idx = 0; idx < _colList.Count; ++idx){
					var shrinp = _colList[idx].GetComponent<Shrinp>();
					shrinp.OnDamage(m_LaserAttackValue);
				}
			}

			// 保持した海老リストに現在の衝突した海老以外をリストから削除.
			for(int idx = 0; idx < m_BeforeAttackShrinpList.Count; ++idx){
				if(!_colList.Contains(m_BeforeAttackShrinpList[idx].gameObject)){
					m_BeforeAttackShrinpList[idx].OnNotDamage();
					m_BeforeAttackShrinpList.RemoveAt(idx);
				}
			}

		}else if(_colList.Count <= 0){
			LaserNotAttack();
		}
	}

	/// <summary> レーザーがあたっていないとき. </summary>
	private void LaserNotAttack(){
		m_LaserDamageRepeatTime = 0f;
		for(int idx = 0; idx < m_BeforeAttackShrinpList.Count; ++idx){
			m_BeforeAttackShrinpList[idx].OnNotDamage();
		}
		m_BeforeAttackShrinpList.Clear();
	}
}
