using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Buffs.DamageOverTime;
namespace CalamityMod.Projectiles.Ranged
{
    public class SepticSkewerHarpoon : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Septic Skewer");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.ranged = true;
            projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 8;
            projectile.timeLeft = 900;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 171, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            Vector2 vector62 = player.Center - projectile.Center;
            projectile.ai[1] += 1f;
            if (projectile.ai[1] > 5f)
            {
                projectile.alpha = 0;
            }
            if (projectile.ai[1] % 8f == 0f && projectile.owner == Main.myPlayer && Main.rand.NextBool(5))
            {
                Vector2 vector63 = vector62 * -1f;
                vector63.Normalize();
                vector63 *= Main.rand.Next(45, 65) * 0.1f;
                vector63 = vector63.RotatedBy((Main.rand.NextDouble() - 0.5) * MathHelper.PiOver2);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, vector63.X, vector63.Y, ModContent.ProjectileType<SepticSkewerBacteria>(), (int)(projectile.damage * 0.175), projectile.knockBack * 0.2f, projectile.owner, -10f, 0f);
            }
            if (player.dead)
            {
                projectile.Kill();
                return;
            }
            if (projectile.alpha == 0)
            {
                if (projectile.position.X + (float)(projectile.width / 2) > player.position.X + (float)(player.width / 2))
                {
                    player.ChangeDir(1);
                }
                else
                {
                    player.ChangeDir(-1);
                }
            }
            if (projectile.ai[0] == 0f)
            {
                projectile.extraUpdates = 2;
            }
            else
            {
                projectile.extraUpdates = 3;
            }
            Vector2 vector14 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
            float xDist = player.position.X + (float)(player.width / 2) - vector14.X;
            float yDist = player.position.Y + (float)(player.height / 2) - vector14.Y;
            float playerDist = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
            if (projectile.ai[0] == 0f)
            {
                if (playerDist > 2000f)
                {
                    projectile.ai[0] = 1f;
                }
                projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
                projectile.ai[1] += 1f;
                if (projectile.ai[1] > 5f)
                {
                    projectile.alpha = 0;
                }
                if (projectile.ai[1] > 8f)
                {
                    projectile.ai[1] = 8f;
                }
                if (projectile.ai[1] >= 10f)
                {
                    projectile.ai[1] = 15f;
                    projectile.velocity.Y = projectile.velocity.Y + 0.3f;
                }
            }
            else if (projectile.ai[0] == 1f)
            {
                projectile.tileCollide = false;
                projectile.rotation = (float)Math.Atan2((double)yDist, (double)xDist) - 1.57f;
                float returnSpeed = 20f;
                if (playerDist < 50f)
                {
                    projectile.Kill();
                }
                playerDist = returnSpeed / playerDist;
                xDist *= playerDist;
                yDist *= playerDist;
                projectile.velocity.X = xDist;
                projectile.velocity.Y = yDist;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.ai[0] = 1f;
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 180);
        }
    }
}
