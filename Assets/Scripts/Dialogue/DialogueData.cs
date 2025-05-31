using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "NewDialogueData", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public DialogueLine[] dialogueLines;

    public CutsceneImages[] cutsceneImages;

    public CutsceneAudio[] cutsceneAudios;
}

[Serializable]
public class DialogueLine
{
    public string speakerName;
    public string line;
}

[Serializable]
public class CutsceneImages
{
    public int dialogueStartIndex;
    public Sprite image;
}

[Serializable]
public class CutsceneAudio
{
    public int dialogueStartIndex;
    public AudioClip audio;
}


