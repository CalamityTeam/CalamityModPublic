using CalamityMod.Items.Placeables.FurnitureSacrilegious;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.FurnitureSacrilegious
{
    public class MonolithOfTheAccursedTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[]
            {
                16,
                16,
				16
            };
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            TileID.Sets.HasOutlines[Type] = true;
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Monolith of the Accursed");
            AddMapEntry(new Color(43, 19, 42), name);
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 60, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 8, 0f, 0f, 1, new Color(100, 100, 100), 1f);
            return false;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.tile[i, j].TileFrameX < 27)
            {
				r = 1.2f;
				g = 0.2f;
				b = 0.2f;
            }
            else
            {
                r = 0f;
                g = 0f;
                b = 0f;
            }
        }

        public override bool RightClick(int i, int j)
		{
			CalamityUtils.LightHitWire(Type, i, j, 2, 3);
			return true;
		}

        public override void HitWire(int i, int j) => CalamityUtils.LightHitWire(Type, i, j, 2, 3);

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (Main.tile[i, j].TileFrameX > 27)
				return;

            Player player = Main.LocalPlayer;
            if (player is null)
                return;
            if (player.active)
                player.Calamity().monolithAccursedShader = 10;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 32, ModContent.ItemType<MonolithOfTheAccursed>());
        }

        public override void MouseOver(int i, int j) => CalamityUtils.MouseOver(i, j, ModContent.ItemType<MonolithOfTheAccursed>());

		// For drawing the floating icon
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Main.tile[i, j].TileFrameX > 27)
				return;

			Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Tiles/FurnitureSacrilegious/MonolithOfTheAccursedTile_Icon").Value;
            Tile tile = Main.tile[i, j];
            int xPos = tile.TileFrameX;
            int yPos = tile.TileFrameY;

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            float yOffset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi / 5f) * 2f;
			Vector2 correction = new Vector2(16f , -10f);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y + yOffset) + zero + correction;

			Rectangle rect = new Rectangle(xPos, yPos, texture.Width, texture.Height);
			Color color = new Color(100, 100, 100, 0);
            Vector2 origin = rect.Size() / 2f;

            for (int c = 0; c < 5; c++)
            {
				spriteBatch.Draw(texture, drawOffset, rect, color, 0f, origin, 1f, SpriteEffects.None, 0f);
			}
		}
    }
}
