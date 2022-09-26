using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeCalamitasClone : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Calamitas Clone");
            Tooltip.SetDefault("You are indeed stronger than I thought.\n" +
                "Though the bloody inferno still lingers, observing your progress.");
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
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ModContent.ItemType<CalamitasTrophy>()).AddIngredient(ModContent.ItemType<PearlShard>(), 10).Register();
        }
    }
}
