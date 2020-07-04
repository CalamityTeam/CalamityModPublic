using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeAstrumDeus : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astrum Deus");
            Tooltip.SetDefault("God of the stars and largest vessel for the Astral Infection.\n" +
                "Though struck down from its place among the stars its remnants have gathered strength, aiming to take its rightful place in the cosmos once more.\n" +
                "Favorite this item to gain increased movement speed in space.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 9;
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
				modPlayer.astrumDeusLore = true;
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddTile(TileID.Bookcases);
            r.AddIngredient(ModContent.ItemType<AstrumDeusTrophy>());
            r.AddIngredient(ModContent.ItemType<VictoryShard>(), 10);
            r.AddRecipe();
        }
    }
}
