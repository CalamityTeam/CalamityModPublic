using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
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
            projectile.ranged = true;
            projectile.extraUpdates = 0;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 9;
			projectile.timeLeft = 600;
        }

        public override void AI()
        {
            if (Main.rand.Next(5) == 0)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 171, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            Vector2 vector62 = Main.player[projectile.owner].Center - projectile.Center;
            float[] var_2_1DE21_cp_0 = projectile.ai;
            int var_2_1DE21_cp_1 = 1;
            float num73 = var_2_1DE21_cp_0[var_2_1DE21_cp_1];
            var_2_1DE21_cp_0[var_2_1DE21_cp_1] = num73 + 1f;
            if (projectile.ai[1] > 5f)
            {
                projectile.alpha = 0;
            }
            if ((int)projectile.ai[1] % 4 == 0 && projectile.owner == Main.myPlayer && Main.rand.NextBool(5))
            {
                Vector2 vector63 = vector62 * -1f;
                vector63.Normalize();
                vector63 *= (float)Main.rand.Next(45, 65) * 0.1f;
                vector63 = vector63.RotatedBy((Main.rand.NextDouble() - 0.5) * 1.5707963705062866, default);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, vector63.X, vector63.Y, ModContent.ProjectileType<SepticSkewerBacteria>(), (int)((double)projectile.damage * 0.4), projectile.knockBack * 0.2f, projectile.owner, -10f, 0f);
            }
            if (Main.player[projectile.owner].dead)
            {
                projectile.Kill();
                return;
            }
            if (projectile.alpha == 0)
            {
                if (projectile.position.X + (float)(projectile.width / 2) > Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2))
                {
                    Main.player[projectile.owner].ChangeDir(1);
                }
                else
                {
                    Main.player[projectile.owner].ChangeDir(-1);
                }
            }
            if (projectile.ai[0] == 0f)
            {
                projectile.extraUpdates = 0;
            }
            else
            {
                projectile.extraUpdates = 1;
            }
            Vector2 vector14 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
            float num166 = Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2) - vector14.X;
            float num167 = Main.player[projectile.owner].position.Y + (float)(Main.player[projectile.owner].height / 2) - vector14.Y;
            float num168 = (float)Math.Sqrt((double)(num166 * num166 + num167 * num167));
            if (projectile.ai[0] == 0f)
            {
                if (num168 > 2000f)
                {
                    projectile.ai[0] = 1f;
                }
                else if (num168 > 1000f)
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
                projectile.rotation = (float)Math.Atan2((double)num167, (double)num166) - 1.57f;
                float num169 = 20f;
                if (num168 < 50f)
                {
                    projectile.Kill();
                }
                num168 = num169 / num168;
                num166 *= num168;
                num167 *= num168;
                projectile.velocity.X = num166;
                projectile.velocity.Y = num167;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 300);
        }
    }
}
