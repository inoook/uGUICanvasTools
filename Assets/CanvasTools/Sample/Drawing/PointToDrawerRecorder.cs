using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointToDrawerRecorder : MonoBehaviour {

    [SerializeField] PointToCatmullRom pointToCatmullRom = null;
    private List<List<Vector3>> recordPointsList;

    [SerializeField] bool isPlaying = false;

    int index = 0; // 線分の中のセグメント
    int indexDraw = 0; // 線分

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        if (isPlaying) {
            if (recordPointsList.Count <= 0) {
                isPlaying = false;// END
                return;
            }

            if (pointToCatmullRom.createChildDrawer) {
                if (index == 0) {
                    // create
                    pointToCatmullRom.AddChildDrawer();
                }
            }

            index++;

            List<List<Vector3>> playingPointsList = new List<List<Vector3>>();
            List<Vector3> recordStroke = null;

            for (int i = 0; i < indexDraw; i++) {
                recordStroke = recordPointsList[i];
                List<Vector3> playStroke = new List<Vector3>();
                if (i == indexDraw - 1) {
                    for (int n = 0; n < index; n++) {
                        playStroke.Add(recordStroke[n]);
                    }
                }
                else {
                    playStroke = recordStroke;
                }
                playingPointsList.Add(playStroke);
            }
            pointToCatmullRom.SetPointsList(playingPointsList);

            if (indexDraw > recordPointsList.Count-1) {
                if (index > recordPointsList[recordPointsList.Count - 1].Count-1) {
                    isPlaying = false;// END
                }
            }
            if (index >= recordStroke.Count) {
                index = 0;
                indexDraw++;
            }
        }
    }

    void ResetIndex() {
        index = 0;
        indexDraw = 1;
    }


    [ContextMenu("Clear")]
    public void Clear() {
        ResetIndex();
        isPlaying = false;

        if (recordPointsList != null) {
            recordPointsList.Clear();
        }
        pointToCatmullRom.Clear();
    }

    [ContextMenu("GetRecord")]
    void GetRecord() {
        recordPointsList = pointToCatmullRom.GetPointsList();
    }

    [ContextMenu("GetRecord_Str")]
    public string GetRecordStr() {
        recordPointsList = pointToCatmullRom.GetPointsList();
        if(recordPointsList.Count < 1) {
            return "";
        }
        string str = RecordDataToString(recordPointsList);

        _dmy_str = str;
        return str;
    }

    [ContextMenu("PlayRecordData")]
    void PlayRecordData() {
        isPlaying = true;
        ResetIndex();
    }

    [ContextMenu("PlayRecordData_str")]
    void PlayRecordDataFromStr() {
        PlayRecordData(_dmy_str);
    }

    public void PlayRecordData(string str) {
        pointToCatmullRom.Clear();

        recordPointsList = StringToRecordData(str);

        isPlaying = true;
        ResetIndex();
    }

    string _dmy_str;

    #region utils
    private static string DATA_SPLITTER = "@";

    public static string RecordDataToString(List<List<Vector3>> recordPointsList) {
        string str = "";
        for(int i = 0; i < recordPointsList.Count; i++) {
            List<Vector3> list = recordPointsList[i];
            string lineStr = "";
            for (int n = 0; n < list.Count; n++) {
                Vector3 vector3 = list[n];
                lineStr += vector3.x.ToString("0.0")+","+ vector3.y.ToString("0.0") + ",";
            }
            lineStr = lineStr.Remove(lineStr.Length - 1);
            str += lineStr + DATA_SPLITTER;
        }
        str = str.Remove(str.Length - 1);

        return str;
    }

    public List<List<Vector3>> StringToRecordData(string str) {
        List<List<Vector3>> recordPointsList = new List<List<Vector3>>();
        if(str == "") {
            return recordPointsList;
        }
        int dataSeq = 2;
        string[] lineStr = str.Split(DATA_SPLITTER[0]);
        for (int i = 0; i < lineStr.Length; i++) {
            string[] strArray = lineStr[i].Split(","[0]);

            List<Vector3> list = new List<Vector3>();
            for (int n = 0; n < strArray.Length / dataSeq; n++) {
                float x = 0;
                float y = 0;
                float z = 0;
                float.TryParse(strArray[n * dataSeq + 0], out x);
                float.TryParse(strArray[n * dataSeq + 1], out y);
                list.Add(new Vector3(x, y, z));
            }
            recordPointsList.Add(list);
        }

        return recordPointsList;
    }
    #endregion
}
