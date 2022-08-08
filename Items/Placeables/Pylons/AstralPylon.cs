using CalamityMod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Pylons
{
    public class AstralPylon : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Pylons.AstralPylonTile>());

            Item.value = Item.buyPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Blue;
        }
    }
}
