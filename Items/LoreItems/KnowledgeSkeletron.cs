using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeSkeletron : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Skeletron");
			Tooltip.SetDefault("The curse is said to only affect the elderly.\n" +
                "After they are afflicted they become an immortal vessel for an ancient demon of the underworld.\n" +
				"Place in your inventory to gain increased defense and damage while in the dungeon.");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 3;
			item.consumable = false;
		}

		public override bool CanUseItem(Player player)
		{
			return false;
		}

		public override void UpdateInventory(Player player)
		{
			if (player.ZoneDungeon)
			{
				CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
				modPlayer.skeletronLore = true;
			}
		}
	}
}
