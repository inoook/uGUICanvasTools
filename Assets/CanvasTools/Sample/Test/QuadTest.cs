using UnityEngine;
using System.Collections;

/// <summary>
/// Quad を複数描画する 
/// </summary>
public class QuadTest : MonoBehaviour {

	public CanvasQuadDrawer drawer;
    private CanvasGraphics graphics;

    public int numX = 10;
	public int numY = 10;

	public Vector2 offset;

	public float size = 1.0f;

	public bool isUpdate = true;

	public float angle = 0.0f;


	// Use this for initialization
	void Start () {
        graphics = new CanvasGraphics();
        graphics.verticesDrawer = drawer;

        Draw();
	}
	
	// Update is called once per frame
	void Update () {
		if(!isUpdate){ return; }

		Draw();
	}

    void Draw() {
        graphics.Clear();

        // quadTest
        Rect rect = new Rect(-0.5f, -0.5f, 1, 1);

        float offsetX = offset.x * numX / 2.0f;
        float offsetY = offset.y * numY / 2.0f;

        angle += Time.deltaTime * 50;
        float _angle = angle;

        for (int i = 0; i < numX; i++) {
            for (int j = 0; j < numY; j++) {
                float x = i * offset.x - offsetX;
                float y = j * offset.y - offsetY;
                float z = 0;
                _angle += 0.1f;

                float r = 1.0f / numX * i;
                float g = 1.0f / numY * j;
                Color color = new Color(r, g, 0, 1);

                graphics.DrawQuad(rect, x, y, z, size, size, _angle, color);
            }
        }
        graphics.Render();
    }

}
