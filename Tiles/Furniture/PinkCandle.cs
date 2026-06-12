using CalamityMod.Buffs.Placeables;
using CalamityMod.Items.Placeables.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Furniture
{
    public class PinkCandle : ModTile
    {
        // TODO -- Unique sounds for each Candle.
        public static readonly SoundStyle ActivationSound = new("CalamityMod/Sounds/Item/LouderPhantomPhoenix2");

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.addTile(Type);
            AdjTiles = new int[] { TileID.Candles };
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AddMapEntry(new Color(238, 145, 105), CalamityUtils.GetItemName<VigorousCandle>());
            TileID.Sets.HasOutlines[Type] = true;
            AnimationFrameHeight = 18;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool RightClick(int i, int j)
        {
            Player p = Main.LocalPlayer;

            // Forcibly remove all candle buffs.
            p.ClearBuff(ModContent.BuffType<BlueCandleBuff>());
            p.ClearBuff(ModContent.BuffType<PurpleCandleBuff>());
            p.ClearBuff(ModContent.BuffType<PinkCandleBuff>());
            p.ClearBuff(ModContent.BuffType<YellowCandleBuff>());

            // 108000 is the duration used by Ammo Box.
            p.AddBuff(ModContent.BuffType<PinkCandleBuff>(), 108000);

            // Play a sound.
            SoundEngine.PlaySound(ActivationSound, new Vector2(i * 16, j * 16));

            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<VigorousCandle>();
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter >= 6)
            {
                frame = (frame + 1) % 10;
                frameCounter = 0;
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.75f;
            g = 0.35f;
            b = 0.65f;
        }
    }
}
