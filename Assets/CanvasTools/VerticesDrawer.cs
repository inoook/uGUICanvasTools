using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// http://forum.unity3d.com/threads/custom-mesh-rendering-under-ui-canvas.265700/
[RequireComponent (typeof (CanvasRenderer))]
public class VerticesDrawer : MonoBehaviour {

	private GameObject gObj;
	private CanvasRenderer pCanvasRenderer;
	private Material mat;

	List<UIVertex> vertexList;
	
	void Awake () {
		vertexList = new List<UIVertex>();

		gObj = this.gameObject;
		mat = new Material(Shader.Find("UI/Default"));
		pCanvasRenderer = gObj.GetComponent<CanvasRenderer>();
	}

	public void Clear()
	{
		if(vertexList != null){
			vertexList.Clear();
		}

		if(pCanvasRenderer != null){
			pCanvasRenderer.Clear();
		}
	}

	// quad
	public void DrawQuad(Rect quadRect, float x, float y, float scaleX, float scaleY, float angle, Color color)
	{
		List<UIVertex> lineList = GetQuadVertex(quadRect, x, y, scaleX, scaleY, angle, color);
		vertexList.AddRange( lineList );
	}
	public void DrawQuad(Rect quadRect, float x, float y, float scaleX, float scaleY, float angle)
	{
		DrawQuad(quadRect, x, y, scaleX, scaleY, angle, Color.white);
	}

	// line
	public void DrawLine(Vector2 p0, Vector2 p1, float thickness, Color color)
	{
		List<UIVertex> lineList = GetDrawLineVertex(p0, p1, thickness, color);
		vertexList.AddRange( lineList );
	}
	public void DrawLine(Vector2 p0, Vector2 p1, float thickness)
	{
		DrawLine(p0, p1, thickness, Color.white);
	}

	public void SetColor(Color c)
	{
		pCanvasRenderer.SetColor(c);
	}

	public void Render()
	{
		Draw(vertexList);
	}

	public void Draw(List<UIVertex> vertexList) {
		pCanvasRenderer.SetMaterial(mat, null);
		pCanvasRenderer.SetVertices(vertexList);
	}

	#region helper
	/// <summary>
	/// Gets the draw line vertex.
	/// </summary>
	/// <returns>The draw line vertex.</returns>
	/// <param name="p0">P0.</param>
	/// <param name="p1">P1.</param>
	/// <param name="thickness">Thickness.</param>
	public List<UIVertex> GetDrawLineVertex(Vector2 p0, Vector2 p1, float thickness, Color color)
	{
		float x = p0.x;
		float y = p0.y;
		float length = Vector2.Distance(p0, p1);
		Vector2 delta = p1 - p0;
		float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

		return GetLineQuadVertex(x, y, length, thickness, angle, color);
	}
	public List<UIVertex> GetDrawLineVertex(Vector2 p0, Vector2 p1, float thickness)
	{
		return GetDrawLineVertex(p0, p1, thickness, Color.white);
	}
	
	static Rect lineQuad = new Rect(0, -0.5f, 1.0f, 1.0f);
	/// <summary>
	/// Gets the line quad vertex.
	/// </summary>
	/// <returns>The line quad vertex.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="length">Length.</param>
	/// <param name="thickness">Thickness.</param>
	/// <param name="angle">Angle.</param>
	public List<UIVertex> GetLineQuadVertex(float x, float y, float length, float thickness, float angle, Color color)
	{
		return GetQuadVertex(lineQuad, x, y, length, thickness, angle, color);
	}
	public List<UIVertex> GetLineQuadVertex(float x, float y, float length, float thickness, float angle)
	{
		return GetLineQuadVertex(x, y, length, thickness, angle, Color.white);
	}

	/// <summary>
	/// Gets the quad vertex.
	/// </summary>
	/// <returns>The quad vertex.</returns>
	/// <param name="quadRect">Quad rect.</param>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="scaleX">Scale x.</param>
	/// <param name="scaleY">Scale y.</param>
	/// <param name="angle">Angle.</param>
	public List<UIVertex> GetQuadVertex(Rect quadRect, float x, float y, float scaleX, float scaleY, float angle, Color color)
	{
		List<UIVertex> pVertexList = new List<UIVertex>();
		
		Vector3 a = new Vector3(quadRect.xMin, quadRect.yMax, 0);
		Vector3 b = new Vector3(quadRect.xMax, quadRect.yMax, 0);
		Vector3 c = new Vector3(quadRect.xMin, quadRect.yMin, 0);
		Vector3 d = new Vector3(quadRect.xMax, quadRect.yMin, 0);
		
		// transform
		Matrix4x4 mtx = Matrix4x4.TRS(new Vector3(x, y, 0), Quaternion.Euler(new Vector3(0,0,angle)), new Vector3(scaleX, scaleY, 1));
		a = mtx.MultiplyPoint(a);
		b = mtx.MultiplyPoint(b);
		c = mtx.MultiplyPoint(c);
		d = mtx.MultiplyPoint(d);
		
		// add UIVertex
		UIVertex uiVertex = new UIVertex();
		
		Vector3 normal = new Vector3(0,0,1);
		uiVertex.normal = normal;
		uiVertex.color = color;
		//pUIVertex.tangent = tangent;
		
		uiVertex.position = c;
		uiVertex.uv0 = new Vector2(1, 1);
		pVertexList.Add(uiVertex);
		
		uiVertex.position = a;
		uiVertex.uv0 = new Vector2(0, 0);
		pVertexList.Add(uiVertex);
		
		uiVertex.position = b;
		uiVertex.uv0 = new Vector2(0, 1);
		pVertexList.Add(uiVertex);
		
		uiVertex.position = d;
		uiVertex.uv0 = new Vector2(1, 0);
		pVertexList.Add(uiVertex);
		
		return pVertexList;
	}
	public List<UIVertex> GetQuadVertex(Rect quadRect, float x, float y, float scaleX, float scaleY, float angle)
	{
		return GetQuadVertex(quadRect, x, y, scaleX, scaleY, angle, Color.white);
	}
	#endregion
}
