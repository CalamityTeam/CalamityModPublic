using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("AstralArcanum")]
    public class Purity : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.defense = 8;
            Item.width = 18;
            Item.height = 44;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.statLifeMax2 += 100;

            // Grant life regen based on missing health
            if (!(modPlayer.rOoze || modPlayer.aAmpoule))
            {
                float missingLifeRatio = (player.statLifeMax2 - player.statLife) / player.statLifeMax2;
                float lifeRegenToGive = MathHelper.Lerp(6f, 14f, missingLifeRatio);
                player.lifeRegen += (int)lifeRegenToGive;
            }

            // Abyss light, debuff near-immunity, and massively enhances debuff halving
            modPlayer.purity = true;

            // Inherits effects from Honey Dew and Living Dew
            modPlayer.alwaysHoneyRegen = true;
            modPlayer.honeyTurboRegen = true;
            modPlayer.honeyDewHalveDebuffs = true;
            modPlayer.livingDewHalveDebuffs = true;

            // Add light if the other accessories aren't equipped and visibility is turned on
            if (!(modPlayer.rOoze || modPlayer.aAmpoule) && !hideVisual)
                Lighting.AddLight(player.Center, new Vector3(1.376f, 1.658f, 2.103f));
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AmbrosialAmpoule>().
                AddIngredient<InfectedJewel>().
                AddIngredient<AuricBar>(4).
                AddIngredient<AscendantSpiritEssence>(5).
                AddTile(ModContent.TileType<CosmicAnvil>()).
                Register();
        }
    }
}
