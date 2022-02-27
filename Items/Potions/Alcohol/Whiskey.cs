using CalamityMod.Buffs.Alcohol;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class Whiskey : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Whiskey");
            Tooltip.SetDefault(@"The burning sensation makes it tastier
Boosts damage and knockback by 4% and critical strike chance by 2%
Reduces defense by 5%");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = ItemRarityID.LightRed;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.buffType = ModContent.BuffType<WhiskeyBuff>();
            item.buffTime = CalamityUtils.SecondsToFrames(480f);
            item.value = Item.buyPrice(0, 1, 30, 0);
        }
    }
}
