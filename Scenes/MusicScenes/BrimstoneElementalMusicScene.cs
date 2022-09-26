using CalamityMod.NPCs.BrimstoneElemental;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class BrimstoneElementalMusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override int NPCType => ModContent.NPCType<BrimstoneElemental>();
        public override int? MusicModMusic => CalamityMod.Instance.GetMusicFromMusicMod("BrimstoneElemental");
        public override int VanillaMusic => MusicID.Boss4;
        public override int OtherworldMusic => MusicID.OtherworldlyBoss2;
    }
}
