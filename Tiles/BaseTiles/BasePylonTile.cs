using System;
using CalamityMod.TileEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;

namespace CalamityMod.Tiles.BaseTiles
{
    public abstract class BasePylonTile : ModPylon
    {
        public const int CrystalHorizontalFrameCount = 1;
        public const int CrystalVerticalFrameCount = 8;
        public const int CrystalFrameHeight = 64;

        public Asset<Texture2D> crystalTexture;
        public Asset<Texture2D> mapIcon;

        /// <summary>
        /// The type of dust created by the pylon
        /// </summary>
        public virtual int DustID => 43;
        public virtual Color DustColor => Color.White;
        public virtual Color LightColor => Color.White;
        public abstract int AssociatedItem { get; }
        public abstract Color PylonMapColor { get; }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                // Pre-loading textures
                crystalTexture = ModContent.Request<Texture2D>(Texture + "_Crystal");
                mapIcon = ModContent.Request<Texture2D>(Texture + "_MapIcon");
            }
        }

        public override void Unload()
        {
            crystalTexture = null;
            mapIcon = null;
        }

        public override void SetStaticDefaults()
        {
            RegisterItemDrop(AssociatedItem);

            TEModdedPylon moddedPylon = ModContent.GetInstance<TECalamityPylon>();
            this.SetUpPylon(moddedPylon, true);

            TileID.Sets.InteractibleByNPCs[Type] = true;
            TileID.Sets.PreventsSandfall[Type] = true;

            AddMapEntry(PylonMapColor, CalamityUtils.GetItemName(AssociatedItem));
        }

        public override void MouseOver(int i, int j)
        {
            // Uses the pylon item as an icon when hovering over it.
            Main.LocalPlayer.cursorItemIconEnabled = true;
            Main.LocalPlayer.cursorItemIconID = AssociatedItem;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<TECalamityPylon>().Kill(i, j);
        }

        public override bool CreateDust(int i, int j, ref int type) => false;
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX < 66)
            {
                r = LightColor.R / 255f;
                g = LightColor.G / 255f;
                b = LightColor.B / 255f;
            }
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Vector2 offScreen = new Vector2(Main.offScreenRange);
            if (Main.drawToScreen)
            {
                offScreen = Vector2.Zero;
            }

            Point p = new Point(i, j);
            Tile tile = Main.tile[p.X, p.Y];
            if (tile == null || !tile.HasTile)
                return;

            Texture2D crystalTex = crystalTexture.Value;

            int frameY = (Main.tileFrameCounter[597] + p.X + p.Y) % CrystalFrameHeight / CrystalVerticalFrameCount;
            Rectangle frame = crystalTexture.Frame(CrystalHorizontalFrameCount, CrystalVerticalFrameCount, 0, frameY);
            Vector2 origin = frame.Size() / 2f;

            // Used to make the crystal bob, and the 6-way glow pulse a bit.
            float sineOffset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi / 5f);

            Vector2 tilePosition = p.ToWorldCoordinates(24f, 64f);
            Vector2 drawCenter = tilePosition + offScreen + Vector2.UnitY * (-40f + sineOffset * 4f);
            drawCenter -= Vector2.One; // This isn't vanilla, but since our sheets lack the extra side 2 pixels, it makes our pylons offset by one pixel in each direction (imperceptible). This makes our pylons 100% aligned with vanilla.

            // Vanilla just has a nextbool(4) but thats because vanilla doesnt care about the update rate. We do this because in the updateEveryFrame lightning mode, the game runs the draw calls 4x as often.
            bool frameToSpawnDust = !Lighting.UpdateEveryFrame || Main.rand.NextBool(4);
            if (!Main.gamePaused && Main.instance.IsActive && frameToSpawnDust && Main.rand.NextBool(10))
            {
                //Important to remember to remove the offscreen vector. Vanilla drawing doesn't have this vector, but we do. If we don't remove it, the dust just gets spawned offscreen.
                Rectangle dustBox = Utils.CenteredRectangle(drawCenter - offScreen, frame.Size());

                int dust = Dust.NewDust(dustBox.TopLeft(), dustBox.Width, dustBox.Height, DustID, 0f, 0f, 254, DustColor, 0.5f);
                Main.dust[dust].velocity *= 0.1f;
                Main.dust[dust].velocity.Y -= 0.2f;
            }

            // Crystal is 80% fullbright.
            Color crystalColor = Color.Lerp(Lighting.GetColor(p.X, p.Y), Color.White, 0.8f);
            spriteBatch.Draw(crystalTex, drawCenter - Main.screenPosition, frame, crystalColor * 0.7f, 0f, origin, 1f, 0f, 0f);

            // Draw a 6-way glowing effect.
            float glowOpacity = (float)Math.Sin((double)Main.GlobalTimeWrappedHourly * MathHelper.TwoPi) * 0.2f + 0.8f;
            Color glowColor = new Color(255, 255, 255, 0) * 0.1f * glowOpacity;
            float oneSixth = 1f / 6f;
            float offset = 6f + sineOffset * 2f;
            for (float k = 0f; k < 1f; k += oneSixth)
                spriteBatch.Draw(crystalTex, drawCenter - Main.screenPosition + (MathHelper.TwoPi * k).ToRotationVector2() * offset, frame, glowColor, 0f, origin, 1f, 0f, 0f);

            int tileSelectionTier = 0;
            if (Main.InSmartCursorHighlightArea(p.X, p.Y, out var actuallySelected))
            {
                tileSelectionTier = 1;
                if (actuallySelected)
                    tileSelectionTier = 2;
            }

            // Draw the selection glow.
            if (tileSelectionTier != 0)
            {
                int averageBrightness = (crystalColor.R + crystalColor.G + crystalColor.B) / 3;
                if (averageBrightness > 10)
                {
                    // Use the vanilla crystal sheet to get the autoselect outline.
                    Texture2D vanillaCrystalSheet = TextureAssets.Extra[181].Value;
                    Rectangle smartCursorGlowFrame = vanillaCrystalSheet.Frame(12, 8, 2, frameY);

                    Color selectionGlowColor = Colors.GetSelectionGlowColor(tileSelectionTier == 2, averageBrightness);
                    spriteBatch.Draw(vanillaCrystalSheet, drawCenter - Main.screenPosition, smartCursorGlowFrame, selectionGlowColor, 0f, origin, 1f, SpriteEffects.None, 0f);
                }
            }
        }

        public override void DrawMapIcon(ref MapOverlayDrawContext context, ref string mouseOverText, TeleportPylonInfo pylonInfo, bool isNearPylon, Color drawColor, float deselectedScale, float selectedScale)
        {
            // Just like in SpecialDraw, we want things to be handled the EXACT same way vanilla would handle it, which ModPylon also has built in methods for:
            bool mouseOver = DefaultDrawMapIcon(ref context, mapIcon, pylonInfo.PositionInTiles.ToVector2() + new Vector2(1.5f, 2f), drawColor, deselectedScale, selectedScale);
            DefaultMapClickHandle(mouseOver, pylonInfo, Lang.GetItemName(AssociatedItem).Key, ref mouseOverText);
        }
    }
}
