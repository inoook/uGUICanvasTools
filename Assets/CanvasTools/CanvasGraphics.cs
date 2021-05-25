using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CanvasGraphics
{
	public enum CMD{
		MOVE_TO, LINE_TO, CURVE_TO
	}
	public List<CMD> cmds;
	public List<Vector3> points;
	public List<Color> colors;
	public List<float> thicknessList;
	
	public CanvasQuadDrawer verticesDrawer;

	private float m_thickness = 30;
	private Color m_color = Color.white;
	
	public bool curveSmoothing = true;
    public float smoothDotLimit = 0.5f; // dotがこの値以上のときはスムージングする。-1 〜 1。　-1 ならば常にスムージング / 1 ならばスムージング無し
    public bool isUseAvg = false; // スムージング時に前のcrossとの平均を使うか

    public CanvasGraphics()
	{
		cmds = new List<CMD>();
		points = new List<Vector3>();
		colors = new List<Color>();
		thicknessList = new List<float>();
	}

	public void SetDefaultColor(Color c)
	{
		m_color = c;
	}
	public void SetDefaultThickness(float thickness)
	{
		m_thickness = thickness;
	}
	public void SetDefaultFaceDir(Vector3 faceDir)
	{
		verticesDrawer.defaultFaceDir = faceDir;
	}

	public void MoveTo(float x, float y)
	{
		MoveTo(new Vector3(x, y ,0), m_color, m_thickness);
	}
	public void MoveTo(float x, float y, float z, Color c, float thickness = 1.0f )
	{
		MoveTo(new Vector3(x, y, z), c, thickness);
	}
	public void MoveTo(Vector3 pt)
	{
		MoveTo(pt, m_color);
	}
	public void MoveTo(Vector3 pt, Color c, float thickness = 1.0f)
	{
		cmds.Add(CMD.MOVE_TO);
		points.Add(pt);
		thicknessList.Add(thickness);

		colors.Add(c);
	}

	public void LineTo(float x, float y)
	{
		LineTo(new Vector3(x, y ,0), m_color, m_thickness);
	}
	public void LineTo(float x, float y, float z, Color color, float thickness = 1.0f)
	{
		LineTo(new Vector3(x, y ,z), color, thickness);
	}
	public void LineTo(Vector3 pt, float thickness)
	{
		LineTo(pt, m_color, thickness);
	}
	public void LineTo(Vector3 pt)
	{
		LineTo(pt, m_color);
	}
	public void LineTo(Vector3 pt, Color c, float thickness = 1.0f)
	{
		cmds.Add(CMD.LINE_TO);
		points.Add(pt);
		thicknessList.Add(thickness);

		colors.Add(c);
	}

	// https://scrapbox.io/oculusgogo/LineRenderer%E3%82%92%E3%83%99%E3%82%B8%E3%82%A7%E6%9B%B2%E7%B7%9A%E3%81%A7%E6%8F%8F%E7%94%BB%E3%81%99%E3%82%8B
	public void QuadraticCurveTo(Vector3 cp, Vector3 end, Color c, float thickness = 1.0f)
	{
		Vector3 start = points[points.Count - 1];

		for (int i = 0; i < 10; i++)
		{
			float t = (1 / 10f) * (i+1);
			Vector3 Q0 = Vector3.Lerp(start, cp, t);
			Vector3 Q1 = Vector3.Lerp(cp, end, t);
			// Interpolate along line S2: Q1 - Q0
			Vector3 pt = Vector3.Lerp(Q0, Q1, t);

			LineTo(pt, c, thickness);
		}
	}

	public void QuadraticCurveTo(Vector3 start, Vector3 cp, Vector3 end, Color c, float thickness = 1.0f)
	{
		// TODO: 分割数を距離に応じて可変にする
		for (int i = 0; i < 10; i++)
		{
			float t = (1 / 10f) * (i + 1);
			Vector3 Q0 = Vector3.Lerp(start, cp, t);
			Vector3 Q1 = Vector3.Lerp(cp, end, t);
			// Interpolate along line S2: Q1 - Q0
			Vector3 pt = Vector3.Lerp(Q0, Q1, t);

			LineTo(pt, c, thickness);
		}
	}

	public void Clear()
	{
		cmds.Clear();
		points.Clear();
		thicknessList.Clear();
		colors.Clear();
		
		if(verticesDrawer != null){
			verticesDrawer.Clear();
		}
	}

	public void Render()
	{
		//if (points.Count < 3) {
		if (points.Count < 2) {
			if (verticesDrawer != null) {
                verticesDrawer.Render();
            }
            return;
		}

		if(!curveSmoothing){
			SimpleLinesRender();
		}
		else{
			SmoothingLinesRender();
		}
		
		if(verticesDrawer != null){
			verticesDrawer.Render();
		}
	}

	private void SimpleLinesRender()
    {
		for (int i = 0; i < points.Count - 1; i++)
		{
			Vector3 pt0 = points[i];
			Vector3 pt1 = points[i + 1];
			Color color0 = colors[i];
			Color color1 = colors[i + 1];
			float thickness = thicknessList[i];
			if (cmds[i + 1] == CMD.MOVE_TO)
			{

			}
			else
			{
				//Debug.DrawLine(pt0, pt1, color);
				if (verticesDrawer != null)
				{
					verticesDrawer.DrawLine(pt0, pt1, thickness, color0, color1);
				}
			}
		}
	}

    private void SmoothingLinesRender()
    {
        if (points == null || points.Count <= 1) { return; }
        
        Vector3 lineDir;
        Vector3 preLineDir = Vector3.zero;

        Vector3 fromPos = points[0];

        for (int i = 0; i < points.Count; i++)
        {
            CMD cmd = cmds[i];
            float thickness = thicknessList[i];

            if (cmd == CMD.MOVE_TO) {
                fromPos = points[i];
            }

            Vector3 toPos = points[i];
            lineDir = toPos - fromPos;
            
            bool isLineEnd = false;// 線の終端

            if (i < cmds.Count - 1) {
                if (cmds[i + 1] == CMD.MOVE_TO)
                {
                    lineDir = preLineDir;
                    isLineEnd = true;
                } else {
                    Vector3 nextVector3 = points[i + 1];
                    lineDir = nextVector3 - toPos;
                }
            } else {
                // end
                if (points.Count > 2)
                {
                    lineDir = preLineDir;
                } else {
                    lineDir = points[1] - points[0];
                }
            }

            // draw
            if (cmd != CMD.MOVE_TO)
            {
                Color color = colors[i];
                Color pre_color = colors[i-1];

                CMD pre_cmd = cmds[i-1];
				if (pre_cmd == CMD.MOVE_TO)
				{
					// CMD.LINE_TO
					verticesDrawer.DrawLine(fromPos, toPos, thickness, color);
                }else{
                    // TODO: もう少し線の繋がりが綺麗になると良いが。
                    int startIndex = i;
                    startIndex = Mathf.Min(i + 1, points.Count - 1);
                    lineDir = points[startIndex] - points[i - 1];

                    float dot = Vector3.Dot(lineDir.normalized, preLineDir.normalized);
                    if (!isLineEnd && dot > smoothDotLimit)
                    {
                        // smoothing
                        //verticesDrawer.DrawLineTo(toPos, lineDir, thickness, Color.green);
                        if (!isUseAvg){
                            verticesDrawer.DrawLineTo(toPos, lineDir, thickness, color);
                        } else {
                            verticesDrawer.DrawLineTo(toPos, preLineDir, lineDir, thickness, color);
                        }
                    } else {
                        verticesDrawer.DrawLine(fromPos, toPos, thickness, pre_color, color);
                    }
                }
            }
            fromPos = toPos;
            preLineDir = lineDir;
        }
    }
    
	// extend
	public void DrawLine(Vector3[] points, float[] thicknessList = null)
	{
		for(int i=0; i < points.Length; i++)
		{
			Vector3 pt = points[i];
			float thickness = m_thickness;
			if(thicknessList != null){
				thickness = thicknessList[i];
			}
			if(i == 0){
				this.MoveTo(pt, m_color, thickness);
			}else{
				this.LineTo(pt, m_color, thickness);
			}
		}
	}

	public void DrawLine(List<Vector3> points)
	{
		for(int i=0; i < points.Count; i++)
		{
			Vector3 pt = points[i];
			if(i == 0){
				this.MoveTo(pt, m_color, m_thickness);
			}else{
				this.LineTo(pt, m_color, m_thickness);
			}
		}
	}

	public void DrawSpline(Vector3[] points, float[] thicknesses = null, Color[] colors = null)
	{
		if(points.Length < 2) return;

		Vector3[] pts = new Vector3[points.Length+2];
		points.CopyTo(pts, 1);

		pts[0] = points[0];
		pts[pts.Length-1] = points[points.Length-1];

		float thickness = m_thickness;
		if(thicknesses != null){
			thickness = thicknesses[0];
		}

		Color color = m_color;
		if(colors != null){
			color = colors[0];
		}

		this.MoveTo(pts[0], color, thickness);
		
		int j = 0;
		for(int i=0; i < pts.Length-3; i++, j++)
		{
			Vector3 p0 = pts[i+0];
			Vector3 p1 = pts[i+1];
			Vector3 p2 = pts[i+2];
			Vector3 p3 = pts[i+3];
			float d = Vector3.Distance(p3, p0) / 100;
			
			//int numSegments = 5;//曲線分割数（補完する数）
			int numSegments = (int)(d * 2.0f)+1;
			
			float fromThickness = m_thickness;
			float toThickness = m_thickness;
			if(thicknesses != null){
				fromThickness = thicknesses[j];
				toThickness = thicknesses[j+1];
			}

			Color fromColor = m_color;
			Color toColor = m_color;
			if(colors != null){
				fromColor = colors[j];
				toColor = colors[j+1];
			}

			splineTo(p0, p1, p2, p3, numSegments, fromThickness, toThickness, fromColor, toColor);
		}
	}

	public void DrawQuadraticCurve(Vector3[] points, float[] thicknesses = null, Color[] colors = null)
    {
		if (points.Length < 2) return;

		Vector3 old = Vector3.zero;
		Vector3 currentMid = Vector3.zero;
		Vector3 oldMid = Vector3.zero;
		Color color;
		float thickness;
		for (int i = 0; i < points.Length; i++)
		{
			color = colors[i];
			thickness = thicknesses[i];
			Vector3 pt = points[i];
			if (i == 0)
			{
				currentMid = pt;
				old = currentMid;
				oldMid = pt;
				MoveTo(currentMid, color, thickness);
			}
			else
			{
				currentMid = getMidInputCoords(old, pt);
			}

			QuadraticCurveTo(oldMid, old, currentMid, color, thickness);
			old = pt;
			oldMid = currentMid;
		}

		int endIndex = points.Length - 1;
		color = colors[endIndex];
		thickness = thicknesses[endIndex];
		LineTo(points[endIndex], color, thickness);
	}

	Vector3 getMidInputCoords(Vector3 old, Vector3 current)
	{
		return (old + current) / 2;
	}


	public void DrawQuad(Rect quadRect, float x, float y, float z, float scaleX, float scaleY, float angle, Color color) {
        verticesDrawer.DrawQuad(quadRect, x, y, z, scaleX, scaleY, angle, color);
    }

    public void DrawCircle(float radius, float x, float y, int divid, Color color, float innerRadius = 0, float scaleX = 1, float scaleY = 1, float angle = 0)
	{
		if(verticesDrawer != null){
			verticesDrawer.DrawCircle(radius, x, y, divid, color, innerRadius, scaleX, scaleY, angle);
		}
	}

    private void splineTo(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int numSegments, float fromThickness, float toThickness, Color fromColor, Color toColor)
	{
		float d = 1.0f / numSegments;
		for(int i=0; i<numSegments; i++)
		{
			float t = d * (i+1);
			Vector3 pos = catmullRom(p0,p1,p2,p3,t);
			float thickness = Mathf.Lerp(fromThickness, toThickness, t);
			Color color = Color.Lerp(fromColor, toColor, t);
			this.LineTo( pos, color, thickness);
		}
	}

	private Vector3 catmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		return 0.5f *
			(
				( -p0 + 3f * p1 - 3f * p2 + p3 ) * ( t * t * t )
				+ ( 2f * p0 - 5f * p1 + 4f * p2 - p3 ) * ( t * t )
				+ ( -p0 + p2 ) * t
				+ 2f * p1
				);
	}

	// Gokit
	Vector3 getPoint(List<Vector3> _nodes, float t )
	{
		int numSections = _nodes.Count - 3;
		int currentNode = Mathf.Min( Mathf.FloorToInt( t * (float)numSections ), numSections - 1 );
		float u = t * (float)numSections - (float)currentNode;
		
		Vector3 a = _nodes[currentNode];
		Vector3 b = _nodes[currentNode + 1];
		Vector3 c = _nodes[currentNode + 2];
		Vector3 d = _nodes[currentNode + 3];
		
		return .5f *
			(
				( -a + 3f * b - 3f * c + d ) * ( u * u * u )
				+ ( 2f * a - 5f * b + 4f * c - d ) * ( u * u )
				+ ( -a + c ) * u
				+ 2f * b
				);
	}
}
