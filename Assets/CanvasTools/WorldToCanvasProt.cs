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
		rectTrans.anchoredPosition = CanvasUtils.WorldToCanvasAnchoredPosition(target.position, canvas, worldViewCam);
	}
}
