using Terraria.Audio;
using Terraria.Audio;
using Terraria.Audio;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Audio;

namespace CalamityMod.Sounds.Item
{
    public class IceBarrageCast : ModSound
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
