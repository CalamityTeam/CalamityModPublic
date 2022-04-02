using CalamityMod.Items.Materials;
using CalamityMod.Items.Critters;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Potions;
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
            item.rare = ItemRarityID.Green;
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
            DropHelper.DropItemCondition(player, ModContent.ItemType<PrismShard>(), CalamityWorld.downedDesertScourge, 5, 10);
            DropHelper.DropItemCondition(player, ModContent.ItemType<Items.Placeables.SeaPrism>(), CalamityWorld.downedDesertScourge, 0.2f, 2, 5);
            DropHelper.DropItemCondition(player, ModContent.ItemType<MolluskHusk>(), CalamityWorld.downedCLAMHardMode, 0.12f, 2, 5);

            // Weapons
            DropHelper.DropItemFromSetCondition(player, CalamityWorld.downedCLAMHardMode, 0.07f,
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
            DropHelper.DropItemChance(player, Main.rand.NextBool(2) ? healingPotID : manaPotID, 0.25f, 2, 5);

            //Money
            DropHelper.DropItem(player, ItemID.SilverCoin, 10, 90);
            DropHelper.DropItemChance(player, ItemID.GoldCoin, 0.5f, 1, 5);
        }
    }
}
