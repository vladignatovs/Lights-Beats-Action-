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
    }
}
