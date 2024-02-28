using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using CalamityMod.Particles;
using static CalamityMod.CalamityUtils;
using static Terraria.ModLoader.ModContent;
using Terraria.Graphics.Shaders;
using Terraria.Graphics.Effects;
using ReLogic.Content;
using CalamityMod.Graphics.Primitives;

namespace CalamityMod.Projectiles.Summon
{
    public class WulfrumEnergyBurst : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public ref float OriginalRotation => ref Projectile.ai[0];
        public NPC Target
        {
            get
            {
                if (Projectile.ai[1] < 0 || Projectile.ai[1] > Main.maxNPCs)
                    return null;

                return Main.npc[(int)Projectile.ai[1]];
            }
            set
            {
                if (value == null)
                    Projectile.ai[1] = -1;
                else
                    Projectile.ai[1] = value.whoAmI;
            }
        }

        public static float MaxDeviationAngle = MathHelper.PiOver4;
        public static float HomingRange = 350;
        public static float HomingAngle = MathHelper.PiOver4 * 1f;

        internal Color PrimColorMult = Color.White;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;

            Projectile.timeLeft = 140;
            Projectile.MaxUpdates = 3;
        }

        public NPC FindTarget()
        {
            float bestScore = 0;
            NPC bestTarget = null;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC potentialTarget = Main.npc[i];

                if (!potentialTarget.CanBeChasedBy(null, false))
                    continue;

                float distance = potentialTarget.Distance(Projectile.Center);
                float angle = Projectile.velocity.AngleBetween((potentialTarget.Center - Projectile.Center));

                float extraDistance = potentialTarget.width / 2 + potentialTarget.height / 2;

                if (distance - extraDistance < HomingRange && angle < HomingAngle / 2f)
                {
                    if (!Collision.CanHit(Projectile.Center, 1, 1, potentialTarget.Center, 1, 1) && extraDistance < distance)
                        continue;

                    float attemptedScore = EvaluatePotentialTarget(distance - extraDistance, angle / 2f);
                    if (attemptedScore > bestScore)
                    {
                        bestTarget = potentialTarget;
                        bestScore = attemptedScore;
                    }
                }
            }
            return bestTarget;

        }

        public float EvaluatePotentialTarget(float distance, float angle)
        {
            float score = 1 - distance / HomingRange * 0.5f;

            score += (1 - Math.Abs(angle) / (HomingAngle / 2f)) * 0.5f;

            return score;
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 140)
            {
                if (OriginalRotation == 0)
                {
                    OriginalRotation = Projectile.velocity.ToRotation();
                    Projectile.rotation = OriginalRotation;
                }
                Target = null;
            }
            else
            {
                Target = FindTarget();
            }

            Lighting.AddLight(Projectile.Center, (Color.GreenYellow * 0.8f).ToVector3() * 0.5f);

            if (Target != null)
            {
                float distanceFromTarget = (Target.Center - Projectile.Center).Length();

                Projectile.rotation = Projectile.rotation.AngleTowards((Target.Center - Projectile.Center).ToRotation(), 0.07f * (float)Math.Pow((1 - distanceFromTarget / HomingRange), 2));
            }

            Projectile.velocity *= 1.01f;
            Projectile.velocity = Projectile.rotation.ToRotationVector2() * Projectile.velocity.Length();

            //Blast off.
            if (Projectile.timeLeft == 140)
            {
                Vector2 dustCenter = Projectile.Center + Projectile.velocity * 1f;

                for (int i = 0; i < 5; i++)
                {
                    Dust chust = Dust.NewDustPerfect(dustCenter, 178, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(0.2f, 0.5f), Scale: Main.rand.NextFloat(1f, 1.2f));
                    chust.noGravity = true;
                }
            }

            if (Projectile.timeLeft <= 137)
            {
                if (Main.rand.NextBool(4))
                {
                    Vector2 dustCenter = Projectile.Center + Projectile.velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(-3f, 3f);

                    Dust chust = Dust.NewDustPerfect(dustCenter, 178, -Projectile.velocity * Main.rand.NextFloat(0.2f, 0.5f), Scale: Main.rand.NextFloat(0.6f, 1.15f));
                    chust.noGravity = true;
                }

                if (Main.rand.NextBool(4))
                {
                    Vector2 dustCenter = Projectile.Center + Projectile.velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(-3f, 3f);

                    Dust largeDust = Dust.NewDustPerfect(dustCenter, 178, -Projectile.velocity * Main.rand.NextFloat(0.2f, 0.4f), Scale: Main.rand.NextFloat(0.4f, 1f));
                    largeDust.noGravity = true;
                    largeDust.noLight = true;
                }
            }
        }

        internal Color ColorFunction(float completionRatio)
        {
            float fadeOpacity = (float)Math.Sqrt(1 - completionRatio);
            return Color.Chartreuse.MultiplyRGB(PrimColorMult) * fadeOpacity;
        }

        internal float WidthFunction(float completionRatio)
        {
            return 9.4f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ZapTrail"));

            CalamityUtils.DrawChromaticAberration(Vector2.UnitX, 1.5f, delegate (Vector2 offset, Color colorMod)
            {
                PrimColorMult = colorMod;
                PrimitiveSet.Prepare(Projectile.oldPos, new(WidthFunction, ColorFunction, (_) => Projectile.Size * 0.5f + offset, shader: GameShaders.Misc["CalamityMod:TrailStreak"]), 30);
            });

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            float stretchy = MathHelper.Clamp((Projectile.velocity.Length() - 6f) / 16f, 0f, 1f);
            Vector2 scale = new Vector2(1f + stretchy * -0.2f, stretchy * 0.5f + 1f);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation + MathHelper.PiOver2, tex.Size() / 2f, Projectile.scale * scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
