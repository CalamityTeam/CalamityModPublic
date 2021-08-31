using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeBumblebirb : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Dragonfolly");
            Tooltip.SetDefault("A failure of twisted scientific ambition; it appears our faulted arrogance over life has shown once more in the results.\n" +
                "Originally intended to be a clone of the Jungle Dragon, these were left to roam about the jungle, attacking anything in their path.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Red;
            item.consumable = false;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddTile(TileID.Bookcases);
            r.AddIngredient(ModContent.ItemType<BumblebirbTrophy>());
            r.AddIngredient(ModContent.ItemType<VictoryShard>(), 10);
            r.AddRecipe();
        }
    }
}
