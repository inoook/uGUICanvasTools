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
	}

	public static Vector3 WorldToCanvasPosition(Vector3 pos, Canvas canvas, Camera viewCam){

		RectTransform canvasRectT = canvas.GetComponent<RectTransform>();
		Vector3 screenPos = viewCam.WorldToScreenPoint(pos);
		Vector3 worldPos = Vector2.zero;
		RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRectT, screenPos, canvas.worldCamera, out worldPos);
		return worldPos;
	}

	/// <summary>
	/// rectTrans(image) を変形しpos0, pos1を結合するラインとして描画
	/// </summary>
	/// <param name="pos0"></param>
	/// <param name="pos1"></param>
	/// <param name="rectTrans"></param>
	/// <param name="thickness"></param>
	public static void DrawLine(Vector3 pos0, Vector3 pos1, RectTransform rectTrans, float thickness)
	{
		// drawLine
		float dist = Vector3.Distance(pos0, pos1);

		rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, dist);
		rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, thickness);

		rectTrans.pivot = new Vector2(0, 0.5f); // update pivot

		Vector3 dir = pos1 - pos0;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		rectTrans.eulerAngles = new Vector3(0, 0, angle);

		rectTrans.anchoredPosition = pos0;
	}

	#endregion
}
