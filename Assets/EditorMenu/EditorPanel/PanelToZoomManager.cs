using UnityEngine;
using UnityEngine.EventSystems;

public class PanelToZoomManager : MonoBehaviour, IScrollHandler {
    Vector3 initialScale;
    [SerializeField] float zoomSpeed = 0.1f;
    [SerializeField] float maxZoom = 5f;
    RectTransform rectTransform;
    // Start is called before the first frame update
    void Start() {
        initialScale = transform.localScale;
        rectTransform = transform as RectTransform;
    }

    public void OnScroll(PointerEventData eventData) {
        //scrollDelta is vector, .y can be either -1 or 1 depending on if they scroll in or out.
        float deltaY = eventData.scrollDelta.y;
        float mousePosition = Input.mousePosition.x;
        // Variables used for convinience
        var _currentScale = rectTransform.localScale.x;
        var _initialScale = initialScale.x;
        var _currentPosition = rectTransform.anchoredPosition.x;
        /* point of zoom equals to:
            -- the current position of the content object (-1000) 
            -- minus the mouse position (960) == (-1960)
            -- divided by the current scale (x2) == (-980)
            -- times the zoomSpeed (0.5f) == (-490) <- point of zoom */
        var pointOfZoom = (_currentPosition - mousePosition)/_currentScale*zoomSpeed;

        /* point of zoom is then added to the current position with each scroll wheel input, 
        which serves as a step that content object has to make in order to stay on the needed position */
        if(deltaY > 0  && _currentScale != _initialScale * maxZoom) { //scrolled in and scale not max
            rectTransform.anchoredPosition += new Vector2(pointOfZoom, 0);
        } else if(deltaY < 0 && _currentScale != _initialScale) { // scrolled out and scale not min
            rectTransform.anchoredPosition -= new Vector2(pointOfZoom, 0);
        }
        var delta = Vector3.one * (deltaY * zoomSpeed);
        var desiredScale = transform.localScale + delta; //adds the recieved input converted into vector to a localScale
        desiredScale = ClampDesiredScale(desiredScale); //using a custom function to make sure that desired scale doesnt go out of bounds
        transform.localScale = desiredScale; //set the localscale to the desired scale
    }

    private Vector3 ClampDesiredScale(Vector3 desiredScale) {
        desiredScale = Vector3.Max(initialScale, desiredScale); //this creates a vector out of every maximal value of each vector
        desiredScale = Vector3.Min(initialScale * maxZoom, desiredScale); //this creates a vector out of every minimal value of each vector
        /*so the desiredscale is first clamped between:
            -- initial scale = (1,1,1), desiredScale = (2,2,2) == (2,2,2), if desiredScale (0.5,0.5,0.5), returns (1,1,1)
        and then it is clamped between: 
            -- initial scale * maxZoom = (10,10,10), desired scale = (2,2,2) == (2,2,2), if desiredScale (11,11,11), returns (10,10,10) */
        desiredScale = new Vector3(desiredScale.x, desiredScale.y, 1);
        return desiredScale;
    }
}
