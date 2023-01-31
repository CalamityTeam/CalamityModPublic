using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.BrimstoneCragCatches
{
    [LegacyName("ChaoticFish")]
    public class Havocfish : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Havocfish");
            Tooltip.SetDefault("The horns lay a curse on those who touch it\n" +
            "Right click to extract essence");
            SacrificeTotal = 10;
            ItemID.Sets.CanBePlacedOnWeaponRacks[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.value = Item.sellPrice(silver: 10);
            Item.rare = ItemRarityID.Green;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.GoodieBags;
		}

        public override bool CanRightClick() => true;
        public override void ModifyItemLoot(ItemLoot itemLoot) => itemLoot.Add(ModContent.ItemType<EssenceofHavoc>(), 1, 5, 10);
    }
}
