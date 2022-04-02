using CalamityMod.Buffs.Mounts;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
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

            // previous stats commented for balance.
            // for run and dash speed, each 1 unit is 5.08mph in-game.
            mountData.runSpeed = 10f; // 12f
            mountData.dashSpeed = 14.15f; // 18.5f
            mountData.acceleration = 0.13f; // 0.2f

            mountData.fatigueMax = 0;
            mountData.flightTimeMax = 600;
            mountData.fallDamage = 0f;

            mountData.jumpHeight = 10;
            mountData.jumpSpeed = 4f;
            mountData.blockExtraJumps = false;
            mountData.constantJump = false;

            mountData.totalFrames = 12;
            mountData.heightBoost = 44;
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
            if (Main.myPlayer == player.whoAmI && Main.rand.NextBool(260))
            {
                Vector2 pos = player.Center + Main.rand.NextVector2Circular(45f, 45f);
                Vector2 vel = Main.rand.NextVector2Circular(15f, 15f);
                int damage = (int)(180 * player.AverageDamage());
                float kb = 1f;
                Projectile birb = Projectile.NewProjectileDirect(pos, vel, ModContent.ProjectileType<Minibirb>(), damage, kb, player.whoAmI);
                if (birb.whoAmI.WithinBounds(Main.maxProjectiles))
                    birb.Calamity().forceTypeless = true;
            }
        }
    }
}
