using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class Hiveling : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hiveling");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.SentryShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
			Player player = Main.player[projectile.owner];
            projectile.frameCounter++;
            if (projectile.frameCounter > 3)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 2.355f;
            projectile.spriteDirection = -projectile.direction;
            projectile.rotation = projectile.velocity.X * 0.05f;
            float centerX = projectile.Center.X;
            float centerY = projectile.Center.Y;
            float num474 = 1200f;
            bool homeIn = false;
            int target = (int)projectile.ai[0];
            
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
				if (npc.CanBeChasedBy(projectile, false))
				{
					float num476 = npc.Center.X;
					float num477 = npc.Center.Y;
					float num478 = Math.Abs(projectile.Center.X - num476) + Math.Abs(projectile.Center.Y - num477);
					if (num478 < num474)
					{
						centerX = num476;
						centerY = num477;
						homeIn = true;
					}
				}
            }
			else if (Main.npc[target].CanBeChasedBy(projectile, false))
            {
                float num476 = Main.npc[target].Center.X;
                float num477 = Main.npc[target].Center.Y;
				float num478 = Math.Abs(projectile.Center.X - num476) + Math.Abs(projectile.Center.Y - num477);
                if (num478 < num474)
                {
                    centerX = num476;
                    centerY = num477;
                    homeIn = true;
                }
            }
            if (!homeIn)
            {
                for (int i = 0; i < Main.maxNPCs; ++i)
                {
                    NPC npc = Main.npc[i];
                    if (npc is null || !npc.active)
                        continue;

                    if (npc.CanBeChasedBy(projectile, false))
                    {
						float num476 = npc.Center.X;
						float num477 = npc.Center.Y;
						float num478 = Math.Abs(projectile.Center.X - num476) + Math.Abs(projectile.Center.Y - num477);
						if (num478 < num474)
						{
							centerX = num476;
							centerY = num477;
							homeIn = true;
						}
                    }
                }
            }
            if (homeIn)
            {
                float num483 = 10f;
                Vector2 vector35 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                float num484 = centerX - vector35.X;
                float num485 = centerY - vector35.Y;
                float num486 = (float)Math.Sqrt((double)(num484 * num484 + num485 * num485));
                num486 = num483 / num486;
                num484 *= num486;
                num485 *= num486;
                projectile.velocity.X = (projectile.velocity.X * 30f + num484) / 31f;
                projectile.velocity.Y = (projectile.velocity.Y * 30f + num485) / 31f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 60);
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, ModContent.DustType<AstralBlue>(), projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, ModContent.DustType<AstralOrange>(), projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }
    }
}
