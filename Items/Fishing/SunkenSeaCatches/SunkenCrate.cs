using CalamityMod.Items.Materials;
using CalamityMod.Items.Critters;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Tiles.SunkenSea;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.SunkenSeaCatches
{
	public class SunkenCrate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sunken Crate");
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
            item.createTile = ModContent.TileType<SunkenCrateTile>();
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            //Modded materials
			DropHelper.DropItem(player, ModContent.ItemType<Items.Placeables.Navystone>(), 10, 30);
			DropHelper.DropItem(player, ModContent.ItemType<Items.Placeables.EutrophicSand>(), 10, 30);
            if (CalamityWorld.downedDesertScourge)
            {
				DropHelper.DropItem(player, ModContent.ItemType<PrismShard>(), 10, 20);
				DropHelper.DropItem(player, ModContent.ItemType<Items.Placeables.SeaPrism>(), 5, 10);
            }
            if (Main.hardMode)
            {
                DropHelper.DropItemChance(player, ModContent.ItemType<MolluskHusk>(), 0.5f, 5, 15);
            }

            // Weapons
            DropHelper.DropItemFromSetCondition(player, CalamityWorld.downedCLAMHardMode, 0.2f,
                ModContent.ItemType<ShellfishStaff>(),
                ModContent.ItemType<ClamCrusher>(),
                ModContent.ItemType<Poseidon>(),
                ModContent.ItemType<ClamorRifle>());

            //Bait
            DropHelper.DropItemChance(player, ItemID.MasterBait, 10, 1, 2);
            DropHelper.DropItemChance(player, ItemID.JourneymanBait, 5, 1, 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<SeaMinnowItem>(), 5, 1, 3);
            DropHelper.DropItemChance(player, ItemID.ApprenticeBait, 3, 2, 3);

            //Potions
            DropHelper.DropItemChance(player, ItemID.ObsidianSkinPotion, 10, 1, 3);
            DropHelper.DropItemChance(player, ItemID.SwiftnessPotion, 10, 1, 3);
            DropHelper.DropItemChance(player, ItemID.IronskinPotion, 10, 1, 3);
            DropHelper.DropItemChance(player, ItemID.NightOwlPotion, 10, 1, 3);
            DropHelper.DropItemChance(player, ItemID.ShinePotion, 10, 1, 3);
            DropHelper.DropItemChance(player, ItemID.MiningPotion, 10, 1, 3);
            DropHelper.DropItemChance(player, ItemID.HeartreachPotion, 10, 1, 3);
            DropHelper.DropItemChance(player, ItemID.TrapsightPotion, 10, 1, 3); //Dangersense Potion
            if (Main.hardMode)
            {
                DropHelper.DropItem(player, Main.rand.Next(100) >= 49 ? ItemID.GreaterHealingPotion: ItemID.GreaterManaPotion, 5, 10);
            }
            else
            {
                DropHelper.DropItem(player, Main.rand.Next(100) >= 49 ? ItemID.HealingPotion : ItemID.ManaPotion, 5, 10);
            }

            //Money
            DropHelper.DropItem(player, ItemID.SilverCoin, 10, 90);
            DropHelper.DropItemChance(player, ItemID.GoldCoin, 0.5f, 1, 5);
        }
    }
}
