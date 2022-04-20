using Terraria.Audio;
using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Sounds.NPCHit
{
    public class AstrumAureusHit : ModSound
    {
        public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan)
        {
            soundInstance = Sound.Value.CreateInstance();
            soundInstance.Volume = volume * 0.7f;
            soundInstance.Pan = pan;
            SoundInstanceGarbageCollector.Track(soundInstance);
            return soundInstance;
        }
    }
}
