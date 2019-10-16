using Microsoft.Xna.Framework.Audio;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Sounds.Custom
{
    public class AtlasIdle0 : ModSound
    {
        public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan, SoundType type)
        {
            soundInstance = sound.CreateInstance();
            soundInstance.Pan = pan;
            soundInstance.Volume = volume * 0.6f;
            Main.PlaySoundInstance(soundInstance);
            return soundInstance;
        }
    }
}
