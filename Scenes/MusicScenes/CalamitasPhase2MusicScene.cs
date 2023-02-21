using CalamityMod.NPCs;
using CalamityMod.NPCs.SupremeCalamitas;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class CalamitasPhase2MusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override int NPCType => ModContent.NPCType<SupremeCalamitas>();
        public override int? MusicModMusic => CalamityMod.Instance.GetMusicFromMusicMod("CalamitasPhase2");
        public override int VanillaMusic => MusicID.Boss3;
        public override int OtherworldMusic => MusicID.OtherworldlyWoF;

        public override bool AdditionalCheck() => CalamityGlobalNPC.SCalLament != -1;
    }
}
