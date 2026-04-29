using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioLineTextManager : MonoBehaviour {
    public GameObject audioLine;
    RectTransform rectTransform;
    [SerializeField] Text text;
    void Start() {
        rectTransform = audioLine.GetComponent<RectTransform>();
    }

    void Update() {
        text.text = Math.Round(rectTransform.localPosition.x / 50, 2).ToString();
    }
}
