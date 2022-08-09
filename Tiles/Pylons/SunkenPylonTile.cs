using CalamityMod.Systems;
using CalamityMod.TileEntities;
using CalamityMod.Items.Placeables.Pylons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;

namespace CalamityMod.Tiles.Pylons
{

    public class SunkenPylonTile : ModPylon
    {
        public const int CrystalHorizontalFrameCount = 1;
        public const int CrystalVerticalFrameCount = 8;
        public const int CrystalFrameHeight = 64;

        public Asset<Texture2D> crystalTexture;
        public Asset<Texture2D> mapIcon;

        public override void Load()
        {
            // Pre-loading textures
            crystalTexture = ModContent.Request<Texture2D>(Texture + "_Crystal");
            mapIcon = ModContent.Request<Texture2D>(Texture + "_MapIcon");
        }

        public override void SetStaticDefaults()
        {
            TEModdedPylon moddedPylon = ModContent.GetInstance<TECalamityPylon>();
            this.SetUpPylon(moddedPylon, true);

            TileID.Sets.InteractibleByNPCs[Type] = true;
            TileID.Sets.PreventsSandfall[Type] = true;

            ModTranslation pylonName = CreateMapEntryName();
            AddMapEntry(Color.White, pylonName);
        }

        public override int? IsPylonForSale(int npcType, Player player, bool isNPCHappyEnough)
        {
            // Pylon will sell regardless of NPC happiness.
            // TODO -- It would probably be nice to have pylons sell dependent on NPC happiness, but not a huge priority.
            return player.Calamity().ZoneSunkenSea ? ModContent.ItemType<SunkenPylon>() : null;
        }


        public override void MouseOver(int i, int j)
        {
            // Uses the pylon item as an icon when hovering over it.
            Main.LocalPlayer.cursorItemIconEnabled = true;
            Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<SunkenPylon>();
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<TECalamityPylon>().Kill(i, j);
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 2, 3, ModContent.ItemType<SunkenPylon>());
        }

        public override bool ValidTeleportCheck_BiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData)
        {
            // Copied from ExampleMod for documentation if needed later -pixl
            //
            // Right before this hook is called, the sceneData parameter exports its information based on wherever the destination pylon is,
            // and by extension, it will call ALL ModSystems that use the TileCountsAvailable method. This means, that if you determine biomes
            // based off of tile count, when this hook is called, you can simply check the tile threshold.

            return BiomeTileCounterSystem.SunkenSeaTiles >= 100;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            return false;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;

            if (player is null)
                return;
            if (!player.dead && player.active)
                // Grants underwater breathing
                player.gills = true;

        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX < 66)
            {
                r = 0.2f;
                g = 0.8f;
                b = 1f;
            }
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            // We want to draw the pylon crystal the exact same way vanilla does, so we can use this built in method in ModPylon for default crystal drawing:
            // DefaultDrawPylonCrystal(spriteBatch, i, j, crystalTexture, Color.White, CrystalFrameHeight, CrystalHorizontalFrameCount, CrystalVerticalFrameCount);

            // TODO -- This currently just rips the above method's code to fix things like dust particles. Please replace the below code and re-enable the method call once tML fixes their oversights.

            Vector2 vector = new Vector2(Main.offScreenRange);
            if (Main.drawToScreen)
            {
                vector = Vector2.Zero;
            }

            Point p = new Point(i, j);
            Tile tile = Main.tile[p.X, p.Y];
            if (tile == null || !tile.HasTile)
            {
                return;
            }

            TileObjectData tileData = TileObjectData.GetTileData(tile);
            Texture2D value = TextureAssets.Extra[181].Value;
            int frameY = (Main.tileFrameCounter[597] + p.X + p.Y) % CrystalFrameHeight / CrystalVerticalFrameCount;
            Rectangle rectangle = crystalTexture.Frame(CrystalHorizontalFrameCount, CrystalVerticalFrameCount, 0, frameY);
            Rectangle value2 = crystalTexture.Frame(CrystalHorizontalFrameCount, CrystalVerticalFrameCount, 1, frameY);
            value.Frame(CrystalHorizontalFrameCount, CrystalVerticalFrameCount, 0, frameY);
            Vector2 origin = rectangle.Size() / 2f;
            Vector2 vector2 = p.ToWorldCoordinates(0f, 0f) + new Vector2((float)tileData.Width / 2f * 16f, ((float)tileData.Height / 2f + 1.5f) * 16f);
            float num = (float)Math.Sin(Main.GlobalTimeWrappedHourly * (MathF.PI * 2f) / 5f);
            Vector2 vector3 = vector2 + vector + new Vector2(0f, -40f) + new Vector2(0f, num * 4f);

            if (!Main.gamePaused && Main.instance.IsActive && Main.rand.NextBool(40))
            {
                Rectangle r = Utils.CenteredRectangle(vector3, rectangle.Size());
                int num2 = Dust.NewDust(r.TopLeft(), r.Width, r.Height, 400, 0f, 0f, 254, Color.White, 0.5f);
                Main.dust[num2].velocity *= 0.1f;
                Main.dust[num2].velocity.Y -= 0.2f;
            }

            Color color = Color.Lerp(Lighting.GetColor(p.X, p.Y), Color.White, 0.8f);
            spriteBatch.Draw(crystalTexture.Value, vector3 - Main.screenPosition, rectangle, color * 0.7f, 0f, origin, 1f, SpriteEffects.None, 0f);
            float num3 = (float)Math.Sin((double)Main.GlobalTimeWrappedHourly * (Math.PI * 2.0) / 1.0) * 0.2f + 0.8f;
            Color color2 = new Color(255, 255, 255, 0) * 0.1f * num3;
            for (float num4 = 0f; num4 < 1f; num4 += 355f / (678f * MathF.PI))
            {
                spriteBatch.Draw(crystalTexture.Value, vector3 - Main.screenPosition + (MathF.PI * 2f * num4).ToRotationVector2() * (6f + num * 2f), rectangle, color2, 0f, origin, 1f, SpriteEffects.None, 0f);
            }

            int num5 = 0;
            if (Main.InSmartCursorHighlightArea(p.X, p.Y, out var actuallySelected))
            {
                num5 = 1;
                if (actuallySelected)
                {
                    num5 = 2;
                }
            }

            if (num5 != 0)
            {
                int num6 = (color.R + color.G + color.B) / 3;
                if (num6 > 10)
                {
                    Color selectionGlowColor = Colors.GetSelectionGlowColor(num5 == 2, num6);
                    spriteBatch.Draw(crystalTexture.Value, vector3 - Main.screenPosition, value2, selectionGlowColor, 0f, origin, 1f, SpriteEffects.None, 0f);
                }
            }
        }

        public override void DrawMapIcon(ref MapOverlayDrawContext context, ref string mouseOverText, TeleportPylonInfo pylonInfo, bool isNearPylon, Color drawColor, float deselectedScale, float selectedScale)
        {
            // Just like in SpecialDraw, we want things to be handled the EXACT same way vanilla would handle it, which ModPylon also has built in methods for:
            bool mouseOver = DefaultDrawMapIcon(ref context, mapIcon, pylonInfo.PositionInTiles.ToVector2() + new Vector2(1.5f, 2f), drawColor, deselectedScale, selectedScale);
            DefaultMapClickHandle(mouseOver, pylonInfo, "Mods.CalamityMod.ItemName.SunkenPylon", ref mouseOverText);
        }
    }
}
