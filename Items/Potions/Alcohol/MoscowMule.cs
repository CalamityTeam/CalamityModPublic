using CalamityMod.Buffs.Alcohol;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class MoscowMule : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Moscow Mule");
            Tooltip.SetDefault(@"I once heard the copper mug can be toxic and I told 'em 'listen dummy, I'm already poisoning myself'
Boosts damage and knockback by 9% and critical strike chance by 3%
Reduces life regen by 2");
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
            item.buffType = ModContent.BuffType<MoscowMuleBuff>();
            item.buffTime = CalamityUtils.SecondsToFrames(480f);
            item.value = Item.buyPrice(0, 5, 30, 0);
        }
    }
}
