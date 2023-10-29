using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Enemy
{
    public class StormMarkHostile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Enemy";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 900;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Color newColor3 = new Color(255, 255, 255);
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = -1;
                SoundEngine.PlaySound(SoundID.Item60, Projectile.Center);
            }
            if (Projectile.localAI[1] < 30f)
            {
                int totalDust = Projectile.ai[0] != 0f ? 2 : 1;
                for (int i = 0; i < totalDust; i++)
                {
                    float lerpvalue = -0.5f;
                    float lerpvalue2 = 0.9f;
                    float randomLerp = Main.rand.NextFloat();
                    Vector2 dustMovement = new Vector2(MathHelper.Lerp(0.1f, 1f, Main.rand.NextFloat()), MathHelper.Lerp(lerpvalue, lerpvalue2, randomLerp));
                    dustMovement.X *= MathHelper.Lerp(2.2f, 0.6f, randomLerp);
                    dustMovement.X *= -1f;
                    Vector2 dustMovement2 = new Vector2(2f, 10f);
                    Vector2 position4 = Projectile.Center + new Vector2(60f, Projectile.ai[0] != 0f ? 800f : 200f) * dustMovement * 0.5f + dustMovement2;
                    Dust stormy = Main.dust[Dust.NewDust(position4, 0, 0, 16, 0f, 0f, 0, default, 0.5f)];
                    stormy.position = position4;
                    stormy.customData = Projectile.Center + dustMovement2;
                    stormy.fadeIn = 1f;
                    stormy.scale = 0.3f;
                    if (dustMovement.X > -1.2f)
                    {
                        stormy.velocity.X = 1f + Main.rand.NextFloat();
                    }
                    stormy.velocity.Y = Main.rand.NextFloat() * -0.5f - 1f;
                }
            }
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 0.8f;
                Projectile.direction = 1;
                Point projCenter = Projectile.Center.ToTileCoordinates();
                Projectile.Center = new Vector2((float)(projCenter.X * 16 + 8), (float)(projCenter.Y * 16 + 8));
            }
            Projectile.rotation = Projectile.localAI[1] / 40f * 6.28318548f * (float)Projectile.direction;
            if (Projectile.localAI[1] < 33f)
            {
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 8;
                }
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }
            if (Projectile.localAI[1] > 103f)
            {
                if (Projectile.alpha < 255)
                {
                    Projectile.alpha += 16;
                }
                if (Projectile.alpha > 255)
                {
                    Projectile.alpha = 255;
                }
            }
            if (Projectile.alpha == 0)
            {
                Lighting.AddLight(Projectile.Center, newColor3.ToVector3() * 0.5f);
            }
            for (int j = 0; j < 2; j++)
            {
                if (Main.rand.NextBool(10))
                {
                    Vector2 dustVel = Vector2.UnitY.RotatedBy((double)((float)j * 3.14159274f), default).RotatedBy((double)Projectile.rotation, default);
                    Dust stormDust = Main.dust[Dust.NewDust(Projectile.Center, 0, 0, 16, 0f, 0f, 225, newColor3, 1f)];
                    stormDust.noGravity = true;
                    stormDust.noLight = true;
                    stormDust.scale = Projectile.Opacity * Projectile.localAI[0];
                    stormDust.position = Projectile.Center;
                    stormDust.velocity = dustVel * 2.5f;
                }
            }
            for (int k = 0; k < 2; k++)
            {
                if (Main.rand.NextBool(10))
                {
                    Vector2 dustVel2 = Vector2.UnitY.RotatedBy((double)((float)k * 3.14159274f), default);
                    Dust stormDust2 = Main.dust[Dust.NewDust(Projectile.Center, 0, 0, 16, 0f, 0f, 225, newColor3, 1.5f)];
                    stormDust2.noGravity = true;
                    stormDust2.noLight = true;
                    stormDust2.scale = Projectile.Opacity * Projectile.localAI[0];
                    stormDust2.position = Projectile.Center;
                    stormDust2.velocity = dustVel2 * 2.5f;
                }
            }
            if (Projectile.localAI[1] < 33f || Projectile.localAI[1] > 87f)
            {
                Projectile.scale = Projectile.Opacity / 2f * Projectile.localAI[0];
            }
            Projectile.velocity = Vector2.Zero;
            Projectile.localAI[1] += 1f;
            if (Projectile.localAI[1] == 60f && Projectile.owner == Main.myPlayer)
            {
                int projectileDamage = Projectile.ai[0] != 0f ? (int)Projectile.ai[0] : Main.expertMode ? 25 : 40;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TornadoHostile>(), projectileDamage, 3f, Projectile.owner, 0f, Projectile.ai[1]);
            }
            if (Projectile.localAI[1] >= 120f)
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color originalColor = Lighting.GetColor((int)((double)Projectile.position.X + (double)Projectile.width * 0.5) / 16, (int)(((double)Projectile.position.Y + (double)Projectile.height * 0.5) / 16.0));
            Vector2 drawPos = Projectile.position + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Texture2D texture2D27 = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle rectangl = texture2D27.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Color alphaColor = Projectile.GetAlpha(originalColor);
            Vector2 halfRect = rectangl.Size() / 2f;
            Color whiteColor = Main.hslToRgb(0.25f, 1f, 1f).MultiplyRGBA(new Color(255, 255, 255, 0));
            Main.spriteBatch.Draw(texture2D27, drawPos, new Microsoft.Xna.Framework.Rectangle?(rectangl), whiteColor, 0f, halfRect, new Vector2(1f, 5f) * Projectile.scale * 2f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture2D27, drawPos, new Microsoft.Xna.Framework.Rectangle?(rectangl), alphaColor, Projectile.rotation, halfRect, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture2D27, drawPos, new Microsoft.Xna.Framework.Rectangle?(rectangl), alphaColor, 0f, halfRect, new Vector2(1f, 8f) * Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
