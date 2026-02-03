using UnityEngine;

public class EditorButtonManager : MonoBehaviour {
    void Start() {
        if (StateNameManager.LatestMainMenuState != MainMenuState.Local) gameObject.SetActive(false);
    }
}
