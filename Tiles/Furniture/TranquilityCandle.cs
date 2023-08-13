using CalamityMod.Buffs.Placeables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Furniture
{
    public class TranquilityCandle : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpCandle(ModContent.ItemType<Items.Placeables.Furniture.TranquilityCandle>(), false, false);
            AddMapEntry(new Color(238, 145, 105), CalamityUtils.GetItemName<Items.Placeables.Furniture.TranquilityCandle>());
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<Items.Placeables.Furniture.TranquilityCandle>();
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            if (player is null)
                return;
            if (!player.dead && player.active && Main.tile[i, j].TileFrameX < 18)
                player.AddBuff(ModContent.BuffType<TranquilityCandleBuff>(), 20);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.tile[i, j].TileFrameX < 18)
            {
                r = 1f;
                g = 0.55f;
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

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (Main.tile[i, j].TileFrameX < 18)
                CalamityUtils.DrawFlameSparks(62, 5, i, j);
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            CalamityUtils.DrawFlameEffect(ModContent.Request<Texture2D>("CalamityMod/Tiles/Furniture/TranquilityCandleFlame").Value, i, j);
        }

        public override bool RightClick(int i, int j)
        {
            CalamityUtils.LightHitWire(Type, i, j, 1, 1);
            return true;
        }
    }
}
