using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class CanvasLineTest : MonoBehaviour
{
	public RectTransform p0;
	public RectTransform p1;

	public float thickness = 1.0f;

	// Update is called once per frame
	void LateUpdate()
	{
		Vector3 pos0 = p0.anchoredPosition;
		Vector3 pos1 = p1.anchoredPosition;

		RectTransform drawRectTrans = this.gameObject.GetComponent<RectTransform>();
		CanvasUtils.DrawLine(pos0, pos1, drawRectTrans, thickness);
	}
}
