using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// http://wonderfl.net/c/4IBM
public class PointToCatmullRom : MonoBehaviour {

	public Camera worldCam;

	Graphics graphics;
	public CanvasQuadDrawer verticesDrawer;
	public bool useSmoothing = true;
	public bool useCatmull = true;

	private List<Vector3> points;
	private List<List<Vector3>> pointsList;

	public float thickness = 2.0f;
	public Color drawColor = Color.red;

	private Vector2 preWorldPos;

	// Use this for initialization
	void Start () {
		graphics = new Graphics();
		graphics.verticesDrawer = verticesDrawer;

		pointsList = new List<List<Vector3>>();
//		points = new List<Vector3>();
//		pointsList.Add(points);
	}
	
	// Update is called once per frame
	void Update () {

		graphics.curveSmoothing = useCatmull && useSmoothing;

		Vector3 mouse = Input.mousePosition;
		mouse.z = 10;

		RectTransform rectTrans = this.gameObject.GetComponent<RectTransform>();
		Camera canvasCam = this.GetComponentInParent<Canvas>().worldCamera;
		Vector2 worldPos = Vector2.zero;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTrans, mouse, canvasCam, out worldPos);
		float mouseX = worldPos.x;
		float mouseY = worldPos.y;

		if(Input.GetMouseButtonDown(0)){
			// create new pointGroup
			points = new List<Vector3>();
			pointsList.Add(points);

//			plot(mouseX, mouseY);
		}

		if(Input.GetMouseButton(0)){
			float delta = Vector2.Distance(worldPos, preWorldPos);
			if(delta > 5){
				Plot(mouseX, mouseY);
				preWorldPos = worldPos;
			}
		}
		Draw();
	}

	private void Plot(float x, float y)
	{
		points.Add(new Vector3(x, y, 0));
	}

	// update
	private void Draw()
	{
		graphics.Clear();
		graphics.SetDefaultColor(drawColor);
		graphics.SetDefaultThickness(thickness);

		for(int i = 0; i < pointsList.Count; i++){
			List<Vector3> points = pointsList[i];
			float[] distances = new float[points.Count];
			Color[] colors = new Color[points.Count];
			for(int j = 0; j < distances.Length; j++){
				if(j == 0){
					distances[j] = thickness;
					colors[j] = drawColor;
				}else{
					distances[j] = (Vector3.Distance(points[j-1], points[j]) / 40) * thickness + thickness;
					float t = 1 - Mathf.Clamp01(Vector3.Distance(points[j-1], points[j]) / 100);
					colors[j] = Color.Lerp(Color.black, drawColor, t);
				}
			}
			//
			if(useCatmull){
				graphics.DrawSpline(points.ToArray(), distances, colors);
			}else{
				graphics.DrawLine(points.ToArray(), distances);
			}
		}
		graphics.Render();
	}

}


