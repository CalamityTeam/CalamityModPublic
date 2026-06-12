using System;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class OmnibladeSwing : ModProjectile, ILocalizedModType
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Omniblade>();
        public Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 308;
            Projectile.height = 184;

            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = TrueMeleeNoSpeedDamageClass.Instance;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.ownerHitCheck = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 5;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            float scale = Owner.GetMeleeScale();
            Vector2 newSize = new Point(hitbox.Width, hitbox.Height).ToVector2() * scale;
            hitbox = new Rectangle(hitbox.X - (int)((newSize.X - hitbox.Width) / 2f), hitbox.Y - (int)((newSize.Y - hitbox.Height) / 2f), (int)newSize.X, (int)newSize.Y);
        }
        public override void AI()
        {
            Projectile.scale = Owner.GetMeleeScale();
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 3;
            if (Projectile.frame >= Main.projFrames[Type])
                Projectile.Kill();

            Vector2 playerRotatedPoint = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            if (Main.myPlayer == Projectile.owner)
            {
                if (!Owner.CantUseHoldout())
                    HandleChannelMovement(playerRotatedPoint);
                else
                    Projectile.Kill();
            }

            // Rotation and directioning.
            Projectile.rotation = Owner.gravDir == -1f ? MathHelper.Pi : 0f;
            Projectile.direction = Projectile.spriteDirection = MathF.Sign(Owner.Center.DirectionTo(Owner.ClampedMouseWorld()).X);
            Projectile.Center = Owner.MountedCenter + new Vector2(Projectile.width * 0.1f * Projectile.scale * Projectile.direction, -Projectile.height * 0.5f * Projectile.scale);
            Owner.ChangeDir(Projectile.direction);

            // Prevents the projectile from dying
            Projectile.timeLeft = 2;

            // Player item-based field manipulation.
            Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
        }

        public void HandleChannelMovement(Vector2 playerRotatedPoint)
        {
            Vector2 newVelocity = Vector2.UnitX * (Main.MouseWorld.X > playerRotatedPoint.X).ToDirectionInt();

            // Sync if a velocity component changes.
            if (Projectile.velocity.X != newVelocity.X || Projectile.velocity.Y != newVelocity.Y)
                Projectile.netUpdate = true;

            Projectile.velocity = newVelocity;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 170);

        // Don't suffer from the same issues Murasama did in the past; encouraging people to kill their wrists for some extra DPS is bad lmao
        public override bool? CanDamage() => Projectile.frameCounter > 6;
    }
}
