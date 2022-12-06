using CalamityMod.Buffs.Mounts;
using CalamityMod.CalPlayer;
using CalamityMod.NPCs.TownNPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;

namespace CalamityMod.Items.Mounts
{
    public class AlicornMount : ModMount
    {
        public override void SetStaticDefaults()
        {
            MountData.spawnDust = 234;
            MountData.spawnDustNoGravity = true;
            MountData.buff = ModContent.BuffType<AlicornBuff>();
            MountData.heightBoost = 35;
            MountData.fallDamage = 0f;
            MountData.runSpeed = 5.6f;
            MountData.dashSpeed = 17.6f;
            MountData.flightTimeMax = 9999;
            MountData.fatigueMax = 0;
            MountData.jumpHeight = 12;
            MountData.acceleration = 0.4f;
            MountData.jumpSpeed = 9.21f;
            MountData.swimSpeed = 4f;
            MountData.blockExtraJumps = false;
            MountData.totalFrames = 15;
            MountData.constantJump = false;
            int baseYOffset = 26;
            int[] array = new int[MountData.totalFrames];
            for (int l = 0; l < array.Length; l++)
            {
                array[l] = baseYOffset;
            }
            array[1] = array[3] = array[5] = array[7] = array[12] = baseYOffset - 2;
            MountData.playerYOffsets = array;
            MountData.xOffset = -4;
            MountData.bodyFrame = 3;
            MountData.yOffset = 5; //-8
            MountData.playerHeadOffset = 36; //30
            MountData.standingFrameCount = 1;
            MountData.standingFrameDelay = 12;
            MountData.standingFrameStart = 0;
            MountData.runningFrameCount = 8; //7
            MountData.runningFrameDelay = 42; //36
            MountData.runningFrameStart = 1; //9
            MountData.flyingFrameCount = 6; //0
            MountData.flyingFrameDelay = 4; //0
            MountData.flyingFrameStart = 9; //0
            MountData.inAirFrameCount = 1; //1
            MountData.inAirFrameDelay = 12; //12
            MountData.inAirFrameStart = 10; //10
            MountData.idleFrameCount = 1; //4
            MountData.idleFrameDelay = 12; //12
            MountData.idleFrameStart = 5;
            MountData.idleFrameLoop = true;
            MountData.swimFrameCount = MountData.inAirFrameCount;
            MountData.swimFrameDelay = MountData.inAirFrameDelay;
            MountData.swimFrameStart = MountData.inAirFrameStart;
            if (Main.netMode != NetmodeID.Server)
            {
                MountData.frontTextureExtra = ModContent.Request<Texture2D>("CalamityMod/Items/Mounts/AlicornMountExtra");
                MountData.textureWidth = MountData.backTexture.Width();
                MountData.textureHeight = MountData.backTexture.Height();
            }
        }

        public override void SetMount(Player player, ref bool skipDust)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].type == ModContent.NPCType<FAP>())
                {
                    Main.npc[i].active = false;
                    Main.npc[i].netUpdate = true;
                    break;
                }
            }
        }

        public override void Dismount(Player player, ref bool skipDust)
        {
            bool anyPlayerOnFabMount = false;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player2 = Main.player[i];
                if (!player2.active)
                    continue;

                // The player that is dismounting is technically not on the mount anymore.
                if (player2.Calamity().fab && player2.whoAmI != player.whoAmI)
                {
                    anyPlayerOnFabMount = true;
                    break;
                }
            }

            // Spawn Cirrus if no other players are on the Alicorn mount.
            if (!anyPlayerOnFabMount)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<FAP>()))
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        NPC.NewNPC(NPC.GetSource_TownSpawn(), (int)player.Center.X, (int)player.Center.Y, ModContent.NPCType<FAP>());
                }
            }
        }

        public override void UpdateEffects(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.fabsolVodka)
                player.GetDamage<GenericDamageClass>() += 0.1f;

            if (player.velocity.Length() > 9f)
            {
                int rand = Main.rand.Next(2);
                bool momo = false;
                if (rand == 1)
                {
                    momo = true;
                }
                Color meme;
                if (momo)
                {
                    meme = new Color(255, 68, 242);
                }
                else
                {
                    meme = new Color(25, 105, 255);
                }
                Rectangle rect = player.getRect();
                int dust = Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, 234, 0, 0, 0, meme);
                Main.dust[dust].noGravity = true;
            }

            if (player.velocity.Y != 0f)
            {
                if (player.mount.PlayerOffset == 28)
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
