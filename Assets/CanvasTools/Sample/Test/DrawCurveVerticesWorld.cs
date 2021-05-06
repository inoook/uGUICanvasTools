using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawCurveVerticesWorld : MonoBehaviour {

	public CanvasQuadDrawer drawer;

	public float thickness = 1.0f;
	public AnimationCurve curve;
	
	public Vector3 amp = Vector3.one;
	public int splitNum = 100;

	public Color color = Color.yellow;

	private CanvasGraphics graphics;

	public bool curveSmoothing = true;
    [SerializeField] bool isUseAvg = true;
	
	// Use this for initialization
	void Start () {
		graphics = new CanvasGraphics();
		graphics.verticesDrawer = drawer;
	}

	
	// Update is called once per frame
	void Update () {
        graphics.curveSmoothing = curveSmoothing;
        graphics.isUseAvg = isUseAvg;

		amp.x = Screen.width;

		float num = splitNum;
		float d = 1.0f / num;

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
		}
		graphics.Render();

	}
}
