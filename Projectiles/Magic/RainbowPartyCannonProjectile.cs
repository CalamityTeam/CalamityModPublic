using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class RainbowPartyCannonProjectile : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<RainbowPartyCannon>();
        public Player Owner => Main.player[Projectile.owner];
        public ref float Time => ref Projectile.ai[0];
        public const float ChargeDelay = 60f;
        public override void SetDefaults()
        {
            Projectile.width = 92;
            Projectile.height = 66;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Item heldItem = Owner.ActiveItem();
            Projectile.damage = heldItem is null ? 0 : Owner.GetWeaponDamage(heldItem);

            UpdatePlayerVisuals(Owner.Center);

            bool stillUsingCannon = Owner.channel && !Owner.noItems && !Owner.CCed;
            if (!stillUsingCannon)
            {
                Projectile.Kill();
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
                    Vector2 velocityDirection = Projectile.velocity.RotatedByRandom(0.1f);
                    Vector2 shotOffset = velocityDirection * Projectile.Size * 0.5f;
                    Dust.NewDustPerfect(Projectile.Center + shotOffset, Main.rand.Next(139, 143), velocityDirection * Main.rand.NextFloat(5f, 9f));
                }
            }

            Time++;

            // Forces the cannon to disappear immediately if anything goes wrong.
            Projectile.timeLeft = 2;
        }

        public void ConsumeManaAndFireProjectile(Item heldItem)
        {
            if (Owner.whoAmI != Main.myPlayer)
                return;

            if (!Owner.CheckMana(heldItem.mana, true))
            {
                Projectile.Kill();
                return;
            }

            if (heldItem.UseSound.HasValue)
                SoundEngine.PlaySound(heldItem.UseSound.GetValueOrDefault(), Projectile.Center);

            Vector2 spawnPosition = Projectile.Center + Projectile.velocity * 150f;

            // Shoot farther back if the cannon is shoved into a bunch of tiles.
            if (!Collision.CanHitLine(Owner.MountedCenter, 16, 16, spawnPosition, 16, 16))
                spawnPosition = Projectile.Center + Projectile.velocity * 50f;

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPosition, Projectile.velocity.SafeNormalize(Vector2.Zero) * heldItem.shootSpeed, ModContent.ProjectileType<RainbowComet>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
        }

        public void UpdatePlayerVisuals(Vector2 center)
        {
            // Place the cannon directly into the player's hand at all times.
            Projectile.Center = center;

            // Set the direction of the cannon if it hasn't been set yet.
            if (Projectile.velocity == Vector2.Zero || Projectile.velocity.Length() != 1f)
                Projectile.velocity = (Main.MouseWorld - center).SafeNormalize(Vector2.UnitX * Projectile.direction);

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.direction = Projectile.spriteDirection = (Math.Cos(Projectile.rotation) > 0).ToDirectionInt();

            // The cannon is a holdout projectile. Change the player's channel and direction values to reflect this.
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
            if (Projectile.spriteDirection == -1)
                Projectile.rotation += MathHelper.Pi;
        }

        public override bool? CanDamage() => false;
    }
}
