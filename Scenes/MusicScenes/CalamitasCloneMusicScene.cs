using CalamityMod.NPCs.CalClone;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class CalamitasCloneMusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override int NPCType => ModContent.NPCType<CalamitasClone>();
        public override int? MusicModMusic => CalamityMod.Instance.GetMusicFromMusicMod("CalamitasClone");
        public override int VanillaMusic => MusicID.Boss2;
        public override int OtherworldMusic => MusicID.OtherworldlyBoss2;
        public override int[] AdditionalNPCs => new int[]
		{
			ModContent.NPCType<Cataclysm>(),
			ModContent.NPCType<Catastrophe>()
		};
    }
}
