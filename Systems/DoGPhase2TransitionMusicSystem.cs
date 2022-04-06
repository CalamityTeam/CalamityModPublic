using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class DoGPhase2TransitionMusicSystem : ModSceneEffect
    {
        public override int Music => CalamityMod.Instance.GetMusicFromMusicMod("DevourerOfGodsP2") ?? MusicID.LunarBoss;
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override bool IsSceneEffectActive(Player player) => CalamityWorld.DoGSecondStageCountdown <= 530 && CalamityWorld.DoGSecondStageCountdown > 50;
    }
}
