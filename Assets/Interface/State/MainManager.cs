using JetBrains.Annotations;
using UnityEngine;

public class MainManager : MonoBehaviour {
    [UsedImplicitly]
    public void Quit() => SceneStateManager.Quit();
}