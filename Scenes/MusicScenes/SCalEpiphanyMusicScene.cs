using CalamityMod.NPCs;
using CalamityMod.NPCs.SupremeCalamitas;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class SCalEpiphanyMusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override int NPCType => ModContent.NPCType<SupremeCalamitas>();
        public override int? MusicModMusic => CalamityMod.Instance.GetMusicFromMusicMod("SupremeCalamitas3");
        public override int VanillaMusic => MusicID.LunarBoss;
        public override int OtherworldMusic => MusicID.OtherworldlyLunarBoss;

        public override bool AdditionalCheck() => CalamityGlobalNPC.SCalEpiphany != -1;
    }
}
