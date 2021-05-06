using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// マウスでcanvasに描画を行う。
/// </summary>
// http://wonderfl.net/c/4IBM
// http://unity-michi.com/post-694/
public class PointToCatmullRom : MonoBehaviour {

	CanvasGraphics graphics;
	public CanvasQuadDrawer verticesDrawer;
    [SerializeField] bool useSmoothing = true;
    [SerializeField] bool useCatmull = true;
    [SerializeField] bool useAvg = false;

    [SerializeField] RectTransform rectTrans = null;
    [SerializeField] float smoothDotLimit = 0.5f;
    [SerializeField] float drawMinDist = 2.0f;
    [SerializeField] float distLimit = 100;

	private List<Vector3> points;
	private List<List<Vector3>> pointsList;

    [SerializeField] Texture2D strokeTexture = null;

    public float thickness = 2.0f;
    public float thicknessMin = 2.0f;

    [Header("塗り色 移動距離で 0 -> 1 へ塗り色変化")]
    public Color drawColor0 = Color.red;
    public Color drawColor1 = Color.red;
    
    bool isDraw;

    private Vector2 preMousePos;
    [SerializeField] float mouseDamping = 0.4f;

    [SerializeField] public bool createChildDrawer = true;

	// Use this for initialization
	void Start () {
		graphics = new CanvasGraphics();

        if (!createChildDrawer) {
            if(verticesDrawer == null) {
                verticesDrawer = this.gameObject.AddComponent<CanvasQuadDrawer>();
                if(strokeTexture != null) {
                    verticesDrawer.texture_ = strokeTexture;
                }
            }
            graphics.verticesDrawer = verticesDrawer;
        }

		pointsList = new List<List<Vector3>>();
	}
    // Update is called once per frame
    void Update () {

        graphics.isUseAvg = useAvg;
        graphics.curveSmoothing = useCatmull && useSmoothing;
        graphics.smoothDotLimit = smoothDotLimit;

		Vector3 mouse = Input.mousePosition;
		mouse.z = 10;

		Camera canvasCam = this.GetComponentInParent<Canvas>().worldCamera;
		Vector2 worldPos = Vector2.zero;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTrans, mouse, canvasCam, out worldPos);
		float mouseX = worldPos.x;
		float mouseY = worldPos.y;

        bool isContain = rectTrans.rect.Contains(new Vector2(mouseX, mouseY));

        //isDraw = false;
		if(isContain && Input.GetMouseButtonDown(0)){

            if (createChildDrawer) {
                AddChildDrawer();
            }

            // create new pointGroup
            points = new List<Vector3>();
			pointsList.Add(points);

            Plot(mouseX, mouseY);

            preMousePos = new Vector2(mouseX, mouseY);

            isDraw = true;
        }
        if (Input.GetMouseButtonUp(0)) {
            isDraw = false;
        }

        if (isDraw && Input.GetMouseButton(0)){
            float delta = Vector2.Distance(new Vector2(mouseX, mouseY), preMousePos);

            mouseX = Mathf.Lerp(preMousePos.x, mouseX, mouseDamping);
            mouseY = Mathf.Lerp(preMousePos.y, mouseY, mouseDamping);

            if (!isContain) {
                //Vector2 normalizePos = new Vector2(mouseX / 400f + 0.5f, mouseY / 400f + 0.5f);
                Vector2 normalizePos = Rect.PointToNormalized(rectTrans.rect, new Vector2(mouseX, mouseY));
                Vector2 pos = Rect.NormalizedToPoint(rectTrans.rect, normalizePos);
                mouseX = pos.x;
                mouseY = pos.y;
            }

            //float delta = Vector2.Distance(worldPos, preWorldPos);
            if (delta > drawMinDist) {
                Plot(mouseX, mouseY);

                isDraw = true;
            }

            preMousePos.x = mouseX;
            preMousePos.y = mouseY;
        }
        if (isDraw) {
            Draw();
        }
	}

	private void Plot(float x, float y)
	{
		points.Add(new Vector3(x, y, 0));
	}

	// update
	private void Draw()
	{
        if(pointsList.Count < 1) {
            graphics.Clear();
            graphics.Render();
            return;
        }

        graphics.Clear();
		graphics.SetDefaultColor(drawColor0);
		graphics.SetDefaultThickness(thickness);

        Vector3 dir = -Vector3.forward;
        graphics.SetDefaultFaceDir(dir);

        // childを作るかどうかでの処理わけ。childを作らない場合は pointsList を全て描画、作る場合は最新の pointsList のみ描画
        int startIndex = createChildDrawer ? pointsList.Count - 1 : 0;

        for (int i = startIndex; i < pointsList.Count; i++) {
        //for (int i = 0; i < pointsList.Count; i++) { 
			List<Vector3> points = pointsList[i];
			float[] thicknessArray = new float[points.Count];
			Color[] colorArray = new Color[points.Count];
			for(int j = 0; j < thicknessArray.Length; j++){
				if(j == 0){
					thicknessArray[j] = thicknessMin;
					colorArray[j] = drawColor0;
				}else{
                    float dist = Vector3.Distance(points[j - 1], points[j]);
                    //thicknessArray[j] = (1-(dist / 40f)) * thickness + thickness;
					float t = 1 - Mathf.Clamp01(dist / distLimit);
					colorArray[j] = Color.Lerp(drawColor0, drawColor1, t);
                    //thicknessArray[j] = thickness;
                    //thicknessArray[j] = t * thickness + thicknessMin;
                    thicknessArray[j] = ((t * thickness + thicknessMin) + thicknessArray[j - 1]) * 0.5f; // avg
                }
            }
			//
			if(useCatmull){
				graphics.DrawSpline(points.ToArray(), thicknessArray, colorArray);
			}else{
				graphics.DrawLine(points.ToArray(), thicknessArray);
			}
		}
		graphics.Render();
	}

    public List<List<Vector3>> GetPointsList() {
        return pointsList;
    }
    public void Clear() {

        ClearChildren();

        isDraw = false;
        pointsList.Clear();
        
        Draw();
    }
    public void SetPointsList(List<List<Vector3>> pointsList_) {
        isDraw = false;
        pointsList = pointsList_;
        Draw();
    }

    //
    public void AddChildDrawer() {
        // 線分ごとにgameObjectを作る場合。
        graphics.verticesDrawer = CreateDrawer();
    }

    public CanvasQuadDrawer CreateDrawer() {
        GameObject gObj = new GameObject("stroke");
        gObj.AddComponent<RectTransform>();
        gObj.AddComponent<CanvasRenderer>();
        var drawer = gObj.AddComponent<CanvasQuadDrawer>();
        if (strokeTexture != null) {
            drawer.texture_ = strokeTexture;
        }

        gObj.transform.SetParent(this.transform);
        gObj.transform.localPosition = Vector3.zero;

        return drawer;
    }
    void ClearChildren() {
        foreach(Transform t in this.transform) {
            Destroy(t.gameObject);
        }
    }
}


