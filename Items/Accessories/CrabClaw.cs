using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;

namespace CalamityMod.Items.Accessories
{
    public class CrabClaw : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.crabClaw = true;
            player.thorns += 1.1f;
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
