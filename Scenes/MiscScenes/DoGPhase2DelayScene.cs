using CalamityMod.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class DoGPhase2DelayScene : ModSceneEffect
    {
        public override int Music => CalamityMod.Instance.GetMusicFromMusicMod("DevourerOfGodsP2") ?? MusicID.LunarBoss;
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override bool IsSceneEffectActive(Player player)
        {
            if (CalamityGlobalNPC.DoGHead < 0 || !Main.npc[CalamityGlobalNPC.DoGHead].active)
                return false;

            return Main.npc[CalamityGlobalNPC.DoGHead].localAI[2] <= 530f && Main.npc[CalamityGlobalNPC.DoGHead].localAI[2] > 50f;
        }
    }
}
