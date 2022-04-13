using CalamityMod.Buffs.Alcohol;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

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
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.rare = ItemRarityID.Lime;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<EverclearBuff>();
            Item.buffTime = CalamityUtils.SecondsToFrames(60f);
            Item.value = Item.buyPrice(0, 2, 0, 0);
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }
    }
}
