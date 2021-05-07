using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadraticCurveToTest : MonoBehaviour
{
    private CanvasGraphics graphics;
    public CanvasQuadDrawer drawer;

    [SerializeField] Vector3[] points = null;
    [SerializeField] Color color = Color.red;
    [SerializeField] float thickness = 10;

    // Start is called before the first frame update
    void Start()
    {
        graphics = new CanvasGraphics();
        graphics.verticesDrawer = drawer;
        graphics.curveSmoothing = true;
        graphics.smoothDotLimit = -1f;
    }

    // Update is called once per frame
    void Update()
    {
        Draw();
    }

    void Draw() {
        graphics.Clear();

        Vector3 old = Vector3.zero;
        Vector3 currentMid = Vector3.zero;
        Vector3 oldMid = Vector3.zero;
        for (int i = 0; i < points.Length; i++)
        {
            Vector3 pt = points[i];
            if (i == 0)
            {
                currentMid = pt;
                old = currentMid;
                oldMid = pt;
                graphics.MoveTo(currentMid, color, thickness);
            }
            else
            {
                currentMid = getMidInputCoords(old, pt);
            }

            graphics.QuadraticCurveTo(oldMid, old, currentMid, color, thickness);
            old = pt;
            oldMid = currentMid;
        }
        graphics.LineTo(points[points.Length-1], color, thickness);
        //
        Color _color = Color.green;
        for (int i = 0; i < points.Length; i++)
        {
            Vector3 pt = points[i];
            if (i == 0)
            {
                graphics.MoveTo(pt, _color, thickness*0.25f);
            }
            else
            {
                graphics.LineTo(pt, _color, thickness * 0.25f);
            }
        }
        graphics.Render();

    }

    Vector3 getMidInputCoords(Vector3 old, Vector3 current)
    {
        //return new Vector3(
        //    (old.x + current.x) >> 1,
        //    (old.y + current.y) >> 1,
        //    0);
        return (old + current) / 2;
    }
}
