using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.SunkenSea
{
    public class AlgalPrismTorch : ModTile
    {
        public override void SetStaticDefaults() => this.SetUpTorch(ModContent.ItemType<Items.Placeables.Furniture.AlgalPrismTorch>(), true, true);

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(150, 255, 190), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<Items.Placeables.Furniture.AlgalPrismTorch>();
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX < 66)
            {
                r = 0.7f;
                g = 1.2f;
                b = 0.9f;
            }
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = 0;
            if (WorldGen.SolidTile(i, j - 1))
            {
                offsetY = 2;
                if (WorldGen.SolidTile(i - 1, j + 1) || WorldGen.SolidTile(i + 1, j + 1))
                    offsetY = 4;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            CalamityUtils.DrawFlameEffect(ModContent.Request<Texture2D>("CalamityMod/Tiles/SunkenSea/AlgalPrismTorchFlame").Value, i, j, 2);
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (Main.tile[i, j].TileFrameX < 66)
                CalamityUtils.DrawFlameSparks(61, 5, i, j);
        }

        public override bool RightClick(int i, int j)
        {
            CalamityUtils.RightClickBreak(i, j);
            return true;
        }

		public override float GetTorchLuck(Player player)
		{
			// Note: Total Torch luck never goes below zero
			return player.Calamity().ZoneSunkenSea ? 1f : -1f;
		}
    }
}
