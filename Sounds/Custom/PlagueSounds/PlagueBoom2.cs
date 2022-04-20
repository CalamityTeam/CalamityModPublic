using Terraria.Audio;
using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Sounds.Custom.PlagueSounds
{
    public class PlagueBoom2 : ModSound
    {
        public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan)
        {
            soundInstance = Sound.Value.CreateInstance();
            soundInstance.Volume = volume;
            soundInstance.Pan = pan;
            SoundInstanceGarbageCollector.Track(soundInstance);
            return soundInstance;
        }
    }
}
