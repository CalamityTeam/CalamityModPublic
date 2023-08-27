using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class AmbrosialAmpoule : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.statLifeMax2 += 70;

            // Grant life regen based on missing health
            if (!(modPlayer.rOoze || modPlayer.purity))
            {
                float missingLifeRatio = (player.statLifeMax2 - player.statLife) / player.statLifeMax2;
                float lifeRegenToGive = MathHelper.Lerp(4f, 12f, missingLifeRatio);
                player.lifeRegen += (int)lifeRegenToGive;
            }

            // bool left in for abyss light purposes
            modPlayer.aAmpoule = true;

            // Inherits all effects of Honey Dew and Living Dew
            modPlayer.alwaysHoneyRegen = true;
            modPlayer.honeyTurboRegen = true;
            modPlayer.honeyDewHalveDebuffs = true;
            modPlayer.livingDewHalveDebuffs = true;

            // Add light if the other accessories aren't equipped and visibility is turned on
            if (!(modPlayer.rOoze || modPlayer.purity) && !hideVisual)
                Lighting.AddLight(player.Center, new Vector3(1.2f, 1.7f, 2.7f));
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<LivingDew>().
                AddIngredient<RadiantOoze>().
                AddIngredient<LifeAlloy>(3).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
