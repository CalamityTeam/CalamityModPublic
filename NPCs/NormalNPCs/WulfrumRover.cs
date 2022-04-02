using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class WulfrumRover : ModNPC
    {
        public float TimeSpentStuck
        {
            get => npc.ai[0];
            set => npc.ai[0] = value;
        }

        public float SuperchargeTimer
        {
            get => npc.ai[3];
            set => npc.ai[3] = value;
        }

        public const float PlayerTargetingThreshold = 90f;
        public const float PlayerSearchDistance = 500f;
        public const float StuckJumpPromptTime = 90f;
        public const float MaxMovementSpeedX = 6f;
        public const float JumpSpeed = -4f;
        public bool Supercharged => SuperchargeTimer > 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Rover");
            Main.npcFrameCount[npc.type] = 16;
        }

        public override void SetDefaults()
        {
            aiType = -1;
            npc.aiStyle = -1;
            npc.damage = 10;
            npc.width = 40;
            npc.height = 40;
            npc.defense = 4;
            npc.lifeMax = 32;
            npc.knockBackResist = 0.15f;
            npc.value = Item.buyPrice(0, 0, 1, 15);
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            banner = npc.type;
            bannerItem = ModContent.ItemType<WulfrumRoverBanner>();
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToElectricity = true;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            int frame = (int)(npc.frameCounter / 5) % (Main.npcFrameCount[npc.type] / 2);
            if (Supercharged)
                frame += Main.npcFrameCount[npc.type] / 2;

            npc.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            npc.TargetClosest(false);

            if (Supercharged)
            {
                SuperchargeTimer--;
                npc.defense = 13;
            }
            else if (!Supercharged)
            {
                npc.defense = 4;
            }

            Player player = Main.player[npc.target];
            if (Collision.CanHitLine(player.position, player.width, player.height, npc.position, npc.width, npc.height) &&
                Math.Abs(player.Center.X - npc.Center.X) < PlayerSearchDistance &&
                Math.Abs(player.Center.X - npc.Center.X) > PlayerTargetingThreshold)
            {
                int direction = Math.Sign(player.Center.X - npc.Center.X) * (npc.confused ? -1 : 1);
                if (npc.direction != direction)
                {
                    npc.direction = direction;
                    npc.netUpdate = true;
                }
            }
            else if (npc.collideX)
            {
                npc.direction *= -1;
                npc.netUpdate = true;
            }

            if (npc.oldPosition == npc.position)
            {
                TimeSpentStuck++;
                if (Main.netMode != NetmodeID.MultiplayerClient && TimeSpentStuck > StuckJumpPromptTime)
                {
                    npc.velocity.Y = JumpSpeed;
                    TimeSpentStuck = 0f;
                    npc.netUpdate = true;
                }
            }
            else
                TimeSpentStuck = 0f;

            npc.spriteDirection = -npc.direction;
            npc.velocity.X = MathHelper.Lerp(npc.velocity.X, MaxMovementSpeedX * npc.direction, 0.0125f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float pylonMult = NPC.AnyNPCs(ModContent.NPCType<WulfrumPylon>()) ? 5.5f : 1f;
            if (spawnInfo.playerSafe || spawnInfo.player.Calamity().ZoneSulphur)
                return 0f;
            return SpawnCondition.OverworldDaySlime.Chance * (Main.hardMode ? 0.010f : 0.135f) * pylonMult;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (!Main.dedServ)
            {
                for (int k = 0; k < 4; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 3, hitDirection, -1f, 0, default, 1f);
                }
                if (npc.life <= 0)
                {
                    for (int k = 0; k < 20; k++)
                    {
                        Dust.NewDust(npc.position, npc.width, npc.height, 3, hitDirection, -1f, 0, default, 1f);
                    }
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (Supercharged)
            {
                Texture2D shieldTexture = ModContent.GetTexture("CalamityMod/NPCs/NormalNPCs/WulfrumRoverShield");
                Rectangle frame = shieldTexture.Frame(1, 11, 0, (int)(Main.GlobalTime * 8) % 11);
                spriteBatch.Draw(shieldTexture,
                                 npc.Center - Main.screenPosition + Vector2.UnitY * (npc.gfxOffY + 6f),
                                 frame,
                                 Color.White * 0.625f,
                                 npc.rotation,
                                 shieldTexture.Size() * 0.5f / new Vector2(1f, 11f),
                                 npc.scale + (float)Math.Cos(Main.GlobalTime) * 0.1f,
                                 SpriteEffects.None,
                                 0f);
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(npc, ModContent.ItemType<WulfrumShard>());
            DropHelper.DropItemCondition(npc, ModContent.ItemType<EnergyCore>(), Supercharged);
            DropHelper.DropItemChance(npc, ModContent.ItemType<RoverDrive>(), 10);
            DropHelper.DropItemChance(npc, ModContent.ItemType<WulfrumBattery>(), 0.07f);
        }
    }
}
