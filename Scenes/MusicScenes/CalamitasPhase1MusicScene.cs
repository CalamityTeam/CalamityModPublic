using CalamityMod.NPCs;
using CalamityMod.NPCs.SupremeCalamitas;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class CalamitasPhase1MusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override int NPCType => ModContent.NPCType<SupremeCalamitas>();
        public override int? MusicModMusic => CalamityMod.Instance.GetMusicFromMusicMod("CalamitasPhase1");
        public override int VanillaMusic => MusicID.Boss2;
        public override int OtherworldMusic => MusicID.OtherworldlyBoss2;

        public override bool AdditionalCheck() => CalamityGlobalNPC.SCalGrief != -1;
    }
}
