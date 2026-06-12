using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Crags;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("Gehenna")]
    public class SlagsplitterPauldron : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public static int PauldronSlamDamage => 250;
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 34;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.sPauldron = true;
            modPlayer.sPauldronVisual = !hideVisual;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
            AddIngredient<ScorchedBone>(12).
            AddIngredient<AncientBoneDust>(4).
            AddIngredient<EssenceofHavoc>(8).
            AddTile(TileID.Anvils).
            Register();
        }
    }
}
