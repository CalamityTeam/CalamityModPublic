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
            MountData.dashSpeed = 10f;
            MountData.flightTimeMax = 750;
            MountData.fatigueMax = 0;
            MountData.jumpHeight = 12;
            MountData.acceleration = 0.6f;
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
            array[8] = 38; // Wing flap sound
            array[9] = 40;
            array[10] = 40;
            array[12] = 28;
            array[14] = 28;
            MountData.playerYOffsets = array;
            MountData.xOffset = -56;
            MountData.bodyFrame = 3;
            MountData.yOffset = -20;
            MountData.playerHeadOffset = 38;
            MountData.standingFrameCount = 5;
            MountData.standingFrameDelay = 12;
            MountData.standingFrameStart = 0;
            MountData.runningFrameCount = MountData.standingFrameCount;
            MountData.runningFrameDelay = 48;
            MountData.runningFrameStart = 11;
            MountData.flyingFrameCount = 6;
            MountData.flyingFrameDelay = MountData.flyingFrameCount;
            MountData.flyingFrameStart = MountData.standingFrameCount;
            MountData.inAirFrameCount = MountData.flyingFrameCount;
            MountData.inAirFrameDelay = MountData.standingFrameDelay;
            MountData.inAirFrameStart = MountData.standingFrameCount;
            MountData.idleFrameCount = MountData.standingFrameCount;
            MountData.idleFrameDelay = MountData.standingFrameDelay;
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

            // Same as Celestial Starboard vertical velocity increase while holding W
            if (player.controlJump && player.TryingToHoverUp)
            {
                player.velocity.Y -= 0.4f * player.gravDir;
                if (player.gravDir == 1f)
                {
                    if (player.velocity.Y > 0f)
                        player.velocity.Y -= 1f;
                    else if (player.velocity.Y > 0f - MountData.jumpSpeed)
                        player.velocity.Y -= 0.2f;

                    if (player.velocity.Y < (0f - MountData.jumpSpeed) * 3f)
                        player.velocity.Y = (0f - MountData.jumpSpeed) * 3f;
                }
                else
                {
                    if (player.velocity.Y < 0f)
                        player.velocity.Y += 1f;
                    else if (player.velocity.Y < MountData.jumpSpeed)
                        player.velocity.Y += 0.2f;

                    if (player.velocity.Y > MountData.jumpSpeed * 3f)
                        player.velocity.Y = MountData.jumpSpeed * 3f;
                }
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
