using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgePlaguebringerGoliath : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Plaguebringer Goliath");
            Tooltip.SetDefault("A horrific amalgam of steel, flesh, and infection, capable of destroying an entire civilization in just one onslaught.\n" +
                "Its plague nuke barrage can leave an entire area uninhabitable for months. A shame that it came to this but the plague must be contained.\n" +
                "Favorite this item to gain 20% increased wing flight time, but at the cost of reduced life regen.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 8;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
			if (item.favorited)
				modPlayer.plaguebringerGoliathLore = true;
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddTile(TileID.Bookcases);
            r.AddIngredient(ModContent.ItemType<PlaguebringerGoliathTrophy>());
            r.AddIngredient(ModContent.ItemType<VictoryShard>(), 10);
            r.AddRecipe();
        }
    }
}
