using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointToDrawerRecorderTest : MonoBehaviour {

    [SerializeField] PointToDrawerRecorder pointToDrawerRecorder = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void Clear() {
        _recordStr = "";
        pointToDrawerRecorder.Clear();
    }

    [SerializeField] Rect drawRect = new Rect(10,10,100,100);
    [SerializeField] string _recordStr = "";

    private void OnGUI() {
        GUILayout.BeginArea(drawRect);
        if (GUILayout.Button("Clear")) {
            Clear();
        }
        if (GUILayout.Button("Record")) {
            _recordStr = pointToDrawerRecorder.GetRecordStr();
            Debug.Log(_recordStr.Length);
        }
        if (GUILayout.Button("Replay")) {
            pointToDrawerRecorder.PlayRecordData(_recordStr);
        }
        GUILayout.EndArea();
    }
}
