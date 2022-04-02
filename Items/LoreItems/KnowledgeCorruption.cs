using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeCorruption : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Corruption");
            Tooltip.SetDefault("The rotten and forever-deteriorating landscape of infected life, brought upon by a deadly microbe long ago.\n" +
                "It is rumored that the microbe was created through experimentation by a long-dead race, predating the Terrarians.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Green;
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
            r.AddIngredient(ItemID.EaterofWorldsTrophy);
            r.AddIngredient(ModContent.ItemType<VictoryShard>(), 10);
            r.AddRecipe();
        }
    }
}
