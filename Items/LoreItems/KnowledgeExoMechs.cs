using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeExoMechs : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Exo Mechanical Trio");
            Tooltip.SetDefault("The fruits of masterful craftsmanship and optimization, created only with the objective to destroy.\n" +
                "Yet in the end, they achieved little more than the original designs they were derived from.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Red;
            item.consumable = false;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool CanUseItem(Player player) => false;

        // Requires an Exo Mechs trophy.
        /*
        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddTile(TileID.Bookcases);
            r.AddIngredient(ModContent.ItemType<SupremeCalamitasTrophy>());
            r.AddIngredient(ModContent.ItemType<VictoryShard>(), 10);
            r.AddRecipe();
        }
        */
    }
}
