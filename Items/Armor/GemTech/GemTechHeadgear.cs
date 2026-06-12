using System.Collections.Generic;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Armor.GemTech
{
    [AutoloadEquip(EquipType.Head)]
    public class GemTechHeadgear : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PostMoonLord";
        public const int GemBreakDamageLowerBound = 100;
        public const int GemDamage = 40000;
        public const int GemDamageSoftcapThreshold = 100000;
        public const int GemRegenTime = 1800;

        public const int MeleeShardBaseDamage = 825;
        public const int MeleeShardDelay = 330;
        public const float MeleeDamageBoost = 0.45f;
        public const float MeleeCritBoost = 0.12f;
        public const float MeleeSpeedBoost = 0.26f;

        public const int MaxFlechettes = 8;
        public static float RangedAmmoReduction = 0.7f;
        public const float RangedDamageBoost = 0.5f;
        public const float RangedCritBoost = 0.16f;

        public const int MagicManaBoost = 100;
        public const int NonMagicItemManaRegenBoost = 8;
        public const float MagicDamageBoost = 0.5f;
        public const float MagicCritBoost = 0.16f;

        public const int SummonMinionCountBoost = 4;
        public const float SummonDamageBoost = 0.72f;

        public const int RogueStealthBoost = 130;
        public const float RogueDamageBoost = 0.5f;
        public const float RogueCritBoost = 0.16f;

        public const int BaseGemDefenseBoost = 75;
        public const int BaseGemLifeRegenBoost = 2;
        public const float BaseGemDRBoost = 0.06f;
        public const float BaseGemMovementSpeedBoost = 0.4f;
        public const float BaseGemJumpSpeedBoost = 0.4f;

        public const int AllGemsWeaponUseLifeRegenBoost = 2;
        public const int AllGemsMultiWeaponUseLifeRegenBoost = 3;
        public const int AllGemsLifeRegenBoostTime = 480;
        public const int AllGemsMultiWeaponLifeRegenBoostTime = 150;

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 32;
            Item.defense = 14;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = RarityType<ExoticRainbow>();
            Item.Calamity().donorItem = true;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ItemType<GemTechBodyArmor>() && legs.type == ItemType<GemTechSchynbaulds>();

        public static bool HasArmorSet(Player player) => player.armor[0].type == ItemType<GemTechHeadgear>() && player.armor[1].type == ItemType<GemTechBodyArmor>() && player.armor[2].type == ItemType<GemTechSchynbaulds>();

        public override void UpdateArmorSet(Player player)
        {
            player.Calamity().GemTechSet = true;
            player.Calamity().wearingRogueArmor = true;
            if (player.Calamity().GemTechState.IsRedGemActive)
                player.Calamity().rogueStealthMax += RogueStealthBoost * 0.01f;
            if (player.Calamity().GemTechState.IsYellowGemActive)
                player.GetAttackSpeed<MeleeDamageClass>() += MeleeSpeedBoost;

            Color AbilityBriefColor = Color.Lerp(Color.White, Main.DiscoColor, 0.3f);
            player.setBonus = this.GetLocalization("AbilityBrief").Format(AbilityBriefColor.Hex3()); // Added below
        }

        public static void ModifySetTooltips(ModItem item, List<TooltipLine> tooltips)
        {
            if (HasArmorSet(Main.LocalPlayer))
            {
                int setBonusIndex = tooltips.FindIndex(x => x.Name == "SetBonus" && x.Mod == "Terraria");

                if (setBonusIndex != -1)
                {
                    if (!Main.keyState.PressingShift())
                    {
                        setBonusIndex++;
                        TooltipLine briefDescription = new TooltipLine(item.Mod, "CalamityMod:SetBonus1", CalamityUtils.GetTextValueFromModItem<GemTechHeadgear>("AbilityDescription"));
                        briefDescription.OverrideColor = Color.Lerp(Color.White, Main.DiscoColor, 0.5f);
                        tooltips.Insert(setBonusIndex, briefDescription);

                        setBonusIndex++;
                        TooltipLine holdShiftIndicator = new TooltipLine(item.Mod, IHoldShiftTooltipItem.ExtensionIndicatorTooltipID, CalamityUtils.GetTextValue("Misc.ShiftToExpand"));
                        holdShiftIndicator.OverrideColor = IHoldShiftTooltipItem.DefaultExtensionIndicatorColor;
                        tooltips.Insert(setBonusIndex, holdShiftIndicator);
                    }
                    else
                    {
                        setBonusIndex++;
                        TooltipLine largerDescription = new TooltipLine(item.Mod, "CalamityMod:SetBonus1", CalamityUtils.GetTextFromModItem<GemTechHeadgear>("GeneralGemInfo").Format(GemBreakDamageLowerBound, GemDamage, GemRegenTime.FramesToSeconds()));
                        largerDescription.OverrideColor = Color.Lerp(Color.White, Main.DiscoColor, 0.5f);
                        tooltips.Insert(setBonusIndex, largerDescription);

                        setBonusIndex++;
                        TooltipLine redGemTooltip = new TooltipLine(item.Mod, "CalamityMod:SetBonus2", CalamityUtils.GetTextFromModItem<GemTechHeadgear>("RedGemInfo").Format(RogueStealthBoost));
                        redGemTooltip.OverrideColor = new Color(224, 24, 0);
                        tooltips.Insert(setBonusIndex, redGemTooltip);

                        setBonusIndex++;
                        TooltipLine yellowGemTooltip = new TooltipLine(item.Mod, "CalamityMod:SetBonus3", CalamityUtils.GetTextValueFromModItem<GemTechHeadgear>("YellowGemInfo"));
                        yellowGemTooltip.OverrideColor = new Color(237, 170, 43);
                        tooltips.Insert(setBonusIndex, yellowGemTooltip);

                        setBonusIndex++;
                        TooltipLine greenGemTooltip = new TooltipLine(item.Mod, "CalamityMod:SetBonus4", CalamityUtils.GetTextValueFromModItem<GemTechHeadgear>("GreenGemInfo"));
                        greenGemTooltip.OverrideColor = new Color(37, 188, 108);
                        tooltips.Insert(setBonusIndex, greenGemTooltip);

                        setBonusIndex++;
                        TooltipLine blueGemTooltip = new TooltipLine(item.Mod, "CalamityMod:SetBonus5", CalamityUtils.GetTextFromModItem<GemTechHeadgear>("BlueGemInfo").Format(SummonMinionCountBoost));
                        blueGemTooltip.OverrideColor = new Color(37, 119, 206);
                        tooltips.Insert(setBonusIndex, blueGemTooltip);

                        setBonusIndex++;
                        TooltipLine purpleGemTooltip = new TooltipLine(item.Mod, "CalamityMod:SetBonus6", CalamityUtils.GetTextFromModItem<GemTechHeadgear>("PurpleGemInfo").Format(MagicManaBoost));
                        purpleGemTooltip.OverrideColor = new Color(200, 58, 209);
                        tooltips.Insert(setBonusIndex, purpleGemTooltip);

                        setBonusIndex++;
                        TooltipLine pinkGemTooltip = new TooltipLine(item.Mod, "CalamityMod:SetBonus7", CalamityUtils.GetTextFromModItem<GemTechHeadgear>("PinkGemInfo").Format(BaseGemDefenseBoost, BaseGemLifeRegenBoost.ToRegenPerSecond()));
                        pinkGemTooltip.OverrideColor = new Color(255, 115, 206);
                        tooltips.Insert(setBonusIndex, pinkGemTooltip);

                        setBonusIndex++;
                        TooltipLine liferegenTooltip = new TooltipLine(item.Mod, "CalamityMod:SetBonus8", CalamityUtils.GetTextFromModItem<GemTechHeadgear>("GemBonusInfo").Format(AllGemsWeaponUseLifeRegenBoost.ToRegenPerSecond(), AllGemsLifeRegenBoostTime.FramesToSeconds(), AllGemsMultiWeaponUseLifeRegenBoost.ToRegenPerSecond(), AllGemsMultiWeaponLifeRegenBoostTime.FramesToSeconds()));
                        liferegenTooltip.OverrideColor = new Color(230, 230, 230);
                        tooltips.Insert(setBonusIndex, liferegenTooltip);
                    }
                }
            }
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips) => ModifySetTooltips(this, tooltips);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ExoPrism>(10).
                AddIngredient(ItemID.FragmentSolar, 5).
                AddIngredient(ItemID.FragmentVortex, 5).
                AddIngredient(ItemID.FragmentNebula, 5).
                AddIngredient(ItemID.FragmentStardust, 5).
                AddIngredient<MeldBlob>(5).
                AddIngredient<CoreofCalamity>(2).
                AddTile<DraedonsForge>().
                SortBeforeFirstRecipesOf(ModContent.ItemType<GemTechBodyArmor>()).
                Register();
        }
    }
}
