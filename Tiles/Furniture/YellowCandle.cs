using CalamityMod.Buffs.Placeables;
using CalamityMod.Items.Placeables.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Furniture
{
    public class YellowCandle : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.addTile(Type);
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AddMapEntry(new Color(238, 145, 105), CalamityUtils.GetItemName<SpitefulCandle>());
            AnimationFrameHeight = 18;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter >= 6)
            {
                frame = (frame + 1) % 5;
                frameCounter = 0;
            }
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            if (player == null || !player.active || player.dead)
                return;
            player.AddBuff(ModContent.BuffType<CirrusYellowCandleBuff>(), 20);

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            // All NPCs within a certain distance of the player get "Tagged" with Spite.
            for (int m = 0; m < Main.maxNPCs; m++)
            {
                NPC npc = Main.npc[m];
                if (npc is null || !npc.active || npc.friendly)
                    continue;

                float dist2 = npc.DistanceSQ(player.Center);
                if (dist2 < 23040000f) // 4800px range
                    Main.npc[m].AddBuff(ModContent.BuffType<CirrusYellowCandleBuff>(), 20, false);
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.75f;
            g = 0.75f;
            b = 0.35f;
        }
    }
}
