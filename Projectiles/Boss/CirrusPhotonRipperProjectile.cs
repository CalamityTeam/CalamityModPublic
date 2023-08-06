using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.NPCs.SupremeCalamitas;

namespace CalamityMod.Projectiles.Boss
{
    public class CirrusPhotonRipperProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";

        public override string Texture => "CalamityMod/Projectiles/Melee/PhotonRipperProjectile";

        public NPC Cirrus => Main.npc.IndexInRange((int)Projectile.ai[2]) ? Main.npc[(int)Projectile.ai[2]] : null;
        public const float ZeroChargeDamageRatio = 0.36f;
        public const float ToothDamageRatio = 0.1666667f;
        public const int ToothShootRate = 5; // One chainsaw tooth is emitted every this many frames.
        public const int ChargeUpTime = 150;

        public ref float Time => ref Projectile.ai[0];

        // This is the damage dealt by the chainsaw teeth. It is recalculated every frame in two steps:
        // 1) The chainsaw's damage itself is calculated as a long-lasting holdout (like Last Prism -- in case buffs wear off or similar)
        // 2) The chainsaw teeth damage scales with charging up. The chainsaw's own damage does not.
        public ref float ToothDamage => ref Projectile.ai[1];
        public float ChargeUpPower => MathHelper.Clamp((float)Math.Pow(Time / ChargeUpTime, 1.6D), 0f, 1f);

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 132;
            Projectile.height = 56;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowmaskTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/PhotonRipperGlowmask").Value;
            Rectangle glowmaskRectangle = glowmaskTexture.Frame(1, 6, 0, Projectile.frame);
            Vector2 origin = texture.Size() * 0.5f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            SpriteEffects direction = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, direction, 0);
            Main.EntitySpriteDraw(glowmaskTexture, drawPosition, glowmaskRectangle, Color.HotPink, Projectile.rotation, origin, Projectile.scale, direction, 0);
            return false;
        }

        public override void AI()
        {
            // Die if Cirrus disappears.
            if (Cirrus is null || !Cirrus.active)
            {
                Projectile.Kill();
                return;
            }

            // Die if Cirrus shouldn't be using it.
            int cirrusBulletHellCounter = Cirrus.ModNPC<SupremeCalamitas>().bulletHellCounter2;
            if (!(cirrusBulletHellCounter > SupremeCalamitas.SecondBulletHellEndValue && cirrusBulletHellCounter < SupremeCalamitas.ThirdBulletHellEndValue) &&
                !(cirrusBulletHellCounter > SupremeCalamitas.FourthBulletHellEndValue && cirrusBulletHellCounter < SupremeCalamitas.FifthBulletHellEndValue))
            {
                Projectile.Kill();
                return;
            }

            Projectile.damage = SupremeCalamitas.CirrusPhotonRipperDamage;
            DetermineDamage();

            PlayChainsawSounds();

            // Determines the owner's position whilst incorporating their fullRotation field.
            // It uses vector transformation on a Z rotation matrix based on said rotation under the hood.
            // This is essentially just the pure mathematical definition of the RotatedBy method.
            Vector2 cirrusRotatedPosition = Cirrus.Center;
            float rotation = Cirrus.rotation;
            Vector2 vector = Cirrus.Bottom + new Vector2(0f, Cirrus.gfxOffY);
            Vector2 vector2 = new Vector2(0f, -4f) + new Vector2(0f, 4f).RotatedBy(rotation);
            cirrusRotatedPosition.Y += Cirrus.gfxOffY;
            cirrusRotatedPosition = vector + (cirrusRotatedPosition - vector).RotatedBy(rotation) + vector2;
            HandleMovement(cirrusRotatedPosition);
            DetermineVisuals(cirrusRotatedPosition);
            EmitPrettyDust();

            if (Time % ToothShootRate == ToothShootRate - 1f)
                ReleasePrismTeeth();

            // Prevent the projectile from dying normally.
            // However, if anything for whatever reason goes wrong it will immediately be destroyed on the next frame.
            Projectile.timeLeft = 2;

            Time++;
        }

        public void PlayChainsawSounds()
        {
            if (Projectile.soundDelay <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item22, Projectile.Center);
                Projectile.soundDelay = (int)MathHelper.Lerp(30f, 12f, ChargeUpPower);
            }
        }

        public void DetermineDamage()
        {
            // Set the initial tooth damage the instant the projectile is created.
            if (ToothDamage == 0f)
            {
                ToothDamage = ToothDamageRatio * Projectile.damage;
                Projectile.netUpdate = true;
            }

            // And then do time based damage calculations. This does not execute if the original damage is 0.
            // This line covers adjusting tooth damage if the projectile's own damage changes.
            if (ToothDamage != 0f)
            {
                float fullMult = ToothDamageRatio;
                float zeroMult = ZeroChargeDamageRatio * ToothDamageRatio;
                ToothDamage = (int)MathHelper.SmoothStep(Projectile.damage * zeroMult, Projectile.damage * fullMult, ChargeUpPower);
            }
        }

        public void DetermineVisuals(Vector2 cirrusRotatedPosition)
        {
            float directionAngle = Projectile.velocity.ToRotation();
            Projectile.rotation = directionAngle;

            int oldDirection = Projectile.spriteDirection;
            if (oldDirection == -1)
                Projectile.rotation += MathHelper.Pi;

            Projectile.direction = Projectile.spriteDirection = (Math.Cos(directionAngle) > 0).ToDirectionInt();

            // If the direction differs from what it originaly was, undo the previous 180 degree turn.
            // If this is not done, the chainsaw will have 1 frame of rotational "jitter" when the direction changes based on the original angle.
            // This effect looks very strange in-game.
            if (Projectile.spriteDirection != oldDirection)
                Projectile.rotation -= MathHelper.Pi;

            // Positioning close to the Cirrus' arm.
            Projectile.position = cirrusRotatedPosition - Projectile.Size * 0.5f + directionAngle.ToRotationVector2() * 30f;

            // Update the position a tiny bit every frame at random to make it look like the saw is vibrating.
            // It is reset on the next frame.
            Projectile.position += Main.rand.NextVector2Circular(1.4f, 1.4f);

            // Update glowmask frames.
            // Smoothstep is essentially like a linear interpolation but instead of being a straight line its
            // curve is pseudo-logistic, with low increases at the ends of the curve.
            Projectile.frameCounter += (int)MathHelper.SmoothStep(12f, 33f, ChargeUpPower);
            if (Projectile.frameCounter >= 32)
            {
                Projectile.frame = (Projectile.frame + 1) % 6;
                Projectile.frameCounter = 0;
            }
        }

        public void HandleMovement(Vector2 cirrusRotatedPosition)
        {
            Vector2 idealAimDirection = (Main.player[Cirrus.target].Center - cirrusRotatedPosition).SafeNormalize(Vector2.UnitX * Cirrus.direction);

            float angularAimVelocity = 0.03f;
            float directionAngularDisparity = Projectile.velocity.AngleBetween(idealAimDirection) / MathHelper.Pi;

            // Increase the turn speed if close to the ideal direction, since successive linear interpolations are asymptotic.
            angularAimVelocity += MathHelper.Lerp(0f, 0.05f, Utils.GetLerpValue(0.28f, 0.08f, directionAngularDisparity, true));

            if (directionAngularDisparity > 0.02f)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, idealAimDirection, angularAimVelocity);
            else
                Projectile.velocity = idealAimDirection;

            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX * Cirrus.direction);
        }

        public void EmitPrettyDust()
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 2; i++)
            {
                Vector2 spawnPosition = Projectile.Center + Projectile.velocity * 35f;

                // Spawn the dust a little bit on the chainsaw. X variance is less than Y variance to ensure that dust does not
                // spawn too far from the blade.
                spawnPosition += Main.rand.NextVector2CircularEdge(9f, 35f).RotatedBy(Projectile.velocity.ToRotation() + MathHelper.PiOver2);

                Dust rainbowSpark = Dust.NewDustPerfect(spawnPosition, 261);
                rainbowSpark.velocity = Projectile.velocity * 3f + Main.rand.NextVector2CircularEdge(1.5f, 1.5f);
                rainbowSpark.noGravity = true;
                rainbowSpark.color = Color.HotPink;
                rainbowSpark.scale = Main.rand.NextFloat(0.9f, 1.25f);
            }
        }

        public void ReleasePrismTeeth()
        {
            // Play the sound that the crystal vile shard uses.
            // Hopefully this isn't too cancerous to listen to, given the shoot rate of the crystals.
            SoundEngine.PlaySound(SoundID.Item101, Projectile.Center);

            if (Main.myPlayer != Projectile.owner)
                return;

            float shootReach = MathHelper.SmoothStep(Projectile.width * 1.8f, Projectile.width * 5.3f + 16f, ChargeUpPower);
            float distanceFromTarget = Cirrus.Distance(Main.player[Cirrus.target].Center);

            // If the distance to the mouse is less than the base reach, reach only to mouse.
            // This way the player can more directly control the crystals if they want.
            // This comes with a small constant offset to cancel out other factors.
            // If the mouse is very close to the player, set the shoot reach to its minimum.
            if (distanceFromTarget < shootReach)
            {
                if (distanceFromTarget > 40f)
                    shootReach = distanceFromTarget + 32f;
                else
                    shootReach = 72f;
			}

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Cirrus.Center, Projectile.velocity, ModContent.ProjectileType<CirrusPhotonRipperPrismTooth>(), (int)ToothDamage, 0f, Projectile.owner, shootReach, Projectile.whoAmI, Projectile.ai[2]);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // Collision is done as a line to bypass the fact that hitboxes cannot rotate and that this projectile is notably flat in terms of sprite shape.
            float _ = 0f;
            float width = Projectile.scale * 36f;
            Vector2 start = Projectile.Center;
            Vector2 end = Projectile.Center + Projectile.velocity * 70f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }
    }
}
