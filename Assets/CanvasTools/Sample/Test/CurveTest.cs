using UnityEngine;
using System.Collections;

public class CurveTest : MonoBehaviour {
	
	public CanvasQuadDrawer drawer;
	
	public float thickness = 1.0f;
	public Color color = Color.yellow;
	public CanvasGraphics graphics;
	
	// Use this for initialization
	void Start () {
		graphics = new CanvasGraphics();
		graphics.verticesDrawer = drawer;

	}

	Vector3[] point;
	public int num = 12;
	public float r = 10;

	public bool curveSmoothing = true;


	void UpdatePoints ()
	{
		num = (num <= 1) ? 2 : num;
		float d = Mathf.PI * 2 / num;
		
		point = new Vector3[num+1+1];
		for (int i = 0; i < (num+1+1); i++) {
			float x = r * Mathf.Sin (d * i);
			float z = r * Mathf.Cos (d * i);
			Vector3 pt = new Vector3 (x, z, 0);
			point[i] = pt;
		}
	}
	
	// Update is called once per frame
	void Update () {

		UpdatePoints();

		graphics.Clear();
		graphics.curveSmoothing = curveSmoothing;

		float d = 1.0f / num;
        for (int i = 0; i < (num + 1); i++)
        {
            Vector3 pt = point[i];
            Color c = Color.Lerp(Color.black, color, d * i);
            float thickness = Mathf.Lerp(10, 50, d * i);
            if (i == 0)
            {
                graphics.MoveTo(pt, c, thickness);
            }
            else
            {
                graphics.LineTo(pt, c, thickness);
            }
        }

        for (int i = 0; i < (num + 1); i++)
        {
            Vector3 pt = point[i] + new Vector3(200, 100);
            Color c = Color.Lerp(Color.black, color, d * i);
            if (i == 0)
            {
                graphics.MoveTo(pt * 0.5f, c, 20);
            }
            else
            {
                graphics.LineTo(pt * 0.5f, c, 20);
            }
        }

        graphics.Render();
	}
}
