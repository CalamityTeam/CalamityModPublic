using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeAquaticScourge : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aquatic Scourge");
            Tooltip.SetDefault("A horror born of pollution and insatiable hunger; based on size alone this was merely a juvenile.\n" +
                "These scourge creatures are the largest aquatic predators and very rarely do they frequent such shallow waters.\n" +
                "Favorite this item to gain immunity to the sulphurous waters and increase the stat gains from the Well Fed buff.\n" +
                "However, without the Well Fed buff your stats will decrease due to your insatiable hunger.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 5;
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
				modPlayer.aquaticScourgeLore = true;
		}

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddTile(TileID.Bookcases);
            r.AddIngredient(ModContent.ItemType<AquaticScourgeTrophy>());
            r.AddIngredient(ModContent.ItemType<VictoryShard>(), 10);
            r.AddRecipe();
        }
    }
}
