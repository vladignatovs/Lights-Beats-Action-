using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SpawnPosManager : MonoBehaviour {
    public GameObject audioLine;
    public InputField inputField;
    public GameObject spSettings;
    RectTransform rectTransform;
    bool locked = true;
    public float offset;
    // Start is called before the first frame update
    void Start() {
        rectTransform = transform as RectTransform;
    }

    // Update is called once per frame
    void Update() {
        if(Input.GetKeyDown(KeyCode.L)) {
            LockPosition();
        }
        var pos = rectTransform.localPosition;
        if(audioLine.activeSelf && !locked) {
            rectTransform.localPosition = new Vector3(audioLine.transform.localPosition.x,pos.y,pos.z);
            inputField.text = (rectTransform.localPosition.x/50).ToString();
        }
        offset = rectTransform.localPosition.x/50;
    }

    public void LockPosition() {
        locked = !locked;
    }
    
    public void ShowSPSettings() {
        spSettings.SetActive(!spSettings.activeSelf);
    }

    public void RemoveSpawnPos() {
        var pos = rectTransform.localPosition;
        rectTransform.localPosition = new Vector3(0, pos.y, pos.z);
        inputField.text = "0";
    }

    public void SetSpawnPos(string value) {
        var pos = rectTransform.localPosition;

        value.FloatTryParse(out float posx);
        rectTransform.localPosition = new Vector3(posx*50, pos.y, pos.z);
        inputField.text = posx.ToString();
    }
}
