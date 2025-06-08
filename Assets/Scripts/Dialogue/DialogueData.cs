using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "NewDialogueData", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public DialogueLine[] dialogueLines;

    public CutsceneImages[] cutsceneImages;

    public CutsceneAudio[] cutsceneAudios;
    public AudioClip backgroundMusic;
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
    public Sprite secondaryImage;
    public float alternateDuration;
    public bool isAutoplay;
    public float autoplayDuration;
}

[Serializable]
public class CutsceneAudio
{
    public int dialogueStartIndex;
    public AudioClip audio;
    public float delay = 0;
}


