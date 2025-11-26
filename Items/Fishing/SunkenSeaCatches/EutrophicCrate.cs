using CalamityMod.Tiles.SunkenSea;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.SunkenSeaCatches
{
    [LegacyName("SunkenCrate")]
    public class EutrophicCrate : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Fishing";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            ItemID.Sets.IsFishingCrate[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<EutrophicCrateTile>());
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

            itemLoot.AddBiomeCrateLootRules(false);
        }
    }
}
