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

            HitSound = SoundID.Item52;
            DustType = 32;
            MinPick = 30;
            ItemDrop = ModContent.ItemType<Items.Placeables.DraedonStructures.RustedPipes>();
            AddMapEntry(new Color(128, 90, 77));
        }

        public override bool CanExplode(int i, int j) => false;

        public override void PlaceInWorld(int i, int j, Item item)
        {
            SoundEngine.PlaySound(SoundID.Item52 with { Volume = SoundID.Item52.Volume * 0.75f, Pitch = SoundID.Item52.Pitch - 0.5f }, new Vector2( i * 16, j * 16 ));
        }
    }
}
