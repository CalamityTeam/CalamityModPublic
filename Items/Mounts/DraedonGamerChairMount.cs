using CalamityMod.Buffs.Mounts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Mounts
{
    public class DraedonGamerChairMount : ModMountData
    {
        public const float MovementSpeed = 12f;
        public override void SetDefaults()
        {
            mountData.spawnDust = 182;
            mountData.spawnDustNoGravity = true;
            mountData.buff = ModContent.BuffType<DraedonGamerChairBuff>();
            mountData.heightBoost = 0;
            mountData.flightTimeMax = int.MaxValue;
            mountData.fatigueMax = int.MaxValue;
            mountData.fallDamage = 0f;
            mountData.usesHover = true;
            mountData.runSpeed = MovementSpeed;
            mountData.dashSpeed = MovementSpeed;
            mountData.acceleration = MovementSpeed;
            mountData.swimSpeed = MovementSpeed;
            mountData.jumpHeight = 8;
            mountData.jumpSpeed = 8f;
            mountData.blockExtraJumps = true;
            mountData.totalFrames = 5;

            int[] verticalOffsets = new int[mountData.totalFrames];
            for (int l = 0; l < verticalOffsets.Length; l++)
                verticalOffsets[l] = 0;

            mountData.playerYOffsets = verticalOffsets;
            mountData.xOffset = 2;
            mountData.bodyFrame = 3;
            mountData.yOffset = 28;
            mountData.playerHeadOffset = 3;
            mountData.standingFrameCount = 5;
            mountData.standingFrameDelay = 5;
            mountData.standingFrameStart = 0;
            mountData.runningFrameCount = 5;
            mountData.runningFrameDelay = 5;
            mountData.runningFrameStart = 0;
            mountData.flyingFrameCount = 5;
            mountData.flyingFrameDelay = 5;
            mountData.flyingFrameStart = 0;
            mountData.inAirFrameCount = 5;
            mountData.inAirFrameDelay = 5;
            mountData.inAirFrameStart = 0;
            mountData.idleFrameCount = 5;
            mountData.idleFrameDelay = 5;
            mountData.idleFrameStart = 0;
            mountData.idleFrameLoop = true;
            mountData.swimFrameCount = 5;
            mountData.swimFrameDelay = 5;
            mountData.swimFrameStart = 0;
            if (Main.netMode != NetmodeID.Server)
            {
                mountData.frontTextureGlow = ModContent.Request<Texture2D>("CalamityMod/Items/Mounts/DraedonGamerChairMount_Glowmask");
                mountData.textureWidth = mountData.frontTexture.Width;
                mountData.textureHeight = mountData.frontTexture.Height;
            }
        }

        public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            rotation = drawPlayer.velocity.X * 0.004f;
            drawPlayer.fullRotation = rotation;
            frame = texture.Frame(1, 5, 0, (int)(Main.GlobalTime * 13f) % 5);
            return true;
        }
    }
}
