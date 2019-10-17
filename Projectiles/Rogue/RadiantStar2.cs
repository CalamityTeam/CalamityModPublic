using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Dusts;
using Terraria.ID;
using CalamityMod.Buffs;
namespace CalamityMod.Projectiles
{
    public class RadiantStar2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Radiant Star");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 2.355f;
            if (projectile.spriteDirection == -1)
            {
                projectile.rotation -= 1.57f;
            }
            if (projectile.ai[0] == 1f)
            {
                float num472 = projectile.Center.X;
                float num473 = projectile.Center.Y;
                float num474 = 600f;
                for (int num475 = 0; num475 < 200; num475++)
                {
                    if (Main.npc[num475].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[num475].Center, 1, 1) && !CalamityPlayer.areThereAnyDamnBosses)
                    {
                        float npcCenterX = Main.npc[num475].position.X + (float)(Main.npc[num475].width / 2);
                        float npcCenterY = Main.npc[num475].position.Y + (float)(Main.npc[num475].height / 2);
                        float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - npcCenterX) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - npcCenterY);
                        if (num478 < num474)
                        {
                            if (Main.npc[num475].position.X < num472)
                            {
                                Main.npc[num475].velocity.X += 0.25f;
                            }
                            else
                            {
                                Main.npc[num475].velocity.X -= 0.25f;
                            }
                            if (Main.npc[num475].position.Y < num473)
                            {
                                Main.npc[num475].velocity.Y += 0.25f;
                            }
                            else
                            {
                                Main.npc[num475].velocity.Y -= 0.25f;
                            }
                        }
                    }
                }
            }
            else
            {
                float centerX = projectile.Center.X;
                float centerY = projectile.Center.Y;
                float num474 = 600f;
                bool homeIn = false;
                for (int i = 0; i < 200; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
                    {
                        float num476 = Main.npc[i].position.X + (float)(Main.npc[i].width / 2);
                        float num477 = Main.npc[i].position.Y + (float)(Main.npc[i].height / 2);
                        float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num476) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num477);
                        if (num478 < num474)
                        {
                            num474 = num478;
                            centerX = num476;
                            centerY = num477;
                            homeIn = true;
                        }
                    }
                }
                if (homeIn)
                {
                    float num483 = 24f;
                    Vector2 vector35 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                    float num484 = centerX - vector35.X;
                    float num485 = centerY - vector35.Y;
                    float num486 = (float)Math.Sqrt((double)(num484 * num484 + num485 * num485));
                    num486 = num483 / num486;
                    num484 *= num486;
                    num485 *= num486;
                    projectile.velocity.X = (projectile.velocity.X * 20f + num484) / 21f;
                    projectile.velocity.Y = (projectile.velocity.Y * 20f + num485) / 21f;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, ModContent.DustType<AstralBlue>(), projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
