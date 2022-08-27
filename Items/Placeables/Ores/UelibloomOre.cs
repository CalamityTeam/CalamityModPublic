using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Ores
{
    public class UelibloomOre : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 100;
            DisplayName.SetDefault("Uelibloom Ore");
			ItemID.Sets.SortingPriorityMaterials[Type] = 105;
        }

        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<Tiles.Ores.UelibloomOre>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.width = 10;
            Item.height = 10;
            Item.maxStack = 999;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.value = Item.sellPrice(gold: 1);
        }
    }
}
