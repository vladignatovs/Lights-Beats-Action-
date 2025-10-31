using Unity.VisualScripting;
using UnityEngine;

public class SettingsPanelManager : MonoBehaviour {
    public ActionLineManager ActionLineManager;

    #region defaultMethods
    public void ChangeBeat(string value) {
        ActionLineManager.changeBeat(value);
    }
    
    public void ChangeTimes(string value) {
        ActionLineManager.changeTimes(value);
    }
    
    public void ChangeDelay(string value) {
        ActionLineManager.changeDelay(value);
    }

    public void ChangeObject(int value) {
        ActionLineManager.changeObject(value);
    }
    // TODO:
    // public void ChangePosition(string value) {
    //     ActionLineManager.changePosition(value);
    // }

    public void ChangeRotation(string value) {
        ActionLineManager.changeRotation(value);
    }

    // TODO
    // public void ChangeScale(string value) {
    //     ActionLineManager.changeScale(value);
    // }

    public void ChangeAnimationDuration(string value) {
        ActionLineManager.changeAnimationDuration(value);
    }
    
    public void ChangeLifeTime(string value) {
        ActionLineManager.changeLifeTime(value);
    }

    public void AddGroup(string value) {
        ActionLineManager.addGroup(value);
    }

    public void DeleteGroup(int value) {
        ActionLineManager.deleteGroup(value);
    }

    public void CloneAction() {
        ActionLineManager.cloneAction();
    }

    public void DeleteAction() {
        ActionLineManager.deleteAction();
    }

    public void ShowTimesClones(bool value) {
        ActionLineManager.ShowTimesClones(value);
    }

    public void ShowLifeTimeLine(bool value) {
        ActionLineManager.ShowLifeTimeLine(value);
    }

    public void SetLayer(string value) {
        ActionLineManager.SetLayer(value);
    }
    #endregion
    #region customMethods
    /// <summary>
    /// Used to only change x parameter of the position property. 
    /// </summary>
    /// <param name="value"></param>
    public void ChangePositionX(string value) {
        if (float.TryParse(value, out var result)) {
            Debug.Log(result);
            ActionLineManager.changePosition(result, ActionLineManager.Action.PositionY);
        } 
        else
            Debug.Log("Couldnt parse this value: " + value);
    }
    /// <summary>
    /// Used to only change y parameter of the position property. 
    /// </summary>
    /// <param name="value"></param>
    public void ChangePositionY(string value) {
        if (float.TryParse(value, out var result))
            ActionLineManager.changePosition(ActionLineManager.Action.PositionX, result);
        else
            Debug.Log("Couldnt parse this value: " + value);
    }

    // TODO
    public void ChangeScaleX(string value) {
        if (float.TryParse(value, out var result))
            ActionLineManager.changeScale(result, ActionLineManager.Action.ScaleY);
        else
            Debug.Log("Couldnt parse this value: " + value);
    }

    public void ChangeScaleY(string value) {
        if (float.TryParse(value, out var result))
            ActionLineManager.changeScale(ActionLineManager.Action.ScaleX, result);
        else
            Debug.Log("Couldnt parse this value: " + value);
    }

    #endregion
    #region controllerMethods
    public void ChangePositionXValue(string value) {

        value = value.Replace(".", ","); ////////////////////

        if(float.TryParse(value.Replace(";",""), out float x)) {
            ActionLineManager.Action.PositionX = x; 
        }
        Debug.Log(ActionLineManager.Action.PositionX);
    } 

    public void ChangePositionYValue(string value) {

        value = value.Replace(".", ","); ////////////////////

        if(float.TryParse(value.Replace(";",""), out float y)) {
            ActionLineManager.Action.PositionY = y;
        }
        Debug.Log(ActionLineManager.Action.PositionY);
    }

    public void ChangeScaleXValue(string value) {

        value = value.Replace(".", ","); ////////////////////

        if(float.TryParse(value.Replace(";",""), out float x)) {
            ActionLineManager.Action.ScaleX = x;
        }
        Debug.Log(ActionLineManager.Action.ScaleX);
    }

    public void ChangeScaleYValue(string value) {

        value = value.Replace(".", ","); ////////////////////

        if(float.TryParse(value.Replace(";",""), out float y)) {
            ActionLineManager.Action.ScaleY = y;
        }
        Debug.Log(ActionLineManager.Action.ScaleY);
    }

    public void AddOneToScaleYValue(bool isOn) {
        var y = ActionLineManager.Action.ScaleY;
        /* The states should be as following :
            1. = only possible when false
            2. = only possible when true
            3. = only possible when false
            4. = only possible when true */
        var scaleY = isOn 
            ? (y == 1 ? 2 : (y == 3 ? 4 : ((y >= 1 && y <= 4) ? y : 1)))
            : (y == 2 ? 1 : (y == 4 ? 3 : ((y >= 1 && y <= 4) ? y : 1)));
        ActionLineManager.Action.ScaleY = scaleY;
        Debug.Log(ActionLineManager.Action.ScaleY);
    }

    public void AddTwoToScaleYValue(bool isOn) {
        var y = ActionLineManager.Action.ScaleY;
        /* The states should be as following :
            1. = only possible when false
            2. = only possible when false
            3. = only possible when true
            4. = only possible when true */

        var scaleY = isOn 
            ? (y == 1 ? 3 : (y == 2 ? 4 : ((y >= 1 && y <= 4) ? y : 1))) 
            : (y == 3 ? 1 : (y == 4 ? 2 : ((y >= 1 && y <= 4) ? y : 1)));
        ActionLineManager.Action.ScaleY = scaleY;

        Debug.Log(ActionLineManager.Action.ScaleY);
    }

    public void MinusLayer() {
        ActionLineManager.MinusLayer();
    }
    public void PlusLayer() {
        ActionLineManager.PlusLayer();
    }
    #endregion
}
