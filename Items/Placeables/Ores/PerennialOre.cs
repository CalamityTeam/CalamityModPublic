using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Ores
{
    public class PerennialOre : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 100;
            DisplayName.SetDefault("Perennial Ore");
			ItemID.Sets.SortingPriorityMaterials[Type] = 92; // Shroomite Bar
        }

        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<Tiles.Ores.PerennialOre>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.width = 12;
            Item.height = 10;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 18);
            Item.rare = ItemRarityID.Lime;
        }
    }
}
