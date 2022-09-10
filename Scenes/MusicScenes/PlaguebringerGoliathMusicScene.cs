using CalamityMod.NPCs.PlaguebringerGoliath;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class PlaguebringerGoliathMusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override int NPCType => ModContent.NPCType<PlaguebringerGoliath>();
        public override int? MusicModMusic => CalamityMod.Instance.GetMusicFromMusicMod("PlaguebringerGoliath");
        public override int VanillaMusic => MusicID.Boss3;
        public override int OtherworldMusic => MusicID.OtherworldlyPlantera;
    }
}
