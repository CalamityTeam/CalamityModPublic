using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Astral;
using Terraria;
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
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<MonolithCrate>();
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<AstralCrateTile>());
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
            // 4-10 Starblight Soot @ 50%
            // This is our equivalent to Crystal Shards/Ichor
            itemLoot.Add(ModContent.ItemType<StarblightSoot>(), 2, 4, 10);

            // Astrophage @ 5%
            itemLoot.Add(ModContent.ItemType<AstrophageItem>(), 20);

            itemLoot.AddBiomeCrateLootRules();
        }
    }
}
