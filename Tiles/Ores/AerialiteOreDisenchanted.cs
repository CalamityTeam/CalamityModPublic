
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles.Ores
{
    public class AerialiteOreDisenchanted : ModTile
    {
        internal static Texture2D GlowTexture;
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                GlowTexture = ModContent.Request<Texture2D>("CalamityMod/Tiles/Ores/AerialiteOreDisenchantedGlow", AssetRequestMode.ImmediateLoad).Value;
            Main.tileOreFinderPriority[Type] = 445;
            Main.tileBlockLight[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileLighted[Type] = true;
            //Main.tileNoSunLight[Type] = false;

            TileID.Sets.Ore[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.SetMerge(Type, ModContent.TileType<AerialiteOre>());
            CalamityUtils.SetMerge(Type, TileID.Cloud);
            CalamityUtils.SetMerge(Type, TileID.RainCloud);
            CalamityUtils.SetMerge(Type, TileID.SnowCloud);
            //Main.tileMerge[TileID.Cloud][ModContent.TileType<AerialiteOre>()] = true;

            //Main.tileShine[Type] = 3500;
            Main.tileShine2[Type] = true;

            TileID.Sets.ChecksForMerge[Type] = true;
            DustType = 33;
            //ItemDrop = ModContent.ItemType<Items.Placeables.Ores.AerialiteOre>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Disenchanted Aerialite Ore");
            AddMapEntry(new Color(204, 170, 81), name);
            MineResist = 2f;
            MinPick = 65;
            HitSound = SoundID.Tink;
            Main.tileSpelunker[Type] = true;
        }
        public override void PostSetDefaults()
        {
        Main.tileNoSunLight[Type] = false;
        }

        int animationFrameWidth = 288;

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type, ModContent.TileType<AerialiteOre>());
            return false;
        }
        /*public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.21f;
            g = 0.52f;
            b = 0.63f;
        }*/
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
            Color drawColour = GetDrawColour(i, j, new Color(75, 75, 75, 50));
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
