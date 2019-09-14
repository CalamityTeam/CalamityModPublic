using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeWallofFlesh : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Wall of Flesh");
			Tooltip.SetDefault("I see the deed is done.\n" +
                "The unholy amalgamation of flesh and hatred has been defeated.\n" +
                "Prepare to face the terrors that lurk in the light and dark parts of this world.\n" +
				"Place in your inventory to gain increased item grab range.");
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
			CalamityPlayer modPlayer = player.GetCalamityPlayer();
			modPlayer.wallOfFleshLore = true;
		}
	}
}
