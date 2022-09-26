using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.Items.Tools;
using CalamityMod.Projectiles.Boss;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Audio;
using Terraria.ID;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Projectiles.Typeless;
using System.Linq;

namespace CalamityMod.Tiles.Furniture
{
    public class WulfrumLure : ModTile
    {
        public const int Width = 2;
        public const int Height = 2;

        public Asset<Texture2D> CogTexture;
        public Asset<Texture2D> CoverTexture;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Wulfrum Lure");
            AddMapEntry(new Color(194, 255, 67), name);
            TileID.Sets.DisableSmartCursor[Type] = true;

            DustType = 83;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, Width * 16, Height * 16, ModContent.ItemType<WulfrumLureItem>());
        }

        public override bool RightClick(int i, int j)
        {
            Tile tile = Main.tile[i, j];

            int left = i - tile.TileFrameX / 18;
            int top = j - tile.TileFrameY / 18;

            if (!Main.LocalPlayer.HasItem(ModContent.ItemType<EnergyCore>()))
                return true;

            if (Main.projectile.Any(p => p.active && p.type == ModContent.ProjectileType<WulfrumLureSignal>() && (p.Center - Main.LocalPlayer.Center).Length() < 2000))
                return true;

            Vector2 lurePosition = new Vector2(left + Width / 2, top).ToWorldCoordinates();
            lurePosition += new Vector2(0f, -24f);

            SoundEngine.PlaySound(WulfrumTreasurePinger.ScanBeepSound, lurePosition);
            Projectile.NewProjectile(new EntitySource_WorldEvent(), lurePosition, Vector2.Zero, ModContent.ProjectileType<WulfrumLureSignal>(), 0, 0f, Main.myPlayer);

            Main.LocalPlayer.ConsumeItem(ModContent.ItemType<EnergyCore>(), true);

            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<EnergyCore>();
            Main.LocalPlayer.noThrow = 2;
            Main.LocalPlayer.cursorItemIconEnabled = true;
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (drawData.tileFrameX % (Width * 18) == 0 && drawData.tileFrameY % (Height * 18 ) == 0)
            {
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
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
            {
                return;
            }

            if (CogTexture == null)
                CogTexture = ModContent.Request<Texture2D>("CalamityMod/Tiles/Furniture/WulfrumLureCog");
            Texture2D cogTexture = CogTexture.Value;

            if (CoverTexture == null)
                CoverTexture = ModContent.Request<Texture2D>("CalamityMod/Tiles/Furniture/WulfrumLureCover");
            Texture2D coverTex = CoverTexture.Value;

            Vector2 worldPos = p.ToWorldCoordinates(16f, 16f);

            Color color = Lighting.GetColor(p.X, p.Y);

            bool direction = tile.TileFrameY / (Height * 16) != 0;

            SpriteEffects effects = direction ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 drawPos = worldPos + offScreen - Main.screenPosition;
            spriteBatch.Draw(cogTexture, drawPos, null, color, Main.GlobalTimeWrappedHourly * 1.5f * (direction ? -1 : 1), cogTexture.Size() / 2f, 1f, effects, 0f);

            spriteBatch.Draw(coverTex, p.ToWorldCoordinates(0f, 0f) + Vector2.UnitY * 2 + offScreen - Main.screenPosition, null, color, 0f, Vector2.Zero, 1f, effects, 0f);
        }
    }
}
