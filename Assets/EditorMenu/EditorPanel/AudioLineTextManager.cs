using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioLineTextManager : MonoBehaviour {
    public GameObject audioLine;
    RectTransform rectTransform;
    [SerializeField] Text text;
    // Start is called before the first frame update
    void Start() {
        rectTransform = audioLine.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update() {
        text.text = Math.Round(rectTransform.localPosition.x / 50, 2).ToString();
    }
}
