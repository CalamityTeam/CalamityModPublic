using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Sounds.NPCKilled
{
    public class AstralEnemyDeath : ModSound
    {
        public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan, SoundType type)
        {
            soundInstance = sound.CreateInstance();
            soundInstance.Pan = pan;
            soundInstance.Volume = volume * 0.7f;
            Main.PlaySoundInstance(soundInstance);
            return soundInstance;
        }
    }
}
