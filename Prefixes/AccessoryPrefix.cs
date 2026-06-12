using System.Collections.Generic;
using CalamityMod.Prefixes.VanillaPrefixChanges;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Prefixes
{
    public class Invigorating : ModPrefix, ILocalizedModType
    {
        public new string LocalizationCategory => "Prefixes.Accessory";
        public override PrefixCategory Category => PrefixCategory.Accessory;

        public static float GetPartialLifeRegenAmount = 0.5f;
        public override void ApplyAccessoryEffects(Player player)
        {
            player.Calamity().partialLifeRegen += GetPartialLifeRegenAmount;
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult = VanillaPrefixChange.RarityPlusOneButClosestToTierTwo;
        }

        public LocalizedText LifeRegenTooltip => CalamityUtils.GetText($"{LocalizationCategory}.LifeRegenTooltip");
        public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {
            yield return new TooltipLine(Mod, "CalamityMod:PrefixLifeRegenBoost", LifeRegenTooltip.Format(GetPartialLifeRegenAmount.ToRegenPerSecond()))
            {
                IsModifier = true
            };
        }
    }

    public class Dauntless : ModPrefix, ILocalizedModType
    {
        public new string LocalizationCategory => "Prefixes.Accessory";
        public override PrefixCategory Category => PrefixCategory.Accessory;

        public override void ApplyAccessoryEffects(Player player)
        {
            player.statLifeMax2 += GetHealthBoostAmount();
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult = VanillaPrefixChange.RarityPlusOneButClosestToTierTwo;
        }

        public LocalizedText LifeBoostTooltip => CalamityUtils.GetText($"{LocalizationCategory}.MaxLifeBoostTooltip");
        public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {
            yield return new TooltipLine(Mod, "CalamityMod:PrefixMaxLifeBoost", LifeBoostTooltip.Format(GetHealthBoostAmount()))
            {
                IsModifier = true
            };
        }

        public static int GetHealthBoostAmount()
        {
            return 20;
        }
    }

    public class Friendly : ModPrefix, ILocalizedModType
    {
        public new string LocalizationCategory => "Prefixes.Accessory";
        public override PrefixCategory Category => PrefixCategory.Accessory;

        public override void ApplyAccessoryEffects(Player player)
        {
            // give Minion
            player.Calamity().friendlyMinions++;
        }
        public override void ModifyValue(ref float valueMult)
        {
            valueMult = 2.5f;
        }
        public override float RollChance(Item item) => 0; // Is manually applied by an item, can't be rerolled normally
        public override bool CanRoll(Item item)
        {
            return true;
        }
        public LocalizedText FriendlyTooltip => CalamityUtils.GetText($"{LocalizationCategory}.FriendlyTooltip");
        public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {
            yield return new TooltipLine(Mod, "CalamityMod:PrefixFriendly", FriendlyTooltip.Format(1))
            {
                IsModifier = true
            };
        }
    }
}
