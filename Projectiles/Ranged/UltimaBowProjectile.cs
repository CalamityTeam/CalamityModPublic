using CalamityMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class UltimaBowProjectile : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Ultima>();
        public override string Texture => "CalamityMod/Items/Weapons/Ranged/Ultima";

        public float Time
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public const float PositioningOffset = 35f;
        public override void SetDefaults()
        {
            Projectile.width = 82;
            Projectile.height = 114;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.direction = (Math.Cos(Projectile.velocity.ToRotation()) > 0).ToDirectionInt();
            AttemptToFireProjectiles(player);
            AttachToPlayer(player);
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == -1).ToInt() * MathHelper.Pi;
            Time++;
        }

        public void AttemptToFireProjectiles(Player player)
        {
            bool canFire = player.channel && player.HasAmmo(player.ActiveItem()) && !player.noItems && !player.CCed;
            if (!canFire)
            {
                Projectile.Kill();
                return;
            }
            if (Projectile.owner == Main.myPlayer && Time % player.ActiveItem().useTime == 0)
            {
                int type = ProjectileID.WoodenArrowFriendly; // This doesn't really matter. It's overwritten anyway. But it is passed into the PickAmmo method.
                float shotSpeed = player.ActiveItem().shootSpeed;
                int damage = player.GetWeaponDamage(player.ActiveItem());
                float knockBack = player.ActiveItem().knockBack;

                player.PickAmmo(player.ActiveItem(), out type, out shotSpeed, out damage, out knockBack, out _); // Pick ammo and consume it (this incorporates the bow's chance to not consume ammo).

                if (player.ActiveItem().UseSound.HasValue)
                    SoundEngine.PlaySound(player.ActiveItem().UseSound.GetValueOrDefault(), Projectile.Center);

                type = ModContent.ProjectileType<UltimaBolt>();
                float shootLaserChance = Utils.GetLerpValue(Ultima.FullChargeTime * 0.35f, Ultima.FullChargeTime, Time, true);
                Vector2 shotPosition = player.RotatedRelativePoint(player.MountedCenter, true);
                shotPosition += Projectile.velocity.ToRotation().ToRotationVector2().RotatedByRandom(MathHelper.ToRadians(40f)).RotatedBy(-0.25f * Projectile.spriteDirection) * 42f;

                Projectile.velocity = player.SafeDirectionTo(Main.MouseWorld);

                Vector2 shotVelocity = Projectile.velocity * shotSpeed; // The velocity should always be a unit vector.

                // Fire a laser.
                if (Main.rand.NextFloat() <= shootLaserChance)
                {
                    type = ModContent.ProjectileType<UltimaRay>();
                    shotVelocity = shotVelocity.RotatedByRandom(0.03f);
                }
                // Sometimes fire little sparks if the bow is fully charged.
                if (Time >= Ultima.FullChargeTime * 0.7f && Main.rand.NextBool(6))
                {
                    // To ensure that the sparks don't spawn on top of the laser itself.
                    float offsetAngle = Main.rand.NextFloat(0.2f, 0.5f) * Main.rand.NextBool().ToDirectionInt();
                    Vector2 sparkVelocity = Projectile.SafeDirectionTo(Main.MouseWorld, Vector2.UnitY).RotatedByRandom(0.5f) * 13f;
                    sparkVelocity = sparkVelocity.RotatedBy(offsetAngle);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), shotPosition, sparkVelocity, ModContent.ProjectileType<UltimaSpark>(), damage / 3, knockBack, Projectile.owner);
                }
                knockBack = player.GetWeaponKnockback(player.ActiveItem(), knockBack);

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), shotPosition, shotVelocity, type, damage, knockBack, Projectile.owner);
                Projectile.netUpdate = true;
            }
        }

        public void AttachToPlayer(Player player)
        {
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter, true) + Projectile.velocity.ToRotation().ToRotationVector2() * PositioningOffset;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = player.itemAnimation = 2;
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
        }

        public override bool? CanDamage() => false;
    }
}
