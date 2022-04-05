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
    public class Contagion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Contagion");
            Tooltip.SetDefault("Fires contagion arrows that leave exploding orbs behind as they travel");
        }

        public override void SetDefaults()
        {
            Item.damage = 880;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 22;
            Item.height = 50;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 5f;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ContagionBow>();
            Item.shootSpeed = 20f;
            Item.useAmmo = AmmoID.Arrow;

            Item.value = CalamityGlobalItem.Rarity16BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.HotPink;
            Item.Calamity().devItem = true;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<Phangasm>()).AddIngredient(ModContent.ItemType<PlagueCellCluster>(), 20).AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5).AddTile(ModContent.TileType<DraedonsForge>()).Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<ContagionBow>(), damage, knockback, player.whoAmI);
            return false;
        }
    }
}
