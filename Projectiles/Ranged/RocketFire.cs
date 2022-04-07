using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class RocketFire : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Rogue/TotalityFire";
        private bool initialized = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fire");
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 120;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            //make it face the way it's going
            if (Projectile.ai[1] > 0f)
            {
                Projectile.rotation = -Projectile.velocity.X * 0.05f + MathHelper.PiOver2;
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            Projectile.ai[1]--;

            //frames
            if (!initialized)
            {
                initialized = true;
                Projectile.frame = Main.rand.Next(Main.projFrames[Projectile.type]);
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }

            //movement
            if (Projectile.velocity.X != Projectile.velocity.X)
            {
                Projectile.velocity.X *= -0.1f;
            }
            if (Projectile.velocity.X != Projectile.velocity.X)
            {
                Projectile.velocity.X *= -0.5f;
            }
            if (Projectile.velocity.Y != Projectile.velocity.Y && Projectile.velocity.Y > 1f)
            {
                Projectile.velocity.Y *= -0.5f;
            }
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 5f)
            {
                Projectile.ai[0] = 5f;
                if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
                {
                    Projectile.velocity.X *= 0.97f;
                    if (Projectile.velocity.X > -0.01f && Projectile.velocity.X < 0.01f)
                    {
                        Projectile.velocity.X = 0f;
                        Projectile.netUpdate = true;
                    }
                }
                Projectile.velocity.Y += 0.2f;
            }
            if (Projectile.velocity.Y < 0.25f && Projectile.velocity.Y > 0.15f)
            {
                Projectile.velocity.X *= 0.8f;
            }
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }

            //dust
            if (Main.rand.NextBool(4))
            {
                int fire = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 1f);
                Dust dust = Main.dust[fire];
                dust.position.X -= 2f;
                dust.position.Y += 2f;
                dust.scale += (float)Main.rand.Next(50) * 0.01f;
                dust.noGravity = true;
                dust.velocity.Y -= 2f;
            }
            if (Main.rand.NextBool(10))
            {
                int fire = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 1f);
                Dust dust = Main.dust[fire];
                dust.position.X -= 2f;
                dust.position.Y += 2f;
                dust.scale += 0.3f + (float)Main.rand.Next(50) * 0.01f;
                dust.noGravity = true;
                dust.velocity *= 0.1f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[1] = 10f;
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 120);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
