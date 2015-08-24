using UnityEngine;
using System.Collections;

public class QuadTest : MonoBehaviour {

	public CanvasQuadDrawer drawer;

	public int numX = 10;
	public int numY = 10;

	public Vector2 offset;

	public float size = 1.0f;

	public bool isUpdate = true;

	public float angle = 0.0f;

	// Use this for initialization
	void Start () {
		Draw();
	}
	
	// Update is called once per frame
	void Update () {
		if(!isUpdate){ return; }

		Draw();
	}

	void Draw()
	{
		drawer.Clear();

		// quadTest
		Rect rect = new Rect(-0.5f, -0.5f, 1,1);
		
		float offsetX = offset.x * numX / 2.0f;
		float offsetY = offset.y * numY / 2.0f;

		angle += Time.deltaTime * 50;
		float _angle = angle;
		
		for(int i = 0; i < numX; i++){
			for(int j = 0; j < numY; j++){
				float x = i * offset.x - offsetX;
				float y = j * offset.y - offsetY;
				_angle += 0.1f;
				
				float r = 1.0f/numX * i;
				float g = 1.0f/numY * j;
				Color color = new Color(r, g, 0, 1);
				
				drawer.DrawQuad(rect, x, y, size, size, _angle, color); 
			}
		}

//		// lineTest
//		Color color = Color.red;
//		drawer.AddQuadLine(0, 0, 100, 10, angle, color, true);
//		drawer.AddQuadLine(200, 0, 100, 10, angle, color);
//		drawer.AddQuadLine(400, 200, 100, 100, angle, color);

//		// Test
//		Rect rect0 = new Rect(0, -0.5f, 1,1);
//		drawer.DrawQuad(rect0, 200, 200, 100, 10, angle, Color.yellow);

//		Vector3[] points = new Vector3[4];
//		float d = Mathf.PI / 5;
//		for(int i = 0; i < 2; i++){
//			points[i] = new Vector3(Mathf.Cos(d * i), Mathf.Sin(d * i), 0);
//		}
//		points[2] = Vector3.zero;
//		points[3] = Vector3.zero;
//		drawer.AddVector3Vertex(points, 200,200, 100, 100, angle, Color.blue);

//		//  TestC Circle
//		drawer.DrawCircle(radius,200,200,divid, Color.blue, innerRadius, 1, 1, angle);
//		drawer.DrawCircle(radius,50,200,divid, Color.blue, innerRadius, 1, 1, angle);

		drawer.Render();
	}
	public int divid = 12;
	public float radius = 200;
	public float innerRadius = 100;
}
