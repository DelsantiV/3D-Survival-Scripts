using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Unity.IO.LowLevel.Unsafe;
using System.IO;
using System.Text;
public class PlayerInputConfig : ISaveable
{

    private List<ControlInput> controls;
    private string InputConfigJsonPath = "PlayerInputConfig.json";

    public PlayerInputConfig() 
    {
        controls = new List<ControlInput>();
        LoadFromJson();
    }

    public ControlInput GetControlFromKeyCode(KeyCode keyCode)
    {
        return controls.Find(cI => cI.keyCode == keyCode);
    }

    public ControlInput GetControlFromControlType(Controls controlType)
    {
        return controls.Find(cI => cI.controlType == controlType);
    }

    public KeyCode GetKeyCodeForControl(Controls controlType)
    {
        ControlInput controlInput = GetControlFromControlType(controlType);
        if (controlInput != null) { return controlInput.keyCode; }
        return KeyCode.None;
    }

    public void RegisterControlInput(Controls controlType, KeyCode keyCode, bool overwrite = true, bool hardcordOverwrite = false)
    {
        ControlInput controlInput = new(controlType, keyCode);
        ControlInput controlWithSameType = GetControlFromControlType(controlType);
        ControlInput controlWithSameKey = GetControlFromKeyCode(keyCode);

        if (controlWithSameKey == null && controlWithSameType == null)
        {
            Debug.Log("Creating new binding between keycode " + keyCode + " and control " + controlType);
            controls.Add(controlInput);
            return;
        }
        
        if (controlWithSameType.IsIdenticalTo(controlWithSameKey))
        {
            Debug.Log("Key " + keyCode + " is already affected to control " + controlType);
            return;
        }

        if (controlWithSameKey == null)
        {
            if (overwrite)
            {
                controlWithSameType.keyCode = keyCode;
                Debug.Log("Successfully change keycode for " + controlType + " to " + keyCode);
            }
        }

        if (controlWithSameType == null)
        {
            if (overwrite)
            {
                controlWithSameKey.controlType = controlType;
                Debug.Log("Successfully change control for keycode " + keyCode + " to " + controlType);
            }
        }

        if (hardcordOverwrite)
        {
            controlWithSameType.controlType = controlWithSameKey.controlType;
            controlWithSameKey.controlType = controlType;
        }

        Debug.Log("Keycodes and Control are already attributed");
    }

    public void DeleteControlInput(ControlInput controlInput)
    {
        ControlInput cI = controls.Find(x => x.IsIdenticalTo(controlInput));
        if (cI == null) 
        {
            Debug.Log("No such control input registered, cannot delete");
            return; 
        }
        else
        {
            controls.Remove(cI);
        }
    }

    private void InitializeFromJSON(TextAsset jsonFile)
    {
        if (jsonFile != null)
        {
            JObject jsonParsedFile = JObject.Parse(jsonFile.text);
            CreateAllControlsFromJSON(jsonParsedFile);
        }
    }
    private void InitializeFromJSON(string jsonFile)
    {
        if (jsonFile != null)
        {
            JObject jsonParsedFile = JObject.Parse(jsonFile);
            CreateAllControlsFromJSON(jsonParsedFile);
        }
    }

    private void CreateAllControlsFromJSON(JObject jsonParsedFile)
    {
        foreach (Controls control in Enum.GetValues(typeof(Controls)))
        {
            KeyCode keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), jsonParsedFile[control.ToString()].ToString());
            if (keyCode != KeyCode.None)
            {
                RegisterControlInput(control, keyCode);
            }
        }
        Debug.Log("Successfully loaded key bindings !");
    }

    private void OnBindingsLoadingCompleted(AsyncOperationHandle<TextAsset> bindingsLoading)
    {
        if (bindingsLoading.Status == AsyncOperationStatus.Succeeded)
        {
            InitializeFromJSON(bindingsLoading.Result);
        }
        Addressables.Release(bindingsLoading);
    }

    public void SaveToJson()
    {
        string dataPath = Application.dataPath + "/Saves/";
        if (!File.Exists(dataPath)) { Directory.CreateDirectory(dataPath); }
        Dictionary<string, string> savedBindings = new();
        foreach(ControlInput controlInput in controls)
        {
            savedBindings.TryAdd(controlInput.controlType.ToString(), controlInput.keyCode.ToString());
        }
        string json = JsonConvert.SerializeObject(savedBindings, Formatting.Indented);
        File.WriteAllText(dataPath + InputConfigJsonPath, json);
    }

    public void LoadFromJson()
    {
        string dataPath = Application.dataPath + "/Saves/";
        if (File.Exists(dataPath + InputConfigJsonPath))
        {
            InitializeFromJSON(File.ReadAllText(dataPath + InputConfigJsonPath));
        }

        else
        {
            AsyncOperationHandle<TextAsset> bindingsLoading = Addressables.LoadAssetAsync<TextAsset>(InputConfigJsonPath);
            bindingsLoading.Completed += OnBindingsLoadingCompleted;
        }
    }
}

public class ControlInput
{
    public Controls controlType;
    public KeyCode keyCode;
    public ControlInput(Controls controlType, KeyCode keyCode)
    {
        this.controlType = controlType;
        this.keyCode = keyCode;
    }

    public bool IsIdenticalTo(ControlInput other)
    {
        if (other.controlType == controlType && other.keyCode == keyCode) { return true; }
        return false;
    }

    public override string ToString()
    {
        return "Key " + keyCode.ToString() + " binded  to control " + controlType.ToString();
    }
}

public enum Controls
{
    Collect,
    Jump,
    Sprint,
    MoveForward,
    MoveBackward,
    MoveRight,
    MoveLeft,
    ChangeWalkMode,
    PrefHandAction,
    OtherHandAction,
    GetPrefHandPile,
    GetOtherHandPile,
    SwitchHandMode
}