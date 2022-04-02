using CalamityMod.Items.Placeables.Furniture.Fountains;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Furniture.Fountains
{
    public class SunkenSeaFountainTile : ModTile
    {
        public override void SetDefaults()
        {
            this.SetUpFountain();
            AddMapEntry(new Color(104, 195, 255), Language.GetText("MapObject.WaterFountain"));
            animationFrameHeight = 72;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (Main.tile[i, j].frameX < 36)
            {
                if (CalamityGlobalTile.WaterStyles.Any((style) => style.Name == "SunkenSeaWater"))
                {
                    Main.fountainColor = CalamityGlobalTile.WaterStyles.FirstOrDefault((style) => style.Name == "SunkenSeaWater").Type;
                }
            }
        }

        public override bool HasSmartInteract()
        {
            return true;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(119, 102, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 33, 0f, 0f, 1, new Color(255, 255, 255), 1f);
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

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 16, 32, ModContent.ItemType<SunkenSeaFountain>());
        }

        public override void HitWire(int i, int j)
        {
            CalamityUtils.LightHitWire(Type, i, j, 2, 4);
        }

        public override bool NewRightClick(int i, int j)
        {
            CalamityUtils.LightHitWire(Type, i, j, 2, 4);
            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.showItemIcon = true;
            player.showItemIcon2 = ModContent.ItemType<SunkenSeaFountain>();
        }
    }
}
