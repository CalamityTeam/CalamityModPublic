using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using System;

namespace CalamityMod.Tiles
{
    //TODO: this needs animation, will do that later today
    public class GiantPlanteraBulb : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileObjectData.newTile.Width = 5;
            TileObjectData.newTile.Height = 5;
            TileObjectData.newTile.Origin = new Point16(2, 4);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AnimationFrameHeight = 90;
            AddMapEntry(Main.hardMode ? new Color(243, 82, 171) : new Color(107, 125, 33));
            DustType = DustID.JungleGrass;

            base.SetStaticDefaults();
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 2;
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
			return Main.hardMode;
        }

        public override bool CanExplode(int i, int j)
        {
			return Main.hardMode;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            float x = i * 16;
            float y = j * 16;
            float distanceFromPlayer = -1f;
            int player = 0;
            for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
            {
                float dist = Math.Abs(Main.player[playerIndex].position.X - x) + Math.Abs(Main.player[playerIndex].position.Y - y);
                if (dist < distanceFromPlayer || distanceFromPlayer == -1f)
                {
                    player = playerIndex;
                    distanceFromPlayer = dist;
                }
            }

            // Spawn Plantera if the bulb was broken within a distance of 50 tiles or less
            if (distanceFromPlayer / 16f < 50f)
                NPC.SpawnOnPlayer(player, NPCID.Plantera);
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
		{
            if (Main.hardMode)
            {
                frameCounter++;
                if (frameCounter > 25)
                {
                    frameCounter = 0;
                    frame++;
                    if (frame > 6)
                    {
                        frame = 1;
                    }
                }
            }
            else
            {
                frame = 0;
            }
		}

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.hardMode)
            {
                r = 243f / 500f;
                g = 82f / 500f;
                b = 171f / 500f;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
			Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Tiles/GiantPlanteraBulbGlow").Value;
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

			spriteBatch.Draw(tex, new Vector2(i * 16, j * 16 + 2) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.Yellow);
        }
    }
}
