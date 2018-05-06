/*
 * コライダ管理クラス.
 * 
 * @file	ColliderManager.cs
 * @author	Lotos
 * @date	2018-5-06
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderManager : MonoBehaviour {

	#region Param
    // コライダリスト.
    private List<GameObject> m_ColList = new List<GameObject>();

    // コライダ.
    private Collider2D m_Collider2D;
	#endregion


	#region Property
	// 当たり判定内にあるオブジェクト.
    public List<GameObject> ColList{
        get { return m_ColList; }
    }

    public Collider2D Col{
        get {return m_Collider2D; }
    }
	#endregion

    void Start(){
        m_Collider2D = transform.GetComponent<Collider2D>();
    }

    /// <summary> 当たり判定開始. </summary>
    void OnTriggerEnter2D(Collider2D other){
        if (!m_ColList.Contains(other.gameObject)){
            if(m_Collider2D != null){
                m_ColList.Add(other.gameObject);
            }
        }
    }

    /// <summary> 当たり判定終了. </summary>
    void OnTriggerExit2D(Collider2D other){
        if (m_ColList.Contains(other.gameObject)){
			m_ColList.Remove(other.gameObject);
		}  
    }
}
