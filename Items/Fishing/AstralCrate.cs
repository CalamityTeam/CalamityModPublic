using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.World;
using CalamityMod.Utilities;

namespace CalamityMod.Items.Fishing
{
    public class AstralCrate : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Astral Crate");
			Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
		}

		public override void SetDefaults()
		{
			item.maxStack = 999;
			item.consumable = true;
			item.width = 32;
			item.height = 32;
			item.rare = 2;
            item.value = Item.sellPrice(gold: 1);
			item.createTile = mod.TileType("AstralCrate");
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = 1;
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void RightClick(Player player)
		{
			//Modded materials
			DropHelper.DropItem(player, mod.ItemType("Stardust"), 10, 20);
			DropHelper.DropItem(player, ItemID.FallenStar, 10, 20);
			if (CalamityWorld.downedAstrageldon)
			{
				DropHelper.DropItemChance(player, mod.ItemType("AstralJelly"), 0.5f, 5, 10);
			}
			if (CalamityWorld.downedStarGod)
			{
				DropHelper.DropItemChance(player, mod.ItemType("AstralOre"), 0.5f, 10, 20);
				DropHelper.DropItemChance(player, mod.ItemType("AstralBar"), 0.25f, 5, 10);
			}

			// Weapons
			DropHelper.DropItemFromSetCondition(player, CalamityWorld.downedAstrageldon, 0.2f,
				mod.ItemType("StellarKnife"),
				mod.ItemType("AstralachneaStaff"),
				mod.ItemType("TitanArm"),
				mod.ItemType("HivePod"),
				mod.ItemType("AstralScythe"),
				mod.ItemType("StellarCannon"));

			//Bait
			DropHelper.DropItemChance(player, mod.ItemType("Twinkler"), 5, 1, 3);
			DropHelper.DropItemChance(player, ItemID.EnchantedNightcrawler, 5, 1, 3);
			DropHelper.DropItemChance(player, mod.ItemType("ArcturusAstroidean"), 5, 1, 3);
			DropHelper.DropItemChance(player, ItemID.Firefly, 3, 1, 3);

			//Potions
			DropHelper.DropItemChance(player, ItemID.ObsidianSkinPotion, 10, 1, 3);
			DropHelper.DropItemChance(player, ItemID.SwiftnessPotion, 10, 1, 3);
			DropHelper.DropItemChance(player, ItemID.IronskinPotion, 10, 1, 3);
			DropHelper.DropItemChance(player, ItemID.NightOwlPotion, 10, 1, 3);
			DropHelper.DropItemChance(player, ItemID.ShinePotion, 10, 1, 3);
			DropHelper.DropItemChance(player, ItemID.MiningPotion, 10, 1, 3);
			DropHelper.DropItemChance(player, ItemID.HeartreachPotion, 10, 1, 3);
			DropHelper.DropItemChance(player, ItemID.TrapsightPotion, 10, 1, 3); //Dangersense Potion
			if (CalamityWorld.downedStarGod)
			{
				DropHelper.DropItemChance(player, ItemID.SuperHealingPotion, 2, 5, 10);
			}
			else
			{
				DropHelper.DropItemChance(player, ItemID.GreaterHealingPotion, 2, 5, 10);
			}
			if (CalamityWorld.downedAstrageldon)
			{
				DropHelper.DropItemChance(player, mod.ItemType("AstralInjection"), 4, 1, 3);
				DropHelper.DropItemChance(player, mod.ItemType("GravityNormalizerPotion"), 4, 1, 3);
			}

			//Money
			DropHelper.DropItem(player, ItemID.SilverCoin, 10, 90);
			DropHelper.DropItemChance(player, ItemID.GoldCoin, 0.5f, 1, 5);
		}
	}
}
