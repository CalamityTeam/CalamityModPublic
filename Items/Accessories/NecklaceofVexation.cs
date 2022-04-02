using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class NecklaceofVexation : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Necklace of Vexation");
            Tooltip.SetDefault("Revenge\n" +
            "20% increased damage when under 50% life\n" +
            "All attacks inflict Cursed Inferno and Venom while wearing Reaver armor");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 34;
            item.value = CalamityGlobalItem.Rarity7BuyPrice;
            item.rare = ItemRarityID.Lime;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.vexation = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.AvengerEmblem);
            recipe.AddIngredient(ModContent.ItemType<DraedonBar>(), 2);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
