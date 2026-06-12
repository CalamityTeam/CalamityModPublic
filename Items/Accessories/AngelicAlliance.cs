using System.Collections.Generic;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    //Developer item, dedicatee: Nincity
    public class AngelicAlliance : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public static readonly SoundStyle ActivationSound = new("CalamityMod/Sounds/Custom/AbilitySounds/AngelicAllianceActivation");

        // This accessory is insane. wtf. - Iris
        public static int MinionSlotBoost = 2;
        public static float TotalSummonDamageBoost = 0.15f; // this is partially all-class -- this number represents the whole thing
        public static float DamageBoost = 0.08f;
        public static int RegenBoostDuringFlight = 4;
        public static int DivineBlessDuration = CalamityUtils.SecondsToFrames(15);
        public static int DivineBlessCooldown = CalamityUtils.SecondsToFrames(60);
        public static int HealPerAngelSpawned = 2;
        public static int BanishingFireDuration = CalamityUtils.SecondsToFrames(1);
        public static int DivineBlessFramesPerHeal = 15;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MinionSlotBoost, TotalSummonDamageBoost.ToPercent(), DamageBoost.ToPercent(), RegenBoostDuringFlight.ToRegenPerSecond(), DivineBlessDuration.FramesToSeconds(), HealPerAngelSpawned, (60 / (float)DivineBlessFramesPerHeal).Round(), (DivineBlessCooldown / 3600f).Round());

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
            player.GetDamage<GenericDamageClass>() += DamageBoost;
            player.GetDamage<SummonDamageClass>() += TotalSummonDamageBoost - DamageBoost;
            player.maxMinions += MinionSlotBoost;
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
