using UnityEngine;
using System.Collections;

public class DrawCircleTest : MonoBehaviour {

	public CanvasQuadDrawer drawer;
    private CanvasGraphics graphics;

    public int divid = 12;
    public float radius = 200;
	public bool isUpdate = true;
    
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
        graphics.DrawCircle(radius, 0,0, divid, Color.red);
        graphics.Render();
    }

}
