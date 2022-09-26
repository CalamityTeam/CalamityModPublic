using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("ChaosAmulet")]
    public class SpelunkersAmulet : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Spelunker's Amulet");
            Tooltip.SetDefault("Spelunker effect and 15% increased mining speed");
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 32;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.pickSpeed -= 0.15f;
            player.findTreasure = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.GoldDust, 7).
                AddIngredient(ItemID.SpelunkerPotion, 7).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
