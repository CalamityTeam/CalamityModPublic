using CalamityMod.Buffs.Alcohol;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class OddMushroom : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Odd Mushroom");
            Tooltip.SetDefault("Trippy");
        }

        public override void SetDefaults()
        {
            item.width = 42;
            item.height = 48;
            item.useTurn = true;
            item.maxStack = 30;
            item.useAnimation = 17;
            item.useTime = 17;
            item.rare = 3;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item2;
            item.consumable = true;
            item.buffType = ModContent.BuffType<Trippy>();
            item.buffTime = CalamityUtils.SecondsToFrames(3600f);
            item.value = Item.buyPrice(1, 0, 0, 0);
        }
    }
}
