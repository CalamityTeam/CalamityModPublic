using System;
using System.Linq;
using CalamityMod.Dusts;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Particles;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles.Magic
{
    public class AerSigilMissile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public static float MaxWidth = 32;
        public ref float Time => ref Projectile.ai[0];

        private int wingAnimationTimer = 0;
        private const int wingAnimationDuration = 80;

        public static Asset<Texture2D> BloomTex;
        public static Asset<Texture2D> SlashTex;
        public static Asset<Texture2D> TrailTex;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 420;
            Projectile.MaxUpdates = 2;

            // Makes trail draw right away
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Projectile.oldPos[i] = Projectile.position;
            }
        }

        public override void AI()
        {
            // Little bit of ambient lighting as it travels
            Lighting.AddLight(Projectile.Center, Color.LightGoldenrodYellow.ToVector3() * 0.4f);

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.FinalExtraUpdate())
                Time++;

            // Required in addition to the SetDefaults loop because otherwise the funny drawing from (0,0) error happens
            if (Projectile.localAI[0] == 0f)
            {
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Projectile.oldPos[i] = Projectile.position;
                }
                Projectile.localAI[0] = 1f;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            for (int i = 0; i < 8; i++)
            {
                float variance = Main.rand.NextFloat(-0.6f, 0.6f);
                int dustStyle = ModContent.DustType<SquashDust>();
                Dust dust2 = Dust.NewDustPerfect(target.Center, dustStyle);
                dust2.scale = Main.rand.NextFloat(1.2f, 1.9f) - Math.Abs(variance);
                dust2.velocity = (Projectile.velocity * 1.5f).RotatedBy(variance) * Main.rand.NextFloat(1f, 1.6f) * (1 - Math.Abs(variance));
                dust2.noGravity = true;
                dust2.color = Color.Lerp(Color.LightGoldenrodYellow, Color.Orange, Main.rand.NextFloat(0f, 1f));
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/UnstableCastersGauntlet/AerSigilGust") { Volume = 0.6f, PitchVariance = 0.1f }, Projectile.Center);

            Vector2 adjustedCenter = Projectile.Center - Main.screenPosition + new Vector2(0, -16).RotatedBy(Projectile.rotation - MathHelper.PiOver2);

            for (int i = 0; i < 11; i++)
            {
                float randomAngle = Main.rand.NextFloat(MathHelper.TwoPi);
                float randomSpeed = Main.rand.NextFloat(6, 10); 
                Vector2 vel = new Vector2((float)Math.Cos(randomAngle) * randomSpeed, (float)Math.Sin(randomAngle) * randomSpeed);
                vel.X *= 3f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, ModContent.ProjectileType<AerSigilFeather>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }


            if (Projectile.owner == Main.myPlayer)
            {
                Projectile explosion = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<AerSigilBlast>(), Projectile.damage * 2, Projectile.knockBack * 6, Projectile.owner);
                explosion.ai[1] = 600f;
                explosion.localAI[1] = Main.rand.NextFloat(0.1f, 0.2f); // Interpolate
                explosion.netUpdate = true;
            }

            Particle orb = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Goldenrod * 0.75f, "CalamityMod/Particles/LargeBloom", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 0.1f, 0.6f * 2.5f, 15);
            GeneralParticleHandler.SpawnParticle(orb);
            Particle orb2 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.White * 0.75f, "CalamityMod/Particles/LargeBloom", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 0.1f, 0.5f * 2.5f, 15);
            GeneralParticleHandler.SpawnParticle(orb2);

        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(Color.LightGoldenrodYellow, Color.Orange, 0.5f) with { A = 0 } * Projectile.Opacity;

        public float TrailWidth(float completionRatio, Vector2 vertexPos)
        {
            float width = Utils.GetLerpValue(1f, 0.4f, completionRatio, true) * (float)Math.Sin(Math.Acos(1 - Utils.GetLerpValue(0f, 0.08f, completionRatio, true)));
            width *= Utils.GetLerpValue(0f, 0.1f, Projectile.timeLeft / 600f, true);
            return width * (MaxWidth * .265f);
        }
        public Color TrailColor(float completionRatio, Vector2 vertexPos)
        {
            Color baseColor = Color.Lerp(Color.LightGoldenrodYellow, Color.Orange, completionRatio);
            return baseColor * 0.2f;
        }

        public float MiniTrailWidth(float completionRatio, Vector2 vertexPos) => TrailWidth(completionRatio, vertexPos) * 5.5f;
        public Color MiniTrailColor(float completionRatio, Vector2 vertexPos) => Color.Lerp(Color.LightGoldenrodYellow, Color.Orange, completionRatio);
        public override bool PreDraw(ref Color lightColor)
        {
            Color mainColor = Color.Lerp(Color.LightGoldenrodYellow, Color.Orange, ((float)Main.timeForVisualEffects * 0.5f + Projectile.whoAmI * 0.12f) % 1);
            Color secondaryColor = Color.Lerp(Color.LightGoldenrodYellow, Color.Orange, ((float)Main.timeForVisualEffects * 0.5f + Projectile.whoAmI * 0.12f + 0.2f) % 1);

            Vector2 adjustedCenter = Projectile.Center - Main.screenPosition + new Vector2(0, -16).RotatedBy(Projectile.rotation - MathHelper.PiOver2);

            wingAnimationTimer++;
            Texture2D wingTexture = Request<Texture2D>("CalamityMod/ExtraTextures/SimpleWings").Value;
            Vector2 origin = wingTexture.Size() * 0.5f;

            float stretchFactorY = 0f;

            // Only run the animation logic if the timer is within the duration
            if (wingAnimationTimer <= wingAnimationDuration)
            {
                float progress = (float)wingAnimationTimer / wingAnimationDuration;

                if (progress < 0.25f) // Stretch Up
                {
                    float stretchProgress = progress / 0.25f;
                    stretchFactorY = MathHelper.Lerp(-1.4f, 0.5f, (float)Math.Sin(stretchProgress * MathHelper.PiOver2));
                }
                else if (progress < 0.5f) // Squash Down and overadjust
                {
                    float squashProgress = (progress - 0.25f) / 0.3f;
                    stretchFactorY = MathHelper.Lerp(0.5f, -0.25f, squashProgress * squashProgress);
                }
                else if (progress < 0.8f) // Stretch up again at a slower pace to a bit above the normal scale
                {
                    float stretchProgress = (progress - 0.6f) / 0.2f;
                    stretchFactorY = MathHelper.Lerp(-0.25f, 0f, stretchProgress * stretchProgress);
                }
                else // Return to normal
                {
                    stretchFactorY = 0f;
                }
            }

            // Calculate scale to display with all factors combined
            float baseScale = 1f;
            float stretchScaleY = 1f + stretchFactorY;
            float stretchScaleX = 1f - (stretchFactorY * 0.5f);
            Vector2 finalScale = new Vector2(baseScale * stretchScaleX, baseScale * stretchScaleY) * 0.6f;

            // Draw backglow of wings
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            Color glowColor = Color.LightGoldenrodYellow with { A = 0 } * 0.8f;
            Main.EntitySpriteDraw(wingTexture, adjustedCenter, null, glowColor, (Projectile.rotation + MathHelper.PiOver2), origin, finalScale * 1.5f, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);


            // Render the trail
            Main.spriteBatch.EnterShaderRegion();

            if (TrailTex == null)
                TrailTex = Request<Texture2D>("CalamityMod/ExtraTextures/Trails/BasicTrail");

            GameShaders.Misc["CalamityMod:ExobladePierce"].SetShaderTexture(TrailTex);
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseImage2("Images/Extra_189");
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseColor(mainColor);
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseSecondaryColor(secondaryColor);
            GameShaders.Misc["CalamityMod:ExobladePierce"].Apply();

            Vector2 offset = Projectile.Size * 0.5f;
            Vector2[] oldPosWithOffset = Projectile.oldPos.Select(p => p - offset).ToArray();

            PrimitiveRenderer.RenderTrail(oldPosWithOffset, new(TrailWidth, TrailColor, (_,_) => Projectile.Size * 1f, shader: GameShaders.Misc["CalamityMod:ExobladePierce"]), 30);

            GameShaders.Misc["CalamityMod:ExobladePierce"].UseColor(mainColor);
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseSecondaryColor(secondaryColor);

            PrimitiveRenderer.RenderTrail(oldPosWithOffset, new(MiniTrailWidth, MiniTrailColor, (_,_) => Projectile.Size * 1f, shader: GameShaders.Misc["CalamityMod:ExobladePierce"]), 30);

            Main.spriteBatch.ExitShaderRegion();


            // Draw the main texture
            Color drawColor = Color.LightGoldenrodYellow * 0.5f;
            Main.EntitySpriteDraw(wingTexture, adjustedCenter, null, drawColor, (Projectile.rotation + MathHelper.PiOver2), origin, finalScale, SpriteEffects.None, 0);


            // Draw bloom circle at the tip
            if (BloomTex == null)
                BloomTex = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle");
            Texture2D bloomTex = BloomTex.Value;
            Main.EntitySpriteDraw(bloomTex, adjustedCenter, null, (Color.White * 4f) with { A = 0 }, 0, bloomTex.Size() / 2f, 0.45f * Projectile.scale, 0, 0);

            return false;
        }
    }
}
