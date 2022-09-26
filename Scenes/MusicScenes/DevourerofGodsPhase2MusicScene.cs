using CalamityMod.NPCs;
using CalamityMod.NPCs.DevourerofGods;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class DevourerofGodsPhase2MusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override int NPCType => ModContent.NPCType<DevourerofGodsHead>();
        public override int? MusicModMusic => CalamityMod.Instance.GetMusicFromMusicMod("DevourerOfGodsP2");
        public override int VanillaMusic => MusicID.LunarBoss;
        public override int OtherworldMusic => MusicID.OtherworldlyLunarBoss;
        public override int[] AdditionalNPCs => new int[]
		{
			ModContent.NPCType<DevourerofGodsBody>(),
			ModContent.NPCType<DevourerofGodsTail>()
		};

        public override bool AdditionalCheck() => CalamityGlobalNPC.DoGP2 != -1;
    }
}
