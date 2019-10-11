using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class AuricOre : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Auric Ore");
        }

        public override void SetDefaults()
        {
            item.createTile = mod.TileType("AuricOre");
            item.useStyle = 1;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 10;
            item.height = 10;
            item.maxStack = 999;
            item.value = Item.sellPrice(gold: 2);
            item.Calamity().postMoonLordRarity = 14;
        }
    }
}
