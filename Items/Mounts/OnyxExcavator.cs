using CalamityMod.Buffs.Mounts;
using CalamityMod.Tiles.Abyss;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
//using Terraria.GameContent.Achievements;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Items.Mounts
{
    public class OnyxExcavator : ModMount
    {
        public override void SetStaticDefaults()
        {
            MountData.spawnDust = 109;
            MountData.spawnDustNoGravity = true;
            MountData.buff = ModContent.BuffType<OnyxExcavatorBuff>();
            MountData.heightBoost = 10;
            MountData.fallDamage = 0f;
            MountData.runSpeed = 8f;
            MountData.flightTimeMax = 0;
            MountData.jumpHeight = 5;
            MountData.acceleration = 0.2f;
            MountData.jumpSpeed = 3f;
            MountData.swimSpeed = 0.5f;
            MountData.totalFrames = 6;
            int[] array = new int[MountData.totalFrames];
            for (int l = 0; l < array.Length; l++)
            {
                array[l] = 6;
            }
            array[1] = 4;
            array[5] = 4;
            MountData.playerYOffsets = array;
            MountData.xOffset = 10;
            MountData.bodyFrame = 3;
            MountData.yOffset = -1; //done
            MountData.playerHeadOffset = 10;
            MountData.standingFrameCount = 1;
            MountData.standingFrameDelay = 12;
            MountData.standingFrameStart = 0;
            MountData.runningFrameCount = 6;
            MountData.runningFrameDelay = 36; //36
            MountData.runningFrameStart = MountData.standingFrameStart;
            MountData.inAirFrameCount = MountData.standingFrameCount;
            MountData.inAirFrameDelay = MountData.standingFrameDelay;
            MountData.inAirFrameStart = MountData.standingFrameStart;
            MountData.idleFrameCount = MountData.standingFrameCount;
            MountData.idleFrameDelay = MountData.standingFrameDelay;
            MountData.idleFrameStart = MountData.standingFrameStart;
            MountData.idleFrameLoop = false;
            MountData.swimFrameCount = MountData.inAirFrameCount;
            MountData.swimFrameDelay = MountData.inAirFrameDelay;
            MountData.swimFrameStart = MountData.inAirFrameStart;
            if (Main.netMode != NetmodeID.Server)
            {
                MountData.backTextureExtra = ModContent.Request<Texture2D>("CalamityMod/Items/Mounts/OnyxExcavatorExtra");
                MountData.frontTextureExtra = ModContent.Request<Texture2D>("CalamityMod/Items/Mounts/OnyxExcavatorExtra2");
                MountData.textureWidth = MountData.backTexture.Width();
                MountData.textureHeight = MountData.backTexture.Height();
            }
        }

        public override bool UpdateFrame(Player mountedPlayer, int state, Vector2 velocity)
        {
            bool speed = Math.Abs(velocity.X) > mountedPlayer.mount.RunSpeed / 2f;
            float direction = Math.Sign(mountedPlayer.velocity.X);
            Lighting.AddLight(mountedPlayer.Center, 0.5f, 0.5f, 0.4f);
            if (speed && velocity.Y == 0f)
            {
                for (int i = 0; i < 2; i++)
                {
                    Dust dust = Dust.NewDustDirect(mountedPlayer.BottomLeft, mountedPlayer.width, 6, 192, 0f, 0f, 0, default, 1f);
                    dust.velocity = new Vector2(velocity.X * 0.15f, Main.rand.NextFloat() * -2f);
                    dust.noLight = true;
                    dust.scale = 0.2f + Main.rand.NextFloat() * 0.8f;
                    dust.fadeIn = 0.5f + Main.rand.NextFloat() * 1f;
                    dust.shader = GameShaders.Armor.GetSecondaryShader(mountedPlayer.cMount, mountedPlayer);
                }
                if (mountedPlayer.cMount == 0)
                {
                    mountedPlayer.position += new Vector2(direction * 24f, 0f);
                    mountedPlayer.FloorVisuals(true);
                    mountedPlayer.position -= new Vector2(direction * 24f, 0f);
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
                    int highestPickPower = player.GetBestPickPower();

                    float direction = Math.Sign(player.velocity.X);
                    if (Math.Abs(direction) < 0.1f)
                        direction = player.direction == 1 ? 0.1f : -0.1f;
                    float yOffset = 12f;
                    float xOffset = 40f;
                    Vector2 source = player.Center + new Vector2(direction * xOffset, yOffset);
                    int mineHeight = 3;
                    int xRight = (int)(source.X / 16 - mineHeight);
                    int xLeft = (int)(source.X / 16 + mineHeight);
                    int yTop = (int)(source.Y / 16 - mineHeight);
                    int yBottom = (int)(source.Y / 16 + mineHeight);
                    if (xRight < 0)
                    {
                        xRight = 0;
                    }
                    if (xLeft > Main.maxTilesX)
                    {
                        xLeft = Main.maxTilesX;
                    }
                    if (yTop < 0)
                    {
                        yTop = 0;
                    }
                    if (yBottom > Main.maxTilesY)
                    {
                        yBottom = Main.maxTilesY;
                    }
                    //AchievementsHelper.CurrentlyMining = true;
                    for (int x = xRight; x <= xLeft; x++)
                    {
                        for (int y = yTop; y <= yBottom; y++)
                        {
                            source.Y -= 2f;
                            Vector2 tileVec = new Vector2(Math.Abs(x - source.X / 16f), Math.Abs(y - source.Y / 16f));
                            float tileDist = tileVec.Length();
                            Tile tile = CalamityUtils.ParanoidTileRetrieval(x, y);

                            int pickReq = tile.GetRequiredPickPower(x, y);

                            if (tileDist < mineHeight)
                            {
                                bool t = 1 == 1;
                                bool f = 1 == 2;
                                bool canBreakTileCheck = TileLoader.CanKillTile(x, y, tile.TileType, ref t) && TileLoader.CanKillTile(x, y, tile.TileType, ref f);
                                List<int> tileExcludeList = new List<int>()
                                {
                                    ModContent.TileType<AbyssGravel>(),
                                    ModContent.TileType<PyreMantle>(),
                                    ModContent.TileType<PyreMantleMolten>(),
                                    ModContent.TileType<Voidstone>(),
                                    TileID.DemonAltar,
                                    TileID.ElderCrystalStand
                                };

                                if (tile.HasTile && !player.noBuilding && !Main.tileContainer[tile.TileType] &&
                                    tileExcludeList.TrueForAll(z => tile.TileType != z) && pickReq <= highestPickPower && canBreakTileCheck)
                                {
                                    WorldGen.KillTile(x, y, false, false, false);
                                    if (!Main.tile[x, y].HasTile && Main.netMode != NetmodeID.SinglePlayer)
                                    {
                                        NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, x, y, 0f, 0, 0, 0);
                                    }
                                }
                            }
                        }
                    }
                    //AchievementsHelper.CurrentlyMining = false;
                }
            }
        }
    }
}
