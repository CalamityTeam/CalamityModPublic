using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;

namespace CalamityMod.Items.Accessories
{
    public class Baroclaw : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 44;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.baroclaw = true;
            player.thorns += 2.5f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CrawCarapace>().
                AddIngredient<DepthCells>(12).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
