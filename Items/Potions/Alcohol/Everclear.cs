using CalamityMod.Buffs.Alcohol;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class Everclear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Everclear");
            Tooltip.SetDefault(@"This is the most potent booze I have, be careful with it
Boosts damage by 25%
Reduces life regen by 10 and defense by 30%");
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
            item.buffType = ModContent.BuffType<EverclearBuff>();
            item.buffTime = CalamityUtils.SecondsToFrames(60f);
            item.value = Item.buyPrice(0, 2, 0, 0);
        }
    }
}
