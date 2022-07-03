using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeDukeFishron : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Duke Fishron");
            Tooltip.SetDefault("The mutant terror of the sea was once the trusted companion of an old king; he tamed it using its favorite treat.\n" +
                "Long ago, the creature flew in desperation from the raging bloody inferno consuming its home, ultimately finding its way to the ocean.");
            SacrificeTotal = 1;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Yellow;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ItemID.DukeFishronTrophy).AddIngredient(ModContent.ItemType<VictoryShard>(), 10).Register();
        }
    }
}
