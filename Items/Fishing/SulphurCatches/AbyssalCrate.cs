using CalamityMod.Items.Materials;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Tiles.Abyss;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.SulphurCatches
{
	public class AbyssalCrate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abyssal Crate");
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
            item.createTile = ModContent.TileType<AbyssalCrateTile>();
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
            DropHelper.DropItem(player, ModContent.ItemType<Items.Placeables.SulphurousSand>(), 5, 10);
            DropHelper.DropItem(player, ModContent.ItemType<Items.Placeables.SulphurousSandstone>(), 5, 10);
            DropHelper.DropItem(player, ModContent.ItemType<Acidwood>(), 5, 10);
            DropHelper.DropItemChance(player, ItemID.Starfish, 0.5f, 2, 3);
            DropHelper.DropItemChance(player, ItemID.Seashell, 0.5f, 2, 3);
            DropHelper.DropItemChance(player, ItemID.Coral, 0.5f, 2, 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<CloakingGland>(), 0.5f, 2, 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<VictoryShard>(), 0.5f, 2, 3);
			if (CalamityWorld.downedEoCAcidRain)
			{
				DropHelper.DropItemChance(player, ModContent.ItemType<SulfuricScale>(), 0.5f, 5, 10);
			}
			if (CalamityWorld.downedAquaticScourgeAcidRain)
			{
				DropHelper.DropItemChance(player, ModContent.ItemType<CorrodedFossil>(), 0.5f, 5, 10);
			}
            if (CalamityWorld.downedCalamitas)
            {
                DropHelper.DropItemChance(player, ModContent.ItemType<DepthCells>(), 0.5f, 5, 10);
                DropHelper.DropItemChance(player, ModContent.ItemType<Lumenite>(), 0.5f, 5, 10);
                DropHelper.DropItemChance(player, ModContent.ItemType<Items.Placeables.PlantyMush>(), 0.5f, 5, 10);
                DropHelper.DropItemChance(player, ModContent.ItemType<Items.Placeables.Tenebris>(), 0.5f, 5, 10);
            }
            if (NPC.downedGolemBoss)
            {
                DropHelper.DropItemChance(player, ModContent.ItemType<CruptixBar>(), 0.25f, 5, 10);
            }
            if (CalamityWorld.downedPolterghast)
            {
                DropHelper.DropItemChance(player, ModContent.ItemType<ReaperTooth>(), 0.25f, 5, 10);
            }

            // Weapons
            DropHelper.DropItemFromSetCondition(player, NPC.downedBoss3, 0.2f,
                ModContent.ItemType<Archerfish>(),
                ModContent.ItemType<BallOFugu>(),
                ModContent.ItemType<HerringStaff>(),
                ModContent.ItemType<Lionfish>(),
                ModContent.ItemType<BlackAnurian>());

            DropHelper.DropItemFromSetCondition(player, CalamityWorld.downedAquaticScourgeAcidRain, 0.2f,
                ModContent.ItemType<SkyfinBombers>(),
                ModContent.ItemType<NuclearRod>(),
                ModContent.ItemType<SulphurousGrabber>(),
                ModContent.ItemType<FlakToxicannon>(),
                ModContent.ItemType<SpentFuelContainer>(),
                ModContent.ItemType<SlitheringEels>(),
                ModContent.ItemType<BelchingSaxophone>());

            // Equipment
            DropHelper.DropItemFromSetCondition(player, NPC.downedBoss3, 0.4f,
                ModContent.ItemType<StrangeOrb>(),
                ModContent.ItemType<DepthCharm>(),
                ModContent.ItemType<IronBoots>(),
                ModContent.ItemType<AnechoicPlating>());

            //Bait
            DropHelper.DropItemChance(player, ItemID.MasterBait, 10, 1, 2);
            DropHelper.DropItemChance(player, ItemID.JourneymanBait, 5, 1, 3);
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
            DropHelper.DropItemChance(player, ModContent.ItemType<AnechoicCoating>(), 10, 1, 3);
            if (Main.hardMode)
            {
                DropHelper.DropItem(player, Main.rand.Next(100) >= 49 ? ItemID.GreaterHealingPotion : ItemID.GreaterManaPotion, 5, 10);
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
