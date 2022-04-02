using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class SandMark : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mark");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 900;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.minion = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            Color newColor3 = new Color(255, 255, 255);
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = -1;
                Main.PlaySound(SoundID.Item60, projectile.Center);
            }
            if (projectile.localAI[1] < 30f)
            {
                for (int num1134 = 0; num1134 < 1; num1134++)
                {
                    float value79 = -0.5f;
                    float value80 = 0.9f;
                    float amount4 = Main.rand.NextFloat();
                    Vector2 value81 = new Vector2(MathHelper.Lerp(0.1f, 1f, Main.rand.NextFloat()), MathHelper.Lerp(value79, value80, amount4));
                    value81.X *= MathHelper.Lerp(2.2f, 0.6f, amount4);
                    value81.X *= -1f;
                    Vector2 value82 = new Vector2(2f, 10f);
                    Vector2 position4 = projectile.Center + new Vector2(60f, 200f) * value81 * 0.5f + value82;
                    Dust dust34 = Main.dust[Dust.NewDust(position4, 0, 0, 269, 0f, 0f, 0, default, 0.5f)];
                    dust34.position = position4;
                    dust34.customData = projectile.Center + value82;
                    dust34.fadeIn = 1f;
                    dust34.scale = 0.3f;
                    if (value81.X > -1.2f)
                    {
                        dust34.velocity.X = 1f + Main.rand.NextFloat();
                    }
                    dust34.velocity.Y = Main.rand.NextFloat() * -0.5f - 1f;
                }
            }
            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 0.8f;
                projectile.direction = 1;
                Point point9 = projectile.Center.ToTileCoordinates();
                projectile.Center = new Vector2((float)(point9.X * 16 + 8), (float)(point9.Y * 16 + 8));
            }
            projectile.rotation = projectile.localAI[1] / 40f * 6.28318548f * (float)projectile.direction;
            if (projectile.localAI[1] < 33f)
            {
                if (projectile.alpha > 0)
                {
                    projectile.alpha -= 8;
                }
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                }
            }
            if (projectile.localAI[1] > 103f)
            {
                if (projectile.alpha < 255)
                {
                    projectile.alpha += 16;
                }
                if (projectile.alpha > 255)
                {
                    projectile.alpha = 255;
                }
            }
            if (projectile.alpha == 0)
            {
                Lighting.AddLight(projectile.Center, newColor3.ToVector3() * 0.5f);
            }
            for (int num1135 = 0; num1135 < 2; num1135++)
            {
                if (Main.rand.NextBool(10))
                {
                    Vector2 value83 = Vector2.UnitY.RotatedBy((double)((float)num1135 * 3.14159274f), default).RotatedBy((double)projectile.rotation, default);
                    Dust dust35 = Main.dust[Dust.NewDust(projectile.Center, 0, 0, 269, 0f, 0f, 225, newColor3, 1f)];
                    dust35.noGravity = true;
                    dust35.noLight = true;
                    dust35.scale = projectile.Opacity * projectile.localAI[0];
                    dust35.position = projectile.Center;
                    dust35.velocity = value83 * 2.5f;
                }
            }
            for (int num1136 = 0; num1136 < 2; num1136++)
            {
                if (Main.rand.NextBool(10))
                {
                    Vector2 value84 = Vector2.UnitY.RotatedBy((double)((float)num1136 * 3.14159274f), default);
                    Dust dust36 = Main.dust[Dust.NewDust(projectile.Center, 0, 0, 269, 0f, 0f, 225, newColor3, 1.5f)];
                    dust36.noGravity = true;
                    dust36.noLight = true;
                    dust36.scale = projectile.Opacity * projectile.localAI[0];
                    dust36.position = projectile.Center;
                    dust36.velocity = value84 * 2.5f;
                }
            }
            if (projectile.localAI[1] < 33f || projectile.localAI[1] > 87f)
            {
                projectile.scale = projectile.Opacity / 2f * projectile.localAI[0];
            }
            projectile.velocity = Vector2.Zero;
            projectile.localAI[1] += 1f;
            if (projectile.localAI[1] == 60f && projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<SandTornado>(), projectile.damage, 2f, projectile.owner, 0f, 0f);
            }
            if (projectile.localAI[1] >= 120f)
            {
                projectile.Kill();
                return;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Color color25 = Lighting.GetColor((int)((double)projectile.position.X + (double)projectile.width * 0.5) / 16, (int)(((double)projectile.position.Y + (double)projectile.height * 0.5) / 16.0));
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Vector2 vector38 = projectile.position + new Vector2((float)projectile.width, (float)projectile.height) / 2f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
            Texture2D texture2D27 = Main.projectileTexture[projectile.type];
            Rectangle rectangle11 = texture2D27.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            Color alpha5 = projectile.GetAlpha(color25);
            Vector2 origin7 = rectangle11.Size() / 2f;
            Color color47 = Main.hslToRgb(0.25f, 1f, 1f).MultiplyRGBA(new Color(255, 255, 255, 0));
            Main.spriteBatch.Draw(texture2D27, vector38, new Microsoft.Xna.Framework.Rectangle?(rectangle11), color47, 0f, origin7, new Vector2(1f, 5f) * projectile.scale * 2f, spriteEffects, 0f);
            Main.spriteBatch.Draw(texture2D27, vector38, new Microsoft.Xna.Framework.Rectangle?(rectangle11), alpha5, projectile.rotation, origin7, projectile.scale, spriteEffects, 0f);
            Main.spriteBatch.Draw(texture2D27, vector38, new Microsoft.Xna.Framework.Rectangle?(rectangle11), alpha5, 0f, origin7, new Vector2(1f, 8f) * projectile.scale, spriteEffects, 0f);
            return false;
        }
    }
}
