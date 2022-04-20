using Terraria.Audio;
using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Sounds.Item
{
    public class PulseRifleFire : ModSound
    {
        public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan)
        {
            soundInstance = Sound.Value.CreateInstance();
            soundInstance.Volume = volume * 1f;
            soundInstance.Pan = pan;
            SoundInstanceGarbageCollector.Track(soundInstance);
            return soundInstance;
        }
    }
}
