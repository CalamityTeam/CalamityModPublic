using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class MarniteObliteratorProj : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<MarniteObliterator>();
        public override string Texture => "CalamityMod/Items/Tools/MarniteObliterator";
        public static Asset<Texture2D> GlowmaskTex;
        public static Asset<Texture2D> BloomTex;

        public Player Owner => Main.player[Projectile.owner];
        public ref float MoveInIntervals => ref Projectile.localAI[0];
        public ref float SpeenBeams => ref Projectile.localAI[1];
        public ref float Timer => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = TrueMeleeNoSpeedDamageClass.Instance;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            Timer++;
            SpeenBeams += Timer > 140 ? 1 : 1 + 2f * (float)Math.Pow(1 - Timer / 140f, 2);


            if (Projectile.soundDelay <= 0)
            {
                SoundEngine.PlaySound(MarniteObliterator.UseSound, Projectile.Center);
                Projectile.soundDelay = 23;
            }

            if ((Owner.Center - Projectile.Center).Length() >= 5f)
            {
                if ((Owner.MountedCenter - Projectile.Center).Length() >= 30f)
                {
                    DelegateMethods.v3_1 = Color.Blue.ToVector3() * 0.5f;
                    Utils.PlotTileLine(Owner.MountedCenter + Owner.MountedCenter.DirectionTo(Projectile.Center) * 30f, Projectile.Center, 8f, DelegateMethods.CastLightOpen);
                }

                Lighting.AddLight(Projectile.Center, Color.Blue.ToVector3() * 0.7f);
            }

                if (MoveInIntervals > 0f)
                    MoveInIntervals -= 1f;

            if (!Owner.channel || Owner.noItems || Owner.CCed)
                Projectile.Kill();

            else if (MoveInIntervals <= 0f)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 newVelocity = Owner.Calamity().mouseWorld - Owner.MountedCenter;

                    if (Main.tile[Player.tileTargetX, Player.tileTargetY].HasTile)
                    {
                        newVelocity = new Vector2(Player.tileTargetX, Player.tileTargetY) * 16f + Vector2.One * 8f - Owner.MountedCenter;
                        MoveInIntervals = 2f;
                    }

                    newVelocity = Vector2.Lerp(newVelocity, Projectile.velocity, 0.7f);
                    if (float.IsNaN(newVelocity.X) || float.IsNaN(newVelocity.Y))
                        newVelocity = -Vector2.UnitY;

                    if (newVelocity.Length() < 50f)
                        newVelocity = newVelocity.SafeNormalize(-Vector2.UnitY) * 50f;

                    //Tile reach is a fucking square in terraria. Can you believe that?
                    int tileBoost = Owner.inventory[Owner.selectedItem].tileBoost;
                    int fullRangeX = (Player.tileRangeX + tileBoost - 1) * 16 + 11; //Why are those 2 separate variables? Whatever
                    int fullRangeY = (Player.tileRangeY + tileBoost - 1) * 16 + 11;

                    newVelocity.X = Math.Clamp(newVelocity.X, -fullRangeX, fullRangeX);
                    newVelocity.Y = Math.Clamp(newVelocity.Y, -fullRangeY, fullRangeY);

                    if (newVelocity != Projectile.velocity)
                        Projectile.netUpdate = true;

                    Projectile.velocity = newVelocity;
                }
            }

            //Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            Owner.heldProj = Projectile.whoAmI;
            Owner.ChangeDir(Math.Sign(Projectile.velocity.X));
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() * Owner.gravDir - MathHelper.PiOver2);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() * Owner.gravDir - MathHelper.PiOver2 - MathHelper.PiOver4 * 0.5f * Owner.direction);

            Owner.SetDummyItemTime(2);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Center = Owner.MountedCenter + Projectile.velocity;
        }

        internal Color ColorFunction(float completionRatio)
        {
            float fadeOpacity = (float)Math.Sqrt(1 - completionRatio);
            return Color.DeepSkyBlue * fadeOpacity;
        }

        internal float WidthFunction(float completionRatio)
        {
            return 29.4f * completionRatio;
        }

        public void DrawBeam(Texture2D beamTex, Vector2 direction, int beamIndex)
        {
            Vector2 startPos = Owner.MountedCenter + direction * 17f + direction.RotatedBy(MathHelper.PiOver2) * (float)Math.Cos(MathHelper.TwoPi * beamIndex / 3f + SpeenBeams * 0.06f) * 13f;
            float rotation = (Projectile.Center - startPos).ToRotation();
            Vector2 beamOrigin = new Vector2(beamTex.Width / 2f, beamTex.Height);
            Vector2 beamScale = new Vector2(5.4f, (startPos - Projectile.Center).Length() / (float)beamTex.Height);

            CalamityUtils.DrawChromaticAberration(direction.RotatedBy(MathHelper.PiOver2), 4f, delegate (Vector2 offset, Color colorMod)
            {
                Color beamColor = Color.Lerp(Color.Blue, Color.Goldenrod, 0.5f + 0.5f * (float)Math.Sin(SpeenBeams * 0.2f));
                beamColor *= 0.54f;
                beamColor = beamColor.MultiplyRGB(colorMod);

                Main.EntitySpriteDraw(beamTex, startPos + offset - Main.screenPosition, null, beamColor, rotation + MathHelper.PiOver2, beamOrigin, beamScale, SpriteEffects.None, 0);

                beamScale.X = 2.4f;
                beamColor = Color.Lerp(Color.DeepSkyBlue, Color.Chocolate, 0.5f + 0.5f * (float)Math.Sin(SpeenBeams * 0.2f + 1.2f));
                beamColor = beamColor.MultiplyRGB(colorMod);

                Main.EntitySpriteDraw(beamTex, startPos + offset - Main.screenPosition, null, beamColor, rotation + MathHelper.PiOver2, beamOrigin, beamScale, SpriteEffects.None, 0);
            });
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.active)
                return false;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);


            Vector2 normalizedVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Texture2D beamTex = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/SimpleGradient").Value;

            for (int i = 0; i < 3; i++)
            {
                float beamElevation = (float)Math.Sin(MathHelper.TwoPi * i / 3f + SpeenBeams * 0.06f);
                if (beamElevation < 0)
                    DrawBeam(beamTex, normalizedVelocity, i);
            }

            if (BloomTex == null)
                BloomTex = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle");
            Texture2D bloomTex = BloomTex.Value;

            Main.EntitySpriteDraw(bloomTex, Projectile.Center - Main.screenPosition, null, Color.DeepSkyBlue * 0.3f, MathHelper.PiOver2, bloomTex.Size() / 2f, 0.3f * Projectile.scale, SpriteEffects.None, 0);

            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/DoubleTrail"));
            PrimitiveSet.Prepare(new Vector2[] { Projectile.Center, Owner.MountedCenter - normalizedVelocity * 13f }, new(WidthFunction, ColorFunction, shader: GameShaders.Misc["CalamityMod:TrailStreak"]), 30);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new Vector2(9f, tex.Height / 2f);
            SpriteEffects effect = SpriteEffects.None;
            if (Owner.direction * Owner.gravDir < 0)
                effect = SpriteEffects.FlipVertically;

            Main.EntitySpriteDraw(tex, Owner.MountedCenter + normalizedVelocity * 10f - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, effect, 0);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            //Draw some bloom
            if (GlowmaskTex == null)
                GlowmaskTex = ModContent.Request<Texture2D>("CalamityMod/Items/Tools/MarniteObliteratorBloom");
            Texture2D glowTex = GlowmaskTex.Value;
            float bloomOpacity = (float)Math.Pow(Math.Clamp(Timer / 100f, 0f, 1f), 2) * (0.85f + (0.5f + 0.5f * (float)Math.Sin(Main.GlobalTimeWrappedHourly))) * 0.8f;
            Color bloomColor = Color.Lerp(Color.DeepSkyBlue, Color.Chocolate, 0.5f + 0.5f * (float)Math.Sin(SpeenBeams * 0.2f + 1.2f));

            Main.EntitySpriteDraw(glowTex, Owner.MountedCenter + normalizedVelocity * 10f - Main.screenPosition, null, bloomColor * bloomOpacity, Projectile.rotation, origin, Projectile.scale, effect, 0);



            for (int i = 0; i < 3; i++)
            {
                float beamElevation = (float)Math.Sin(MathHelper.TwoPi * i / 3f + SpeenBeams * 0.06f);
                if (beamElevation >= 0)
                    DrawBeam(beamTex, normalizedVelocity, i);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);


            return false;
        }
    }
}
