using System;
using UnityEngine;
using UnityEngine.UI;

public class ScrollBarPercentageManager : MonoBehaviour
{
    public Scrollbar scrollbar;
    public Text text;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var value = scrollbar.value*100;
        var percentage = Math.Round(Mathf.Min(Mathf.Max(0.0f, value), 100.0f), 2);
        text.text = percentage.ToString() + "%";
    }
}
