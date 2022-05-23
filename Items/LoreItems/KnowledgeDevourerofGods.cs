using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeDevourerofGods : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Devourer of Gods");
            Tooltip.SetDefault("This serpent’s power to assimilate the abilities and energy of those it consumed is unique in almost all the known cosmos, save for its lesser brethren.\n" +
                "I would have soon had to eliminate it as a threat had it been given more time and creatures to feast upon.");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Red;
            Item.consumable = false;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ModContent.ItemType<DevourerofGodsTrophy>()).AddIngredient(ModContent.ItemType<VictoryShard>(), 10).Register();
        }
    }
}
