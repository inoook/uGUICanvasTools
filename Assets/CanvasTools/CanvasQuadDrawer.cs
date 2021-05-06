using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// http://forum.unity3d.com/threads/custom-mesh-rendering-under-ui-canvas.265700/
/// <summary>
/// OnPopulateMesh を使用して描画
/// Canvas quad drawer.
/// </summary>
[RequireComponent (typeof (CanvasRenderer))]
public class CanvasQuadDrawer : MaskableGraphic
{
	public Vector3 defaultFaceDir = -Vector3.forward;

	int vertexCount = 0;
	public int bufferQuadCount = 10000;

    // https://cfm-art.sakura.ne.jp/sys/archives/319
    /// <summary>
    /// メッシュに設定したいテクスチャをここで指定
    /// </summary>
    [SerializeField] public Texture2D texture_;

    /// <summary>
    /// メッシュに設定するテクスチャの指定
    /// </summary>
    public override Texture mainTexture {
        get {
            // ここで設定したいテクスチャを返すようにする
            return texture_;
        }
    }


    protected override void OnPopulateMesh(VertexHelper vh) {
        Init();
        vh.Clear(); // これが無いと正常に描画されない。

        // 頂点の順番
        for (int i = 0; i < triangleList.Count/3; i++) {
            int t0 = triangleList[i * 3 + 0];
            int t1 = triangleList[i * 3 + 1];
            int t2 = triangleList[i * 3 + 2];
            vh.AddTriangle(t0, t1, t2);
        }

        // UIVertex:各頂点の情報
        for (int i = 0; i < pointList.Count; i++) {
            var v = new UIVertex();
            v.position = pointList[i];
            v.uv0 = uvList[i];
            v.normal = normalList[i];
            v.color = colorList[i] * this.color;

            vh.AddVert(v);
        }
    }

    protected override void OnDestroy() {
        this.canvasRenderer.Clear();
    }

    void ClearData()
    {
        // clear
        pointList.Clear();
        uvList.Clear();
        normalList.Clear();
        colorList.Clear();
        triangleList.Clear();
    }

	private bool isInit = false;
	public void Init()
	{
		if (!isInit) {
			SetBufferQuadNum (bufferQuadCount);
            
            isInit = true;
        }
	}
    
    List<Vector3> pointList = new List<Vector3>();
    List<Vector3> normalList = new List<Vector3>();
    List<Color> colorList = new List<Color>();
    List<Vector2> uvList = new List<Vector2>();
    List<int> triangleList = new List<int>();

	public void SetBufferQuadNum(int num)
	{
		if(num > 16383){
			Debug.LogWarning("max quad cout = 16383");
		}
		num = Mathf.Clamp(num, 0, 16383);
		bufferQuadCount = num;
	}

	public void Clear()
	{
		vertexCount = 0;

        ClearData();
	}

	// quad
	public void DrawQuad(Rect quadRect, float x, float y, float z, float scaleX, float scaleY, float angle, Color color)
	{
		AddQuadVertex(quadRect, x, y, z, scaleX, scaleY, angle, color);
	}
	public void DrawQuad(Rect quadRect, float x, float y, float z, float scaleX, float scaleY, float angle)
	{
		DrawQuad(quadRect, x, y, z, scaleX, scaleY, angle, Color.white);
	}

	// line
    public void DrawLine(Vector3 from, Vector3 to, float thickness, Color colorFrom, Color colorTo, Vector3 faceDir)
    {
        AddDrawLineVertex(from, to, thickness, colorFrom, colorTo, faceDir);
    }
    public void DrawLine(Vector3 from, Vector3 to, float thickness, Color colorFrom, Color colorTo)
    {
        AddDrawLineVertex(from, to, thickness, colorFrom, colorTo, defaultFaceDir);
    }
	public void DrawLine(Vector3 from, Vector3 to, float thickness, Color color, Vector3 faceDir)
	{
        AddDrawLineVertex(from, to, thickness, color, color, faceDir);
	}
    public void DrawLine(Vector3 from, Vector3 to, float thickness, Color color)
    {
        AddDrawLineVertex(from, to, thickness, color, color, defaultFaceDir);
    }
	public void DrawLine(Vector3 from, Vector3 to, float thickness)
	{
        AddDrawLineVertex(from, to, thickness, Color.white, Color.white, defaultFaceDir);
	}

    // lineTo
    public void DrawLineTo(Vector3 from, Vector3 dir, float thickness, Color color)
    {
        AddDrawLineVertexTo(from, dir, thickness, color, defaultFaceDir);
    }

