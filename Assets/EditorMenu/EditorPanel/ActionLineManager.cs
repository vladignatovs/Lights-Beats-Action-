using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class ActionLineManager : MonoBehaviour {
    [Header("Action control")]
    public Action Action;
    ActionCreator _actionCreator;
    public bool Selected = false;
    float _maxPos;
    public static List<ActionLineManager> ActionLineManagersSingleton = new ();
    [Header("Action settings")]
    Toggle _activeSelectedActionToggle;
    GameObject _selectPanel;
    GameObject _settingsPanel;
    [SerializeField] Image _actionLineImage;
    [SerializeField] Color normalColor;
    [SerializeField] Color selectColor;
    bool _showTimes = true;
    public bool ShowTimes  {
        get => _showTimes;
        private set { _showTimes = value; }
    }
    bool _showLifeTime = true;
    public bool ShowLifeTime {
        get => _showLifeTime;
        private set { _showLifeTime = value; }
    }

    int _layer;
    public int Layer {
        get => _layer;
        private set {
            _layer = Mathf.Max(value, 0);
            CompareWithLayer();
        }
    }
    [Header("Instantiating")]
    [SerializeField] GameObject _timesActionLine;
    [SerializeField] GameObject _selectedActionToggle;
    [SerializeField] GameObject _lifeTimeLine;
    [Header("Action visualization")]
    GameObject _visualizedGameObject;
    Transform _logicTransform;
    void Awake() { //those were not avaliable for the showSettings functions as soon as an object was instantiated, so I added them in Awake() method instead
        _actionCreator = GameObject.FindGameObjectWithTag("ActionCreator").GetComponent<ActionCreator>();
        _selectPanel = _actionCreator.SelectPanel;
        _settingsPanel = _actionCreator.SettingsPanel;
        _logicTransform = GameObject.FindGameObjectWithTag("Logic").transform;
    }
    void Start() {
        // UP TO CHANGE
        var actionSettings = _actionCreator.ActionsSettings[_actionCreator.Actions.IndexOf(Action)];
        ShowTimes = actionSettings.Item1;
        ShowLifeTime = actionSettings.Item2;
        Layer = actionSettings.Item3;
        // UP TO CHANGE

        UpdateTimesAndLifetime(Action.Times);
        _maxPos = (_actionCreator.Content.GetComponent<RectTransform>().rect.width-5) / 50;

        ActionLineManagersSingleton.Add(this);
    }

    void OnDestroy() => ActionLineManagersSingleton.Remove(this);

    #region changeBeat
    /// <summary>
    /// Sets the value of <c>beat</c> variable to the input value, with a few limitations:
    /// <br> 1. If the input value cannot be parsed into float, ignores the change request. </br>
    /// <br> 2. Clamps the input value between 0 (beat can't be negative) and maxBeat (Overall amount of beats avaliable minus delay). </br>
    /// </summary>
    /// <remarks>
    /// Beat variable is more restrictive than delay variable, as times variable is tied to it.
    /// </remarks>
    /// <param name="value"></param>
    public void changeBeat(string value) {
        // if parsing is successful, set the max value of the beat and clamp the action.beat value between it and 0
        if(value.FloatTryParse(out float beat)) {
            var maxBeat = _maxPos - Action.Delay;
            
            // using Mathf.Clamp because beat shouldn't be less than 0 and larger than maxBeat
            Action.Beat = (float)Math.Round(Mathf.Clamp(beat, 0, maxBeat), 5);
            transform.localPosition = new Vector3(Action.Beat*50 + Action.Delay*50, transform.localPosition.y, transform.localPosition.z);
            UpdateTimesAndLifetime(Action.Times);
        }
        // if parsing is not successful dont change the value of action.beat
    }
    #endregion
    #region changeTimes
    public void changeTimes(string value) {
        Action.Times = int.Parse(value);
        UpdateTimesAndLifetime(Action.Times);
    }
    #endregion
    #region UpdateTimesAndLifetime
    /// <summary>
    /// Used to primarily instantiate timesLines, set their parent and change their color, as well as changing the size and color 
    /// of the LifeTimeLines. Clears all of the timeLines before hand.
    /// </summary>
    /// <param name="times"></param>
    void UpdateTimesAndLifetime(int times) {
        ClearTimesClones();
        UpdateLifeTimeLine(transform);
        if(ShowTimes) {
            for(int i = 2; i <= times; i++) {
                GameObject timesAction = Instantiate(
                    _timesActionLine, 
                    new Vector3(Action.Beat * 50 * (i - 1), transform.position.y, transform.position.z),
                    Quaternion.identity);
                timesAction.transform.SetParent(transform, false);
                UpdateLifeTimeLine(timesAction.transform);
                var actionLineColor = _actionLineImage.color;
                timesAction.GetComponent<Image>().color = new Color(actionLineColor.r, actionLineColor.g, actionLineColor.b, .25f);
            }
        }
    }
    void ClearTimesClones() {
        foreach(Transform child in transform) {
            if(!child.gameObject.CompareTag("LifeTimeLine")) {
                Destroy(child.gameObject);
            }
        }
    }
    void UpdateLifeTimeLine(Transform parentTransform) {
        var rectTransform = parentTransform.GetChild(0) as RectTransform;
        rectTransform.sizeDelta = new Vector2(ShowLifeTime ? Action.LifeTime*50 : 0, 5);
        rectTransform.anchoredPosition = new Vector2(0, Action.GObject != null && Action.GObject.Contains("Controller") ? -175 : 0);
        var actionLineColor = _actionLineImage.color;
        rectTransform.GetComponent<Image>().color = new Color(actionLineColor.r, actionLineColor.g, actionLineColor.b, .03f);
    }
    #endregion
    #region changeDelay
    /// <summary>
    /// Sets the value of <c>delay</c> variable to the input value, with a few limitations:
    /// <br> 1. If the input value cannot be parsed into float, ignores the change request. </br>
    /// <br> 2. Clamps the input value between minDelay (can't be less than negative beat, as it will result in an overall negative position) 
    /// and maxDelay (Overall amount of beats avaliable minus beat). </br>
    /// </summary>
    /// <remarks>
    /// Delay variable is less restrictive than beat variable.
    /// </remarks>
    /// <param name="value"></param>
    public void changeDelay(string value) {
        // if parsing is successful, set the max value of the beat and clamp the action.beat value between it and 0
        if(value.FloatTryParse(out float delay)) {
            var maxDelay = _maxPos - Action.Beat;
            var minDelay = -Action.Beat;

            // Using Mathf.Clamp because delay might be negative, but shouldn't be less than negative action.beat 
            // (so that the action line wouldn't go below zero position) and shouldn't be larger than maxDelay
            Action.Delay = (float)Math.Round(Mathf.Clamp(delay, minDelay, maxDelay), 5); 
            transform.localPosition = new Vector3(Action.Beat*50 + Action.Delay*50, transform.localPosition.y, transform.localPosition.z);
        }
        // if parsing is not successful dont change the value of action.beat
    }
    #endregion
    #region changeObject
    public void changeObject(int value) {
        // used try to catch the IndexOutOfRangeException, as it can only occur when the celected option is 0
        try {
            Action.GObject = _actionCreator.CreatableObjects[value-1].name;
        } catch (System.IndexOutOfRangeException) {
            Action.GObject = null; 
        }
        Destroy(_visualizedGameObject);
        VisualizeAction(); // <------------- VISUALIZATION
        _visualizedGameObject.GetComponent<VisualizerManager>().ToggleSelect(Selected);
        
        // Updating the Times and LifeTime lines of each time as if the object ends up as Controller, lifetime lines should be lower than usual.
        UpdateTimesAndLifetime(Action.Times);
    }
    #endregion
    #region changePosition
    /// <summary>
    /// Used to change the position property of an action, as well as change the position of the <c>visualizedGameObject</c>. If 
    /// char ';' is found:
    /// <br/>Will set the value of <c>x</c> to the parsed value of string before ';' char.
    /// <br/>Will set the value of <c>y</c> to the parsed value of string after ';' char.
    /// <br/>NOTE: if parsing is unsuccessful (that part of string is null, or contains various symbols apart from numbers), will default that
    /// value to 0.<br/><br/>
    /// 
    /// If the character is not found however, it will set <c>x</c> to the parsed string value, and set <c>y</c> to 0.
    /// </summary>
    /// <remarks>
    /// Only works if the length of the value is greater than zero.
    /// </remarks>
    /// <param name="value"></param>
    public void changePosition(float x, float y) {
        Action.PositionX = x;
        Action.PositionY = y;
        _visualizedGameObject.transform.position = new(x, y, 0); // <------------- VISUALIZATION
    }
    #endregion
    #region changeRotation
    public void changeRotation(string value) {
        // Create a dictionary, where each key is a text representation of the boolean value,
        // and each dictionary value is an angle, which will give either a 1 or 0 in quaternion.angleaxis.z
        var boolToAngleMap = new Dictionary<string, string> {
            { "true", "180" },
            { "false", "0" }
        };
        // TryGetValue returns a bool, if key gets something, then it will make the value equal to it
        // so if the value is "true", then the key is correct, and it will set the value to 180
        if (boolToAngleMap.TryGetValue(value, out string angleString)) {
            value = angleString;
        }
        // TryParse the string value to a float, if cannot parse, it is defaulted to 0.
        value.FloatTryParse(out float angle);

        // // Sets the rotation to the value of angle, created in the TryParse func
        // Action.Rotation = Quaternion.AngleAxis(angle, transform.forward);

        Action.Rotation = angle;

        _visualizedGameObject.transform.rotation = Quaternion.Euler(0,0, angle); // <------------- VISUALIZATION
        Debug.Log(Action.Rotation);
    }
    #endregion
    #region changeScale
    public void changeScale(float x, float y) {
        Action.ScaleX = x;
        Action.ScaleY = y;
        _visualizedGameObject.transform.localScale = new(x, y, 1); // <------------- VISUALIZATION
    }
    #endregion
    #region changeAnimationDuration
    public void changeAnimationDuration(string value) {    
        Action.AnimationDuration = value.FloatParse();
    }
    #endregion
    #region changeLifeTime
    public void changeLifeTime(string value) {
        // checks if lifeTime can be parsed.
        if(value.FloatTryParse(out float lifeTime)) {
            // Clamps the lifeTime between 0 (lifeTime can't be negative) and _maxPos - action position (_lifeTime can't be over the length of the song).
            Action.LifeTime = Mathf.Clamp(lifeTime, 0, _maxPos - (Action.Beat + Action.Delay));
        }
        UpdateTimesAndLifetime(Action.Times);
    }
    #endregion
    #region changeGroups
    /// <summary>
    /// Adds an int value to a groups list. If the value is negative, will set it to zero instead. Will also cap the value at 99999.
    /// </summary>
    public void addGroup(string value) {

        if(int.TryParse(value, out int parsedValue)) {
            parsedValue = Mathf.Clamp(parsedValue, 0, 99999);
            if (!Action.Groups.Contains(parsedValue)) {
                Action.Groups.Add(parsedValue);
                Debug.Log("added " + parsedValue);
            }
        }
    }

    public void deleteGroup(int value) {
        if(value != 0 && Action.Groups.Remove(value)) {
            Debug.Log("deleted " + value);
        }
    }
    #endregion
    #region cloneAction
    /// <summary>
    /// Used to clone the action. The action to clone is determined by the caller script instance, and it instantiates the same actionLine
    /// gameObject, assigns the actionCreator.Content as parent and renames the gameObject. Also clones the action of the caller script and
    /// adds it to the list. Also uses the <c>SelfSelect()</c> method.
    /// </summary>
    public void cloneAction() {
        // creates a new actionLine gameobject, which takes all of the parameters of the current one.
        GameObject clonedActionLine = Instantiate(gameObject, _actionCreator.Content);
        // sets a name, due to the fact that if the cloning is done multiple times in the row the "(Clone)" will stack up
        clonedActionLine.name = "ActionLine(Clone)";
        var actionLineManager = clonedActionLine.GetComponent<ActionLineManager>();
        var action = Action.Clone();
        // sets the action object to the clone of the current one
        actionLineManager.Action = action;

        //
        actionLineManager.ShowTimes = ShowTimes;
        actionLineManager.ShowLifeTime = ShowLifeTime;
        actionLineManager.Layer = Layer;
        //

        // adds said action object to the list
        _actionCreator.AddActionToActionsList(actionLineManager.Action);
        // used to manually select the newlycreated action
        actionLineManager.SelfSelect();
    }
    #endregion
    #region deleteAction
    public void deleteAction() {
        Destroy(_visualizedGameObject); // <------------- VISUALIZATION

        // does everything to remove the visuals of the selected gameObject.
        SelfDeselect();

        _actionCreator.RemoveActionFromActionsList(Action);
        Destroy(gameObject);
    }
    #endregion
    #region selectAction
    /// <summary>
    /// This method goes through each GameObject with tag ActionLine, and its goal is to mainly:
    /// <br> 1. Select sharing action lines with its caller, to prevent confusion with overlaying; </br>
    /// <br> 2. Make the selectPanel appear when the conditions are met. </br>
    /// </summary>
    /// <remarks>
    /// NOTE: this method is used as main method for actionLine manual selection.
    /// </remarks>
    public void CheckForMultiple() {
        // sets the value of the bool, to later monitor whether at least one of the actionLines is selected.
        bool atLeastOneSelected = false;
        // the target position of the so called "caller" (actionLine which was initially selected, appears as the caller for other sharing its position actionLines.)
        Vector3 targetPosition = transform.localPosition;
        // list of all gameObjects with the tag ActionLine
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("ActionLine");
        // foreach loop goes through each object in the list.
        foreach (GameObject gameObject in gameObjects) {
            // field that hold the actionLineManager of the current object in the list
            var actionLineManager = gameObject.GetComponent<ActionLineManager>();
            // guaranteed to run AT LEAST once each call of this method.
            // selects all of the objects that are on the target position
            if (gameObject.transform.localPosition.x == targetPosition.x) {
                // explanation of the method below. [!!!!!!]
                actionLineManager.SelectAction();
            }
            // sets the value of atLeastOneSelected bool to true if the condition of action being selected is met.
            if(actionLineManager.Selected) {
                atLeastOneSelected = true;
            }
        }
        ToggleSelectPanel(atLeastOneSelected);
    }
    
    /// <summary>
    /// Used to set "Active" state of the selectPanel to the given value.
    /// </summary>
    /// <param name="value"></param>
    public void ToggleSelectPanel(bool value) {
        _selectPanel.SetActive(value);
    }

    /// <summary>
    /// Frequently used method, mainly changes the value of the <c>selected</c> bool to its opposite value, but is also used for multiple other purposes. 
    /// <br> On select: </br>
    ///     <br> 1. Change the color of the actionLine and its timesClones accordingly; </br>
    ///     <br> 2. Instantiate a selectedActionToggle button object, used to further select and modify the action itself; </br>
    ///     <br> 3. Assign a unique listener to the created toggle, as well as the toggle group; </br>
    ///     <br> 4. Visualize the action. </br>
    /// <br> On deselect: </br>
    ///     <br> 1. Change the color of the actionLine and its timesClones accordingly; </br>
    ///     <br> 2. Destroy the active toggle and visualized object. </br>
    /// </summary>
    public void SelectAction() {
        // changes the "selected" state to its opposite. is frequently used but its value is only changed through this method.
        Selected = !Selected;
        if(Selected) {
            // helper field which is used to easily access the selectedActions object (Content of selectPanel).
            var selectedActions = _selectPanel.transform.GetChild(0);
            // instantiates the selectedActionToggle, and sets its parent;
            // also is the only way to assign the value of activeSelectedActionToggle
            _activeSelectedActionToggle = Instantiate(_selectedActionToggle, selectedActions).GetComponent<Toggle>();
            // adds Listener to the toggle, as well as the group.
            /* NOTE: if toggle is selected, toggles settingPanel as true, else false. */
            _activeSelectedActionToggle.onValueChanged.AddListener((bool isOn) => ToggleSettingsPanel(isOn));
            _activeSelectedActionToggle.group = selectedActions.GetComponent<ToggleGroup>();
            
            VisualizeAction(); // <------------- VISUALIZATION]

            _actionLineImage.color = selectColor;
            UpdateTimesAndLifetime(Action.Times);
        } else {
            Destroy(_activeSelectedActionToggle != null ? _activeSelectedActionToggle.gameObject : null);

            Destroy(_visualizedGameObject); // <------------- VISUALIZATION

            _actionLineImage.color = normalColor;
            UpdateTimesAndLifetime(Action.Times);
        }
    }
    #endregion

    #region selfSelect
    /// <summary>
    /// Used to imitate manual selection of the actionLine. This means that the method forces the selection, as well as the appearance of the
    /// selectPanel, press of the corresponding toggle button, and further appearance of the settingsPanel. If actionLine is selected beforehand,
    /// deselects and proceeds with the said actions as normal.
    /// </summary>
    public void SelfSelect() {
        // if selected, deselects
        if(Selected) {
            SelectAction();
        }
        // at this point the object is guaranteed to be deselected, so it proceeds to select itself.
        SelectAction();
        // since the object is to be selected, it is guaranteed to also require to show a selectPanel.
        ToggleSelectPanel(true);
        // imitates the press of a toggle, forcing the call of the ToggleSettingsPanel() method as well changing the visual state of the toggle.
        _activeSelectedActionToggle.isOn = true;
    }
    #endregion

    #region selfDeselect
    /// <summary>
    /// Used to imitate manual deselection of the actionLine. This means that the method forces the deselection, as well as the disappearance of the
    /// selectPanel if the required conditions are met, press of the corresponding toggle button, and further disappearance of the settingsPanel. If 
    /// actionLine is deselected beforehand, selects and proceeds with the said actions as normal.
    /// </summary>
    public void SelfDeselect() {
        /// if deselected, selects
        if(!Selected) {
            SelectAction();
        }
        // at this point the object is guaranteed to be selected, so it proceeds to deselect itself.
        SelectAction();
        // imitates the press of a toggle, forcing the call of the ToggleSettingsPanel() method as well changing the visual state of the toggle.
        _activeSelectedActionToggle.isOn = false;
        // the object being deselected doesnt guarantee the selectpanel to get disabled, as there might be other active selected actionLines, so
        // it loops through all of the actions, and if at least one of them is selected, leaves the selectPanel on, else toggles to false.
        if(!ActionLineManagersSingleton.Any(actionLineManager => actionLineManager.Selected)) {
            ToggleSelectPanel(false);
        }
    }
    #endregion    
    #region settingsPanel
    /// <summary>
    /// Used to set the "Active" state of the settingsPanel and visualized object to the given value, as well as parse the caller script into
    /// the SettingsPanelManager when <c>true</c>.
    /// </summary>
    /// <remarks>
    /// NOTE: Is used as a Listener for the activeSelectedActionToggle button. This method is called twice
    /// <example>
    /// when selecting option A -> option B right away without deselecting the option A first, as the first option will deselect by itself
    /// as it is a part of a shared toggle group.
    /// </example>
    /// </remarks>
    /// <param name="value"></param>
    public void ToggleSettingsPanel(bool value) {
        // using if statement to prevent errors when trying to deselect visualizedGameObject, as it might not exist in the scene.
        if(_visualizedGameObject != null) {
            // toggles the select state of the visualize game object to the given one.
            _visualizedGameObject.GetComponent<VisualizerManager>().ToggleSelect(value);
        }
        // if the value is true, then also changes the values of the panel and visualizerManager
        if(value) {
            // the order is important, due to the fact that each inputField is updated on Enabled, 
            // so it makes sense to change it before setting the active state
            _settingsPanel.GetComponent<SettingsPanelManager>().ActionLineManager = this;
        }
        // if the value is false, however, will skip the unwanted chunk of code
        // sets the active state of the settings panel, running the OnEnable() method of each InputField and Dropdown.
        _settingsPanel.SetActive(value);
    }
    #endregion

    #region visualizeAction
    /// <summary>
    /// Used to instantiate visualized "hollow" object of the currently selected actionLine. Is also used to set the value of the 
    /// activeSelectedActionToggle image sprite to the one used by the visualized object.
    /// </summary>
    /// <remarks>
    /// NOTE: does not check whether the activeSelectedActionToggle is null or not, might throw an exception.
    /// </remarks>
    public void VisualizeAction() {
        var gameObj = Resources.Load<GameObject>("Hollows/" + Action.GObject);
        if(gameObj != null) {
            _visualizedGameObject = Instantiate(gameObj, new(Action.PositionX, Action.PositionY, 0), Quaternion.Euler(0, 0, Action.Rotation), _logicTransform); //set parent to logic so that the editorBeatManager would automatically clear it when run
            _visualizedGameObject.transform.localScale = new(Action.ScaleX, Action.ScaleY, 1);
            _visualizedGameObject.GetComponent<VisualizerManager>().actionLineManager = this;
        } else {
            var Null = Resources.Load<GameObject>("Hollows/Null");
            _visualizedGameObject = Instantiate(Null, new(Action.PositionX, Action.PositionY, 0), Quaternion.Euler(0, 0, Action.Rotation), _logicTransform);
            _visualizedGameObject.transform.localScale = new(Action.ScaleX, Action.ScaleY, 1);
            _visualizedGameObject.GetComponent<VisualizerManager>().actionLineManager = this;
        }

        var toggleImage = _activeSelectedActionToggle.transform.GetChild(1).GetComponent<Image>();
        if(_visualizedGameObject.TryGetComponent<SpriteRenderer>(out var visualizedSpriteRenderer)) {
            toggleImage.sprite = visualizedSpriteRenderer.sprite;
        } else if (_visualizedGameObject.TryGetComponent<Image>(out var visualizedImage)) {
            toggleImage.sprite = visualizedImage.sprite;
        }
    }
    #endregion
    #region ActionSettings
    public void ShowTimesClones(bool value) {
        ShowTimes = value;

        int actionIndex = _actionCreator.Actions.IndexOf(Action);
        var actionSettings = _actionCreator.ActionsSettings[actionIndex] as (bool, bool, int)?;
        if(actionSettings.HasValue) {
            var newActionSettings = (value, actionSettings.Value.Item2, actionSettings.Value.Item3);
            _actionCreator.ActionsSettings[actionIndex] = newActionSettings;
        }

        UpdateTimesAndLifetime(Action.Times);
    }
    
    public void ShowLifeTimeLine(bool value) {
        ShowLifeTime = value;

        int actionIndex = _actionCreator.Actions.IndexOf(Action);
        var actionSettings = _actionCreator.ActionsSettings[actionIndex] as (bool, bool, int)?;
        if(actionSettings.HasValue) {
            var newActionSettings = (actionSettings.Value.Item1, value, actionSettings.Value.Item3);
            _actionCreator.ActionsSettings[actionIndex] = newActionSettings;
        }

        UpdateTimesAndLifetime(Action.Times);
    }


    /// <summary>
    /// Sets an active state of the gameObject according to the comparison of this objects layer value to the layer of LayerPanelManager. 
    /// If layer is equal to the it, sets active state to true, else false.
    /// </summary>
    public bool CompareWithLayer() { 
        var activeState = _actionCreator.LayerPanelManager.Layer == 0 || _actionCreator.LayerPanelManager.Layer == Layer;
        gameObject.SetActive(activeState);
        return activeState;
    }
    public void MinusLayer() => Layer--;
    public void PlusLayer() => Layer++;
    public void SetLayer(string value) {
        if(int.TryParse(value, out int layer)) {
            Layer = layer;

            int actionIndex = _actionCreator.Actions.IndexOf(Action);
            var actionSettings = _actionCreator.ActionsSettings[actionIndex] as (bool, bool, int)?;
            if(actionSettings.HasValue) {
                var newActionSettings = (actionSettings.Value.Item1, actionSettings.Value.Item2, layer);
                _actionCreator.ActionsSettings[actionIndex] = newActionSettings;
            }
        }
    }
    #endregion
}