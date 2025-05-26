using System;
using CalamityMod.DataStructures;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class BlazingPhantomBlade : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";

        internal const float StartingScale = 1f;

        internal const float SunlightBladeMaxVelocity = DefiledGreatsword.ShootSpeed * 2f;
        internal const int SunlightBladePierce = 10;

        internal const float FadeInTime = 30f;
        internal const float FadeOutTime = 30f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.NoMeleeSpeedVelocityScaling[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1; // Blazing blades and hyper blades hit four times, sunlight blades hit ten times.
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.timeLeft = 220;
            Projectile.noEnchantmentVisuals = true;
            Projectile.scale = StartingScale;
        }

        public override void AI()
        {
            // True Night's Edge AI
            //float fadeInTime = 30f;
            //float fadeOutTime = 30f;

            // Defiled Greatsword has three variants for this projectile
            float fullyVisibleDuration = Projectile.ai[1];
            bool hyperBlade = fullyVisibleDuration == DefiledGreatsword.ProjectileFullyVisibleDuration + DefiledGreatsword.ProjectileFullyVisibleDurationIncreasePerAdditionalProjectile;
            bool sunlightBlade = fullyVisibleDuration == DefiledGreatsword.ProjectileFullyVisibleDuration + DefiledGreatsword.ProjectileFullyVisibleDurationIncreasePerAdditionalProjectile * 2f;

            float timeBeforeFadeOut = fullyVisibleDuration + FadeInTime;
            float projectileDuration = timeBeforeFadeOut + FadeOutTime;

            if (Projectile.localAI[0] == 0f)
                SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);

            Projectile.localAI[0] += 1f;
            Projectile.Opacity = Utils.Remap(Projectile.localAI[0], 0f, fullyVisibleDuration, 0f, 1f) * Utils.Remap(Projectile.localAI[0], timeBeforeFadeOut, projectileDuration, 1f, 0f);
            if (Projectile.localAI[0] >= projectileDuration)
            {
                Projectile.localAI[1] = 1f;
                Projectile.Kill();
                return;
            }

            Player player = Main.player[Projectile.owner];
            Projectile.direction = (Projectile.spriteDirection = (int)Projectile.ai[0]);

            Projectile.localAI[1] += 1f;
            Projectile.rotation += Projectile.ai[0] * MathHelper.TwoPi * (4f + Projectile.Opacity * 4f) / 90f;
            Projectile.scale = Utils.Remap(Projectile.localAI[0], fullyVisibleDuration + 2f, projectileDuration, 1.12f, 1f) * Projectile.ai[2] * StartingScale;
            float randomDustSpawnLocation = Projectile.rotation + Main.rand.NextFloatDirection() * MathHelper.PiOver2 * 0.7f;
            Vector2 dustPosition = Projectile.Center + randomDustSpawnLocation.ToRotationVector2() * 84f * Projectile.scale;
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustPerfect(dustPosition, DustID.Venom, null, 100, default, 1.4f);
                dust.noGravity = true;
                dust.velocity *= 0f;
                dust.fadeIn = 1.5f;
            }

            for (int i = 0; (float)i < 3f * Projectile.Opacity; i++)
            {
                Vector2 dustVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitX);
                int dustType1 = sunlightBlade ? DustID.IchorTorch : hyperBlade ? DustID.CursedTorch : DustID.CrimsonTorch;
                int dustType2 = sunlightBlade ? DustID.YellowTorch : hyperBlade ? DustID.GreenTorch : DustID.RedTorch;
                int dustType = ((Main.rand.NextFloat() < Projectile.Opacity) ? dustType1 : dustType2);
                Dust dust = Dust.NewDustPerfect(dustPosition, dustType, Projectile.velocity * 0.2f + dustVelocity * 3f, 100, default, 1.4f);
                dust.noGravity = true;
                dust.customData = Projectile.Opacity * 0.2f;
            }

            // Home in on targets
            if (!sunlightBlade)
            {
                CalamityUtils.HomeInOnNPC(Projectile, true, hyperBlade ? 500f : 250f, hyperBlade ? 16f : 8f, hyperBlade ? 10f : 15f);
            }

            // Sunlight Blades accelerate
            else
            {
                if (Projectile.velocity.Length() < SunlightBladeMaxVelocity)
                {
                    Projectile.velocity *= 1.05f;
                    if (Projectile.velocity.Length() > SunlightBladeMaxVelocity)
                    {
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= SunlightBladeMaxVelocity;
                    }
                }
            }

            // Enchantment visual bullshit
            Vector2 boxPosition = Projectile.position;
            int boxWidth = Projectile.width;
            int boxHeight = Projectile.height;
            for (float i = -MathHelper.PiOver4; i <= MathHelper.PiOver4; i += MathHelper.PiOver2)
            {
                Rectangle rect = Utils.CenteredRectangle(Projectile.Center + (Projectile.rotation + i).ToRotationVector2() * 70f * Projectile.scale, new Vector2(60f * Projectile.scale, 60f * Projectile.scale));
                Projectile.EmitEnchantmentVisualsAt(rect.TopLeft(), rect.Width, rect.Height);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 distanceFromTarget = targetHitbox.ClosestPointInRect(Projectile.Center) - Projectile.Center;
            distanceFromTarget.SafeNormalize(Vector2.UnitX);
            float projectileSize = 100f * Projectile.scale;
            if (distanceFromTarget.Length() < projectileSize && Collision.CanHit(Projectile.Center, 0, 0, targetHitbox.Center.ToVector2(), 0, 0))
                return true;

            return null;
        }

        public override void CutTiles()
        {
            Vector2 startPoint = (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 60f * Projectile.scale;
            Vector2 endPoint = (Projectile.rotation + MathHelper.PiOver4).ToRotationVector2() * 60f * Projectile.scale;
            float projectileSize = 60f * Projectile.scale;
            Utils.PlotTileLine(Projectile.Center + startPoint, Projectile.Center + endPoint, projectileSize, DelegateMethods.CutTiles);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Defiled Greatsword has three variants for this projectile
            float fullyVisibleDuration = Projectile.ai[1];
            bool hyperBlade = fullyVisibleDuration == DefiledGreatsword.ProjectileFullyVisibleDuration + DefiledGreatsword.ProjectileFullyVisibleDurationIncreasePerAdditionalProjectile;
            bool sunlightBlade = fullyVisibleDuration == DefiledGreatsword.ProjectileFullyVisibleDuration + DefiledGreatsword.ProjectileFullyVisibleDurationIncreasePerAdditionalProjectile * 2f;

            Texture2D asset = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Microsoft.Xna.Framework.Rectangle rectangle = asset.Frame(1, 4);
            Vector2 origin = rectangle.Size() / 2f;
            float num = Projectile.scale * 1.1f;
            SpriteEffects effects = ((!(Projectile.ai[0] >= 0f)) ? SpriteEffects.FlipVertically : SpriteEffects.None);
            float num2 = 0.975f;
            float fromValue = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).ToVector3().Length() / (float)Math.Sqrt(3D);
            fromValue = Utils.Remap(fromValue, 0.2f, 1f, 0f, 1f);
            float num3 = MathHelper.Min(0.15f + fromValue * 0.85f, Utils.Remap(Projectile.localAI[0], 30f, 96f, 1f, 0f));
            float num4 = 2f;
            for (float num5 = num4; num5 >= 0f; num5 -= 1f)
            {
                if (!(Projectile.oldPos[(int)num5] == Vector2.Zero))
                {
                    Vector2 vectorScale = Projectile.Center - Projectile.velocity * 0.5f * num5;
                    float num6 = Projectile.oldRot[(int)num5] + Projectile.ai[0] * MathHelper.TwoPi * 0.1f * (0f - num5);
                    Vector2 position = vectorScale - Main.screenPosition;
                    float num7 = 1f - num5 / num4;
                    float num8 = Projectile.Opacity * num7 * num7 * 0.85f;
                    float amount = Projectile.Opacity * Projectile.Opacity;

                    Color colorOne = Color.Lerp(new Color(sunlightBlade ? 20 : hyperBlade ? 60 : 40, sunlightBlade ? 40 : hyperBlade ? 20 : 60, sunlightBlade ? 60 : hyperBlade ? 40 : 20, 120),
                        new Color(sunlightBlade ? 225 : hyperBlade ? 25 : 225, sunlightBlade ? 225 : hyperBlade ? 225 : 40, sunlightBlade ? 25 : hyperBlade ? 40 : 25, 120), amount);

                    Main.spriteBatch.Draw(asset, position, rectangle, colorOne * num3 * num8, num6 + Projectile.ai[0] * MathHelper.PiOver4 * -1f, origin, num * num2, effects, 0f);

                    Color colorTwo = Color.Lerp(new Color(sunlightBlade ? 40 : hyperBlade ? 180 : 80, sunlightBlade ? 80 : hyperBlade ? 40 : 180, sunlightBlade ? 180 : hyperBlade ? 80 : 40),
                        new Color(sunlightBlade ? 255 : hyperBlade ? 100 : 155, sunlightBlade ? 255 : hyperBlade ? 255 : 100, sunlightBlade ? 100 : hyperBlade ? 155 : 255), amount);

                    Color color3 = Color.White * num8 * 0.5f;
                    color3.A = (byte)((float)(int)color3.A * (1f - num3));

                    Color color4 = color3 * num3 * 0.5f;
                    if (sunlightBlade)
                    {
                        color4.B = (byte)((float)(int)color4.B * num3);
                        color4.R = (byte)((float)(int)color4.R * (0.25f + num3 * 0.75f));
                    }
                    else if (hyperBlade)
                    {
                        color4.G = (byte)((float)(int)color4.G * num3);
                        color4.B = (byte)((float)(int)color4.B * (0.25f + num3 * 0.75f));
                    }
                    else
                    {
                        color4.R = (byte)((float)(int)color4.R * num3);
                        color4.G = (byte)((float)(int)color4.G * (0.25f + num3 * 0.75f));
                    }

                    float num9 = 3f;
                    for (float num10 = -MathHelper.TwoPi + MathHelper.TwoPi / num9; num10 < 0f; num10 += MathHelper.TwoPi / num9)
                    {
                        float num11 = Utils.Remap(num10, -MathHelper.TwoPi, 0f, 0f, 0.5f);
                        Main.spriteBatch.Draw(asset, position, rectangle, color4 * 0.15f * num11, num6 + Projectile.ai[0] * 0.01f + num10, origin, num, effects, 0f);

                        Main.spriteBatch.Draw(asset, position, rectangle, Color.Lerp(new Color(sunlightBlade ? 30 : hyperBlade ? 160 : 80, sunlightBlade ? 80 : hyperBlade ? 30 : 160, sunlightBlade ? 160 : hyperBlade ? 80 : 30),
                            new Color(sunlightBlade ? 200 : hyperBlade ? 255 : 200, sunlightBlade ? 200 : hyperBlade ? 200 : 0, sunlightBlade ? 255 : hyperBlade ? 0 : 255), amount) * fromValue * num8 * num11,
                            num6 + num10, origin, num * 0.8f, effects, 0f);

                        Main.spriteBatch.Draw(asset, position, rectangle, colorTwo * fromValue * num8 * MathHelper.Lerp(0.05f, 0.4f, fromValue) * num11, num6 + num10, origin, num * num2, effects, 0f);

                        Main.spriteBatch.Draw(asset, position, asset.Frame(1, 4, 0, 3),
                            new Color(sunlightBlade ? 150 : hyperBlade ? 255 : 200, sunlightBlade ? 255 : hyperBlade ? 200 : 150, sunlightBlade ? 200 : hyperBlade ? 150 : 255) * MathHelper.Lerp(0.05f, 0.5f, fromValue) * num8 * num11,
                            num6 + num10, origin, num, effects, 0f);
                    }

                    Main.spriteBatch.Draw(asset, position, rectangle, color4 * 0.15f, num6 + Projectile.ai[0] * 0.01f, origin, num, effects, 0f);

                    Main.spriteBatch.Draw(asset, position, rectangle, Color.Lerp(new Color(sunlightBlade ? 30 : hyperBlade ? 160 : 80, sunlightBlade ? 80 : hyperBlade ? 30 : 160, sunlightBlade ? 160 : hyperBlade ? 80 : 30),
                        new Color(sunlightBlade ? 255 : hyperBlade ? 0 : 255, sunlightBlade ? 255 : hyperBlade ? 255 : 100, sunlightBlade ? 0 : hyperBlade ? 100 : 0), amount) * num3 * num8, num6, origin, num * 0.8f, effects, 0f);

                    Main.spriteBatch.Draw(asset, position, rectangle, colorTwo * fromValue * num8 * MathHelper.Lerp(0.05f, 0.4f, num3), num6, origin, num * num2, effects, 0f);

                    Main.spriteBatch.Draw(asset, position, asset.Frame(1, 4, 0, 3),
                        new Color(sunlightBlade ? 255 : hyperBlade ? 100 : 255, sunlightBlade ? 255 : hyperBlade ? 255 : 75, sunlightBlade ? 75 : hyperBlade ? 75 : 100) * MathHelper.Lerp(0.05f, 0.5f, num3) * num8,
                        num6, origin, num, effects, 0f);
                }
            }

            float num12 = 1f - Projectile.localAI[0] * 1f / 80f;
            if (num12 < 0.5f)
                num12 = 0.5f;

            float num13 = MathHelper.Min(num3, MathHelper.Lerp(1f, fromValue, Utils.Remap(Projectile.localAI[0], 0f, 80f, 0f, 1f)));

            Texture2D value = TextureAssets.Extra[98].Value;
            SpriteEffects dir = SpriteEffects.None;
            Vector2 drawpos = Projectile.Center - Main.screenPosition + (Projectile.rotation + (MathHelper.Pi / (20f / 3f)) * Projectile.ai[0]).ToRotationVector2() * ((float)asset.Width * 0.5f - 4f) * num * num12;
            Color drawColor = new Color(255, 255, 255, 0) * Projectile.Opacity * 0.5f * num13;
            Color shineColor = new Color(sunlightBlade ? 255 : hyperBlade ? 50 : 255, sunlightBlade ? 255 : hyperBlade ? 255 : 75, sunlightBlade ? 50 : hyperBlade ? 75 : 50) * num13;
            Color color = shineColor * Projectile.Opacity * 0.5f;
            color.A = 0;
            float flareCounter = Projectile.Opacity;
            float fadeInStart = 0f;
            float fadeInEnd = 1f;
            float fadeOutStart = 1f;
            float fadeOutEnd = 2f;
            float rotation = MathHelper.PiOver4;
            Vector2 scale = new Vector2(2f, 2f);
            Vector2 fatness = Vector2.One;
            Vector2 origin2 = value.Size() / 2f;
            Color color2 = drawColor * 0.5f;
            float num14 = Utils.GetLerpValue(fadeInStart, fadeInEnd, flareCounter, clamped: true) * Utils.GetLerpValue(fadeOutEnd, fadeOutStart, flareCounter, clamped: true);
            Vector2 vector = new Vector2(fatness.X * 0.5f, scale.X) * num14;
            Vector2 vector2 = new Vector2(fatness.Y * 0.5f, scale.Y) * num14;
            color *= num14;
            color2 *= num14;
            Main.EntitySpriteDraw(value, drawpos, null, color, MathHelper.PiOver2 + rotation, origin2, vector, dir);
            Main.EntitySpriteDraw(value, drawpos, null, color, 0f + rotation, origin2, vector2, dir);
            Main.EntitySpriteDraw(value, drawpos, null, color2, MathHelper.PiOver2 + rotation, origin2, vector * 0.6f, dir);
            Main.EntitySpriteDraw(value, drawpos, null, color2, 0f + rotation, origin2, vector2 * 0.6f, dir);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Venom, 120);
            target.AddBuff(BuffID.OnFire3, 120);

            bool sunlightBlade = Projectile.ai[1] == DefiledGreatsword.ProjectileFullyVisibleDuration + DefiledGreatsword.ProjectileFullyVisibleDurationIncreasePerAdditionalProjectile * 2f;
            if (Projectile.numHits >= (sunlightBlade ? SunlightBladePierce : 3))
            {
                Projectile.localAI[0] = Projectile.ai[1] + FadeInTime;
            }
        }

        public override bool? CanDamage() => Projectile.localAI[0] > Projectile.ai[1] + FadeInTime ? false : null;
    }
}
