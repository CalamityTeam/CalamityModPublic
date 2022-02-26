using CalamityMod.Buffs.Alcohol;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class CinnamonRoll : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cinnamon Roll");
            Tooltip.SetDefault(@"A great-tasting cinnamon whiskey with a touch of cream soda
Boosts mana regeneration rate and multiplies all fire-based debuff damage by 1.5
Reduces defense by 10%");
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
            item.buffType = ModContent.BuffType<CinnamonRollBuff>();
            item.buffTime = CalamityUtils.SecondsToFrames(480f);
            item.value = Item.buyPrice(0, 5, 30, 0);
        }
    }
}
