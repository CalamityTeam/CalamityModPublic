using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class SilencingSheath : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Silencing Sheath");
            Tooltip.SetDefault("+20 maximum stealth\n" +
                "Stealth generates 15% faster");
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 34;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.rogueStealthMax += 0.2f;
            modPlayer.stealthGenStandstill += 0.15f;
            modPlayer.stealthGenMoving += 0.15f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyEvilBar", 8).
                AddIngredient(ItemID.Silk, 10).
                AddRecipeGroup("Boss2Material", 3).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
