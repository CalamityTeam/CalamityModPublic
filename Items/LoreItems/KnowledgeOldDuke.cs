using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeOldDuke : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Old Duke");
            Tooltip.SetDefault("Strange, to find out that the mutant terror of the seas was not alone in its unique biology.\n" +
                "Perhaps I was mistaken to classify the creature from its relation to pigrons alone.");
            SacrificeTotal = 1;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ModContent.ItemType<OldDukeTrophy>()).AddIngredient(ModContent.ItemType<PearlShard>(), 10).Register();
        }
    }
}
