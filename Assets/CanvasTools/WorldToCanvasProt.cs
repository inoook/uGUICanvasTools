using UnityEngine;
using UnityEngine.UI;
using System.Collections;


/// <summary>
/// Worldに配置された target の位置を canvas 上に表示
/// </summary>
[ExecuteInEditMode]
[RequireComponent (typeof (Image))]
public class WorldToCanvasProt : MonoBehaviour {

	public Camera worldViewCam = null;
	public Canvas canvas = null;

	public Transform target;
	
	// Update is called once per frame
	void Update () {
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
