using CalamityMod.Dusts;
using CalamityMod.Items.Placeables.Furniture.Monoliths;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Furniture.Monoliths
{
    public class PlagueHumidifierTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            RegisterItemDrop(ModContent.ItemType<PlagueHumidifier>());
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.Origin = new Point16(1, 3);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16 };
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, 2, 0);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(44, 150, 54));

            DustType = (int)CalamityDusts.Plague;
            AnimationFrameHeight = 74;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (Main.tile[i, j].TileFrameY < 74)
            {
                return;
            }

            Player player = Main.LocalPlayer;
            if (player is null)
            {
                return;
            }

            if (player.active)
            {
                Main.LocalPlayer.Calamity().monolithPlagueShader = 30;
            }

            // Spawn mist particles at the bottom tiles
            if (Main.tile[i,j +1].TileType != Type)
            {
                if (Main.rand.NextBool(30))
                {
                    GeneralParticleHandler.SpawnParticle(new PlagueHumidifierMist(new Vector2(i * 16, j * 16) + new Vector2(Main.rand.NextFloat(0, 16), Main.rand.NextFloat(16)), Main.rand.Next(40, 80), Main.rand.NextFloat(0.8f, 1.2f), Vector2.UnitX * Main.rand.NextFloat(-0.8f, 0.8f)));
                }
            }
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter >= 7.2)
            {
                frameCounter = 0;
                if (++frame >= 4)
                {
                    frame = 0;
                }
            }
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<PlagueHumidifier>();
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool RightClick(int i, int j)
        {
            HitWire(i, j);
            SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));
            return true;
        }

        public override void HitWire(int i, int j)
        {
            int x = i - Main.tile[i, j].TileFrameX / 18 % 2;
            int y = j - Main.tile[i, j].TileFrameY / 18 % 4;
            int tileXX18 = 74;
            for (int l = x; l < x + 2; l++)
            {
                for (int m = y; m < y + 4; m++)
                {
                    if (Main.tile[l, m].HasTile && Main.tile[l, m].TileType == Type)
                    {
                        if (Main.tile[l, m].TileFrameY < tileXX18)
                            Main.tile[l, m].TileFrameY += (short)(tileXX18);
                        else
                            Main.tile[l, m].TileFrameY -= (short)(tileXX18);
                    }
                }
            }
            if (Wiring.running)
            {
                for (int o = 0; o < 2; o++)
                {
                    for (int p = 0; p < 4; p++)
                    {
                        Wiring.SkipWire(x + 0, x + p);
                    }
                }
            }
            NetMessage.SendTileSquare(-1, x, y + 1, 3);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Texture2D texture;
            texture = TextureAssets.Tile[Type].Value;
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }
            int height = 18;
            int animate = 0;
            if (tile.TileFrameY >= 74)
            {
                animate = Main.tileFrame[Type] * AnimationFrameHeight;
            }
            Main.spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY + animate, 16, height), Lighting.GetColor(i, j), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);

            // Draws the Smart Cursor Highlight. Not 100% accurate, but its close enough
            bool actuallySelected;
            Color highlightColor;
            Texture2D texhighlight = ModContent.Request<Texture2D>($"CalamityMod/Tiles/Furniture/Monoliths/{Name}_Highlight").Value;
            if (Main.InSmartCursorHighlightArea(i, j, out actuallySelected))
            {
                if (actuallySelected)
                {
                    highlightColor = new Color(252, 252, 84);
                }
                else
                {
                    highlightColor = new Color(125, 125, 125);
                }
                Main.spriteBatch.Draw(texhighlight, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY + animate, 16, height), highlightColor, 0f, default(Vector2), 1f, SpriteEffects.None, 0f); ;
            }
            return false;
        }
    }
}
