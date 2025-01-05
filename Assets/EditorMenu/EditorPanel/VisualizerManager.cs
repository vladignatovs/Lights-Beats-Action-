using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VisualizerManager : MonoBehaviour, IDragHandler, IScrollHandler {
    public ActionLineManager actionLineManager;
    Camera visualizerCamera;
    ActionCreator actionCreator;
    public Color normalColor;
    public Color selectedColor;
    SpriteRenderer spriteRenderer;
    public bool selected = false;
    InputField positionInput;
    InputField rotationInput;
    InputField scaleInput;
    float angle = 0;

    void Start() {
        actionCreator = GameObject.FindGameObjectWithTag("ActionCreator").GetComponent<ActionCreator>();
        visualizerCamera = GameObject.FindGameObjectWithTag("VisualizerCamera").GetComponent<Camera>();
        
        positionInput = actionCreator.PositionInput;
        rotationInput = actionCreator.RotationInput;
        scaleInput = actionCreator.ScaleInput;
    }

    /// <summary>
    /// Used mainly to change the color of the visualized gameObject, as well as change the value of "selected" bool, which is used to access
    /// OnDrag and OnScroll function. It's secondary function is to change the z position of the object, to push the selected one forward
    /// for easier control.
    /// </summary>
    /// <param name="isOn"></param>
    public void ToggleSelect(bool isOn) {
        selected = isOn;
        Image image = null;

        if(!TryGetComponent(out spriteRenderer)) {
            TryGetComponent(out image);
        }

        var pos = transform.position;
        if (selected) {
            transform.position = new Vector3(pos.x, pos.y, -1);
            if (spriteRenderer != null) {
                spriteRenderer.color = selectedColor;
            } else if (image != null) {
                image.color = selectedColor;
            }
        } else {
            transform.position = new Vector3(pos.x, pos.y, 0);
            if (spriteRenderer != null) {
                spriteRenderer.color = normalColor;
            } else if (image != null) {
                image.color = normalColor;
            }
        }
    }

    public void OnDrag(PointerEventData eventData) {
        if(selected) {
            var mousePos = visualizerCamera.ScreenToWorldPoint(eventData.position);
            var roundedPos = new Vector3(Mathf.Round(mousePos.x * 4) / 4, Mathf.Round (mousePos.y * 4) / 4, transform.position.z);
            transform.position = roundedPos;
            actionLineManager.changePosition(transform.position.x + ";" + transform.position.y);
            positionInput.GetComponent<InputFieldManager>().ReAssignInputValue();
        }
    }

    public void OnScroll(PointerEventData eventData) {
        if(selected) {
            if(Input.GetKey(KeyCode.LeftShift)) {
                angle += eventData.scrollDelta.y*5;
                transform.rotation = Quaternion.AngleAxis(angle, transform.forward);
                actionLineManager.changeRotation(angle.ToString());
                rotationInput.GetComponent<InputFieldManager>().ReAssignInputValue();
            } else {
                // TODO: make an actually good way of scaling up/down the object.
                var scroll = eventData.scrollDelta.y/10;
                transform.localScale += new Vector3(scroll, scroll, 1);
                actionLineManager.changeScale(transform.localScale.x + ";" + transform.localScale.y);
                scaleInput.GetComponent<InputFieldManager>().ReAssignInputValue();
            }
        }
    }
}
