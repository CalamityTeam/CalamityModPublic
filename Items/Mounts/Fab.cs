using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System;
using CalamityMod.Items;

namespace CalamityMod.Items.Mounts
{
    class Fab : ModMountData
    {
        public override void SetDefaults()
        {
            mountData.spawnDust = 234;
            mountData.spawnDustNoGravity = true;
            mountData.buff = mod.BuffType("Fab");
            mountData.heightBoost = 20;
            mountData.fallDamage = 0f; //0.5
            mountData.runSpeed = 18f; //12
            mountData.dashSpeed = 12f; //8
            mountData.flightTimeMax = 50000;
            mountData.fatigueMax = 0;
            mountData.jumpHeight = 20;
            mountData.acceleration = 0.75f;
            mountData.jumpSpeed = 15f; //10
            mountData.blockExtraJumps = false;
            mountData.totalFrames = 15;
            mountData.constantJump = false;
            int[] array = new int[mountData.totalFrames];
            for (int l = 0; l < array.Length; l++)
            {
                array[l] = 30;
            }
            mountData.playerYOffsets = array;
            mountData.xOffset = 0;
            mountData.bodyFrame = 3;
            mountData.yOffset = 0; //-8
            mountData.playerHeadOffset = 26; //30
            mountData.standingFrameCount = 1;
            mountData.standingFrameDelay = 12;
            mountData.standingFrameStart = 0;
            mountData.runningFrameCount = 8; //7
            mountData.runningFrameDelay = 36; //36
            mountData.runningFrameStart = 1; //9
            mountData.flyingFrameCount = 6; //0
            mountData.flyingFrameDelay = 4; //0
            mountData.flyingFrameStart = 9; //0
            mountData.inAirFrameCount = 1; //1
            mountData.inAirFrameDelay = 12; //12
            mountData.inAirFrameStart = 10; //10
            mountData.idleFrameCount = 5; //4
            mountData.idleFrameDelay = 12; //12
            mountData.idleFrameStart = 0;
            mountData.idleFrameLoop = true;
            mountData.swimFrameCount = mountData.inAirFrameCount;
            mountData.swimFrameDelay = mountData.inAirFrameDelay;
            mountData.swimFrameStart = mountData.inAirFrameStart;
            if (Main.netMode != 2)
            {
                mountData.frontTextureExtra = mod.GetTexture("Items/Mounts/FabExtra");
                mountData.textureWidth = mountData.backTexture.Width;
                mountData.textureHeight = mountData.backTexture.Height;
            }
        }

        public override void UpdateEffects(Player player)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            if (modPlayer.fabsolVodka)
            {
                player.meleeDamage += 0.1f;
                player.rangedDamage += 0.1f;
                player.magicDamage += 0.1f;
                player.minionDamage += 0.1f;
                player.thrownDamage += 0.1f;
            }
            if (Math.Abs(player.velocity.X) > 15f)
            {
                int rand = Main.rand.Next(2);
                bool momo = false;
                if (rand == 1)
                {
                    momo = true;
                }
                Color meme;
                if (momo)
                {
                    meme = new Color(255, 68, 242);
                }
                else
                {
                    meme = new Color(25, 105, 255);
                }
                Rectangle rect = player.getRect();
                int dust = Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, 234, 0, 0, 0, meme);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
