using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BoxCollider2DGizmoColor : MonoBehaviour {

	[SerializeField]
	private Color m_GizmoColor;

	/// <summary> ギズモの描画. </summary>
	private void OnDrawGizmos(){
		Gizmos.color = m_GizmoColor;
		var pos = transform.position;
		pos.x += GetComponent<BoxCollider2D>().offset.y;
		pos.y += GetComponent<BoxCollider2D>().offset.x;
		var scale = new Vector2(GetComponent<BoxCollider2D>().size.y, GetComponent<BoxCollider2D>().size.x);
		Gizmos.DrawWireCube(pos, scale);
	}
}
