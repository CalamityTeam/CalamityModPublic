
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class NightsGazeSpark : ModProjectile
    {
        public static int lifetime = 150;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Night's Gaze Spark");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 5;
            projectile.timeLeft = lifetime;
            projectile.Calamity().rogue = true;
            projectile.localAI[0] = 10f;
        }

        public override void AI()
        {
            projectile.rotation += projectile.direction * projectile.ai[0];

            if (projectile.timeLeft < (lifetime - projectile.ai[1]) && projectile.localAI[0] >= 0)
            {
                projectile.velocity.Normalize();
                projectile.velocity *= projectile.localAI[0];
                projectile.localAI[0]--;
            }

            if (projectile.localAI[0] == 0)
            {
                projectile.Kill();
            }

            for (int i = 0; i < 3; i++)
            {
                float dustVelocity = Main.rand.NextFloat(0f, 0.5f);
                int dustToUse = Main.rand.Next(0, 3);
                int dustType = 0;
                switch (dustToUse)
                {
                    case 0:
                        dustType = 109;
                        break;
                    case 1:
                        dustType = 111;
                        break;
                    case 2:
                        dustType = 132;
                        break;
                }

                int dust = Dust.NewDust(projectile.Center, 1, 1, dustType, projectile.velocity.X, projectile.velocity.Y, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= dustVelocity;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 120);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            projectile.Kill();
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int dustToUse = Main.rand.Next(0, 3);
                int dustType = 0;
                switch (dustToUse)
                {
                    case 0:
                        dustType = 109;
                        break;
                    case 1:
                        dustType = 111;
                        break;
                    case 2:
                        dustType = 132;
                        break;
                }

                int dust = Dust.NewDust(projectile.Center, 1, 1, dustType, projectile.velocity.X, projectile.velocity.Y, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.75f;
            }
        }
    }
}
