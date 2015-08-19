using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// http://forum.unity3d.com/threads/custom-mesh-rendering-under-ui-canvas.265700/
public class RenderMeshOnCanvas : MonoBehaviour {
	
	public Material material;
	public GameObject meshGObj;
	
	// Use this for initialization
	void Start () {
		
		Mesh mesh = meshGObj.GetComponent<MeshFilter>().sharedMesh;

		if(material == null){
			material = new Material(Shader.Find("UI/Default"));
			Debug.LogWarning("createMat");
		}
		
		CanvasRenderer canvasRenderer = GetComponent<CanvasRenderer>();  
		
		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.triangles;
		Vector3[] normals = mesh.normals;
		Vector4[] tangents = mesh.tangents;
		
		Vector2[] UVs = mesh.uv;
		
		List<UIVertex> uiVertices = new List<UIVertex>();
		
		for (int i = 0; i < triangles.Length; ++i){
			UIVertex temp = new UIVertex();
			temp.position = vertices[triangles[i]];
			temp.uv0 = UVs[triangles[i]];  
			temp.normal = normals[triangles[i]];
			temp.tangent = tangents[triangles[i]];
			temp.color = Color.white;
			uiVertices.Add (temp);
			if (i%3 == 0)
				uiVertices.Add (temp);
		}
		
		canvasRenderer.Clear ();      
		canvasRenderer.SetMaterial(material, null);
		canvasRenderer.SetVertices(uiVertices);
	}
}