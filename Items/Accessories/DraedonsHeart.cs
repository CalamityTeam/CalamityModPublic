using System.Collections.Generic;
using CalamityMod.Balancing;
using CalamityMod.CalPlayer;
using CalamityMod.Rarities;
using CalamityMod.World;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class DraedonsHeart : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        private const double ContactDamageReduction = 0.15D;

        // Duration of Nanomachines in frames.
        internal static readonly int NanomachinesDuration = 120;
        // Health gained every other frame while using Nanomachines.
        internal static readonly int NanomachinesHealPerFrame = 3;

        // Duration of time where Nanomachines won't accumulate after taking damage, in frames.
        internal static readonly int NanomachinePauseAfterDamage = 60;
        // Same as above, but for hits that are fully absorbed by a shield.
        internal static readonly int NanomachinePauseAfterShieldDamage = 30;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(
            (ContactDamageReduction * 100).ToString("N0"),
            NanomachinesHealPerFrame * (NanomachinesDuration / 2),
            NanomachinesDuration / 60);

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 11));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 68;
            Item.accessory = true;
            Item.defense = 20;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<ExoticRainbow>();
            Item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();

            // On the first frame of equipping Draedon's Heart, lose all adrenaline.
            // This occurs because you didn't have nanomachines LAST frame.
            if (!modPlayer.hadNanomachinesLastFrame)
                modPlayer.adrenaline = 0f;

            modPlayer.draedonsHeart = true;
            player.noKnockback = true;
            modPlayer.hadNanomachinesLastFrame = true;
            modPlayer.AdrenalineDuration = NanomachinesDuration;
            modPlayer.contactDamageReduction += ContactDamageReduction;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            // Add the hotkey
            list.IntegrateHotkey(CalamityKeybinds.AdrenalineHotKey);

            // Add the proper description which changes depending on world difficulty
            string desc = this.GetLocalization(CalamityWorld.revenge ? "NanomachinesReplace" : "NanomachinesAdd").Format(NanomachinePauseAfterDamage / 60);
            list.FindAndReplace("[NANODESC]", desc);

            // Add the proper DR value
            string fullAdrenDRString = (100f * BalancingConstants.FullAdrenalineDR).ToString("N0");
            list.FindAndReplace("[DR]", fullAdrenDRString);
        }
    }
}
