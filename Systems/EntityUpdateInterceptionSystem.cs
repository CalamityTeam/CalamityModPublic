using CalamityMod.NPCs;
using CalamityMod.Particles;
using CalamityMod.Particles.Metaballs;
using CalamityMod.TileEntities;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class EntityUpdateInterceptionSystem : ModSystem
    {
        #region Fusable Particle Updating
        public override void PostUpdateProjectiles()
        {
            // Update all fusable particles.
            // These are really only visual and as such don't really need any complex netcode.
            foreach (BaseFusableParticleSet.FusableParticleRenderCollection particleSet in FusableParticleManager.ParticleSets)
            {
                foreach (BaseFusableParticleSet.FusableParticle particle in particleSet.ParticleSet.Particles)
                    particleSet.ParticleSet.UpdateBehavior(particle);
            }
        }
        #endregion

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
