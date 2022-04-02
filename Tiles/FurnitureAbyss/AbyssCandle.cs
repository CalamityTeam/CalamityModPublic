using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAbyss
{
	public class AbyssCandle : ModTile
    {
        public override void SetDefaults()
        {
			this.SetUpCandle(true, 0);

            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Candle");
            AddMapEntry(new Color(253, 221, 3), name);
            disableSmartCursor = true;
            adjTiles = new int[] { TileID.Candles };
            drop = ModContent.ItemType<Items.Placeables.FurnitureAbyss.AbyssCandle>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(100, 130, 150), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.tile[i, j].frameX < 18)
            {
                r = 0.8f;
                g = 0.9f;
                b = 1f;
            }
            else
            {
                r = 0f;
                g = 0f;
                b = 0f;
            }
        }

        public override void HitWire(int i, int j)
        {
            CalamityUtils.LightHitWire(Type, i, j, 1, 1);
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.showItemIcon = true;
            player.showItemIcon2 = ModContent.ItemType<Items.Placeables.FurnitureAbyss.AbyssCandle>();
        }

        public override bool NewRightClick(int i, int j)
        {
            CalamityUtils.RightClickBreak(i, j);
            return true;
        }
    }
}
