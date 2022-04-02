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
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Red;
            item.consumable = false;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
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
            r.AddIngredient(ModContent.ItemType<DevourerofGodsTrophy>());
            r.AddIngredient(ModContent.ItemType<VictoryShard>(), 10);
            r.AddRecipe();
        }
    }
}
