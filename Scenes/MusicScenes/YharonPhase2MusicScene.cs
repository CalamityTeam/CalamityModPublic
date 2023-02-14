using CalamityMod.NPCs;
using CalamityMod.NPCs.Yharon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class YharonPhase2MusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override int NPCType => ModContent.NPCType<Yharon>();
        public override int? MusicModMusic => CalamityMod.Instance.GetMusicFromMusicMod("YharonPhase2");
        public override int VanillaMusic => MusicID.LunarBoss;
        public override int OtherworldMusic => MusicID.OtherworldlyLunarBoss;

        public override bool AdditionalCheck() => CalamityGlobalNPC.yharonP2 != -1;
    }
}
