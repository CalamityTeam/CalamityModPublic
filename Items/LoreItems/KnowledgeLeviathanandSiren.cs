using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeLeviathanandSiren : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Leviathan and Anahita");
            Tooltip.SetDefault("An odd pair of creatures; one seeking companionship and the other seeking sustenance.\n" +
                "Perhaps two genetic misfits outcast from their homes that found comfort in assisting one another.\n" +
                "Favorite this item to gain increased max health while wearing the aquatic heart and treasure detect while wearing the strange orb.\n" +
                "Allows the young Ocean Spirit light pet to move normally while outside of liquids.\n" +
				"However, if you're not submerged in liquid you will have decreased defense and damage reduction.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 7;
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
				modPlayer.leviathanAndSirenLore = true;
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddTile(TileID.Bookcases);
            r.AddIngredient(ModContent.ItemType<LeviathanTrophy>());
            r.AddIngredient(ModContent.ItemType<VictoryShard>(), 10);
            r.AddRecipe();
        }
    }
}
