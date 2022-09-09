using CalamityMod.NPCs;
using CalamityMod.NPCs.DevourerofGods;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class DevourerofGodsMusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override int NPCType => ModContent.NPCType<DevourerofGodsHead>();
        public override int? MusicModMusic => CalamityMod.Instance.GetMusicFromMusicMod("DevourerOfGodsP1");
        public override int VanillaMusic => MusicID.Boss3;
        public override int OtherworldMusic => MusicID.OtherworldlyBoss2;

        public override bool AdditionalCheck() => CalamityGlobalNPC.DoGHead != -1 && CalamityGlobalNPC.DoGP2 == -1;
    }
}
