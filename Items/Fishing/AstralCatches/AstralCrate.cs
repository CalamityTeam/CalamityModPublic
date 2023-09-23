using CalamityMod.Items.Accessories;
using CalamityMod.Items.Critters;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Tiles.Astral;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.AstralCatches
{
    public class AstralCrate : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Fishing";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            ItemID.Sets.IsFishingCrate[Type] = true;
            ItemID.Sets.IsFishingCrateHardmode[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);
            Item.createTile = ModContent.TileType<AstralCrateTile>();
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
            var postAstrumAureus = itemLoot.DefineConditionalDropSet(() => DownedBossSystem.downedAstrumAureus);
            var postAstrumDeus = itemLoot.DefineConditionalDropSet(() => DownedBossSystem.downedAstrumDeus);

            // Materials
            itemLoot.Add(ModContent.ItemType<Stardust>(), 1, 5, 10);
            itemLoot.Add(ItemID.FallenStar, 1, 5, 10);
            itemLoot.Add(ItemID.Meteorite, 5, 10, 20);
            itemLoot.Add(ItemID.MeteoriteBar, 10, 1, 3);

            postAstrumAureus.Add(ModContent.ItemType<AureusCell>(), 5, 2, 5);
            postAstrumDeus.Add(ModContent.ItemType<AstralOre>(), 5, 10, 20);
            postAstrumDeus.Add(ModContent.ItemType<AstralBar>(), 10, 1, 3);
            postAstrumDeus.Add(ModContent.ItemType<MeldBlob>(), 4, 5, 10);

            // Weapons (and Starburster Core)
            postAstrumAureus.Add(new OneFromOptionsDropRule(10, 1,
                ModContent.ItemType<AstralScythe>(),
                ModContent.ItemType<TitanArm>(),
                ModContent.ItemType<StellarCannon>(),
                ModContent.ItemType<AstralachneaStaff>(),
                ModContent.ItemType<HivePod>(),
                ModContent.ItemType<StellarKnife>(),
                ModContent.ItemType<StarbusterCore>()
            ));

            // Pet
            itemLoot.Add(ModContent.ItemType<AstrophageItem>(), 10);

            // Bait
            itemLoot.Add(ModContent.ItemType<TwinklerItem>(), 5, 1, 3);
            itemLoot.Add(ItemID.EnchantedNightcrawler, 5, 1, 3);
            itemLoot.Add(ModContent.ItemType<ArcturusAstroidean>(), 5, 1, 3);
            itemLoot.Add(ItemID.Firefly, 3, 1, 3);

            // Potions
            postAstrumAureus.Add(ModContent.ItemType<AstralInjection>(), 10, 1, 3);
            postAstrumAureus.Add(ModContent.ItemType<GravityNormalizerPotion>(), 10, 1, 3);
            itemLoot.AddCratePotionRules();

            // Money
            itemLoot.Add(ItemID.SilverCoin, 1, 10, 90);
            itemLoot.Add(ItemID.GoldCoin, 2, 1, 5);
        }
    }
}
