using CalamityMod.Items.Critters;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Tiles.SunkenSea;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.SunkenSeaCatches
{
    public class PrismCrate : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Fishing";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            ItemID.Sets.IsFishingCrate[Type] = true;
            ItemID.Sets.IsFishingCrateHardmode[Type] = true;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<EutrophicCrate>();
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<PrismCrateTile>());
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
            // 20-50 Blocks @ 100%; Individually 50%
            itemLoot.Add(new OneFromRulesRule(1, new IItemDropRule[2]
            {
                ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Placeables.Navystone>(), 1, 20, 50),
                ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Placeables.EutrophicSand>(), 1, 20, 50)
            }));

            itemLoot.AddBiomeCrateLootRules();
        }
    }
}
