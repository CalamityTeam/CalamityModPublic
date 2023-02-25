using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public abstract class LoreItem : ModItem
    {
        // This line is the short, generic lore tooltip which indicates there is more to be read.
        // It can be overridden as desired for flavor.
        public virtual string ShortTooltip => "Whispers from on high dance in your ears...";
        public virtual Color ShortTooltipColor => new(227, 175, 64); // #E3AF40

        // This line is what tells the player to hold Shift. There is essentially no reason to change it
        public virtual string LeftShiftExpandTooltip => "Press \"Left Shift\" to listen closer";
        public virtual Color LeftShiftExpandColor => new(190, 190, 190); // #BEBEBE

        // This string contains the actual lore of the lore item
        public virtual string Lore => "";
        // By default, lore text appears in white, but this can be changed.
        public virtual Color? LoreColor => null;

        public override void SetStaticDefaults()
        {
            string basicLine = ShortTooltipColor.ColorMessage(ShortTooltip, true);
            string leftShift = LeftShiftExpandColor.ColorMessage(LeftShiftExpandTooltip, false);
            Tooltip.SetDefault(basicLine + leftShift);
            SacrificeTotal = 1;
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
            TooltipLine fullLore = new(Mod, "CalamityMod:Lore", Lore);
            if (LoreColor.HasValue)
                fullLore.OverrideColor = LoreColor.Value;
            CalamityUtils.HoldShiftTooltip(tooltips, new TooltipLine[] { fullLore }, true);
        }
    }
}
