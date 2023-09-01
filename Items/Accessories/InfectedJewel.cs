using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Potions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("CelestialJewel")]
    public class InfectedJewel : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.defense = 6;
            Item.width = 26;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.infectedJewel = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CrownJewel>().
                AddIngredient<AureusCell>(10).
                AddIngredient<Stardust>(25).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
