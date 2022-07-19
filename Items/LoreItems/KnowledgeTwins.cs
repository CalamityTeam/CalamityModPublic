using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeTwins : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Twins");
            Tooltip.SetDefault("The bio-mechanical watchers of the night, originally created as security using the souls extracted from human eyes.\n" +
                "These creatures did not belong in this world, it's best to be rid of them.");
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
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ItemID.RetinazerTrophy).AddIngredient(ModContent.ItemType<PearlShard>(), 10).Register();
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ItemID.SpazmatismTrophy).AddIngredient(ModContent.ItemType<PearlShard>(), 10).Register();
        }
    }
}
