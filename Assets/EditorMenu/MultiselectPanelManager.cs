using UnityEngine;

public class MultiselectPanelManager : MonoBehaviour {
    
    [SerializeField] GameObject multiselectPanel;
    [SerializeField] Camera _visualizerCamera;
    RectTransform multiselectRectTransform;
    public GameObject Content;

    Vector2 startPos;
    Vector2 endPos;

    void Start() {
        startPos = endPos = Vector2.zero;
        multiselectRectTransform = multiselectPanel.GetComponent<RectTransform>();
    }
    void Update() {

        // on click
        if(Input.GetMouseButtonDown(1)) {
            startPos = Input.mousePosition;
            DrawMultiselectPanel();
        }
        // on drag
        if(Input.GetMouseButton(1)) {
            endPos = Input.mousePosition;
            DrawMultiselectPanel();
        }
        
        // on release
        if(Input.GetMouseButtonUp(1)) {
            SelectActions();
            startPos = endPos = Vector2.zero;
            DrawMultiselectPanel();
        }
    }

    /// <summary>
    /// Draws the multiselect panel by first assigning its position between the static and dynamic corners, and then changing its scale
    /// to fit between those corners.
    /// Uses both <c>startPos</c> and <c>endPos</c> variables to define both position and scale.
    /// </summary>
    void DrawMultiselectPanel() {
        Vector2 staticCorner = startPos;
        Vector2 dynamicCorner = endPos;

        multiselectRectTransform.anchoredPosition = (staticCorner + dynamicCorner) / 2;

        multiselectRectTransform.localScale = new Vector3(staticCorner.x - dynamicCorner.x, staticCorner.y - dynamicCorner.y, 1);
    }
    /// <summary>
    /// Goes through the list of gameObjects with the tag of ActionLine, and selects the unselected ones that were hit by the multiselect 
    /// panel. Defines "hit" by using the bottomLeft and topRight corners of the panel and position of the actionLine. Also forses the 
    /// selectPanel to appear if at least one actionLine got or already is selected.
    /// </summary>
    void SelectActions() {
        /* Point of these fields is to make sure that the rectangle corners are correctly ordered: 
            -- startPos = (1,1), endPos = (-1,-1). 
                -- Mathf.Min(1,-1) = -1
                -- Mathf.Min(1,-1) = -1
                    -- minPos = (-1,-1) basically the bottom-left corner
                -- Mathf.Max(1,-1) = 1
                -- Mathf.Max(1,-1) = 1
                    -- maxPos = (1,1) basically top-right corner*/
        Vector2 bottomLeftCorner = new (Mathf.Min(startPos.x, endPos.x), Mathf.Min(startPos.y, endPos.y));
        Vector2 topRightCorner = new (Mathf.Max(startPos.x, endPos.x), Mathf.Max(startPos.y, endPos.y));
        // helper fields used for efficiency and perfomance
        var contentScale = Content.transform.localScale.x;
        var atLeastOneSelected = false;

        var _actionLines = GameObject.FindGameObjectsWithTag("ActionLine");
        foreach(var actionLine in _actionLines) {
            // helper fields used for optimization and readability
            Vector3 screenPos = _visualizerCamera.WorldToScreenPoint(actionLine.transform.position);
            var actionLineX = screenPos.x;
            var actionLineY = screenPos.y;

            /* Main if statement, that defines if the action gets selected or not. It works like that:
                -- actionLineX = 1, actionLineY = 1; bottomLeftCorner = (0,2), topRightCorner = (2,4) <- defines a 2x2 square
                    -- first line: if(1+5 >= 0 AND 1 <= 2) Y
                    -- second line: if(1+180 >= 2 AND 1 <= 4) X, shouldnt be like that as the actionLine extends 180 upwards
                        -- ^^ in order to fix the logic here and make the action Line select when needed simply adding the 180 to the first
                        if statement of the second line fixes the issue. <- decided to also add 5 to the first if statement of the first line,
                        to fix another minor issue by the same logic. Used contentScale to also increase that value with according scale. */
            if(actionLineX+5*contentScale >= bottomLeftCorner.x && actionLineX <= topRightCorner.x
            && actionLineY+180*contentScale >= bottomLeftCorner.y && actionLineY <= topRightCorner.y) {
                var _actionLineManager = actionLine.GetComponent<ActionLineManager>();
                // checks if the action has not been selected before
                if(!_actionLineManager.Selected) {
                    _actionLineManager.SelectAction();
                }
                // at this point every actionLine is selected at this point, so it just checks if there are none selected currently, 
                // and if there are, changes the value of the bool and toggles select panel.
                if(!atLeastOneSelected) {
                    _actionLineManager.ToggleSelectPanel(true);
                    atLeastOneSelected = true;
                }
            }
        }
    }
}
