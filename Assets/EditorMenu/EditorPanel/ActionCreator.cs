using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] Camera _visualizerCamera;
    [Header("Referencing")]
    public GameObject SelectPanel;
    public GameObject SettingsPanel;
    public InputField PositionXInput;
    public InputField PositionYInput;
    public InputField RotationInput;
    public InputField ScaleXInput;
    public InputField ScaleYInput;
    public LayerPanelManager LayerPanelManager;
    [Header("Global Settings (IMPORTANT)")]
    public List<Action> Actions; //important VERY 
    public GameObject[] CreatableObjects; // pretty important, could try make it static
    public Level Level;
    // TODO: put action settings as fields of actions
    // LevelSettings _levelSettings;
    // public List<(bool,bool,int)> ActionsSettings;
    int _id;
    #region Set Up
    /// <summary>
    /// Used to manually spawn in the beatLines, set the Content size and load in the Level property.
    /// </summary>
    void Awake() {
        // If the editor was SOMEHOW opened from a state different from local, meaning 
        // that its trying to open a level that is not local in editor, reject
        if(StateNameManager.LatestMainMenuState != MainMenuState.Local) SceneManager.LoadScene("MainMenu");
        // Gets the id of the level in order to later find it
        _id = StateNameManager.Level.id;
        // Spawns beatLines according to the beat amount in the level
        for(int i = 0; i < StateNameManager.BeatAmount; i++) {
            Instantiate(_beatLine, _editorPanel);
        }
        // Sets the size of the Content to fit the number of beatlines
        Content.sizeDelta = new Vector2(StateNameManager.BeatAmount*50 + 5, 180);
        Level = StateNameManager.Level;
        // foreach(var action in Level.actions) {
        //     Debug.Log(action.Beat);
        //     Debug.Log(action.Times);
        //     Debug.Log(action.Delay);
        //     Debug.Log(action.GObject);
        //     Debug.Log(action.PositionX + "; " + action.PositionY);
        //     Debug.Log(action.Rotation);
        //     Debug.Log(action.ScaleX + "; " + action.ScaleY);
        //     Debug.Log(action.AnimationDuration);
        //     Debug.Log(action.LifeTime);
        //     foreach(var group in action.Groups)
        //         Debug.Log(group);
        // }
    }
    /// <summary>
    /// Used to set the Actions list value, audioClip and bpm to the ones in the loaded Level property. Also spawns in actionLines
    /// if there are any actions saved in the Actions list.
    /// </summary>
    void Start() {
        // Sets the value of actions list to the value of the actions list in the saved Level, found by id
        Actions = Level.actions;
        // ActionsSettings = _levelSettings.actionsSettings;

        _audioLineManager.audioSource.clip = Resources.Load<AudioClip>("Audio/" + Level.audioPath);
        _audioLineManager.bpm = Level.bpm;
        //

        // Spawns actionLines according to the actions list
        foreach(Action action in Actions) {
            float x = action.Beat*50 + action.Delay*50;
            var newActionLine = CreateActionLineAt(new(x, 0));
            // var newActionLine = Instantiate(_actionLine, Content);
            // RectTransform rt = newActionLine.GetComponent<RectTransform>();
            // rt.anchoredPosition = new Vector2(x, 0);
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
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            Content,
            Input.mousePosition,
            _visualizerCamera,
            out var localPos
        );

        var newActionLine = CreateActionLineAt(new(localPos.x,0));
        newActionLine.GetComponent<ActionLineManager>().Action =
            new Action(newActionLine.GetComponent<RectTransform>().anchoredPosition.x / 50);
        AddActionToActionsList(newActionLine.GetComponent<ActionLineManager>().Action);
    }
    /// <summary>
    /// Used to create new action. Alternate method of creating new action, which also uses a "template" action. Places the actionLine
    /// in the far left corner of the screen, and also uses <c>SelfSelect()</c> method. 
    /// </summary>
    public void CreateActionButton() {
        // convert to canvas-local coordinates
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            Content,
            Vector2.zero,
            _visualizerCamera,
            out var localPos
        );
        var newActionLine = CreateActionLineAt(localPos);
        newActionLine.GetComponent<ActionLineManager>().Action = new Action(newActionLine.transform.localPosition.x/50) {
            GObject = "Explosion",
            LifeTime = 1
        };
        AddActionToActionsList(newActionLine.GetComponent<ActionLineManager>().Action);
        newActionLine.GetComponent<ActionLineManager>().SelfSelect();
    }

    public void AddActionToActionsList(Action a) {
        Debug.Log("added an action: " + a.GObject);
        Actions.Add(a);
        // ActionsSettings.Add((true, true, LayerPanelManager.Layer));
    }

    public void RemoveActionFromActionsList(Action a) {
        // ActionsSettings.RemoveAt(Actions.IndexOf(a));
        Actions.Remove(a);
    }

    GameObject CreateActionLineAt(Vector2 position) {
        var newActionLine = Instantiate(_actionLine, Content);
        RectTransform rt = newActionLine.GetComponent<RectTransform>();
        rt.anchoredPosition = position;
        return newActionLine;
    }
    #endregion
    #region ActionOptions
    public async void saveAllActions() {
        await LocalLevelManager.SaveLevel(Level);
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


    // TODO: heavily revamp Pause Menu
    public async void PublishLevel() {
        Level = await ServerLevelManager.PublishLevel(Level);
        await LocalLevelManager.SaveLevel(Level);
    }
    
    public void DeleteLevel() {
        LocalLevelManager.DeleteLevel(Level.id);
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