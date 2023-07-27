using CalamityMod.NPCs.PrimordialWyrm;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class AEWMusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
        public override int NPCType => ModContent.NPCType<PrimordialWyrmHead>();
        public override int? MusicModMusic => CalamityMod.Instance.GetMusicFromMusicMod("PrimordialWyrm");
        public override int VanillaMusic => MusicID.Boss3;
        public override int OtherworldMusic => MusicID.OtherworldlyBoss2;
    }
}
