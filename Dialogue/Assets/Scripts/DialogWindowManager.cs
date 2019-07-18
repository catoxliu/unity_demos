using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogWindowManager : MonoBehaviour {

    public DialogWindow CurrentWindow;
    public DialogWindow PreviousWindow;

    public Text CurrentText;
    public Text PreviousText;

    public DialogStatementGenerator DialogGenerator;

    public UnityAction DialogFinishAction;

    private bool isCurrentLeftHand, isPrevious;

    // Use this for initialization
    void Start()
    {
        DialogGenerator.StatementStartAction += StatementStart;
        DialogGenerator.EmotionAction += ChangeEmotion;
        DialogGenerator.DialogFinishAction += DialogFinish;
        DialogGenerator.StatementFinishAction += StatementEnd;
        DialogGenerator.ShakeAction += Shake;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
            || Input.GetMouseButtonDown(0))
        {
            DialogGenerator.UserInput();
            CurrentWindow.UserInput();
        }
    }

    void OnDestroy()
    {
        DialogGenerator.StatementStartAction -= StatementStart;
        DialogGenerator.EmotionAction -= ChangeEmotion;
        DialogGenerator.DialogFinishAction -= DialogFinish;
        DialogGenerator.StatementFinishAction -= StatementEnd;
        DialogGenerator.ShakeAction -= Shake;
    }

    public void StartDialog(SerializableDialogue content)
    {
        CurrentWindow.Show();
        DialogGenerator.SerializableDialogues = content;
        DialogGenerator.Generate();
    }

    void StatementStart(bool isLeftHand)
    {
        if (isPrevious) SwapStatements();

        isCurrentLeftHand = isLeftHand;
        if (isLeftHand)
        {
            CurrentWindow.SetLeftHand();
        }
        else
        {
            CurrentWindow.SetRightHand();
        }
        CurrentWindow.Show();

        AudioManager.Instance.Play();
    }

    void StatementEnd()
    {
        isPrevious = true;
        PreviousText.text = CurrentText.text;
    }

    void ChangeEmotion(Sprite emotion)
    {
        if (emotion != null)
        {
            CurrentWindow.Charactor.sprite = emotion;
        }
    }

    void Shake(float seconds)
    {
        CurrentWindow.ShakeWindow(seconds);
    }

    void DialogFinish()
    {
        CurrentWindow.Hide();
        PreviousWindow.Hide();
        isPrevious = false;
        if (DialogFinishAction != null)
            DialogFinishAction();
    }

    void SwapStatements()
    {
        PreviousWindow.Show();
        
        if (isCurrentLeftHand)
        {
            PreviousWindow.SetLeftHand();
        }
        else
        {
            PreviousWindow.SetRightHand();
        }
        PreviousWindow.Charactor.sprite = CurrentWindow.Charactor.sprite;
    }

}
