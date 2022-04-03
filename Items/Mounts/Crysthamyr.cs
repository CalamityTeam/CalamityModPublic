using CalamityMod.Buffs.Mounts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;

namespace CalamityMod.Items.Mounts
{
    public class Crysthamyr : ModMountData
    {
        public override void SetDefaults()
        {
            mountData.spawnDust = 173;
            mountData.spawnDustNoGravity = true;
            mountData.buff = ModContent.BuffType<GazeOfCrysthamyrBuff>();
            mountData.heightBoost = 36;
            mountData.fallDamage = 0f;
            mountData.runSpeed = 5f;
            mountData.dashSpeed = 15f;
            mountData.flightTimeMax = 750;
            mountData.fatigueMax = 0;
            mountData.jumpHeight = 12;
            mountData.acceleration = 0.35f;
            mountData.jumpSpeed = 9f;
            mountData.swimSpeed = 6f;
            mountData.blockExtraJumps = true;
            mountData.totalFrames = 16;
            mountData.constantJump = false;
            int[] array = new int[mountData.totalFrames];
            for (int l = 0; l < array.Length; l++)
            {
                array[l] = 30;
            }
            array[1] = 28;
            array[5] = 40;
            array[6] = 40;
            array[7] = 40;
            array[8] = 38; //wing flap sound
            array[9] = 40;
            array[10] = 40;
            array[12] = 28;
            array[14] = 28;
            mountData.playerYOffsets = array;
            mountData.xOffset = -56;
            mountData.bodyFrame = 3;
            mountData.yOffset = -20; //done
            mountData.playerHeadOffset = 38; //30
            mountData.standingFrameCount = 5;
            mountData.standingFrameDelay = 12;
            mountData.standingFrameStart = 0;
            mountData.runningFrameCount = mountData.standingFrameCount;
            mountData.runningFrameDelay = 48; //36
            mountData.runningFrameStart = 11;
            mountData.flyingFrameCount = 6;
            mountData.flyingFrameDelay = mountData.flyingFrameCount;
            mountData.flyingFrameStart = mountData.standingFrameCount;
            mountData.inAirFrameCount = mountData.flyingFrameCount;
            mountData.inAirFrameDelay = mountData.standingFrameDelay;
            mountData.inAirFrameStart = mountData.standingFrameCount;
            mountData.idleFrameCount = mountData.standingFrameCount; //done
            mountData.idleFrameDelay = mountData.standingFrameDelay; //done
            mountData.idleFrameStart = mountData.standingFrameStart;
            mountData.idleFrameLoop = true;
            mountData.swimFrameCount = mountData.inAirFrameCount;
            mountData.swimFrameDelay = mountData.runningFrameDelay;
            mountData.swimFrameStart = mountData.inAirFrameStart;
            if (Main.netMode != NetmodeID.Server)
            {
                mountData.frontTextureExtra = ModContent.Request<Texture2D>("CalamityMod/Items/Mounts/CrysthamyrExtra");
                mountData.textureWidth = mountData.backTexture.Width;
                mountData.textureHeight = mountData.backTexture.Height;
            }
        }

        public override void UpdateEffects(Player player)
        {
            Lighting.AddLight((int)(player.position.X + (float)(player.width / 2)) / 16, (int)(player.position.Y + (float)(player.height / 2)) / 16, 1f, 0f, 1f);
            if (Math.Abs(player.velocity.X) > 6f || Math.Abs(player.velocity.Y) > 6f)
            {
                int rand = Main.rand.Next(3);
                switch (rand)
                {
                    case 0:
                        rand = 173;
                        break;
                    case 1:
                        rand = 109;
                        break;
                    case 2:
                        rand = 70;
                        break;
                }
                Rectangle rect = player.getRect();
                rect.Inflate(50, 20);
                if (Math.Abs(player.velocity.X) > 14f || Math.Abs(player.velocity.Y) > 14f)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        int dust = Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, rand, 0, 0, 0, default);
                        Main.dust[dust].noGravity = true;
                    }
                }
                int dust2 = Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, rand, 0, 0, 0, default);
                Main.dust[dust2].noGravity = true;
            }
            if (player.velocity.Y != 0f)
            {
                if (player.mount.PlayerOffset == 38)
                {
                    if (!player.flapSound)
                        SoundEngine.PlaySound(SoundID.Item32, player.position);
                    player.flapSound = true;
                }
                else
                    player.flapSound = false;
            }
        }
    }
}
