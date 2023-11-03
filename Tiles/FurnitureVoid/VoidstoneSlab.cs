using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles.FurnitureVoid
{
    public class VoidstoneSlab : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            HitSound = SoundID.Tink;
            MineResist = 7f;
            MinPick = 180;
            AddMapEntry(new Color(27, 24, 31));
            AnimationFrameHeight = 270;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 180, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            int uniqueAnimationFrame = Main.tileFrame[Type] + i;
            int xPos = i % 3;
            int yPos = j % 3;
            switch (xPos)
            {
                case 0:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrame = 0;
                            break;
                        case 1:
                            uniqueAnimationFrame = 1;
                            break;
                        case 2:
                            uniqueAnimationFrame = 2;
                            break;
                        default:
                            uniqueAnimationFrame = 0;
                            break;
                    }
                    break;
                case 1:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrame = 2;
                            break;
                        case 1:
                            uniqueAnimationFrame = 3;
                            break;
                        case 2:
                            uniqueAnimationFrame = 4;
                            break;
                        default:
                            uniqueAnimationFrame = 0;
                            break;
                    }
                    break;
                case 2:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrame = 4;
                            break;
                        case 1:
                            uniqueAnimationFrame = 0;
                            break;
                        case 2:
                            uniqueAnimationFrame = 1;
                            break;
                        default:
                            uniqueAnimationFrame = 0;
                            break;
                    }
                    break;
            }

            frameYOffset = uniqueAnimationFrame * AnimationFrameHeight;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Main.tile[i, j].TileType == ModContent.TileType<VoidstoneSlab>())
            {
                int xPos = Main.tile[i, j].TileFrameX;
                int yPos = Main.tile[i, j].TileFrameY;
                int yOffset = 0;
                int relavtiveXPos = i % 3;
                int relavtiveYPos = j % 3;
                switch (relavtiveXPos)
                {
                    case 0:
                        switch (relavtiveYPos)
                        {
                            case 0:
                                yOffset = 0;
                                break;
                            case 1:
                                yOffset = 1;
                                break;
                            case 2:
                                yOffset = 2;
                                break;
                            default:
                                yOffset = 0;
                                break;
                        }
                        break;
                    case 1:
                        switch (relavtiveYPos)
                        {
                            case 0:
                                yOffset = 2;
                                break;
                            case 1:
                                yOffset = 3;
                                break;
                            case 2:
                                yOffset = 4;
                                break;
                            default:
                                yOffset = 0;
                                break;
                        }
                        break;
                    case 2:
                        switch (relavtiveYPos)
                        {
                            case 0:
                                yOffset = 4;
                                break;
                            case 1:
                                yOffset = 0;
                                break;
                            case 2:
                                yOffset = 1;
                                break;
                            default:
                                yOffset = 0;
                                break;
                        }
                        break;
                }
                yOffset *= 270;
                yPos += yOffset;
                Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/Tiles/FurnitureVoid/VoidstoneSlabGlow").Value;
                Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
                Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + zero;
                Color drawColour = GetDrawColour(i, j, new Color(75, 75, 75, 75));
                Tile trackTile = Main.tile[i, j];
                if (!trackTile.IsHalfBlock && trackTile.Slope == 0)
                {
                    Main.spriteBatch.Draw(glowmask, drawOffset, new Rectangle?(new Rectangle(xPos, yPos, 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                }
                else if (trackTile.IsHalfBlock)
                {
                    Main.spriteBatch.Draw(glowmask, drawOffset + new Vector2(0f, 8f), new Rectangle?(new Rectangle(xPos, yPos, 18, 8)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                }
            }
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
