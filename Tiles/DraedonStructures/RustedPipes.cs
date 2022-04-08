using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class RustedPipes : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<LaboratoryPipePlating>()] = true;

            SoundType = SoundID.Item;
            SoundStyle = 52;
            DustType = 32;
            MinPick = 30;
            ItemDrop = ModContent.ItemType<Items.Placeables.DraedonStructures.RustedPipes>();
            AddMapEntry(new Color(128, 90, 77));
        }

        public override bool CanExplode(int i, int j) => false;

        public override void PlaceInWorld(int i, int j, Item item)
        {
            SoundEngine.PlaySound(SoundID.Item,i * 16, j * 16, 52, 0.75f, -0.5f);
        }
    }
}
