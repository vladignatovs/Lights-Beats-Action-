using UnityEngine;
using UnityEngine.UI;

public class LayerPanelManager : MonoBehaviour {
    [SerializeField] InputField _layerInput;
    [SerializeField] int _layer = 0;
    public int Layer {
        get => _layer;
        private set {
            var newLayer = Mathf.Max(value, 0);
            if(newLayer != _layer) {
                _layer = newLayer;
                RenderLayer();
            }
        }
    }

    void OnEnable() {
        RenderLayer();
    }

    void AssignInputValue(string value) {
        _layerInput.text = value;
    }

    public void PlusLayer() => Layer++;
    public void MinusLayer() => Layer--;
    public void SetLayer(string value) {
        if (int.TryParse(value, out int layer)) {
            Layer = layer;   
        }
    }
    void RenderLayer() {
        AssignInputValue(Layer.ToString());
        var actionLineManagers = ActionLineManager.ActionLineManagersSingleton;

        foreach(var actionLineManager in actionLineManagers) {
            actionLineManager.CompareWithLayer();
        }
        // var childCount = _scrollablePanelContentTransform.childCount;
        // var layerIsZero = Layer == 0;

        // for(int i = 0; i < childCount; i++) {
        //     var child = _scrollablePanelContentTransform.GetChild(i);

        //     if(child.CompareTag(ACTIONLINETAG)) {
        //         // only using this to optimize call of GetComponent()
        //         if(layerIsZero) {
        //             child.gameObject.SetActive(true);
        //         } else {
        //             child.GetComponent<ActionLineManager>().CompareWithLayer();
        //         }
        //     }
        // }
    }
}
