using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class ClamorRifleProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Energy Bolt");
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.ranged = true;
        }

        public override void AI()
        {
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 3f)
            {
                Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0f / 255f, (255 - projectile.alpha) * 0.25f / 255f, (255 - projectile.alpha) * 0.25f / 255f);
                for (int num151 = 0; num151 < 3; num151++)
                {
                    int num154 = 14;
                    int num155 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width - num154 * 2, projectile.height - num154 * 2, 68, 0f, 0f, 100, default, 1f);
                    Main.dust[num155].noGravity = true;
                    Main.dust[num155].velocity *= 0.1f;
                    Main.dust[num155].velocity += projectile.velocity * 0.5f;
                }
            }
            float centerX = projectile.Center.X;
            float centerY = projectile.Center.Y;
            float num474 = 300f;
            bool homeIn = false;
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
                        centerX = num476;
                        centerY = num477;
                        homeIn = true;
                    }
                }
            }
            if (homeIn)
            {
                float num483 = 18f;
                Vector2 vector35 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                float num484 = centerX - vector35.X;
                float num485 = centerY - vector35.Y;
                float num486 = (float)Math.Sqrt((double)(num484 * num484 + num485 * num485));
                num486 = num483 / num486;
                num484 *= num486;
                num485 *= num486;
                projectile.velocity.X = (projectile.velocity.X * 25f + num484) / 26f;
                projectile.velocity.Y = (projectile.velocity.Y * 25f + num485) / 26f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            int num251 = Main.rand.Next(2, 3);
            if (projectile.owner == Main.myPlayer)
            {
                for (int num252 = 0; num252 < num251; num252++)
                {
                    Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    while (value15.X == 0f && value15.Y == 0f)
                    {
                        value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    }
                    value15.Normalize();
                    value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value15.X, value15.Y, ModContent.ProjectileType<ClamorRifleProjSplit>(), (int)((double)projectile.damage * 0.5), 0f, projectile.owner, 0f, 0f);
                }
            }
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 118);
        }
    }
}
