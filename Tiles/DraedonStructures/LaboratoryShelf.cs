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
            HitSound = SoundID.Tink;
            DustType = 30;
            AddMapEntry(new Color(97, 87, 86));
            ItemDrop = ModContent.ItemType<Items.Placeables.DraedonStructures.LaboratoryShelf>();
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
