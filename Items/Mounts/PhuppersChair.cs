using CalamityMod.Dusts;
using CalamityMod.Buffs.Mounts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Mounts
{
    public class PhuppersChair : ModMount
    {
        public override void SetStaticDefaults()
        {
            MountData.spawnDust = (int)CalamityDusts.Brimstone;
            MountData.spawnDustNoGravity = true;
            MountData.buff = ModContent.BuffType<BrimstoneMount>();
            MountData.heightBoost = 12;
            MountData.flightTimeMax = int.MaxValue - 1;
            MountData.fatigueMax = int.MaxValue - 1;
            MountData.fallDamage = 0f;
            MountData.usesHover = true;
            MountData.runSpeed = 12f;
            MountData.dashSpeed = 12f;
            MountData.acceleration = 0.2f;
            MountData.jumpHeight = 10;
            MountData.jumpSpeed = 4f;
            MountData.blockExtraJumps = true;
            MountData.totalFrames = 4;
            int[] array = new int[MountData.totalFrames];
            for (int l = 0; l < array.Length; l++)
            {
                array[l] = 12;
            }
            MountData.playerYOffsets = array;
            MountData.xOffset = 1;
            MountData.bodyFrame = 3;
            MountData.yOffset = 0;
            MountData.playerHeadOffset = 18;
            MountData.standingFrameCount = 4;
            MountData.standingFrameDelay = 4;
            MountData.standingFrameStart = 0;
            MountData.runningFrameCount = 4;
            MountData.runningFrameDelay = 16;
            MountData.runningFrameStart = 0;
            MountData.flyingFrameCount = 4;
            MountData.flyingFrameDelay = 4;
            MountData.flyingFrameStart = 0;
            MountData.inAirFrameCount = 4;
            MountData.inAirFrameDelay = 4;
            MountData.inAirFrameStart = 0;
            MountData.idleFrameCount = 4;
            MountData.idleFrameDelay = 8;
            MountData.idleFrameStart = 0;
            MountData.idleFrameLoop = true;
            MountData.swimFrameCount = 4;
            MountData.swimFrameDelay = 4;
            MountData.swimFrameStart = 0;
            if (Main.netMode != NetmodeID.Server)
            {
                MountData.textureWidth = MountData.backTexture.Width();
                MountData.textureHeight = MountData.backTexture.Height();
            }
        }
    }
}
