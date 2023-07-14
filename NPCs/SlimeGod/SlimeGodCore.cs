using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.TownNPCs;
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
using System.Collections.Generic;
using System.Linq;

namespace CalamityMod.NPCs.SlimeGod
{
    [AutoloadBossHead]
    public class SlimeGodCore : ModNPC
    {
        private bool slimesSpawned = false;
        private int buffedSlime = 0;

        public static readonly SoundStyle PossessionSound = new("CalamityMod/Sounds/Custom/SlimeGodPossession");
        public static readonly SoundStyle ExitSound = new("CalamityMod/Sounds/Custom/SlimeGodExit");
        public static readonly SoundStyle ShotSound = new("CalamityMod/Sounds/Custom/SlimeGodShot", 2);
        public static readonly SoundStyle BigShotSound = new("CalamityMod/Sounds/Custom/SlimeGodBigShot", 2);

        public override void SetStaticDefaults()
        {
            NPCID.Sets.BossBestiaryPriority.Add(Type);
			NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.GetNPCDamage();
            NPC.npcSlots = 10f;
            NPC.width = 44;
            NPC.height = 44;
            if (CalamityWorld.LegendaryMode && CalamityWorld.revenge)
                NPC.scale = 2f;

            NPC.defense = 6;
            NPC.LifeMaxNERB(420);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPCID.Sets.TrailCacheLength[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 25, 0, 0);
            NPC.Opacity = 0.8f;
            NPC.boss = true;
            NPC.BossBar = ModContent.GetInstance<SlimeGodBossBar>();
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.SlimeGodCore")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(buffedSlime);
            writer.Write(NPC.Opacity);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            buffedSlime = reader.ReadInt32();
            NPC.Opacity = reader.ReadSingle();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            CalamityGlobalNPC.slimeGod = NPC.whoAmI;

            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || bossRush;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if (Main.netMode != NetmodeID.MultiplayerClient && !slimesSpawned)
            {
                slimesSpawned = true;
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<EbonianPaladin>());
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<CrimulanPaladin>());
            }

            // Set damage
            NPC.damage = NPC.defDamage;

            // Enrage based on large slimes
            bool purpleSlimeAlive = false;
            bool redSlimeAlive = false;

            if (CalamityGlobalNPC.slimeGodPurple != -1)
            {
                if (Main.npc[CalamityGlobalNPC.slimeGodPurple].active)
                {
                    if (buffedSlime == 1)
                        Main.npc[CalamityGlobalNPC.slimeGodPurple].localAI[1] = 1f;
                    else
                        Main.npc[CalamityGlobalNPC.slimeGodPurple].localAI[1] = 0f;

                    calamityGlobalNPC.newAI[0] = Main.npc[CalamityGlobalNPC.slimeGodPurple].Center.X;
                    calamityGlobalNPC.newAI[1] = Main.npc[CalamityGlobalNPC.slimeGodPurple].Center.Y;

                    purpleSlimeAlive = true;
                }
            }

            if (CalamityGlobalNPC.slimeGodRed != -1)
            {
                if (Main.npc[CalamityGlobalNPC.slimeGodRed].active)
                {
                    if (buffedSlime == 2)
                        Main.npc[CalamityGlobalNPC.slimeGodRed].localAI[1] = 1f;
                    else
                        Main.npc[CalamityGlobalNPC.slimeGodRed].localAI[1] = 0f;

                    NPC.ai[1] = Main.npc[CalamityGlobalNPC.slimeGodRed].Center.X;
                    NPC.ai[2] = Main.npc[CalamityGlobalNPC.slimeGodRed].Center.Y;

                    redSlimeAlive = true;
                }
            }

            // Start shooting blobs more often, move faster and buff large slimes more often if one type of large slime is dead
            bool phase2 = !purpleSlimeAlive || !redSlimeAlive;

            // Vanish phase
            if (!purpleSlimeAlive && !redSlimeAlive)
            {
                // Make sure Opacity is set to 0.8f if it's below that when the vanish phase starts
                if (NPC.ai[3] == 0f)
                {
                    if (!NPC.AnyNPCs(ModContent.NPCType<EbonianPaladin>()) && !NPC.AnyNPCs(ModContent.NPCType<CrimulanPaladin>()))
                    {
                        NPC.ai[3] = 1f;
                        NPC.Opacity = 0.8f;
                    }
                }

                // Emit dust
                if (!Main.zenithWorld) // you must see his glory.
                {
                    for (int k = 0; k < 5; k++)
                    {
                        Color color = Main.rand.NextBool() ? Color.Lavender : Color.Crimson;
                        color.A = 150;
                        Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, 0f, 0f, NPC.alpha, color, 1f);
                    }
                }

