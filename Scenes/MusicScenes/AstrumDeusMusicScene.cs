using CalamityMod.NPCs.AstrumDeus;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class AstrumDeusMusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override int NPCType => ModContent.NPCType<AstrumDeusHead>();
        public override int? MusicModMusic => CalamityMod.Instance.GetMusicFromMusicMod("AstrumDeus");
        public override int VanillaMusic => MusicID.Boss3;
        public override int OtherworldMusic => MusicID.OtherworldlyBoss2;
        public override int[] AdditionalNPCs => new int[]
		{
			ModContent.NPCType<AstrumDeusBody>(),
			ModContent.NPCType<AstrumDeusTail>()
		};
    }
}
