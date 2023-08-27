using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class RadiantOoze : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();

            // Grant life regen based on missing health
            if (!(modPlayer.aAmpoule || modPlayer.purity))
            {
                float missingLifeRatio = (player.statLifeMax2 - player.statLife) / player.statLifeMax2;
                float lifeRegenToGive = MathHelper.Lerp(4f, 12f, missingLifeRatio);
                player.lifeRegen += (int)lifeRegenToGive;
            }
            
            // bool left in for abyss light purposes
            modPlayer.rOoze = true;

            // Add light if the other accessories aren't equipped and visibility is turned on
            if (!(modPlayer.aAmpoule || modPlayer.purity) && !hideVisual)
                Lighting.AddLight(player.Center, new Vector3(1f, 1f, 0.6f));
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BlightedGel>(45).
                AddIngredient<PurifiedGel>(15).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
