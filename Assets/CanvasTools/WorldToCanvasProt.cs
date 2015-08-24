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
		rectTrans.anchoredPosition = CanvasUtils.WorldToCanvasAnchoredPosition(target.position, canvas, worldViewCam);
	}
}
