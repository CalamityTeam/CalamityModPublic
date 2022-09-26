using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing
{
    public class GlimmeringGemfish : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glimmering Gemfish");
            Tooltip.SetDefault("Right click to extract gems");
            SacrificeTotal = 10;
            ItemID.Sets.CanBePlacedOnWeaponRacks[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 34;
            Item.height = 30;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 50);
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.GoodieBags;
		}

        public override bool CanRightClick() => true;
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            int gemMin = 1;
            int gemMax = 3;
            itemLoot.Add(ItemID.Amethyst, 2, gemMin, gemMax);
            itemLoot.Add(ItemID.Topaz, 2, gemMin, gemMax);
            itemLoot.Add(ItemID.Sapphire, 4, gemMin, gemMax);
            itemLoot.Add(ItemID.Emerald, 4, gemMin, gemMax);
            itemLoot.Add(ItemID.Ruby, 8, gemMin, gemMax);
            itemLoot.Add(ItemID.Diamond, 10, gemMin, gemMax);
            itemLoot.Add(ItemID.Amber, 8, gemMin, gemMax);

            Mod thorium = CalamityMod.Instance.thorium;
            if (thorium is not null)
            {
				try
				{
					itemLoot.Add(thorium.Find<ModItem>("Pearl").Type, 4, gemMin, gemMax);
					itemLoot.Add(thorium.Find<ModItem>("Opal").Type, 4, gemMin, gemMax);
					itemLoot.Add(thorium.Find<ModItem>("Onyx").Type, 4, gemMin, gemMax);
				}
				catch
				{
					CalamityMod.Instance.Logger.Debug("One of the items in this file got renamed internally. Please report this in the #bugs-read-pins channel of the official Calamity discord server.");
				}
            }
        }
    }
}
