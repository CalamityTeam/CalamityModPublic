using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
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
            Tooltip.SetDefault("These bullets aren't finished.");
        }

        public override void SetDefaults()
        {
            item.damage = 42;
            item.ranged = true;
            item.width = 22;
            item.height = 22;
            item.maxStack = 999;
            item.consumable = true;
            item.knockBack = 3f;
            item.value = Item.buyPrice(silver: 4);
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.shoot = ModContent.ProjectileType<GodSlayerSlugMain>();
            item.shootSpeed = 6f;
            item.ammo = ItemID.MusketBall;
        }

        public override void AddRecipes()
        {
            /*
			ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>());
            recipe.AddRecipeGroup("NForEE");
            recipe.AddIngredient(ItemID.EmptyBullet, 999);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this, 999);
            recipe.AddRecipe();
			*/
        }
    }
}
