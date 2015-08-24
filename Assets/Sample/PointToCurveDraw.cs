using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// http://wonderfl.net/c/nULD
public class PointToCurveDraw : MonoBehaviour {

	public Camera worldCam;

	Curve curve;

	Graphics graphics;
	public CanvasQuadDrawer verticesDrawer;

	float lastX;
	float lastY;

	public Color drawColor = Color.red;
	public Color guideColor = Color.white;

	// Use this for initialization
	void Start () {
		curve = new Curve();

		graphics = new Graphics();
		graphics.verticesDrawer = verticesDrawer;
	}

	// Update is called once per frame
	void Update () {

		Vector3 mouse = Input.mousePosition;
		mouse.z = 10;
//		Vector3 worldPos = worldCam.ScreenToWorldPoint(mouse);

		RectTransform rectTrans = this.gameObject.GetComponent<RectTransform>();
		Camera canvasCam = this.GetComponentInParent<Canvas>().worldCamera;
		Vector2 worldPos = Vector2.zero;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTrans, mouse, canvasCam, out worldPos);

		float mouseX = worldPos.x;
		float mouseY = worldPos.y;

		if( Input.GetMouseButtonDown(0) ){
			lastX = mouseX;
			lastY = mouseY;
			curve.push(lastX, lastY, Time.time*1000); // 点(x,y,t)を追加
		}
		if( Input.GetMouseButtonUp(0) ){
			curve.drawEnd(graphics); // 線を最後の点まで引く
			curve.reset();
		}

		bool press = Input.GetMouseButton(0);
		
		if (press)
		{
//			graphics.MoveTo(lastX, lastY);
			lastX = mouseX;
			lastY = mouseY;
//			graphics.LineTo(lastX, lastY, 0, guideColor);

			curve.push(lastX, lastY, Time.time*1000); // 点(x,y,t)を追加
			curve.draw(graphics, drawColor); // 線を引く
		}
		
		graphics.Render();
	}
}

class Curve
{
	// (x,y,t) 3点と 初速(u0,v0) から曲線の式を求める
	private float 	_x0, _y0,
					_x1, _y1,
					_x2, _y2;
	private float _t0, _t1, _t2;
	private float _u0, _v0;
	
	// dt = t - t0 として
	// 位置を x(dt) = ax*dt^3 + bx*dt^2 + u0*dt + x0
	// 速度を u(dt) = 3*ax*dt^2 + 2*bx*dt + u0;
	// という関数でフィッティングする
	private float _ax, _bx,
			_ay, _by;
	// 上記係数が求まると次回計算の初速も求まる
	private float _u1, _v1;
	
	// pushされた点の数
	public int _sampleNum;
	
	public Curve() 
	{
		reset();
	}
	
	public void push(float x, float y, float t)
	{
		_x0 = _x1;
		_x1 = _x2;
		_x2 =  x;

		_y0 = _y1;
		_y1 = _y2;
		_y2 =  y;

		_t0 = _t1;
		_t1 = _t2;
		_t2 =  t;

		_sampleNum++;
		
		if (_sampleNum < 3) return; // 3点目までは計算しない
		
		_u0 = _u1;
		_v0 = _v1;
		
		float dx1 = _x1 - _x0;
		float dx2 = _x2 - _x0;

		float dy1 = _y1 - _y0;
		float dy2 = _y2 - _y0;

		float dt1 = _t1 - _t0;
		float dt2 = _t2 - _t0;

		if (dt2 == 0) dt2 = 1.0f;
		if (dt1 == 0) dt1 = 0.5f * dt2;
		float k1 = 1.0f / (dt1 * dt1 * (dt1 - dt2)); 
		float k2 = 1.0f / (dt2 * dt2 * (dt2 - dt1)); 
		
		if (_sampleNum == 3)
		{
			_u0 = 0.0f;//0.5 * dx1 / dt1;
			_v0 = 0.0f;//0.5 * dy1 / dt1;
		}
		
		float p1 = dx1 - _u0 * dt1;
		float p2 = dx2 - _u0 * dt2;
		float q1 = dy1 - _v0 * dt1;
		float q2 = dy2 - _v0 * dt2;
		_ax = p1 * k1 + p2 * k2;
		_ay = q1 * k1 + q2 * k2;
		_bx = - p1 * k1 * dt2 - p2 * k2 * dt1;
		_by = - q1 * k1 * dt2 - q2 * k2 * dt1;
		// 次回の初速を求めておく
		_u1 = 3.0f * _ax * dt1 * dt1 + 2.0f * _bx * dt1 + _u0;
		_v1 = 3.0f * _ay * dt1 * dt1 + 2.0f * _by * dt1 + _v0;
	}
	
	public void draw(Graphics g, Color color)
	{
		if (_sampleNum < 3) return; // 線が引けない
		
		float dx1 = _x1 - _x0;
		float dy1 = _y1 - _y0;

		float dt1 = _t1 - _t0;
		
		// 曲線の分割数 直線距離ピクセル数の半分とした
//		int n = ( (int)(0.5f * Mathf.Sqrt(dx1 * dx1 + dy1 * dy1)) | 0) + 1;
		int n = ( (int)(0.25f * Mathf.Sqrt(dx1 * dx1 + dy1 * dy1)) | 0) + 1;
//		n *= 2;

		g.MoveTo(_x0, _y0, 0, color, 20);
		for (int i = 1; i <= n; i++)
		{
			float tt = dt1 * i / n;
			float tt2 = tt * tt;
			float tt3 = tt2 * tt;
			float xt = _ax * tt3 + _bx * tt2 + _u0 * tt + _x0;
			float yt = _ay * tt3 + _by * tt2 + _v0 * tt + _y0;
			g.LineTo(xt, yt, 0, color, 20);
		}
		
	}
	
	public void drawEnd(Graphics g)
	{
		if (_sampleNum < 2) return;
		else if (_sampleNum == 2)
		{
			g.MoveTo(_x0, _y0);
			g.LineTo(_x1, _y1);
			return;
		}
		
		// 最後の1点まで線を引くために、前回計算した係数の値をそのまま利用する。
		float dx = _x2 - _x1;
		float dy = _y2 - _y1;
		float dt10 = _t1 - _t0;
		float dt21 = _t2 - _t1;
		int n = ((int)(0.5f * Mathf.Sqrt(dx * dx + dy * dy)) | 0) + 1;
		g.MoveTo(_x1, _y1);
		for (int i = 1; i <= n; i++)
		{
			float tt = dt10 + (dt21 * i / n);
			float tt2 = tt * tt;
			float tt3 = tt2 * tt;
			float xt = _ax * tt3 + _bx * tt2 + _u0 * tt + _x0;
			float yt = _ay * tt3 + _by * tt2 + _v0 * tt + _y0;
			g.LineTo(xt, yt);
		}
	}
	
	public void reset()
	{
		_sampleNum = 0;
	}

}

