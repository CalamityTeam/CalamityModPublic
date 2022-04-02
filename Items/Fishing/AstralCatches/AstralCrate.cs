using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Critters;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Tiles.Astral;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.AstralCatches
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
            item.rare = ItemRarityID.Green;
            item.value = Item.sellPrice(gold: 1);
            item.createTile = ModContent.TileType<AstralCrateTile>();
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
            DropHelper.DropItem(player, ModContent.ItemType<Stardust>(), 5, 10);
            DropHelper.DropItem(player, ItemID.FallenStar, 5, 10);
            DropHelper.DropItemChance(player, ItemID.Meteorite, 0.2f, 10, 20);
            DropHelper.DropItemChance(player, ItemID.MeteoriteBar, 0.1f, 1, 3);
            DropHelper.DropItemCondition(player, ModContent.ItemType<AstralJelly>(), CalamityWorld.downedAstrageldon, 0.2f, 2, 5);
            DropHelper.DropItemCondition(player, ModContent.ItemType<AstralOre>(), CalamityWorld.downedStarGod, 0.2f, 10, 20);
            DropHelper.DropItemCondition(player, ModContent.ItemType<AstralBar>(), CalamityWorld.downedStarGod, 0.1f, 1, 3);
            DropHelper.DropItemCondition(player, ModContent.ItemType<MeldBlob>(), CalamityWorld.downedStarGod, 0.25f, 5, 10);

            // Weapons
            DropHelper.DropItemFromSetCondition(player, CalamityWorld.downedAstrageldon, 0.1f,
                ModContent.ItemType<StellarKnife>(),
                ModContent.ItemType<AstralachneaStaff>(),
                ModContent.ItemType<TitanArm>(),
                ModContent.ItemType<HivePod>(),
                ModContent.ItemType<AstralScythe>(),
                ModContent.ItemType<StellarCannon>(),
                ModContent.ItemType<StarbusterCore>());

            //Pet
            DropHelper.DropItemChance(player, ModContent.ItemType<AstrophageItem>(), 10);

            //Bait
            DropHelper.DropItemChance(player, ModContent.ItemType<TwinklerItem>(), 5, 1, 3);
            DropHelper.DropItemChance(player, ItemID.EnchantedNightcrawler, 5, 1, 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<ArcturusAstroidean>(), 5, 1, 3);
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
            DropHelper.DropItemCondition(player, ModContent.ItemType<AstralInjection>(), CalamityWorld.downedAstrageldon, 0.1f, 1, 3);
            DropHelper.DropItemCondition(player, ModContent.ItemType<GravityNormalizerPotion>(), CalamityWorld.downedAstrageldon, 0.1f, 1, 3);
            int healingPotID = ItemID.HealingPotion;
            int manaPotID = ItemID.ManaPotion;
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
            DropHelper.DropItemChance(player, Main.rand.NextBool(2) ? healingPotID : manaPotID, 0.25f, 2, 5);

            //Money
            DropHelper.DropItem(player, ItemID.SilverCoin, 10, 90);
            DropHelper.DropItemChance(player, ItemID.GoldCoin, 0.5f, 1, 5);
        }
    }
}
