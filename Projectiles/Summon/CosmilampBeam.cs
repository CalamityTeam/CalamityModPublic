using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class CosmilampBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public ref float Timer => ref Projectile.ai[0];

        public const int SlowdownTime = 45;

        public const int Lifetime = 120;

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 32;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 3;
            Projectile.MaxUpdates = 3;
            Projectile.timeLeft = Projectile.MaxUpdates * Lifetime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.MaxUpdates * 15;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.2f, 0.01f, 0.1f);
            Projectile.Opacity = Utils.GetLerpValue(0f, Projectile.MaxUpdates * 10f, Projectile.timeLeft, true);

            // Emit light.
            Lighting.AddLight(Projectile.Center, Vector3.One * Projectile.Opacity * 0.7f);

            NPC potentialTarget = Projectile.Center.MinionHoming(Cosmilamp.MaxTargetingDistance, Main.player[Projectile.owner]);

            // Slow down before doing anything else.
            if (Timer < SlowdownTime)
                Projectile.velocity *= 0.995f;

            // Afterwards, race towards nearby targets at incredible speeds.
            else if (potentialTarget is not null)
            {
                Vector2 idealVelocity = Projectile.SafeDirectionTo(potentialTarget.Center) * Cosmilamp.BeamHomeSpeed;
                if (!Projectile.WithinRange(potentialTarget.Center, 160f))
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, idealVelocity, 0.036f);
                    Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(idealVelocity.ToRotation(), 0.12f).ToRotationVector2() * Projectile.velocity.Length();
                }

                // Spin around to give a slice motion if close to the target. To prevent overshooting, the amount of angular velocity varies based on how large the target is.
                else
                {
                    float angularTurnSpeed = MathHelper.Pi * potentialTarget.Size.Length() / 12000f;
                    if (angularTurnSpeed > MathHelper.Pi * 0.05f)
                        angularTurnSpeed = MathHelper.Pi * 0.05f;

                    Projectile.velocity = Projectile.velocity.RotatedBy(angularTurnSpeed);
                }
            }

            if (Projectile.FinalExtraUpdate())
                Timer++;
        }

        internal Color ColorFunction(float completionRatio)
        {
            float streakOpacity = Utils.GetLerpValue(0.8f, 0.54f, completionRatio, true) * Projectile.Opacity;
            Color endColor = Color.Lerp(Color.Fuchsia, Color.DarkViolet, (float)Math.Sin(completionRatio * MathHelper.Pi * 1.6f - Main.GlobalTimeWrappedHourly * 4f) * 0.5f + 0.5f);
            endColor = CalamityUtils.MulticolorLerp(completionRatio * completionRatio, endColor, Color.MediumPurple, Color.Red);
            if (Projectile.localAI[0] == 1f)
                endColor = Color.Lerp(endColor, Color.White, 0.5f) * streakOpacity;

            return Color.Lerp(endColor, Color.Black, 0.3f) * streakOpacity;
        }

        internal float WidthFunction(float completionRatio)
        {
            float expansionCompletion = 1f - (float)Math.Pow(1f - Utils.GetLerpValue(0f, 0.3f, completionRatio, true), 2D);
            float maxWidth = Projectile.Opacity * Projectile.width * 1.65f;
            if (Projectile.localAI[0] == 1f)
                maxWidth *= 0.4f;

            return MathHelper.Lerp(0f, maxWidth, expansionCompletion);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.localAI[0] = 0f;
            GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));
            var primSet = PrimitiveSet.Prepare(Projectile.oldPos, new(WidthFunction, ColorFunction, (_) => Projectile.Size * 0.5f, shader: GameShaders.Misc["CalamityMod:ImpFlameTrail"]), 42);

            Projectile.localAI[0] = 1f;
            if (primSet.HasValue)
                primSet.Value.Render();
            return false;
        }
    }
}
