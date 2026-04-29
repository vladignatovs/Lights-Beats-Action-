using UnityEngine;

public class ControllerVisualizerManager : MonoBehaviour {
    [SerializeField] Transform _controllerPanelTransform;
    void Awake() {
        _controllerPanelTransform = GameObject.FindGameObjectWithTag("ControllerPanel").transform;
        transform.SetParent(_controllerPanelTransform);
    }

    void Update() {
        if (transform.localScale != Vector3.one) {
            transform.localScale = Vector3.one;
        }
    }
}
