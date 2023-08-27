using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class HoneyDew : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statLifeMax2 += 30;
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.alwaysHoneyRegen = true;
            modPlayer.honeyTurboRegen = true;
            modPlayer.honeyDewHalveDebuffs = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BottledHoney, 10).
                AddIngredient(ItemID.BeeWax, 3).
                AddIngredient<MurkyPaste>(3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
