using CalamityMod.Buffs.Potions;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.BrimstoneCragCatches
{
    public class Bloodfin : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Fishing";

        public static int FramesForExtraRegen = 15;

        public static int BuffType = ModContent.BuffType<BloodfinBoost>();
        public static int BuffDuration = 10;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(FramesForExtraRegen.FramesToSeconds(), BuffDuration);

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 30;
            ItemID.Sets.CanBePlacedOnWeaponRacks[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToHealingPotion(38, 36, 240);
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override void OnConsumeItem(Player player)
        {
            player.AddBuff(BuffType, CalamityUtils.SecondsToFrames(BuffDuration));
        }
    }
}
