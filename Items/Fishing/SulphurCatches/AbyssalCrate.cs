using CalamityMod.Items.Materials;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Tools.ClimateChange;
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
            DropHelper.DropItemChance(player, ModContent.ItemType<VictoryShard>(), 0.25f, 2, 3);
			DropHelper.DropItemCondition(player, ModContent.ItemType<SulfuricScale>(), CalamityWorld.downedEoCAcidRain, 0.1f, 1, 3);
			DropHelper.DropItemCondition(player, ModContent.ItemType<CorrodedFossil>(), CalamityWorld.downedAquaticScourgeAcidRain, 0.1f, 1, 3);
			DropHelper.DropItemCondition(player, ModContent.ItemType<DepthCells>(), CalamityWorld.downedCalamitas, 0.2f, 2, 5);
			DropHelper.DropItemCondition(player, ModContent.ItemType<Lumenite>(), CalamityWorld.downedCalamitas, 0.2f, 2, 5);
			DropHelper.DropItemCondition(player, ModContent.ItemType<Items.Placeables.PlantyMush>(), CalamityWorld.downedCalamitas, 0.2f, 2, 5);
			DropHelper.DropItemCondition(player, ModContent.ItemType<Items.Placeables.Tenebris>(), CalamityWorld.downedCalamitas, 0.2f, 2, 5);
			DropHelper.DropItemCondition(player, ModContent.ItemType<CruptixBar>(), NPC.downedGolemBoss, 0.1f, 1, 3);
			DropHelper.DropItemCondition(player, ModContent.ItemType<ReaperTooth>(), CalamityWorld.downedPolterghast && CalamityWorld.downedBoomerDuke, 0.1f, 1, 5);

            // Weapons
            DropHelper.DropItemFromSetCondition(player, CalamityWorld.downedSlimeGod || Main.hardMode, 0.1f,
                ModContent.ItemType<Archerfish>(),
                ModContent.ItemType<BallOFugu>(),
                ModContent.ItemType<HerringStaff>(),
                ModContent.ItemType<Lionfish>(),
                ModContent.ItemType<BlackAnurian>());

            DropHelper.DropItemFromSetCondition(player, CalamityWorld.downedAquaticScourgeAcidRain, 0.1f,
                ModContent.ItemType<SkyfinBombers>(),
                ModContent.ItemType<NuclearRod>(),
                ModContent.ItemType<SulphurousGrabber>(),
                ModContent.ItemType<FlakToxicannon>(),
                ModContent.ItemType<SpentFuelContainer>(),
                ModContent.ItemType<SlitheringEels>(),
                ModContent.ItemType<BelchingSaxophone>());

            // Equipment
            DropHelper.DropItemFromSetCondition(player, CalamityWorld.downedSlimeGod || Main.hardMode, 0.25f,
                ModContent.ItemType<StrangeOrb>(),
                ModContent.ItemType<DepthCharm>(),
                ModContent.ItemType<IronBoots>(),
                ModContent.ItemType<AnechoicPlating>(),
                ModContent.ItemType<TorrentialTear>());

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
			int healingPotID = ItemID.LesserHealingPotion;
			int manaPotID = ItemID.LesserManaPotion;
			if (CalamityWorld.downedDoG)
			{
				healingPotID = ModContent.ItemType<SupremeHealingPotion>();
				manaPotID = ModContent.ItemType<SupremeManaPotion>();
			}
			else if (CalamityWorld.downedProvidence)
			{
				healingPotID = ItemID.SuperHealingPotion;
				manaPotID = ItemID.SuperManaPotion;
			}
			else if (NPC.downedMechBossAny)
			{
				healingPotID = ItemID.GreaterHealingPotion;
				manaPotID = ItemID.GreaterManaPotion;
			}
			else if (NPC.downedBoss3)
			{
				healingPotID = ItemID.HealingPotion;
				manaPotID = ItemID.ManaPotion;
			}
			DropHelper.DropItemChance(player, Main.rand.Next(100) >= 49 ? healingPotID : manaPotID, 0.25f, 2, 5);

            //Money
            DropHelper.DropItem(player, ItemID.SilverCoin, 10, 90);
            DropHelper.DropItemChance(player, ItemID.GoldCoin, 0.5f, 1, 5);
        }
    }
}
