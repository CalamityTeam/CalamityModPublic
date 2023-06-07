using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public abstract class LoreItem : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Lore";

        // All lore items initially have a short tooltip which indicates there is more to be read.
        public override LocalizedText Tooltip => CalamityUtils.GetText($"{LocalizationCategory}.ShortTooltip");

        // By default, lore text appears in white, but this can be changed.
        public virtual Color? LoreColor => null;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override bool CanUseItem(Player player) => false;

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = (ContentSamples.CreativeHelper.ItemGroup)CalamityResearchSorting.LoreItems;
        }

        // All lore items use the same code for holding SHIFT to extend tooltips.
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine fullLore = new(Mod, "CalamityMod:Lore", this.GetLocalizedValue("Lore"));
            if (LoreColor.HasValue)
                fullLore.OverrideColor = LoreColor.Value;
            CalamityUtils.HoldShiftTooltip(tooltips, new TooltipLine[] { fullLore }, true);
        }
    }
}
