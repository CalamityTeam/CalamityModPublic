using CalamityMod.NPCs.ProfanedGuardians;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class ProfanedGuardiansMusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override int NPCType => ModContent.NPCType<ProfanedGuardianCommander>();
        public override int? MusicModMusic => CalamityMod.Instance.GetMusicFromMusicMod("Guardians");
        public override int VanillaMusic => MusicID.Boss1;
        public override int OtherworldMusic => MusicID.OtherworldlyBoss1;
    }
}
