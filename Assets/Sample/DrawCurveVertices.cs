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

	private Graphics graphics;
	
	// Use this for initialization
	void Start () {
		graphics = new Graphics();
		graphics.verticesDrawer = drawer;
//		graphics.curveSmoothing = false;
	}
	
	// Update is called once per frame
	void Update () {

		amp.x = Screen.width;

		float num = splitNum;
		float d = 1.0f / num;

		d = Mathf.Clamp01(d);

		graphics.Clear();

		for(int i = 0; i < num+1; i++){
			float x = d * i;
			float y = curve.Evaluate(x);

			Vector3 pos = new Vector3(x,y,0);
			pos.Scale(amp);

			Color c = Color.Lerp(Color.black, color, x);

			if(i == 0){
				graphics.MoveTo(pos, c, thickness);
			}else{
				graphics.LineTo(pos, c, thickness);
			}
		}
		graphics.Render();

	}
}
