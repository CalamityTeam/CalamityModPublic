using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeAstralInfection : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Infection");
            Tooltip.SetDefault("This twisted dreamscape, surrounded by unnatural pillars under a dark and hazy sky.\n" +
                "Natural law has been upturned. What will you make of it?");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Cyan;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
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
