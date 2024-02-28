using System;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;
using CalamityMod.Graphics.Primitives;

namespace CalamityMod.Projectiles.Rogue
{
    public class WulfrumKnifeProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        internal Color PrimColorMult = Color.White;
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/WulfrumKnife";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public static int Lifetime = 950;
        public float LifetimeCompletion => MathHelper.Clamp((Lifetime - Projectile.timeLeft) / (float)Lifetime, 0f, 1f);
        public float StealthEffectOpacity => MathHelper.Clamp(1 - LifetimeCompletion, 0f, 1f);

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.DamageType = RogueDamageClass.Instance;

            Projectile.timeLeft = Lifetime;
            Projectile.extraUpdates = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1;
        }


        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.velocity *= 0.998f;

            if (Projectile.timeLeft < Lifetime - 100)
                Projectile.velocity.Y += 0.01f;


            if (!Projectile.Calamity().stealthStrike)
            {
                if (Main.rand.NextBool(10))
                {
                    Vector2 dustCenter = Projectile.Center + Projectile.velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(-3f, 3f);

                    Dust chust = Dust.NewDustPerfect(dustCenter, 15, -Projectile.velocity * Main.rand.NextFloat(0.6f, 1.5f), Scale: Main.rand.NextFloat(1f, 1.4f));
                    chust.noGravity = true;

                    if (!Main.rand.NextBool(5))
                        chust.noLightEmittence = true;
                }
            }

            else
            {
                Lighting.AddLight(Projectile.Center, (Main.rand.NextBool() ? Color.GreenYellow : Color.DeepSkyBlue).ToVector3() * StealthEffectOpacity);

                if (Main.rand.NextBool(7))
                {
                    Vector2 center = Projectile.Center + Projectile.velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(-3f, 3f);
                    Vector2 velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedByRandom(MathHelper.Pi / 6f) * Main.rand.NextFloat(4, 10);
                    GeneralParticleHandler.SpawnParticle(new TechyHoloysquareParticle(center, velocity, Main.rand.NextFloat(1f, 2f), Main.rand.NextBool() ? new Color(99, 255, 229) : new Color(25, 132, 247), 25, StealthEffectOpacity));

                }

                if (Main.rand.NextBool(8))
                {
                    Vector2 dustCenter = Projectile.Center + Projectile.velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(-3f, 3f);

                    Dust chust = Dust.NewDustPerfect(dustCenter, 15, -Projectile.velocity * Main.rand.NextFloat(0.6f, 1.5f), Scale: Main.rand.NextFloat(1f, 1.4f));
                    chust.noGravity = true;
                    chust.noLightEmittence = true;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.rand.NextBool())
            {
                Item.NewItem(Projectile.GetSource_DropAsItem(), (int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height, ModContent.ItemType<WulfrumKnife>());
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(WulfrumKnife.TileHitSound, Projectile.Center);

            return base.OnTileCollide(oldVelocity);
        }

        internal Color ColorFunction(float completionRatio)
        {
            float fadeOpacity = (float)Math.Pow(1 - completionRatio, 2) * StealthEffectOpacity;
            return Color.GreenYellow.MultiplyRGB(PrimColorMult) * fadeOpacity;
        }

        internal float WidthFunction(float completionRatio)
        {
            return 9.4f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            float opacitey = StealthEffectOpacity;

            if (Projectile.Calamity().stealthStrike)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(Request<Texture2D>("CalamityMod/ExtraTextures/Trails/BasicTrail"));

                CalamityUtils.DrawChromaticAberration(Vector2.UnitX, 1f, delegate (Vector2 offset, Color colorMod)
                {
                    PrimColorMult = colorMod;
                    PrimitiveSet.Prepare(Projectile.oldPos, new(WidthFunction, ColorFunction, (_) => Projectile.Size + offset, shader: GameShaders.Misc["CalamityMod:TrailStreak"]), 30);
                });



                CalamityUtils.DrawChromaticAberration(Vector2.UnitX, 3f, delegate (Vector2 offset, Color colorMod)
                {
                    Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + offset, null, Color.GreenYellow.MultiplyRGB(colorMod) * opacitey, Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
                });

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);


                opacitey = MathHelper.Clamp(LifetimeCompletion * 8f, 0f, 1f);
                Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * opacitey, Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            }

            else
            {
                opacitey = MathHelper.Clamp(LifetimeCompletion * 15f, 0f, 1f);

                Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * opacitey, Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }

    }
}
