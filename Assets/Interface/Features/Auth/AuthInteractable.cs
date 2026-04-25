using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class AuthInteractable : AuthExclusive {
    [SerializeField] Selectable _selectable;

    void Reset() {
        _selectable = GetComponent<Selectable>();
    }

    protected override void Start() {
        if (_selectable == null) {
            _selectable = GetComponent<Selectable>();
        }

        base.Start();
    }

    protected override void ApplyState(bool isAllowed) {
        if (_selectable == null) {
            return;
        }

        _selectable.interactable = isAllowed;
    }
}
