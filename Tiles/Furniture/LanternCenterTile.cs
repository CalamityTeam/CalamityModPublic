using CalamityMod.Items.Placeables.Furniture;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Furniture
{
    public class LanternCenterTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 18 };
            TileObjectData.newTile.StyleLineSkip = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(99, 99, 99), CalamityUtils.GetItemName<LanternCenter>());
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.InteractibleByNPCs[Type] = true;
            TileID.Sets.DisableSmartInteract[Type] = true;

            AnimationFrameHeight = 54;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool CreateDust(int i, int j, ref int type) => false;

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (!LanternNight.LanternsUp)
            {
                frame = 0;
                frameCounter = 0;
            }
            else
            {
                frameCounter++;
                if (frameCounter >= 6)
                {
                    frame = (frame + 1) % 7;
                    frameCounter = 0;
                }
                frame = Math.Clamp(frame, 1, 6);
            }
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            LanternNight.ManualLanterns = false;
        }

        public override void HitWire(int i, int j)
        {
            LanternNight.ToggleManualLanterns();
        }

        public override bool RightClick(int i, int j)
        {
            LanternNight.ToggleManualLanterns();
            SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));
            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<LanternCenter>();
        }
    }
}
