using CalamityMod.Items.Materials;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Tools.ClimateChange;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Items.Placeables.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;

using CrateTile = CalamityMod.Tiles.Abyss.SulphurousCrateTile;

namespace CalamityMod.Items.Fishing.SulphurCatches
{
    [LegacyName("AbyssalCrate")]
    public class SulphurousCrate : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Fishing";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            ItemID.Sets.IsFishingCrate[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);
            Item.createTile = ModContent.TileType<CrateTile>();
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.Crates;
		}

        public override bool CanRightClick() => true;
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            // The contents of this crate depend massively on progression, so it uses a large number of LeadingConditionRules.
            var tier1AbyssAvailable = itemLoot.DefineConditionalDropSet(() => DownedBossSystem.downedSlimeGod || Main.hardMode);
            var tier2AbyssAvailable = itemLoot.DefineConditionalDropSet(() => DownedBossSystem.downedCalamitasClone);
            var scoriaAvailable = itemLoot.DefineConditionalDropSet(() => NPC.downedGolemBoss);
            var tier1AcidRain = itemLoot.DefineConditionalDropSet(() => DownedBossSystem.downedEoCAcidRain);
            var tier2AcidRain = itemLoot.DefineConditionalDropSet(() => DownedBossSystem.downedAquaticScourgeAcidRain);
            var tier3AcidRain = itemLoot.DefineConditionalDropSet(() => DownedBossSystem.downedPolterghast && DownedBossSystem.downedBoomerDuke);

            // Materials
            itemLoot.Add(ModContent.ItemType<SulphurousSand>(), 1, 5, 10);
            itemLoot.Add(ModContent.ItemType<SulphurousSandstone>(), 1, 5, 10);
            itemLoot.Add(ModContent.ItemType<HardenedSulphurousSandstone>(), 1, 5, 10);
            itemLoot.Add(ModContent.ItemType<Acidwood>(), 1, 5, 10);

            tier1AcidRain.Add(ModContent.ItemType<SulphuricScale>(), 10, 1, 3);
            tier2AcidRain.Add(ModContent.ItemType<CorrodedFossil>(), 10, 1, 3);
            tier2AbyssAvailable.Add(ModContent.ItemType<DepthCells>(), 5, 2, 5);
            tier2AbyssAvailable.Add(ModContent.ItemType<Lumenyl>(), 5, 2, 5);
            tier2AbyssAvailable.Add(ModContent.ItemType<PlantyMush>(), 5, 2, 5);
            scoriaAvailable.Add(ModContent.ItemType<ScoriaOre>(), 5, 16, 28);
            scoriaAvailable.Add(ModContent.ItemType<ScoriaBar>(), new Fraction(15, 100), 4, 7);
            tier3AcidRain.Add(ModContent.ItemType<ReaperTooth>(), 10, 1, 5);

            // Pre-HM Abyss Weapons
            tier1AbyssAvailable.Add(new OneFromOptionsDropRule(10, 1,
                ModContent.ItemType<BallOFugu>(),
                ModContent.ItemType<Archerfish>(),
                ModContent.ItemType<BlackAnurian>(),
                ModContent.ItemType<HerringStaff>(),
                ModContent.ItemType<Lionfish>()
            ));

            // Post-AS Acid Rain Weapons (and Nuclear Rod)
            tier2AcidRain.Add(new OneFromOptionsDropRule(10, 1,
                ModContent.ItemType<SulphurousGrabber>(),
                ModContent.ItemType<FlakToxicannon>(),
                ModContent.ItemType<BelchingSaxophone>(),
                ModContent.ItemType<SlitheringEels>(),
                ModContent.ItemType<SkyfinBombers>(),
                ModContent.ItemType<SpentFuelContainer>(),
                ModContent.ItemType<NuclearFuelRod>()
            ));

            // Pre-HM Abyss Equipment (and Torrential Tear)
            tier1AbyssAvailable.Add(new OneFromOptionsDropRule(4, 1,
                ModContent.ItemType<AnechoicPlating>(),
                ModContent.ItemType<DepthCharm>(),
                ModContent.ItemType<IronBoots>(),
                ModContent.ItemType<StrangeOrb>(),
                ModContent.ItemType<TorrentialTear>()
            ));

            // Bait
            itemLoot.Add(ItemID.MasterBait, 10, 1, 2);
            itemLoot.Add(ItemID.JourneymanBait, 5, 1, 3);
            itemLoot.Add(ItemID.ApprenticeBait, 3, 2, 3);

            // Potions
            itemLoot.Add(ModContent.ItemType<AnechoicCoating>(), 10, 1, 3);
            itemLoot.AddCratePotionRules();

            // Money
            itemLoot.Add(ItemID.SilverCoin, 1, 10, 90);
            itemLoot.Add(ItemID.GoldCoin, 2, 1, 5);
        }
    }
}
