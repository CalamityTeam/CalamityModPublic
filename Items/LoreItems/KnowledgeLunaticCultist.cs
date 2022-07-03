using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeLunaticCultist : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lunatic Cultist");
            Tooltip.SetDefault("The gifted one that terminated my grand summoning so long ago with his uncanny powers over the arcane.\n" +
                "Someone I once held in such contempt for his actions is now... deceased, his sealing ritual undone... prepare for the end.\n" +
                "Your impending doom approaches...");
            SacrificeTotal = 1;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Cyan;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ItemID.AncientCultistTrophy).AddIngredient(ModContent.ItemType<VictoryShard>(), 10).Register();
        }
    }
}
