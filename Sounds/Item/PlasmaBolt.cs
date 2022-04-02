using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Sounds.Item
{
    public class PlasmaBolt : ModSound
    {
        public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan, SoundType type)
        {
            if (soundInstance.State == SoundState.Playing)
                soundInstance.Stop();
            soundInstance = sound.CreateInstance();
            soundInstance.Volume = volume * 0.8f;
            soundInstance.Pan = pan;
            Main.PlaySoundInstance(soundInstance);
            return soundInstance;
        }
    }
}
