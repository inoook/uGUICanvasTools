using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent (typeof (Image))]
public class WorldToCanvasProt : MonoBehaviour {

	public Camera worldViewCam;
	public Canvas canvas;
	private RectTransform canvasRectT;

	public Transform target;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
		RectTransform rectTrans = this.gameObject.GetComponent<RectTransform>();
		rectTrans.anchoredPosition = WorldToCanvasAnchoredPosition(target.position, canvas, worldViewCam);
	}

	#region static method
	// http://answers.unity3d.com/questions/799616/unity-46-beta-19-how-to-convert-from-world-space-t.html
	public static Vector2 WorldToCanvasAnchoredPosition(Vector3 pos, Canvas canvas, Camera viewCam){
		Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(viewCam, pos) * 1.0f / canvas.scaleFactor;
		RectTransform canvasRectT = canvas.GetComponent<RectTransform>();
		return screenPos - canvasRectT.sizeDelta / 2f;
	}
	#endregion
}
