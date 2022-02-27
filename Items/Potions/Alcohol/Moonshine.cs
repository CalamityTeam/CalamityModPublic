using CalamityMod.Buffs.Alcohol;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class Moonshine : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Moonshine");
            Tooltip.SetDefault(@"This stuff is pretty strong but I'm sure you can handle it
Increases defense by 10 and damage reduction by 5%
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
            item.buffType = ModContent.BuffType<MoonshineBuff>();
            item.buffTime = CalamityUtils.SecondsToFrames(480f);
            item.value = Item.buyPrice(0, 1, 30, 0);
        }
    }
}