                // Slow down
                NPC.velocity *= 0.97f;

                // Rotate
                NPC.rotation += NPC.direction * 0.3f;

                // Gradually turn invisible
                NPC.Opacity -= 0.005f;

                // Drop loot, explode into dust and vanish once invisible
                if (NPC.Opacity <= 0f)
                {
                    NPC.Opacity = 0f;
                    SoundEngine.PlaySound(PossessionSound, NPC.Center);
                    NPC.position.X = NPC.position.X + (NPC.width / 2);
                    NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                    NPC.width = 40;
                    NPC.height = 40;
                    NPC.position.X = NPC.position.X - (NPC.width / 2);
                    NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                    for (int num621 = 0; num621 < 40; num621++)
                    {
                        Color color = Main.rand.NextBool() ? Color.Lavender : Color.Crimson;
                        color.A = 150;
                        int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 4, 0f, 0f, NPC.alpha, color, 2f);
                        Main.dust[num622].velocity *= 3f;
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[num622].scale = 0.5f;
                            Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                        }
                    }
                    for (int num623 = 0; num623 < 70; num623++)
                    {
                        Color color = Main.rand.NextBool() ? Color.Lavender : Color.Crimson;
                        color.A = 150;
                        int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 4, 0f, 0f, NPC.alpha, color, 3f);
                        Main.dust[num624].noGravity = true;
                        Main.dust[num624].velocity *= 5f;
                        num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 4, 0f, 0f, NPC.alpha, color, 2f);
                        Main.dust[num624].velocity *= 2f;
                    }

                    // Let the player know that the Slime God isn't dead fr
                    if (!DownedBossSystem.downedSlimeGod)
                    {
                        string key = "Mods.CalamityMod.Status.Boss.SlimeGodRun";
                        Color messageColor = Color.Magenta;

                        CalamityUtils.DisplayLocalizedText(key, messageColor);
                    }

                    // Set Slime God to have interacted with all players
                    for (int i = Main.maxPlayers - 1; i >= 0; i--)
                    {
                        NPC.ApplyInteraction(i);
                    }
                    NPC.active = false;
                    NPC.HitEffect();
                    NPC.NPCLoot();
                    NPC.netUpdate = true;
                }

                return;
            }

            // Despawn
            if (!player.active || player.dead || Vector2.Distance(player.Center, NPC.Center) > (bossRush ? CalamityGlobalNPC.CatchUpDistance350Tiles : CalamityGlobalNPC.CatchUpDistance200Tiles))
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead || Vector2.Distance(player.Center, NPC.Center) > (bossRush ? CalamityGlobalNPC.CatchUpDistance350Tiles : CalamityGlobalNPC.CatchUpDistance200Tiles))
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
                            if (Main.npc[x].type == ModContent.NPCType<EbonianPaladin>() || Main.npc[x].type == ModContent.NPCType<SplitEbonianPaladin>() ||
                                Main.npc[x].type == ModContent.NPCType<CrimulanPaladin>() || Main.npc[x].type == ModContent.NPCType<SplitCrimulanPaladin>())
                            {
                                Main.npc[x].active = false;
                                Main.npc[x].netUpdate = true;
                            }
                        }
                        NPC.active = false;
                        NPC.netUpdate = true;
                    }

                    NPC.Opacity = 0.8f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    calamityGlobalNPC.newAI[0] = 0f;
                    calamityGlobalNPC.newAI[1] = 0f;
                    calamityGlobalNPC.newAI[2] = 0f;
                    NPC.netUpdate = true;
                    return;
                }
            }
            else if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            // Hide inside large slime
            float hideInsideLargeSlimePhaseGateValue = phase2 ? 300f : 900f;
            float hideInsideLargeSlimePhaseDuration = 600f;
            float exitLargeSlimeGateValue = hideInsideLargeSlimePhaseGateValue + hideInsideLargeSlimePhaseDuration;
            calamityGlobalNPC.newAI[2] += 1f;
            if (calamityGlobalNPC.newAI[2] >= hideInsideLargeSlimePhaseGateValue)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.rotation += NPC.direction * 0.3f;

                if (buffedSlime == 0)
                {
                    SoundEngine.PlaySound(PossessionSound, NPC.Center);

                    if (purpleSlimeAlive && redSlimeAlive)
                        buffedSlime = Main.rand.Next(2) + 1;
                    else if (purpleSlimeAlive)
                        buffedSlime = 1;
                    else if (redSlimeAlive)
                        buffedSlime = 2;
                }

                Vector2 purpleSlimeVector = new Vector2(calamityGlobalNPC.newAI[0], calamityGlobalNPC.newAI[1]);
                Vector2 redSlimeVector = new Vector2(NPC.ai[1], NPC.ai[2]);
                Vector2 goToVector = buffedSlime == 1 ? purpleSlimeVector : redSlimeVector;

                Vector2 goToPosition = goToVector - NPC.Center;
                NPC.velocity = Vector2.Normalize(goToPosition) * 24f;

                // Reduce velocity to 0 to avoid spastic movement when inside big slime.
                if (Vector2.Distance(NPC.Center, goToVector) < 24f)
                {
                    NPC.velocity = Vector2.Zero;

                    NPC.Opacity -= 0.2f;
                    if (NPC.Opacity < 0f)
                        NPC.Opacity = 0f;
                }

                bool slimeDead;
                if (goToVector == purpleSlimeVector)
                    slimeDead = CalamityGlobalNPC.slimeGodPurple < 0 || !Main.npc[CalamityGlobalNPC.slimeGodPurple].active;
                else
                    slimeDead = CalamityGlobalNPC.slimeGodRed < 0 || !Main.npc[CalamityGlobalNPC.slimeGodRed].active;

                if (calamityGlobalNPC.newAI[2] >= exitLargeSlimeGateValue || slimeDead)
                {
                    NPC.TargetClosest();
                    calamityGlobalNPC.newAI[2] = 0f;
                    NPC.velocity = Vector2.UnitY * -12f;
                    SoundEngine.PlaySound(ExitSound, NPC.Center);
                    for (int i = 0; i < 20; i++)
                    {
                        Color color = Main.rand.NextBool() ? Color.Lavender : Color.Crimson;
                        color.A = 150;
                        int dust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 4, 0f, 0f, NPC.alpha, color, 2f);
                        Main.dust[dust2].velocity *= 3f;
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[dust2].scale = 0.5f;
                            Main.dust[dust2].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                        }
                    }
                    for (int j = 0; j < 30; j++)
                    {
                        Color color = Main.rand.NextBool() ? Color.Lavender : Color.Crimson;
                        color.A = 150;
                        int dust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 4, 0f, 0f, NPC.alpha, color, 3f);
                        Main.dust[dust2].noGravity = true;
                        Main.dust[dust2].velocity *= 5f;
                        dust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 4, 0f, 0f, NPC.alpha, color, 2f);
                        Main.dust[dust2].velocity *= 2f;
                    }
                }

                return;
            }
            else if (expertMode)
            {
                float divisor = bossRush ? 90f : death ? 180f : revenge ? 240f : 300f;
                if (phase2)
                    divisor /= 2;

                if (calamityGlobalNPC.newAI[2] % divisor == 0f)
                {
                    SoundEngine.PlaySound(ShotSound, NPC.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (Main.rand.NextBool())
                        {
                            float projectileVelocity = 4f;
                            int type = ModContent.ProjectileType<UnstableEbonianGlob>();
                            int damage = NPC.GetProjectileDamage(type);
                            Vector2 velocity = Vector2.Normalize(player.Center - NPC.Center) * projectileVelocity;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, type, damage, 0f, Main.myPlayer);
                        }
                        else
                        {
                            float projectileVelocity = 8f;
                            int type = ModContent.ProjectileType<UnstableCrimulanGlob>();
                            int damage = NPC.GetProjectileDamage(type);
                            Vector2 velocity = Vector2.Normalize(player.Center - NPC.Center) * projectileVelocity;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, type, damage, 0f, Main.myPlayer);
                        }
                    }
                }
            }

            NPC.Opacity += 0.2f;
            if (NPC.Opacity > 0.8f)
                NPC.Opacity = 0.8f;
            
            buffedSlime = 0;

            float flySpeed = death ? 15f : revenge ? 13.5f : expertMode ? 12f : 9f;
            if (phase2)
                flySpeed *= 1.25f;
            if (bossRush)
                flySpeed *= 1.25f;
            if (Main.getGoodWorld)
                flySpeed *= 1.25f;

            Vector2 vector167 = new Vector2(NPC.Center.X + (NPC.direction * 20), NPC.Center.Y + 6f);
            Vector2 flyDestination = GetFlyDestination(player);
            Vector2 idealVelocity = (flyDestination - vector167).SafeNormalize(Vector2.UnitY) * flySpeed;

            float distanceFromFlyDestination = NPC.Distance(flyDestination);

            NPC.ai[0] -= 1f;
            if (distanceFromFlyDestination < 200f || NPC.ai[0] > 0f)
            {
                if (distanceFromFlyDestination < 200f)
                    NPC.ai[0] = 20f;

                if (NPC.velocity.X < 0f)
                    NPC.direction = -1;
                else
                    NPC.direction = 1;

                NPC.rotation += NPC.direction * 0.3f;

                return;
            }

            float inertia = 50f;
            if (Main.getGoodWorld)
                inertia *= 0.8f;
            if (CalamityWorld.LegendaryMode && CalamityWorld.revenge)
                inertia -= Main.rand.Next(31);

            NPC.velocity = (NPC.velocity * inertia + idealVelocity) / (inertia + 1f);
            if (distanceFromFlyDestination < 350f)
                NPC.velocity = (NPC.velocity * 10f + idealVelocity) / 11f;
            if (distanceFromFlyDestination < 300f)
                NPC.velocity = (NPC.velocity * 7f + idealVelocity) / 8f;

            NPC.rotation = NPC.velocity.X * 0.1f;
        }

        public Vector2 GetFlyDestination(Player target)
        {
            // Find all large slimes in the world.
            // If multiple slimes are present, and they are all relatively close together, try to stay in their general area.
            // If they are far apart, try to stay towards the closest slime.
            // If no slimes exist, or they are all extremely far away, try to stay near the target player instead.
            // TODO -- Consider renaming the big slime god's internal names to be more intuitive?
            int crimulanSlimeID = ModContent.NPCType<CrimulanPaladin>();
            int crimulanSlimeSplitID = ModContent.NPCType<SplitCrimulanPaladin>();
            int ebonianSlimeID = ModContent.NPCType<EbonianPaladin>();
            int ebonianSlimeSplitID = ModContent.NPCType<SplitEbonianPaladin>();
            List<NPC> largeSlimes = new();

            float ignoreGeneralAreaDistanceThreshold = 750f;
            float ignoreAllSlimesDistanceThreshold = 3200f;

            // Find all slimes within a generous area.
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                int npcType = Main.npc[i].type;
                if (npcType != crimulanSlimeID && npcType != crimulanSlimeSplitID && npcType != ebonianSlimeID && npcType != ebonianSlimeSplitID)
                    continue;

                if (!Main.npc[i].active)
                    continue;

                if (!NPC.WithinRange(Main.npc[i].Center, ignoreAllSlimesDistanceThreshold))
                    continue;

                largeSlimes.Add(Main.npc[i]);
            }

            // If no slimes were found, don't bother doing any more calculations. Just use the player's center.
            if (largeSlimes.Count <= 0)
                return target.Center;

            // Find the closest slime.
            NPC closestSlime = largeSlimes.OrderBy(n => n.Distance(NPC.Center)).First();

            // Get the general area of all the slimes by averaging together their positions.
            Vector2 generalSlimeArea = Vector2.Zero;
            for (int i = 0; i < largeSlimes.Count; i++)
                generalSlimeArea += largeSlimes[i].Center;
            generalSlimeArea /= largeSlimes.Count;

            // Determine the average deviation of all slimes from the general area.
            // This provides a general idea of how far apart all the slimes are from each-other.
            float averageGeneralAreaDistanceDeviation = largeSlimes.Average(s => s.Distance(generalSlimeArea));

            // The slimes are too far apart. Simply go with the closest slime.
            if (averageGeneralAreaDistanceDeviation > ignoreGeneralAreaDistanceThreshold)
                return closestSlime.Center;

            // Otherwise, use the average general position as a place to hover.
            return generalSlimeArea;
        }

        public override Color? GetAlpha(Color drawColor)
        {
            Color newColor = buffedSlime == 0 ? new Color(255, 255, 255, drawColor.A) : drawColor;
            return newColor * NPC.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Color color24 = NPC.GetAlpha(drawColor);
            Color color25 = Lighting.GetColor((int)((double)NPC.position.X + (double)NPC.width * 0.5) / 16, (int)(((double)NPC.position.Y + (double)NPC.height * 0.5) / 16.0));
            Texture2D texture2D3 = TextureAssets.Npc[NPC.type].Value;
            Texture2D pog = ModContent.Request<Texture2D>("CalamityMod/NPCs/SlimeGod/SlimeGodEyes").Value;
            int num156 = TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type];
            int y3 = num156 * (int)NPC.frameCounter;
            Rectangle rectangle = new Rectangle(0, y3, texture2D3.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            int num157 = 8;
            int num158 = 2;
            int num159 = 1;
            float num160 = 0f;
            int num161 = num159;
            spriteBatch.Draw(texture2D3, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, color24, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
            if (Main.zenithWorld)
            {
                spriteBatch.Draw(pog, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, color24, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
            }
            if (!Main.zenithWorld)
                while (((num158 > 0 && num161 < num157) || (num158 < 0 && num161 > num157)) && CalamityConfig.Instance.Afterimages)
                {
                    Color color26 = NPC.GetAlpha(color25);
                    {
                        goto IL_6899;
                    }
                    IL_6881:
                    num161 += num158;
                    continue;
                    IL_6899:
                    float num164 = (float)(num157 - num161);
                    if (num158 < 0)
                    {
                        num164 = (float)(num159 - num161);
                    }
                    color26 *= num164 / ((float)NPCID.Sets.TrailCacheLength[NPC.type] * 1.5f);
                    Vector2 value4 = NPC.oldPos[num161];
                    float num165 = NPC.rotation;
                    Main.spriteBatch.Draw(texture2D3, value4 + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, num165 + NPC.rotation * num160 * (float)(num161 - 1) * -(float)spriteEffects.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), origin2, NPC.scale, spriteEffects, 0f);
                    goto IL_6881;
                }
            return false;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }

        public override void OnKill()
        {
            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            CalamityGlobalNPC.SetNewShopVariable(new int[] { ModContent.NPCType<THIEF>() }, DownedBossSystem.downedSlimeGod);

            // Mark the Slime God as dead
            DownedBossSystem.downedSlimeGod = true;
            CalamityNetcode.SyncWorld();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Every Slime God piece drops Gel, even if it's not the last one.
            npcLoot.Add(ItemID.Gel, 1, 32, 48);

            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<SlimeGodBag>()));

            // Normal drops: Everything that would otherwise be in the bag
            LeadingConditionRule normalOnly = new LeadingConditionRule(new Conditions.NotExpert());
            npcLoot.Add(normalOnly);
            {
                // Weapons
                int[] weapons = new int[]
                {
                    ModContent.ItemType<OverloadedBlaster>(),
                    ModContent.ItemType<AbyssalTome>(),
                    ModContent.ItemType<EldritchTome>(),
                    ModContent.ItemType<CorroslimeStaff>(),
                    ModContent.ItemType<CrimslimeStaff>()
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, weapons));

                // Materials
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<PurifiedGel>(), 1, 30, 45));

                // Vanity
                normalOnly.Add(ModContent.ItemType<SlimeGodMask>(), 7);
                normalOnly.Add(ModContent.ItemType<SlimeGodMask2>(), 7);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                // Equipment
                normalOnly.Add(ModContent.ItemType<ManaPolarizer>());
            }

            npcLoot.Add(ModContent.ItemType<SlimeGodTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<SlimeGodRelic>());

            // GFB Gelatin Crystal drop
            npcLoot.DefineConditionalDropSet(DropHelper.GFB).Add(ItemID.QueenSlimeCrystal);

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedSlimeGod, ModContent.ItemType<LoreSlimeGod>(), desc: DropHelper.FirstKillText);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Color color = Main.rand.NextBool() ? Color.Lavender : Color.Crimson;
                color.A = 150;
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, hit.HitDirection, -1f, NPC.alpha, color, 1f);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
            {
                int debufftype = Main.zenithWorld ? BuffID.VortexDebuff : BuffID.Slow;
                target.AddBuff(debufftype, 180, true);
                target.AddBuff(BuffID.Weak, 180, true);
                target.AddBuff(BuffID.Darkness, 180, true);
			}
        }
    }
}
