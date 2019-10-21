using CalamityMod.Buffs.Mounts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Mounts
{
    class AngryDogMount : ModMountData
    {
        public override void SetDefaults()
        {
            mountData.spawnDust = 192;
            mountData.spawnDustNoGravity = true;
            mountData.buff = ModContent.BuffType<TundraLeashBuff>();
            mountData.heightBoost = 36; //8
            mountData.fallDamage = 0.3f;
            mountData.runSpeed = 6.5f;
            mountData.flightTimeMax = 0;
            mountData.jumpHeight = 16;
            mountData.acceleration = 0.21f;
            mountData.jumpSpeed = 7f;
            mountData.swimSpeed = 3f;
            mountData.totalFrames = 13;
            int[] array = new int[mountData.totalFrames];
            for (int l = 0; l < array.Length; l++)
            {
                array[l] = 28;
            }
            array[1] = 26;
            array[4] = 26;
            array[7] = 26;
            array[10] = 26;
            mountData.playerYOffsets = array;
            mountData.xOffset = -6;
            mountData.bodyFrame = 3;
            mountData.yOffset = 15; //done
            mountData.playerHeadOffset = 38;
            mountData.standingFrameCount = 6;
            mountData.standingFrameDelay = 12;
            mountData.standingFrameStart = 0;
            mountData.runningFrameCount = mountData.standingFrameCount;
            mountData.runningFrameDelay = 36; //36
            mountData.runningFrameStart = mountData.standingFrameCount;
            mountData.inAirFrameCount = 1;
            mountData.inAirFrameDelay = mountData.standingFrameDelay;
            mountData.inAirFrameStart = mountData.standingFrameDelay;
            mountData.idleFrameCount = mountData.standingFrameCount; //done
            mountData.idleFrameDelay = mountData.standingFrameDelay; //done
            mountData.idleFrameStart = mountData.standingFrameStart;
            mountData.idleFrameLoop = true;
            mountData.swimFrameCount = mountData.inAirFrameCount;
            mountData.swimFrameDelay = mountData.inAirFrameDelay;
            mountData.swimFrameStart = mountData.inAirFrameStart;
            if (Main.netMode != NetmodeID.Server)
            {
                mountData.textureWidth = mountData.backTexture.Width;
                mountData.textureHeight = mountData.backTexture.Height;
            }
        }

        public override bool UpdateFrame(Player mountedPlayer, int state, Vector2 velocity)
        {
            bool speed = Math.Abs(velocity.X) > mountedPlayer.mount.RunSpeed / 2f;
            float num10 = (float)Math.Sign(mountedPlayer.velocity.X);
            float num11 = 12f;
            float num12 = 40f;
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
                    mountedPlayer.position += new Vector2(num10 * 24f, 0f);
                    mountedPlayer.FloorVisuals(true);
                    mountedPlayer.position -= new Vector2(num10 * 24f, 0f);
                }
            }
            if (num10 == (float)mountedPlayer.direction)
            {
                for (int j = 0; j < (int)(3f * mountedPlayer.basiliskCharge); j++)
                {
                    Dust dust = Main.dust[Dust.NewDust(mountedPlayer.BottomLeft, mountedPlayer.width, 6, 67, 0f, 0f, 0, default, 1f)];
                    Vector2 value2 = mountedPlayer.Center + new Vector2(num10 * num12, num11);
                    dust.position = mountedPlayer.Center + new Vector2(num10 * (num12 - 2f), num11 - 6f + Main.rand.NextFloat() * 12f);
                    dust.velocity = (dust.position - value2).SafeNormalize(Vector2.Zero) * (3.5f + Main.rand.NextFloat() * 0.5f);
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
