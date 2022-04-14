using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class RustyMedallion : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Rusty Medallion");
            Tooltip.SetDefault("Causes most ranged weapons to sometimes release acid droplets from the sky");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 32;
            Item.rare = ItemRarityID.Blue;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.rustyMedal = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SulfuricScale>(20).
                AddRecipeGroup("IronBar", 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
