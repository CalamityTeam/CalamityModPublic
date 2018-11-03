using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class ProvidenceMusicbox: ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Music Box (Providence)");
        }

        public override void SetDefaults()
        {
            item.useStyle = 1;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.createTile = mod.TileType("ProvidenceMusicbox");
            item.width = 24;
            item.height = 24;
            item.rare = 4;
            item.value = 100000;
            item.accessory = true;
        }
    }
}