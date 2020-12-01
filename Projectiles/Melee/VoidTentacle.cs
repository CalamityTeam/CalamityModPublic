using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class VoidTentacle : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Void Tentacle");
        }
        public override void SetDefaults()
        {
            projectile.height = 160;
            projectile.width = 160;
            projectile.melee = true;
            projectile.friendly = true;
            projectile.MaxUpdates = 3;

            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 4;
        }

        public override void AI()
        {
            // HOW CAN THIS CODE EVER RUN
            if (projectile.velocity.X != projectile.velocity.X)
            {
                if (Math.Abs(projectile.velocity.X) < 1f)
                    projectile.velocity.X = -projectile.velocity.X;
                else
                    projectile.Kill();
            }
            if (projectile.velocity.Y != projectile.velocity.Y)
            {
                if (Math.Abs(projectile.velocity.Y) < 1f)
                    projectile.velocity.Y = -projectile.velocity.Y;
                else
                    projectile.Kill();
            }

            Vector2 center10 = projectile.Center;
            projectile.scale = 1f - projectile.localAI[0];
            projectile.width = (int)(20f * projectile.scale);
            projectile.height = projectile.width;
            projectile.position.X = center10.X - (float)(projectile.width / 2);
            projectile.position.Y = center10.Y - (float)(projectile.height / 2);
            if ((double)projectile.localAI[0] < 0.1)
            {
                projectile.localAI[0] += 0.01f;
            }
            else
            {
                projectile.localAI[0] += 0.025f;
            }
            if (projectile.localAI[0] >= 0.95f)
            {
                projectile.Kill();
            }
            projectile.velocity.X = projectile.velocity.X + projectile.ai[0] * 1.5f;
            projectile.velocity.Y = projectile.velocity.Y + projectile.ai[1] * 1.5f;
            if (projectile.velocity.Length() > 16f)
            {
                projectile.velocity.Normalize();
                projectile.velocity *= 16f;
            }
            projectile.ai[0] *= 1.05f;
            projectile.ai[1] *= 1.05f;
            if (projectile.scale < 1f)
            {
                int i = 0;
                while (i < projectile.scale * 4f)
                {
                    int dustID = Main.rand.NextBool(5) ? 199 : 175;
                    int idx = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustID, projectile.velocity.X, projectile.velocity.Y, 100, default, 1.1f);
                    Main.dust[idx].position = (Main.dust[idx].position + projectile.Center) / 2f;
                    Main.dust[idx].noGravity = true;
                    Main.dust[idx].velocity *= 0.1f;
                    Main.dust[idx].velocity -= projectile.velocity * (1.3f - projectile.scale);
                    Main.dust[idx].fadeIn = (float)(100 + projectile.owner);
                    Main.dust[idx].scale += projectile.scale * 0.75f;
                    i++;
                }
            }
        }
    }
}
