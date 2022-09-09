using CalamityMod.Events;
using CalamityMod.NPCs;
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

        public override bool AdditionalCheck()
		{
			if (CalamityGlobalNPC.aquaticScourge == -1)
				return false;
			return Main.npc[CalamityGlobalNPC.aquaticScourge].justHit || Main.npc[CalamityGlobalNPC.aquaticScourge].life <= Main.npc[CalamityGlobalNPC.aquaticScourge].lifeMax * 0.999 || BossRushEvent.BossRushActive || Main.getGoodWorld;
		}
    }
}
