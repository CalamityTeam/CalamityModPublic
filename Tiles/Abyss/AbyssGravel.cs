using CalamityMod.Tiles.Abyss.AbyssAmbient;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Abyss
{
    public class AbyssGravel : ModTile
    {
        int subsheetHeight = 270;
        int subsheetWidth = 288;

        public static readonly SoundStyle MineSound = new("CalamityMod/Sounds/Custom/AbyssGravelMine", 3);
        
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMergeDirt[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithAbyss(Type);

            ItemDrop = ModContent.ItemType<Items.Placeables.AbyssGravel>();
            AddMapEntry(new Color(25, 28, 54));
            MineResist = 5f;
            MinPick = 65;
            HitSound = MineSound;
            DustType = 33;
        }

        public override void RandomUpdate(int i, int j)
        {
            Tile up = Main.tile[i, j - 1];
            Tile up2 = Main.tile[i, j - 2];

            if (WorldGen.genRand.Next(5) == 0 && !up.HasTile && !up2.HasTile && up.LiquidAmount > 0)
            {
                up.TileType = (ushort)ModContent.TileType<AbyssWeeds>();
                up.HasTile = true;
                up.TileFrameY = 0;
                up.TileFrameX = (short)(WorldGen.genRand.Next(4) * 18);
                WorldGen.SquareTileFrame(i, j - 1, true);

                if (Main.netMode == NetmodeID.Server) 
                {
                    NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
                }
            }
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = i % 1 * subsheetWidth; //the 1 should be 2, but it looks weird right now, should be looked into later
            frameYOffset = j % 1 * subsheetHeight;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type, TileID.Dirt);
            return false;
        }
    }
}
