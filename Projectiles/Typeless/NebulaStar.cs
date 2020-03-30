using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Typeless
{
    public class NebulaStar : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star");
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 3600;
        }

        public override void AI()
        {
            float num944 = 1f - (float)projectile.alpha / 255f;
            num944 *= projectile.scale;
            Lighting.AddLight(projectile.Center, 0.25f * num944, 0.025f * num944, 0.275f * num944);
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] >= 90f)
            {
                projectile.localAI[0] *= -1f;
            }
            if (projectile.localAI[0] >= 0f)
            {
                projectile.scale += 0.003f;
            }
            else
            {
                projectile.scale -= 0.003f;
            }
            projectile.rotation += 0.0025f * projectile.scale;
            float num945 = 1f;
            float num946 = 1f;
            if (projectile.identity % 6 == 0)
            {
                num946 *= -1f;
            }
            if (projectile.identity % 6 == 1)
            {
                num945 *= -1f;
            }
            if (projectile.identity % 6 == 2)
            {
                num946 *= -1f;
                num945 *= -1f;
            }
            if (projectile.identity % 6 == 3)
            {
                num946 = 0f;
            }
            if (projectile.identity % 6 == 4)
            {
                num945 = 0f;
            }
            projectile.localAI[1] += 1f;
            if (projectile.localAI[1] > 60f)
            {
                projectile.localAI[1] = -180f;
            }
            if (projectile.localAI[1] >= -60f)
            {
                projectile.velocity.X = projectile.velocity.X + 0.002f * num946;
                projectile.velocity.Y = projectile.velocity.Y + 0.002f * num945;
            }
            else
            {
                projectile.velocity.X = projectile.velocity.X - 0.002f * num946;
                projectile.velocity.Y = projectile.velocity.Y - 0.002f * num945;
            }
            projectile.ai[0] += 1f;
            if (projectile.ai[0] > 5400f)
            {
                projectile.damage = 0;
                projectile.ai[1] = 1f;
                if (projectile.alpha < 255)
                {
                    projectile.alpha += 5;
                    if (projectile.alpha > 255)
                    {
                        projectile.alpha = 255;
                    }
                }
                else if (projectile.owner == Main.myPlayer)
                {
                    projectile.Kill();
                }
            }
            else
            {
                float num947 = (projectile.Center - Main.player[projectile.owner].Center).Length() / 100f;
                if (num947 > 4f)
                {
                    num947 *= 1.1f;
                }
                if (num947 > 5f)
                {
                    num947 *= 1.2f;
                }
                if (num947 > 6f)
                {
                    num947 *= 1.3f;
                }
                if (num947 > 7f)
                {
                    num947 *= 1.4f;
                }
                if (num947 > 8f)
                {
                    num947 *= 1.5f;
                }
                if (num947 > 9f)
                {
                    num947 *= 1.6f;
                }
                if (num947 > 10f)
                {
                    num947 *= 1.7f;
                }
                projectile.ai[0] += num947;
                if (projectile.alpha > 50)
                {
                    projectile.alpha -= 10;
                    if (projectile.alpha < 50)
                    {
                        projectile.alpha = 50;
                    }
                }
            }
            bool flag49 = false;
            Vector2 center12 = new Vector2(0f, 0f);
            float num948 = 600f;
            for (int num949 = 0; num949 < Main.maxNPCs; num949++)
            {
                if (Main.npc[num949].CanBeChasedBy(projectile, false))
                {
                    float num950 = Main.npc[num949].position.X + (float)(Main.npc[num949].width / 2);
                    float num951 = Main.npc[num949].position.Y + (float)(Main.npc[num949].height / 2);
                    float num952 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num950) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num951);
                    if (num952 < num948)
                    {
                        num948 = num952;
                        center12 = Main.npc[num949].Center;
                        flag49 = true;
                    }
                }
            }
            if (flag49)
            {
                Vector2 vector101 = center12 - projectile.Center;
                vector101.Normalize();
                vector101 *= 0.75f;
                projectile.velocity = (projectile.velocity * 10f + vector101) / 11f;
                return;
            }
            if ((double)projectile.velocity.Length() > 0.2)
            {
                projectile.velocity *= 0.98f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.owner == Main.myPlayer && projectile.ai[1] == 0f)
            {
                Vector2 value10 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                value10.Normalize();
                value10 *= 0.3f;
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value10.X, value10.Y, ModContent.ProjectileType<NebulaDust>(), projectile.damage, 0f, projectile.owner, 0f, 0f);
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Rectangle frame = new Rectangle(0, 0, Main.projectileTexture[projectile.type].Width, Main.projectileTexture[projectile.type].Height);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Typeless/NebulaStarGlow"), projectile.Center - Main.screenPosition, frame, Color.White, projectile.rotation, projectile.Size / 2, 1f, SpriteEffects.None, 0f);
        }
    }
}
