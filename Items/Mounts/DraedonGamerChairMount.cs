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
    public class DraedonGamerChairMount : ModMount
    {
        public const float MovementSpeed = 12f;
        public override void SetStaticDefaults()
        {
            MountData.spawnDust = 182;
            MountData.spawnDustNoGravity = true;
            MountData.buff = ModContent.BuffType<DraedonGamerChairBuff>();
            MountData.heightBoost = 0;
            MountData.flightTimeMax = int.MaxValue;
            MountData.fatigueMax = int.MaxValue;
            MountData.fallDamage = 0f;
            MountData.usesHover = true;
            MountData.runSpeed = MovementSpeed;
            MountData.dashSpeed = MovementSpeed;
            MountData.acceleration = MovementSpeed;
            MountData.swimSpeed = MovementSpeed;
            MountData.jumpHeight = 8;
            MountData.jumpSpeed = 8f;
            MountData.blockExtraJumps = true;
            MountData.totalFrames = 5;

            int[] verticalOffsets = new int[MountData.totalFrames];
            for (int l = 0; l < verticalOffsets.Length; l++)
                verticalOffsets[l] = 0;

            MountData.playerYOffsets = verticalOffsets;
            MountData.xOffset = 2;
            MountData.bodyFrame = 3;
            MountData.yOffset = 28;
            MountData.playerHeadOffset = 3;
            MountData.standingFrameCount = 5;
            MountData.standingFrameDelay = 5;
            MountData.standingFrameStart = 0;
            MountData.runningFrameCount = 5;
            MountData.runningFrameDelay = 5;
            MountData.runningFrameStart = 0;
            MountData.flyingFrameCount = 5;
            MountData.flyingFrameDelay = 5;
            MountData.flyingFrameStart = 0;
            MountData.inAirFrameCount = 5;
            MountData.inAirFrameDelay = 5;
            MountData.inAirFrameStart = 0;
            MountData.idleFrameCount = 5;
            MountData.idleFrameDelay = 5;
            MountData.idleFrameStart = 0;
            MountData.idleFrameLoop = true;
            MountData.swimFrameCount = 5;
            MountData.swimFrameDelay = 5;
            MountData.swimFrameStart = 0;
            if (Main.netMode != NetmodeID.Server)
            {
                MountData.frontTextureGlow = ModContent.Request<Texture2D>("CalamityMod/Items/Mounts/DraedonGamerChairMount_Glowmask");
                MountData.textureWidth = MountData.frontTexture.Width();
                MountData.textureHeight = MountData.frontTexture.Height();
            }
        }

        public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            rotation = drawPlayer.velocity.X * 0.004f;
            drawPlayer.fullRotation = rotation;
            frame = texture.Frame(1, 5, 0, (int)(Main.GlobalTimeWrappedHourly * 13f) % 5);
            return true;
        }
    }
}
