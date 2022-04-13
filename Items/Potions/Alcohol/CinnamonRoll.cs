using CalamityMod.Buffs.Alcohol;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

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
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.rare = ItemRarityID.Yellow;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<CinnamonRollBuff>();
            Item.buffTime = CalamityUtils.SecondsToFrames(480f);
            Item.value = Item.buyPrice(0, 5, 30, 0);
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }
    }
}
