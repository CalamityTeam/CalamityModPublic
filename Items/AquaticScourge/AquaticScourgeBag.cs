using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.AquaticScourge
{
	public class AquaticScourgeBag : ModItem
	{
        public override int BossBagNPC => mod.NPCType("AquaticScourgeHead");

        public override void SetStaticDefaults()
 		{
 			DisplayName.SetDefault("Treasure Bag");
 			Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
 		}
		
		public override void SetDefaults()
		{
			item.maxStack = 999;
			item.consumable = true;
			item.width = 24;
			item.height = 24;
			item.rare = 9;
			item.expert = true;
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void OpenBossBag(Player player)
		{
			player.TryGettingDevArmor();

            // Materials
            DropHelper.DropItem(player, ItemID.SoulofSight, 25, 40);
            DropHelper.DropItem(player, mod.ItemType("VictoryShard"), 15, 25);
            DropHelper.DropItem(player, ItemID.Coral, 7, 11);
            DropHelper.DropItem(player, ItemID.Seashell, 7, 11);
            DropHelper.DropItem(player, ItemID.Starfish, 7, 11);

            // Weapons
            DropHelper.DropItemChance(player, mod.ItemType("SubmarineShocker"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("Barinautical"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("Downpour"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("DeepseaStaff"), 3);

            // Equipment
            DropHelper.DropItem(player, mod.ItemType("AquaticEmblem"));
            DropHelper.DropItemChance(player, mod.ItemType("AeroStone"), 8);

            // Vanity
            // there is no Aquatic Scourge mask yet

            // Fishing
            DropHelper.DropItemChance(player, ItemID.HighTestFishingLine, 10);
            DropHelper.DropItemChance(player, ItemID.AnglerTackleBag, 10);
            DropHelper.DropItemChance(player, ItemID.TackleBox, 10);
            DropHelper.DropItemChance(player, ItemID.AnglerEarring, 8);
            DropHelper.DropItemChance(player, ItemID.FishermansGuide, 8);
            DropHelper.DropItemChance(player, ItemID.WeatherRadio, 8);
            DropHelper.DropItemChance(player, ItemID.Sextant, 8);
            DropHelper.DropItemChance(player, ItemID.AnglerHat, 3);
            DropHelper.DropItemChance(player, ItemID.AnglerVest, 3);
            DropHelper.DropItemChance(player, ItemID.AnglerPants, 3);
            DropHelper.DropItemChance(player, ItemID.FishingPotion, 3, 2, 3);
            DropHelper.DropItemChance(player, ItemID.SonarPotion, 3, 2, 3);
            DropHelper.DropItemChance(player, ItemID.CratePotion, 3, 2, 3);
            DropHelper.DropItemCondition(player, ItemID.GoldenBugNet, NPC.downedBoss3, 12, 1, 1);
        }
	}
}
