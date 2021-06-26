using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class RainbowPartyCannonProjectile : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public ref float Time => ref projectile.ai[0];
        public const float ChargeDelay = 60f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Party Cannon");
        }

        public override void SetDefaults()
        {
            projectile.width = 92;
            projectile.height = 66;
            projectile.penetrate = -1;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Item heldItem = Owner.ActiveItem();
            projectile.damage = (int)((heldItem?.damage ?? 0) * Owner.MagicDamage());

            UpdatePlayerVisuals(Owner.Center);

            bool stillUsingCannon = Owner.channel && !Owner.noItems && !Owner.CCed;
            if (!stillUsingCannon)
            {
                projectile.Kill();
                return;
            }

            if (Time > ChargeDelay && Time % heldItem.useTime == 0)
            {
                ConsumeManaAndFireProjectile(heldItem);
            }
            else if (Time <= ChargeDelay && !Main.dedServ)
            {
                if (Main.rand.NextBool(3))
                {
                    Vector2 velocityDirection = projectile.velocity.RotatedByRandom(0.1f);
                    Vector2 shotOffset = velocityDirection * projectile.Size * 0.5f;
                    Dust.NewDustPerfect(projectile.Center + shotOffset, Main.rand.Next(139, 143), velocityDirection * Main.rand.NextFloat(5f, 9f));
                }
            }

            Time++;

            // Forces the cannon to disappear immediately if anything goes wrong.
            projectile.timeLeft = 2;
        }

        public void ConsumeManaAndFireProjectile(Item heldItem)
        {
            if (Owner.whoAmI != Main.myPlayer)
                return;

            if (!Owner.CheckMana(heldItem.mana, true))
            {
                projectile.Kill();
                return;
            }

            Main.PlaySound(heldItem.UseSound, projectile.Center);
            Vector2 spawnPosition = projectile.Center + projectile.velocity * 150f;

            // Shoot farther back if the cannon is shoved into a bunch of tiles.
            if (!Collision.CanHitLine(Owner.MountedCenter, 16, 16, spawnPosition, 16, 16))
                spawnPosition = projectile.Center + projectile.velocity * 50f;

            Projectile.NewProjectile(spawnPosition, projectile.velocity.SafeNormalize(Vector2.Zero) * heldItem.shootSpeed, ModContent.ProjectileType<RainbowComet>(), projectile.damage, projectile.knockBack, Owner.whoAmI);
        }

        public void UpdatePlayerVisuals(Vector2 center)
        {
            // Place the cannon directly into the player's hand at all times.
            projectile.Center = center;

            // Set the direction of the cannon if it hasn't been set yet.
            if (projectile.velocity == Vector2.Zero || projectile.velocity.Length() != 1f)
                projectile.velocity = (Main.MouseWorld - center).SafeNormalize(Vector2.UnitX * projectile.direction);

            projectile.rotation = projectile.velocity.ToRotation();
            projectile.direction = projectile.spriteDirection = (Math.Cos(projectile.rotation) > 0).ToDirectionInt();

            // The cannon is a holdout projectile. Change the player's channel and direction values to reflect this.
            Owner.ChangeDir(projectile.direction);
            Owner.heldProj = projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            Owner.itemRotation = (projectile.velocity * projectile.direction).ToRotation();
            if (projectile.spriteDirection == -1)
                projectile.rotation += MathHelper.Pi;
        }

        public override bool CanDamage() => false;
    }
}
