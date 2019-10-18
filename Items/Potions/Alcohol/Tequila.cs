using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class Tequila : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tequila");
            Tooltip.SetDefault(@"Boosts damage, damage reduction, and knockback by 3%, crit chance by 2%, and defense by 5 during daytime
Reduces life regen by 1
Great for mixing up daytime drinks");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = 2;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = 2;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.buffType = ModContent.BuffType<Buffs.Tequila>();
            item.buffTime = 18000; //5 minutes
            item.value = Item.buyPrice(0, 5, 0, 0);
        }
    }
}
