using CalamityMod.Buffs.Alcohol;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class EvergreenGin : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Evergreen Gin");
            Tooltip.SetDefault(@"It tastes like a Christmas tree if you can imagine that
Boosts nature-based weapon damage by 15% and damage reduction by 5%
Reduces life regen by 1");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = 4;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.buffType = ModContent.BuffType<EvergreenGinBuff>();
            item.buffTime = CalamityUtils.SecondsToFrames(480f);
            item.value = Item.buyPrice(0, 16, 60, 0);
        }
    }
}
