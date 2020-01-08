using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Sounds.Custom
{
    public class MMMMMMMMMMMMM : ModSound
    {
        public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan, SoundType type)
        {
            soundInstance = sound.CreateInstance();
            soundInstance.Volume = volume * 1f;
            soundInstance.Pan = pan;
            soundInstance.IsLooped = true;
            Main.PlaySoundInstance(soundInstance);
            return soundInstance;
        }
    }
}
