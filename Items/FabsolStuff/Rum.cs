using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.FabsolStuff
{
    public class Rum : ModItem
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rum");
			Tooltip.SetDefault(@"Boosts life regen by 2 and movement speed by 10%
Reduces defense by 8
Sweet and potent, just how I like it");
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
            item.buffType = mod.BuffType("Rum");
            item.buffTime = 18000; //5 minutes
            item.value = Item.buyPrice(0, 5, 0, 0);
		}
    }
}
