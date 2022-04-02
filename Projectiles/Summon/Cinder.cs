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
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.timeLeft = 300;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 10;
            projectile.minion = true;
        }

        public override void AI()
        {
            if (projectile.velocity.X != projectile.velocity.X)
            {
                projectile.velocity.X *= -0.1f;
            }
            if (projectile.velocity.X != projectile.velocity.X)
            {
                projectile.velocity.X *= -0.5f;
            }
            if (projectile.velocity.Y != projectile.velocity.Y && projectile.velocity.Y > 1f)
            {
                projectile.velocity.Y *= -0.5f;
            }
            projectile.ai[0]++;
            if (projectile.ai[0] > 5f)
            {
                projectile.ai[0] = 5f;
                if (projectile.velocity.Y == 0f && projectile.velocity.X != 0f)
                {
                    projectile.velocity.X *= 0.97f;
                    if (Math.Abs(projectile.velocity.X) < 0.01)
                    {
                        projectile.velocity.X = 0f;
                        projectile.netUpdate = true;
                    }
                }
            }

            if (projectile.ai[0] >= FallDelay && projectile.velocity.Y < FallSpeedMax)
            {
                projectile.velocity.Y += FallAcceleration;
            }

            projectile.rotation += projectile.velocity.X * 0.1f;
            int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default, 1f);
            Main.dust[idx].position += new Vector2(2f);
            Main.dust[idx].scale += Main.rand.NextFloat(0.5f);
            Main.dust[idx].noGravity = true;
            Main.dust[idx].velocity.Y -= 2f;
            if (Main.rand.NextBool(2))
            {
                idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default, 1f);
                Main.dust[idx].position += new Vector2(2f);
                Main.dust[idx].scale += 0.3f + Main.rand.NextFloat(0.5f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity.Y -= 2f;
            }
            if (projectile.velocity.Y < 0.25f && projectile.velocity.Y > 0.15f)
            {
                projectile.velocity.X *= 0.8f;
            }
            projectile.rotation = -projectile.velocity.X * 0.05f;
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
