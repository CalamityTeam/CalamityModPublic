using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Crags.Tree
{
    public class SpineSapling : ModTile
    {
        public override void Load()
        {
            On_Player.TryReplantingTree += LetAxeofRegrowthReplant;
        }

        private static void LetAxeofRegrowthReplant(On_Player.orig_TryReplantingTree orig, Player self, int x, int y)
        {
            if (Main.tile[x, y + 1].TileType == ModContent.TileType<BrimstoneSlag>())
            {
                // Destroy the Spine Tree tile above the base to give space for the Small Spine to place
                WorldGen.KillTile(x, y - 1);
                if (!TileObject.CanPlace(Player.tileTargetX, Player.tileTargetY, ModContent.TileType<SpineSapling>(), 0, self.direction, out var objectData))
                    return;
                bool placed = TileObject.Place(objectData);
                WorldGen.SquareTileFrame(Player.tileTargetX, Player.tileTargetY);
                if (placed)
                {
                    TileObjectData.CallPostPlacementPlayerHook(Player.tileTargetX, Player.tileTargetY, ModContent.TileType<SpineSapling>(), 0, self.direction, objectData.alternate, objectData);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendObjectPlacement(-1, Player.tileTargetX, Player.tileTargetY, objectData.type, objectData.style, objectData.alternate, objectData.random, self.direction);
                }
            }
            else
                orig(self, x, y);
        }

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.CommonSapling[Type] = true;
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorValidTiles = new[] { ModContent.TileType<Tiles.Crags.BrimstoneSlag>() };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawFlipHorizontal = true;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.RandomStyleRange = 3;
            TileObjectData.newTile.StyleMultiplier = 3;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(38, 25, 27), CreateMapEntryName());
            DustType = DustID.Blood;
            AdjTiles = new int[] { TileID.Saplings };
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void RandomUpdate(int i, int j)
        {
            if (WorldGen.genRand.NextBool(20))
            {
                // if below tile is somewhat not sappling (only possible if it's from below part)
                if (Main.tile[i, j + 1].TileType != Type)
                    SpineTree.Spawn(i, j, 22, 28, true);
            }
        }

        public override bool CanDrop(int i, int j) => false;
    }
}
