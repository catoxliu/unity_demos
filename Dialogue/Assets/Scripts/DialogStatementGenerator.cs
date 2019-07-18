using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogStatementGenerator : MonoBehaviour {
    
    public SerializableDialogue SerializableDialogues;
    public Text DialogStatUIText;

    public UnityAction<bool> StatementStartAction;
    public UnityAction<Sprite> EmotionAction;
    public UnityAction DialogFinishAction;
    public UnityAction StatementFinishAction;
    public UnityAction<float> ShakeAction;

    private float wordSpeed = 0.04f;
    private bool isWaitInput, isSkip, isHide, isWaitSeconds;
    private string closeTagBuffer = string.Empty;

    private readonly string CLOSETAG_BOLD = "</b>";
    private readonly string CLOSETAG_ITALIC = "</i>";
    private readonly string CLOSETAG_COLOR = "</color>";
    private readonly string CLOSETAG_SIZE = "</size>";

    public void Generate()
    {
        StartCoroutine(GenerateCoroutine());
    }

	IEnumerator GenerateCoroutine () {
		for (int i = 0, max = SerializableDialogues.DialogElements.Count; i < max;i++)
        {
            switch(SerializableDialogues.DialogElements[i])
            {
                case SerializableDialogue.DialogElementType.DET_SpeedStart:
                    wordSpeed = SerializableDialogues.DialogFloatParams[
                        SerializableDialogues.DialogElementsParamIndex[i]];
                    break;
                case SerializableDialogue.DialogElementType.DET_BoldStart:
                    DialogStatUIText.text += "<b>";
                    closeTagBuffer = CLOSETAG_BOLD + closeTagBuffer;
                    break;
                case SerializableDialogue.DialogElementType.DET_BoldEnd:
                    DialogStatUIText.text += CLOSETAG_BOLD;
                    closeTagBuffer = closeTagBuffer.Replace(CLOSETAG_BOLD, "");
                    break;
                case SerializableDialogue.DialogElementType.DET_ItalicStart:
                    DialogStatUIText.text += "<i>";
                    closeTagBuffer = CLOSETAG_ITALIC + closeTagBuffer;
                    break;
                case SerializableDialogue.DialogElementType.DET_ItalicEnd:
                    DialogStatUIText.text += CLOSETAG_ITALIC;
                    closeTagBuffer = closeTagBuffer.Replace(CLOSETAG_ITALIC, "");
                    break;
                case SerializableDialogue.DialogElementType.DET_ColorStart:
                    DialogStatUIText.text += "<color="+
                        SerializableDialogues.DialogStringParams[
                        SerializableDialogues.DialogElementsParamIndex[i]]+">";
                    closeTagBuffer = CLOSETAG_COLOR + closeTagBuffer;
                    break;
                case SerializableDialogue.DialogElementType.DET_ColorEnd:
                    DialogStatUIText.text += CLOSETAG_COLOR;
                    closeTagBuffer = closeTagBuffer.Replace(CLOSETAG_COLOR, "");
                    break;
                case SerializableDialogue.DialogElementType.DET_SizeStart:
                    DialogStatUIText.text += "<size=" +
                        SerializableDialogues.DialogElementsParamIndex[i] + ">";
                    closeTagBuffer = CLOSETAG_SIZE + closeTagBuffer;
                    break;
                case SerializableDialogue.DialogElementType.DET_SizeEnd:
                    DialogStatUIText.text += CLOSETAG_SIZE;
                    closeTagBuffer = closeTagBuffer.Replace(CLOSETAG_SIZE, "");
                    break;
                case SerializableDialogue.DialogElementType.DET_Text:
                    if (isHide)
                        break;
                    if (isSkip)
                    {
                        DialogStatUIText.text +=
                            SerializableDialogues.DialogStringParams[
                            SerializableDialogues.DialogElementsParamIndex[i]];
                    }
                    else
                    {
                        yield return WritingWord(
                        SerializableDialogues.DialogStringParams[
                            SerializableDialogues.DialogElementsParamIndex[i]]);
                    }
                    break;
                case SerializableDialogue.DialogElementType.DET_Input:
                    isWaitInput = true;
                    yield return WaitForInput();
                    break;
                case SerializableDialogue.DialogElementType.DET_Wait:
                    isWaitSeconds = true;
                    yield return WaitForSeconds(
                        SerializableDialogues.DialogFloatParams[
                            SerializableDialogues.DialogElementsParamIndex[i]]);
                    break;
                case SerializableDialogue.DialogElementType.DET_StatementEnd:
                    if (StatementFinishAction != null)
                        StatementFinishAction();
                    break;
                case SerializableDialogue.DialogElementType.DET_StatementStart:
                    DialogStatUIText.text = string.Empty;
                    isSkip = false;
                    if (StatementStartAction != null)
                        StatementStartAction(
                            SerializableDialogues.LeftHands[
                            SerializableDialogues.DialogElementsParamIndex[i]]);
                    break;
                case SerializableDialogue.DialogElementType.DET_HideStart:
                    isHide = true;
                    break;
                case SerializableDialogue.DialogElementType.DET_HideEnd:
                    isHide = false;
                    break;
                case SerializableDialogue.DialogElementType.DET_Emotion:
                    if (EmotionAction != null)
                        EmotionAction(
                            SerializableDialogues.DialogEmotions[
                            SerializableDialogues.DialogElementsParamIndex[i]]);
                    break;
                case SerializableDialogue.DialogElementType.DET_Shake:
                    var seconds = SerializableDialogues.DialogFloatParams[
                            SerializableDialogues.DialogElementsParamIndex[i]];
                    if (ShakeAction != null)
                        ShakeAction(seconds);
                    isWaitSeconds = true;
                    yield return WaitForSeconds(seconds);
                    break;
                default:
                    break;
            }
        }

        if (DialogFinishAction != null)
            DialogFinishAction();

    }

    public void UserInput()
    {
        if (isWaitSeconds)
        {
            isWaitSeconds = false;
        }
        else
        {
            isWaitInput = false;
            isSkip = true;
        }
    }

    IEnumerator WaitForInput()
    {
        yield return null;
        while (isWaitInput)
        {
            yield return null;
        }
    }

    IEnumerator WaitForSeconds(float seconds)
    {
        float passTime = 0;
        while (isWaitSeconds)
        {
            yield return null;
            passTime += Time.deltaTime;
            if (passTime >= seconds)
                break;
        }
        isWaitSeconds = false;
    }

    IEnumerator WritingWord(string text)
    {
        int max = text.Length;
        float wordCount = 0;
        var backupText = DialogStatUIText.text;

        while (true)
        {
            if (isSkip)
                break;

            wordCount += Time.deltaTime / wordSpeed;

            if (wordCount > max)
                break;

            if (wordCount >= 1.0f)
            {
                DialogStatUIText.text += text.Substring(0, Mathf.FloorToInt(wordCount));
            }
            DialogStatUIText.text += closeTagBuffer;

            yield return null;

            DialogStatUIText.text = backupText;
        }
        DialogStatUIText.text = backupText + text;
    }

}
