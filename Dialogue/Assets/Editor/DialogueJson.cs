using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class DialogueJson
{
    public Dialogue[] Dialogues;
}

[System.Serializable]
public class Dialogue
{
    public string ID;
    public Statement[] Statements;
}

[System.Serializable]
public class Statement
{
    public string StringID;
    public bool LeftHand;
    public string Charactor;
}