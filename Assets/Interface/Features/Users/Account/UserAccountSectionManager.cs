using JetBrains.Annotations;
using UnityEngine;

public class UserAccountSectionManager : MonoBehaviour {
    [SerializeField] GameObject _mainSection;
    [SerializeField] GameObject _friendRequestsSection;
    [SerializeField] GameObject _socialSection;

    void Start() {
        ToMainSection();
    }

    [UsedImplicitly]
    public void ToMainSection() {
        _mainSection.SetActive(true);
        _friendRequestsSection.SetActive(false);
        _socialSection.SetActive(false);
    }

    [UsedImplicitly]
    public void ToFriendRequestsSection() {
        _mainSection.SetActive(false);
        _friendRequestsSection.SetActive(true);
        _socialSection.SetActive(false);
    }

    [UsedImplicitly]
    public void ToSocialSection() {
        _mainSection.SetActive(false);
        _friendRequestsSection.SetActive(false);
        _socialSection.SetActive(true);
    }
}
