using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeWallofFlesh : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wall of Flesh");
            Tooltip.SetDefault("I see the deed is done.\n" +
                "The unholy amalgamation of flesh and hatred has been defeated.\n" +
                "Prepare to face the terrors that lurk in the light and dark parts of this world.");
            SacrificeTotal = 1;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.LightRed;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ItemID.WallofFleshTrophy).AddIngredient(ModContent.ItemType<PearlShard>(), 10).Register();
        }
    }
}
