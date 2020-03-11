using CalamityMod.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class OldDukeSharkVomit : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Puke");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.width = 86;
            projectile.height = 36;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.timeLeft = 360;
            projectile.alpha = 255;
        }

        public override void AI()
        {
            projectile.ai[0] += 1f;
            if (projectile.ai[0] < 10f)
            {
                projectile.alpha = 255 - (int)(255 * projectile.ai[0] / 10f);
            }
            projectile.velocity.Y += 0.2f;
            projectile.rotation = projectile.velocity.ToRotation();
		}
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < Main.rand.Next(28, 41); i++)
            {
                Dust.NewDustPerfect(projectile.Center + Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(10f),
                    (int)CalamityDusts.SulfurousSeaAcid,
                    Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(1f, 4f));
            }
        }
    }
}
