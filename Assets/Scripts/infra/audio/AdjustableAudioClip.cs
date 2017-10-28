using UnityEngine;
using System;

namespace Infra.Audio {
[Serializable]
public class AdjustableAudioClip {
    public float volume = 1.0f;
    [Tooltip("If >0 will stop playing the clip after that many seconds")]
    public float stopAt = 0f;
    public AudioClip clip;
}
}
