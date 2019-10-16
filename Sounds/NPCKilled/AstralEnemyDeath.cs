using Microsoft.Xna.Framework.Audio;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

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
