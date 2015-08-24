using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// http://forum.unity3d.com/threads/custom-mesh-rendering-under-ui-canvas.265700/
[RequireComponent (typeof (CanvasRenderer))]
public class CanvasQuadDrawer : MonoBehaviour {

	private GameObject gObj;
	private CanvasRenderer canvasRenderer;
	private Material mat;

	UIVertex[] uiVertexList;

	int vertexCount = 0;
	public int bufferQuadCount = 10000;
	
	void Awake () {

		SetBufferQuadNum(bufferQuadCount);
//		uiVertexList = new UIVertex[bufferQuadCount*4];

		gObj = this.gameObject;
		mat = new Material(Shader.Find("UI/Default"));
		canvasRenderer = gObj.GetComponent<CanvasRenderer>();
	}

	public void SetBufferQuadNum(int num)
	{
		if(num > 16383){
			Debug.LogWarning("max quad cout = 16383");
		}
		num = Mathf.Clamp(num, 0, 16383);
		bufferQuadCount = num;

		uiVertexList = new UIVertex[bufferQuadCount*4];
	}

	public void Clear()
	{
		vertexCount = 0;

		if(canvasRenderer != null){
			canvasRenderer.Clear();
		}
	}

	// quad
	public void DrawQuad(Rect quadRect, float x, float y, float scaleX, float scaleY, float angle, Color color)
	{
		AddQuadVertex(quadRect, x, y, scaleX, scaleY, angle, color);
	}
	public void DrawQuad(Rect quadRect, float x, float y, float scaleX, float scaleY, float angle)
	{
		DrawQuad(quadRect, x, y, scaleX, scaleY, angle, Color.white);
	}

	// line
	public void DrawLine(Vector2 from, Vector2 to, float thickness, Color color)
	{
		AddDrawLineVertex(from, to, thickness, color);
	}
	public void DrawLine(Vector2 from, Vector2 to, float thickness)
	{
		DrawLine(from, to, thickness, Color.white);
	}

	public void SetColor(Color c)
	{
		canvasRenderer.SetColor(c);
	}

	/// <summary>
	/// Render this instance.
	/// </summary>
	/// <returns>The draw quad count.</returns>
	public int Render()
	{
//		Debug.Log(uiVertexList.Length);
		canvasRenderer.SetMaterial(mat, null);
		canvasRenderer.SetVertices(uiVertexList, vertexCount);
		
		return vertexCount / 4;
	}
	public void SetUiVertexList(UIVertex[] vertices){
		vertexCount = vertices.Length;
		//uiVertexList = vertices;
		vertices.CopyTo(uiVertexList, 0);
	}

	/// <summary>
	/// Gets the draw quad count.
	/// </summary>
	/// <returns>The draw quad count.</returns>
	public int GetDrawQuadCount()
	{
		return vertexCount / 4;
	}

	#region helper
	/// <summary>
	/// Adds the draw line vertex.
	/// </summary>
	/// <param name="p0">P0.</param>
	/// <param name="p1">P1.</param>
	/// <param name="thickness">Thickness.</param>
	/// <param name="color">Color.</param>
	public void AddDrawLineVertex(Vector2 p0, Vector2 p1, float thickness, Color color)
	{
		float x = p0.x;
		float y = p0.y;
		float length = Vector2.Distance(p0, p1);
		Vector2 delta = p1 - p0;
		float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

		AddLineQuadVertex(x, y, length, thickness, angle, color);
	}
	public void AddDrawLineVertex(Vector2 p0, Vector2 p1, float thickness)
	{
		AddDrawLineVertex(p0, p1, thickness, Color.white);
	}
	
	static Rect LINEQUAD = new Rect(0, -0.5f, 1.0f, 1.0f);
	/// <summary>
	/// Adds the line quad vertex.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="length">Length.</param>
	/// <param name="thickness">Thickness.</param>
	/// <param name="angle">Angle.</param>
	/// <param name="color">Color.</param>
	public void AddLineQuadVertex(float x, float y, float length, float thickness, float angle, Color color)
	{
		AddQuadVertex(LINEQUAD, x, y, length, thickness, angle, color);
	}
	public void AddLineQuadVertex(float x, float y, float length, float thickness, float angle)
	{
		AddLineQuadVertex(x, y, length, thickness, angle, Color.white);
	}
	
