using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class EvergreenGin : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Evergreen Gin");
            Tooltip.SetDefault(@"Boosts nature-based weapon damage by 15% and damage reduction by 5%
Reduces life regen by 1
It tastes like a Christmas tree if you can imagine that");
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
            item.useStyle = 2;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.buffType = ModContent.BuffType<Buffs.Alcohol.EvergreenGin>();
            item.buffTime = 18000; //5 minutes
            item.value = Item.buyPrice(0, 16, 60, 0);
        }
    }
}
