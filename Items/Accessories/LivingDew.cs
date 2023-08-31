using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class LivingDew : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 22;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statLifeMax2 += 50;

            // Inherits all effects of Honey Dew
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.alwaysHoneyRegen = true;
            modPlayer.honeyTurboRegen = true;
            modPlayer.honeyDewHalveDebuffs = true;
            modPlayer.livingDewHalveDebuffs = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<HoneyDew>().
                AddIngredient<TrapperBulb>(3).
                AddIngredient<LivingShard>(6).
                // TODO -- Replace with Water Essence
                AddIngredient<EssenceofSunlight>(5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
