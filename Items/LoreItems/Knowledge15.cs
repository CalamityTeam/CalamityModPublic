using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class Knowledge15 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Slime God");
			Tooltip.SetDefault("It is a travesty, one of the most threatening biological terrors ever created.\n" +
                "If this creature were allowed to combine every slime on the planet it would become nearly unstoppable.\n" +
				"Place in your inventory to become slimed and able to slide around on tiles quickly, at the cost of reduced defense.");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 4;
			item.consumable = false;
		}

		public override bool CanUseItem(Player player)
		{
			return false;
		}

		public override void UpdateInventory(Player player)
		{
			player.slippy2 = true;
			if (Main.myPlayer == player.whoAmI)
			{
				player.AddBuff(BuffID.Slimed, 2);
			}
			player.statDefense -= 10;
		}
	}
}