	static Vector3 NORMAL = new Vector3(0,0,1); 
	static Vector2 UV_C = new Vector2(1, 1);
	static Vector2 UV_A = new Vector2(0, 0);
	static Vector2 UV_B = new Vector2(0, 1);
	static Vector2 UV_D = new Vector2(1, 0);
	/// <summary>
	/// Adds the quad vertex.
	/// </summary>
	/// <param name="quadRect">Quad rect.</param>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="scaleX">Scale x.</param>
	/// <param name="scaleY">Scale y.</param>
	/// <param name="angle">Angle.</param>
	/// <param name="color">Color.</param>
	public void AddQuadVertex(Rect quadRect, float x, float y, float scaleX, float scaleY, float angle, Color color)
	{
		if(bufferQuadCount * 4 < vertexCount+4){ return; }

		Vector3 a = new Vector3(quadRect.xMin, quadRect.yMax, 0);
		Vector3 b = new Vector3(quadRect.xMax, quadRect.yMax, 0);
		Vector3 c = new Vector3(quadRect.xMin, quadRect.yMin, 0);
		Vector3 d = new Vector3(quadRect.xMax, quadRect.yMin, 0);
		
		// transform
		Matrix4x4 mtx = Matrix4x4.TRS(new Vector3(x, y, 0), Quaternion.Euler(new Vector3(0,0,angle)), new Vector3(scaleX, scaleY, 1));

		a = mtx.MultiplyPoint3x4(a);
		b = mtx.MultiplyPoint3x4(b);
		c = mtx.MultiplyPoint3x4(c);
		d = mtx.MultiplyPoint3x4(d);
		
		// add UIVertex
		UIVertex uiVertex = new UIVertex();

		uiVertex.normal = NORMAL;
		uiVertex.color = color;
		//pUIVertex.tangent = tangent;
		
		uiVertex.position = c;
		uiVertex.uv0 = UV_C;
		uiVertexList[vertexCount] = uiVertex;
		vertexCount ++;
		
		uiVertex.position = a;
		uiVertex.uv0 = UV_A;
		uiVertexList[vertexCount] = uiVertex;
		vertexCount ++;
		
		uiVertex.position = b;
		uiVertex.uv0 = UV_B;
		uiVertexList[vertexCount] = uiVertex;
		vertexCount ++;
		
		uiVertex.position = d;
		uiVertex.uv0 = UV_D;
		uiVertexList[vertexCount] = uiVertex;
		vertexCount ++;
	}
	public void AddQuadVertex(Rect quadRect, float x, float y, float scaleX, float scaleY, float angle)
	{
		AddQuadVertex(quadRect, x, y, scaleX, scaleY, angle, Color.white);
	}

	public void AddVector3Vertex(Vector3[] vertices, float x, float y, float scaleX, float scaleY, float angle, Color color)
	{
		if(bufferQuadCount * 4 < vertexCount+4){ return; }
		
		Vector3 a = vertices[0];
		Vector3 b = vertices[1];
		Vector3 c = vertices[2];
		Vector3 d = vertices[3];
		
		// transform
		Matrix4x4 mtx = Matrix4x4.TRS(new Vector3(x, y, 0), Quaternion.Euler(new Vector3(0,0,angle)), new Vector3(scaleX, scaleY, 1));
		
		a = mtx.MultiplyPoint3x4(a);
		b = mtx.MultiplyPoint3x4(b);
		c = mtx.MultiplyPoint3x4(c);
		d = mtx.MultiplyPoint3x4(d);
		
		// add UIVertex
		UIVertex uiVertex = new UIVertex();
		
		uiVertex.normal = NORMAL;
		uiVertex.color = color;
		//pUIVertex.tangent = tangent;
		
		uiVertex.position = c;
		uiVertex.uv0 = UV_C;
		uiVertexList[vertexCount] = uiVertex;
		vertexCount ++;
		
		uiVertex.position = a;
		uiVertex.uv0 = UV_A;
		uiVertexList[vertexCount] = uiVertex;
		vertexCount ++;
		
		uiVertex.position = b;
		uiVertex.uv0 = UV_B;
		uiVertexList[vertexCount] = uiVertex;
		vertexCount ++;
		
		uiVertex.position = d;
		uiVertex.uv0 = UV_D;
		uiVertexList[vertexCount] = uiVertex;
		vertexCount ++;
	}

