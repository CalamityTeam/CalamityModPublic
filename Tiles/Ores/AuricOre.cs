using Microsoft.Xna.Framework;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Tiles.Astral;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using ReLogic.Content;
using Terraria.ObjectData;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;

namespace CalamityMod.Tiles.Ores
{
    public class AuricOre : ModTile
    {
        public static readonly SoundStyle MineSound = new("CalamityMod/Sounds/Custom/AuricMine", 3);
        public static bool Animate;
        internal static Texture2D GlowTexture;

        public byte[,] tileAdjacency;
        public byte[,] secondTileAdjacency;
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                GlowTexture = ModContent.Request<Texture2D>("CalamityMod/Tiles/Ores/AuricOreGlow", AssetRequestMode.ImmediateLoad).Value;
            AnimationFrameHeight = 90;
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileOreFinderPriority[Type] = 1000;
            Main.tileShine[Type] = 3500;
            Main.tileShine2[Type] = false;

            CalamityUtils.MergeWithGeneral(Type);

            TileID.Sets.Ore[Type] = true;
            TileID.Sets.OreMergesWithMud[Type] = true;

            DustType = 55;
            AddMapEntry(new Color(255, 200, 0), CreateMapEntryName());
            MineResist = 5f;
            MinPick = 250;
            HitSound = MineSound;

            TileFraming.SetUpUniversalMerge(Type, TileID.Dirt, out tileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.Stone, out secondTileAdjacency);
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (!Animate)
            {return;}
            r = 0.24f;
            g = 0.40f;
            b = 0.47f;
        }
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (!Animate)
            {return;}
                frameCounter++;
            if (frameCounter > 4)
            {
                frameCounter = 0;
                frame++;
                if (frame > 7)
                {
                    Animate = false;
                    frame = 0;
                }
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int xPos = Main.tile[i, j].TileFrameX;
            int yPos = Main.tile[i, j].TileFrameY + AnimationFrameHeight * Main.tileFrame[Type];
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/Tiles/Ores/AuricOreGlow").Value;
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + zero;
            Color drawColour = GetDrawColour(i, j, new Color(225, 255, 255, 255));
            Tile trackTile = Main.tile[i, j];
            double num6 = Main.time * 0.08;
            TileFraming.SlopedGlowmask(i, j, 0, glowmask, drawOffset, null, GetDrawColour(i, j, drawColour), default);

            TileFraming.DrawUniversalMergeFrames(i, j, secondTileAdjacency, "CalamityMod/Tiles/Merges/StoneMerge");
            TileFraming.DrawUniversalMergeFrames(i, j, tileAdjacency, "CalamityMod/Tiles/Merges/DirtMerge");
        }
        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.GetAdjacencyData(i, j, TileID.Dirt, out tileAdjacency[i, j]);
            TileFraming.GetAdjacencyData(i, j, TileID.Stone, out secondTileAdjacency[i, j]);
            return true;
        }
        private Color GetDrawColour(int i, int j, Color colour)
        {
            int colType = Main.tile[i, j].TileColor;
            Color paintCol = WorldGen.paintColor(colType);
            if (colType >= 13 && colType <= 24)
            {
                colour.R = (byte)(paintCol.R / 255f * colour.R);
                colour.G = (byte)(paintCol.G / 255f * colour.G);
                colour.B = (byte)(paintCol.B / 255f * colour.B);
            }
            return colour;
        }
    }
}
