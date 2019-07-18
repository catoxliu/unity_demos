using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum DialogCharactorName
{
    DCN_Floom,
    DCN_Klaus,
    DCN_Kol,
    DCN_Shopkeeper,
    DCN_END,
}

public enum DialogCharactorEmotion
{
    DCE_angry,
    DCE_confused,
    DCE_eyeroll,
    DCE_happy,
    DCE_scared,
    DCE_determined,
    DCE_laughing,
    DCE_sad,
    DCE_shocked,
    DCE_depressed,
    DCE_END,
}

public class SerializableDialogue : ScriptableObject
{
	public enum DialogElementType
    {
        DET_Text,
        DET_ColorStart,
        DET_ColorEnd,
        DET_BoldStart,
        DET_BoldEnd,
        DET_ItalicStart,
        DET_ItalicEnd,
        DET_SizeStart,
        DET_SizeEnd,
        DET_SpeedStart,
        DET_SpeedEnd,
        DET_Wait,
        DET_Shake,
        DET_Input,
        DET_HideStart,
        DET_HideEnd,
        DET_Emotion,
        DET_StatementStart, //use this for a new statement start.
        DET_StatementEnd,   //use this for a statement finish.
        DET_End
    }

    public List<DialogElementType> DialogElements;
    public List<int> DialogElementsParamIndex;

    public List<string> DialogStringParams;
    public List<float> DialogFloatParams;

    public List<bool> LeftHands;
    public List<DialogCharactorName> Charactors;

    public List<Sprite> DialogEmotions;

#if UNITY_EDITOR
    public void AddStringElement(DialogElementType type, string text)
    {
        DialogElements.Add(type);
        DialogStringParams.Add(text);
        DialogElementsParamIndex.Add(DialogStringParams.Count - 1);
    }

    public void AddElementWithoutParam(DialogElementType type)
    {
        DialogElements.Add(type);
        DialogElementsParamIndex.Add(0);
    }

    public void AddFloatElement(DialogElementType type, float f)
    {
        DialogElements.Add(type);
        DialogFloatParams.Add(f);
        DialogElementsParamIndex.Add(DialogFloatParams.Count - 1);
    }

    public void AddIntElement(DialogElementType type, int i)
    {
        DialogElements.Add(type);
        DialogElementsParamIndex.Add(i);
    }

    public void Init()
    {
        DialogElements = new List<DialogElementType>();
        DialogElementsParamIndex = new List<int>();
        DialogFloatParams = new List<float>();
        DialogStringParams = new List<string>();
        LeftHands = new List<bool>();
        Charactors = new List<DialogCharactorName>();
        DialogEmotions = new List<Sprite>();
    }

    public void StatementStart(bool left, string name)
    {
        LeftHands.Add(left);
        Charactors.Add(GetCharactor(name));
        DialogElements.Add(DialogElementType.DET_StatementStart);
        DialogElementsParamIndex.Add(LeftHands.Count - 1);
        AddEmotion(name, string.Empty);
    }

    public void AddEmotion(string charactor_name, string emotion)
    {
        DialogElements.Add(DialogElementType.DET_Emotion);
        DialogEmotions.Add(GetEmotion(charactor_name, emotion));
        DialogElementsParamIndex.Add(DialogEmotions.Count - 1);
    }

    public Sprite GetEmotion(string charactor_name, string emotion)
    {
        var guids = AssetDatabase.FindAssets("_" + emotion,
            new [] { "Assets/Sprites/characters/"+charactor_name } );
        Sprite sprite = null;
        foreach(var guid in guids)
        {
            sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
                AssetDatabase.GUIDToAssetPath(guid));
            if (sprite != null)
                return sprite;
        }
        Debug.LogError("Can't find charactor [" + charactor_name
                + "] emotion [" + emotion + "] !");
        return null;
    }

    public DialogCharactorName GetCharactor(string name)
    {
        switch (name)
        {
            case "floom":
                return DialogCharactorName.DCN_Floom;
            case "klaus":
                return DialogCharactorName.DCN_Klaus;
            case "kol":
                return DialogCharactorName.DCN_Kol;
            case "shopkeeper":
                return DialogCharactorName.DCN_Shopkeeper;
            default:
                return DialogCharactorName.DCN_END;
        }
    }

#endif
}
