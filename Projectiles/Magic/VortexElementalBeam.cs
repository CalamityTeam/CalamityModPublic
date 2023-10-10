using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityMod.Projectiles.Magic
{
    public class VortexElementalBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public const int Lifetime = 30;
        public const float LightningTurnRandomnessFactor = 1.7f;
        public ref float InitialVelocityAngle => ref Projectile.ai[0];
        // Technically not a ratio, and more of a seed, but it is used in a 0-2pi squash
        // later in the code to get an arbitrary unit vector (which is then checked).
        public ref float BaseTurnAngleRatio => ref Projectile.ai[1];
        public ref float AccumulatedXMovementSpeeds => ref Projectile.localAI[0];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 20;
            Projectile.timeLeft = 45 * Projectile.extraUpdates;

            // Readjust the velocity magnitude the moment this projectile is created
            // to make velocity setting outside the scope of this projectile less irritating
            // to consider alongside extraUpdate multipliers.
            // Also set the initial angle.
            if (Projectile.velocity != Vector2.Zero)
            {
                Projectile.velocity /= Projectile.extraUpdates;
            }
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(AccumulatedXMovementSpeeds);

        public override void ReceiveExtraAI(BinaryReader reader) => AccumulatedXMovementSpeeds = reader.ReadSingle();

        public override void AI()
        {
            // frameCounter in this context is really just an arbitrary timer
            // which allows random turning to occur.
            Projectile.frameCounter++;

            Projectile.scale += 0.04f / Projectile.MaxUpdates;
            if (Projectile.scale > 1f)
                Projectile.scale = 1f;

            Lighting.AddLight(Projectile.Center, Color.Pink.ToVector3());
            if (Projectile.frameCounter >= Projectile.extraUpdates * 2)
            {
                Projectile.frameCounter = 0;

                float originalSpeed = MathHelper.Min(6f, Projectile.velocity.Length());
                UnifiedRandom unifiedRandom = new UnifiedRandom((int)BaseTurnAngleRatio);
                int turnTries = 0;
                Vector2 newBaseDirection = -Vector2.UnitY;
                Vector2 potentialBaseDirection;

                do
                {
                    BaseTurnAngleRatio = unifiedRandom.Next() % 100;
                    potentialBaseDirection = (BaseTurnAngleRatio / 100f * MathHelper.TwoPi).ToRotationVector2();

                    // Ensure that the new potential direction base is always moving upwards (this is supposed to be somewhat similar to a -UnitY + RotatedBy).
                    potentialBaseDirection.Y = -Math.Abs(potentialBaseDirection.Y);

                    bool canChangeLightningDirection = true;

                    // Potential directions with very little Y speed should not be considered, because this
                    // consequentially means that the X speed would be quite large.
                    if (potentialBaseDirection.Y > -0.02f)
                        canChangeLightningDirection = false;

                    // This mess of math basically encourages movement at the ends of an extraUpdate cycle,
                    // discourages super frequenent randomness as the accumulated X speed changes get larger,
                    // or if the original speed is quite large.
                    if (Math.Abs(potentialBaseDirection.X * (Projectile.extraUpdates + 1) * 2f * originalSpeed + AccumulatedXMovementSpeeds) > Projectile.MaxUpdates * LightningTurnRandomnessFactor)
                    {
                        canChangeLightningDirection = false;
                    }

                    // If the above checks were all passed, redefine the base direction of the lightning.
                    if (canChangeLightningDirection)
                        newBaseDirection = potentialBaseDirection;

                    turnTries++;
                }
                while (turnTries < 100);

                if (Projectile.velocity != Vector2.Zero)
                {
                    AccumulatedXMovementSpeeds += newBaseDirection.X * (Projectile.extraUpdates + 1) * 2f * originalSpeed;
                    Projectile.velocity = newBaseDirection.RotatedBy(InitialVelocityAngle + MathHelper.PiOver2) * originalSpeed;
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                }
            }
        }

        #region Drawing
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D lightningSegmentTexture = ModContent.Request<Texture2D>(Texture).Value;
            Projectile.GetAlpha(lightColor);
            Vector2 lightningScale = new Vector2(Projectile.scale) / 2f;
            for (int i = 0; i < 3; i++)
            {
                // c_1 and f_1 are general fields that are used for drawing methods in here.
                // c_1 is the color to be used, and f_1 is the opacity.
                if (i == 0)
                {
                    lightningScale = new Vector2(Projectile.scale) * 0.6f;
                    DelegateMethods.c_1 = Color.DarkCyan * 0.85f;
                }
                else if (i == 1)
                {
                    lightningScale = new Vector2(Projectile.scale) * 0.4f;
                    DelegateMethods.c_1 = Color.Cyan * 0.85f;
                }
                else
                {
                    lightningScale = new Vector2(Projectile.scale) * 0.2f;
                    DelegateMethods.c_1 = Color.White;
                }

                DelegateMethods.f_1 = 1f;
                for (int j = Projectile.oldPos.Length - 1; j > 0; j--)
                {
                    if (Projectile.oldPos[j] != Vector2.Zero)
                    {
                        Vector2 start = Projectile.oldPos[j] + Projectile.Size * 0.5f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
                        Vector2 end = Projectile.oldPos[j - 1] + Projectile.Size * 0.5f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
                        Utils.DrawLaser(Main.spriteBatch, lightningSegmentTexture, start, end, lightningScale, DelegateMethods.LightningLaserDraw);
                    }
                }
            }
            return false;
        }
        #endregion

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ElementalMix>(), 30);
            for (int i = 0; i < Projectile.oldPos.Length - 1; i++)
            {
                // Skip zeroed old positions. They are almost certainly a
                // result of incomplete indices.
                if (Projectile.oldPos[i + 1] == Vector2.Zero)
                    continue;

                for (int j = 0; j < 8; j++)
                {
                    Vector2 dustSpawnPosition = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[i + 1], j / 8f);
                    dustSpawnPosition += Projectile.Size * 0.5f;

                    Dust electricity = Dust.NewDustPerfect(dustSpawnPosition, Main.rand.NextBool() ? 226 : 229);
                    electricity.velocity = Vector2.Zero;
                    electricity.scale = Main.rand.NextFloat(1.1f, 1.18f);
                    electricity.noGravity = true;
                }
            }
        }
    }
}
