using CalamityMod.NPCs;
using CalamityMod.NPCs.SupremeCalamitas;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class SCalAcceptanceMusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override int NPCType => ModContent.NPCType<SupremeCalamitas>();
        public override int? MusicModMusic => CalamityMod.Instance.GetMusicFromMusicMod("SupremeCalamitas4");
        public override int VanillaMusic => MusicID.Eerie;
        public override int OtherworldMusic => MusicID.OtherworldlyEerie;

        public override bool AdditionalCheck() => CalamityGlobalNPC.SCalAcceptance != -1;
    }
}
