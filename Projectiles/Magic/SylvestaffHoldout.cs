using System;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Physics;
using CalamityMod.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class SylvestaffHoldout : ModProjectile, IPixelatedPrimitiveRenderer
    {
        public GeneralDrawLayer LayerToRenderTo => GeneralDrawLayer.BeforeProjectiles | GeneralDrawLayer.AfterPlayers;

        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Sylvestaff>();

        /// <summary>
        ///     The left ribbon on this holdout.
        /// </summary>
        public RopeHandle? LeftRibbon;

        /// <summary>
        ///     The right ribbon on this holdout.
        /// </summary>
        public RopeHandle? RightRibbon;

        /// <summary>
        ///     The player owner of this holdout staff.
        /// </summary>
        public Player Owner => Main.player[Projectile.owner];

        /// <summary>
        ///     A general purpose, ever-increment timer used by this holdout staff.
        /// </summary>
        public ref float Time => ref Projectile.ai[0];

        /// <summary>
        ///     The point of attachment for ribbons on this staff.
        /// </summary>
        public Vector2 RibbonAttachPoint => Projectile.Center + Projectile.velocity * Projectile.scale * Projectile.width * 0.34f;

        /// <summary>
        ///     The length of ribbons attached to this staff.
        /// </summary>
        private static float RibbonLength => 70f;

        public override string Texture => ModContent.GetInstance<Sylvestaff>().Texture;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 94;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.timeLeft = 72000;
        }

        public override void AI()
        {
            if (!Owner.channel)
                Projectile.Kill();

            if (LeftRibbon is null && RightRibbon is null)
                InitializeRibbons();
            // Refund the mana used to spawn the holdout
            if (Time == 0)
                Owner.statMana += Owner.GetManaCost(Owner.HeldItem);

            AimTowardsMouse();
            HandleHoldoutLogic();
            OrientOwnerArms();
            FireAwesomeMagicRays();
            UpdateRibbon(LeftRibbon, Projectile.velocity.RotatedBy(-MathHelper.PiOver2));
            UpdateRibbon(RightRibbon, Projectile.velocity.RotatedBy(MathHelper.PiOver2));

            Time++;
        }

        /// <summary>
        ///     Initializes the ribbons attached to this staff.
        /// </summary>
        private void InitializeRibbons()
        {
            int ribbonSegmentCount = 12;
            float distancePerSegment = RibbonLength / ribbonSegmentCount;
            RopeSettings ribbonSettings = new RopeSettings()
            {
                StartIsFixed = true,
                Mass = 0.72f,
                RespondToEntityMovement = true,
                RespondToWind = true
            };
            LeftRibbon = ModContent.GetInstance<RopeManagerSystem>().RequestNew(RibbonAttachPoint, Projectile.Center, ribbonSegmentCount, distancePerSegment, Vector2.Zero, ribbonSettings, 25);
            RightRibbon = ModContent.GetInstance<RopeManagerSystem>().RequestNew(RibbonAttachPoint, Projectile.Center, ribbonSegmentCount, distancePerSegment, Vector2.Zero, ribbonSettings, 25);
        }

        /// <summary>
        ///     Handles all logic pertaining to making this staff act as a holdout projectile.
        /// </summary>
        private void HandleHoldoutLogic()
        {
            Vector2 center = Owner.MountedCenter + Vector2.UnitY * 7f + Projectile.velocity * Projectile.width * 0.31f;

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Center = Owner.RotatedRelativePoint(center) - Vector2.UnitY * Owner.gfxOffY;
            Projectile.spriteDirection = (Math.Cos(Projectile.rotation) > 0).ToDirectionInt();

            // The staff is a holdout projectile; change the player's variables to reflect this.
            Owner.ChangeDir(Projectile.spriteDirection);
            Owner.SetDummyItemTime(2);
            Owner.heldProj = Projectile.whoAmI;

            Projectile.rotation += MathHelper.PiOver4;
            if (Projectile.spriteDirection == -1)
                Projectile.rotation += MathHelper.PiOver2;
        }

        /// <summary>
        ///     Orients the owner player's arm rotation to help make it look like they're actually holding the staff.
        /// </summary>
        private void OrientOwnerArms()
        {
            float baseRotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            float directionVerticality = MathF.Abs(Projectile.velocity.X);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * MathHelper.PiOver4);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 0.33f);
        }

        /// <summary>
        ///     Makes this staff aim towards the owner's mouse.
        /// </summary>
        private void AimTowardsMouse()
        {
            if (Main.myPlayer != Projectile.owner)
                return;

            Vector2 idealDirection = Projectile.SafeDirectionTo(Main.MouseWorld);
            Vector2 newDirection = Vector2.Lerp(Projectile.velocity, idealDirection, Sylvestaff.TurnSpeedInterpolant).SafeNormalize(Vector2.UnitX * Owner.direction);
            if (Projectile.velocity != newDirection)
            {
                Projectile.velocity = newDirection;
                Projectile.netUpdate = true;
                Projectile.netSpam = 0;
            }
        }

        /// <summary>
        ///     Updates a given ribbon.
        /// </summary>
        private void UpdateRibbon(RopeHandle? ribbon, Vector2 gravityDirection)
        {
            // Ensure that the handle is properly initialized before proceeding any further.
            if (ribbon is not RopeHandle rope)
                return;

            rope.Start = RibbonAttachPoint;
            rope.Gravity = gravityDirection * 0.15f - Projectile.velocity * 0.4f;
        }

        /// <summary>
        ///     Handles the firing of magic ray projectiles for this staff.
        /// </summary>
        private void FireAwesomeMagicRays()
        {
            Item heldItem = Owner.HeldItem;
            if (heldItem is null)
                return;

            if (Time % heldItem.useAnimation == heldItem.useAnimation - 1 && Owner.CheckMana(heldItem.mana, true))
            {
                SoundEngine.PlaySound(Sylvestaff.FireSound, Projectile.Center);
                if (Main.myPlayer == Projectile.owner)
                {
                    int damage = Owner.GetWeaponDamage(heldItem);
                    Vector2 shootVelocity = Projectile.velocity * heldItem.shootSpeed;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVelocity, ModContent.ProjectileType<SylvRay>(), damage, heldItem.knockBack, Projectile.owner);

                    // Apply a minor amount of recoil.
                    Projectile.velocity -= Projectile.velocity.RotatedBy(Projectile.spriteDirection * MathHelper.PiOver2) * Sylvestaff.StaffRecoilForce;
                }
            }
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch, GeneralDrawLayer layer)
        {
            bool backLayer = layer == GeneralDrawLayer.BeforeProjectiles;
            RenderRibbon(LeftRibbon, -1, backLayer);
            RenderRibbon(RightRibbon, 1, backLayer);
        }

        /// <summary>
        ///     The function responsible for dictating the width of this staff's ribbons.
        /// </summary>
        private float RibbonWidthFunction(float completionRatio, Vector2 vertexPos) => Projectile.scale * Utils.GetLerpValue(0f, 0.2f, completionRatio, true) * 3.6f;

        /// <summary>
        ///     The function responsible for dictating the color of this staff's ribbons.
        /// </summary>
        private Color RibbonColorFunction(float completionRatio, Vector2 vertexPos)
        {
            Color light = Lighting.GetColor(RibbonAttachPoint.ToTileCoordinates());
            return Projectile.GetAlpha(light);
        }

        /// <summary>
        ///     Renders one of this staff's ribbons.
        /// </summary>
        private void RenderRibbon(RopeHandle? ribbon, int direction, bool backLayer)
        {
            // Ensure that the handle is properly initialized before proceeding any further.
            if (ribbon is not RopeHandle rope)
                return;

            Vector2 forwardDirection = Projectile.velocity;
            Vector2 sideDirection = forwardDirection.RotatedBy(MathHelper.PiOver2 * direction);
            Vector2 attachmentPoint = RibbonAttachPoint;
            Vector2[] ribbonPositions = [.. rope.Positions];
            int positionCount = ribbonPositions.Length;
            for (int i = 0; i < ribbonPositions.Length; i++)
            {
                float completionRatio = i / (float)positionCount;
                float wave = MathF.Cos(MathHelper.Pi * completionRatio * 1.5f - MathHelper.TwoPi * Time / 97f) * completionRatio;

                Vector2 backwardsOffset = forwardDirection * i * -RibbonLength / positionCount;
                Vector2 sideWavyOffset = sideDirection * wave * RibbonLength * 0.5f;
                Vector2 rigidPosition = attachmentPoint + backwardsOffset + sideWavyOffset;

                ribbonPositions[i] = Vector2.Lerp(ribbonPositions[i], rigidPosition, 0.76f);
            }

            Vector2 intersectionPosition = Vector2.Transform((Projectile.Center - Main.screenPosition) * 0.5f, Main.GameViewMatrix.TransformationMatrix);
            MiscShaderData ribbonShader = GameShaders.Misc["CalamityMod:SylvestaffRibbon"];
            ribbonShader.UseShaderSpecificData(new Vector4(intersectionPosition.X, intersectionPosition.Y, sideDirection.X, sideDirection.Y));
            ribbonShader.UseSaturation(backLayer ? -1f : 1f);

            PrimitiveSettings primitiveSettings = new PrimitiveSettings(RibbonWidthFunction, RibbonColorFunction, pixelate: true, shader: ribbonShader);
            PrimitiveRenderer.RenderTrail(ribbonPositions, primitiveSettings, 33);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            SpriteEffects direction = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, direction, 0f);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            LeftRibbon?.Dispose();
            RightRibbon?.Dispose();
        }

        public override bool? CanDamage() => false;
    }
}
