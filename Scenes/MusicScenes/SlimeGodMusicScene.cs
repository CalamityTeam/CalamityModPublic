using CalamityMod.NPCs.SlimeGod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class SlimeGodMusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;

        public override int NPCType => ModContent.NPCType<SlimeGodCore>();
        public override int? MusicModMusic => CalamityMod.Instance.GetMusicFromMusicMod("SlimeGod");
        public override int VanillaMusic => MusicID.Boss1;
        public override int OtherworldMusic => MusicID.OtherworldlyBoss1;
    }
}
