using CalamityMod.Projectiles.Environment;
using CalamityMod.Tiles.Abyss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Ores
{
    [LegacyName("ChaoticOre")]
    public class ScoriaOre : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileOreFinderPriority[Type] = 850;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithAbyss(Type);

            TileID.Sets.Ore[Type] = true;

            DustType = 105;
            ItemDrop = ModContent.ItemType<Items.Placeables.Ores.ScoriaOre>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Scoria");
            AddMapEntry(new Color(210, 101, 28), name);
            MineResist = 4f;
            MinPick = 210;
            HitSound = SoundID.Tink;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Tile up = Main.tile[i, j - 1];
            Tile up2 = Main.tile[i, j - 2];
            if (closer && Main.rand.NextBool(30) && !up.HasTile && !up2.HasTile)
            {
                Dust dust;
                dust = Main.dust[Dust.NewDust(new Vector2(i * 16f, j * 16f), 16, 16, 127, 0f, -10f, 47, new Color(255, 255, 255), 1.0465117f)];
                dust.noGravity = true;
                dust.fadeIn = 1.2209302f;

                dust = Main.dust[Dust.NewDust(new Vector2(i * 16f, j * 16f), 16, 16, 31, 0f, -1.9069767f, 195, new Color(255, 255, 255), 1f)];
                dust.noGravity = false;
                dust.fadeIn = 1.4209302f;

            }
            if (!closer && j < Main.maxTilesY - 205)
            {
                if (Main.tile[i, j].LiquidAmount <= 0)
                {
                    Main.tile[i, j].LiquidAmount = 255;
                    Main.tile[i, j].Get<LiquidData>().LiquidType = LiquidID.Water;
                }
            }
            if (Main.gamePaused)
            {
                return;
            }
            if (closer && Main.rand.NextBool(400))
            {
                int tileLocationY = j + 1;
                if (Main.tile[i, tileLocationY] != null)
                {
                    if (!Main.tile[i, tileLocationY].HasTile)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(new EntitySource_WorldEvent(), (float)(i * 16 + 16), (float)(tileLocationY * 16 + 16), 0f, 0.1f, ModContent.ProjectileType<LavaChunk>(), 25, 2f, Main.myPlayer, 0f, 0f);
                        }
                    }
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
        
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.04f;
            g = 0.00f;
            b = 0.00f;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type, ModContent.TileType<AbyssGravel>(), false, false, false, false, resetFrame);
            return false;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int xPos = Main.tile[i, j].TileFrameX;
            int yPos = Main.tile[i, j].TileFrameY;
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/Tiles/Ores/ScoriaOreGlow").Value;
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + zero;
            Color drawColour = GetDrawColour(i, j, new Color(50, 50, 50, 50));
            Tile trackTile = Main.tile[i, j];
            double num6 = Main.time * 0.08;
            if (!trackTile.IsHalfBlock && trackTile.Slope == 0)
            {
                Main.spriteBatch.Draw(glowmask, drawOffset, new Rectangle?(new Rectangle(xPos, yPos, 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
            else if (trackTile.IsHalfBlock)
            {
                Main.spriteBatch.Draw(glowmask, drawOffset + new Vector2(0f, 8f), new Rectangle?(new Rectangle(xPos, yPos, 18, 8)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
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
