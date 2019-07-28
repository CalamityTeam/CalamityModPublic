using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class Knowledge24 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Calamitas Doppelganger");
			Tooltip.SetDefault("You are indeed stronger than I thought.\n" +
                "Though the bloody inferno still lingers, observing your progress.\n" +
				"Place in your inventory to gain a boost to your minion slots but at the cost of reduced max health.");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 5;
			item.consumable = false;
		}

		public override bool CanUseItem(Player player)
		{
			return false;
		}

		public override void UpdateInventory(Player player)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			modPlayer.calamitasLore = true;
		}
	}
}
