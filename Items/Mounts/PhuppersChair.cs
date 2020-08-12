using CalamityMod.Dusts;
using CalamityMod.Buffs.Mounts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Mounts
{
    public class PhuppersChair : ModMountData
    {
        public override void SetDefaults()
        {
            mountData.spawnDust = (int)CalamityDusts.Brimstone;
            mountData.spawnDustNoGravity = true;
            mountData.buff = ModContent.BuffType<BrimstoneMount>();
            mountData.heightBoost = 12;
            mountData.flightTimeMax = int.MaxValue - 1;
            mountData.fatigueMax = int.MaxValue - 1;
            mountData.fallDamage = 0f;
            mountData.usesHover = true;
            mountData.runSpeed = 12f;
            mountData.dashSpeed = 12f;
            mountData.acceleration = 0.2f;
            mountData.jumpHeight = 10;
            mountData.jumpSpeed = 4f;
            mountData.blockExtraJumps = true;
            mountData.totalFrames = 4;
            int[] array = new int[mountData.totalFrames];
            for (int l = 0; l < array.Length; l++)
            {
                array[l] = 12;
            }
            mountData.playerYOffsets = array;
            mountData.xOffset = 1;
            mountData.bodyFrame = 3;
            mountData.yOffset = 0;
            mountData.playerHeadOffset = 18;
            mountData.standingFrameCount = 4;
            mountData.standingFrameDelay = 4;
            mountData.standingFrameStart = 0;
            mountData.runningFrameCount = 4;
            mountData.runningFrameDelay = 16;
            mountData.runningFrameStart = 0;
            mountData.flyingFrameCount = 4;
            mountData.flyingFrameDelay = 4;
            mountData.flyingFrameStart = 0;
            mountData.inAirFrameCount = 4;
            mountData.inAirFrameDelay = 4;
            mountData.inAirFrameStart = 0;
            mountData.idleFrameCount = 4;
            mountData.idleFrameDelay = 8;
            mountData.idleFrameStart = 0;
            mountData.idleFrameLoop = true;
            mountData.swimFrameCount = 4;
            mountData.swimFrameDelay = 4;
            mountData.swimFrameStart = 0;
            if (Main.netMode != NetmodeID.Server)
            {
                mountData.textureWidth = mountData.backTexture.Width;
                mountData.textureHeight = mountData.backTexture.Height;
            }
        }
    }
}
