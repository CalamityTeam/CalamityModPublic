using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.Ores
{
    public class AuricOre : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Auric Ore");
            Tooltip.SetDefault("Laced with the dormant power of a deity");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<Tiles.Ores.AuricOre>();
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 10;
            item.height = 10;
            item.maxStack = 999;
            item.value = Item.sellPrice(gold: 4);
            item.Calamity().customRarity = CalamityRarity.Violet;
        }
    }
}
