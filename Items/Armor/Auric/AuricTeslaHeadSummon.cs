using System.Collections.Generic;
using CalamityMod.Items.Armor.Bloodflare;
using CalamityMod.Items.Armor.Silva;
using CalamityMod.Items.Armor.Tarragon;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using static CalamityMod.Items.Armor.Bloodflare.BloodflareHeadSummon;
using static CalamityMod.Items.Armor.Silva.SilvaArmor;

namespace CalamityMod.Items.Armor.Auric
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("AuricTeslaSpaceHelmet")]
    public class AuricTeslaHeadSummon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PostMoonLord";

        public static int MinionSlotBoost = 2;
        public static float SummonDamageBoost = 0.32f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MinionSlotBoost, SummonDamageBoost.ToPercent());

        // Set Bonus
        public static int SetBonusMinionSlotBoost = 4;
        public static float SetBonusSummonDamageBoost = 0.55f;
        public static int CrystalDamage = 500;

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.defense = 18; // 104
            Item.rare = ModContent.RarityType<BurnishedAuric>();
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<AuricTeslaBodyArmor>() && legs.type == ModContent.ItemType<AuricTeslaCuisses>();

        public override void ArmorSetShadows(Player player) => player.armorEffectDrawOutlines = true;

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = this.GetLocalization("SetBonus").Format(SetBonusMinionSlotBoost, SetBonusSummonDamageBoost.ToPercent());
            var modPlayer = player.Calamity();
            modPlayer.tarraSet = true;
            modPlayer.tarraSummon = true;
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareSummon = true;
            modPlayer.silvaSet = true;
            modPlayer.silvaSummon = true;
            modPlayer.auricSet = true;
            modPlayer.WearingPostMLSummonerSet = true;
            player.maxMinions += SetBonusMinionSlotBoost;
            player.GetDamage<SummonDamageClass>() += SetBonusSummonDamageBoost;
        }

        public override void UpdateEquip(Player player)
        {
            player.maxMinions += MinionSlotBoost;
            player.GetDamage<SummonDamageClass>() += SummonDamageBoost;
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
                        Color[] armorColors = { AuricTeslaBodyArmor.tooltipTarragonColor, AuricTeslaBodyArmor.tooltipBloodflareColor, AuricTeslaBodyArmor.tooltipSilvaColor };
                        var LocalizedText = this.GetLocalization($"SetBonus{setBonusTooltipNumber}");
                        line.Text = (setBonusTooltipNumber == 3 ? LocalizedText.Format(SetBonusRegenBoost.ToRegenPerSecond(), AccelerationBoost.ToPercent(), ReviveDuration.FramesToSeconds(), (ReviveCooldown / 60).FramesToSeconds())
                        : setBonusTooltipNumber == 2 ? LocalizedText.Format(DefenseBoostBelowHealthThreshold, DefenseBoostHealthThreshold.ToPercent())
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
                AddIngredient<SilvaHeadSummon>().
                AddIngredient<BloodflareHeadSummon>().
                AddIngredient<TarragonHeadSummon>().
                AddIngredient<AuricBar>(12).
                AddTile<CosmicAnvil>().
                SortBeforeFirstRecipesOf(ModContent.ItemType<AuricTeslaHeadRogue>()).
                Register();
        }
    }
}
