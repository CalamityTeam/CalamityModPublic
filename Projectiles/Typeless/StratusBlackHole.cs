using System.Linq;
using CalamityMod.CalPlayer;
using CalamityMod.Systems.Graphic.PixelationSystem;
using CalamityMod.Systems.Mechanic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class StratusBlackHole : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 94;
            Projectile.height = 94;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.DamageType = AverageDamageClass.Instance;
            Projectile.MaxUpdates = 1;
            Projectile.timeLeft = CalamityUtils.MinutesToFrames(10) * Projectile.MaxUpdates;
            Projectile.localNPCHitCooldown = 30 * Projectile.MaxUpdates;
            Projectile.aiStyle = 0;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.Opacity = 0;
            Projectile.ContinuouslyUpdateDamageStats = true;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.975f;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frame--;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame < 0)
            {
                Projectile.frame = Main.projFrames[Type] - 1;
            }
            if (Projectile.timeLeft < 30)
            {
                Projectile.Opacity = Projectile.timeLeft / 30f;
            }
            else if (Projectile.Opacity < 1)
            {
                Projectile.Opacity += 0.05f;
            }
            foreach (var player in Main.ActivePlayers)
            {
                if (player.Distance(Projectile.Center) <= 600 && !player.dead)
                {
                    if (player.miscCounter % 30 == 15)
                    {
                        player.Calamity().StratusStarburst++;
                        if (player.Calamity().StratusStarburst <= CalamityPlayer.MaxStratusStarburst)
                            player.Calamity().StarburstEntities.Add(new StarburstEntity(Projectile.Center));
                        player.Calamity().StratusStarburstResetTimer = (int)MathHelper.Max(player.Calamity().StratusStarburstResetTimer, 180);
                    }
                    if (player.wingsLogic > 0 && player.wingTimeMax > 0)
                    {
                        if (player.wingTime <= 0 && player.Calamity().AvaliableStarburst > 0)
                        {
                            player.Calamity().StratusStarburst--;
                            player.wingTime += 5;
                        }
                    }
                    else if (player.rocketBoots > 0)
                    {
                        if (player.rocketTime <= 0 && player.Calamity().AvaliableStarburst > 0)
                        {
                            player.Calamity().StratusStarburst--;
                            player.rocketTime += 1;
                        }
                    }
                    else
                    {
                        var activeJumps = player.extraJumps.Where(x => x.Enabled).Count();
                         if (activeJumps > 0 && !player.AnyExtraJumpUsable() && player.Calamity().AvaliableStarburst >= 5 * activeJumps && !player.gravControl2)
                        {
                            player.Calamity().StratusStarburst -= 5 * activeJumps;
                            player.RefreshDoubleJumps();
                        } 
                    }
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.timeLeft > (CalamityUtils.MinutesToFrames(10) - 45) * Projectile.MaxUpdates)
                return false;
            return targetHitbox.IntersectsConeFastInaccurate(Projectile.Center, 600, 0, MathHelper.TwoPi);
        }
        static Texture2D _TransparentBloomTex;

        public static Texture2D GetTransparentBloomTex()
        {
            if (_TransparentBloomTex == null)
            {
                _TransparentBloomTex = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
                var BaseArray = new Color[_TransparentBloomTex.Width * _TransparentBloomTex.Height];
                var ColorArray = new Color[_TransparentBloomTex.Width * _TransparentBloomTex.Height];
                _TransparentBloomTex.GetData(BaseArray);
                for (var i = 0; i < BaseArray.Length; i++)
                {
                    ColorArray[i] = new Color((int)BaseArray[i].R, (int)BaseArray[i].R, (int)BaseArray[i].R, (int)BaseArray[i].R);
                }
                _TransparentBloomTex.SetData(ColorArray);
            }
            return _TransparentBloomTex;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            PixelationManager.AddPixelatedDrawer((matrix) => DrawAura(this, matrix), Enums.GeneralDrawLayer.BeforeAllTiles);
            PixelationManager.AddPixelatedDrawer((matrix) => DrawSingularity(this, matrix), Enums.GeneralDrawLayer.AfterProjectiles);
            PixelationManager.AddPixelatedDrawer((matrix) => DrawAuraOutside(this, matrix), Enums.GeneralDrawLayer.AfterEverything);


            return false;
        }


        private static void DrawAuraOutside(StratusBlackHole mproj, Matrix matrix)
        {

            Vector2 drawPosition = mproj.Projectile.Center - Main.screenPosition;

            //Draw the outer particles
            Main.spriteBatch.EnterShaderRegion(matrix: matrix);
            GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].UseOpacity(0.1f);
            GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].UseSaturation(0.1f);
            GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Neurons"), 1);
            GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].Apply();
            Texture2D telegraphBase = ModContent.Request<Texture2D>("CalamityMod/Particles/HigResThinCircle").Value;
            Main.EntitySpriteDraw(telegraphBase, drawPosition, null, Color.SkyBlue * mproj.Projectile.Opacity * 0.75f, 0, telegraphBase.Size() / 2f, 1200f * mproj.Projectile.Opacity / telegraphBase.Width, 0, 0);
            Main.spriteBatch.ExitShaderRegion(matrix: matrix);
        }
        private static void DrawAura(StratusBlackHole mproj, Matrix matrix)
        {

            Vector2 drawPosition = mproj.Projectile.Center - Main.screenPosition;
            #region AoE
            //Draw the bloom circle

            Texture2D telegraphBase = GetTransparentBloomTex();
            Main.EntitySpriteDraw(telegraphBase, drawPosition, null, Color.DarkSlateBlue * 0.75f * mproj.Projectile.Opacity, 0, telegraphBase.Size() / 2f, 1200f * 1.25f * mproj.Projectile.Opacity / telegraphBase.Width, 0, 0);

            //Draw the inner particles
            Main.spriteBatch.EnterShaderRegion(matrix: matrix);
            GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].UseOpacity(1f);
            GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].UseSaturation(0.2f);
            GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/MeltyNoiseHighContrast"), 1);
            GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].Apply();
            telegraphBase = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
            Main.EntitySpriteDraw(telegraphBase, drawPosition, null, Color.SkyBlue * 0.5f * mproj.Projectile.Opacity, 0, telegraphBase.Size() / 2f, 1200f * mproj.Projectile.Opacity / telegraphBase.Width, 0, 0);


            //Draw the outer particles
            Main.spriteBatch.EnterShaderRegion(matrix: matrix);
            GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].UseOpacity(0.1f);
            GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].UseSaturation(0.1f);
            GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Neurons"), 1);
            GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].Apply();
            telegraphBase = ModContent.Request<Texture2D>("CalamityMod/Particles/HighResFoggyCircleHardEdge").Value;
            Main.EntitySpriteDraw(telegraphBase, drawPosition, null, Color.SkyBlue * mproj.Projectile.Opacity * 0.75f, 0, telegraphBase.Size() / 2f, 1200f * mproj.Projectile.Opacity / telegraphBase.Width, 0, 0);
            Main.spriteBatch.ExitShaderRegion(matrix: matrix);
            #endregion


        }

        private static void DrawSingularity(StratusBlackHole mproj, Matrix matrix)
        {

            Vector2 drawPosition = mproj.Projectile.Center - Main.screenPosition;

            #region Singularity
            var telegraphBase = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/BasicCircle").Value;

            Main.spriteBatch.EnterShaderRegion(matrix: matrix);
            GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].UseOpacity(0.5f);
            GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].UseSaturation(0.2f);
            GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/VoidGashes"), 1);
            GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].Apply();
            Main.EntitySpriteDraw(telegraphBase, drawPosition, null, Color.Lerp(Color.DarkSlateBlue, Color.SkyBlue, 0.75f), 0.5f, telegraphBase.Size() / 2f, 84f * mproj.Projectile.Opacity / telegraphBase.Width, 0, 0);

            Main.spriteBatch.EnterShaderRegion(matrix: matrix);
            GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].UseOpacity(0.5f);
            GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].UseSaturation(0.2f);
            GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/VoidGashes"), 1);
            GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].Apply();
            telegraphBase = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/BasicCircle").Value;
            Main.EntitySpriteDraw(telegraphBase, drawPosition, null, Color.Lerp(Color.DarkSlateBlue, Color.SkyBlue, 1f), 0, telegraphBase.Size() / 2f, 84f * mproj.Projectile.Opacity / telegraphBase.Width, 0, 0);
            Main.spriteBatch.EnterShaderRegion(matrix: matrix);
            GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].UseOpacity(0.25f);
            GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].UseSaturation(0.1f);
            GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/VoidGashes"), 1);
            GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].Apply();
            Main.EntitySpriteDraw(telegraphBase, drawPosition, null, Color.Black, 1, telegraphBase.Size() / 2f, 72f * mproj.Projectile.Opacity / telegraphBase.Width, 0, 0);
            Main.spriteBatch.ExitShaderRegion(matrix: matrix);

            telegraphBase = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/BasicCircle").Value;
            Main.EntitySpriteDraw(telegraphBase, drawPosition, null, Color.Black, 0, telegraphBase.Size() / 2f, 64f * mproj.Projectile.Opacity / telegraphBase.Width, 0, 0);
            #endregion
        }
    }
}
