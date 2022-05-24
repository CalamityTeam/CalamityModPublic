using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeMechs : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Mechanical Trio");
            Tooltip.SetDefault("I see you have awakened Draedon's old toys.\n" +
                "Once useful tools turned into savage beasts when their AIs went rogue, a mistake that Draedon failed to rectify in time.");
            SacrificeTotal = 1;
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
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ItemID.RetinazerTrophy).AddIngredient(ModContent.ItemType<VictoryShard>(), 10).Register();
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ItemID.SpazmatismTrophy).AddIngredient(ModContent.ItemType<VictoryShard>(), 10).Register();
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ItemID.DestroyerTrophy).AddIngredient(ModContent.ItemType<VictoryShard>(), 10).Register();
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ItemID.SkeletronPrimeTrophy).AddIngredient(ModContent.ItemType<VictoryShard>(), 10).Register();
        }
    }
}
