using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using CalamityMod.World;
using CalamityMod.Sounds;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class WulfrumAmplifier : ModNPC
    {
        public bool Charging
        {
            get => NPC.ai[0] != 0f;
            set => NPC.ai[0] = value.ToInt();
        }
        public float ChargeRadius
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }
        public const float ChargeRadiusMax = 495f;
        public const float SuperchargeTime = 720f;

        public int laserDelay = 150;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
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
            NPC.damage = 10;
            NPC.width = 44;
            NPC.height = 44;
            NPC.defense = 4;
            NPC.lifeMax = Main.zenithWorld ? 72: 46;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 50);
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = CommonCalamitySounds.WulfrumNPCDeathSound;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<WulfrumAmplifierBanner>();
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            if (Main.zenithWorld)
                NPC.scale = 1.5f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.WulfrumAmplifier")
            });
        }

        public override void AI()
        {
            // Setting this in SetDefaults will disable expert mode scaling, so put it here instead
            NPC.damage = 0;

            List<int> SuperchargableEnemies = new List<int>()
            {
                ModContent.NPCType<WulfrumDrone>(),
                ModContent.NPCType<WulfrumGyrator>(),
                ModContent.NPCType<WulfrumHovercraft>(),
                ModContent.NPCType<WulfrumRover>()
            };

            NPC.TargetClosest(false);

            Player player = Main.player[NPC.target];

            if (!Charging && NPC.Distance(player.Center) < ChargeRadiusMax * 0.667f)
            {
                // Spawn some off-screen enemies to act as threats if the player enters the field.
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int enemiesToSpawn = CalamityWorld.LegendaryMode ? 4 :CalamityWorld.death ? 3 : CalamityWorld.revenge ? 2 : 1;
                    for (int i = 0; i < enemiesToSpawn; i++)
                    {
                        int tries = 0;
                        Vector2 spawnPosition;
                        do
                        {
                            spawnPosition = player.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(600f, 1015f) * new Vector2(1.5f, 1f);
                            if (spawnPosition.Y > player.Center.Y)
                                spawnPosition.Y = player.Center.Y;
                            if (tries > 500)
                                break;
                            tries++;
                        }
                        while (WorldGen.SolidTile(CalamityUtils.ParanoidTileRetrieval((int)spawnPosition.X / 16, (int)spawnPosition.Y / 16)));

                        if (tries < 500 && !Main.zenithWorld)
                        {
                            int npcToSpawn = Main.rand.NextBool() ? ModContent.NPCType<WulfrumDrone>() : ModContent.NPCType<WulfrumHovercraft>();
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnPosition.X, (int)spawnPosition.Y, npcToSpawn);
                        }
                        else if (tries < 500 && Main.zenithWorld)
                        {
                            //Summon the army
                            int npcToSpawn = CalamityWorld.LegendaryMode ? 0 : Main.rand.Next(0,4);
                            switch (enemiesToSpawn){
                                case 0: npcToSpawn = ModContent.NPCType<WulfrumDrone>();
                                    break;
                                case 1: npcToSpawn = ModContent.NPCType<WulfrumHovercraft>();
                                    break;
                                case 2: npcToSpawn = ModContent.NPCType<WulfrumGyrator>();
                                    break;
                                case 3: npcToSpawn = ModContent.NPCType<WulfrumRover>();
                                    break;
                            }
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnPosition.X, (int)spawnPosition.Y, npcToSpawn);
                        }
                    }
                }

                Charging = true;
                NPC.netUpdate = true;
            }
            else if (Charging)
            {
                ChargeRadius = (int)MathHelper.Lerp(ChargeRadius, ChargeRadiusMax, 0.1f);

                if (Main.rand.NextBool(4))
                {
                    float dustCount = MathHelper.TwoPi * ChargeRadius / 8f;
                    for (int i = 0; i < dustCount; i++)
                    {
                        float angle = MathHelper.TwoPi * i / dustCount;
                        Dust dust = Dust.NewDustPerfect(NPC.Center, 229);
                        dust.position = NPC.Center + angle.ToRotationVector2() * ChargeRadius;
                        dust.scale = 0.7f;
                        dust.noGravity = true;
                        dust.velocity = NPC.velocity;
                    }
                }

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npcAtIndex = Main.npc[i];
                    if (!npcAtIndex.active)
                        continue;

                    // For some strange reason, the Wulfrum enemies are not counted when SuperchargableEnemies is a static list declared up front.
                    // What I assume is going on is that it hasn't been loaded yet since it's later alphabetically (Amplifier is before the other enemies).
                    if (!SuperchargableEnemies.Contains(npcAtIndex.type) && npcAtIndex.type != ModContent.NPCType<WulfrumRover>())
                        continue;
                    if (npcAtIndex.ai[3] > 0f)
                        continue;
                    if (NPC.Distance(npcAtIndex.Center) > ChargeRadius)
                        continue;

                    npcAtIndex.ai[3] = SuperchargeTime; // Supercharge the npc for a while if isn't already supercharged.
                    npcAtIndex.netUpdate = true;

                    // And emit some dust.

                    // Dust doesn't need to be spawned for the server.
                    if (Main.dedServ)
                        continue;

                    for (int j = 0; j < 10; j++)
                    {
                        Dust.NewDust(npcAtIndex.position, npcAtIndex.width, npcAtIndex.height, 226);
                    }
                }
                if (CalamityWorld.LegendaryMode)
                {
                    laserDelay--;
                    NPC.spriteDirection = (player.Center.X - NPC.Center.X < 0).ToDirectionInt();
                    for (int times = CalamityWorld.LegendaryMode ? 3 : 2; times > 0 && laserDelay == 0; times--)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.UnitX * 6f * NPC.spriteDirection, NPC.SafeDirectionTo(player.Center, Vector2.UnitY) * 4.5f, ProjectileID.SaucerMissile, 10, 0f);
                    }
                    if (laserDelay <= 0)
                        laserDelay = 150;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            int frame = (int)(NPC.frameCounter / 8) % Main.npcFrameCount[NPC.type];

            NPC.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || spawnInfo.Player.Calamity().ZoneSulphur || (!spawnInfo.Player.ZoneOverworldHeight && !Main.remixWorld) || (!spawnInfo.Player.ZoneNormalCaverns && spawnInfo.Player.ZoneGlowshroom && Main.remixWorld))
                return 0f;

            // Spawn less frequently in the inner third of the world.
            if (spawnInfo.PlayerFloorX > Main.maxTilesX * 0.333f && spawnInfo.PlayerFloorX < Main.maxTilesX - Main.maxTilesX * 0.333f)
                return (Main.remixWorld ? SpawnCondition.Cavern.Chance : SpawnCondition.OverworldDaySlime.Chance) * (Main.hardMode ? 0.01f : 0.06f) * (!NPC.AnyNPCs(NPC.type) ? 1.3f : 1f);

            return (Main.remixWorld ? SpawnCondition.Cavern.Chance : SpawnCondition.OverworldDaySlime.Chance) * (Main.hardMode ? 0.033f : 0.15f) * (!NPC.AnyNPCs(NPC.type) ? 1.3f : 1f);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GrassBlades, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GrassBlades, hit.HitDirection, -1f, 0, default, 1f);
                }

                if (!Main.dedServ)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("WulfrumAmplifierGore").Type, 1f);

                    int randomGoreCount = Main.rand.Next(1, 4);
                    for (int i = 0; i < randomGoreCount; i++)
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("WulfrumEnemyGore" + Main.rand.Next(1, 11).ToString()).Type, 1f);
                    }
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<WulfrumMetalScrap>(), 1, 2, 3);
            npcLoot.Add(ModContent.ItemType<WulfrumBattery>(), new Fraction(7, 100));
            npcLoot.Add(ModContent.ItemType<AbandonedWulfrumHelmet>(), new Fraction(5, 100));
            npcLoot.Add(ModContent.ItemType<EnergyCore>());
        }
    }
}
