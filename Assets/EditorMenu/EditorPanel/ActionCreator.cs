using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionCreator : MonoBehaviour {
    [Header("Instantiating")]
    [SerializeField] GameObject _beatLine;
    [SerializeField] GameObject _actionLine;
    [Header("Parenting")]
    [SerializeField] RectTransform _content;
    public RectTransform Content {
        get => _content;
        private set { _content = value; }
    }
    [SerializeField] RectTransform _editorPanel;
    [SerializeField] AudioLineManager _audioLineManager;
    [SerializeField] GameObject _confirmationPanel;
    [Header("Referencing")]
    public GameObject SelectPanel;
    public GameObject SettingsPanel;
    public InputField PositionInput;
    public InputField RotationInput;
    public InputField ScaleInput;
    public LayerPanelManager LayerPanelManager;
    [Header("Global Settings (IMPORTANT)")]
    public List<Action> Actions; //important VERY 
    public GameObject[] CreatableObjects; // pretty important, could try make it static
    public Level Level;
    LevelSettings _levelSettings;
    public List<(bool,bool,int)> ActionsSettings;
    int _id;
    #region Set Up
    /// <summary>
    /// Used to manually spawn in the beatLines, set the Content size and load in the Level property.
    /// </summary>
    void Awake() {
        // Gets the id of the level in order to later find it
        _id = StateNameManager.Level.localId;
        // Spawns beatLines according to the beat amount in the level
        for(int i = 0; i < StateNameManager.BeatAmount; i++) {
            Instantiate(_beatLine, _editorPanel);
        }
        // Sets the size of the Content to fit the number of beatlines
        Content.sizeDelta = new Vector2(StateNameManager.BeatAmount*50 + 5, 180);
        Level = StateNameManager.Level;
        foreach(var action in Level.actions) {
            Debug.Log(action.beat);
            Debug.Log(action.times);
            Debug.Log(action.delay);
            Debug.Log(action.gObject);
            Debug.Log(action.position);
            Debug.Log(action.rotation);
            Debug.Log(action.scale);
            Debug.Log(action.animationDuration);
            Debug.Log(action.lifeTime);
            foreach(var group in action.groups)
                Debug.Log(group);
        }
        _levelSettings = LevelSettings.GetLevelSettings(_id);
    }
    /// <summary>
    /// Used to set the Actions list value, audioClip and bpm to the ones in the loaded Level property. Also spawns in actionLines
    /// if there are any actions saved in the Actions list.
    /// </summary>
    void Start() {
        // Sets the value of actions list to the value of the actions list in the saved Level, found by id
        Actions = Level.actions;
        ActionsSettings = _levelSettings.actionsSettings;

        _audioLineManager.audioSource.clip = Resources.Load<AudioClip>(Level.audioPath);
        _audioLineManager.bpm = Level.bpm;
        //

        // Spawns actionLines according to the actions list
        foreach(Action action in Actions) {
            float x = action.beat*50 + action.delay*50;
            GameObject newActionLine = Instantiate(_actionLine, new Vector3(x, 0, 0), Quaternion.identity, Content);
            newActionLine.GetComponent<ActionLineManager>().Action = action; 
        }
    }
    #endregion
    #region ActionCreation
    /// <summary>
    /// Used to create new action. Is the default way of creating new actions, which takes the position of the mouse cursor and places
    /// the actionLine beneath it. Also manually instantiates actionLine object, as well as creates new <c>Action</c> object and adds it to the list 
    /// of actions of actionCreator.
    /// </summary>
    public void createNewAction() {
        float x = Input.mousePosition.x;
        GameObject newActionLine = Instantiate(_actionLine, new Vector3(x, 0, 0), Quaternion.identity, Content);
        newActionLine.GetComponent<ActionLineManager>().Action = new Action(newActionLine.transform.localPosition.x/50);
        AddActionToActionsList(newActionLine.GetComponent<ActionLineManager>().Action);
    }
    /// <summary>
    /// Used to create new action. Alternate method of creating new action, which also uses a "template" action. Places the actionLine
    /// in the far left corner of the screen, and also uses <c>SelfSelect()</c> method. 
    /// </summary>
    public void CreateActionButton() {
        GameObject newActionLine = Instantiate(_actionLine, Vector3.zero, Quaternion.identity, Content);
        newActionLine.GetComponent<ActionLineManager>().Action = new Action(newActionLine.transform.localPosition.x/50) {
            gObject = "Explosion",
            position = new Vector3(0, 0, 0),
            scale = new Vector3(1, 1, 1),
            lifeTime = 1
        };
        AddActionToActionsList(newActionLine.GetComponent<ActionLineManager>().Action);
        newActionLine.GetComponent<ActionLineManager>().SelfSelect();
    }

    public void AddActionToActionsList(Action a) {
        Debug.Log("added an action: " + a.gObject);
        Actions.Add(a);
        ActionsSettings.Add((true, true, LayerPanelManager.Layer));
    }

    public void RemoveActionFromActionsList(Action a) {
        ActionsSettings.RemoveAt(Actions.IndexOf(a));
        Actions.Remove(a);
    }
    #endregion
    #region ActionOptions
    public async void saveAllActions() {
        await LevelManager.SaveLevel(Level);
    }

    /// <summary>
    /// Used to select all actions on a selected layer.Goes through all ActionLineManager instances, and checks if selected, as well as 
    /// compares with layer. If comparing returns true, selects without forcing appearance of SettingsPanel. Also toggles 
    /// the SelectPanel to <c>true</c>.
    /// </summary>
    public void SelectAllActions() { 
        var actionLineManagers = ActionLineManager.ActionLineManagersSingleton;
        
        foreach(var actionLineManager in actionLineManagers) {
            if(actionLineManager.CompareWithLayer() && !actionLineManager.Selected) {
                actionLineManager.SelectAction();
                actionLineManager.ToggleSelectPanel(true);
            }
        }
    }

    /// <summary>
    /// Used to deselect all actions on a selected layer. Goes through all ActionLineManager instances, and compares with layer. If comparing returns
    /// true, self deselects. Also toggles the SelectPanel to <c>false</c>.
    /// </summary>
    public void DeselectAllActions() {
        var actionLineManagers = ActionLineManager.ActionLineManagersSingleton;
        foreach(var actionLineManager in actionLineManagers) {
            if(actionLineManager.CompareWithLayer()) {
                actionLineManager.SelfDeselect();
            }
        }
    }
    
    public void DeleteLevel() {
        LevelManager.DeleteLevel(Level.localId);
        // Level.DeleteLevel(_id);
        // LevelCompletionsManager.DeleteLevelCompletion(_id);
        // LevelSettings.DeleteLevelSettings(_id);
    }
    
    public void ShowConfirmationPanel() {
        if(_confirmationPanel.activeSelf) {
            _confirmationPanel.SetActive(false);
        } else {
            _confirmationPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Used by the UI to set the active state of the audioLine.
    /// </summary>
    public void ToggleAudioLine() {
        var audioSource = _audioLineManager.audioSource;
        if(audioSource.isPlaying) {
            _audioLineManager.gameObject.SetActive(false);
        } else if(!audioSource.isPlaying) {
            _audioLineManager.gameObject.SetActive(true);
        }
    }
    #endregion
}