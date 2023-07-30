using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Archerfish : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.damage = 16;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 78;
            Item.height = 36;
            Item.useTime = 11;
            Item.useAnimation = 11;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item85;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ArcherfishShot>();
            Item.shootSpeed = 11f;
            Item.useAmmo = AmmoID.Bullet;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, -5);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Reposition to the gun's tip
            Vector2 newPos = position + new Vector2(60f, player.direction * (Math.Abs(velocity.SafeNormalize(Vector2.Zero).X) < 0.02f ? -2f : -8f)).RotatedBy(velocity.ToRotation());

            // Fires a spread of harmless bubbles
            for (int i = 0; i < 4; i++)
            {
                Gore bubble = Gore.NewGorePerfect(source, newPos, velocity.RotatedByRandom(MathHelper.ToRadians(30f)) * 0.5f, 411);
                bubble.timeLeft = 6 + Main.rand.Next(4);
                bubble.scale = Main.rand.NextFloat(0.6f, 0.8f);
                bubble.type = Main.rand.NextBool(3) ? 412 : 411;
            }

            // Replaces standard bullets with water jets.
            if (type == ProjectileID.Bullet)
                Projectile.NewProjectile(source, newPos, velocity, Item.shoot, damage, knockback, player.whoAmI);
            else
                Projectile.NewProjectile(source, newPos, velocity, type, damage, knockback, player.whoAmI);

            // Always fires a close range water blast.
            int waterRingDamage = (int)(damage * 0.5f);
            float boostedKB = knockback + 5f;
            Projectile.NewProjectile(source, newPos, velocity * 0.5f, ModContent.ProjectileType<ArcherfishRing>(), waterRingDamage, boostedKB, player.whoAmI);

            return false;
        }
    }
}
