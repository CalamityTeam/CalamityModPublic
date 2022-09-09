using CalamityMod.NPCs.HiveMind;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class HiveMindMusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;

        public override int NPCType => ModContent.NPCType<HiveMind>();
        public override int? MusicModMusic => CalamityMod.Instance.GetMusicFromMusicMod("HiveMind");
        public override int VanillaMusic => MusicID.Boss2;
        public override int OtherworldMusic => MusicID.OtherworldlyWoF;
    }
}
