using CalamityMod.Buffs.Placeables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Furniture
{
    public class ChaosCandle : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpCandle();
            ItemDrop = ModContent.ItemType<Items.Placeables.Furniture.ChaosCandle>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Chaos Candle");
            AddMapEntry(new Color(238, 145, 105), name);
            AdjTiles = new int[] { TileID.Candles };
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.showItemIcon = true;
            player.showItemIcon2 = ModContent.ItemType<Items.Placeables.Furniture.ChaosCandle>();
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            if (player is null)
                return;
            if (!player.dead && player.active)
                player.AddBuff(ModContent.BuffType<ChaosCandleBuff>(), 20);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.tile[i, j].TileFrameX < 18)
            {
                r = 0.85f;
                g = 0.25f;
                b = 0.25f;
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
                CalamityUtils.DrawFlameSparks(235, 5, i, j);
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            CalamityUtils.DrawFlameEffect(ModContent.Request<Texture2D>("CalamityMod/Tiles/Furniture/ChaosCandleFlame").Value, i, j);
        }

        public override bool RightClick(int i, int j)
        {
            CalamityUtils.RightClickBreak(i, j);
            return true;
        }
    }
}
