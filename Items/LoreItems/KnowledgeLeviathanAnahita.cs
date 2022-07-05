using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeLeviathanandSiren")]
    public class KnowledgeLeviathanAnahita : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Leviathan and Anahita");
            Tooltip.SetDefault("An odd pair of creatures; one seeking companionship and the other seeking sustenance.\n" +
                "Perhaps two genetic misfits outcast from their homes that found comfort in assisting one another.");
            SacrificeTotal = 1;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Lime;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ModContent.ItemType<LeviathanTrophy>()).AddIngredient(ModContent.ItemType<VictoryShard>(), 10).Register();
        }
    }
}
