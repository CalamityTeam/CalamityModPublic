using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.FabsolStuff
{
    public class Screwdriver : ModItem
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Screwdriver");
			Tooltip.SetDefault(@"Boosts piercing projectile damage by 10%
Reduces life regen by 1
Do you have a screw loose?");
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
            item.buffType = mod.BuffType("Screwdriver");
            item.buffTime = 18000; //5 minutes
            item.value = Item.buyPrice(0, 16, 60, 0);
		}
    }
}
