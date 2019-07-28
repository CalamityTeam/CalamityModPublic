using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class BadgeofBravery : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Badge of Bravery");
            Tooltip.SetDefault("15% increased melee speed");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.value = Item.buyPrice(0, 21, 0, 0);
            item.accessory = true;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 12;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.badgeOfBravery = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "UeliaceBar", 2);
            recipe.AddIngredient(ItemID.FeralClaws);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

		public override void OnCraft(Recipe recipe)
		{
			if (Main.rand.Next(40) == 0)
				recipe.createItem.type = mod.ItemType("SamuraiBadge");
			else
				recipe.createItem.type = item.type;
		}
	}
}
