using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.Items.DifficultyItems
{
    public class MLGRune : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Demon Trophy");
			Tooltip.SetDefault("Boosts spawn rate by 1.25 times\n" +
			                   "Effects cannot be reversed");
		}

		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 28;
			item.maxStack = 99;
			item.rare = 1;
			item.useAnimation = 45;
			item.useTime = 45;
			item.useStyle = 4;
			item.UseSound = SoundID.Item119;
			item.consumable = true;
		}

		public override bool UseItem(Player player)
		{
			CalamityWorld.demonMode = true;
			CalamityMod.UpdateServerBoolean();
			return true;
		}
	}
}
