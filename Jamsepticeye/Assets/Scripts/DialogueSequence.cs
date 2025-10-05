using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Sequence", fileName = "NewDialogueSequence")]
public class DialogueSequence : ScriptableObject
{
    [Tooltip("Optional id for addressing via code")]
    public string sequenceId;

    [TextArea(2, 6)]
    public string[] lines;

    [Header("Optional voice clips (same order/length as lines)")]
    public AudioClip[] voiceClips;

    [Header("Options")]
    public bool waitForVoiceToFinish = true;
    public float postVoicePause = 0.25f;
}
