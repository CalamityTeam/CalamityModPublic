using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Sounds.Item
{
    public class LaserCannon : ModSound
    {
        public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan, SoundType type)
        {
            soundInstance = sound.CreateInstance();
            soundInstance.Volume = volume * 0.85f;
            soundInstance.Pan = pan;
            Main.PlaySoundInstance(soundInstance);
            return soundInstance;
        }
    }
}
