using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Items.Placeables.Plates;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("GodlySoulArtifact", "AuricSoulArtifact")]
    public class FadedIdolatry : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.fadedIdolatry = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<YharonSoulFragment>(5).
                AddIngredient<Plagueplate>(25).
                AddIngredient<ExodiumCluster>(25).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
