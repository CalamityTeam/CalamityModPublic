using CalamityMod.Projectiles.Boss;
using CalamityMod.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.TileEntities
{
    public class TEDraedonFieldGenerator : ModTileEntity
    {
        public int Time;
        public int ActiveTimer;
        public const int ActiveTimerMax = 45;
        public override bool ValidTile(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            return tile.active() && tile.type == ModContent.TileType<DreadonFactoryFieldGenerator>();
        }
        public override void Update()
        {
            Player player = Main.player[Player.FindClosest(Position.ToVector2() * 16, 1, 1)];
            Vector2 positionWorldCoords = Position.ToVector2() * 16f;
            if (Vector2.Distance(player.Center, positionWorldCoords) < 960f && !player.dead)
            {
                if (ActiveTimer < ActiveTimerMax)
                    ActiveTimer++;
                Time++;

                if (Time % 20 == 19)
                {
                    Projectile projectile = Projectile.NewProjectileDirect(positionWorldCoords, Vector2.Normalize(player.Center - positionWorldCoords) * 5f, ModContent.ProjectileType<DoGNebulaShot>(), 40, 4f);
                    projectile.tileCollide = false;
                }
                if (Time % 75 == 74)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        float angle = i / 9f * MathHelper.TwoPi;
                        Projectile projectile = Projectile.NewProjectileDirect(positionWorldCoords, Vector2.Normalize(player.Center - positionWorldCoords).RotatedBy(angle) * 7f, ModContent.ProjectileType<DoGNebulaShot>(), 40, 4f);
                        projectile.tileCollide = false;
                    }
                }
            }
            else if (ActiveTimer > 0)
            {
                ActiveTimer--;
                ClientToServerSync();
            }
        }
        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 3);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
                return -1;
            }
            return Place(i, j);
        }
        public void ClientToServerSync()
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                var netMessage = CalamityMod.Instance.GetPacket();
                netMessage.Write((byte)CalamityModMessageType.DraedonFieldGeneratorSync);
                netMessage.Write(Main.LocalPlayer.Calamity().CurrentlyViewedCharger.ID);
                netMessage.Write(Time);
                netMessage.Write(ActiveTimer);
                netMessage.Send();
            }
        }
    }
}
