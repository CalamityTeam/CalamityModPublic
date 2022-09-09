using CalamityMod.NPCs.Perforator;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class PerforatorMusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;

        public override int NPCType => ModContent.NPCType<PerforatorHive>();
        public override int? MusicModMusic => CalamityMod.Instance.GetMusicFromMusicMod("Perforators");
        public override int VanillaMusic => MusicID.Boss2;
        public override int OtherworldMusic => MusicID.OtherworldlyWoF;
    }
}
