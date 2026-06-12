using System.Collections.Generic;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Items.Armor.Bloodflare;
using CalamityMod.Items.Armor.GodSlayer;
using CalamityMod.Items.Armor.Tarragon;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using static CalamityMod.Items.Armor.Bloodflare.BloodflareHeadRanged;
using static CalamityMod.Items.Armor.GodSlayer.GodSlayerHeadRanged;

namespace CalamityMod.Items.Armor.Auric
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("AuricTeslaHoodedFacemask")]
    public class AuricTeslaHeadRanged : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PostMoonLord";

        public static float RangedDamageBoost = 0.22f;
        public static int RangedCritBoost = 20;
        public static float AmmoReduction = 0.7f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(RangedDamageBoost.ToPercent(), RangedCritBoost, (1f - AmmoReduction).ToPercent());

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.defense = 39; // 125
            Item.rare = ModContent.RarityType<BurnishedAuric>();
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<AuricTeslaBodyArmor>() && legs.type == ModContent.ItemType<AuricTeslaCuisses>();

        public override void ArmorSetShadows(Player player) => player.armorEffectDrawOutlines = true;

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = this.GetLocalizedValue("SetBonus");
            var modPlayer = player.Calamity();
            modPlayer.tarraSet = true;
            modPlayer.tarraRanged = true;
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareRanged = true;
            modPlayer.godSlayer = true;
            modPlayer.godSlayerRanged = true;
            modPlayer.auricSet = true;

            if (modPlayer.godSlayerDashHotKeyPressed || (player.dashDelay != 0 && modPlayer.LastUsedDashID == GodslayerArmorDash.ID))
                modPlayer.DeferredDashID = GodslayerArmorDash.ID;
        }

        public override void UpdateEquip(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.ammoCost *= AmmoReduction;
            player.GetDamage<RangedDamageClass>() += RangedDamageBoost;
            player.GetCritChance<RangedDamageClass>() += RangedCritBoost;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            ref var holdingShift = ref AuricTeslaBodyArmor.holdingShift;
            ref var setBonusTooltipNumber = ref AuricTeslaBodyArmor.setBonusTooltipNumber;
            if (Main.keyState.PressingShift())
            {
                if (!holdingShift)
                {
                    holdingShift = true;
                    setBonusTooltipNumber++;
                    if (setBonusTooltipNumber > 3) setBonusTooltipNumber = 1;
                }
                foreach (TooltipLine line in tooltips)
                {
                    if (line.Name == "SetBonus")
                    {
                        Color[] armorColors = { AuricTeslaBodyArmor.tooltipTarragonColor, AuricTeslaBodyArmor.tooltipBloodflareColor, AuricTeslaBodyArmor.tooltipGodslayerColor };
                        var LocalizedText = CalamityUtils.GetTextFromModItem(Type, $"SetBonus{setBonusTooltipNumber}");
                        line.Text = (setBonusTooltipNumber == 3 ? LocalizedText.Format(ShrapnelRoundCooldown.FramesToSeconds(), CalamityKeybinds.GodSlayerDashHotKey.TooltipHotkeyString(), GodSlayerChestplate.DashCooldown.FramesToSeconds())
                        : setBonusTooltipNumber == 2 ? LocalizedText.Format(CalamityUtils.GetArmorSetBonusKey(), SoulCooldown.FramesToSeconds(), BloodBombCooldown.FramesToSeconds())
                        : LocalizedText.Format());
                        line.OverrideColor = armorColors[setBonusTooltipNumber - 1];
                    }
                }
            }
            else
            {
                holdingShift = false;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<GodSlayerHeadRanged>().
                AddIngredient<BloodflareHeadRanged>().
                AddIngredient<TarragonHeadRanged>().
                AddIngredient<AuricBar>(12).
                AddTile<CosmicAnvil>().
                SortBeforeFirstRecipesOf(ModContent.ItemType<AuricTeslaHeadMagic>()).
                Register();
        }
    }
}
