using CalamityMod.Buffs.Mounts;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Mounts
{
	public class BUMBLEDOGE : ModMountData
    {
        public override void SetDefaults()
        {
            mountData.spawnDust = 60;
            mountData.spawnDustNoGravity = true;
            mountData.buff = ModContent.BuffType<BumbledogeMount>();
            mountData.flightTimeMax = 600;
            mountData.fatigueMax = 0;
            mountData.fallDamage = 0f;
            mountData.runSpeed = 12f;
            mountData.heightBoost = 44;
            mountData.acceleration = 0.2f;
            mountData.jumpHeight = 10;
            mountData.jumpSpeed = 4f;
            mountData.blockExtraJumps = false;
            mountData.totalFrames = 12;
            mountData.constantJump = false;
            int[] array = new int[mountData.totalFrames];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = 30;
            }
            mountData.playerYOffsets = array;
            mountData.playerHeadOffset = mountData.heightBoost;
            mountData.xOffset = 1;
            mountData.bodyFrame = 3;
            mountData.yOffset = 0;
            mountData.standingFrameCount = 1;
            mountData.standingFrameDelay = 12;
            mountData.standingFrameStart = 0;
            mountData.runningFrameCount = 5;
            mountData.runningFrameDelay = 20;
            mountData.runningFrameStart = 1;
            mountData.flyingFrameCount = 4;
            mountData.flyingFrameDelay = 7;
            mountData.flyingFrameStart = 7;
            mountData.inAirFrameCount = 1;
            mountData.inAirFrameDelay = 11;
            mountData.inAirFrameStart = 8;
            mountData.idleFrameCount = 1;
            mountData.idleFrameDelay = 10;
            mountData.idleFrameStart = 0;
            mountData.idleFrameLoop = true;
            mountData.swimFrameCount = mountData.inAirFrameCount;
            mountData.swimFrameDelay = mountData.inAirFrameDelay;
            mountData.swimFrameStart = mountData.inAirFrameStart;

            mountData.dashSpeed = 18.5f;
            mountData.dashingFrameCount = mountData.flyingFrameCount;
            mountData.dashingFrameDelay = 5;
            mountData.dashingFrameStart = mountData.flyingFrameStart;
            if (Main.netMode != NetmodeID.Server)
            {
                mountData.textureWidth = mountData.backTexture.Width;
                mountData.textureHeight = mountData.backTexture.Height;
            }
        }
        public override void UpdateEffects(Player player)
        {
            if (Main.rand.NextBool(260) && Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectileDirect(player.Center + Main.rand.NextVector2Circular(45f, 45f),
                                               Main.rand.NextVector2Circular(15f, 15f),
                                               ModContent.ProjectileType<Minibirb>(),
                                               (int)(180 * player.AverageDamage()),
                                               1f,
                                               player.whoAmI).Calamity().forceTypeless = true;
            }
        }
    }
}
