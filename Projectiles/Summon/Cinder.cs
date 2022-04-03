using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class Cinder : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public const float FallAcceleration = 0.185f;
        public const float FallSpeedMax = 16;
        public const float FallDelay = 300;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cinder");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 300;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.minion = true;
        }

        public override void AI()
        {
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
            Projectile.ai[0]++;
            if (Projectile.ai[0] > 5f)
            {
                Projectile.ai[0] = 5f;
                if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
                {
                    Projectile.velocity.X *= 0.97f;
                    if (Math.Abs(Projectile.velocity.X) < 0.01)
                    {
                        Projectile.velocity.X = 0f;
                        Projectile.netUpdate = true;
                    }
                }
            }

            if (Projectile.ai[0] >= FallDelay && Projectile.velocity.Y < FallSpeedMax)
            {
                Projectile.velocity.Y += FallAcceleration;
            }

            Projectile.rotation += Projectile.velocity.X * 0.1f;
            int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 1f);
            Main.dust[idx].position += new Vector2(2f);
            Main.dust[idx].scale += Main.rand.NextFloat(0.5f);
            Main.dust[idx].noGravity = true;
            Main.dust[idx].velocity.Y -= 2f;
            if (Main.rand.NextBool(2))
            {
                idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 1f);
                Main.dust[idx].position += new Vector2(2f);
                Main.dust[idx].scale += 0.3f + Main.rand.NextFloat(0.5f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity.Y -= 2f;
            }
            if (Projectile.velocity.Y < 0.25f && Projectile.velocity.Y > 0.15f)
            {
                Projectile.velocity.X *= 0.8f;
            }
            Projectile.rotation = -Projectile.velocity.X * 0.05f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
