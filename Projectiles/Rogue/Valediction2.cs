using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class Valediction2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scythe");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 70;
            projectile.height = 70;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = 12;
            projectile.timeLeft = 750;
            projectile.extraUpdates = 3;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 15;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 8;
                Main.PlaySound(SoundID.Item7, projectile.position);
            }
            if (projectile.localAI[0] == 0f)
            {
                projectile.ai[1] += 1f;
                if (projectile.ai[1] >= 60f)
                {
                    projectile.localAI[0] = 1f;
                    projectile.ai[1] = 0f;
                    projectile.netUpdate = true;
                }
                else
                {
                    float num472 = projectile.Center.X;
                    float num473 = projectile.Center.Y;
                    float num474 = 400f;
                    bool flag17 = false;
                    for (int num475 = 0; num475 < 200; num475++)
                    {
                        if (Main.npc[num475].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[num475].Center, 1, 1))
                        {
                            float num476 = Main.npc[num475].position.X + (float)(Main.npc[num475].width / 2);
                            float num477 = Main.npc[num475].position.Y + (float)(Main.npc[num475].height / 2);
                            float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num476) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num477);
                            if (num478 < num474)
                            {
                                num474 = num478;
                                num472 = num476;
                                num473 = num477;
                                flag17 = true;
                            }
                        }
                    }
                    if (flag17)
                    {
                        float num483 = 20f;
                        Vector2 vector35 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                        float num484 = num472 - vector35.X;
                        float num485 = num473 - vector35.Y;
                        float num486 = (float)Math.Sqrt((double)(num484 * num484 + num485 * num485));
                        num486 = num483 / num486;
                        num484 *= num486;
                        num485 *= num486;
                        projectile.velocity.X = (projectile.velocity.X * 20f + num484) / 21f;
                        projectile.velocity.Y = (projectile.velocity.Y * 20f + num485) / 21f;
                    }
                }
            }
            else
            {
                float num633 = 700f;
                bool flag24 = false;
                if (projectile.ai[0] == 1f)
                {
                    projectile.ai[1] += 1f;
                    if (projectile.ai[1] > 40f)
                    {
                        projectile.ai[1] = 1f;
                        projectile.ai[0] = 0f;
                        projectile.netUpdate = true;
                    }
                    else
                    {
                        flag24 = true;
                    }
                }
                if (flag24)
                {
                    return;
                }
                Vector2 vector46 = projectile.position;
                bool flag25 = false;
                for (int num645 = 0; num645 < 200; num645++)
                {
                    NPC nPC2 = Main.npc[num645];
                    if (nPC2.CanBeChasedBy(projectile, false))
                    {
                        float num646 = Vector2.Distance(nPC2.Center, projectile.Center);
                        if (!flag25)
                        {
                            num633 = num646;
                            vector46 = nPC2.Center;
                            flag25 = true;
                        }
                    }
                }
                if (flag25 && projectile.ai[0] == 0f)
                {
                    Vector2 vector47 = vector46 - projectile.Center;
                    float num648 = vector47.Length();
                    vector47.Normalize();
                    if (num648 > 200f)
                    {
                        float scaleFactor2 = 8f;
                        vector47 *= scaleFactor2;
                        projectile.velocity = (projectile.velocity * 40f + vector47) / 41f;
                    }
                    else
                    {
                        float num649 = 4f;
                        vector47 *= -num649;
                        projectile.velocity = (projectile.velocity * 40f + vector47) / 41f;
                    }
                }
                if (projectile.ai[1] > 0f)
                {
                    projectile.ai[1] += (float)Main.rand.Next(1, 4);
                }
                if (projectile.ai[1] > 40f)
                {
                    projectile.ai[1] = 0f;
                    projectile.netUpdate = true;
                }
                if (projectile.ai[0] == 0f)
                {
                    if (projectile.ai[1] == 0f && flag25 && num633 < 500f)
                    {
                        projectile.ai[1] += 1f;
                        if (Main.myPlayer == projectile.owner)
                        {
                            projectile.ai[0] = 1f;
                            Vector2 value20 = vector46 - projectile.Center;
                            value20.Normalize();
                            projectile.velocity = value20 * 8f;
                            projectile.netUpdate = true;
                        }
                    }
                }
            }
            projectile.rotation += 0.5f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 600);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 21);
            projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
            projectile.width = 100;
            projectile.height = 100;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num621 = 0; num621 < 5; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 33, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 8; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 33, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 33, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
        }
    }
}
