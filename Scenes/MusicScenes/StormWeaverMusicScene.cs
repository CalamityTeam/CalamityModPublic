using CalamityMod.NPCs.StormWeaver;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class StormWeaverMusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override int NPCType => ModContent.NPCType<StormWeaverHead>();
        public override int? MusicModMusic => CalamityMod.Instance.GetMusicFromMusicMod("StormWeaver");
        public override int VanillaMusic => MusicID.Boss3;
        public override int OtherworldMusic => MusicID.OtherworldlyBoss2;
        public override int[] AdditionalNPCs => new int[]
		{
			ModContent.NPCType<StormWeaverBody>(),
			ModContent.NPCType<StormWeaverTail>()
		};
    }
}
