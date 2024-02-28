using System;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles.Melee
{
    public class WulfrumScrew : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        internal Color PrimColorMult = Color.White;


        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 60;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public static int Lifetime = 950;
        public float LifetimeCompletion => MathHelper.Clamp((Lifetime - Projectile.timeLeft) / (float)Lifetime, 0f, 1f);
        public ref float BazingaTime => ref Projectile.ai[0];
        public ref float AlreadyBazinged => ref Projectile.ai[1];
        public static float BazingaTimeMax = 120f;
        public float BazingaTimeCompletion => (BazingaTimeMax - BazingaTime) / BazingaTimeMax;
        public float FadePercent => Math.Clamp(Projectile.timeLeft / FadeTime, 0f, 1f);
        public static float FadeTime => 30f;
        public Player Owner => Main.player[Projectile.owner];

        public static Asset<Texture2D> SheenTex;

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Melee;

            Projectile.timeLeft = Lifetime;
            Projectile.extraUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1;
            Projectile.scale = 1.2f;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.velocity *= 0.998f;

            if (Projectile.timeLeft < Lifetime - 100 && BazingaTime == 0)
                Projectile.velocity.Y += 0.01f;


            if (BazingaTime > 0)
            {
                BazingaTime--;

                if (BazingaTime == BazingaTimeMax - 2)
                    Owner.Calamity().GeneralScreenShakePower = 0;

                Vector2 dustCenter = Projectile.Center + Projectile.velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(-3f, 3f);

                Dust chust = Dust.NewDustPerfect(dustCenter, 15, -Projectile.velocity * Main.rand.NextFloat(0.6f, 1.5f), Scale: Main.rand.NextFloat(1f, 1.4f));
                chust.noGravity = true;

                if (!Main.rand.NextBool(5))
                    chust.noLightEmittence = true;
            }

            if (Projectile.Center.Distance(Owner.MountedCenter) > 1300 && Projectile.timeLeft > FadeTime)
                Projectile.timeLeft = (int)FadeTime;

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(WulfrumKnife.TileHitSound, Projectile.Center);
            

            bool screwRegained = false;

            //50% chance to gain back the screw on kill.
            if (target.life - hit.Damage <= 0)
            {
                if (Main.rand.NextBool() && Main.myPlayer == Owner.whoAmI)
                {
                    if (Owner.HeldItem.ModItem is WulfrumScrewdriver screwdriver && !screwdriver.ScrewStored)
                    {
                        WulfrumScrewdriver.ScrewStart = new Vector3(Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 4f * Main.rand.NextFloat() - Main.screenPosition, Projectile.rotation);
                        WulfrumScrewdriver.ScrewTimer = WulfrumScrewdriver.ScrewTime;
                        screwdriver.ScrewStored = true;

                        SoundEngine.PlaySound(SoundID.Item156);

                        screwRegained = true;
                    }
                }
            }

            if (!screwRegained && Main.netMode != NetmodeID.Server)
            {
                Gore screwGore = Gore.NewGorePerfect(Projectile.GetSource_Death(), Projectile.position, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(4f, 6f) + Projectile.velocity * 0.7f, Mod.Find<ModGore>("WulfrumScrewGore").Type);
                screwGore.timeLeft = 20;
            }

            //Screenstun
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

            if (BazingaTime > 0)
                SoundEngine.PlaySound(CommonCalamitySounds.WulfrumNPCDeathSound, Projectile.Center);

            SoundEngine.PlaySound(WulfrumKnife.TileHitSound, Projectile.Center);
            if (Main.netMode != NetmodeID.Server)
            {
                Gore screwGore = Gore.NewGorePerfect(Projectile.GetSource_Death(), Projectile.position, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(1f, 3f) + Projectile.velocity * 0.7f, Mod.Find<ModGore>("WulfrumScrewGore").Type);
                screwGore.timeLeft = 20;
            }



            return base.OnTileCollide(oldVelocity);
        }

        internal Color ColorFunction(float completionRatio)
        {
            float fadeOpacity = (float)Math.Pow(1 - completionRatio, 2) * (float)Math.Pow(1 - BazingaTimeCompletion, 0.4f);
            return Color.GreenYellow.MultiplyRGB(PrimColorMult) * fadeOpacity;
        }

        internal float WidthFunction(float completionRatio)
        {
            return 9.4f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            
            float distanceFromAim = Projectile.Center.ShortestDistanceToLine(Owner.MountedCenter, Main.MouseWorld);
            float distanceFromPlayerAcrossSightLine = (Owner.MountedCenter - Projectile.Center.ClosestPointOnLine(Owner.MountedCenter, Main.MouseWorld)).Length();

            float opacity = MathHelper.Clamp(1f - distanceFromAim / 90f, 0f, 1f) * (1f -  Math.Clamp((float)Math.Pow(distanceFromPlayerAcrossSightLine / 300f, 9f) , 0f, 1f));

            //Draw a sightline before the player hits it.
            if (Owner.whoAmI == Main.myPlayer && BazingaTime == 0 && opacity > 0)
            {
                Texture2D empty = Request<Texture2D>("CalamityMod/Projectiles/InvisibleProj").Value;

                //Setup the laser sights effect.
                Effect laserScopeEffect = Filters.Scene["CalamityMod:PixelatedSightLine"].GetShader().Shader;
                laserScopeEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/CertifiedCrustyNoise").Value);
                laserScopeEffect.Parameters["noiseOffset"].SetValue(Main.GameUpdateCount * -0.003f);

                laserScopeEffect.Parameters["mainOpacity"].SetValue((float)Math.Pow(opacity , 0.5f)); //Opacity increases as the screw gets close to the cursor

                laserScopeEffect.Parameters["Resolution"].SetValue(new Vector2(700f * 0.2f));
                laserScopeEffect.Parameters["laserAngle"].SetValue((Main.MouseWorld - Owner.MountedCenter).ToRotation() * -1);

                laserScopeEffect.Parameters["laserWidth"].SetValue(0.0025f + (float)Math.Pow(opacity, 5) * ((float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f) * 0.002f + 0.002f));
                laserScopeEffect.Parameters["laserLightStrenght"].SetValue(3f);

                laserScopeEffect.Parameters["color"].SetValue(Color.GreenYellow.ToVector3());
                laserScopeEffect.Parameters["darkerColor"].SetValue(Color.Black.ToVector3());
                laserScopeEffect.Parameters["bloomSize"].SetValue(0.06f + (1 - opacity) * 0.1f);
                laserScopeEffect.Parameters["bloomMaxOpacity"].SetValue(0.4f);
                laserScopeEffect.Parameters["bloomFadeStrenght"].SetValue(3f);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, laserScopeEffect, Main.GameViewMatrix.TransformationMatrix);

                Main.EntitySpriteDraw(empty, Projectile.Center - Main.screenPosition, null, Color.White, 0, empty.Size() / 2f, 700f, 0, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }



            opacity = BazingaTimeCompletion * FadePercent;

            if (BazingaTime > 0)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(Request<Texture2D>("CalamityMod/ExtraTextures/Trails/BasicTrail"));

                CalamityUtils.DrawChromaticAberration(Vector2.UnitX, 1f, delegate (Vector2 offset, Color colorMod)
                {
                    PrimColorMult = colorMod;
                    PrimitiveSet.Prepare(Projectile.oldPos, new(WidthFunction, ColorFunction, (_) => Projectile.Size * 0.5f + offset, shader: GameShaders.Misc["CalamityMod:TrailStreak"]), 30);
                });

                //Draw the screw with chroma abberation
                CalamityUtils.DrawChromaticAberration(Vector2.UnitX, 3f, delegate (Vector2 offset, Color colorMod)
                {
                    Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + offset, null, Color.GreenYellow.MultiplyRGB(colorMod) * opacity, Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
                });

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * (1 - opacity), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

                //Very, very, very, unfortunate spritebatch restart x2, but draw a sheen texture like zenith.
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);


                if (SheenTex == null)
                    SheenTex = ModContent.Request<Texture2D>("CalamityMod/Particles/HalfStar");
                Texture2D shineTex = SheenTex.Value;
                Vector2 shineScale = new Vector2(1f, 3f - opacity * 2f);

                Main.EntitySpriteDraw(shineTex, Projectile.Center - Main.screenPosition, null, Color.GreenYellow * (1 - opacity) * 0.7f, MathHelper.PiOver2, shineTex.Size() / 2f, shineScale * Projectile.scale, SpriteEffects.None, 0);


                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            }

            else
            {
                Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * FadePercent, Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }

    }
}
