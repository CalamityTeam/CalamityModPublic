using CalamityMod.Buffs.Mounts;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Mounts
{
    public class BUMBLEDOGE : ModMount
    {
        public override void SetStaticDefaults()
        {
            MountData.spawnDust = 60;
            MountData.spawnDustNoGravity = true;
            MountData.buff = ModContent.BuffType<BumbledogeMount>();

            // previous stats commented for balance.
            // for run and dash speed, each 1 unit is 5.08mph in-game.
            MountData.runSpeed = 10f; // 12f
            MountData.dashSpeed = 14.15f; // 18.5f
            MountData.acceleration = 0.13f; // 0.2f

            MountData.fatigueMax = 0;
            MountData.flightTimeMax = 600;
            MountData.fallDamage = 0f;

            MountData.jumpHeight = 10;
            MountData.jumpSpeed = 4f;
            MountData.blockExtraJumps = false;
            MountData.constantJump = false;

            MountData.totalFrames = 12;
            MountData.heightBoost = 44;
            int[] array = new int[MountData.totalFrames];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = 30;
            }
            MountData.playerYOffsets = array;
            MountData.playerHeadOffset = MountData.heightBoost;
            MountData.xOffset = 1;
            MountData.bodyFrame = 3;
            MountData.yOffset = 0;
            MountData.standingFrameCount = 1;
            MountData.standingFrameDelay = 12;
            MountData.standingFrameStart = 0;
            MountData.runningFrameCount = 5;
            MountData.runningFrameDelay = 20;
            MountData.runningFrameStart = 1;
            MountData.flyingFrameCount = 4;
            MountData.flyingFrameDelay = 7;
            MountData.flyingFrameStart = 7;
            MountData.inAirFrameCount = 1;
            MountData.inAirFrameDelay = 11;
            MountData.inAirFrameStart = 8;
            MountData.idleFrameCount = 1;
            MountData.idleFrameDelay = 10;
            MountData.idleFrameStart = 0;
            MountData.idleFrameLoop = true;
            MountData.swimFrameCount = MountData.inAirFrameCount;
            MountData.swimFrameDelay = MountData.inAirFrameDelay;
            MountData.swimFrameStart = MountData.inAirFrameStart;
            MountData.dashingFrameCount = MountData.flyingFrameCount;
            MountData.dashingFrameDelay = 5;
            MountData.dashingFrameStart = MountData.flyingFrameStart;
            if (Main.netMode != NetmodeID.Server)
            {
                MountData.textureWidth = MountData.backTexture.Width();
                MountData.textureHeight = MountData.backTexture.Height();
            }
        }
        public override void UpdateEffects(Player player)
        {
            if (Main.myPlayer == player.whoAmI && Main.rand.NextBool(260))
            {
                Vector2 pos = player.Center + Main.rand.NextVector2Circular(45f, 45f);
                Vector2 vel = Main.rand.NextVector2Circular(15f, 15f);
                int damage = (int)player.GetBestClassDamage().ApplyTo(180);
                float kb = 1f;
                Projectile birb = Projectile.NewProjectileDirect(new EntitySource_Parent(player), pos, vel, ModContent.ProjectileType<MiniatureFolly>(), damage, kb, player.whoAmI);
                if (birb.whoAmI.WithinBounds(Main.maxProjectiles))
                    birb.DamageType = DamageClass.Generic;
            }
        }
    }
}
