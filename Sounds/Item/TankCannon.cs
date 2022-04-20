using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Sounds.Item
{
    public class TankCannon : ModSound
    {
        public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan)
        {
            soundInstance = Sound.Value.CreateInstance();
            soundInstance.Volume = MathHelper.Clamp(volume * 1.1f, 0f, 1f);
            soundInstance.Pan = pan;
            soundInstance.Pitch = (float)Main.rand.Next(-25, 26) * 0.01f;
            SoundInstanceGarbageCollector.Track(soundInstance);
            return soundInstance;
        }
    }
}
