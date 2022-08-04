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

        public override bool CanRightClick() => true;
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            int gemMin = 1;
            int gemMax = 3;

            itemLoot.Add(ItemID.Amethyst, 2, gemMin, gemMax);
            itemLoot.Add(ItemID.Topaz, new Fraction(4, 10), gemMin, gemMax);
            itemLoot.Add(ItemID.Sapphire, new Fraction(3, 10), gemMin, gemMax);
            itemLoot.Add(ItemID.Emerald, 5, gemMin, gemMax);
            itemLoot.Add(ItemID.Ruby, new Fraction(15, 100), gemMin, gemMax);
            itemLoot.Add(ItemID.Diamond, 10, gemMin, gemMax);
            itemLoot.Add(ItemID.Amber, 4, gemMin, gemMax);

            Mod thorium = CalamityMod.Instance.thorium;
            if (thorium != null)
            {
                itemLoot.Add(thorium.Find<ModItem>("Pearl").Type, 4, gemMin, gemMax);
                itemLoot.Add(thorium.Find<ModItem>("Opal").Type, 4, gemMin, gemMax);
                itemLoot.Add(thorium.Find<ModItem>("Onyx").Type, 4, gemMin, gemMax);
            }
        }
    }
}
