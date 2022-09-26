using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeAstralInfection : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Infection");
            Tooltip.SetDefault("This twisted dreamscape, surrounded by unnatural pillars under a dark and hazy sky.\n" +
                "Natural law has been upturned. What will you make of it?");
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
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ModContent.ItemType<AstrumDeusTrophy>()).AddIngredient(ModContent.ItemType<PearlShard>(), 10).Register();
        }
    }
}
