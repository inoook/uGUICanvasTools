using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Graphics
{
	public enum CMD{
		MOVE_TO, LINE_TO
	}
	public List<CMD> cmds;
	public List<Vector3> points;
	public List<Color> colors;
	public List<float> thicknessList;
	
	public CanvasQuadDrawer verticesDrawer;

	private float m_thickness = 30;
	private Color m_color = Color.white;
	
	public bool curveSmoothing = true;
	
	public Graphics()
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
		if(!curveSmoothing){
			for(int i = 0; i < points.Count-1; i++){
				Vector3 pt0 = points[i];
				Vector3 pt1 = points[i+1];
				Color color = colors[i+1];
				float thickness = thicknessList[i];
				if(cmds[i+1] == CMD.MOVE_TO){
					
				}else{
//					Debug.DrawLine(pt0, pt1, color);
					if(verticesDrawer != null){
						verticesDrawer.DrawLine(pt0, pt1, thickness, color);
					}
				}
			}
		}else{
			UpdateLines();
		}
		
		if(verticesDrawer != null){
			verticesDrawer.Render();
		}
	}

	//
	static Vector3 NORMAL = new Vector3(0,0,1); 
	static Vector2 UV_C = new Vector2(1, 1);
	static Vector2 UV_A = new Vector2(0, 0);
	static Vector2 UV_B = new Vector2(0, 1);
	static Vector2 UV_D = new Vector2(1, 0);
	//
	private Vector3 lookDir = Vector3.forward;

	private void UpdateLines(){

		if(points == null || points.Count < 1){ return; }

		List<Vector3> myVertices = new List<Vector3>();
		
		Vector3 lineDir; 
		Vector3 preLineDir = Vector3.zero;
		
		Vector3 crossDir;

		Vector3 fromPos = points[0];
		
		for(int i = 0; i < points.Count; i++){
			CMD cmd = cmds[i];
			float thickness = thicknessList[i];
			
			if(cmd == CMD.MOVE_TO){
				fromPos = points[i];
			}
			
			Vector3 toPos = points[i];

			lineDir = toPos - fromPos;
			
			if(i < cmds.Count-1){
				if(cmds[i+1] == CMD.MOVE_TO){
					lineDir = preLineDir;
				}else{
					Vector3 nextVector3 = points[i+1];
					lineDir = nextVector3 - toPos;
				}
			}else{
				// end
				if(points.Count > 2){
					lineDir = preLineDir;
					//preLineDir = points[i-1] - points[i-2];
				}else{
					lineDir = points[1] - points[0];
				}
			}

			Vector3 vecA, vecB;
			if(cmd == CMD.MOVE_TO){
				// CMD.MOVE_TO
				lineDir = points[i+1] - points[i];
				crossDir = CrossUnitVectorToDir(lineDir, lookDir);
				
				crossDir *= thickness/2;
				
				vecA = new Vector3( fromPos.x + crossDir.x, fromPos.y + crossDir.y, fromPos.z + crossDir.z);
				vecB = new Vector3( fromPos.x - crossDir.x, fromPos.y - crossDir.y, fromPos.z - crossDir.z);
			}else{
				// CMD.LINE_TO
				Vector3 cross0 = CrossUnitVectorToDir(lineDir, lookDir);
				Vector3 cross1 = CrossUnitVectorToDir(preLineDir, lookDir);
				
				crossDir = (cross0 + cross1) * 0.5f;// preLineDir and lineDir avrg
				crossDir.Normalize();
				
				crossDir *= thickness/2;
				
				vecA = new Vector3( toPos.x + crossDir.x, toPos.y + crossDir.y, toPos.z + crossDir.z);
				vecB = new Vector3( toPos.x - crossDir.x, toPos.y - crossDir.y, toPos.z - crossDir.z);
			}

			myVertices.Add( vecA );
			myVertices.Add( vecB );
			
//			Debug.DrawLine(fromPos, toPos, Color.green);//Debug

			fromPos = toPos;
			preLineDir = lineDir;
		}

		//  create UIVertex
		int num = (myVertices.Count/2-1) * 4;
		UIVertex[] uiVertices = new UIVertex[num];
		for(int i = 0; i < myVertices.Count/2-1; i++){
			CMD cmd = cmds[i+1];
			
			if(cmd != CMD.MOVE_TO){
				int index = i * 2;

				Vector3 a = myVertices[index+0];
				Vector3 b = myVertices[index+1];
				Vector3 c = myVertices[index+2];
				Vector3 d = myVertices[index+3];

				Color color0 = colors[i];
				Color color1 = colors[i+1];
				
				UIVertex uiVertex = new UIVertex();
				uiVertex.normal = NORMAL;
//				uiVertex.color = color;

				uiVertex.position = c;
				uiVertex.uv0 = UV_C;
				uiVertex.color = color1;
				uiVertices[i*4+0] = uiVertex;
				
				uiVertex.position = a;
				uiVertex.uv0 = UV_A;
				uiVertex.color = color0;
				uiVertices[i*4+1] = uiVertex;
				
				uiVertex.position = b;
				uiVertex.uv0 = UV_B;
				uiVertex.color = color0;
				uiVertices[i*4+2] = uiVertex;
				
				uiVertex.position = d;
				uiVertex.uv0 = UV_D;
				uiVertex.color = color1;
				uiVertices[i*4+3] = uiVertex;
			}
		}

		verticesDrawer.SetUiVertexList(uiVertices);
	}

	private Vector3 CrossUnitVectorToDir(Vector3 forwardDir, Vector3 lookDir)
	{
		Vector3 v = Vector3.Cross(lookDir, forwardDir.normalized);
		v.Normalize();
		return v;
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

	public void DrawCircle(float radius, float x, float y, int divid, Color color, float innerRadius = 0, float scaleX = 1, float scaleY = 1, float angle = 0)
	{
		if(verticesDrawer != null){
			verticesDrawer.DrawCircle(radius, x, y, divid, color, innerRadius, scaleX, scaleY, angle);
		}
	}
//
//	public void DrawSpline(List<Vector3> points, float[] thicknesses = null)
//	{
//		if(points.Count < 2) return;
//
//		List<Vector3> pts = new List<Vector3>();
//		pts.AddRange(points);
//
//		pts.Insert(0, points[0]);
//		pts.Add(points[points.Count-1]);
//		
//		this.MoveTo(pts[0], m_color, m_thickness);
//
//		int j = 0;
//		for(int i=0; i < pts.Count-3; i++, j++)
//		{
//			Vector3 p0 = pts[i+0];
//			Vector3 p1 = pts[i+1];
//			Vector3 p2 = pts[i+2];
//			Vector3 p3 = pts[i+3];
//			float d = Vector3.Distance(p3, p0) / 100;
//
//			//int numSegments = 5;//曲線分割数（補完する数）
//			int numSegments = (int)(d * 2.0f)+1;
//
//			float fromThickness = m_thickness;
//			float toThickness = m_thickness;
//			if(thicknesses != null){
//				fromThickness = thicknesses[j];
//				toThickness = thicknesses[j+1];
//			}
//			splineTo(p0, p1, p2, p3, numSegments, fromThickness, toThickness);
//		}
//	}

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
