using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Abyss.AbyssAmbient 
{
    public class VoidWeeds : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileCut[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileFrameImportant[Type] = true;
			TileID.Sets.ReplaceTileBreakUp[Type] = true;
			TileID.Sets.SwaysInWindBasic[Type] = true;
			TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.addTile(Type);

            DustType = 33;
            HitSound = SoundID.Grass;

            AddMapEntry(new Color(2, 2, 2));

            base.SetStaticDefaults();
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = -16;
            height = 32;
        }
    }
}
