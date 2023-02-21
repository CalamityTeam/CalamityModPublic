using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Ores
{
    public class AuricOre : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 100;
            DisplayName.SetDefault("Auric Ore");
            Tooltip.SetDefault("Infused with long lost draconic energies");
			ItemID.Sets.SortingPriorityMaterials[Type] = 119;
        }

        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<Tiles.Ores.AuricOre>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.width = 10;
            Item.height = 10;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(gold: 4);
            Item.rare = ModContent.RarityType<Violet>();
        }
    }
}
