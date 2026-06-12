using System;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class SpringStoolFX : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";

        float rotationFactor = 0.015f;
        public static float EaseOutElastic(float x, float amplitude = 1f, float period = 0.3f)
        {
            if (x == 0f) return 0f;
            if (x == 1f) return 1f;

            amplitude = Math.Max(1f, amplitude);

            float s = (float)(period / (2 * Math.PI) * Math.Asin(1f / amplitude));

            return (float)(
                amplitude *
                Math.Pow(2, -10f * x) *
                Math.Sin((x - s) * (2 * Math.PI) / period)
                + 1f
            );
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 78;
            Projectile.height = 142;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.alpha = 0;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.frame < 4)
            {
                Projectile.frameCounter++;

                if (Projectile.frameCounter >= 4)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;

                    if (Projectile.frame > 4)
                    {
                        {
                            Projectile.frame = 4; // clamp to last frame
                        }
                    }
                }
            }

            float progress = MathHelper.Clamp(Projectile.ai[0] / 70f, 0f, 1f);

            Projectile.ai[1] = EaseOutElastic(progress);


            Projectile.ai[0]++;

            if (Projectile.ai[0] < 3f) // Stick to bottom of player initially
            {
                Projectile.Center = player.Bottom + new Vector2(0f, -25f);
            }

            else
            {
                // Begin rotating after initial launch at an increasingly fast rate
                rotationFactor *= 1.033f;
                Projectile.rotation -= rotationFactor; 
            }

            if (Projectile.ai[0] == 4f)
            {
                {
                    SoundStyle boing = new("CalamityMod/Sounds/Item/Springy");
                    SoundEngine.PlaySound(boing with { Volume = 1f, PitchVariance = 0.1f });
                }
            }

            if (Projectile.ai[0] == 16) // About when the spring retracts into itself, spawn dusts and sparks
            {
                for (int k = 0; k < 6; k++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? 7 : 8);
                    dust.scale = Main.rand.NextFloat(0.8f, 1.3f);
                    dust.velocity = new Vector2(8, 2).RotatedByRandom(100) * Main.rand.NextFloat(0.25f, 1.2f);
                    dust.noGravity = false;
                    Particle spark = new AltLineParticle(Projectile.Center, new Vector2(8, 8).RotatedByRandom(100) * Main.rand.NextFloat(0.4f, 0.9f), true, 15, 0.8f, Color.Lerp(Color.LightGray, Color.Brown, Main.rand.NextFloat(0.3f, 0.7f)) * 0.75f);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }

            if (Projectile.ai[0] >= 28f) // Begin fading
            {
                Projectile.alpha += 10;
                Projectile.scale *= 0.98f;
                
            }

            if (Projectile.alpha >= 255 || !player.active || player.dead)
                Projectile.Kill();
        }

        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            int frameHeight = texture.Height / Main.projFrames[Type];
            Rectangle sourceRect = new Rectangle(0, frameHeight * Projectile.frame, texture.Width, frameHeight);
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, frameHeight * 0.5f);
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            Vector2 scale = new Vector2(Projectile.scale, Projectile.scale * Projectile.ai[1]);

            Main.EntitySpriteDraw(texture, drawPos, sourceRect, Projectile.GetAlpha(lightColor), Projectile.rotation + (Main.player[Projectile.owner].gravDir == -1 ? MathHelper.Pi : 0), drawOrigin, scale, SpriteEffects.None, 0);
            return false;    
        }
    }
}
