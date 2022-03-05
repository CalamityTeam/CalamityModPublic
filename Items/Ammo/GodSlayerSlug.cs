using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class GodSlayerSlug : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("God Slayer Slug");
            Tooltip.SetDefault("Heavy ammunition with unlimited piercing that tears through spacetime\n" +
                "After a slug lands a hit, if it strikes a wall or runs out of targets to pierce,\n" +
                "it warps backwards through space and supercharges, exploding on impact");
        }

        public override void SetDefaults()
        {
            item.damage = 28;
            item.ranged = true;
            item.width = 22;
            item.height = 22;
            item.maxStack = 999;
            item.consumable = true;
            item.knockBack = 3f;
            item.value = Item.sellPrice(copper: 28);
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.shoot = ModContent.ProjectileType<GodSlayerSlugProj>();
            item.shootSpeed = 6f;
            item.ammo = ItemID.MusketBall;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.EmptyBullet, 999);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>());
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this, 999);
            recipe.AddRecipe();
        }
    }
}
