using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    // Photoviscerator left click main projectile (the flamethrower itself)
    public class ExoFire : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public bool ProducedAcceleration = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exo Flames");
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.MaxUpdates = 3;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 5;
            projectile.timeLeft = 180;
        }

        public override void AI()
        {
            projectile.ai[0] += 1f;
            if (projectile.ai[0] <= 3f)
                return;

            float dustScale = 1f;
            if (projectile.ai[0] == 8f)
            {
                dustScale = 0.25f;
            }
            else if (projectile.ai[0] == 9f)
            {
                dustScale = 0.5f;
            }
            else if (projectile.ai[0] == 10f)
            {
                dustScale = 0.75f;
            }

            int dustID = Main.rand.NextBool() ? 107 : 234;
            if (Main.rand.NextBool(4))
                dustID = 269;

            if (Main.rand.NextBool())
            {
                for (int i = 0; i < 2; i++)
                {
                    Dust d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, dustID, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default, 0.6f);
                    if (Main.rand.NextBool(3))
                    {
                        d.scale *= 1.5f;
                        d.velocity.X *= 1.2f;
                        d.velocity.Y *= 1.2f;
                    }
                    else
                        d.scale *= 0.75f;

                    d.noGravity = true;
                    d.velocity.X *= 0.8f;
                    d.velocity.Y *= 0.8f;
                    d.scale *= dustScale;
                    d.velocity += projectile.velocity;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.ExoDebuffs();

        public override void OnHitPvp(Player target, int damage, bool crit) => target.ExoDebuffs();
    }
}
