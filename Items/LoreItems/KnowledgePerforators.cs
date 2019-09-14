using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgePerforators : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Perforators and their Hive");
			Tooltip.SetDefault("An abomination of comingled flesh, bone, and organ, infested primarily by blood-slurping worms.\n" +
                "The chunks left over from the brain must have been absorbed by the crimson and reconstituted into it.\n" +
				"Place in your inventory for all of your projectiles to inflict ichor when in the crimson.");
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
			if (player.ZoneCrimson)
			{
				CalamityPlayer modPlayer = player.GetCalamityPlayer();
				modPlayer.perforatorLore = true;
			}
		}
	}
}
