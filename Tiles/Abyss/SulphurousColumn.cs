using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Enums;
using Terraria.Localization;
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
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
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
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(150, 100, 50), CalamityUtils.GetText("Tiles.Column"));
            DustType = (int)CalamityDusts.SulfurousSeaAcid;

            base.SetStaticDefaults();
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Tile t = CalamityUtils.ParanoidTileRetrieval(i, j);
            Tile left = CalamityUtils.ParanoidTileRetrieval(i - 1, j);
            Tile right = CalamityUtils.ParanoidTileRetrieval(i + 1, j);
            if (t.TileFrameX % 36 == 0 && !right.HasTile)
                WorldGen.KillTile(i, j);
            if (t.TileFrameX % 36 == 18 && !left.HasTile)
                WorldGen.KillTile(i, j);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            // Explode into a bunch of rocks when broken.
            for (int k = 0; k < WorldGen.genRand.Next(3, 4 + 1); k++)
            {
                int goreID = Mod.Find<ModGore>($"SulphurousRockGore{WorldGen.genRand.Next(3) + 1}").Type;
                Gore.NewGore(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16f, Main.rand.NextVector2Unit() * WorldGen.genRand.NextFloat(1.4f, 3.2f), goreID);
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 2;
        }
    }
}
