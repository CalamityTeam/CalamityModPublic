
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
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 5;
            Projectile.timeLeft = lifetime;
            Projectile.Calamity().rogue = true;
            Projectile.localAI[0] = 10f;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.direction * Projectile.ai[0];

            if (Projectile.timeLeft < (lifetime - Projectile.ai[1]) && Projectile.localAI[0] >= 0)
            {
                Projectile.velocity.Normalize();
                Projectile.velocity *= Projectile.localAI[0];
                Projectile.localAI[0]--;
            }

            if (Projectile.localAI[0] == 0)
            {
                Projectile.Kill();
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

                int dust = Dust.NewDust(Projectile.Center, 1, 1, dustType, Projectile.velocity.X, Projectile.velocity.Y, 0, default, 1.5f);
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
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            Projectile.Kill();
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

                int dust = Dust.NewDust(Projectile.Center, 1, 1, dustType, Projectile.velocity.X, Projectile.velocity.Y, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.75f;
            }
        }
    }
}
