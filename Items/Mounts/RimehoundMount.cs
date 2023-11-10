using CalamityMod.Buffs.Mounts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Mounts
{
    public class RimehoundMount : ModMount
    {
        public override void SetStaticDefaults()
        {
            MountData.spawnDust = 192;
            MountData.spawnDustNoGravity = true;
            MountData.buff = ModContent.BuffType<RimehoundBuff>();
            MountData.heightBoost = 36; //8
            MountData.fallDamage = 0.3f;
            MountData.runSpeed = 6.5f;
            MountData.flightTimeMax = 0;
            MountData.jumpHeight = 16;
            MountData.acceleration = 0.21f;
            MountData.jumpSpeed = 7f;
            MountData.swimSpeed = 3f;
            MountData.totalFrames = 13;
            int[] array = new int[MountData.totalFrames];
            for (int l = 0; l < array.Length; l++)
            {
                array[l] = 28;
            }
            array[1] = 26;
            array[4] = 26;
            array[7] = 26;
            array[10] = 26;
            MountData.playerYOffsets = array;
            MountData.xOffset = -6;
            MountData.bodyFrame = 3;
            MountData.yOffset = 15; //done
            MountData.playerHeadOffset = 38;
            MountData.standingFrameCount = 6;
            MountData.standingFrameDelay = 12;
            MountData.standingFrameStart = 0;
            MountData.runningFrameCount = MountData.standingFrameCount;
            MountData.runningFrameDelay = 36; //36
            MountData.runningFrameStart = MountData.standingFrameCount;
            MountData.inAirFrameCount = 1;
            MountData.inAirFrameDelay = MountData.standingFrameDelay;
            MountData.inAirFrameStart = MountData.standingFrameDelay;
            MountData.idleFrameCount = MountData.standingFrameCount; //done
            MountData.idleFrameDelay = MountData.standingFrameDelay; //done
            MountData.idleFrameStart = MountData.standingFrameStart;
            MountData.idleFrameLoop = true;
            MountData.swimFrameCount = MountData.inAirFrameCount;
            MountData.swimFrameDelay = MountData.inAirFrameDelay;
            MountData.swimFrameStart = MountData.inAirFrameStart;
            if (Main.netMode != NetmodeID.Server)
            {
                MountData.textureWidth = MountData.backTexture.Width();
                MountData.textureHeight = MountData.backTexture.Height();
            }
        }

        public override bool UpdateFrame(Player mountedPlayer, int state, Vector2 velocity)
        {
            bool speed = Math.Abs(velocity.X) > mountedPlayer.mount.RunSpeed / 2f;
            float direction = (float)Math.Sign(mountedPlayer.velocity.X);
            float dustYOffset = 12f;
            float dustXOffset = 40f;
            if (!speed)
            {
                mountedPlayer.basiliskCharge = 0f;
            }
            else
            {
                mountedPlayer.basiliskCharge = Utils.Clamp(mountedPlayer.basiliskCharge + 0.00555555569f, 0f, 1f);
            }
            if ((double)mountedPlayer.position.Y > Main.worldSurface * 16.0 + 160.0)
            {
                Lighting.AddLight(mountedPlayer.Center, 0.2f, 0.25f, 0.25f);
            }
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
                    mountedPlayer.position += new Vector2(direction * 24f, 0f);
                    mountedPlayer.FloorVisuals(true);
                    mountedPlayer.position -= new Vector2(direction * 24f, 0f);
                }
            }
            if (direction == (float)mountedPlayer.direction)
            {
                for (int j = 0; j < (int)(3f * mountedPlayer.basiliskCharge); j++)
                {
                    Dust dust = Main.dust[Dust.NewDust(mountedPlayer.BottomLeft, mountedPlayer.width, 6, 67, 0f, 0f, 0, default, 1f)];
                    Vector2 dustVel = mountedPlayer.Center + new Vector2(direction * dustXOffset, dustYOffset);
                    dust.position = mountedPlayer.Center + new Vector2(direction * (dustXOffset - 2f), dustYOffset - 6f + Main.rand.NextFloat() * 12f);
                    dust.velocity = (dust.position - dustVel).SafeNormalize(Vector2.Zero) * (3.5f + Main.rand.NextFloat() * 0.5f);
                    if (dust.velocity.Y < 0f)
                    {
                        Dust expr_808_cp_0_cp_0 = dust;
                        expr_808_cp_0_cp_0.velocity.Y *= 1f + 2f * Main.rand.NextFloat();
                    }
                    dust.velocity += mountedPlayer.velocity * 0.55f;
                    dust.velocity *= mountedPlayer.velocity.Length() / mountedPlayer.mount.RunSpeed;
                    dust.velocity *= mountedPlayer.basiliskCharge;
                    dust.noGravity = true;
                    dust.noLight = true;
                    dust.scale = 0.2f + Main.rand.NextFloat() * 0.8f;
                    dust.fadeIn = 0.5f + Main.rand.NextFloat() * 1f;
                    dust.shader = GameShaders.Armor.GetSecondaryShader(mountedPlayer.cMount, mountedPlayer);
                }
            }
            return true;
        }
    }
}
