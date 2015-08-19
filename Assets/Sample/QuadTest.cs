using UnityEngine;
using System.Collections;

public class QuadTest : MonoBehaviour {

	public VerticesDrawer drawer;

	public int numX = 10;
	public int numY = 10;

	public Vector2 offset;

	public float size = 1.0f;

	public bool isUpdate = true;

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
		
		Rect rect = new Rect(-0.5f, -0.5f, 1,1);
		
		float offsetX = offset.x * numX / 2.0f;
		float offsetY = offset.y * numY / 2.0f;
		
		float angle = 0;
		
		for(int i = 0; i < numX; i++){
			for(int j = 0; j < numY; j++){
				float x = i * offset.x - offsetX;
				float y = j * offset.y - offsetY;
				angle += 0.1f;
				
				float r = 1.0f/numX * i;
				float g = 1.0f/numY * j;
				Color color = new Color(r, g, 0, 1);
				
				drawer.DrawQuad(rect, x, y, size, size, angle, color); 
			}
		}
		drawer.Render();
	}
}
