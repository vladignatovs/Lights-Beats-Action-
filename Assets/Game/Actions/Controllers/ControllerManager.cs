using UnityEngine;

public abstract class ControllerManager : MonoBehaviour {
    [SerializeField] protected ControllerGroupManager _controllerGroupManager;
    [SerializeField] protected DurationsManager _durationsManager;
    protected float _lifeTime;

    void Start() {
        _lifeTime = _durationsManager.getLifeTimeInSeconds();
    }

    protected void LateUpdate() {
        float timer = _durationsManager.Timer;

        if(timer > _lifeTime) {
            Destroy(gameObject);
        }
    }

    public abstract void SetAllUniqueValues(Action action);
}