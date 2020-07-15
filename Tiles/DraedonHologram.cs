using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ObjectData;
using Terraria.DataStructures;

namespace CalamityMod.Tiles
{
    public class DraedonHologram : ModTile
    {
        public bool CloseToPlayer = false;
        public const int Width = 6;
        public const int Height = 7;
        public const int IdleFrames = 7;
        public const int TalkingFrames = 8;
        public const int FrameCount = IdleFrames + TalkingFrames;
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);
            TileObjectData.newTile.Height = 7;
            TileObjectData.newTile.Origin = new Point16(2, 6);
            TileObjectData.newTile.CoordinateHeights = new int[TileObjectData.newTile.Height];
            TileObjectData.newTile.CoordinatePadding = 0;
            for (int i = 0; i < TileObjectData.newTile.CoordinateHeights.Length; i++)
            {
                TileObjectData.newTile.CoordinateHeights[i] = 16;
            }
            TileObjectData.addTile(Type);
            animationFrameHeight = 112;

            soundType = SoundID.Tink;
            minPick = 100;
            AddMapEntry(new Color(99, 131, 199));
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            CloseToPlayer = closer;
        }
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (frameCounter++ % 6 == 5)
            {
                frame++;
                if (frame >= (!CloseToPlayer ? 5 : FrameCount))
                {
                    if (!CloseToPlayer && frame > 7)
                        frame = 6;
                    else
                        frame = !CloseToPlayer ? 0 : TalkingFrames;
                }
            }
        }
        public override bool NewRightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            if (player.Calamity().CurrentlyViewedHologramX == -1 ||
                player.Calamity().CurrentlyViewedHologramY == -1)
            {
                Main.PlaySound(SoundID.Chat, -1, -1, 1, 1f, 0f);
                player.Calamity().CurrentlyViewedHologramX = i;
                player.Calamity().CurrentlyViewedHologramY = j;
                player.talkNPC = -1;
            }
            else
            {
                player.Calamity().CurrentlyViewedHologramX = player.Calamity().CurrentlyViewedHologramY = -1;
            }
            return true;
        }
    }
}