	public void DrawCircle(float radius, float x, float y, int divid, Color color, float innerRadius = 0, float scaleX = 1, float scaleY = 1, float angle = 0)
	{
		divid = (divid <= 1) ? 2 : divid;
		float d = Mathf.PI * 2 / divid;
		
		Vector3[] vertices = new Vector3[divid+1+1];
		for (int i = 0; i < (divid+1+1); i++) {
			float vx = Mathf.Sin (d * i);
			float vz = Mathf.Cos (d * i);
			Vector3 pt = new Vector3 (vx, vz, 0);
			vertices[i] = pt;
		}

		//
		if( innerRadius == 0 && divid % 2 == 0 ){
			for(int j = 0; j < divid/2; j++){
				int i = j * 2;
				Vector3[] points = new Vector3[4];
				points[0] = radius * vertices[i];
				points[1] = 0 * vertices[i+1];
				points[2] = radius * vertices[i+1];
				points[3] = radius * vertices[i+2];
				
				AddVector3Vertex(points, x, y, scaleX, scaleY, angle, color);
			}
		}else{
			for(int i = 0; i < divid; i++){
				Vector3[] points = new Vector3[4];
				points[0] = radius * vertices[i];
				points[1] = radius * vertices[i+1];
				points[2] = innerRadius * vertices[i];
				points[3] = innerRadius * vertices[i+1];

				AddVector3Vertex(points, x, y, scaleX, scaleY, angle, color);
			}
		}
	}

	//
	private Vector3 preVertex0;
	private Vector3 preVertex1;
	public void AddQuadLine(float x, float y, float scaleX, float scaleY, float angle, Color color, bool isStartPoint = false)
	{
		if(bufferQuadCount * 4 < vertexCount+4){ return; }

		Rect quadRect = LINEQUAD;

//		Vector3 a_ = new Vector3(quadRect.xMin, quadRect.yMax, 0);
		Vector3 b = new Vector3(quadRect.xMin, quadRect.yMax, 0);
//		Vector3 c_ = new Vector3(quadRect.xMin, quadRect.yMin, 0);
		Vector3 d = new Vector3(quadRect.xMin, quadRect.yMin, 0);
		
		// transform
		Matrix4x4 mtx = Matrix4x4.TRS(new Vector3(x, y, 0), Quaternion.Euler(new Vector3(0,0,angle)), new Vector3(scaleX, scaleY, 1));

		Vector3 a = preVertex0;
		Vector3 c = preVertex1;

//		a = mtx.MultiplyPoint3x4(a);
		b = mtx.MultiplyPoint3x4(b);
//		c = mtx.MultiplyPoint3x4(c);
		d = mtx.MultiplyPoint3x4(d);

		if(!isStartPoint){
			// add UIVertex
			UIVertex uiVertex = new UIVertex();
			
			uiVertex.normal = NORMAL;
			uiVertex.color = color;
			
			uiVertex.position = c;
			uiVertex.uv0 = UV_C;
			uiVertexList[vertexCount] = uiVertex;
			vertexCount ++;
			
			uiVertex.position = a;
			uiVertex.uv0 = UV_A;
			uiVertexList[vertexCount] = uiVertex;
			vertexCount ++;
			
			uiVertex.position = b;
			uiVertex.uv0 = UV_B;
			uiVertexList[vertexCount] = uiVertex;
			vertexCount ++;
			
			uiVertex.position = d;
			uiVertex.uv0 = UV_D;
			uiVertexList[vertexCount] = uiVertex;
			vertexCount ++;
		}
		//
		preVertex0 = b;
		preVertex1 = d;
	}
	#endregion
}
