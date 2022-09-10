using CalamityMod.NPCs.CeaselessVoid;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class CeaselessVoidMusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override int NPCType => ModContent.NPCType<CeaselessVoid>();
        public override int? MusicModMusic => CalamityMod.Instance.GetMusicFromMusicMod("CeaselessVoid");
        public override int VanillaMusic => MusicID.Boss3;
        public override int OtherworldMusic => MusicID.OtherworldlyBoss2;
    }
}
