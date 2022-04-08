using CalamityMod.Dusts;
using CalamityMod.Items.Placeables.LivingFire;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
namespace CalamityMod.Tiles.LivingFire
{
    public class LivingHolyFireBlockTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            SoundType = SoundID.Dig;
            ItemDrop = ModContent.ItemType<LivingHolyFireBlock>();
            AddMapEntry(new Color(255, 215, 0));
            AnimationFrameHeight = 90;
            Main.tileSolid[Type] = false;
            Main.tileNoAttach[Type] = false;
            Main.tileFrameImportant[Type] = false;
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(CanPlaceAlter, -1, 0, true );
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(AfterPlacement, -1, 0, false);

            TileObjectData.addTile(Type);
        }

        public int CanPlaceAlter(int i, int j, int type, int style, int direction, int alternate) => 1;

        public static int AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 1, 1);
            }
            return 1;
        }

        public override bool CanPlace(int i, int j)
        {
            List<List<int>> around = new List<List<int>>
            {
                new List<int>() {i, j-1 },
                new List<int>() {i-1, j },
                new List<int>() {i+1, j },
                new List<int>() {i, j+1 }
            };
            //if (Main.tile[i, j]) || tile.type == Type

            //return true; //temporary for testing purposes

            if (Main.tile[i, j].WallType != WallID.None)
            {
                return true;
            }

            for (int k = 0; k < around.Count; ++k)
            {
                Tile tile = Main.tile[around[k][0], around[k][1]];
                if (tile.HasTile && (Main.tileSolid[tile.TileType] || CalamityLists.livingFireBlockList.Contains(tile.TileType)))
                {
                    return true;
                }
            }
            return false;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frame = Main.tileFrame[TileID.LivingFire];
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 1f;
            g = 1f;
            b = 0f;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, (int)CalamityDusts.ProfanedFire, 0f, 0f, 1, default, 1f);
            return false;
        }
    }
}
