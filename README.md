# uGUICanvasTools
uGUI用のtoolもろもろ

//TODO: 線を太くしたときの見え方を綺麗に

unity 2019.4.12f1

`public void DrawLine(Vector2 from, Vector2 to, float thickness)`  
Debug.DrawLineのような感じで、drawer.DrawLine(from, to, thickness)で描画する。 

`public void DrawQuad(Rect quadRect, float x, float y, float scaleX, float scaleY, float angle, Color color)`  
quadRectで指定した形（主にピボットの調整につかう）を指定位置[x, y]、サイズ[scaleX, scaleY]、角度、色で描画

## サンプル
- canvasVerticesDrawer.unity  
AnimationCurveの内容をCanvasへ描画している。

- canvasDrawing.unity  
マウスでのドローイング

参考: http://forum.unity3d.com/threads/custom-mesh-rendering-under-ui-canvas.265700/
