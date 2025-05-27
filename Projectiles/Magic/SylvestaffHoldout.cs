using System;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class SylvestaffHoldout : ModProjectile, IPixelatedPrimitiveRenderer
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Sylvestaff>();

        /// <summary>
        ///     The player owner of this holdout staff.
        /// </summary>
        public Player Owner => Main.player[Projectile.owner];

        /// <summary>
        ///     A general purpose, ever-increment timer used by this holdout staff.
        /// </summary>
        public ref float Time => ref Projectile.ai[0];

        public override string Texture => "CalamityMod/Items/Weapons/Magic/Sylvestaff";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 84;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
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

            AimTowardsMouse();
            HandleHoldoutLogic();
            OrientOwnerArms();
            FireAwesomeMagicRays();

            Time++;
        }

        /// <summary>
        ///     
        /// </summary>
        private void HandleHoldoutLogic()
        {
            Vector2 center = Owner.MountedCenter + Vector2.UnitY * 7f + Projectile.velocity * Projectile.width * 0.37f;

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
            Vector2 newDirection = Vector2.Lerp(Projectile.velocity, idealDirection, 0.2f).SafeNormalize(Vector2.UnitX * Owner.direction);
            if (Projectile.velocity != newDirection)
            {
                Projectile.velocity = newDirection;
                Projectile.netUpdate = true;
                Projectile.netSpam = 0;
            }
        }

        /// <summary>
        ///     Handles the firing of magic ray projectiles for this staff.
        /// </summary>
        private void FireAwesomeMagicRays()
        {
            Item heldItem = Owner.ActiveItem();
            if (heldItem is null)
                return;

            if (Time % heldItem.useAnimation == 0)
            {
                SoundEngine.PlaySound(SoundID.Item60, Projectile.Center);
                if (Main.myPlayer == Projectile.owner)
                {
                    int damage = Owner.GetWeaponDamage(heldItem);
                    Vector2 shootVelocity = Projectile.velocity * heldItem.shootSpeed;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVelocity, ModContent.ProjectileType<SylvRay>(), damage, heldItem.knockBack, Projectile.owner);

                    // Apply a minor amount of recoil.
                    Projectile.velocity -= Projectile.velocity.RotatedBy(Projectile.spriteDirection * MathHelper.PiOver2) * 0.16f;
                }
            }
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            /*
                feelerShader.Parameters["feelerColorStart"]?.SetValue(0.61f);
                feelerShader.Parameters["colorSpacingFactor"]?.SetValue(1.9f);
                feelerShader.Parameters["pixelationFactor"]?.SetValue(40f);
                feelerShader.Parameters["outlineColor"]?.SetValue(new Color(109, 102, 112).ToVector4());
             */
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return true;
        }

        public override bool? CanDamage() => false;
    }
}
