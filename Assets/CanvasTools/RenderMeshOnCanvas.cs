using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// http://forum.unity3d.com/threads/custom-mesh-rendering-under-ui-canvas.265700/
public class RenderMeshOnCanvas : MonoBehaviour {
	
	public Material material;
	public GameObject meshGObj;

    void Start() {
        Canvas.willRenderCanvases += Canvas_willRenderCanvases;
    }

    private void OnDestroy() {
        Canvas.willRenderCanvases -= Canvas_willRenderCanvases;
    }

    void Canvas_willRenderCanvases() {
		// canvas上にmeshGObjで指定したMeshを描画する。

		Mesh mesh = meshGObj.GetComponent<MeshFilter>().sharedMesh;

		if(material == null){
			material = Canvas.GetDefaultCanvasMaterial();
			Debug.LogWarning("createMat");
		}

        CanvasRenderer canvasRenderer = GetComponent<CanvasRenderer>();

        canvasRenderer.Clear();
        canvasRenderer.SetMesh(mesh);
        canvasRenderer.SetMaterial(material, null);
    }

}