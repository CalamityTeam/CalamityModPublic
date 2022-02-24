using CalamityMod.Buffs.Alcohol;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class CaribbeanRum : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Caribbean Rum");
            Tooltip.SetDefault(@"Why is the rum gone?
Boosts life regen by 2, movement speed by 10% and wing flight time by 20%
Makes you floaty and reduces defense by 10%");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = ItemRarityID.Lime;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.buffType = ModContent.BuffType<CaribbeanRumBuff>();
            item.buffTime = CalamityUtils.SecondsToFrames(480f);
            item.value = Item.buyPrice(0, 5, 30, 0);
        }
    }
}
