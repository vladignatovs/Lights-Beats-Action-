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

    public void ChangePosition(string value) {
        ActionLineManager.changePosition(value);
    }

    public void ChangeRotation(string value) {
        ActionLineManager.changeRotation(value);
    }

    public void ChangeScale(string value) {
        ActionLineManager.changeScale(value);
    }

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
        var _value = value.Replace(";","")+ ";" + ActionLineManager.Action.position.y;
        ActionLineManager.changePosition(_value);
    }
    /// <summary>
    /// Used to only change y parameter of the position property. 
    /// </summary>
    /// <param name="value"></param>
    public void ChangePositionY(string value) {
        var _value = ActionLineManager.Action.position.x + ";" + value.Replace(";","");
        ActionLineManager.changePosition(_value);
    }

    public void ChangeScaleX(string value) {
        var _value = value.Replace(";","")+ ";" + ActionLineManager.Action.scale.y;
        ActionLineManager.changeScale(_value);
    }

    public void ChangeScaleY(string value) {
        var _value = ActionLineManager.Action.scale.x + ";" + value.Replace(";","");
        ActionLineManager.changeScale(_value);
    }

    #endregion
    #region controllerMethods
    public void ChangePositionXValue(string value) {

        value = value.Replace(".", ","); ////////////////////

        if(float.TryParse(value.Replace(";",""), out float x)) {
            var position = ActionLineManager.Action.position;
            ActionLineManager.Action.position = new Vector3(x, position.y, position.z);
        }
        Debug.Log(ActionLineManager.Action.position.x);
    } 

    public void ChangePositionYValue(string value) {

        value = value.Replace(".", ","); ////////////////////

        if(float.TryParse(value.Replace(";",""), out float y)) {
            var position = ActionLineManager.Action.position;
            ActionLineManager.Action.position = new Vector3(position.x, y, position.z);
        }
        Debug.Log(ActionLineManager.Action.position.y);
    }

    public void ChangeScaleXValue(string value) {

        value = value.Replace(".", ","); ////////////////////

        if(float.TryParse(value.Replace(";",""), out float x)) {
            var position = ActionLineManager.Action.scale;
            ActionLineManager.Action.scale = new Vector3(x, position.y, position.z);
        }
        Debug.Log(ActionLineManager.Action.scale.x);
    }

    public void ChangeScaleYValue(string value) {

        value = value.Replace(".", ","); ////////////////////

        if(float.TryParse(value.Replace(";",""), out float y)) {
            var position = ActionLineManager.Action.scale;
            ActionLineManager.Action.scale = new Vector3(position.x, y, position.z);
        }
        Debug.Log(ActionLineManager.Action.scale.y);
    }

    public void AddOneToScaleYValue(bool isOn) {
        var scale = ActionLineManager.Action.scale;
        /* The states should be as following :
            1. = only possible when false
            2. = only possible when true
            3. = only possible when false
            4. = only possible when true */
        var scaleY = isOn 
            ? (scale.y == 1 ? 2 : (scale.y == 3 ? 4 : ((scale.y >= 1 && scale.y <= 4) ? scale.y : 1)))
            : (scale.y == 2 ? 1 : (scale.y == 4 ? 3 : ((scale.y >= 1 && scale.y <= 4) ? scale.y : 1)));
        ActionLineManager.Action.scale = new Vector3(scale.x, scaleY, scale.z);
        Debug.Log(ActionLineManager.Action.scale.y);
    }

    public void AddTwoToScaleYValue(bool isOn) {
        var scale = ActionLineManager.Action.scale;
        /* The states should be as following :
            1. = only possible when false
            2. = only possible when false
            3. = only possible when true
            4. = only possible when true */

        var scaleY = isOn 
            ? (scale.y == 1 ? 3 : (scale.y == 2 ? 4 : ((scale.y >= 1 && scale.y <= 4) ? scale.y : 1))) 
            : (scale.y == 3 ? 1 : (scale.y == 4 ? 2 : ((scale.y >= 1 && scale.y <= 4) ? scale.y : 1)));
        ActionLineManager.Action.scale = new Vector3(scale.x, scaleY, scale.z);

        Debug.Log(ActionLineManager.Action.scale.y);
    }

    public void MinusLayer() {
        ActionLineManager.MinusLayer();
    }
    public void PlusLayer() {
        ActionLineManager.PlusLayer();
    }
    #endregion
}
