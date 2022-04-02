using CalamityMod.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class FrogGore4 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gore from a not frog of the explosive variety");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.width = 14;
            projectile.height = 16;
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
            projectile.rotation += projectile.velocity.X * 0.03f;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < Main.rand.Next(4, 8 + 1); i++)
            {
                Dust.NewDustPerfect(projectile.Center + Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(10f),
                    (int)CalamityDusts.SulfurousSeaAcid,
                    Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(1f, 4f));
            }
        }
    }
}
