using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class CircletMark : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 900;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
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
                for (int i = 0; i < 1; i++)
                {
                    float lerpvalue1 = -0.5f;
                    float lerpvalue2 = 0.9f;
                    float randomLerp = Main.rand.NextFloat();
                    Vector2 dustMovement = new Vector2(MathHelper.Lerp(0.1f, 1f, Main.rand.NextFloat()), MathHelper.Lerp(lerpvalue1, lerpvalue2, randomLerp));
                    dustMovement.X *= MathHelper.Lerp(2.2f, 0.6f, randomLerp);
                    dustMovement.X *= -1f;
                    Vector2 dustMovement2 = new Vector2(2f, 10f);
                    Vector2 position4 = Projectile.Center + new Vector2(60f, 200f) * dustMovement * 0.5f + dustMovement2;
                    Dust dust = Main.dust[Dust.NewDust(position4, 0, 0, 269, 0f, 0f, 0, default, 0.5f)];
                    dust.position = position4;
                    dust.customData = Projectile.Center + dustMovement2;
                    dust.fadeIn = 1f;
                    dust.scale = 0.3f;
                    if (dustMovement.X > -1.2f)
                    {
                        dust.velocity.X = 1f + Main.rand.NextFloat();
                    }
                    dust.velocity.Y = Main.rand.NextFloat() * -0.5f - 1f;
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
                    Dust dusty = Main.dust[Dust.NewDust(Projectile.Center, 0, 0, 269, 0f, 0f, 225, newColor3, 1f)];
                    dusty.noGravity = true;
                    dusty.noLight = true;
                    dusty.scale = Projectile.Opacity * Projectile.localAI[0];
                    dusty.position = Projectile.Center;
                    dusty.velocity = dustVel * 2.5f;
                }
            }
            for (int k = 0; k < 2; k++)
            {
                if (Main.rand.NextBool(10))
                {
                    Vector2 dustVel2 = Vector2.UnitY.RotatedBy((double)((float)k * 3.14159274f), default);
                    Dust dustier = Main.dust[Dust.NewDust(Projectile.Center, 0, 0, 269, 0f, 0f, 225, newColor3, 1.5f)];
                    dustier.noGravity = true;
                    dustier.noLight = true;
                    dustier.scale = Projectile.Opacity * Projectile.localAI[0];
                    dustier.position = Projectile.Center;
                    dustier.velocity = dustVel2 * 2.5f;
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
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CircletTornado>(), Projectile.damage, 2f, Projectile.owner, 0f, 0f);
            }
            if (Projectile.localAI[1] >= 120f)
            {
                Projectile.Kill();
                return;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color originalColor = Lighting.GetColor((int)((double)Projectile.position.X + (double)Projectile.width * 0.5) / 16, (int)(((double)Projectile.position.Y + (double)Projectile.height * 0.5) / 16.0));
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Vector2 drawPos = Projectile.position + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Texture2D texture2D27 = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle rectangle = texture2D27.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Color alphaColor = Projectile.GetAlpha(originalColor);
            Vector2 origin7 = rectangle.Size() / 2f;
            Color sandyColor = Main.hslToRgb(0.25f, 1f, 1f).MultiplyRGBA(new Color(255, 255, 255, 0));
            Main.spriteBatch.Draw(texture2D27, drawPos, new Microsoft.Xna.Framework.Rectangle?(rectangle), sandyColor, 0f, origin7, new Vector2(1f, 5f) * Projectile.scale * 2f, spriteEffects, 0);
            Main.spriteBatch.Draw(texture2D27, drawPos, new Microsoft.Xna.Framework.Rectangle?(rectangle), alphaColor, Projectile.rotation, origin7, Projectile.scale, spriteEffects, 0);
            Main.spriteBatch.Draw(texture2D27, drawPos, new Microsoft.Xna.Framework.Rectangle?(rectangle), alphaColor, 0f, origin7, new Vector2(1f, 8f) * Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}
