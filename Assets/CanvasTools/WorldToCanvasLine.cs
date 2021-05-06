using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent (typeof (Image))]
public class WorldToCanvasLine : MonoBehaviour {

	public Camera worldViewCam;
	public Canvas canvas;

	public Transform p0;
	public Transform p1;

	public float thickness = 1.0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
		Vector3 pos0 = WorldToCanvasProt.WorldToCanvasAnchoredPosition(p0.position, canvas, worldViewCam); 
		Vector3 pos1 = WorldToCanvasProt.WorldToCanvasAnchoredPosition(p1.position, canvas, worldViewCam);

		RectTransform drawRectTrans = this.gameObject.GetComponent<RectTransform>();
		CanvasLine.DrawLine(pos0, pos1, drawRectTrans, thickness);
	}
}
