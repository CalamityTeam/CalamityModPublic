using CalamityMod.NPCs;
using CalamityMod.Particles;
using CalamityMod.TileEntities;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class EntityUpdateInterceptionSystem : ModSystem
    {
        #region Ash Drawing
        public override void PostUpdateDusts() => DeathAshParticle.UpdateAll();
        #endregion Ash Drawing

        #region Post NPC Updating
        // TODO -- Apply caching to this process. For now most of the looping issues should be eradicated but it can be reduced further.
        public override void PostUpdateNPCs() => CalamityGlobalNPC.ResetTownNPCNameBools();
        #endregion

        #region Tile Entity Time Handler
        public override void PostUpdateTime() => TileEntityTimeHandler.Update();
        #endregion

        #region Particles updating
        public override void PostUpdateEverything()
        {
            if (!Main.dedServ)
                GeneralParticleHandler.Update();
        }
        #endregion
    }
}
