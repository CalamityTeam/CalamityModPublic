using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class RustedShelf : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpPlatform(true);
            SoundType = SoundID.Tink;
            DustType = 32;
            AddMapEntry(new Color(128, 90, 77));
            ItemDrop = ModContent.ItemType<Items.Placeables.DraedonStructures.RustedShelf>();
            TileID.Sets.DisableSmartCursor[Type] = true;
            AdjTiles = new int[] { TileID.Platforms };
        }

        public override bool CanExplode(int i, int j) => false;

        public override void PostSetDefaults()
        {
            Main.tileNoSunLight[Type] = false;
        }
    }
}
