using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeQueenBee : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Queen Bee");
            Tooltip.SetDefault("As crude as the giant insects are they can prove useful in certain situations... given the ability to control them.");
            SacrificeTotal = 1;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Orange;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.QueenBeeTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
