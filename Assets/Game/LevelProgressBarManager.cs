using UnityEngine;
using UnityEngine.UI;

public class LevelProgressBarManager : MonoBehaviour {
    [Header("Progress Bar functionality")]
    [SerializeField] Slider _baseSlider;
    [SerializeField] Slider _completionSlider;
    [SerializeField] AttemptManager _attemptManager;
    [Header("Progress Bar visuals")]
    [SerializeField] SpriteRenderer _player;
    [SerializeField] Image _completionFill;
    [SerializeField] Image _accuracyFill;
    [SerializeField] CanvasGroup _progressBarCanvasGroup;
    readonly float _fadeDistance = 200;

    void Update() {
        UpdateProgressFromAttemptManager();
        UpdateBarFadeFromPlayerDistance();
    }

    void UpdateProgressFromAttemptManager() {
        float completionPercent = Mathf.Clamp01(_attemptManager.CompletionPercent);
        float accuracyPercent = Mathf.Clamp01(_attemptManager.AccuracyPercent);

        _baseSlider.value = completionPercent;
        _completionFill.fillAmount = completionPercent;
        _completionSlider.value = accuracyPercent;
        _accuracyFill.fillAmount = accuracyPercent;
    }

    void UpdateBarFadeFromPlayerDistance() {
        // Get the player position according to the screen
        Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint(_player.transform.position);

        // Get the corners of the RectTransform of the slider. Positions are according to the screen as well.
        /*  corner 0 -- bottom left
            corner 1 -- top left
            corner 2 -- top left
            corner 3 -- bottom right */
        Vector3[] corners = new Vector3[4];
        _baseSlider.GetComponent<RectTransform>().GetWorldCorners(corners);

        // Using a complicated function that returns the closest point on the slider to the playerScreenPosition
        Vector3 closestPoint = GetClosestPointOnLineSegment(corners[0], corners[3], playerScreenPosition);
        // Gets the distance between the player and closest point 
        float distance = Vector3.Distance(playerScreenPosition, closestPoint);

        float fadeMultiplier = distance <= _fadeDistance ? distance / _fadeDistance : 1f;

        _progressBarCanvasGroup.alpha = fadeMultiplier;
    }

    // Method that returns the closest point of the given vector ab to a point "point".
    Vector3 GetClosestPointOnLineSegment(Vector3 a, Vector3 b, Vector3 point) {
        Vector3 ab = b - a; // Gives the projection of the vector between these two points
        float ab2 = ab.sqrMagnitude; // Gives the squared length of the vector projection
        Vector3 ap = point - a; // Gives the projection of the vector between beginning of a vector and a point
        float t = Mathf.Clamp01(Vector3.Dot(ap, ab) / ab2); /* Gives the proportional coefficient of 
        the nearest point to the vector from the point: 
            -- ((10,0) and (5,-10); Dot product is 50 (10*5 + 0*-10), squared magnitude is 100. 
            Aquired quotient is 0.5, which corresponds to the point on the vector which is closest to the needed point) */
        return a + t * ab; /* Adds the part of the vector projection to the start of the vector. If t = 0, will simply return a
        as it already represents the closest point. If t = 1, will return vector simillar to b, ((10,10) + 1*(10,0) = (20,10))
        if t is in between 0 and 1 will add a corresponding part of the projection to the vector. IF UNCERTAIN USE DESM0S.*/
    }
}
