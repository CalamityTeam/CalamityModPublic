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
    public class Crysthamyr : ModMount
    {
        public override void SetStaticDefaults()
        {
            MountData.spawnDust = 173;
            MountData.spawnDustNoGravity = true;
            MountData.buff = ModContent.BuffType<GazeOfCrysthamyrBuff>();
            MountData.heightBoost = 36;
            MountData.fallDamage = 0f;
            MountData.runSpeed = 5f;
            MountData.dashSpeed = 15f;
            MountData.flightTimeMax = 750;
            MountData.fatigueMax = 0;
            MountData.jumpHeight = 12;
            MountData.acceleration = 0.35f;
            MountData.jumpSpeed = 9f;
            MountData.swimSpeed = 6f;
            MountData.blockExtraJumps = true;
            MountData.totalFrames = 16;
            MountData.constantJump = false;
            int[] array = new int[MountData.totalFrames];
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
            MountData.playerYOffsets = array;
            MountData.xOffset = -56;
            MountData.bodyFrame = 3;
            MountData.yOffset = -20; //done
            MountData.playerHeadOffset = 38; //30
            MountData.standingFrameCount = 5;
            MountData.standingFrameDelay = 12;
            MountData.standingFrameStart = 0;
            MountData.runningFrameCount = MountData.standingFrameCount;
            MountData.runningFrameDelay = 48; //36
            MountData.runningFrameStart = 11;
            MountData.flyingFrameCount = 6;
            MountData.flyingFrameDelay = MountData.flyingFrameCount;
            MountData.flyingFrameStart = MountData.standingFrameCount;
            MountData.inAirFrameCount = MountData.flyingFrameCount;
            MountData.inAirFrameDelay = MountData.standingFrameDelay;
            MountData.inAirFrameStart = MountData.standingFrameCount;
            MountData.idleFrameCount = MountData.standingFrameCount; //done
            MountData.idleFrameDelay = MountData.standingFrameDelay; //done
            MountData.idleFrameStart = MountData.standingFrameStart;
            MountData.idleFrameLoop = true;
            MountData.swimFrameCount = MountData.inAirFrameCount;
            MountData.swimFrameDelay = MountData.runningFrameDelay;
            MountData.swimFrameStart = MountData.inAirFrameStart;
            if (Main.netMode != NetmodeID.Server)
            {
                MountData.frontTextureExtra = ModContent.Request<Texture2D>("CalamityMod/Items/Mounts/CrysthamyrExtra");
                MountData.textureWidth = MountData.backTexture.Width();
                MountData.textureHeight = MountData.backTexture.Height();
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
