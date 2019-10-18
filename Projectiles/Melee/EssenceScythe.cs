using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
namespace CalamityMod.Projectiles.Melee
{
    public class EssenceScythe : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scythe");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 100;
            projectile.height = 78;
            projectile.aiStyle = 18;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.alpha = 55;
            projectile.penetrate = 10;
            projectile.timeLeft = 300;
            projectile.ignoreWater = true;
            aiType = 274;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.5f / 255f, (255 - projectile.alpha) * 0f / 255f, (255 - projectile.alpha) * 0.65f / 255f);
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 173, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            float num472 = projectile.Center.X;
            float num473 = projectile.Center.Y;
            float num474 = 600f;
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
                float num483 = 18f;
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

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 2);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 origin = new Vector2(50f, 39f);
            Color color = Color.White;
            if (projectile.timeLeft < 85)
            {
                byte b2 = (byte)(projectile.timeLeft * 3);
                byte a2 = (byte)(100f * ((float)b2 / 255f));
                color = new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Melee/EssenceScytheGlow"), projectile.Center - Main.screenPosition, null, color, projectile.rotation, origin, 1f, spriteEffects, 0f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(150, 0, 200, 0);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 2;
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300);
            if (target.life <= 0)
            {
                if (projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, ModContent.ProjectileType<EssenceFlame>(), 0, 0, projectile.owner, 0f, 0f);
                }
            }
        }
    }
}
