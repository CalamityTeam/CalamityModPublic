using CalamityMod.NPCs.Cryogen;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class CryogenMusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override int NPCType => ModContent.NPCType<Cryogen>();
        public override int? MusicModMusic => CalamityMod.Instance.GetMusicFromMusicMod("Cryogen");
        public override int VanillaMusic => MusicID.FrostMoon;
        public override int OtherworldMusic => MusicID.OtherworldlyIce;
    }
}
