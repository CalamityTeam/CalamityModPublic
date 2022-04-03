using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class LaboratoryShelf : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpPlatform(true);
            soundType = SoundID.Tink;
            dustType = 30;
            AddMapEntry(new Color(97, 87, 86));
            drop = ModContent.ItemType<Items.Placeables.DraedonStructures.LaboratoryShelf>();
            disableSmartCursor = true;
            adjTiles = new int[] { TileID.Platforms };
        }

        public override bool CanExplode(int i, int j) => false;

        public override void PostSetDefaults()
        {
            Main.tileNoSunLight[Type] = false;
        }
    }
}
