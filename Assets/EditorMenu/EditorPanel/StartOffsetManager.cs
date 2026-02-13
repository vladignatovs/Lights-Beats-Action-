using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class StartOffsetManager : MonoBehaviour {
    [SerializeField] GameObject _audioLine;
    [SerializeField] InputField _inputField;
    [SerializeField] GameObject _startOffsetSettings;
    [SerializeField] RectTransform _rectTransform;
    bool locked = true;
    
    public float StartOffset { get; private set; }

    void Update() {
        if(Input.GetKeyDown(KeyCode.L)) {
            ToggleLock();
        }
        
        var pos = _rectTransform.localPosition;
        if(_audioLine.activeSelf && !locked) {
            _rectTransform.localPosition = new Vector3(_audioLine.transform.localPosition.x, pos.y, pos.z);
            UpdateInputField();
        }

        StartOffset = _rectTransform.localPosition.x / 50f;
    }

    [UsedImplicitly]
    public void ToggleLock() {
        locked = !locked;
    }
    
    [UsedImplicitly]
    public void ShowStartOffsetSettings() {
        _startOffsetSettings.SetActive(!_startOffsetSettings.activeSelf);
    }

    [UsedImplicitly]
    public void ClearStartOffset() {
        SetStartOffsetInternal(0);
    }

    [UsedImplicitly]
    public void OnInputFieldChanged(string value) {
        if (value.FloatTryParse(out float offset)) {
            SetStartOffsetInternal(offset);
        }
    }

    public void SetStartOffset(float offset) {
        SetStartOffsetInternal(offset);
    }

    private void SetStartOffsetInternal(float offset) {
        var pos = _rectTransform.localPosition;
        _rectTransform.localPosition = new Vector3(offset * 50f, pos.y, pos.z);
        UpdateInputField();
        StartOffset = offset;
    }

    private void UpdateInputField() {
        _inputField.text = (_rectTransform.localPosition.x / 50f).ToString();
    }
}
