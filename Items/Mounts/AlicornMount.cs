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
    public class AlicornMount : ModMountData
    {
        public override void SetDefaults()
        {
            mountData.spawnDust = 234;
            mountData.spawnDustNoGravity = true;
            mountData.buff = ModContent.BuffType<AlicornBuff>();
            mountData.heightBoost = 35;
            mountData.fallDamage = 0f;
            mountData.runSpeed = 5.6f;
            mountData.dashSpeed = 16.8f;
            mountData.flightTimeMax = 500;
            mountData.fatigueMax = 0;
            mountData.jumpHeight = 12;
            mountData.acceleration = 0.4f;
            mountData.jumpSpeed = 8.01f;
            mountData.swimSpeed = 4f;
            mountData.blockExtraJumps = false;
            mountData.totalFrames = 15;
            mountData.constantJump = false;
            int baseYOffset = 26;
            int[] array = new int[mountData.totalFrames];
            for (int l = 0; l < array.Length; l++)
            {
                array[l] = baseYOffset;
            }
            array[1] = array[3] = array[5] = array[7] = array[12] = baseYOffset - 2;
            mountData.playerYOffsets = array;
            mountData.xOffset = -4;
            mountData.bodyFrame = 3;
            mountData.yOffset = 5; //-8
            mountData.playerHeadOffset = 36; //30
            mountData.standingFrameCount = 1;
            mountData.standingFrameDelay = 12;
            mountData.standingFrameStart = 0;
            mountData.runningFrameCount = 8; //7
            mountData.runningFrameDelay = 42; //36
            mountData.runningFrameStart = 1; //9
            mountData.flyingFrameCount = 6; //0
            mountData.flyingFrameDelay = 4; //0
            mountData.flyingFrameStart = 9; //0
            mountData.inAirFrameCount = 1; //1
            mountData.inAirFrameDelay = 12; //12
            mountData.inAirFrameStart = 10; //10
            mountData.idleFrameCount = 1; //4
            mountData.idleFrameDelay = 12; //12
            mountData.idleFrameStart = 5;
            mountData.idleFrameLoop = true;
            mountData.swimFrameCount = mountData.inAirFrameCount;
            mountData.swimFrameDelay = mountData.inAirFrameDelay;
            mountData.swimFrameStart = mountData.inAirFrameStart;
            if (Main.netMode != NetmodeID.Server)
            {
                mountData.frontTextureExtra = ModContent.Request<Texture2D>("CalamityMod/Items/Mounts/AlicornMountExtra");
                mountData.textureWidth = mountData.backTexture.Width;
                mountData.textureHeight = mountData.backTexture.Height;
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
                        NPC.NewNPC((int)player.Center.X, (int)player.Center.Y, ModContent.NPCType<FAP>());
                }
            }
        }

        public override void UpdateEffects(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.fabsolVodka)
                player.allDamage += 0.1f;

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
