using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.IO;

public class Json2Asset {

    private static string JSONFilePath = "Assets/JSON/dialogue.json";
    private static string AssetPath = "Assets/Dialogues/";
    private static string AssetExtention = ".asset";
    private static string AssetBundleName = "dialogue_content";
    private static string AssetBundlePath = "Assets/AssetBundles/";

    [MenuItem("Dialogue/ParseJSON")]
    static void ParseJson () {
        TextAsset json = AssetDatabase.LoadAssetAtPath<TextAsset>(JSONFilePath);
        var dialogues = JsonUtility.FromJson<DialogueJson>(json.text);

        foreach (var dialogue in dialogues.Dialogues)
        {
            var serializeDialog = ScriptableObject.CreateInstance<SerializableDialogue>();
            serializeDialog.Init();

            foreach (var statement in dialogue.Statements)
            {
                serializeDialog.StatementStart(statement.LeftHand, statement.Charactor);
                
                ParseRichText(statement.StringID, serializeDialog, statement.Charactor);

                serializeDialog.AddElementWithoutParam(
                    SerializableDialogue.DialogElementType.DET_StatementEnd);
            }

            AssetDatabase.CreateAsset(serializeDialog, 
                AssetPath + dialogue.ID + AssetExtention);

            AssetImporter.GetAtPath(AssetPath + dialogue.ID + AssetExtention)
                .SetAssetBundleNameAndVariant(AssetBundleName, "");

            Debug.Log("Convert " + dialogue.ID + " finish!");
        }

        Debug.Log("All json to assets");
        BuildAssetBundle();
    }

    [MenuItem("Dialogue/BuildAB")]
    static void BuildAssetBundle()
    {
        //Build Asset Bundles
        string assetBundleDirectory = AssetBundlePath
            + EditorUserBuildSettings.activeBuildTarget;
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory,
            BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
    }
	
	static void ParseRichText(string text, SerializableDialogue dialogState, string charactor)
    {
        var myRegExp = new Regex(@"\<(.*?)\>");

        MatchCollection matches = myRegExp.Matches(text);

        //Debug.Log("matches found " + matches.Count);

        int startIndex = 0;

        foreach (Match match in matches)
        {
            GroupCollection groups = match.Groups;
            var matchIndex = groups[0].Index;
            var matchText = groups[0].Value;

            if (startIndex == matchIndex)
            {
                startIndex += matchText.Length;
            }
            else
            {
                var context = text.Substring(startIndex, matchIndex - startIndex);
                //Debug.Log(context);
                dialogState.AddStringElement(
                    SerializableDialogue.DialogElementType.DET_Text, context);

                startIndex = matchIndex + matchText.Length;
            }
            //Debug.Log(matchText);

            // make sure there is no space in tag.
            var tag = groups[1].Value.Trim().Replace(" ", "");

            //Equal should compare before StartWith to avoid mis-match
            if (tag.Equals("input"))
            {
                dialogState.AddElementWithoutParam(
                    SerializableDialogue.DialogElementType.DET_Input);
            }
            else if (tag.Equals("b"))
            {
                dialogState.AddElementWithoutParam(
                    SerializableDialogue.DialogElementType.DET_BoldStart);
            }
            else if (tag.Equals("/b"))
            {
                dialogState.AddElementWithoutParam(
                    SerializableDialogue.DialogElementType.DET_BoldEnd);
            }
            else if (tag.Equals("i"))
            {
                dialogState.AddElementWithoutParam(
                    SerializableDialogue.DialogElementType.DET_ItalicStart);
            }
            else if (tag.Equals("/i"))
            {
                dialogState.AddElementWithoutParam(
                    SerializableDialogue.DialogElementType.DET_ItalicEnd);
            }
            else if (tag.Equals("hide"))
            {
                dialogState.AddElementWithoutParam(
                    SerializableDialogue.DialogElementType.DET_HideStart);
            }
            else if (tag.Equals("/hide"))
            {
                dialogState.AddElementWithoutParam(
                    SerializableDialogue.DialogElementType.DET_HideEnd);
            }
            else if (tag.StartsWith("color"))
            {
                dialogState.AddStringElement(
                    SerializableDialogue.DialogElementType.DET_ColorStart,
                    tag.Replace("color=", ""));
            }
            else if (tag.Equals("/color"))
            {
                dialogState.AddElementWithoutParam(
                    SerializableDialogue.DialogElementType.DET_ColorEnd);
            }
            else if (tag.StartsWith("size"))
            {
                int i = 0;
                int.TryParse(tag.Replace("size=", ""), out i);
                dialogState.AddIntElement(
                    SerializableDialogue.DialogElementType.DET_SizeStart,
                    i);
            }
            else if (tag.Equals("/size"))
            {
                dialogState.AddElementWithoutParam(
                    SerializableDialogue.DialogElementType.DET_SizeEnd);
            }
            else if (tag.StartsWith("speed"))
            {
                float f = 0;
                float.TryParse(tag.Replace("speed=", ""), out f);
                dialogState.AddFloatElement(
                    SerializableDialogue.DialogElementType.DET_SpeedStart,
                    f);
            }
            else if (tag.Equals("/speed"))
            {
                dialogState.AddElementWithoutParam(
                    SerializableDialogue.DialogElementType.DET_SpeedEnd);
            }
            else if (tag.StartsWith("wait"))
            {
                float f = 0;
                float.TryParse(tag.Replace("wait=", ""), out f);
                dialogState.AddFloatElement(
                    SerializableDialogue.DialogElementType.DET_Wait,
                    f);
            }
            else if (tag.StartsWith("shake"))
            {
                float f = 0;
                float.TryParse(tag.Replace("shake=", ""), out f);
                dialogState.AddFloatElement(
                    SerializableDialogue.DialogElementType.DET_Shake,
                    f);
            }
            else if (tag.StartsWith("emotion"))
            {
                var emotion = tag.Replace("emotion=", "");
                dialogState.AddEmotion(charactor, emotion);
            }
            else
            {
                Debug.LogError("The tag [" + tag + "] is not defined!");
            }
        }
    }
}
