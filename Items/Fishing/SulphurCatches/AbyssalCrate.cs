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

using CrateTile = CalamityMod.Tiles.Abyss.AbyssalCrateTile;

namespace CalamityMod.Items.Fishing.SulphurCatches
{
    public class AbyssalCrate : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
            DisplayName.SetDefault("Abyssal Crate");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
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

        public override bool CanRightClick() => true;
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ModContent.ItemType<SulphurousSand>(), 1, 5, 10);
            itemLoot.Add(ModContent.ItemType<SulphurousSandstone>(), 1, 5, 10);
            itemLoot.Add(ModContent.ItemType<HardenedSulphurousSandstone>(), 1, 5, 10);
            itemLoot.Add(ModContent.ItemType<Acidwood>(), 1, 5, 10);

            itemLoot.AddIf(() => DownedBossSystem.downedEoCAcidRain, ModContent.ItemType<SulphuricScale>(), 10, 1, 3);
            itemLoot.AddIf(() => DownedBossSystem.downedAquaticScourgeAcidRain, ModContent.ItemType<CorrodedFossil>(), 10, 1, 3);
            itemLoot.AddIf(() => DownedBossSystem.downedCalamitas, ModContent.ItemType<DepthCells>(), 5, 2, 5);
            itemLoot.AddIf(() => DownedBossSystem.downedCalamitas, ModContent.ItemType<Lumenyl>(), 5, 2, 5);
            itemLoot.AddIf(() => DownedBossSystem.downedCalamitas, ModContent.ItemType<PlantyMush>(), 5, 2, 5);
            itemLoot.AddIf(() => DownedBossSystem.downedCalamitas, ModContent.ItemType<Tenebris>(), 5, 2, 5);
            itemLoot.AddIf(() => NPC.downedGolemBoss, ModContent.ItemType<ScoriaOre>(), 5, 16, 28);
            itemLoot.AddIf(() => NPC.downedGolemBoss, ModContent.ItemType<ScoriaBar>(), new Fraction(15, 100), 4, 7);
            itemLoot.AddIf(() => DownedBossSystem.downedPolterghast && DownedBossSystem.downedBoomerDuke, ModContent.ItemType<ReaperTooth>(), 10, 1, 5);

            // Weapons
            var lcrT1Abyss = itemLoot.DefineConditionalDropSet(() => DownedBossSystem.downedSlimeGod || Main.hardMode);
            lcrT1Abyss.Add(new OneFromOptionsNotScaledWithLuckDropRule(10, 1,
                ModContent.ItemType<Archerfish>(),
                ModContent.ItemType<BallOFugu>(),
                ModContent.ItemType<HerringStaff>(),
                ModContent.ItemType<Lionfish>(),
                ModContent.ItemType<BlackAnurian>()));

            var lcrT2AcidRain = itemLoot.DefineConditionalDropSet(() => DownedBossSystem.downedAquaticScourgeAcidRain);
            lcrT2AcidRain.Add(new OneFromOptionsNotScaledWithLuckDropRule(10, 1,
                ModContent.ItemType<SkyfinBombers>(),
                ModContent.ItemType<NuclearRod>(),
                ModContent.ItemType<SulphurousGrabber>(),
                ModContent.ItemType<FlakToxicannon>(),
                ModContent.ItemType<SpentFuelContainer>(),
                ModContent.ItemType<SlitheringEels>(),
                ModContent.ItemType<BelchingSaxophone>()));

            // Equipment
            lcrT1Abyss.Add(new OneFromOptionsNotScaledWithLuckDropRule(4, 1,
                ModContent.ItemType<StrangeOrb>(),
                ModContent.ItemType<DepthCharm>(),
                ModContent.ItemType<IronBoots>(),
                ModContent.ItemType<AnechoicPlating>(),
                ModContent.ItemType<TorrentialTear>()));

            //Bait
            itemLoot.Add(ItemID.MasterBait, 10, 1, 2);
            itemLoot.Add(ItemID.JourneymanBait, 5, 1, 3);
            itemLoot.Add(ItemID.ApprenticeBait, 3, 2, 3);

            //Potions
            itemLoot.Add(ModContent.ItemType<AnechoicCoating>(), 10, 1, 3);
            itemLoot.AddCratePotionRules();

            //Money
            itemLoot.Add(ItemID.SilverCoin, 1, 10, 90);
            itemLoot.Add(ItemID.GoldCoin, 2, 1, 5);
        }
    }
}
