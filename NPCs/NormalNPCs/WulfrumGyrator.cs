using System;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Sounds;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class WulfrumGyrator : ModNPC
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

        public bool Supercharged => SuperchargeTimer > 0;

        public const float PlayerTargetingThreshold = 90f;
        public const float PlayerSearchDistance = 500f;
        public const float StuckJumpPromptTime = 45f;
        public const float MaxMovementSpeedX = 6f;
        public const float JumpSpeed = -8f;
        public const float NPCGravity = 0.3f; // NPC.cs has this, but it's private, and there's no way in hell I'm using reflection.

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 10;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                SpriteDirection = 1
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            AIType = -1;
            NPC.aiStyle = -1;
            NPC.damage = 15;
            NPC.width = 40;
            NPC.height = 40;
            NPC.defense = 5;
            NPC.lifeMax = 18;
            NPC.knockBackResist = 0.15f;
            NPC.value = Item.buyPrice(0, 0, 1, 15);
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = CommonCalamitySounds.WulfrumNPCDeathSound;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<WulfrumGyratorBanner>();
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.WulfrumGyrator")
            });
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (Main.zenithWorld)
            {
                NPC.frameCounter += 1;
            }
            int frame = (int)(NPC.frameCounter / 5) % (Main.npcFrameCount[NPC.type] / 2);
            if (Supercharged)
                frame += Main.npcFrameCount[NPC.type] / 2;

            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            NPC.TargetClosest(Main.zenithWorld ? true : false);

            Player player = Main.player[NPC.target];

            if (Supercharged)
            {
                float chargeJumpSpeed = -12f;
                float maxHeight = chargeJumpSpeed * chargeJumpSpeed * (float)Math.Pow(Math.Sin(NPC.AngleTo(player.Center)), 2) / (4f * NPCGravity);
                bool jumpWouldHitPlayer = maxHeight > Math.Abs(player.Center.Y - NPC.Center.Y) && maxHeight < Math.Abs(player.Center.Y - NPC.Center.Y) + player.height;

                if (Main.netMode != NetmodeID.MultiplayerClient && jumpWouldHitPlayer && NPC.collideY && NPC.velocity.Y == 0f)
                {
                    NPC.velocity.Y = chargeJumpSpeed;
                    NPC.netSpam = 0;
                    NPC.netUpdate = true;
                }

                SuperchargeTimer--;
            }

            // Jump if there's an obstacle ahead.
            if (Main.netMode != NetmodeID.MultiplayerClient && HoleAtPosition(NPC.Center.X + NPC.velocity.X * 4f) && NPC.collideY && NPC.velocity.Y == 0f)
            {
                NPC.velocity.Y = JumpSpeed;
                NPC.netSpam = 0;
                NPC.netUpdate = true;
            }

            if (Collision.CanHitLine(player.position, player.width, player.height, NPC.position, NPC.width, NPC.height) &&
                Math.Abs(player.Center.X - NPC.Center.X) < PlayerSearchDistance &&
                Math.Abs(player.Center.X - NPC.Center.X) > PlayerTargetingThreshold)
            {
                int direction = Math.Sign(player.Center.X - NPC.Center.X) * (NPC.confused ? -1 : 1);
                if (NPC.direction != direction)
                {
                    NPC.direction = direction;
                    NPC.netSpam = 0;
                    NPC.netUpdate = true;
                }
                //GOTTA GO FAST (Legendary only)
                if (CalamityWorld.LegendaryMode)
                {
                    NPC.velocity *= 1.01f;
                    //Overcharged
                    if (Supercharged && Main.rand.NextBool(3))
                    {
                        int spark = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.UnitX * 6f * NPC.spriteDirection, Vector2.Zero, ModContent.ProjectileType<EGloveSpark>(), 10, 0f);
                        if (spark.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[spark].friendly = false;
                            Main.projectile[spark].hostile = true;
                            Main.projectile[spark].timeLeft = 60;
                        }
                    }
                }
            }
            else if (Main.netMode != NetmodeID.MultiplayerClient && NPC.collideX && NPC.collideY && NPC.velocity.Y == 0f)
            {
                NPC.velocity.Y = JumpSpeed;
                NPC.netSpam = 0;
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

            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, MaxMovementSpeedX * NPC.direction * (Supercharged ? 1.25f : 1f), Supercharged ? 0.02f : 0.0125f);
            Vector4 adjustedVectors = Collision.WalkDownSlope(NPC.position, NPC.velocity, NPC.width, NPC.height, NPCGravity);
            NPC.position = adjustedVectors.XY();
            NPC.velocity = adjustedVectors.ZW();
        }

        private bool HoleAtPosition(float xPosition)
        {
            int tileWidth = NPC.width / 16;
            xPosition = (int)(xPosition / 16f) - tileWidth;
            if (NPC.velocity.X > 0)
                xPosition += tileWidth;

            int tileY = (int)((NPC.position.Y + NPC.height) / 16f);
            for (int y = tileY; y < tileY + 2; y++)
            {
                for (int x = (int)xPosition; x < xPosition + tileWidth; x++)
                {
                    if (Main.tile[x, y].HasTile)
                        return false;
                }
            }

            return true;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || spawnInfo.Player.Calamity().ZoneSulphur || (!spawnInfo.Player.ZoneOverworldHeight && !Main.remixWorld) || (!spawnInfo.Player.ZoneNormalCaverns && spawnInfo.Player.ZoneGlowshroom && Main.remixWorld))
                return 0f;

            return (Main.remixWorld ? SpawnCondition.Cavern.Chance : SpawnCondition.OverworldDaySlime.Chance) * (Main.hardMode ? 0.010f : 0.135f) * (NPC.AnyNPCs(ModContent.NPCType<WulfrumAmplifier>()) ? 5.5f : 1f);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (!Main.dedServ)
            {
                for (int k = 0; k < 5; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 3, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (NPC.life <= 0)
                {
                    for (int k = 0; k < 20; k++)
                    {
                        Dust.NewDust(NPC.position, NPC.width, NPC.height, 3, hit.HitDirection, -1f, 0, default, 1f);
                    }

                    if (!Main.dedServ)
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("WulfrumGyratorGore").Type, 1f);

                        int randomGoreCount = Main.rand.Next(1, 4);
                        for (int i = 0; i < randomGoreCount; i++)
                        {
                            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("WulfrumEnemyGore" + Main.rand.Next(1, 11).ToString()).Type, 1f);
                        }
                    }
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<WulfrumMetalScrap>(), 1, 1, 2);
            npcLoot.Add(ModContent.ItemType<WulfrumBattery>(), new Fraction(7, 100));
            npcLoot.AddIf(info => info.npc.ModNPC<WulfrumGyrator>().Supercharged, ModContent.ItemType<EnergyCore>());
        }
    }
}
