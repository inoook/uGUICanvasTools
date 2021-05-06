using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawCurveVerticesWorld : MonoBehaviour {

	public VerticesDrawer drawer;

	public float thickness = 1.0f;
	public AnimationCurve curve;
	
	public Vector3 amp = Vector3.one;
	public int splitNum = 100;

	public Color color = Color.yellow;
<<<<<<< HEAD:Assets/CanvasTools/Sample/Test/DrawCurveVerticesWorld.cs

	private CanvasGraphics graphics;

	public bool curveSmoothing = true;
    [SerializeField] bool isUseAvg = true;
	
	// Use this for initialization
	void Start () {
		graphics = new CanvasGraphics();
		graphics.verticesDrawer = drawer;
=======
	
	// Use this for initialization
	void Start () {
		
>>>>>>> parent of d8c896b (graphics追加):Assets/Sample/DrawCurveVertices.cs
	}

	
	// Update is called once per frame
	void Update () {
        graphics.curveSmoothing = curveSmoothing;
        graphics.isUseAvg = isUseAvg;

		drawer.Clear();
		drawer.SetColor(color);

		amp.x = Screen.width;

		float num = splitNum;
		float d = 1.0f / num;

<<<<<<< HEAD:Assets/CanvasTools/Sample/Test/DrawCurveVerticesWorld.cs
		d = Mathf.Clamp01(d);

		graphics.Clear();
		
        Vector3 dir = -Vector3.forward;
        graphics.SetDefaultFaceDir(dir);
        
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
=======
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
>>>>>>> parent of d8c896b (graphics追加):Assets/Sample/DrawCurveVertices.cs
		}
		
		drawer.Render();
	}
}
