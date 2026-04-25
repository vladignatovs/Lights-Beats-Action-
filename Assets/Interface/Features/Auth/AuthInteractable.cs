using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class AuthInteractable : AuthExclusive {
    [SerializeField] Selectable _selectable;

    void Reset() {
        _selectable = GetComponent<Selectable>();
    }

    protected override void ApplyState(bool isAllowed) {
        _selectable.interactable = isAllowed;
    }
}
