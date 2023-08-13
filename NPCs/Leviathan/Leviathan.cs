using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Boss;
using CalamityMod.UI.VanillaBossBars;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using ReLogic.Content;

namespace CalamityMod.NPCs.Leviathan
{
    [AutoloadBossHead]
    public class Leviathan : ModNPC
    {
        private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;
        private int counter = 0;
        private bool initialised = false;
        private bool gfbAnaSummoned = false;
        private int soundDelay = 0;
        private float extrapitch = 0;
        public static Texture2D AttackTexture = null;

        public static readonly SoundStyle RoarMeteorSound = new("CalamityMod/Sounds/Custom/LeviathanRoarMeteor");
        public static readonly SoundStyle RoarChargeSound = new("CalamityMod/Sounds/Custom/LeviathanRoarCharge");
        public static readonly SoundStyle EmergeSound = new("CalamityMod/Sounds/Custom/LeviathanEmerge");

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0);
            if (!Main.dedServ)
                AttackTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Leviathan/LeviathanAttack", AssetRequestMode.ImmediateLoad).Value;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.npcSlots = 20f;
            NPC.GetNPCDamage();
            NPC.width = 900;
            NPC.height = 450;
            NPC.defense = 40;
            NPC.DR_NERD(0.35f);
            NPC.LifeMaxNERB(60000, 72000, 600000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.Opacity = 0f;
            NPC.value = Item.buyPrice(0, 60, 0, 0);
            NPC.HitSound = SoundID.NPCHit56;
            NPC.DeathSound = SoundID.NPCDeath60;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.BossBar = ModContent.GetInstance<LeviathanAnahitaBossBar>();
            NPC.netAlways = true;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;

            if (Main.getGoodWorld)
                NPC.scale *= 1.3f;

            if (Main.zenithWorld)
                NPC.scale *= 0.3f; 
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Leviathan")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(biomeEnrageTimer);
            writer.Write(NPC.dontTakeDamage);
            writer.Write(soundDelay);
            writer.Write(NPC.Calamity().newAI[3]);
            writer.Write(gfbAnaSummoned);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            biomeEnrageTimer = reader.ReadInt32();
            NPC.dontTakeDamage = reader.ReadBoolean();
            soundDelay = reader.ReadInt32();
            NPC.Calamity().newAI[3] = reader.ReadSingle();
            gfbAnaSummoned = reader.ReadBoolean();
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            CalamityGlobalNPC.leviathan = NPC.whoAmI;

            // This dictates the Leviathan music scene
            if (CalamityGlobalNPC.LeviAndAna == -1)
                CalamityGlobalNPC.LeviAndAna = NPC.whoAmI;

            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool expertMode = Main.expertMode || bossRush;
            Vector2 vector = NPC.Center;

            // Is in spawning animation
            float spawnAnimationTime = 180f;
            bool spawnAnimation = calamityGlobalNPC.newAI[3] < spawnAnimationTime;

            // Don't do damage during spawn animation
            NPC.damage = NPC.defDamage;
            if (spawnAnimation)
                NPC.damage = 0;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phases
            bool phase2 = (lifeRatio < 0.7f || death) && expertMode;
            bool phase3 = lifeRatio < (death ? 0.7f : 0.4f);
            bool phase4 = lifeRatio < (death ? 0.4f : 0.2f);

            bool sirenAlive = false;
            if (CalamityGlobalNPC.siren != -1)
                sirenAlive = Main.npc[CalamityGlobalNPC.siren].active;

            if (CalamityGlobalNPC.siren != -1)
            {
                if (Main.npc[CalamityGlobalNPC.siren].active)
                {
                    if (Main.npc[CalamityGlobalNPC.siren].damage == 0)
                        sirenAlive = false;
                }
            }

            if (phase2 && !sirenAlive && Main.zenithWorld && !gfbAnaSummoned)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int siren = NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X, (int)NPC.position.Y + NPC.height - 1000, ModContent.NPCType<Anahita>(), NPC.whoAmI);
                    CalamityUtils.BossAwakenMessage(siren);
                }
                gfbAnaSummoned = true;
            }

            SoundStyle soundChoiceRage = SoundID.Zombie92;
            SoundStyle soundChoice = Utils.SelectRandom(Main.rand, new SoundStyle[]
            {
                SoundID.Zombie38,
                SoundID.Zombie39,
                SoundID.Zombie49
            });

            if (soundDelay > 0)
                soundDelay--;

            extrapitch = Main.zenithWorld ? 0.3f : 0f;

            if (Main.rand.NextBool(600) && !spawnAnimation)
                SoundEngine.PlaySound(((sirenAlive && !death) ? soundChoice : soundChoiceRage) with { Pitch = soundChoice.Pitch + extrapitch }, vector);

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, vector) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            bool notOcean = player.position.Y < 800f || player.position.Y > Main.worldSurface * 16.0 || (player.position.X > 6400f && player.position.X < (Main.maxTilesX * 16 - 6400));

            // Enrage
            if (notOcean && !bossRush)
            {
                if (biomeEnrageTimer > 0)
                    biomeEnrageTimer--;
            }
            else
                biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

            bool biomeEnraged = biomeEnrageTimer <= 0 || bossRush;

            float enrageScale = bossRush ? 1f : 0f;
            if (biomeEnraged)
            {
                NPC.Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 2f;
            }

            NPC.dontTakeDamage = spawnAnimation;

            // Adjust slowing debuff immunity
            bool immuneToSlowingDebuffs = NPC.ai[0] == 2f;
            NPC.buffImmune[ModContent.BuffType<GlacialState>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<TemporalSadness>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<KamiFlu>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<Eutrophication>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<TimeDistortion>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<GalvanicCorrosion>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<Vaporfied>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[BuffID.Slow] = immuneToSlowingDebuffs;
            NPC.buffImmune[BuffID.Webbed] = immuneToSlowingDebuffs;

            if (!player.active || player.dead || Vector2.Distance(player.Center, vector) > 5600f)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead || Vector2.Distance(player.Center, vector) > 5600f)
                {
                    if (NPC.velocity.Y < -3f)
                        NPC.velocity.Y = -3f;
                    NPC.velocity.Y += 0.2f;
                    if (NPC.velocity.Y > 16f)
                        NPC.velocity.Y = 16f;

                    if (NPC.position.Y > Main.worldSurface * 16.0)
                    {
                        for (int x = 0; x < Main.maxNPCs; x++)
                        {
                            if (Main.npc[x].type == ModContent.NPCType<Anahita>())
                            {
                                Main.npc[x].active = false;
                                Main.npc[x].netUpdate = true;
                            }
                        }
                        NPC.active = false;
                        NPC.netUpdate = true;
                    }

                    if (NPC.ai[0] != 0f)
                    {
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.netUpdate = true;
                    }

                    return;
                }
            }
            else
            {
                bool canCharge = ((phase2 || phase3) && !sirenAlive) || phase4 || death;

                // Slowly drift up when spawning
                if (spawnAnimation)
                {
                    float minSpawnVelocity = 0.4f;
                    float maxSpawnVelocity = 4f;
                    float velocityY = maxSpawnVelocity - MathHelper.Lerp(minSpawnVelocity, maxSpawnVelocity, calamityGlobalNPC.newAI[3] / spawnAnimationTime);
                    NPC.velocity = new Vector2(0f, -velocityY);

                    if (calamityGlobalNPC.newAI[3] == 10f)
                    {
                        SoundEngine.PlaySound(EmergeSound, vector);
                        SoundEngine.PlaySound(soundChoiceRage with { Pitch = soundChoiceRage.Pitch + extrapitch }, vector);
                    }

                    NPC.Opacity = MathHelper.Clamp(calamityGlobalNPC.newAI[3] / spawnAnimationTime, 0f, 1f);

                    calamityGlobalNPC.newAI[3] += 1f;

                    return;
                }

                if (NPC.ai[0] == 0f)
                {
                    float num412 = (sirenAlive && !phase4) ? 3.5f : 7f;
                    float num413 = (sirenAlive && !phase4) ? 0.1f : 0.2f;
                    num412 += 2f * enrageScale;
                    num413 += 0.05f * enrageScale;

                    if (expertMode && (!sirenAlive || phase4))
                    {
                        num412 += death ? 6f * (1f - lifeRatio) : 3.5f * (1f - lifeRatio);
                        num413 += death ? 0.15f * (1f - lifeRatio) : 0.1f * (1f - lifeRatio);
                    }

                    if (Main.getGoodWorld)
                    {
                        num412 *= 1.15f;
                        num413 *= 1.15f;
                    }

                    int num414 = 1;
                    if (vector.X < player.position.X + player.width)
                        num414 = -1;

                    Vector2 vector40 = vector;
                    float num415 = player.Center.X + (num414 * ((sirenAlive && !phase4) ? 1000f : 800f) * NPC.scale) - vector40.X;
                    float num416 = player.Center.Y - vector40.Y;
                    float num417 = (float)Math.Sqrt(num415 * num415 + num416 * num416);
                    num417 = num412 / num417;
                    num415 *= num417;
                    num416 *= num417;

                    if (NPC.velocity.X < num415)
                    {
                        NPC.velocity.X += num413;
                        if (NPC.velocity.X < 0f && num415 > 0f)
                            NPC.velocity.X += num413;
                    }
                    else if (NPC.velocity.X > num415)
                    {
                        NPC.velocity.X -= num413;
                        if (NPC.velocity.X > 0f && num415 < 0f)
                            NPC.velocity.X -= num413;
                    }

                    if (NPC.velocity.Y < num416)
                    {
                        NPC.velocity.Y += num413;
                        if (NPC.velocity.Y < 0f && num416 > 0f)
                            NPC.velocity.Y += num413;
                    }
                    else if (NPC.velocity.Y > num416)
                    {
                        NPC.velocity.Y -= num413;
                        if (NPC.velocity.Y > 0f && num416 < 0f)
                            NPC.velocity.Y -= num413;
                    }

                    float playerLocation = vector.X - player.Center.X;
                    NPC.direction = playerLocation < 0 ? 1 : -1;
                    NPC.spriteDirection = NPC.direction;

                    NPC.ai[1] += 1f;
                    float phaseTimer = 360f;
                    if (!sirenAlive || phase4)
                        phaseTimer -= 120f * (1f - lifeRatio);

                    if (NPC.ai[1] >= phaseTimer)
                    {
                        NPC.ai[0] = canCharge ? (Main.rand.NextBool() ? 2f : 1f) : 1f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.TargetClosest();
                        NPC.netUpdate = true;
                    }
                    else
                    {
                        if (!player.dead)
                        {
                            NPC.ai[2] += 1f;
                            if (!sirenAlive || phase4)
                                NPC.ai[2] += 1f;
                        }

                        if (NPC.ai[2] >= 75f)
                        {
                            NPC.ai[2] = 0f;
                            vector40 = new Vector2(vector.X, vector.Y + 20f);
                            num415 = vector40.X + 1000f * NPC.direction - vector40.X;
                            num416 = player.Center.Y - vector40.Y;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float speed = (sirenAlive && !phase4 && !death) ? 13.5f : 16f;
                                int type = ModContent.ProjectileType<LeviathanBomb>();
                                int damage = NPC.GetProjectileDamage(type);

                                if (expertMode)
                                    speed = (sirenAlive && !phase4 && !death) ? 14f : 17f;

                                speed += 2f * enrageScale;

                                if (!sirenAlive || phase4)
                                    speed += 3f * (1f - lifeRatio);

                                num417 = (float)Math.Sqrt(num415 * num415 + num416 * num416);
                                num417 = speed / num417;
                                num415 *= num417;
                                num416 *= num417;
                                vector40.X += num415 * 4f;
                                vector40.Y += num416 * 4f;

                                if (Main.zenithWorld)
                                {
                                    type = (CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? ProjectileID.BouncyBoulder : ProjectileID.Boulder;
                                    vector40.Y -= 5; //Shoot a bit more up since boulders are affected by gravity
                                    damage *= 2;
                                }

                                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), vector40.X, vector40.Y, num415, num416, type, damage, 0f, Main.myPlayer);
                                if (Main.zenithWorld)
                                    Main.projectile[proj].scale *= 5f;

                                if (soundDelay <= 0)
                                {
                                    soundDelay = 120;
                                    SoundEngine.PlaySound(RoarMeteorSound with { Pitch = RoarMeteorSound.Pitch + extrapitch }, vector);
                                }
                            }
                        }
                    }
                }
                else if (NPC.ai[0] == 1f)
                {
                    Vector2 vector119 = new Vector2(vector.X, NPC.position.Y + NPC.height * 0.8f);
                    Vector2 vector120 = vector;
                    float num1058 = player.Center.X - vector120.X;
                    float num1059 = player.Center.Y - vector120.Y;
                    float num1060 = (float)Math.Sqrt(num1058 * num1058 + num1059 * num1059);

                    NPC.ai[1] += 1f;
                    int num638 = 0;
                    for (int num639 = 0; num639 < Main.maxPlayers; num639++)
                    {
                        if (Main.player[num639].active && !Main.player[num639].dead && (vector - Main.player[num639].Center).Length() < 1000f)
                            num638++;
                    }
                    NPC.ai[1] += num638 / 2;

                    bool flag103 = false;
                    float num640 = (CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 20f : (!sirenAlive || phase4) ? 60f : 40f;
                    if (NPC.ai[1] > num640)
                    {
                        NPC.ai[1] = 0f;
                        NPC.ai[2] += 1f;
                        flag103 = true;
                    }

                    int spawnLimit = (CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 10 : (sirenAlive && !phase4) ? 1 : (death ? 2 : 3);
                    if (flag103 && NPC.CountNPCS(ModContent.NPCType<AquaticAberration>()) < spawnLimit)
                    {
                        SoundEngine.PlaySound(soundChoice with { Pitch = soundChoice.Pitch + extrapitch }, vector);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)vector119.X, (int)vector119.Y, ModContent.NPCType<AquaticAberration>());
                    }

                    if (num1060 > ((sirenAlive && !phase4) ? 1000f : 800f) * NPC.scale)
                    {
                        float num1064 = (sirenAlive && !phase4) ? 0.05f : 0.065f;
                        num1064 += 0.04f * enrageScale;

                        if (expertMode && (!sirenAlive || phase4))
                            num1064 += death ? 0.05f * (1f - lifeRatio) : 0.03f * (1f - lifeRatio);

                        vector120 = vector119;
                        num1058 = player.Center.X - vector120.X;
                        num1059 = player.Center.Y - vector120.Y;

                        if (NPC.velocity.X < num1058)
                        {
                            NPC.velocity.X += num1064;
                            if (NPC.velocity.X < 0f && num1058 > 0f)
                                NPC.velocity.X += num1064;
                        }
                        else if (NPC.velocity.X > num1058)
                        {
                            NPC.velocity.X -= num1064;
                            if (NPC.velocity.X > 0f && num1058 < 0f)
                                NPC.velocity.X -= num1064;
                        }
                        if (NPC.velocity.Y < num1059)
                        {
                            NPC.velocity.Y += num1064;
                            if (NPC.velocity.Y < 0f && num1059 > 0f)
                                NPC.velocity.Y += num1064;
                        }
                        else if (NPC.velocity.Y > num1059)
                        {
                            NPC.velocity.Y -= num1064;
                            if (NPC.velocity.Y > 0f && num1059 < 0f)
                                NPC.velocity.Y -= num1064;
                        }
                    }
                    else
                        NPC.velocity *= 0.9f;

                    float playerLocation = vector.X - player.Center.X;
                    NPC.direction = playerLocation < 0 ? 1 : -1;
                    NPC.spriteDirection = NPC.direction;

                    if (NPC.ai[2] > spawnLimit)
                    {
                        NPC.ai[0] = canCharge ? (Main.rand.NextBool() ? 2f : 0f) : 0f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.TargetClosest();
                        NPC.netUpdate = true;
                    }
                }
                else if (NPC.ai[0] == 2f)
                {
                    Vector2 distFromPlayer = player.Center - vector;
                    float chargeAmt = death ? (phase4 ? 3f : phase3 ? 2f : 1f) : 1f;
                    if (NPC.ai[1] >= chargeAmt * 2f || distFromPlayer.Length() > 2400f)
                    {
                        NPC.ai[0] = canCharge ? (Main.rand.NextBool() ? 1f : 0f) : 0f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.TargetClosest();
                        NPC.netUpdate = true;
                        return;
                    }

                    float gfbchargeboost = Main.zenithWorld ? 1100 : 0;
                    float chargeDistance = ((sirenAlive && !phase4) ? 1100f : 900f) * NPC.scale + gfbchargeboost;
                    chargeDistance -= 50f * enrageScale;
                    if (!sirenAlive || phase4)
                        chargeDistance -= 250f * (1f - lifeRatio);

                    if (NPC.ai[1] % 2f == 0f)
                    {
                        int num24 = 7;
                        for (int j = 0; j < num24; j++)
                        {
                            Vector2 arg_E1C_0 = (Vector2.Normalize(NPC.velocity) * new Vector2((NPC.width + 50) / 2f, NPC.height) * 0.75f).RotatedBy((j - (num24 / 2 - 1)) * MathHelper.Pi / num24) + vector;
                            Vector2 vector4 = ((float)(Main.rand.NextDouble() * MathHelper.Pi) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
                            int num25 = Dust.NewDust(arg_E1C_0 + vector4, 0, 0, 172, vector4.X * 2f, vector4.Y * 2f, 100, default, 1.4f);
                            Main.dust[num25].noGravity = true;
                            Main.dust[num25].noLight = true;
                            Main.dust[num25].velocity /= 4f;
                            Main.dust[num25].velocity -= NPC.velocity;
                        }

                        if (Math.Abs(NPC.position.Y + (NPC.height / 2) - (player.position.Y + (player.height / 2))) < 20f)
                        {
                            NPC.ai[1] += 1f;
                            NPC.ai[2] = 0f;

                            float num1044 = revenge ? 20f : 18f;
                            num1044 += 2f * enrageScale;

                            if (revenge && (!sirenAlive || phase4))
                                num1044 += death ? 9f * (1f - lifeRatio) : 6f * (1f - lifeRatio);

                            if (Main.getGoodWorld)
                                num1044 *= 1.15f;

                            Vector2 vector117 = vector;
                            float num1045 = player.Center.X - vector117.X;
                            float num1046 = player.Center.Y - vector117.Y;
                            float num1047 = (float)Math.Sqrt(num1045 * num1045 + num1046 * num1046);
                            num1047 = num1044 / num1047;
                            NPC.velocity.X = num1045 * num1047;
                            NPC.velocity.Y = num1046 * num1047;

                            float playerLocation = vector.X - player.Center.X;
                            NPC.direction = playerLocation < 0 ? 1 : -1;
                            NPC.spriteDirection = NPC.direction;

                            SoundEngine.PlaySound(RoarChargeSound with { Pitch = RoarChargeSound.Pitch + extrapitch }, vector);

                            return;
                        }

                        float num1048 = revenge ? 7.5f : 6.5f;
                        float num1049 = revenge ? 0.12f : 0.11f;
                        num1048 += 2f * enrageScale;
                        num1049 += 0.04f * enrageScale;

                        if (revenge && (!sirenAlive || phase4))
                        {
                            num1048 += death ? 9f * (1f - lifeRatio) : 6f * (1f - lifeRatio);
                            num1049 += death ? 0.15f * (1f - lifeRatio) : 0.1f * (1f - lifeRatio);
                        }

                        if (Main.getGoodWorld)
                        {
                            num1048 *= 1.15f;
                            num1049 *= 1.15f;
                        }

                        if (vector.Y < player.Center.Y)
                            NPC.velocity.Y += num1049;
                        else
                            NPC.velocity.Y -= num1049;

                        if (NPC.velocity.Y < -num1048)
                            NPC.velocity.Y = -num1048;
                        if (NPC.velocity.Y > num1048)
                            NPC.velocity.Y = num1048;

                        if (Math.Abs(vector.X - player.Center.X) > chargeDistance + 200f)
                            NPC.velocity.X += num1049 * NPC.direction;
                        else if (Math.Abs(vector.X - player.Center.X) < chargeDistance)
                            NPC.velocity.X -= num1049 * NPC.direction;
                        else
                            NPC.velocity.X *= 0.8f;

                        if (NPC.velocity.X < -num1048)
                            NPC.velocity.X = -num1048;
                        if (NPC.velocity.X > num1048)
                            NPC.velocity.X = num1048;

                        float playerLocation2 = vector.X - player.Center.X;
                        NPC.direction = playerLocation2 < 0 ? 1 : -1;
                        NPC.spriteDirection = NPC.direction;
                    }
                    else
                    {
                        if (NPC.velocity.X < 0f)
                            NPC.direction = -1;
                        else
                            NPC.direction = 1;

                        NPC.spriteDirection = NPC.direction;

                        int num1051 = 1;
                        if (vector.X < player.Center.X)
                            num1051 = -1;
                        if (NPC.direction == num1051 && Math.Abs(vector.X - player.Center.X) > chargeDistance)
                            NPC.ai[2] = 1f;

                        if (NPC.ai[2] != 1f)
                            return;

                        float playerLocation = vector.X - player.Center.X;
                        NPC.direction = playerLocation < 0 ? 1 : -1;
                        NPC.spriteDirection = NPC.direction;

                        NPC.velocity *= 0.9f;
                        float num1052 = revenge ? 0.11f : 0.1f;
                        num1052 += 0.02f * enrageScale;

                        if (revenge && (!sirenAlive || phase4))
                        {
                            NPC.velocity *= death ? MathHelper.Lerp(0.75f, 1f, lifeRatio) : MathHelper.Lerp(0.81f, 1f, lifeRatio);
                            num1052 += death ? 0.15f * (1f - lifeRatio) : 0.1f * (1f - lifeRatio);
                        }

                        if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < num1052)
                        {
                            NPC.ai[2] = 0f;
                            NPC.ai[1] += 1f;
                            NPC.TargetClosest();
                        }
                    }
                }
            }
        }

        // Can only hit the target if within certain distance.
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            Vector2 npcCenter = NPC.Center;

            // NOTE: Tail and mouth hitboxes are interchangeable, each hitbox is the same size and is located to the right or left of the body hitbox.
            // Width = 225, Height = 225
            Rectangle mouthHitbox = new Rectangle((int)(npcCenter.X - (NPC.width / 2f)), (int)(npcCenter.Y - (NPC.height / 4f)), NPC.width / 4, NPC.height / 2);
            // Width = 450, Height = 450
            Rectangle bodyHitbox = new Rectangle((int)(npcCenter.X - (NPC.width / 4f)), (int)(npcCenter.Y - (NPC.height / 2f)), NPC.width / 2, NPC.height);
            // Width = 225, Height = 225
            Rectangle tailHitbox = new Rectangle((int)(npcCenter.X + (NPC.width / 4f)), (int)(npcCenter.Y - (NPC.height / 4f)), NPC.width / 4, NPC.height / 2);

            Vector2 mouthHitboxCenter = new Vector2(mouthHitbox.X + (mouthHitbox.Width / 2), mouthHitbox.Y + (mouthHitbox.Height / 2));
            Vector2 bodyHitboxCenter = new Vector2(bodyHitbox.X + (bodyHitbox.Width / 2), bodyHitbox.Y + (bodyHitbox.Height / 2));
            Vector2 tailHitboxCenter = new Vector2(tailHitbox.X + (tailHitbox.Width / 2), tailHitbox.Y + (tailHitbox.Height / 2));

            Rectangle targetHitbox = target.Hitbox;

            float mouthDist1 = Vector2.Distance(mouthHitboxCenter, targetHitbox.TopLeft());
            float mouthDist2 = Vector2.Distance(mouthHitboxCenter, targetHitbox.TopRight());
            float mouthDist3 = Vector2.Distance(mouthHitboxCenter, targetHitbox.BottomLeft());
            float mouthDist4 = Vector2.Distance(mouthHitboxCenter, targetHitbox.BottomRight());

            float minMouthDist = mouthDist1;
            if (mouthDist2 < minMouthDist)
                minMouthDist = mouthDist2;
            if (mouthDist3 < minMouthDist)
                minMouthDist = mouthDist3;
            if (mouthDist4 < minMouthDist)
                minMouthDist = mouthDist4;

            bool insideMouthHitbox = minMouthDist <= 115f * NPC.scale;

            float bodyDist1 = Vector2.Distance(bodyHitboxCenter, targetHitbox.TopLeft());
            float bodyDist2 = Vector2.Distance(bodyHitboxCenter, targetHitbox.TopRight());
            float bodyDist3 = Vector2.Distance(bodyHitboxCenter, targetHitbox.BottomLeft());
            float bodyDist4 = Vector2.Distance(bodyHitboxCenter, targetHitbox.BottomRight());

            float minBodyDist = bodyDist1;
            if (bodyDist2 < minBodyDist)
                minBodyDist = bodyDist2;
            if (bodyDist3 < minBodyDist)
                minBodyDist = bodyDist3;
            if (bodyDist4 < minBodyDist)
                minBodyDist = bodyDist4;

            bool insideBodyHitbox = minBodyDist <= 230f * NPC.scale;

            float tailDist1 = Vector2.Distance(tailHitboxCenter, targetHitbox.TopLeft());
            float tailDist2 = Vector2.Distance(tailHitboxCenter, targetHitbox.TopRight());
            float tailDist3 = Vector2.Distance(tailHitboxCenter, targetHitbox.BottomLeft());
            float tailDist4 = Vector2.Distance(tailHitboxCenter, targetHitbox.BottomRight());

            float minTailDist = tailDist1;
            if (tailDist2 < minTailDist)
                minTailDist = tailDist2;
            if (tailDist3 < minTailDist)
                minTailDist = tailDist3;
            if (tailDist4 < minTailDist)
                minTailDist = tailDist4;

            bool insideTailHitbox = minTailDist <= 115f * NPC.scale;

            return insideMouthHitbox || insideBodyHitbox || insideTailHitbox;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    float randomSpread = Main.rand.Next(-200, 201) / 100f;
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("LeviGore").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("LeviGore2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("LeviGore3").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("LeviGore4").Type, NPC.scale);
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public static void RealOnKill(NPC npc)
        {
            CalamityGlobalNPC.SetNewBossJustDowned(npc);

            // Mark Anahita & Leviathan as dead
            DownedBossSystem.downedLeviathan = true;
            CalamityNetcode.SyncWorld();
        }

        public override void OnKill()
        {
            if (LastAnLStanding())
                RealOnKill(NPC);
        }

        public static bool LastAnLStanding()
        {
            int count = NPC.CountNPCS(ModContent.NPCType<Anahita>()) + NPC.CountNPCS(ModContent.NPCType<Leviathan>());
            return count <= 1;
        }

        public static void DefineAnahitaLeviathanLoot(NPCLoot npcLoot)
        {
            var lastStanding = npcLoot.DefineConditionalDropSet(LastAnLStanding);
            lastStanding.Add(ItemDropRule.BossBag(ModContent.ItemType<LeviathanBag>()));

            LeadingConditionRule normalOnly = new LeadingConditionRule(new Conditions.NotExpert());
            lastStanding.Add(normalOnly);
            {
                // Weapons and such
                int[] items = new int[]
                {
                    ModContent.ItemType<Greentide>(),
                    ModContent.ItemType<Leviatitan>(),
                    ModContent.ItemType<AnahitasArpeggio>(),
                    ModContent.ItemType<Atlantis>(),
                    ModContent.ItemType<GastricBelcherStaff>(),
                    ModContent.ItemType<BrackishFlask>(),
                    ModContent.ItemType<LeviathanTeeth>()
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, items));

                // Vanity
                normalOnly.Add(ModContent.ItemType<LeviathanMask>(), 7);
                normalOnly.Add(ModContent.ItemType<AnahitaMask>(), 7);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                // Equipment
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<LeviathanAmbergris>()));
                normalOnly.Add(ModContent.ItemType<PearlofEnthrallment>(), DropHelper.NormalWeaponDropRateFraction);
                normalOnly.Add(ModContent.ItemType<TheCommunity>(), 10);

                // Fishing
                normalOnly.Add(ItemID.HotlineFishingHook, 10);
                normalOnly.Add(ItemID.BottomlessBucket, 10);
                normalOnly.Add(ItemID.SuperAbsorbantSponge, 10);
                normalOnly.Add(ItemID.FishingPotion, 5, 5, 8);
                normalOnly.Add(ItemID.SonarPotion, 5, 5, 8);
                normalOnly.Add(ItemID.CratePotion, 5, 5, 8);
            }

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).AddIf((info) => LastAnLStanding(), ModContent.ItemType<LeviathanAnahitaRelic>());

            // GFB Boulder, Pizza and Ring drop
            var GFBOnly = npcLoot.DefineConditionalDropSet(DropHelper.GFB);
            {
                GFBOnly.Add(ItemID.BouncyBoulder, 1, 1, 9999);
                GFBOnly.Add(ItemID.Boulder, 1, 1, 9999);
                GFBOnly.Add(ItemID.Pizza, 1, 1, 9999);
                GFBOnly.Add(ItemID.GreedyRing);
            }

            // Lore
            bool shouldDropLore(DropAttemptInfo info) => !DownedBossSystem.downedLeviathan && LastAnLStanding();
            npcLoot.AddConditionalPerPlayer(shouldDropLore, ModContent.ItemType<LoreAbyss>(), desc: DropHelper.FirstKillText);
            npcLoot.AddConditionalPerPlayer(shouldDropLore, ModContent.ItemType<LoreLeviathanAnahita>(), desc: DropHelper.FirstKillText);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            DefineAnahitaLeviathanLoot(npcLoot);

            // Trophy (always directly from boss, never in bag)
            npcLoot.Add(ModContent.ItemType<LeviathanTrophy>(), 10);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(BuffID.Bleeding, 600, true);
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = AttackTexture;
            if (NPC.ai[0] == 1f || NPC.Calamity().newAI[3] < 180f)
            {
                texture = TextureAssets.Npc[NPC.type].Value;
            }
            SpriteEffects spriteEffects = SpriteEffects.FlipHorizontally;
            float xOffset = -50f;
            if (NPC.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.None;
                xOffset *= -1f;
            }
            Rectangle rectangle = new Rectangle(NPC.frame.X, NPC.frame.Y, texture.Width / 2, texture.Height / 3);
            Vector2 origin = rectangle.Size() / 2f;
            spriteBatch.Draw(texture, NPC.Center - screenPos + new Vector2(xOffset, NPC.gfxOffY), rectangle, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            int width = 1011;
            int height = 486;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Scale = 0.2f,
                PortraitScale = 0.3f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;

            if (NPC.IsABestiaryIconDummy)
                NPC.Opacity = 1f;

            if (!initialised)
            {
                counter = 3;
                NPC.frameCounter = 8D;
                initialised = true;
            }

            NPC.frameCounter += 1D;
            if (NPC.frameCounter >= 8D)
            {
                NPC.frameCounter = 0D;
                counter++;
                NPC.frame.X = counter >= 3 ? width + 3 : 0;

                if (counter == 3)
                    NPC.frame.Y = 0;
                else
                    NPC.frame.Y += height;
            }

            if (counter == 6)
            {
                counter = 1;
                NPC.frame.Y = 0;
                NPC.frame.X = 0;
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }
    }
}