    public void DrawLineTo(Vector3 from, Vector3 pre_dir, Vector3 dir, float thickness, Color color)
    {
        AddDrawLineVertexToAvg(from, pre_dir, dir, thickness, color, defaultFaceDir);
    }
    //
	/// <summary>
	/// Render this instance.
	/// </summary>
	/// <returns>The draw quad count.</returns>
	public int Render()
	{
        this.UpdateGeometry();

		return vertexCount / 4;
	}

    public void SetVertexList(List<Vector3> pointList_, List<Vector2> uvList_, List<Vector3> normalList_, List<Color> colorList_, List<int> triangleList_){
        vertexCount = pointList_.Count;

        pointList = pointList_;
        uvList = uvList_;
        normalList = normalList_;
        colorList = colorList_;
        triangleList = triangleList_;
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

	private Vector3 CrossUnitVectorToDir(Vector3 forwardDir, Vector3 lookDir)
	{
		Vector3 v = Vector3.Cross(lookDir.normalized, forwardDir.normalized);
		v.Normalize();
		return v;
	}

    public void AddDrawLineVertex(Vector3 p0, Vector3 p1, float thickness, Color color0, Color color1, Vector3 faceDir)
    {
        //float length = Vector3.Distance(p0, p1);
        Vector3 dir = p1 - p0;

        Vector3 cross = CrossUnitVectorToDir(dir, faceDir) * thickness * 0.5f;

        //AddLineQuadVertex(x, y, z, length, thickness, angle, color);

        if (bufferQuadCount * 4 < vertexCount + 4) { return; }

        Vector3 a = p0 + cross;
        Vector3 b = p0 - cross;
        Vector3 c = p1 + cross;
        Vector3 d = p1 - cross;

        // 0
        pointList.Add(a);
        uvList.Add(UV_A);
        normalList.Add(NORMAL);
        colorList.Add(color0);

        // 1
        pointList.Add(b);
        uvList.Add(UV_B);
        normalList.Add(NORMAL);
        colorList.Add(color0);

        // 2
        pointList.Add(c);
        uvList.Add(UV_C);
        normalList.Add(NORMAL);
        colorList.Add(color1);

        // 3
        pointList.Add(d);
        uvList.Add(UV_D);
        normalList.Add(NORMAL);
        colorList.Add(color1);

        int index = vertexCount;
        triangleList.Add(index + 0);
        triangleList.Add(index + 1);
        triangleList.Add(index + 2);

        triangleList.Add(index + 2);
        triangleList.Add(index + 1);
        triangleList.Add(index + 3);

        vertexCount += 4;
    }

    public void AddDrawLineVertexTo(Vector3 p1, Vector3 dir, float thickness, Color color, Vector3 faceDir)
    {
        //if (bufferQuadCount * 4 < vertexCount + 4) { return; }

        Vector3 cross = CrossUnitVectorToDir(dir, faceDir) * thickness * 0.5f;

        Vector3 c = p1 + cross;
        Vector3 d = p1 - cross;

        // TODO: 頂点を少なくするためには今の方法が良いが、textureを貼ることを考えると2点の追加だと表示に問題があるので、4点に変更したほうが良いか？
        pointList.Add(c);
        uvList.Add(UV_C);
        normalList.Add(NORMAL);
        colorList.Add(color);
        
        pointList.Add(d);
        uvList.Add(UV_D);
        normalList.Add(NORMAL);
        colorList.Add(color);

        int index = vertexCount;
        
        triangleList.Add(index - 2);
        triangleList.Add(index - 1);
        triangleList.Add(index);

        triangleList.Add(index);
        triangleList.Add(index - 1);
        triangleList.Add(index + 1);

        vertexCount += 2;
    }

    public void AddDrawLineVertexToAvg(Vector3 p1, Vector3 pre_dir, Vector3 dir, float thickness, Color color, Vector3 faceDir)
    {
        //if (bufferQuadCount * 4 < vertexCount + 4) { return; }

        Vector3 cross_pre = CrossUnitVectorToDir(pre_dir, faceDir);
        Vector3 cross_current = CrossUnitVectorToDir(dir, faceDir);

        Vector3 cross = (cross_pre + cross_current) * 0.5f * thickness * 0.5f;
        Vector3 c = p1 + cross;
        Vector3 d = p1 - cross;

        pointList.Add(c);
        uvList.Add(UV_C);
        normalList.Add(NORMAL);
        colorList.Add(color);

        pointList.Add(d);
        uvList.Add(UV_D);
        normalList.Add(NORMAL);
        colorList.Add(color);

        int index = vertexCount;

        triangleList.Add(index - 2);
        triangleList.Add(index - 1);
        triangleList.Add(index);

        triangleList.Add(index);
        triangleList.Add(index - 1);
        triangleList.Add(index + 1);

        vertexCount += 2;
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
	public void AddLineQuadVertex(float x, float y, float z, float length, float thickness, float angle, Color color)
	{
		AddQuadVertex(LINEQUAD, x, y, z, length, thickness, angle, color);
	}
	public void AddLineQuadVertex(float x, float y, float z, float length, float thickness, float angle)
	{
		AddLineQuadVertex(x, y, z, length, thickness, angle, Color.white);
	}
	
	static Vector3 NORMAL = new Vector3(0,0,1);

    static Vector2 UV_C = new Vector2(1, 0);
    static Vector2 UV_B = new Vector2(0, 1);
    static Vector2 UV_A = new Vector2(0, 0);
    static Vector2 UV_D = new Vector2(1, 1);

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
    public void AddQuadVertex(Rect quadRect, float x, float y, float z, float scaleX, float scaleY, float angle, Color color)
	{
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

        // 0
        pointList.Add(a);
        uvList.Add(UV_A);
        normalList.Add(NORMAL);
        colorList.Add(color);

        // 1
        pointList.Add(b);
        uvList.Add(UV_B);
        normalList.Add(NORMAL);
        colorList.Add(color);

        // 2
        pointList.Add(c);
        uvList.Add(UV_C);
        normalList.Add(NORMAL);
        colorList.Add(color);

        // 3
        pointList.Add(d);
        uvList.Add(UV_D);
        normalList.Add(NORMAL);
        colorList.Add(color);

        int index = vertexCount;
        triangleList.Add(index + 0);
        triangleList.Add(index + 1);
        triangleList.Add(index + 2);

        triangleList.Add(index + 2);
        triangleList.Add(index + 1);
        triangleList.Add(index + 3);

        vertexCount += 4;
    }
	public void AddQuadVertex(Rect quadRect, float x, float y, float z, float scaleX, float scaleY, float angle)
	{
		AddQuadVertex(quadRect, x, y, z, scaleX, scaleY, angle, Color.white);
	}

    public void AddVector3Vertex(Vector3[] vertices, float x, float y, float scaleX, float scaleY, float angle, Color color) {
        Vector3 a = vertices[0];
        Vector3 b = vertices[1];
        Vector3 c = vertices[2];
        Vector3 d = vertices[3];

        // transform
        Matrix4x4 mtx = Matrix4x4.TRS(new Vector3(x, y, 0), Quaternion.Euler(new Vector3(0, 0, angle)), new Vector3(scaleX, scaleY, 1));

        a = mtx.MultiplyPoint3x4(a);
        b = mtx.MultiplyPoint3x4(b);
        c = mtx.MultiplyPoint3x4(c);
        d = mtx.MultiplyPoint3x4(d);

        // 0
        pointList.Add(a);
        uvList.Add(UV_A);
        normalList.Add(NORMAL);
        colorList.Add(color);

        // 1
        pointList.Add(b);
        uvList.Add(UV_B);
        normalList.Add(NORMAL);
        colorList.Add(color);

        // 2
        pointList.Add(c);
        uvList.Add(UV_C);
        normalList.Add(NORMAL);
        colorList.Add(color);

        // 3
        pointList.Add(d);
        uvList.Add(UV_D);
        normalList.Add(NORMAL);
        colorList.Add(color);

        int index = vertexCount;
        triangleList.Add(index + 0);
        triangleList.Add(index + 1);
        triangleList.Add(index + 2);

        triangleList.Add(index + 2);
        triangleList.Add(index + 1);
        triangleList.Add(index + 3);

        vertexCount += 4;
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
		//if( innerRadius == 0 && divid % 2 == 0 ){
		//	for(int j = 0; j < divid/2; j++){
		//		int i = j * 2;
		//		Vector3[] points = new Vector3[4];
		//		points[0] = radius * vertices[i];
		//		points[1] = 0 * vertices[i+1];
		//		points[2] = radius * vertices[i+1];
		//		points[3] = radius * vertices[i+2];
				
		//		AddVector3Vertex(points, x, y, scaleX, scaleY, angle, color);
		//	}
		//}else{
			for(int i = 0; i < divid; i++){
				Vector3[] points = new Vector3[4];
				points[0] = radius * vertices[i];
				points[1] = radius * vertices[i+1];
				points[2] = innerRadius * vertices[i];
				points[3] = innerRadius * vertices[i+1];

				AddVector3Vertex(points, x, y, scaleX, scaleY, angle, color);
			}
		//}
	}

	#endregion
}
