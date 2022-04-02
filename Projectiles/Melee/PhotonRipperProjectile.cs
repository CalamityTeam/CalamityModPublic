using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class PhotonRipperProjectile : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public const float ZeroChargeDamageRatio = 0.36f;
        public const float ToothDamageRatio = 0.1666667f;
        public const int ToothShootRate = 5; // One chainsaw tooth is emitted every this many frames.
        public const int ChargeUpTime = 150;
        public ref float Time => ref projectile.ai[0];

        // This is the damage dealt by the chainsaw teeth. It is recalculated every frame in two steps:
        // 1) The chainsaw's damage itself is calculated as a long-lasting holdout (like Last Prism -- in case buffs wear off or similar)
        // 2) The chainsaw teeth damage scales with charging up. The chainsaw's own damage does not.
        public ref float ToothDamage => ref projectile.ai[1];
        public float ChargeUpPower => MathHelper.Clamp((float)Math.Pow(Time / ChargeUpTime, 1.6D), 0f, 1f);

        // Seriously though Draedon this seems a bit over the top lmfao.
        public override void SetStaticDefaults() => DisplayName.SetDefault("Extraordinarily Cost-Inefficient Chainsaw");

        public override void SetDefaults()
        {
            projectile.width = 132;
            projectile.height = 56;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.melee = true;
            projectile.ownerHitCheck = true;
            // No reason to ID-static the chainsaw -- multiple players can true melee simultaneously!
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 8;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Texture2D glowmaskTexture = ModContent.GetTexture("CalamityMod/Projectiles/Melee/PhotonRipperGlowmask");
            Rectangle glowmaskRectangle = glowmaskTexture.Frame(1, 6, 0, projectile.frame);
            Vector2 origin = texture.Size() * 0.5f;
            Vector2 drawPosition = projectile.Center - Main.screenPosition;
            SpriteEffects direction = projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(texture, drawPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, origin, projectile.scale, direction, 0f);
            spriteBatch.Draw(glowmaskTexture, drawPosition, glowmaskRectangle, Color.White, projectile.rotation, origin, projectile.scale, direction, 0f);
            return false;
        }

        public override void AI()
        {
            // Recalculate damage every frame for balance reasons, as this is a long-lasting holdout.
            // This is important because you could start using it while benefitting from Auric Tesla standstill bonus, for example.
            projectile.damage = (int)((Owner.ActiveItem()?.damage ?? 0) * Owner.MeleeDamage());
            DetermineDamage();

            PlayChainsawSounds();

            // Determines the owner's position whilst incorporating their fullRotation field.
            // It uses vector transformation on a Z rotation matrix based on said rotation under the hood.
            // This is essentially just the pure mathematical definition of the RotatedBy method.
            Vector2 playerRotatedPosition = Owner.RotatedRelativePoint(Owner.MountedCenter);
            if (Main.myPlayer == projectile.owner)
            {
                if (Owner.channel && !Owner.noItems && !Owner.CCed)
                    HandleChannelMovement(playerRotatedPosition);
                else
                    projectile.Kill();
            }

            DetermineVisuals(playerRotatedPosition);
            ManipulatePlayerValues();
            EmitPrettyDust();

            if (Time % ToothShootRate == ToothShootRate - 1f)
                ReleasePrismTeeth();

            // Prevent the projectile from dying normally. However, if anything for whatever reason
            // goes wrong it will immediately be destroyed on the next frame.
            projectile.timeLeft = 2;

            Time++;
        }

        public void PlayChainsawSounds()
        {
            if (projectile.soundDelay <= 0)
            {
                Main.PlaySound(SoundID.Item22, projectile.Center);
                projectile.soundDelay = (int)MathHelper.Lerp(30f, 12f, ChargeUpPower);
            }
        }

        public void DetermineDamage()
        {
            // Set the initial tooth damage the instant the projectile is created.
            if (Main.myPlayer == projectile.owner && ToothDamage == 0f)
            {
                ToothDamage = ToothDamageRatio * projectile.damage;
                projectile.netUpdate = true;
            }

            // And then do time based damage calculations. This does not execute if the original damage is 0.
            // This line covers adjusting tooth damage if the projectile's own damage changes.
            if (ToothDamage != 0f)
            {
                float fullMult = ToothDamageRatio;
                float zeroMult = ZeroChargeDamageRatio * ToothDamageRatio;
                ToothDamage = (int)MathHelper.SmoothStep(projectile.damage * zeroMult, projectile.damage * fullMult, ChargeUpPower);
            }
        }

        public void DetermineVisuals(Vector2 playerRotatedPosition)
        {
            float directionAngle = projectile.velocity.ToRotation();
            projectile.rotation = directionAngle;

            int oldDirection = projectile.spriteDirection;
            if (oldDirection == -1)
                projectile.rotation += MathHelper.Pi;

            projectile.direction = projectile.spriteDirection = (Math.Cos(directionAngle) > 0).ToDirectionInt();

            // If the direction differs from what it originaly was, undo the previous 180 degree turn.
            // If this is not done, the chainsaw will have 1 frame of rotational "jitter" when the direction changes based on the
            // original angle. This effect looks very strange in-game.
            if (projectile.spriteDirection != oldDirection)
                projectile.rotation -= MathHelper.Pi;

            // Positioning close to the player's arm.
            projectile.position = playerRotatedPosition - projectile.Size * 0.5f + directionAngle.ToRotationVector2() * 30f;

            // Update the position a tiny bit every frame at random to make it look like the saw is vibrating.
            // It is reset on the next frame.
            projectile.position += Main.rand.NextVector2Circular(1.4f, 1.4f);

            // Update glowmask frames.
            // Smoothstep is essentially like a linear interpolation but instead of being a straight line its
            // curve is pseudo-logistic, with low increases at the ends of the curve.
            projectile.frameCounter += (int)MathHelper.SmoothStep(12f, 33f, ChargeUpPower);
            if (projectile.frameCounter >= 32)
            {
                projectile.frame = (projectile.frame + 1) % 6;
                projectile.frameCounter = 0;
            }
        }

        public void HandleChannelMovement(Vector2 playerRotatedPosition)
        {
            Vector2 idealAimDirection = (Main.MouseWorld - playerRotatedPosition).SafeNormalize(Vector2.UnitX * Owner.direction);

            float angularAimVelocity = 0.15f;
            float directionAngularDisparity = projectile.velocity.AngleBetween(idealAimDirection) / MathHelper.Pi;

            // Increase the turn speed if close to the ideal direction, since successive linear interpolations
            // are asymptotic.
            angularAimVelocity += MathHelper.Lerp(0f, 0.25f, Utils.InverseLerp(0.28f, 0.08f, directionAngularDisparity, true));

            if (directionAngularDisparity > 0.02f)
                projectile.velocity = Vector2.Lerp(projectile.velocity, idealAimDirection, angularAimVelocity);
            else
                projectile.velocity = idealAimDirection;

            projectile.velocity = projectile.velocity.SafeNormalize(Vector2.UnitX * Owner.direction);
        }

        public void ManipulatePlayerValues()
        {
            Owner.itemRotation = (projectile.velocity * projectile.direction).ToRotation();
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.ChangeDir(projectile.direction);
        }

        public void EmitPrettyDust()
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 2; i++)
            {
                Vector2 spawnPosition = projectile.Center + projectile.velocity * 35f;

                // Spawn the dust a little bit on the chainsaw. X variance is less than Y variance to ensure that dust does not
                // spawn too far from the blade.
                spawnPosition += Main.rand.NextVector2CircularEdge(9f, 35f).RotatedBy(projectile.velocity.ToRotation() + MathHelper.PiOver2);

                Dust rainbowSpark = Dust.NewDustPerfect(spawnPosition, 261);
                rainbowSpark.velocity = projectile.velocity * 3f + Main.rand.NextVector2CircularEdge(1.5f, 1.5f);
                rainbowSpark.noGravity = true;
                rainbowSpark.color = Main.hslToRgb((Time / 40f + Main.rand.NextFloat(-0.1f, 0.1f)) % 1f, 0.95f, 0.6f);
                rainbowSpark.scale = Main.rand.NextFloat(0.9f, 1.25f);
            }
        }

        public void ReleasePrismTeeth()
        {
            // Play the sound that the crystal vile shard uses.
            // Hopefully this isn't too cancerous to listen to, given the shoot rate of the crystals.
            Main.PlaySound(SoundID.Item101, projectile.Center);

            if (Main.myPlayer != projectile.owner)
                return;

            float shootReach = MathHelper.SmoothStep(projectile.width * 1.8f, projectile.width * 5.3f + 16f, ChargeUpPower);

            // Incorporate item shoot speed into the range of the crystals.
            // This means that projectile speed boosts will improve the range of the chainsaw.
            shootReach *= Owner.ActiveItem().shootSpeed;

            float distanceFromMouse = Owner.Distance(Main.MouseWorld);

            // If the distance to the mouse is less than the base reach, reach only to mouse.
            // This way the player can more directly control the crystals if they want.
            // This comes with a small constant offset to cancel out other factors.
            if (distanceFromMouse < shootReach && distanceFromMouse > 40f)
                shootReach = distanceFromMouse + 32f;

            Projectile.NewProjectile(Owner.Center, projectile.velocity, ModContent.ProjectileType<PrismTooth>(), (int)ToothDamage, 0f, projectile.owner, shootReach, projectile.whoAmI);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // Collision is done as a line to bypass the fact that hitboxes cannot rotate and that
            // this projectile is notably flat in terms of sprite shape.
            float _ = 0f;
            float width = projectile.scale * 36f;
            Vector2 start = projectile.Center;
            Vector2 end = projectile.Center + projectile.velocity * 70f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }
    }
}
