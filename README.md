# uGUICanvasTools
uGUI用のtoolもろもろ

unity 5.1.2f1

###WorldToCanvasProt.cs
ワールド座標のtransformからCanvasへプロットする。

###WorldToCanvasLine.cs
ワールド座標の２つのtransformを繋ぐラインを描画

###CanvasLine.cs
Canvas中の２つのRectTransformを繋ぐラインを描画

上記は、`[ExecuteInEditMode]`でエディタ上でも動作

###VerticesDrawer.cs
Canvasにラインなどを描画するためのDrawer  

`public void DrawLine(Vector2 from, Vector2 to, float thickness)`  
Debug.DrawLineのような感じで、drawer.DrawLine(from, to, thickness)で描画する。 

`public void DrawQuad(Rect quadRect, float x, float y, float scaleX, float scaleY, float angle, Color color)`  
quadRectで指定した形（主にピボットの調整につかう）を指定位置[x, y]、サイズ[scaleX, scaleY]、角度、色で描画

サンプル: canvasVerticesDrawer.unity  
AnimationCurveの内容をCanvasへ描画している。


参考: http://forum.unity3d.com/threads/custom-mesh-rendering-under-ui-canvas.265700/
