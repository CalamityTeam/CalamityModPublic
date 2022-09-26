using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Fishing.SunkenSeaCatches
{
    public class GreenwaveLoach : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greenwave Loach");
            Tooltip.SetDefault("An endangered fish that is highly prized in the market");
            SacrificeTotal = 3;
            ItemID.Sets.CanBePlacedOnWeaponRacks[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 38;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(gold: 10);
            Item.rare = ItemRarityID.Orange;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.Fish;
		}
    }
}
