using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Mounts
{
    class OnyxExcavator : ModMountData
    {
        public override void SetDefaults()
        {
            mountData.spawnDust = 109;
            mountData.spawnDustNoGravity = true;
            mountData.buff = mod.BuffType("OnyxExcavatorBuff");
            mountData.heightBoost = 10;
            mountData.fallDamage = 0f;
            mountData.runSpeed = 8f;
            mountData.flightTimeMax = 0;
            mountData.jumpHeight = 5;
            mountData.acceleration = 0.2f;
            mountData.jumpSpeed = 3f;
            mountData.swimSpeed = 0.5f;
            mountData.totalFrames = 8;
            int[] array = new int[mountData.totalFrames];
            for (int l = 0; l < array.Length; l++)
            {
                array[l] = 2;
            }
            array[1] = 0;
            array[5] = 0;
            mountData.playerYOffsets = array;
            mountData.xOffset = 0; //-6
            mountData.bodyFrame = 3;
            mountData.yOffset = -1; //done
            mountData.playerHeadOffset = 10;
            mountData.standingFrameCount = 1;
            mountData.standingFrameDelay = 12;
            mountData.standingFrameStart = 0;
            mountData.runningFrameCount = 8;
            mountData.runningFrameDelay = 36; //36
            mountData.runningFrameStart = mountData.standingFrameStart;
            mountData.inAirFrameCount = mountData.standingFrameCount;
            mountData.inAirFrameDelay = mountData.standingFrameDelay;
            mountData.inAirFrameStart = mountData.standingFrameStart;
            mountData.idleFrameCount = mountData.standingFrameCount;
            mountData.idleFrameDelay = mountData.standingFrameDelay;
            mountData.idleFrameStart = mountData.standingFrameStart;
            mountData.idleFrameLoop = false;
            mountData.swimFrameCount = mountData.inAirFrameCount;
            mountData.swimFrameDelay = mountData.inAirFrameDelay;
            mountData.swimFrameStart = mountData.inAirFrameStart;
            if (Main.netMode != NetmodeID.Server)
            {
                mountData.backTextureExtra = mod.GetTexture("Items/Mounts/OnyxExcavatorExtra");
                mountData.frontTextureExtra = mod.GetTexture("Items/Mounts/OnyxExcavatorExtra2");
                mountData.textureWidth = mountData.backTexture.Width;
                mountData.textureHeight = mountData.backTexture.Height;
            }
        }

        public override bool UpdateFrame(Player mountedPlayer, int state, Vector2 velocity)
        {
            bool speed = Math.Abs(velocity.X) > mountedPlayer.mount.RunSpeed / 2f;
            float num10 = (float)Math.Sign(mountedPlayer.velocity.X);
            Lighting.AddLight(mountedPlayer.Center, 0.5f, 0.5f, 0.4f);
            if (speed && velocity.Y == 0f)
            {
                for (int i = 0; i < 2; i++)
                {
                    Dust expr_631 = Main.dust[Dust.NewDust(mountedPlayer.BottomLeft, mountedPlayer.width, 6, 192, 0f, 0f, 0, default, 1f)];
                    expr_631.velocity = new Vector2(velocity.X * 0.15f, Main.rand.NextFloat() * -2f);
                    expr_631.noLight = true;
                    expr_631.scale = 0.2f + Main.rand.NextFloat() * 0.8f;
                    expr_631.fadeIn = 0.5f + Main.rand.NextFloat() * 1f;
                    expr_631.shader = GameShaders.Armor.GetSecondaryShader(mountedPlayer.cMount, mountedPlayer);
                }
                if (mountedPlayer.cMount == 0)
                {
                    mountedPlayer.position += new Vector2(num10 * 24f, 0f);
                    mountedPlayer.FloorVisuals(true);
                    mountedPlayer.position -= new Vector2(num10 * 24f, 0f);
                }
            }
            return true;
        }

        public override void UseAbility(Player player, Vector2 mousePosition, bool toggleOn)
        {
            if (Main.mouseLeft && !player.mouseInterface && !Main.blockMouse)
            {
                if (Main.myPlayer == player.whoAmI)
                {
                    float num10 = (float)Math.Sign(player.velocity.X);
                    if ((double)Math.Abs(num10) < 0.1f)
                        num10 = (player.direction == 1 ? 0.1f : -0.1f);
                    float num11 = 12f;
                    float num12 = 40f;
                    Vector2 value2 = player.Center + new Vector2(num10 * num12, num11);
                    int num814 = 3;
                    int num815 = (int)(value2.X / 16f - (float)num814);
                    int num816 = (int)(value2.X / 16f + (float)num814);
                    int num817 = (int)(value2.Y / 16f - (float)num814);
                    int num818 = (int)(value2.Y / 16f + (float)num814);
                    if (num815 < 0)
                    {
                        num815 = 0;
                    }
                    if (num816 > Main.maxTilesX)
                    {
                        num816 = Main.maxTilesX;
                    }
                    if (num817 < 0)
                    {
                        num817 = 0;
                    }
                    if (num818 > Main.maxTilesY)
                    {
                        num818 = Main.maxTilesY;
                    }
                    int tileValueLimit = 600 +
                        (NPC.downedMechBossAny ? 100 : 0) +
                        (NPC.downedPlantBoss ? 100 : 0);
                    AchievementsHelper.CurrentlyMining = true;
                    for (int num824 = num815; num824 <= num816; num824++)
                    {
                        for (int num825 = num817; num825 <= num818; num825++)
                        {
                            value2.Y -= 2f;
                            float num826 = Math.Abs((float)num824 - value2.X / 16f);
                            float num827 = Math.Abs((float)num825 - value2.Y / 16f);
                            double num828 = Math.Sqrt((double)(num826 * num826 + num827 * num827));
                            Tile tile = Framing.GetTileSafely(num824, num825);
                            if (num828 < (double)num814)
                            {
                                if (tile != null && tile.active() && tile.type != (ushort)mod.TileType("AbyssGravel") &&
                                    tile.type != (ushort)mod.TileType("Voidstone") && (tile.type != TileID.Hellstone || Main.hardMode) &&
                                    (tile.type != TileID.LihzahrdBrick || NPC.downedGolemBoss) && tile.type != TileID.BlueDungeonBrick &&
                                    tile.type != TileID.GreenDungeonBrick && tile.type != TileID.PinkDungeonBrick && tile.type != TileID.DemonAltar &&
                                    (tile.type != (ushort)mod.TileType("AstralOre") || CalamityWorld.downedStarGod) &&
                                    ((tile.type != (ushort)mod.TileType("Tenebris") && tile.type != (ushort)mod.TileType("PlantyMush")) || NPC.downedPlantBoss || CalamityWorld.downedCalamitas) &&
                                    (!player.Calamity().ZoneSunkenSea || CalamityWorld.downedDesertScourge) &&
                                    (Main.tileValue[tile.type] < tileValueLimit || tile.type == TileID.Heart || tile.type == TileID.LifeFruit) &&
                                    !player.noBuilding && tile.type != TileID.ElderCrystalStand && tile.type != TileID.Containers)
                                {
                                    WorldGen.KillTile(num824, num825, false, false, false);
                                    if (!Main.tile[num824, num825].active() && Main.netMode != NetmodeID.SinglePlayer)
                                    {
                                        NetMessage.SendData(17, -1, -1, null, 0, (float)num824, (float)num825, 0f, 0, 0, 0);
                                    }
                                }
                            }
                        }
                    }
                    AchievementsHelper.CurrentlyMining = false;
                }
            }
        }
    }
}
