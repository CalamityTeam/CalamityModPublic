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

namespace CalamityMod.Items.Mounts
{
    public class OnyxExcavator : ModMountData
    {
        public override void SetDefaults()
        {
            mountData.spawnDust = 109;
            mountData.spawnDustNoGravity = true;
            mountData.buff = ModContent.BuffType<OnyxExcavatorBuff>();
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
                mountData.backTextureExtra = ModContent.GetTexture("CalamityMod/Items/Mounts/OnyxExcavatorExtra");
                mountData.frontTextureExtra = ModContent.GetTexture("CalamityMod/Items/Mounts/OnyxExcavatorExtra2");
                mountData.textureWidth = mountData.backTexture.Width;
                mountData.textureHeight = mountData.backTexture.Height;
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
                    int highestPickPower = 35; //35% if you have no pickaxes.
                    for (int item = 0; item < Main.maxInventory; item++)
                    {
                        if (player.inventory[item].pick <= 0)
                            continue;
                        if (player.inventory[item].pick > highestPickPower)
                        {
                            highestPickPower = player.inventory[item].pick;
                        }
                    }

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

                            int pickReq = 0;
                            ModTile moddedTile = TileLoader.GetTile(tile.type);
                            if (moddedTile != null)
                            {
                                pickReq = moddedTile.minPick;
                            }
                            else
                            {
                                switch (tile.type)
                                {
                                    case TileID.Chlorophyte:
                                        pickReq = 200;
                                        break;
                                    case TileID.Ebonstone:
                                    case TileID.Crimstone:
                                    case TileID.Pearlstone:
                                    case TileID.DesertFossil:
                                    case TileID.Obsidian:
                                    case TileID.Hellstone:
                                        pickReq = 65;
                                        break;
                                    case TileID.Meteorite:
                                        pickReq = 50;
                                        break;
                                    case TileID.Demonite:
                                    case TileID.Crimtane:
                                        if (y > Main.worldSurface)
                                            pickReq = 55;
                                        break;
                                    case TileID.LihzahrdBrick:
                                    case TileID.LihzahrdAltar:
                                        pickReq = 210;
                                        break;
                                    case TileID.Cobalt:
                                    case TileID.Palladium:
                                        pickReq = 100;
                                        break;
                                    case TileID.Mythril:
                                    case TileID.Orichalcum:
                                        pickReq = 110;
                                        break;
                                    case TileID.Adamantite:
                                    case TileID.Titanium:
                                        pickReq = 150;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            if (Main.tileDungeon[tile.type])
                            {
                                if (x < Main.maxTilesX * 0.35 || x > Main.maxTilesX * 0.65)
                                pickReq = 65;
                            }

                            if (tileDist < mineHeight)
                            {
                                bool t = 1 == 1;
                                bool f = 1 == 2;
                                bool canBreakTileCheck = TileLoader.CanKillTile(x, y, tile.type, ref t) && TileLoader.CanKillTile(x, y, tile.type, ref f);
                                List<int> tileExcludeList = new List<int>()
                                { 
                                    ModContent.TileType<AbyssGravel>(),
                                    ModContent.TileType<Voidstone>(),
                                    TileID.DemonAltar,
                                    TileID.ElderCrystalStand
                                };

                                if (tile.active() && !player.noBuilding && !Main.tileContainer[tile.type] &&
                                    tileExcludeList.TrueForAll(z => tile.type != z) && pickReq < highestPickPower && canBreakTileCheck)
                                {
                                    WorldGen.KillTile(x, y, false, false, false);
                                    if (!Main.tile[x, y].active() && Main.netMode != NetmodeID.SinglePlayer)
                                    {
                                        NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, x, y, 0f, 0, 0, 0);
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
