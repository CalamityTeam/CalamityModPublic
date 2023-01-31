
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;

namespace CalamityMod.Tiles.Ores
{
    public class PerennialOre : ModTile
    {
        internal static Texture2D GlowTexture;
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                GlowTexture = ModContent.Request<Texture2D>("CalamityMod/Tiles/Ores/PerennialOreGlow", AssetRequestMode.ImmediateLoad).Value;
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileOreFinderPriority[Type] = 710;
            Main.tileShine[Type] = 2500;
            Main.tileShine2[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);

            TileID.Sets.Ore[Type] = true;
            TileID.Sets.OreMergesWithMud[Type] = true;

            ItemDrop = ModContent.ItemType<Items.Placeables.Ores.PerennialOre>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Perennial Ore");
            AddMapEntry(new Color(200, 250, 100), name);
            MineResist = 3f;
            MinPick = 200;
            HitSound = SoundID.Tink;
            Main.tileSpelunker[Type] = true;
        }
        int animationFrameWidth = 288;

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
            //The base green color glow
            r = 0.08f;
            g = 0.2f;
            b = 0.04f;
            var tile = Main.tile[i, j];
            var pos = new Vector2(tile.TileFrameX, tile.TileFrameY);

            Vector2[] positionsFlower = new Vector2[]
            {
                //Top row (always y = 0 on tile sheets)
                new Vector2(0, 0),
                new Vector2(36, 0),
                new Vector2(72, 0),
                new Vector2(252, 0),

                //Second row (always y = 18 on tile sheets)
                new Vector2(18, 18),
                new Vector2(54, 18),
                new Vector2(252, 18),

                //Third row (always y = 36 on tile sheets)
                new Vector2(36, 36),
                new Vector2(252, 36),

                //Forth row (always y = 54 on tile sheets)
                new Vector2(250, 54),

                //Nothing on fifth row (always y = 72 on tile sheets)

                //Sixth row (always y = 90 on tile sheets)
                new Vector2(72, 90),
                new Vector2(90, 90),
                new Vector2(144, 90),
                new Vector2(180, 90),
                new Vector2(198, 90),
                new Vector2(216, 90),

                //Seventh row (always y = 108 on tile sheets)
                new Vector2(72, 108),
                new Vector2(144, 108),
                new Vector2(180, 108),

                //Eighth row (always y = 126 on tile sheets)
                new Vector2(0, 126),
                new Vector2(18, 126),
                new Vector2(36, 126),
                new Vector2(54, 126),
                new Vector2(144, 126),
                new Vector2(162, 126),
                new Vector2(180, 126),
                new Vector2(198, 126),
                new Vector2(216, 126),

                //Ninth row (always y = 144 on tile sheets)
                new Vector2(0, 144),
                new Vector2(18, 144),
                new Vector2(36, 144),
                new Vector2(54, 144),
                new Vector2(72, 144),
                new Vector2(90, 144),
                new Vector2(198, 144),
                new Vector2(216, 144),
                //Tenth row (always y = 162 on tile sheets)
                new Vector2(0, 162),
                new Vector2(18, 162),
                new Vector2(36, 162),
                new Vector2(54, 162),
                new Vector2(72, 162),
                new Vector2(144, 162),
                new Vector2(162, 162),
                new Vector2(180, 162),
                //Eleventh row (always y = 180 on tile sheets)
                new Vector2(0, 180),
                new Vector2(18, 180),
                new Vector2(36, 180),
                new Vector2(54, 180),
                new Vector2(144, 180),
                new Vector2(180, 180),
                new Vector2(198, 180),
                new Vector2(216, 180),
                //Twelfth row (always y = 198 on tile sheets)
                new Vector2(18, 198),
                new Vector2(72, 198),
                new Vector2(108, 198),
                new Vector2(144, 198),
                //Thirteenth row (always y = 216 on tile sheets)
                new Vector2(18, 216),
                new Vector2(72, 216),
                //Nothing on fourteenth row (always y = 249 on tile sheets)

                //Nothing on fifteenth row (always y = 252 on tile sheets)
            };
            foreach (var positionFlower in positionsFlower)
            {
                if (pos == positionFlower)
                {
                    float pseudoRandom = MathF.Abs(MathF.Sin((positionFlower.X * 1294.2943f) + (positionFlower.Y * 96.454f))) % 1;
                    float brightness = 0.7f;
                    brightness *= (float)MathF.Sin(j / 14f + Main.GameUpdateCount * 0.017f);
                    brightness *= (float)MathF.Sin(i / 14f + Main.GameUpdateCount * 0.017f);
                    brightness += 0.3f;
                    r = 0.83f;
                    g = 0.16f;
                    b = 0.31f;
                    r *= brightness;
                    g *= brightness;
                    b *= brightness;
                }
            }
        }
        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            int uniqueAnimationFrameX = 0;
            int xPos = i % 4;
            int yPos = j % 4;
            switch (xPos)
            {
                case 0:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrameX = 0;
                            break;
                        case 1:
                            uniqueAnimationFrameX = 2;
                            break;
                        case 2:
                            uniqueAnimationFrameX = 1;
                            break;
                        case 3:
                            uniqueAnimationFrameX = 2;
                            break;
                        default:
                            uniqueAnimationFrameX = 2;
                            break;
                    }
                    break;
                case 1:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrameX = 2;
                            break;
                        case 1:
                            uniqueAnimationFrameX = 0;
                            break;
                        case 2:
                            uniqueAnimationFrameX = 2;
                            break;
                        case 3:
                            uniqueAnimationFrameX = 2;
                            break;
                        default:
                            uniqueAnimationFrameX = 2;
                            break;
                    }
                    break;
                case 2:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrameX = 2;
                            break;
                        case 1:
                            uniqueAnimationFrameX = 0;
                            break;
                        case 2:
                            uniqueAnimationFrameX = 1;
                            break;
                        case 3:
                            uniqueAnimationFrameX = 2;
                            break;
                        default:
                            uniqueAnimationFrameX = 2;
                            break;
                    }
                    break;
                case 3:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrameX = 1;
                            break;
                        case 1:
                            uniqueAnimationFrameX = 2;
                            break;
                        case 2:
                            uniqueAnimationFrameX = 0;
                            break;
                        case 3:
                            uniqueAnimationFrameX = 2;
                            break;
                        default:
                            uniqueAnimationFrameX = 2;
                            break;
                    }
                    break;
            }
            frameXOffset = uniqueAnimationFrameX * animationFrameWidth;
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (GlowTexture is null)
                return;

            int xPos = Main.tile[i, j].TileFrameX;
            int yPos = Main.tile[i, j].TileFrameY;
            int xOffset = 0;
            int relativeXPos = i % 4;
            int relativeYPos = j % 4;
            switch (relativeXPos)
            {
                case 0:
                    switch (relativeYPos)
                    {
                        case 0:
                            xOffset = 0;
                            break;
                        case 1:
                            xOffset = 2;
                            break;
                        case 2:
                            xOffset = 1;
                            break;
                        case 3:
                            xOffset = 2;
                            break;
                        default:
                            xOffset = 2;
                            break;
                    }
                    break;
                case 1:
                    switch (relativeYPos)
                    {
                        case 0:
                            xOffset = 2;
                            break;
                        case 1:
                            xOffset = 0;
                            break;
                        case 2:
                            xOffset = 2;
                            break;
                        case 3:
                            xOffset = 2;
                            break;
                        default:
                            xOffset = 2;
                            break;
                    }
                    break;
                case 2:
                    switch (relativeYPos)
                    {
                        case 0:
                            xOffset = 2;
                            break;
                        case 1:
                            xOffset = 0;
                            break;
                        case 2:
                            xOffset = 1;
                            break;
                        case 3:
                            xOffset = 2;
                            break;
                        default:
                            xOffset = 2;
                            break;
                    }
                    break;
                case 3:
                    switch (relativeYPos)
                    {
                        case 0:
                            xOffset = 1;
                            break;
                        case 1:
                            xOffset = 2;
                            break;
                        case 2:
                            xOffset = 0;
                            break;
                        case 3:
                            xOffset = 2;
                            break;
                        default:
                            xOffset = 2;
                            break;
                    }
                    break;
            }
            xOffset *= 288;
            xPos += xOffset;
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + zero;
            Color drawColour = GetDrawColour(i, j, new Color(175, 175, 175, 175));
            Tile trackTile = Main.tile[i, j];

            if (!trackTile.IsHalfBlock && trackTile.Slope == 0)
            {
                Main.spriteBatch.Draw(GlowTexture, drawOffset, new Rectangle?(new Rectangle(xPos, yPos, 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
            else if (trackTile.IsHalfBlock)
            {
                Main.spriteBatch.Draw(GlowTexture, drawOffset + new Vector2(0f, 8f), new Rectangle?(new Rectangle(xPos, yPos, 18, 8)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
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
