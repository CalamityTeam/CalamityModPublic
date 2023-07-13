using Terraria.DataStructures;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Microsoft.Xna.Framework;
using Terraria;
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
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Bullet;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Both bullets and water jets have very slight inaccuracy
            float SpeedX = velocity.X + Main.rand.Next(-10, 11) * 0.05f;
            float SpeedY = velocity.Y + Main.rand.Next(-10, 11) * 0.05f;

            // Fire either the bullet or the water jet, depending on cadence.
            int projectileToFire = fireWater ? ModContent.ProjectileType<ArcherfishShot>() : type;
            Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, projectileToFire, damage, knockback, player.whoAmI, 0.0f, 0.0f);

            // Always fires a close range water blast.
            // It goes in the same direction as the main shot but has a minor velocity variation to be less monotonous.
            Vector2 waterRingVec = new Vector2(SpeedX, SpeedY) * Main.rand.NextFloat(0.5f, 0.6f);
            int waterRingDamage = (int)(damage * 0.5f);
            float boostedKB = knockback + 7f; // Stronger guaranteed KB than Archerfish for mid-late Hardmode
            Projectile.NewProjectile(source, position, waterRingVec, ModContent.ProjectileType<ArcherfishRing>(), waterRingDamage, boostedKB, player.whoAmI);

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
