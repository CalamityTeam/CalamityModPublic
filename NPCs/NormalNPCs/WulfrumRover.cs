using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class WulfrumRover : ModNPC
    {
        public float TimeSpentStuck
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        public float SuperchargeTimer
        {
            get => NPC.ai[3];
            set => NPC.ai[3] = value;
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
            Main.npcFrameCount[NPC.type] = 16;
        }

        public override void SetDefaults()
        {
            AIType = -1;
            NPC.aiStyle = -1;
            NPC.damage = 10;
            NPC.width = 40;
            NPC.height = 40;
            NPC.defense = 4;
            NPC.lifeMax = 32;
            NPC.knockBackResist = 0.15f;
            NPC.value = Item.buyPrice(0, 0, 1, 15);
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<WulfrumRoverBanner>();
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("An aged automaton of unknown origins. With an incessant sense of duty and worn frame, they scour the land. Their lamps shine with curious energy.")
            });
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            int frame = (int)(NPC.frameCounter / 5) % (Main.npcFrameCount[NPC.type] / 2);
            if (Supercharged)
                frame += Main.npcFrameCount[NPC.type] / 2;

            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            NPC.TargetClosest(false);

            if (Supercharged)
            {
                SuperchargeTimer--;
                NPC.defense = 13;
            }
            else if (!Supercharged)
            {
                NPC.defense = 4;
            }

            Player player = Main.player[NPC.target];
            if (Collision.CanHitLine(player.position, player.width, player.height, NPC.position, NPC.width, NPC.height) &&
                Math.Abs(player.Center.X - NPC.Center.X) < PlayerSearchDistance &&
                Math.Abs(player.Center.X - NPC.Center.X) > PlayerTargetingThreshold)
            {
                int direction = Math.Sign(player.Center.X - NPC.Center.X) * (NPC.confused ? -1 : 1);
                if (NPC.direction != direction)
                {
                    NPC.direction = direction;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.collideX)
            {
                NPC.direction *= -1;
                NPC.netUpdate = true;
            }

            if (NPC.oldPosition == NPC.position)
            {
                TimeSpentStuck++;
                if (Main.netMode != NetmodeID.MultiplayerClient && TimeSpentStuck > StuckJumpPromptTime)
                {
                    NPC.velocity.Y = JumpSpeed;
                    TimeSpentStuck = 0f;
                    NPC.netUpdate = true;
                }
            }
            else
                TimeSpentStuck = 0f;

            NPC.spriteDirection = -NPC.direction;
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, MaxMovementSpeedX * NPC.direction, 0.0125f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float pylonMult = NPC.AnyNPCs(ModContent.NPCType<WulfrumPylon>()) ? 5.5f : 1f;
            if (spawnInfo.PlayerSafe || spawnInfo.Player.Calamity().ZoneSulphur)
                return 0f;
            return SpawnCondition.OverworldDaySlime.Chance * (Main.hardMode ? 0.010f : 0.135f) * pylonMult;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (!Main.dedServ)
            {
                for (int k = 0; k < 4; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 3, hitDirection, -1f, 0, default, 1f);
                }
                if (NPC.life <= 0)
                {
                    for (int k = 0; k < 20; k++)
                    {
                        Dust.NewDust(NPC.position, NPC.width, NPC.height, 3, hitDirection, -1f, 0, default, 1f);
                    }
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Supercharged)
            {
                Texture2D shieldTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/NormalNPCs/WulfrumRoverShield").Value;
                Rectangle frame = shieldTexture.Frame(1, 11, 0, (int)(Main.GlobalTimeWrappedHourly * 8) % 11);
                spriteBatch.Draw(shieldTexture,
                                 NPC.Center - screenPos + Vector2.UnitY * (NPC.gfxOffY + 6f),
                                 frame,
                                 Color.White * 0.625f,
                                 NPC.rotation,
                                 shieldTexture.Size() * 0.5f / new Vector2(1f, 11f),
                                 NPC.scale + (float)Math.Cos(Main.GlobalTimeWrappedHourly) * 0.1f,
                                 SpriteEffects.None,
                                 0f);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<WulfrumShard>(), 1, 1, 2);
            npcLoot.Add(ModContent.ItemType<RoverDrive>(), 10);
            npcLoot.Add(ModContent.ItemType<WulfrumBattery>(), new Fraction(7, 100));
            npcLoot.AddIf(info => info.npc.ModNPC<WulfrumRover>().Supercharged, ModContent.ItemType<EnergyCore>());
        }
    }
}
