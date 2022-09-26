using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class RaidersTalisman : ModItem
    {
        public const float RaiderBonus = 12f;
        public const int RaiderCooldown = 5;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Raider's Talisman");
            Tooltip.SetDefault($"Landing a stealth strike grants a {(int)RaiderBonus}% crit bonus to non-stealth strikes\n" +
                $"This crit bonus decays over {RaiderCooldown} seconds");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 36;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.raiderTalisman = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Leather, 5).
                AddIngredient(ItemID.Obsidian, 20).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
