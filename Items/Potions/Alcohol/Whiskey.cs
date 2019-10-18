using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class Whiskey : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Whiskey");
            Tooltip.SetDefault(@"Boosts damage and knockback by 4% and critical strike chance by 2%
Reduces defense by 8
The burning sensation makes it tastier");
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
            item.buffType = ModContent.BuffType<Buffs.Whiskey>();
            item.buffTime = 18000; //5 minutes
            item.value = Item.buyPrice(0, 3, 30, 0);
        }
    }
}
