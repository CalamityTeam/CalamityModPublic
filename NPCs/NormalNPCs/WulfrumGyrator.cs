using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class WulfrumGyrator : ModNPC
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

        public bool Supercharged => SuperchargeTimer > 0;

        public const float PlayerTargetingThreshold = 90f;
        public const float PlayerSearchDistance = 500f;
        public const float StuckJumpPromptTime = 45f;
        public const float MaxMovementSpeedX = 6f;
        public const float JumpSpeed = -8f;
        public const float NPCGravity = 0.3f; // NPC.cs has this, but it's private, and there's no way in hell I'm using reflection.

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Gyrator");
            Main.npcFrameCount[npc.type] = 10;
        }

        public override void SetDefaults()
        {
            aiType = -1;
            npc.aiStyle = -1;
            npc.damage = 15;
            npc.width = 40;
            npc.height = 40;
            npc.defense = 5;
            npc.lifeMax = 18;
            npc.knockBackResist = 0.15f;
            npc.value = Item.buyPrice(0, 0, 1, 15);
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            banner = npc.type;
            bannerItem = ModContent.ItemType<WulfrumGyratorBanner>();
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

            Player player = Main.player[npc.target];

            if (Supercharged)
            {
                float chargeJumpSpeed = -12f;
                float maxHeight = chargeJumpSpeed * chargeJumpSpeed * (float)Math.Pow(Math.Sin(npc.AngleTo(player.Center)), 2) / (4f * NPCGravity);
                bool jumpWouldHitPlayer = maxHeight > Math.Abs(player.Center.Y - npc.Center.Y) && maxHeight < Math.Abs(player.Center.Y - npc.Center.Y) + player.height;

                if (Main.netMode != NetmodeID.MultiplayerClient && jumpWouldHitPlayer && npc.collideY && npc.velocity.Y == 0f)
                {
                    npc.velocity.Y = chargeJumpSpeed;
                    npc.netSpam = 0;
                    npc.netUpdate = true;
                }

                SuperchargeTimer--;
            }

            // Jump if there's an obstacle ahead.
            if (Main.netMode != NetmodeID.MultiplayerClient && HoleAtPosition(npc.Center.X + npc.velocity.X * 4f) && npc.collideY && npc.velocity.Y == 0f)
            {
                npc.velocity.Y = JumpSpeed;
                npc.netSpam = 0;
                npc.netUpdate = true;
            }

            if (Collision.CanHitLine(player.position, player.width, player.height, npc.position, npc.width, npc.height) &&
                Math.Abs(player.Center.X - npc.Center.X) < PlayerSearchDistance &&
                Math.Abs(player.Center.X - npc.Center.X) > PlayerTargetingThreshold)
            {
                int direction = Math.Sign(player.Center.X - npc.Center.X) * (npc.confused ? -1 : 1);
                if (npc.direction != direction)
                {
                    npc.direction = direction;
                    npc.netSpam = 0;
                    npc.netUpdate = true;
                }
            }
            else if (Main.netMode != NetmodeID.MultiplayerClient && npc.collideX && npc.collideY && npc.velocity.Y == 0f)
            {
                npc.velocity.Y = JumpSpeed;
                npc.netSpam = 0;
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

            npc.velocity.X = MathHelper.Lerp(npc.velocity.X, MaxMovementSpeedX * npc.direction * (Supercharged ? 1.25f : 1f), Supercharged ? 0.02f : 0.0125f);
            Vector4 adjustedVectors = Collision.WalkDownSlope(npc.position, npc.velocity, npc.width, npc.height, NPCGravity);
            npc.position = adjustedVectors.XY();
            npc.velocity = adjustedVectors.ZW();
        }

        private bool HoleAtPosition(float xPosition)
        {
            int tileWidth = npc.width / 16;
            xPosition = (int)(xPosition / 16f) - tileWidth;
            if (npc.velocity.X > 0)
                xPosition += tileWidth;

            int tileY = (int)((npc.position.Y + npc.height) / 16f);
            for (int y = tileY; y < tileY + 2; y++)
            {
                for (int x = (int)xPosition; x < xPosition + tileWidth; x++)
                {
                    if (Main.tile[x, y].active())
                        return false;
                }
            }

            return true;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float pylonMult = NPC.AnyNPCs(ModContent.NPCType<WulfrumPylon>()) ? 5.5f : 1f;
            if (spawnInfo.playerSafe || spawnInfo.player.Calamity().ZoneSulphur)
                return 0f;
            return SpawnCondition.OverworldDaySlime.Chance * (Main.hardMode ? 0.020f : 0.115f) * pylonMult;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (!Main.dedServ)
            {
                for (int k = 0; k < 5; k++)
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

        public override void NPCLoot()
        {
            DropHelper.DropItem(npc, ModContent.ItemType<WulfrumShard>(), 1, 2);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<EnergyCore>(), Supercharged);
            DropHelper.DropItemChance(npc, ModContent.ItemType<WulfrumBattery>(), 0.07f);
        }
    }
}
