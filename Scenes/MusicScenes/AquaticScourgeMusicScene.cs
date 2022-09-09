using CalamityMod.Events;
using CalamityMod.NPCs.AquaticScourge;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class AquaticScourgeMusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override int NPCType => ModContent.NPCType<AquaticScourgeHead>();
        public override int? MusicModMusic => CalamityMod.Instance.GetMusicFromMusicMod("AquaticScourge");
        public override int VanillaMusic => MusicID.Boss2;
        public override int OtherworldMusic => MusicID.OtherworldlyBoss2;
        public override bool AdditionalCheck => BossRushEvent.BossRushActive || Main.getGoodWorld;
        public override bool AdditionalCheckNPC(int j) => Main.npc[j].justHit || Main.npc[j].life <= Main.npc[j].lifeMax * 0.999;
    }
}
