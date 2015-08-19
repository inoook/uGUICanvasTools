using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawCurveVertices : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	public VerticesDrawer drawer;

	public float thickness = 1.0f;
	public AnimationCurve curve;
	
	public Vector2 amp = Vector2.one;
	public int splitNum = 100;

	public Color color = Color.yellow;
	
	// Update is called once per frame
	void Update () {

		drawer.Clear();
		drawer.SetColor(color);

		amp.x = Screen.width;

		float num = splitNum;
		float d = 1.0f / num;
		for(int i = 0; i < num; i++){
			float x0 = d * i;
			float y0 = curve.Evaluate(x0);
			
			float x1 = d * (i+1);
			float y1 = curve.Evaluate(x1);

			Vector2 p0 = new Vector2(x0,y0);
			p0.Scale(amp);
			Vector2 p1 = new Vector2(x1,y1);
			p1.Scale(amp);
			drawer.DrawLine(p0, p1, thickness);
		}
		
		drawer.Render();
	}
}
