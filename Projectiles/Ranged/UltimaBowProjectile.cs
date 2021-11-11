using CalamityMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class UltimaBowProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Ranged/Ultima";

        public float Time
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        public const float PositioningOffset = 35f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ultima");
        }

        public override void SetDefaults()
        {
            projectile.width = 82;
            projectile.height = 114;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ranged = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            projectile.direction = (Math.Cos(projectile.velocity.ToRotation()) > 0).ToDirectionInt();
            AttemptToFireProjectiles(player);
            AttachToPlayer(player);
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == -1).ToInt() * MathHelper.Pi;
            Time++;
        }

        public void AttemptToFireProjectiles(Player player)
        {
            bool canFire = player.channel && player.HasAmmo(player.ActiveItem(), true) && !player.noItems && !player.CCed;
            if (!canFire)
            {
                projectile.Kill();
                return;
            }
            if (projectile.owner == Main.myPlayer && Time % player.ActiveItem().useTime == 0)
            {
                int type = ProjectileID.WoodenArrowFriendly; // This doesn't really matter. It's overwritten anyway. But it is passed into the PickAmmo method.
                float shotSpeed = player.ActiveItem().shootSpeed;
                int damage = player.GetWeaponDamage(player.ActiveItem());
                float knockBack = player.ActiveItem().knockBack;

                player.PickAmmo(player.ActiveItem(), ref type, ref shotSpeed, ref canFire, ref damage, ref knockBack); // Pick ammo and consume it (this incorporates the bow's chance to not consume ammo).

                Main.PlaySound(player.ActiveItem().UseSound, projectile.Center);

                type = ModContent.ProjectileType<UltimaBolt>();
                float shootLaserChance = Utils.InverseLerp(Ultima.FullChargeTime * 0.35f, Ultima.FullChargeTime, Time, true);
                Vector2 shotPosition = player.RotatedRelativePoint(player.MountedCenter, true);
                shotPosition += projectile.velocity.ToRotation().ToRotationVector2().RotatedByRandom(MathHelper.ToRadians(40f)).RotatedBy(-0.25f * projectile.spriteDirection) * 42f;

                projectile.velocity = projectile.SafeDirectionTo(Main.MouseWorld);

                Vector2 shotVelocity = projectile.velocity * shotSpeed; // The velocity should always be a unit vector.

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
                    float offsetAngle = Main.rand.NextFloat(0.2f, 0.5f) * Main.rand.NextBool(2).ToDirectionInt();
                    Vector2 sparkVelocity = projectile.SafeDirectionTo(Main.MouseWorld, Vector2.UnitY).RotatedByRandom(0.5f) * 13f;
                    sparkVelocity = sparkVelocity.RotatedBy(offsetAngle);
                    Projectile.NewProjectile(shotPosition, sparkVelocity, ModContent.ProjectileType<UltimaSpark>(), damage / 3, knockBack, projectile.owner);
                }
                knockBack = player.GetWeaponKnockback(player.ActiveItem(), knockBack);

                Projectile.NewProjectile(shotPosition, shotVelocity, type, damage, knockBack, projectile.owner);
                projectile.netUpdate = true;
            }
        }

        public void AttachToPlayer(Player player)
        {
            projectile.Center = player.RotatedRelativePoint(player.MountedCenter, true) + projectile.velocity.ToRotation().ToRotationVector2() * PositioningOffset;
            projectile.spriteDirection = projectile.direction;
            projectile.timeLeft = 2;
            player.ChangeDir(projectile.direction);
            player.heldProj = projectile.whoAmI;
            player.itemTime = player.itemAnimation = 2;
            player.itemRotation = (projectile.velocity * projectile.direction).ToRotation();
        }

        public override bool CanDamage() => false;
    }
}
