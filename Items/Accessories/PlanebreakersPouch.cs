using System.Collections.Generic;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("ElementalQuiver")]
    public class PlanebreakersPouch : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<CosmicPurple>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<RangedDamageClass>() += 0.15f;
            player.GetCritChance<RangedDamageClass>() += 7;
            player.magicQuiver = true;
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.deadshotBrooch = true;
            modPlayer.ammoCost *= 0.8f;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => tooltips.IntegrateHotkey(CalamityKeybinds.AmmoCycleHotkey);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyQuiver").
                AddIngredient<DeadshotBrooch>().
                AddIngredient<AscendantSpiritEssence>(4).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
