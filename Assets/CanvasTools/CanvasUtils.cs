using UnityEngine;
using System.Collections;

public class CanvasUtils {

	#region static method
	// http://answers.unity3d.com/questions/799616/unity-46-beta-19-how-to-convert-from-world-space-t.html
	// http://westhillapps.blog.jp/archives/44661739.html
	// http://docs.unity3d.com/ScriptReference/RectTransformUtility.ScreenPointToWorldPointInRectangle.html

	public static Vector2 WorldToCanvasAnchoredPosition(Vector3 pos, Canvas canvas, Camera viewCam){
		Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(viewCam, pos) * 1.0f / canvas.scaleFactor;
		RectTransform canvasRectT = canvas.GetComponent<RectTransform>();
		return screenPos - canvasRectT.sizeDelta / 2f;

//		RectTransform canvasRectT = canvas.GetComponent<RectTransform>();
//		Vector3 screenPos = viewCam.WorldToScreenPoint(pos);
//		Vector2 worldPos = Vector2.zero;
//		RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectT, screenPos, canvas.worldCamera, out worldPos);
//		return worldPos;
	}

	public static Vector3 WorldToCanvasPosition(Vector3 pos, Canvas canvas, Camera viewCam){

		RectTransform canvasRectT = canvas.GetComponent<RectTransform>();
		Vector3 screenPos = viewCam.WorldToScreenPoint(pos);
		Vector3 worldPos = Vector2.zero;
		RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRectT, screenPos, canvas.worldCamera, out worldPos);
		return worldPos;
	}

	#endregion
}
