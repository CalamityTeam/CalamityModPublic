using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureEutrophic
{
    public class EutrophicClock : ModTile
    {
        public override void SetStaticDefaults() => this.SetUpClock(ModContent.ItemType<Items.Placeables.FurnitureEutrophic.EutrophicClock>());

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 51, 0f, 0f, 1, new Color(54, 69, 72), 1f);
            return false;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.tile[i, j].TileFrameX < 18)
            {
                r = 0.5f;
                g = 1f;
                b = 1f;
            }
            else
            {
                r = 0f;
                g = 0f;
                b = 0f;
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool RightClick(int x, int y)
        {
            return CalamityUtils.ClockRightClick();
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer)
            {
                Main.SceneMetrics.HasClock = true;
            }
        }

        public override void MouseOver(int i, int j) => CalamityUtils.MouseOver(i, j, ModContent.ItemType<Items.Placeables.FurnitureEutrophic.EutrophicClock>());
    }
}
