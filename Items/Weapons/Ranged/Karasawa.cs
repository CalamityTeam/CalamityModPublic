using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Karasawa : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Karasawa");
            Tooltip.SetDefault("...This is heavy... too heavy.");
        }

        public override void SetDefaults()
        {
            Item.width = 94;
            Item.height = 44;
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 900;
            Item.knockBack = 12f;
            Item.useTime = 52;
            Item.useAnimation = 52;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Item/MechGaussRifle");
            Item.noMelee = true;

            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            Item.Calamity().donorItem = true;

            Item.shoot = ModContent.ProjectileType<KarasawaShot>();
            Item.shootSpeed = 1f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override bool CanUseItem(Player player)
        {
            return CalamityGlobalItem.HasEnoughAmmo(player, Item, 5);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            if (velocity.Length() > 5f)
            {
                velocity.Normalize();
                velocity *= 5f;
            }
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<KarasawaShot>(), damage, knockback, player.whoAmI, 0f, 0f);

            // Consume 5 ammo per shot
            CalamityGlobalItem.ConsumeAdditionalAmmo(player, Item, 5);

            return false;
        }

        // Disable vanilla ammo consumption
        public override bool CanConsumeAmmo(Player player)
        {
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-20, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.LargeRuby).AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 15).AddIngredient(ModContent.ItemType<DubiousPlating>(), 25).AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5).AddIngredient(ModContent.ItemType<CosmiliteBar>(), 8).AddIngredient(ModContent.ItemType<NightmareFuel>(), 20).AddTile(ModContent.TileType<CosmicAnvil>()).Register();
        }
    }
}
