using CalamityMod.NPCs.AdultEidolonWyrm;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class AEWMusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override int NPCType => ModContent.NPCType<AdultEidolonWyrmHead>();
        public override int? MusicModMusic => CalamityMod.Instance.GetMusicFromMusicMod("AdultEidolonWyrm");
        public override int VanillaMusic => MusicID.Boss3;
        public override int OtherworldMusic => MusicID.OtherworldlyBoss2;
    }
}
