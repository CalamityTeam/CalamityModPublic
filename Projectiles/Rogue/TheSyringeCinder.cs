using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Rogue
{
    public class TheSyringeCinder : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Syringe Cinder");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 120;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Calamity().rogue = true;
			projectile.alpha = 100;
        }

        public override void AI()
        {
			//make it face the way it's going
			if (projectile.ai[1] == 1f)
			{
				projectile.rotation += projectile.velocity.X * 0.1f;
				projectile.rotation = -projectile.velocity.X * 0.05f;
			}
			else
			{
				projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + ((3 * MathHelper.Pi) / 2);
				projectile.spriteDirection = ((projectile.velocity.X > 0f) ? -1 : 1);
			}

			//frames
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }

			//movement
            if (projectile.velocity.X != projectile.velocity.X)
            {
                projectile.velocity.X = projectile.velocity.X * -0.1f;
            }
            if (projectile.velocity.X != projectile.velocity.X)
            {
                projectile.velocity.X = projectile.velocity.X * -0.5f;
            }
            if (projectile.velocity.Y != projectile.velocity.Y && projectile.velocity.Y > 1f)
            {
                projectile.velocity.Y = projectile.velocity.Y * -0.5f;
            }
            projectile.ai[0] += 1f;
            if (projectile.ai[0] > 5f)
            {
                projectile.ai[0] = 5f;
                if (projectile.velocity.Y == 0f && projectile.velocity.X != 0f)
                {
                    projectile.velocity.X = projectile.velocity.X * 0.97f;
                    if ((double)projectile.velocity.X > -0.01 && (double)projectile.velocity.X < 0.01)
                    {
                        projectile.velocity.X = 0f;
                        projectile.netUpdate = true;
                    }
                }
                projectile.velocity.Y = projectile.velocity.Y + 0.2f;
            }
			if (Main.rand.NextBool(4))
			{
				int num199 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 89, 0f, 0f, 100, default, 1f);
				Dust expr_8976_cp_0 = Main.dust[num199];
				expr_8976_cp_0.position.X -= 2f;
				Dust expr_8994_cp_0 = Main.dust[num199];
				expr_8994_cp_0.position.Y += 2f;
				Main.dust[num199].scale += (float)Main.rand.Next(50) * 0.01f;
				Main.dust[num199].noGravity = true;
				Dust expr_89E7_cp_0 = Main.dust[num199];
				expr_89E7_cp_0.velocity.Y -= 2f;
			}
            if (Main.rand.NextBool(10))
            {
                int num200 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 89, 0f, 0f, 100, default, 1f);
                Dust expr_8A4E_cp_0 = Main.dust[num200];
                expr_8A4E_cp_0.position.X -= 2f;
                Dust expr_8A6C_cp_0 = Main.dust[num200];
                expr_8A6C_cp_0.position.Y += 2f;
                Main.dust[num200].scale += 0.3f + (float)Main.rand.Next(50) * 0.01f;
                Main.dust[num200].noGravity = true;
                Main.dust[num200].velocity *= 0.1f;
            }
            if ((double)projectile.velocity.Y < 0.25 && (double)projectile.velocity.Y > 0.15)
            {
                projectile.velocity.X = projectile.velocity.X * 0.8f;
            }
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			projectile.ai[1] = 1f;
            if (projectile.penetrate == 0)
            {
                projectile.Kill();
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 120);
        }
    }
}
