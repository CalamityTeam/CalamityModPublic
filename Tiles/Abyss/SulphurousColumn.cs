using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Abyss
{
    public class SulphurousColumn : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileSolidTop[Type] = true;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new int[]
            {
                16,
                16,
                16
            };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            AddMapEntry(new Color(150, 100, 50), name);
            name.SetDefault("Column");
            DustType = (int)CalamityDusts.SulfurousSeaAcid;

            base.SetDefaults();
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            for (int k = 0; k < WorldGen.genRand.Next(3, 4 + 1); k++)
            {
                Gore.NewGore(new Vector2(i, j) * 16f,
                    Vector2.One.RotatedByRandom(MathHelper.TwoPi) * WorldGen.genRand.NextFloat(1.4f, 3.2f),
                    Mod.GetGoreSlot($"Gores/SulphSeaGen/SulphurousRockGore{WorldGen.genRand.Next(3) + 1}"));
            }
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 2;
        }
    }
}
