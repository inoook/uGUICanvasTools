using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SliderValueLabel : MonoBehaviour {

	public Text labelText;
	public Slider slider;

	public string preLabel = "";
	public string format = "0.00";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		labelText.text = preLabel + (slider.value).ToString(format);
	}
}
