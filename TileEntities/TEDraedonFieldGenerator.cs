using CalamityMod.Projectiles.Enemy;
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
        public const float Radius = 660f;
        public override bool ValidTile(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            return tile.active() && tile.type == ModContent.TileType<DraedonFactoryFieldGenerator>();
        }
        public override void Update()
        {
            Player player = Main.player[Player.FindClosest(Position.ToVector2() * 16, 1, 1)];
            Vector2 positionWorldCoords = Position.ToVector2() * 16f;
            if (Vector2.Distance(player.Center, positionWorldCoords) < Radius && !player.dead)
            {
                if (ActiveTimer < ActiveTimerMax)
                    ActiveTimer++;
                Time++;

                if (Time % 75 == 74)
                {
                    float speed = 5f;
                    Vector2 aimOffset = (player.velocity + player.oldVelocity) * 0.5f * player.Distance(positionWorldCoords) / (speed * speed);
                    Vector2 velocity = Vector2.Normalize(player.Center - positionWorldCoords + aimOffset) * speed;
                    Projectile.NewProjectileDirect(positionWorldCoords, velocity, ModContent.ProjectileType<DraedonLaser>(), 27, 4f);
                }
                if (Time % 150 == 149)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        float angle = i / 9f * MathHelper.TwoPi;
                        Projectile.NewProjectileDirect(positionWorldCoords, Vector2.Normalize(player.Center - positionWorldCoords).RotatedBy(angle) * 7f, ModContent.ProjectileType<DraedonLaser>(), 27, 4f);
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
