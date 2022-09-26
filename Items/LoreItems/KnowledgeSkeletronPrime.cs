using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeSkeletronPrime : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skeletron Prime");
            Tooltip.SetDefault("What a silly and pointless contraption for something created with the essence of pure terror.\n" +
                "Draedon obviously took several liberties with its design... I am not impressed.");
            SacrificeTotal = 1;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Pink;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ItemID.SkeletronPrimeTrophy).AddIngredient(ModContent.ItemType<PearlShard>(), 10).Register();
        }
    }
}
