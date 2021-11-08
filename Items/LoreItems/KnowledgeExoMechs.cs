using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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


        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddTile(TileID.Bookcases);
            r.AddIngredient(ModContent.ItemType<ArtemisTrophy>());
            r.AddIngredient(ModContent.ItemType<VictoryShard>(), 10);
            r.AddRecipe();

            r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddTile(TileID.Bookcases);
            r.AddIngredient(ModContent.ItemType<ApolloTrophy>());
            r.AddIngredient(ModContent.ItemType<VictoryShard>(), 10);
            r.AddRecipe();

            r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddTile(TileID.Bookcases);
            r.AddIngredient(ModContent.ItemType<ThanatosTrophy>());
            r.AddIngredient(ModContent.ItemType<VictoryShard>(), 10);
            r.AddRecipe();

            r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddTile(TileID.Bookcases);
            r.AddIngredient(ModContent.ItemType<AresTrophy>());
            r.AddIngredient(ModContent.ItemType<VictoryShard>(), 10);
            r.AddRecipe();
        }
    }
}
