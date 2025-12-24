using UnityEngine;

[System.Serializable]
public class BossReactionData
{
    public BossReactionComponent.BossReactionType reactionType;

    public AudioClip reactionClip;
    public string    reactionText;
    public Color     reactionColor;
    public bool      doGlitch;
}