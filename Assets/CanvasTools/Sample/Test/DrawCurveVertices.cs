using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawCurveVertices : MonoBehaviour {

	public CanvasQuadDrawer drawer;

	public float thickness = 1.0f;
	public AnimationCurve curve;
	
	public Vector2 amp = Vector2.one;
	public int splitNum = 100;

	public Color color = Color.yellow;

	private CanvasGraphics graphics;

	public bool curveSmoothing = false;

	[SerializeField] RectTransform rectTrans = null;
	
	// Use this for initialization
	void Awake () {
		graphics = new CanvasGraphics();
		graphics.verticesDrawer = drawer;
	}

	[SerializeField] bool isAutoUpdate = false;

	void Update () {

		if (isAutoUpdate) {
            CurveRender();
			//KeyframesRender();

			//ForceUpdate ();
		}
	}

	// Update is called once per frame

	// curve
	void CurveRender()
	{
		graphics.curveSmoothing = curveSmoothing;

		amp.x = rectTrans.rect.width;
		amp.y = rectTrans.rect.height;

		float offsetX = -rectTrans.pivot.x;
		float offsetY = -rectTrans.pivot.y;

		float num = splitNum;
		float d = 1.0f / num;

		d = Mathf.Clamp01(d);

		graphics.Clear();

		for (int i = 0; i < num + 1; i++)
		{
			float x = d * i;
			float y = curve.Evaluate(x);

			Vector3 pos = new Vector3(x + offsetX, y + offsetY, 0);
			pos.Scale(amp);

			//			Color c = Color.Lerp(Color.black, color, x);
			Color c = color;

			if (i == 0)
			{
				graphics.MoveTo(pos, c, thickness);
			}
			else
			{
				graphics.LineTo(pos, c, thickness);
			}
		}
		graphics.Render();

	}

	// curve の keyframe で描画
	void KeyframesRender()
	{
		graphics.curveSmoothing = false;

		amp.x = rectTrans.rect.width;
		amp.y = rectTrans.rect.height;

		float offsetX = -rectTrans.pivot.x;
		float offsetY = -rectTrans.pivot.y;

		Keyframe[] keys = curve.keys;
		Color c = color;

		graphics.Clear();

		for (int i = 0; i < keys.Length; i++)
		{
			Keyframe key = keys[i];
			float x = key.time;
			float y = key.value;

			Vector3 pos = new Vector3(x + offsetX, y + offsetY, 0);
			pos.Scale(amp);

			if (i == 0)
			{
				graphics.MoveTo(pos, c, thickness);
			}
			else
			{
				graphics.LineTo(pos, c, thickness);
			}
		}
		graphics.Render();
	}

	// curve の keyframe で描画 (drawer 直接)
	public void ForceUpdate()
	{
		drawer.Init ();

		amp.x = rectTrans.rect.width;
		amp.y = rectTrans.rect.height;

		float offsetX = -rectTrans.pivot.x;
		float offsetY = -rectTrans.pivot.y;

		Keyframe[] keys = curve.keys;
		Color c = color;

		drawer.Clear();


		for(int i = 0; i < keys.Length-1; i++){
			Keyframe key0 = keys [i];
			Keyframe key1 = keys [i+1];

			Vector3 pos0 = new Vector3(key0.time + offsetX, key0.value + offsetY,0);
			pos0.Scale(amp);

			Vector3 pos1 = new Vector3(key1.time + offsetX, key1.value + offsetY,0);
			pos1.Scale(amp);

			drawer.DrawLine (pos0, pos1, thickness, c);
		}
		drawer.Render ();
	}

	void OnDisable()
	{
		drawer.Clear();
	}

	void OnEnable()
	{
		ForceUpdate ();
	}

}
