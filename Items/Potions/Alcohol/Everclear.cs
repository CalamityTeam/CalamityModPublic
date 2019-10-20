using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class Everclear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Everclear");
            Tooltip.SetDefault(@"Boosts damage by 25%
Reduces life regen by 10 and defense by 40
This is the most potent booze I have, be careful with it");
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
            item.buffType = ModContent.BuffType<Buffs.Alcohol.Everclear>();
            item.buffTime = 900; //15 seconds
            item.value = Item.buyPrice(0, 6, 60, 0);
        }
    }
}
