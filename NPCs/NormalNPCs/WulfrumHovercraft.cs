using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using CalamityMod.Sounds;
using Terraria.Audio;
using CalamityMod.World;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Pets;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class WulfrumHovercraft : ModNPC
    {
        internal enum HovercraftAIState
        {
            Searching = 0,
            Hover = 1,
            Slowdown = 2,
            SwoopDownward = 3
        }

        public float StunTime;

        internal HovercraftAIState AIState
        {
            get => (HovercraftAIState)(int)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public float SubphaseTime
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        public float SearchDirection
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }

        public float SuperchargeTimer
        {
            get => NPC.ai[3];
            set => NPC.ai[3] = value;
        }

        public bool Supercharged => SuperchargeTimer > 0;
        public ref float FlyAwayTimer => ref NPC.localAI[0];

        public const float StunTimeMax = 45f;
        public const float SearchXOffset = 345f;
        public const float TotalSubphaseTime = 110f;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 12;
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
            NPC.height = 38;
            NPC.defense = 4;
            NPC.lifeMax = 20;
            NPC.value = Item.buyPrice(0, 0, 1, 50);
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = CommonCalamitySounds.WulfrumNPCDeathSound;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<WulfrumHovercraftBanner>();
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.WulfrumHovercraft")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(StunTime);
            writer.Write(FlyAwayTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            StunTime = reader.ReadSingle();
            FlyAwayTimer = reader.ReadSingle();
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
            NPC.knockBackResist = 0.1f;

            Player player = Main.player[NPC.target];

            bool farFromPlayer = NPC.Distance(player.Center) > 960f;
            bool obstanceInFrontOfPlayer = Main.remixWorld ? false : !Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);

            if (NPC.target < 0 || NPC.target >= 255 || farFromPlayer || obstanceInFrontOfPlayer || player.dead || !player.active)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                farFromPlayer = NPC.Distance(player.Center) > 960f;
                obstanceInFrontOfPlayer = !Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
                // Fly away if there is no living target, or the closest target is too far away... unless its Gfb
                if (player.dead || !player.active || farFromPlayer || obstanceInFrontOfPlayer)
                {
                    if (FlyAwayTimer > 420)
                    {
                        // Don't go away from me >:(
                        if (Main.zenithWorld && player.active && !farFromPlayer)
                        {
                            AIState = HovercraftAIState.SwoopDownward;
                            SoundEngine.PlaySound(SoundID.DD2_KoboldFlyerHurt with { Pitch = SoundID.DD2_KoboldFlyerHurt.Pitch + 0.5f }, NPC.Center); 
                            return;
                        }
                        NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.UnitY * -8f, 0.1f);
                        NPC.rotation = NPC.rotation.AngleTowards(0f, MathHelper.ToRadians(15f));
                        NPC.noTileCollide = true;
                    }
                    else
                    {
                        NPC.velocity *= 0.96f;
                        NPC.rotation = NPC.rotation.AngleTowards(0f, MathHelper.ToRadians(15f));
                        FlyAwayTimer++;
                    }
                    return;
                }
            }

            FlyAwayTimer = Utils.Clamp(FlyAwayTimer - 3, 0, 180);

            NPC.noTileCollide = !farFromPlayer;

            Lighting.AddLight(NPC.Center - Vector2.UnitY * 8f, Color.Lime.ToVector3() * 1.5f);

            if (StunTime > 0)
            {
                if (!Main.dedServ && Main.rand.NextBool(4))
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(8f, 8f), 226).scale = 0.7f;
                    }
                }

                NPC.rotation = NPC.rotation.AngleTowards(0f, MathHelper.ToRadians(15f));
                if (StunTime > StunTimeMax - 15)
                    NPC.velocity *= 0.6f;
                else
                    NPC.knockBackResist = 2.4f;

                StunTime--;
                return;
            }

            if (SearchDirection == 0f)
            {
                if (Math.Abs(player.Center.X + SearchXOffset - NPC.Center.X) < Math.Abs(player.Center.X - SearchXOffset - NPC.Center.X))
                    SearchDirection = 1f;
                else
                    SearchDirection = -1f;

                NPC.netUpdate = true;
            }

            if (AIState == HovercraftAIState.Searching || AIState == HovercraftAIState.Hover)
            {
                Vector2 destination = player.Center + new Vector2(SearchXOffset * SearchDirection, -160f);
                NPC.velocity = NPC.SafeDirectionTo(destination, Vector2.UnitY) * (Supercharged ? 7f : 5f);
                if (AIState == HovercraftAIState.Hover)
                {
                    destination = player.Center + new Vector2(SearchXOffset * -SearchDirection, -160f);
                    NPC.velocity = NPC.SafeDirectionTo(destination, Vector2.UnitY) * (Supercharged ? 5.5f : 4f);
                }

                NPC.rotation = NPC.velocity.X / 16f;
                if (NPC.Distance(destination) < 50f)
                {
                    if (AIState == HovercraftAIState.Searching)
                    {
                        AIState = HovercraftAIState.Slowdown;
                    }
                    else
                    {
                        AIState = HovercraftAIState.SwoopDownward;
                    }
                    NPC.netUpdate = true;
                }
            }

            if (AIState == HovercraftAIState.Slowdown)
            {
                SubphaseTime++;
                if (SubphaseTime < 30f)
                {
                    NPC.velocity *= 0.96f;
                }
                else
                {
                    AIState = HovercraftAIState.Hover;
                    SubphaseTime = 0f;
                    NPC.netUpdate = true;
                }
            }

            if (AIState == HovercraftAIState.SwoopDownward)
            {
                NPC.rotation = 0f;
                float swoopType = Supercharged ? TotalSubphaseTime - 40f : TotalSubphaseTime;
                float swoopSlowdownTime = Supercharged ? 10f : 45f;
                Vector2 swoopVelocity = Vector2.UnitY.RotatedBy(MathHelper.Pi * SubphaseTime / swoopType * -SearchDirection) * (Supercharged ? 11f : 8.5f);

                SubphaseTime++;
                if (SubphaseTime < swoopSlowdownTime)
                {
                    swoopVelocity *= MathHelper.Lerp(1f, 0.75f, Utils.GetLerpValue(45f, 0f, SubphaseTime));
                }
                if (SubphaseTime >= swoopType - swoopSlowdownTime)
                {
                    swoopVelocity *= MathHelper.Lerp(1f, 0.75f, Utils.GetLerpValue(swoopType - 45f, swoopType, SubphaseTime));
                }
                swoopVelocity.Y *= 0.5f;

                NPC.velocity = Vector2.Lerp(NPC.velocity, swoopVelocity, 0.2f);

                if (SubphaseTime >= swoopType)
                {
                    AIState = HovercraftAIState.Searching;
                    SearchDirection = 0f;
                    SubphaseTime = 0f;
                    NPC.netUpdate = true;
                }

                NPC.rotation = NPC.velocity.X / 12f;
            }

            NPC.spriteDirection = (NPC.velocity.X < 0).ToDirectionInt();
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
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("WulfrumHovercraftGore1").Type, 1f);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("WulfrumHovercraftGore2").Type, 1f);

                        int randomGoreCount = Main.rand.Next(1, 4);
                        for (int i = 0; i < randomGoreCount; i++)
                        {
                            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("WulfrumEnemyGore" + Main.rand.Next(1, 11).ToString()).Type, 1f);
                        }
                    }
                }
                //Become a spark piñata in Legendary
                if (CalamityWorld.LegendaryMode && Supercharged)
                {
                    for (int Sparks = Main.rand.Next(2, 5); Sparks > 0; Sparks--)
                    {
                        Vector2 velocity = CalamityUtils.RandomVelocity(50f, 30f, 60f);
                        int spark = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.UnitX * 6f * NPC.spriteDirection, velocity, ModContent.ProjectileType<EGloveSpark>(), 10, 0f);
                        if (spark.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[spark].friendly = false;
                            Main.projectile[spark].hostile = true;
                            Main.projectile[spark].timeLeft = 90;
                        }
                    }
                }
            }
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            StunTime = StunTimeMax;
            NPC.netUpdate = true;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<WulfrumMetalScrap>(), 1, 2, 3);
            npcLoot.Add(ModContent.ItemType<WulfrumBattery>(), new Fraction(7, 100));
            npcLoot.AddIf(info => info.npc.ModNPC<WulfrumHovercraft>().Supercharged, ModContent.ItemType<EnergyCore>());
        }
    }
}
