using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class SummonBrimstoneExplosionSmall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosion");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
            Main.projFrames[projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 108;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.minion = true;
            projectile.penetrate = -1;
            projectile.timeLeft = Main.projFrames[projectile.type] * 5;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0f)
            {
                CreateInitialDust();
                projectile.localAI[0] = 1f;
            }

            // Emit crimson light.
            Lighting.AddLight(projectile.Center, Color.Red.ToVector3() * 1.1f);
            if (projectile.timeLeft % 5f == 4f)
                projectile.frame++;
        }

        public void CreateInitialDust()
        {
            float randomnessSmoothness = 3f;
            for (int i = 0; i < 6; i++)
            {
                randomnessSmoothness += i * 0.36f;

                for (int j = 0; j < 30; j++)
                {
                    Vector2 velocity = Main.rand.NextVector2Square(-randomnessSmoothness, randomnessSmoothness).SafeNormalize(Vector2.UnitY) * randomnessSmoothness * 0.48f;
                    Dust dust = Dust.NewDustDirect(projectile.position + Vector2.One * 22f, 1, 1, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 1.6f);
                    dust.position += Main.rand.NextVector2Square(-10f, 10f);
                    dust.velocity = velocity;
                    dust.noGravity = true;
                }
            }

            projectile.Damage();
        }
    }
}
