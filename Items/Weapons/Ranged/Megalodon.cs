using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Megalodon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        private bool fireWater = false;

        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 72;
            Item.height = 32;
            Item.useTime = 6;
            Item.useAnimation = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.5f;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ArcherfishShot>();
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Bullet;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Reposition to the gun's tip + add some inaccuracy
            Vector2 newPos = position + new Vector2(64f, player.direction * (Math.Abs(velocity.SafeNormalize(Vector2.Zero).X) < 0.02f ? -2f : -8f)).RotatedBy(velocity.ToRotation());
            Vector2 newVel = velocity.RotatedByRandom(MathHelper.ToRadians(5f));

            // Fire either the bullet or the water jet, depending on cadence.
            int projectileToFire = fireWater ? Item.shoot : type;
            Projectile.NewProjectile(source, newPos, newVel, projectileToFire, damage, knockback, player.whoAmI);

            // Always fires a close range water blast.
            // It goes in the same direction as the main shot but has a minor velocity variation to be less monotonous.
            int waterRingDamage = (int)(damage * 0.5f);
            float boostedKB = knockback + 7f; // Stronger guaranteed KB than Archerfish for mid-late Hardmode
            Projectile.NewProjectile(source, newPos, newVel * Main.rand.NextFloat(0.5f, 0.6f), ModContent.ProjectileType<ArcherfishRing>(), waterRingDamage, boostedKB, player.whoAmI);

            // Swap between firing bullets and water jets each shot.
            fireWater = !fireWater;
            return false;
        }

        // Does not consume ammo when firing water jets.
        public override bool CanConsumeAmmo(Item ammo, Player player) => !fireWater;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Megashark).
                AddIngredient<Archerfish>().
                AddIngredient<Voidstone>(10).
                AddIngredient<DepthCells>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
