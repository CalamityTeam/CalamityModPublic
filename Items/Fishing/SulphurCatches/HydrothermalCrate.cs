using CalamityMod.Items.Accessories;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.Tools.ClimateChange;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Tiles.Abyss;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.SulphurCatches
{
    public class HydrothermalCrate : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Fishing";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            ItemID.Sets.IsFishingCrate[Type] = true;
            ItemID.Sets.IsFishingCrateHardmode[Type] = true;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SulphurousCrate>();
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<HydrothermalCrateTile>());
            Item.width = 32;
            Item.height = 32;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Green;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.Crates;
        }

        public override bool CanRightClick() => true;
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            // 20-50 Blocks @ 100%; Individually 33.33%
            itemLoot.Add(new OneFromRulesRule(1, new IItemDropRule[3]
            {
                ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Placeables.SulphurousSand>(), 1, 20, 50),
                ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Placeables.SulphurousSandstone>(), 1, 20, 50),
                ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Placeables.HardenedSulphurousSandstone>(), 1, 20, 50)
            }));

            // Rusty Chest Loot @ 100%; Individually 25%
            itemLoot.Add(new OneFromOptionsNotScaledWithLuckDropRule(1, 1,
                ModContent.ItemType<BrokenWaterFilter>(),
                ModContent.ItemType<EffigyOfDecay>(),
                ModContent.ItemType<RustyBeaconPrototype>(),
                ModContent.ItemType<RustyMedallion>()
            ));

            // Pre-HM Abyss Weapons
            itemLoot.Add(new OneFromOptionsDropRule(10, 1,
                ModContent.ItemType<BallOFugu>(),
                ModContent.ItemType<Archerfish>(),
                ModContent.ItemType<BlackAnurian>(),
                ModContent.ItemType<HerringStaff>(),
                ModContent.ItemType<Lionfish>(),
                ModContent.ItemType<AnechoicPlating>(),
                ModContent.ItemType<DepthCharm>(),
                ModContent.ItemType<IronBoots>(),
                ModContent.ItemType<StrangeOrb>(),
                ModContent.ItemType<TorrentialTear>()
            ));

            itemLoot.AddBiomeCrateLootRules();
        }
    }
}
