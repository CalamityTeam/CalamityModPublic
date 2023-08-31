using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    //Developer item, dedicatee: Nincity
    public class AngelicAlliance : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public static readonly SoundStyle ActivationSound = new("CalamityMod/Sounds/Custom/AbilitySounds/AngelicAllianceActivation");

        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 92;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(CalamityKeybinds.AngelicAllianceHotKey);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.angelicAlliance = true;
            player.GetDamage<GenericDamageClass>() += 0.08f;
            player.GetDamage<SummonDamageClass>() += 0.07f; //7% + 8% = 15%
            player.maxMinions += 2;
            if (player.wingTime < player.wingTimeMax)
                player.lifeRegen += 4;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyHallowedHelmet").
                AddRecipeGroup("AnyHallowedPlatemail").
                AddRecipeGroup("AnyHallowedGreaves").
                AddIngredient(ItemID.PaladinsShield).
                AddIngredient(ItemID.TrueExcalibur).
                AddIngredient(ItemID.CrossNecklace).
                AddIngredient<ShadowspecBar>(5).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
