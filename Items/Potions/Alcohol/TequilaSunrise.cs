using CalamityMod.Buffs.Alcohol;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class TequilaSunrise : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tequila Sunrise");
            Tooltip.SetDefault(@"The greatest daytime drink I've ever had
Boosts damage, damage reduction, and knockback by 7%, crit chance by 3%, and defense by 10 during daytime
Reduces life regen by 1");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = ItemRarityID.Yellow;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.buffType = ModContent.BuffType<TequilaSunriseBuff>();
            item.buffTime = CalamityUtils.SecondsToFrames(480f);
            item.value = Item.buyPrice(0, 6, 60, 0);
        }
    }
}
