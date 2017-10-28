using System;
using Infra.Utils;

namespace Infra.Audio {
[Serializable]
/// <summary>
/// Used to play a random sound from a list of sounds.
/// Does not do actual pooling...
/// </summary>
public class SoundPool {
    public AdjustableAudioClip[] clips;

    public void Play() {
        AdjustableAudioClip clip;
        if (clips.Length == 1) {
            clip = clips[0];
        } else {
            clip = RandomUtils.ChooseRandom(clips);
        }
        AudioManager.PlayClip(clip);
    }
}
}
