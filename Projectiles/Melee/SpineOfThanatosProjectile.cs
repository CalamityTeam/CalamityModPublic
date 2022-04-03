using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class SpineOfThanatosProjectile : ModProjectile
    {
        public List<Vector2> WhipPoints = new List<Vector2>();
        public Player Owner => Main.player[Projectile.owner];
        public float CurrentBendFactor => MaximumBendFactor * CalamityUtils.Convert01To010(Time / Lifetime);
        public Vector2 WhipEnd => Projectile.Center + WhipOutwardness;

        // This wrapper exists solely for clarity as to the definition of velocity in the context of this projectile.
        // You may notice that the Center property of this projectile is constantly set to be near the player. However, since
        // ShouldUpdatePosition is not overriden to false, it still gains its velocity as usual for 1 frame before being reset again.
        public ref Vector2 WhipOutwardness => ref Projectile.velocity;
        public ref float Time => ref Projectile.ai[0];
        public ref float SwingDirection => ref Projectile.ai[1];
        public ref float InitialDirectionRotation => ref Projectile.localAI[0];
        public const int Lifetime = 125;
        public const int FlyBackTime = 40;
        public const int FinalWhipRayShootRate = 10;
        public const int LaserRayCount = 12;
        public const float MaximumBendFactor = 42f;
        public override void SetStaticDefaults() => DisplayName.SetDefault("Spine of Thanatos");

        public override void SetDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 70;
            Projectile.scale = 0.75f;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = Lifetime;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 22;
        }

        public void DetermineWhipPoints()
        {
            Vector2 startingPosition = Owner.RotatedRelativePoint(Owner.MountedCenter);

            List<Vector2> initialPoints = new List<Vector2>()
            {
                startingPosition
            };
            for (int i = 0; i < 12; i++)
            {
                Vector2 bendOffset = Vector2.UnitX * -SwingDirection;

                // Make the bend factor depend on how far along the generated positions we are.
                // The maximum bend should be halfway across the chain.
                bendOffset *= CurrentBendFactor * CalamityUtils.Convert01To010(i / 12f);

                // Smoothly zero out the bending effects if the current position is near the owner.
                bendOffset *= Utils.InverseLerp(0f, 300f, Owner.Distance(Vector2.Lerp(startingPosition, Projectile.Center, i / 12f) + bendOffset), true);
                initialPoints.Add(Vector2.Lerp(startingPosition, Projectile.Center, i / 12f) + bendOffset);
            }
            initialPoints.Add(Projectile.Center);

            BezierCurve bezierCurve = new BezierCurve(initialPoints.ToArray());
            int totalChains = (int)(Projectile.Distance(startingPosition) / 24f / Projectile.scale);
            totalChains = (int)MathHelper.Clamp(totalChains, 40f, 440f);

            WhipPoints = bezierCurve.GetPoints(totalChains);
        }

        public override void AI()
        {
            DetermineWhipPoints();

            // Determines the owner's position whilst incorporating their fullRotation field.
            // It uses vector transformation on a Z rotation matrix based on said rotation under the hood.
            // This is essentially just the pure mathematical definition of the RotatedBy method.
            Vector2 playerRotatedPosition = Owner.RotatedRelativePoint(Owner.MountedCenter);
            if (Main.myPlayer == Projectile.owner)
            {
                if (!Owner.noItems && !Owner.CCed)
                    HandleChannelMovement(playerRotatedPosition);
                else
                    Projectile.Kill();
            }

            ManipulatePlayerValues();

            // Create a bunch of prismatic lasers outward from the non-arcing whip right before it comes back
            // to its owner.
            if (SwingDirection == 0f && Projectile.timeLeft == FlyBackTime)
                CreateBadassPrismExplosion();
            Time++;
        }

        public void HandleChannelMovement(Vector2 playerRotatedPosition)
        {
            // Set the initial direction as a base for rotation.
            if (InitialDirectionRotation == 0f)
                InitialDirectionRotation = WhipOutwardness.ToRotation() - MathHelper.PiOver2;

            float attackCompletionRatio = Utils.InverseLerp(Lifetime, FlyBackTime, Projectile.timeLeft, true);

            // Normally swing from a "cone" to a collision area that causes both whips to collide.
            float baseSwingAngle = MathHelper.Lerp(-1.1f, 1.57f, 1f - attackCompletionRatio);

            Vector2 swingDirection = (SwingDirection * baseSwingAngle + MathHelper.PiOver2).ToRotationVector2();
            swingDirection = swingDirection.RotatedBy(InitialDirectionRotation);

            // If the whip is ready to return to its owner, have its outwardness approach the player again.
            if (Projectile.timeLeft < FlyBackTime)
                WhipOutwardness = Vector2.Lerp(WhipOutwardness, InitialDirectionRotation.ToRotationVector2(), 0.1f);
            else
            {
                // Have acceleration vary based on how much time has passed.
                Vector2 swingSpeedIncrement = swingDirection * MathHelper.SmoothStep(3.8f, 13f, (float)Math.Pow(CalamityUtils.Convert01To010(Time / Lifetime), 8f));
                if (SwingDirection == 0f)
                    swingSpeedIncrement *= 0.84f;
                WhipOutwardness += swingSpeedIncrement;
            }
            Projectile.Center = playerRotatedPosition;
            Projectile.rotation = WhipOutwardness.ToRotation();
        }

        public void ManipulatePlayerValues()
        {
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            if (SwingDirection == 0f)
            {
                Owner.itemRotation = WhipOutwardness.ToRotation() * Projectile.direction;
                Owner.ChangeDir(Projectile.direction);
            }
        }

        public void CreateBadassPrismExplosion()
        {
            var sound = SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, Owner.Center);
            if (sound != null)
                sound.Pitch = MathHelper.Clamp(sound.Pitch + 0.15f, 0f, 1f);

            if (Main.myPlayer != Projectile.owner)
                return;

            Projectile.NewProjectile(WhipEnd, Vector2.Zero, ModContent.ProjectileType<ThanatosBoom>(), Projectile.damage * 2, 0f, Projectile.owner);

            // Fire a bunch of rays rays.
            int rayDamage = (int)(Projectile.damage * 1.5);
            NPC potentialTarget = WhipEnd.ClosestNPCAt(700f);
            for (int i = 0; i < LaserRayCount; i++)
            {
                float rayRotation = Projectile.rotation + MathHelper.Lerp(-0.57f, 0.57f, i / (float)LaserRayCount);
                float targetAimDisparity = 0f;
                if (potentialTarget != null)
                    targetAimDisparity = Projectile.rotation.ToRotationVector2().AngleBetween((potentialTarget.Center - Projectile.Center).SafeNormalize(Vector2.Zero));

                // By default, make the prism rays go outward a good amount based on their rotation
                // to give a fan look.
                Vector2 prismEndPosition = WhipEnd + rayRotation.ToRotationVector2() * 420f;

                // However, if a potential target is within the general line of sight of the whip,
                // fire all lasers at it instead. This results in the entire laser spectrum appearing as one
                // brilliant, blazing laser that does great damage. However, it does take some skill to do.
                if (potentialTarget != null && targetAimDisparity < MathHelper.Pi * 0.27f)
                    prismEndPosition = potentialTarget.Center + potentialTarget.velocity * 4f;

                int prismRay = Projectile.NewProjectile(prismEndPosition, Vector2.Zero, ModContent.ProjectileType<PrismRay>(), rayDamage, Projectile.knockBack * 0.2f, Projectile.owner);
                if (Main.projectile.IndexInRange(prismRay))
                {
                    Main.projectile[prismRay].ModProjectile<PrismRay>().RayHue = i / (float)LaserRayCount;
                    Main.projectile[prismRay].ModProjectile<PrismRay>().StartingPosition = WhipEnd;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            for (int i = 0; i < WhipPoints.Count - 1; i++)
            {
                string whipTexturePath;
                if (i == WhipPoints.Count - 2)
                    whipTexturePath = Texture;
                else if (i == 0)
                    whipTexturePath = "CalamityMod/Projectiles/Melee/SpineOfThanatosTail";
                else
                    whipTexturePath = $"CalamityMod/Projectiles/Melee/SpineOfThanatosBody{i % 2 + 1}";
                Texture2D whipSegmentTexture = ModContent.Request<Texture2D>(whipTexturePath);
                Texture2D whipSegmentGlowmaskTexture = ModContent.Request<Texture2D>($"{whipTexturePath}Glowmask");

                Vector2 origin = whipSegmentTexture.Size() * 0.5f;
                float rotation = (WhipPoints[i + 1] - WhipPoints[i]).ToRotation() + MathHelper.PiOver2;
                Vector2 drawPosition = WhipPoints[i] - Main.screenPosition;
                Color color = Projectile.GetAlpha(Lighting.GetColor((int)WhipPoints[i].X / 16, (int)WhipPoints[i].Y / 16));
                spriteBatch.Draw(whipSegmentTexture, drawPosition, null, color, rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(whipSegmentGlowmaskTexture, drawPosition, null, Color.White, rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            }

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (WhipPoints.Count <= 1)
                return false;

            float width = Projectile.scale * 38f;
            for (int i = 0; i < WhipPoints.Count - 1; i++)
            {
                float _ = 0f;
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), WhipPoints[i], WhipPoints[i + 1], width, ref _))
                    return true;
            }
            return false;
        }
    }
}
