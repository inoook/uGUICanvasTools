using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent (typeof (Image))]
public class CanvasLine : MonoBehaviour {
	
	public RectTransform p0;
	public RectTransform p1;

	public float thickness = 1.0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
		Vector3 pos0 = p0.anchoredPosition; 
		Vector3 pos1 = p1.anchoredPosition; 
		
		RectTransform drawRectTrans = this.gameObject.GetComponent<RectTransform>();
		CanvasLine.DrawLine(pos0, pos1, drawRectTrans, thickness);
	}

	#region static method
	public static void DrawLine(Vector3 pos0, Vector3 pos1, RectTransform rectTrans, float thickness)
	{
		// drawLine
		float dist = Vector3.Distance(pos0, pos1);

		rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, dist);
		rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, thickness);
		
		rectTrans.pivot = new Vector2(0, 0.5f); // update pivot
		
		Vector3 dir = pos1 - pos0;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		rectTrans.eulerAngles = new Vector3(0,0, angle);
		
		rectTrans.anchoredPosition = pos0;
	}
	#endregion
}
