using CalamityMod.Dusts;
using CalamityMod.Items.Placeables.Furniture.Fountains;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Furniture.Fountains
{
    public class AstralFountainTile : ModTile
    {
        public override void SetStaticDefaults() => this.SetUpFountain(ModContent.ItemType<AstralFountainItem>(), new Color(59, 50, 77));

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (Main.tile[i, j].TileFrameX >= 36)
                Main.SceneMetrics.ActiveFountainColor = ModContent.Find<ModWaterStyle>("CalamityMod/AstralWater").Slot;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, ModContent.DustType<AstralBlue>());
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, ModContent.DustType<AstralOrange>());
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter >= 6)
            {
                frame = (frame + 1) % 4;
                frameCounter = 0;
            }
        }

        public override void HitWire(int i, int j)
        {
            CalamityUtils.LightHitWire(Type, i, j, 2, 4);
        }

        public override bool RightClick(int i, int j)
        {
            CalamityUtils.LightHitWire(Type, i, j, 2, 4);
            SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));
            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<AstralFountainItem>();
        }
    }
}
