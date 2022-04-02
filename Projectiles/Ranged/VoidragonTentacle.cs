using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class VoidragonTentacle : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Voidragon Tentacle");
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.ignoreWater = true;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
            projectile.MaxUpdates = 3;
            projectile.ranged = true;
            projectile.extraUpdates = 1;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            projectile.localAI[1]++;
            if (projectile.velocity.X != projectile.velocity.X)
            {
                if (Math.Abs(projectile.velocity.X) < 1f)
                {
                    projectile.velocity.X = -projectile.velocity.X;
                }
                else
                {
                    projectile.Kill();
                }
            }
            if (projectile.velocity.Y != projectile.velocity.Y)
            {
                if (Math.Abs(projectile.velocity.Y) < 1f)
                {
                    projectile.velocity.Y = -projectile.velocity.Y;
                }
                else
                {
                    projectile.Kill();
                }
            }
            Vector2 center10 = projectile.Center;
            projectile.scale = 1f - projectile.localAI[0];
            projectile.width = (int)(20f * projectile.scale);
            projectile.height = projectile.width;
            projectile.position.X = center10.X - (float)(projectile.width / 2);
            projectile.position.Y = center10.Y - (float)(projectile.height / 2);
            if (projectile.localAI[0] < 0.1f)
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
            if (projectile.scale < 1f && projectile.localAI[1] > 5f)
            {
                int dustAmount = 0;
                while ((float)dustAmount < projectile.scale * 10f)
                {
                    int dust = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 62, projectile.velocity.X, projectile.velocity.Y, 100, default, 1.3f);
                    Main.dust[dust].position = (Main.dust[dust].position + projectile.Center) / 2f;
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.1f;
                    Main.dust[dust].velocity -= projectile.velocity * (1.1f - projectile.scale);
                    Main.dust[dust].fadeIn = (float)(125 + projectile.owner);
                    Main.dust[dust].scale += projectile.scale * 0.75f;
                    dustAmount++;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<DemonFlames>(), 180); //Apotheosis also inflicts this, so this weapon might as well
        }
    }
}
