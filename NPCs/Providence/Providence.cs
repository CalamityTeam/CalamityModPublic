using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Tiles.FurnitureProfaned;
using CalamityMod.Tiles.Ores;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;

namespace CalamityMod.NPCs.Providence
{
    [AutoloadBossHead]
    public class Providence : ModNPC
    {
        private bool text = false;
        private bool useDefenseFrames = false;
        private float bossLife;
        private int biomeType = 0;
        private int flightPath = 0;
        private int phaseChange = 0;
        private int immuneTimer = 300;
        private int frameUsed = 0;
        private int healTimer = 0;
        private bool challenge = Main.expertMode && Main.netMode == NetmodeID.SinglePlayer; //Used to determine if Profaned Soul Crystal should drop, couldn't figure out mp mems always dropping it so challenge is singleplayer only.

        public static float normalDR = 0.35f;
        public static float cocoonDR = 0.9f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Providence, the Profaned Goddess");
            Main.npcFrameCount[npc.type] = 3;
			NPCID.Sets.TrailingMode[npc.type] = 1;
		}

        public override void SetDefaults()
        {
            npc.npcSlots = 36f;
            npc.damage = 100;
            npc.width = 600;
            npc.height = 450;
            npc.defense = 50;
            CalamityGlobalNPC global = npc.Calamity();
            global.DR = normalDR;
            global.customDR = true;
            global.flatDRReductions.Add(BuffID.Ichor, 0.05f);
            global.flatDRReductions.Add(BuffID.CursedInferno, 0.05f);
            npc.LifeMaxNERB(440000, 500000, 12500000);
            double HPBoost = CalamityMod.CalamityConfig.BossHealthPercentageBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.value = Item.buyPrice(0, 50, 0, 0);
            npc.boss = true;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.buffImmune[BuffID.Ichor] = false;
            npc.buffImmune[BuffID.CursedInferno] = false;
            npc.buffImmune[BuffID.StardustMinionBleed] = false;
            npc.buffImmune[BuffID.BetsysCurse] = false;
            npc.buffImmune[ModContent.BuffType<AstralInfectionDebuff>()] = false;
            npc.buffImmune[ModContent.BuffType<AbyssalFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<ArmorCrunch>()] = false;
            npc.buffImmune[ModContent.BuffType<DemonFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<GodSlayerInferno>()] = false;
            npc.buffImmune[ModContent.BuffType<Nightwither>()] = false;
            npc.buffImmune[ModContent.BuffType<Shred>()] = false;
            npc.buffImmune[ModContent.BuffType<WhisperingDeath>()] = false;
            npc.buffImmune[ModContent.BuffType<SilvaStun>()] = false;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.netAlways = true;
            npc.chaseable = true;
            npc.canGhostHeal = false;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/ProvidenceTheme");
            else
                music = MusicID.LunarBoss;
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/ProvidenceDeath");
            bossBag = ModContent.ItemType<ProvidenceBag>();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(text);
            writer.Write(useDefenseFrames);
            writer.Write(biomeType);
            writer.Write(phaseChange);
            writer.Write(immuneTimer);
            writer.Write(frameUsed);
            writer.Write(healTimer);
            writer.Write(flightPath);
            writer.Write(npc.dontTakeDamage);
            writer.Write(npc.chaseable);
            writer.Write(npc.canGhostHeal);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            text = reader.ReadBoolean();
            useDefenseFrames = reader.ReadBoolean();
            biomeType = reader.ReadInt32();
            phaseChange = reader.ReadInt32();
            immuneTimer = reader.ReadInt32();
            frameUsed = reader.ReadInt32();
            healTimer = reader.ReadInt32();
            flightPath = reader.ReadInt32();
            npc.dontTakeDamage = reader.ReadBoolean();
            npc.chaseable = reader.ReadBoolean();
            npc.canGhostHeal = reader.ReadBoolean();
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            // whoAmI variable for Guardians and other things
            CalamityGlobalNPC.holyBoss = npc.whoAmI;

            // Rotation
            npc.rotation = npc.velocity.X * 0.004f;

			// Target
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest(true);

			// Target variable and boss center
			Player player = Main.player[npc.target];
            Vector2 vector = npc.Center;

			// Difficulty bools
			bool death = CalamityWorld.death || CalamityWorld.bossRushActive;
			bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;

            // Target's current biome
            bool isHoly = player.ZoneHoly;
            bool isHell = player.ZoneUnderworldHeight;

            // Fire projectiles at normal rate or not
            bool normalAttackRate = true;

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            // Phases
            bool ignoreGuardianAmt = lifeRatio < (death ? 0.2f : 0.15f);
            bool phase2 = lifeRatio < 0.75f || death;

            // Projectile fire rate multiplier
            double attackRateMult = 1D;

            // Distance X needed from target in order to fire holy or molten blasts
            float distanceNeededToShoot = revenge ? 360f : 420f;

            // Inflict Holy Inferno if target is too far away
            if (Vector2.Distance(player.Center, vector) > 2800f)
            {
                if (!player.dead && player.active)
                    player.AddBuff(ModContent.BuffType<HolyInferno>(), 2);
            }

            // Count the remaining Guardians, healer especially because it allows the boss to heal
            int guardianAmt = 0;
            bool healerAlive = false;
            if (CalamityGlobalNPC.holyBossAttacker != -1)
            {
                if (Main.npc[CalamityGlobalNPC.holyBossAttacker].active)
                    guardianAmt++;
            }
            if (CalamityGlobalNPC.holyBossDefender != -1)
            {
                if (Main.npc[CalamityGlobalNPC.holyBossDefender].active)
                    guardianAmt++;
            }
            if (CalamityGlobalNPC.holyBossHealer != -1)
            {
                if (Main.npc[CalamityGlobalNPC.holyBossHealer].active)
                {
                    guardianAmt++;
                    healerAlive = true;
                }
            }

            // Change projectile fire rate depending on Guardian amount
            if (guardianAmt > 0)
            {
                normalAttackRate = ignoreGuardianAmt;
                if (!normalAttackRate)
                {
                    switch (guardianAmt)
                    {
                        case 1:
                            attackRateMult = 1.25;
                            break;
                        case 2:
                            attackRateMult = 1.5;
                            break;
                        case 3:
                            attackRateMult = 2D;
                            break;
                        default:
                            break;
                    }
                }
            }

            // Whether the boss can be homed in on or healed off of
            npc.chaseable = normalAttackRate && npc.ai[0] != 2f && npc.ai[0] != 5f && npc.ai[0] != 7f;
            npc.canGhostHeal = npc.chaseable;

            // Prevent lag by stopping rain
            CalamityMod.StopRain();

            // Set target biome type
            if (biomeType == 0)
            {
                if (isHoly)
                    biomeType = 1;
                else if (isHell)
                    biomeType = 2;
            }

            // Become immune over time if target isn't in hell or hallow
            if (!isHoly && !isHell && !CalamityWorld.bossRushActive)
            {
                if (immuneTimer > 0)
                    immuneTimer--;
            }
            else
                immuneTimer = 300;

            // Take damage or not
            npc.dontTakeDamage = immuneTimer <= 0;

            // Heal
            if (healerAlive)
            {
                float heal = revenge ? 90f : 120f;
                switch (guardianAmt)
                {
                    case 1:
                        heal *= 2f;
                        break;
                    case 2:
                        break;
                    case 3:
                        heal *= 0.5f;
                        break;
                    default:
                        break;
                }

                healTimer++;
                if (healTimer >= heal)
                {
                    healTimer = 0;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int healAmt = npc.lifeMax / 200;
                        if (healAmt > npc.lifeMax - npc.life)
                            healAmt = npc.lifeMax - npc.life;

                        if (healAmt > 0)
                        {
                            npc.life += healAmt;
                            npc.HealEffect(healAmt, true);
                            npc.netUpdate = true;
                        }
                    }
                }
            }

			// Despawn
			bool targetDead = false;
            if ((!Main.dayTime && npc.ai[0] != 2f && npc.ai[0] != 5f && npc.ai[0] != 7f) || !player.active || player.dead)
            {
				if (!player.active || player.dead)
				{
					npc.TargetClosest(false);
					player = Main.player[npc.target];
				}
				if ((!Main.dayTime && npc.ai[0] != 2f && npc.ai[0] != 5f && npc.ai[0] != 7f) || !player.active || player.dead)
				{
					targetDead = true;

					if (npc.timeLeft > 60)
						npc.timeLeft = 60;

					if (npc.velocity.X > 0f)
						npc.velocity.X += 0.2f;
					else
						npc.velocity.X -= 0.2f;

					npc.velocity.Y -= 0.2f;
				}
            }
            else if (npc.timeLeft < 1800)
                npc.timeLeft = 1800;

            // Guardian spawn
            if (bossLife == 0f && npc.life > 0)
                bossLife = npc.lifeMax;
            if (npc.life > 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int num660 = (int)(npc.lifeMax * 0.66);
                    if ((npc.life + num660) < bossLife)
                    {
                        bossLife = npc.life;
                        int x = (int)(npc.position.X + Main.rand.Next(npc.width - 32));
                        int y = (int)(npc.position.Y + Main.rand.Next(npc.height - 32));
                        NPC.NewNPC(x - 100, y - 100, ModContent.NPCType<ProvSpawnDefense>(), 0, 0f, 0f, 0f, 0f, 255);
                        NPC.NewNPC(x + 100, y - 100, ModContent.NPCType<ProvSpawnHealer>(), 0, 0f, 0f, 0f, 0f, 255);
                        NPC.NewNPC(x, y + 100, ModContent.NPCType<ProvSpawnOffense>(), 0, 0f, 0f, 0f, 0f, 255);
                    }
                }
            }

            // Set DR based on current attack phase
            npc.Calamity().DR = (npc.ai[0] == 2f || npc.ai[0] == 5f || npc.ai[0] == 7f) ? cocoonDR : normalDR;

            // Movement
            if (npc.ai[0] != 2f && npc.ai[0] != 5f)
            {
                // Firing holy ray or not
                bool firingLaser = npc.ai[0] == 7f;

                // Change X direction of movement
                if (flightPath == 0)
                {
                    npc.TargetClosest(true);
                    if (vector.X < player.Center.X)
                    {
                        flightPath = 1;
                        calamityGlobalNPC.newAI[0] = 0f;
                    }
                    else
                    {
                        flightPath = -1;
                        calamityGlobalNPC.newAI[0] = 0f;
                    }
                }

                // Get a target
                npc.TargetClosest(true);

                // Increase speed over time if flying in same direction for too long
                if (revenge)
                    calamityGlobalNPC.newAI[0] += 1f;

                // Distance needed from target to change direction
                float num851 = 800f;

                // Increase distance from target when firing molten blasts or holy bombs
                bool stayAwayFromTarget = npc.ai[0] == 3f || npc.ai[0] == 4f;
                if (stayAwayFromTarget)
                    num851 += revenge ? 200f : 100f;

                // Change X movement path if far enough away from target
                float num852 = Math.Abs(vector.X - player.Center.X);
                if (vector.X < player.Center.X && flightPath < 0 && num852 > num851)
                    flightPath = 0;
                if (vector.X > player.Center.X && flightPath > 0 && num852 > num851)
                    flightPath = 0;

                // Velocity and acceleration
                bool increaseSpeed = calamityGlobalNPC.newAI[0] > 150f;
				float accelerationBoost = death ? 0.2f : 0.2f * (1f - lifeRatio);
				float velocityBoost = death ? 4f : 4f * (1f - lifeRatio);
                float acceleration = (expertMode ? 1.1f : 1.05f) + accelerationBoost;
                float velocity = (expertMode ? 16f : 15f) + velocityBoost;
                if (CalamityWorld.bossRushActive)
                {
                    acceleration = 1.3f;
                    velocity = 20f;
                }
                if (firingLaser)
                {
                    acceleration *= normalAttackRate ? 0.4f : 0.2f;
                    velocity *= normalAttackRate ? 0.4f : 0.2f;
                }
                else if (increaseSpeed)
                {
                    velocity += (calamityGlobalNPC.newAI[0] - 150f) * 0.04f;
                    if (velocity > 30f)
                        velocity = 30f;
                }

				if (!targetDead)
				{
					npc.velocity.X += flightPath * acceleration;
					if (npc.velocity.X > velocity)
						npc.velocity.X = velocity;
					if (npc.velocity.X < -velocity)
						npc.velocity.X = -velocity;

					float num855 = player.position.Y - (npc.position.Y + npc.height);
					if (num855 < (firingLaser ? 150f : 200f)) // 150
						npc.velocity.Y -= 0.2f;
					if (num855 > (firingLaser ? 200f : 250f)) // 200
						npc.velocity.Y += 0.2f;

					float speedVariance = normalAttackRate ? 2f : 1f;
					if (npc.velocity.Y > (firingLaser ? speedVariance : 6f))
						npc.velocity.Y = firingLaser ? speedVariance : 6f;
					if (npc.velocity.Y < (firingLaser ? -speedVariance : -6f))
						npc.velocity.Y = firingLaser ? -speedVariance : -6f;
				}
            }

			// Phase switch
			if (npc.ai[0] == -1f)
			{
				npc.noGravity = true;
				npc.noTileCollide = true;

				phaseChange++;
				if (phaseChange > 14)
					phaseChange = 0;

				int phase = 0;
				/* 0 = blasts
				 * 1 = holy fire
				 * 2 = shell heal
				 * 3 = molten blobs
				 * 4 = holy bombs
				 * 5 = shell spears
				 * 6 = crystal
				 * 7 = laser */

				// Holy ray in hallow, Crystal in hell
				bool useLaser = (phase2 && biomeType == 1) || CalamityWorld.bossRushActive;
				bool useCrystal = (phase2 && biomeType == 2) || CalamityWorld.bossRushActive;

				// Unique pattern for Death Mode and Boss Rush
				if (death)
				{
					switch (phaseChange)
					{
						case 0:
							phase = 3;
							break; // 1575 or 1500
						case 1:
							phase = 5;
							break; // 1875 or 1800
						case 2:
							phase = 0;
							break; // 2175 or 2100
						case 3:
							phase = useCrystal ? 6 : 3;
							break;
						case 4:
							phase = useCrystal ? 3 : 2;
							break; // 600
						case 5:
							phase = useCrystal ? 2 : 1;
							break; // 900
						case 6:
							phase = useLaser ? 7 : 4; // 1875 or 1800
							if (useLaser)
							{
								npc.TargetClosest(false);
								Vector2 v3 = player.Center - vector - new Vector2(0f, -22f);
								float num1219 = v3.Length() / 500f;
								if (num1219 > 1f)
									num1219 = 1f;
								num1219 = 1f - num1219;
								num1219 *= 2f;
								if (num1219 > 1f)
									num1219 = 1f;

								npc.localAI[0] = v3.ToRotation();
								npc.localAI[1] = num1219;
							}
							break; // 1200
						case 7:
							phase = useLaser ? 4 : 3;
							break; // 1500
						case 8:
							phase = useLaser ? 3 : 5;
							break;
						case 9:
							phase = 0;
							break; // 2175 or 2100
						case 10:
							phase = useCrystal ? 6 : 2;
							break;
						case 11:
							phase = 3;
							break; // 300
						case 12:
							phase = useLaser ? 7 : 4; // 675 or 600
							if (useLaser)
							{
								npc.TargetClosest(false);
								Vector2 v3 = player.Center - vector - new Vector2(0f, -22f);
								float num1219 = v3.Length() / 500f;
								if (num1219 > 1f)
									num1219 = 1f;
								num1219 = 1f - num1219;
								num1219 *= 2f;
								if (num1219 > 1f)
									num1219 = 1f;

								npc.localAI[0] = v3.ToRotation();
								npc.localAI[1] = num1219;
							}
							break;
						case 13:
							phase = 5;
							break; // 975 or 900
						case 14:
							phase = useLaser ? 4 : 0;
							break; // 1275 or 1200
						default:
							break;
					}
				}
				else
				{
					switch (phaseChange)
					{
						case 0:
							phase = 0;
							break; // 3375 or 3300
						case 1:
							phase = useLaser ? 7 : 1; // 3750 or 3600
							if (useLaser)
							{
								npc.TargetClosest(false);
								Vector2 v3 = player.Center - vector - new Vector2(0f, -22f);
								float num1219 = v3.Length() / 500f;
								if (num1219 > 1f)
									num1219 = 1f;
								num1219 = 1f - num1219;
								num1219 *= 2f;
								if (num1219 > 1f)
									num1219 = 1f;

								npc.localAI[0] = v3.ToRotation();
								npc.localAI[1] = num1219;
							}
							break;
						case 2:
							phase = 4;
							break; // 4050 or 3900
						case 3:
							phase = 3;
							break; // 4350 or 4200
						case 4:
							phase = 5;
							break; // 4650 or 4500
						case 5:
							phase = useCrystal ? 6 : 4;
							break;
						case 6:
							phase = 1;
							break; // 300
						case 7:
							phase = 0;
							break; // 600
						case 8:
							phase = 3;
							break; // 900
						case 9:
							phase = 2;
							break; // 1500
						case 10:
							phase = 4;
							break; // 1800
						case 11:
							phase = useLaser ? 7 : 0; //2175 or 2100
							if (useLaser)
							{
								npc.TargetClosest(false);
								Vector2 v3 = player.Center - vector - new Vector2(0f, -22f);
								float num1219 = v3.Length() / 500f;
								if (num1219 > 1f)
									num1219 = 1f;
								num1219 = 1f - num1219;
								num1219 *= 2f;
								if (num1219 > 1f)
									num1219 = 1f;

								npc.localAI[0] = v3.ToRotation();
								npc.localAI[1] = num1219;
							}
							break;
						case 12:
							phase = 1;
							break; // 2475 or 2400
						case 13:
							phase = 3;
							break; // 2775 or 2700
						case 14:
							phase = 5;
							break; // 3075 or 3000
						default:
							break;
					}
				}

				// Pick a target
				npc.TargetClosest(true);

				// If too far from target, set phase to 0
				if (Math.Abs(vector.X - player.Center.X) > 5600f)
					phase = 0;

				// Reset arrays
				npc.ai[0] = phase;
				npc.ai[1] = 0f;
				npc.ai[2] = 0f;
				npc.ai[3] = 0f;
				calamityGlobalNPC.newAI[1] = 0f;
			}

			// Holy blasts
			else if (npc.ai[0] == 0f)
			{
				npc.noGravity = true;
				npc.noTileCollide = true;

				float num852 = Math.Abs(vector.X - player.Center.X);
				if (num852 > distanceNeededToShoot && npc.position.Y < player.position.Y)
				{
					npc.ai[3] += 1f;

					int shootBoost = death ? 3 : (int)(4f * (1f - lifeRatio));
					int num856 = (expertMode ? 24 : 26) - shootBoost;
					if (npc.Calamity().enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive))
						num856 = 20;

					num856 = (int)(num856 * attackRateMult);

					if (npc.ai[3] >= num856)
						npc.ai[3] = -num856;

					if (npc.ai[3] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
					{
						vector.X += npc.velocity.X * 7f;
						float num857 = player.position.X + player.width * 0.5f - vector.X;
						float num858 = player.Center.Y - vector.Y;
						float num859 = (float)Math.Sqrt(num857 * num857 + num858 * num858);

						float velocityBoost = death ? 2.5f : 2.5f * (1f - lifeRatio);
						float num860 = (expertMode ? 10.25f : 9f) + velocityBoost;
						if (npc.Calamity().enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive))
							num860 = 12.75f;

						if (revenge)
							num860 *= 1.15f;

						num859 = num860 / num859;
						num857 *= num859;
						num858 *= num859;

						int holyDamage = expertMode ? 52 : 65;
						Projectile.NewProjectile(vector.X, vector.Y, num857, num858, ModContent.ProjectileType<HolyBlast>(), holyDamage, 0f, Main.myPlayer, 0f, 0f);
					}
				}
				else if (npc.ai[3] < 0f)
					npc.ai[3] += 1f;

				npc.ai[1] += 1f;
				if (npc.ai[1] >= 300f)
					npc.ai[0] = -1f;
			}

			// Holy fire
			else if (npc.ai[0] == 1f)
			{
				npc.noGravity = true;
				npc.noTileCollide = true;

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.ai[3] += 1f;

					int shootBoost = death ? 4 : (int)(5f * (1f - lifeRatio));
					int num864 = (expertMode ? 36 : 39) - shootBoost;
					if (CalamityWorld.bossRushActive)
						num864 = 31;

					num864 = (int)(num864 * attackRateMult);

					if (npc.ai[3] >= num864)
					{
						npc.ai[3] = 0f;

						Vector2 vector113 = new Vector2(vector.X, npc.position.Y + npc.height - 14f);

						float num865 = npc.velocity.Y;
						if (num865 < 0f)
							num865 = 0f;

						num865 += expertMode ? 4f : 3f;

						int fireDamage = expertMode ? 48 : 60;
						Projectile.NewProjectile(vector113.X, vector113.Y, npc.velocity.X * 0.25f, num865, ModContent.ProjectileType<HolyFire>(), fireDamage, 0f, Main.myPlayer, 0f, 0f);
					}
				}

				npc.ai[1] += 1f;
				if (npc.ai[1] >= 300f)
					npc.ai[0] = -1f;
			}

			// Cocoon flames
			else if (npc.ai[0] == 2f)
			{
				npc.noGravity = true;
				npc.noTileCollide = true;

				npc.TargetClosest(true);

				if (!targetDead)
				{
					if (npc.velocity.Length() <= 2f)
						npc.velocity = Vector2.Zero;
					if (npc.velocity.Length() > 2f)
					{
						npc.velocity *= 0.9f;
						return;
					}
				}

				float divisor = (expertMode ? 2f : 3f) + (float)Math.Floor(3f * lifeRatio) + (attackRateMult > 1D ? (float)Math.Ceiling(attackRateMult * 1.6) : 0f);
				int totalProjectiles = 36;
				int chains = 4;
				float interval = totalProjectiles / chains * divisor;
				double patternInterval = Math.Floor(npc.ai[3] / interval);
				float velocity = 3f;
				Vector2 fireFrom = new Vector2(vector.X, vector.Y + 20f);

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					if (patternInterval % 2 == 0)
					{
						if (npc.ai[3] % divisor == 0f)
						{
							Vector2 spinningPoint = calamityGlobalNPC.newAI[1] % 2f == 0f ? new Vector2(0f, -velocity) : Vector2.Normalize(new Vector2(-velocity, -velocity)) * velocity;
							double radians = MathHelper.TwoPi / chains;
							for (int i = 0; i < chains; i++)
							{
								Vector2 vector2 = spinningPoint.RotatedBy(radians * i + MathHelper.ToRadians(npc.ai[2]));

								int projectileType = ModContent.ProjectileType<HolyBurnOrb>();
								if (Main.rand.NextBool(4) && !death)
									projectileType = ModContent.ProjectileType<HolyLight>();

								Projectile.NewProjectile(fireFrom, vector2, projectileType, 0, 0f, Main.myPlayer, 0f, 0f);
							}

							// Radial offset
							npc.ai[2] += 10f;
						}
					}
					else
					{
						npc.ai[2] = 0f;

						totalProjectiles = 16;
						if (npc.ai[3] % (divisor * totalProjectiles) == 0f)
						{
							calamityGlobalNPC.newAI[1] += 1f;
							double radians = MathHelper.TwoPi / totalProjectiles;
							for (int i = 0; i < totalProjectiles; i++)
							{
								Vector2 vector2 = new Vector2(0f, -velocity).RotatedBy(radians * i);

								int projectileType = ModContent.ProjectileType<HolyBurnOrb>();
								if (Main.rand.NextBool(4) && !death)
									projectileType = ModContent.ProjectileType<HolyLight>();

								Projectile.NewProjectile(fireFrom, vector2, projectileType, 0, 0f, Main.myPlayer, 0f, 0f);
							}
						}
					}
				}

				if (npc.ai[3] == 0f)
				{
					for (int x = 0; x < Main.maxProjectiles; x++)
					{
						Projectile projectile = Main.projectile[x];
						if (projectile.active)
						{
							if (projectile.type == ModContent.ProjectileType<HolyFire2>() || projectile.type == ModContent.ProjectileType<HolyFlare>())
							{
								projectile.Kill();
							}
						}
					}
				}

				// Air is burning text
				npc.ai[3] += 1f;
				if (npc.ai[3] >= 450f && !text)
				{
					text = true;
					string key = "Mods.CalamityMod.ProfanedBossText";
					Color messageColor = Color.Orange;

					if (Main.netMode == NetmodeID.SinglePlayer)
						Main.NewText(Language.GetTextValue(key), messageColor);
					else if (Main.netMode == NetmodeID.Server)
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				}

				// Inflict Icarus Folly
				if (npc.ai[3] >= 600f)
				{
					if (Main.netMode != NetmodeID.Server)
					{
						Player player2 = Main.player[Main.myPlayer];
						bool inLiquid = (player2.wet || player2.honeyWet) && !player2.lavaWet;

						if (!player2.dead && player2.active && Vector2.Distance(player2.Center, vector) < 2800f && !inLiquid)
						{
							Main.PlaySound(SoundID.Item20, player2.position);
							player2.AddBuff(ModContent.BuffType<ExtremeGravity>(), 3000, true);

							for (int num621 = 0; num621 < 40; num621++)
							{
								int num622 = Dust.NewDust(new Vector2(player2.position.X, player2.position.Y),
									player2.width, player2.height, (int)CalamityDusts.ProfanedFire, 0f, 0f, 100, default, 2f);
								Main.dust[num622].velocity *= 3f;
								if (Main.rand.NextBool(2))
								{
									Main.dust[num622].scale = 0.5f;
									Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
								}
							}

							for (int num623 = 0; num623 < 60; num623++)
							{
								int num624 = Dust.NewDust(new Vector2(player2.position.X, player2.position.Y),
									player2.width, player2.height, (int)CalamityDusts.ProfanedFire, 0f, 0f, 100, default, 3f);
								Main.dust[num624].noGravity = true;
								Main.dust[num624].velocity *= 5f;
								num624 = Dust.NewDust(new Vector2(player2.position.X, player2.position.Y),
									player2.width, player2.height, (int)CalamityDusts.ProfanedFire, 0f, 0f, 100, default, 2f);
								Main.dust[num624].velocity *= 2f;
							}
						}
					}

					text = false;
					npc.ai[0] = -1f;
				}
			}

			// Molten blasts
			else if (npc.ai[0] == 3f)
			{
				npc.noGravity = true;
				npc.noTileCollide = true;

				float num852 = Math.Abs(vector.X - player.Center.X);
				if (num852 > distanceNeededToShoot && npc.position.Y < player.position.Y)
				{
					npc.ai[3] += 1f;

					int shootBoost = death ? 3 : (int)(4f * (1f - lifeRatio));
					int num856 = (expertMode ? 24 : 26) - shootBoost;
					if (CalamityWorld.bossRushActive)
						num856 = 20;

					num856 = (int)(num856 * attackRateMult);

					if (npc.ai[3] >= num856)
						npc.ai[3] = -num856;

					if (npc.ai[3] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
					{
						vector.X += npc.velocity.X * 7f;
						float num857 = player.position.X + player.width * 0.5f - vector.X;
						float num858 = player.Center.Y - vector.Y;
						float num859 = (float)Math.Sqrt(num857 * num857 + num858 * num858);

						float shootBoost2 = death ? 2.5f : 2.5f * (1f - lifeRatio);
						float num860 = (expertMode ? 10.25f : 9f) + shootBoost2;
						if (CalamityWorld.bossRushActive)
							num860 = 12.75f;

						if (revenge)
							num860 *= 1.15f;

						num859 = num860 / num859;
						num857 *= num859;
						num858 *= num859;

						int holyDamage = expertMode ? 42 : 55;
						Projectile.NewProjectile(vector.X, vector.Y, num857 * 0.1f, num858, ModContent.ProjectileType<MoltenBlast>(), holyDamage, 0f, Main.myPlayer, 0f, 0f);
					}
				}
				else if (npc.ai[3] < 0f)
					npc.ai[3] += 1f;

				npc.ai[1] += 1f;
				if (npc.ai[1] >= 300f)
					npc.ai[0] = -1f;
			}

			// Holy bombs
			else if (npc.ai[0] == 4f)
			{
				npc.noGravity = true;
				npc.noTileCollide = true;

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.ai[3] += 1f;

					int shootBoost = death ? 9 : (int)(10f * (1f - lifeRatio));
					int num864 = (expertMode ? 73 : 77) - shootBoost;
					if (npc.Calamity().enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive))
						num864 = 63;

					num864 = (int)(num864 * attackRateMult);

					if (npc.ai[3] >= num864)
					{
						npc.ai[3] = 0f;

						Vector2 vector113 = new Vector2(vector.X, npc.position.Y + npc.height - 14f);

						float num865 = npc.velocity.Y;
						if (num865 < 0f)
							num865 = 0f;

						num865 += expertMode ? 4f : 3f;

						int fireDamage = expertMode ? 48 : 60;
						Projectile.NewProjectile(vector113.X, vector113.Y, npc.velocity.X * 0.25f, num865, ModContent.ProjectileType<HolyBomb>(), fireDamage, 0f, Main.myPlayer, 0f, 0f);
					}
				}

				npc.ai[1] += 1f;
				if (npc.ai[1] >= 300f)
					npc.ai[0] = -1f;
			}

			// Cocoon spears
			else if (npc.ai[0] == 5f)
			{
				npc.noGravity = true;
				npc.noTileCollide = true;

				npc.TargetClosest(true);

				if (!targetDead)
				{
					if (npc.velocity.Length() <= 2f)
						npc.velocity = Vector2.Zero;
					if (npc.velocity.Length() > 2f)
					{
						npc.velocity *= 0.9f;
						return;
					}
				}

				if (npc.ai[1] == 0f)
				{
					for (int x = 0; x < Main.maxProjectiles; x++)
					{
						Projectile projectile = Main.projectile[x];
						if (projectile.active)
						{
							if (projectile.type == ModContent.ProjectileType<HolyFire2>() || projectile.type == ModContent.ProjectileType<HolyFlare>())
							{
								projectile.Kill();
							}
						}
					}
				}

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					float shootBoost = death ? 1f : 1f * (1f - lifeRatio);
					npc.ai[2] += ((npc.Calamity().enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive)) ? 1.5f : 1f) + shootBoost;

					if (CalamityWorld.bossRushActive)
						npc.ai[2] += 0.25f;

					double count = 18D * attackRateMult;
					if (npc.ai[2] >= (float)count)
					{
						npc.ai[2] = 0f;

						float velocity = 3f;
						int damage = expertMode ? 52 : 65;
						int projectileType = ModContent.ProjectileType<HolySpear>();

						if (calamityGlobalNPC.newAI[2] % 2f == 0f)
						{
							int totalProjectiles = 12;
							double radians = MathHelper.TwoPi / totalProjectiles;
							Vector2 spinningPoint = Vector2.Normalize(new Vector2(-calamityGlobalNPC.newAI[1], -velocity)) * velocity;

							for (int i = 0; i < totalProjectiles; i++)
							{
								Vector2 vector2 = spinningPoint.RotatedBy(radians * i);
								Projectile.NewProjectile(vector, vector2, projectileType, damage, 0f, Main.myPlayer, 0f, 0f);
							}

							calamityGlobalNPC.newAI[1] += 0.2f;
						}

						calamityGlobalNPC.newAI[2] += 1f;

						velocity = expertMode ? 12f : 10f;
						Vector2 velocity2 = Vector2.Normalize(player.Center - vector) * velocity;
						Projectile.NewProjectile(vector, velocity2, projectileType, damage, 0f, Main.myPlayer, 1f, 0f);
					}
				}

				npc.ai[1] += 1f;
				if (npc.ai[1] >= 300f)
					npc.ai[0] = -1f;
			}

			// Crystal
			else if (npc.ai[0] == 6f)
			{
				npc.noGravity = true;
				npc.noTileCollide = true;

				npc.TargetClosest(true);

				if (!targetDead)
					npc.velocity *= 0.9f;

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.ai[1] += 1f;
					if (npc.ai[1] >= 120f)
					{
						int damage = expertMode ? 57 : 70;
						Projectile.NewProjectile(player.Center.X, player.Center.Y - 360f, 0f, 0f, ModContent.ProjectileType<ProvidenceCrystal>(), damage, 0f, player.whoAmI, 0f, 0f);
						npc.ai[0] = -1f;
					}
				}
			}

			// Holy ray
			else if (npc.ai[0] == 7f)
			{
				npc.noGravity = true;
				npc.noTileCollide = true;

				Vector2 value19 = new Vector2(27f, 59f);

				float rotation = 450f + (guardianAmt * 18);

				npc.ai[2] += 1f;
				if (npc.ai[2] < 120f)
				{
					npc.localAI[1] -= 0.07f;
					if (npc.localAI[1] < 0f)
						npc.localAI[1] = 0f;

					if (npc.ai[2] >= 40f)
					{
						int num1220 = 0;
						if (npc.ai[2] >= 80f)
							num1220 = 1;

						for (int num1221 = 0; num1221 < 1 + num1220; num1221++)
						{
							int num1222 = (int)CalamityDusts.ProfanedFire;
							float num1223 = 1.2f;
							if (num1221 % 2 == 1)
								num1223 = 2.8f;

							Vector2 vector199 = vector + ((float)Main.rand.NextDouble() * MathHelper.TwoPi).ToRotationVector2() * value19 / 2f;
							int num1224 = Dust.NewDust(vector199 - Vector2.One * 8f, 16, 16, num1222, npc.velocity.X / 2f, npc.velocity.Y / 2f, 0, default, 1f);
							Main.dust[num1224].velocity = Vector2.Normalize(vector - vector199) * 3.5f * (10f - num1220 * 2f) / 10f;
							Main.dust[num1224].noGravity = true;
							Main.dust[num1224].scale = num1223;
						}
					}
				}
				else if (npc.ai[2] < (revenge ? 220f : 300f))
				{
					if (npc.ai[2] == 120f)
					{
						if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, vector) < 2800f)
						{
							Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ProvidenceHolyRay"),
								(int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y);
						}

						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							npc.TargetClosest(false);

							Vector2 vector200 = player.Center - vector;
							vector200.Normalize();

							float num1225 = -1f;
							if (vector200.X < 0f)
								num1225 = 1f;

							vector200 = vector200.RotatedBy(-(double)num1225 * MathHelper.TwoPi / 6f, default);
							Projectile.NewProjectile(vector.X, vector.Y - 32f, vector200.X, vector200.Y, ModContent.ProjectileType<ProvidenceHolyRay>(), 100, 0f, Main.myPlayer, num1225 * MathHelper.TwoPi / rotation, npc.whoAmI);

							if (revenge)
								Projectile.NewProjectile(vector.X, vector.Y - 32f, -vector200.X, -vector200.Y, ModContent.ProjectileType<ProvidenceHolyRay>(), 100, 0f, Main.myPlayer, -num1225 * MathHelper.TwoPi / rotation, npc.whoAmI);

							npc.ai[3] = (vector200.ToRotation() + MathHelper.TwoPi + MathHelper.Pi) * num1225;
							npc.netUpdate = true;
						}
					}

					npc.localAI[1] += 0.05f;
					if (npc.localAI[1] > 1f)
						npc.localAI[1] = 1f;

					float num1226 = (npc.ai[3] >= 0f).ToDirectionInt();
					float num1227 = npc.ai[3];
					if (num1227 < 0f)
						num1227 *= -1f;
					num1227 += -(MathHelper.TwoPi + MathHelper.Pi);
					num1227 += num1226 * MathHelper.TwoPi / rotation;

					npc.localAI[0] = num1227;
				}
				else
				{
					npc.localAI[1] -= 0.07f;
					if (npc.localAI[1] < 0f)
						npc.localAI[1] = 0f;
				}

				npc.ai[1] += 1f;
				if (npc.ai[1] >= (revenge ? 235f : 315f))
					npc.ai[0] = -1f;
			}
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void NPCLoot()
        {
            DropHelper.DropBags(npc);
            DropHelper.DropItemChance(npc, ModContent.ItemType<ProvidenceTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeProvidence>(), true, !CalamityWorld.downedProvidence);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedProvidence, 5, 2, 1);

            DropHelper.DropItemCondition(npc, ModContent.ItemType<RuneofCos>(), true, !CalamityWorld.downedProvidence);

			npc.Calamity().SetNewShopVariable(new int[] { ModContent.NPCType<THIEF>() }, CalamityWorld.downedProvidence);

			//Accessories clientside only in Expert
			DropHelper.DropItemCondition(npc, ModContent.ItemType<ElysianWings>(), true, biomeType != 2 && Main.expertMode);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<ElysianAegis>(), true, biomeType == 2 && Main.expertMode);
			//drops pre-scal, cannot be sold, does nothing aka purely vanity. Requires at least expert for consistency with other post scal dev items.
			bool shouldDrop = challenge || (Main.expertMode && Main.rand.NextBool(CalamityWorld.downedSCal ? 10 : 200));
			DropHelper.DropItemCondition(npc, ModContent.ItemType<ProfanedSoulCrystal>(), true, shouldDrop);
			// All other drops are contained in the bag, so they only drop directly on Normal
			if (!Main.expertMode)
            {
                // Materials
                DropHelper.DropItemSpray(npc, ModContent.ItemType<UnholyEssence>(), 20, 30);
                DropHelper.DropItemSpray(npc, ModContent.ItemType<DivineGeode>(), 10, 15);

                // Weapons
                DropHelper.DropItemChance(npc, ModContent.ItemType<HolyCollider>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<SolarFlare>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<TelluricGlare>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<BlissfulBombardier>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<PurgeGuzzler>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<MoltenAmputator>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<DazzlingStabberStaff>(), 4);

                // Equipment
                DropHelper.DropItemChance(npc, ModContent.ItemType<SamuraiBadge>(), 40);
				DropHelper.DropItemCondition(npc, ModContent.ItemType<ElysianWings>(), biomeType != 2);
				DropHelper.DropItemCondition(npc, ModContent.ItemType<ElysianAegis>(), biomeType == 2);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<ProvidenceMask>(), 7);
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                SpawnLootBox();
            }

            // If Providence has not been killed, notify players of Uelibloom Ore
            if (!CalamityWorld.downedProvidence)
            {
                string key2 = "Mods.CalamityMod.ProfanedBossText3";
                Color messageColor2 = Color.Orange;
                string key3 = "Mods.CalamityMod.TreeOreText";
                Color messageColor3 = Color.LightGreen;

                WorldGenerationMethods.SpawnOre(ModContent.TileType<UelibloomOre>(), 15E-05, .4f, .8f);

                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue(key2), messageColor2);
                    Main.NewText(Language.GetTextValue(key3), messageColor3);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key2), messageColor2);
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key3), messageColor3);
                }
            }

			if (challenge)
			{
				if (Main.netMode == NetmodeID.SinglePlayer)
				{
					Main.NewText(Language.GetTextValue("Mods.CalamityMod.ProfanedBossText4"), Color.DarkOrange);
				} 
			}

            // Mark Providence as dead
            CalamityWorld.downedProvidence = true;
            CalamityMod.UpdateServerBoolean();
        }

        private void SpawnLootBox()
        {
            int tileCenterX = (int)(npc.position.X + (float)(npc.width / 2)) / 16;
            int tileCenterY = (int)(npc.position.Y + (float)(npc.height / 2)) / 16;
            int halfBox = npc.width / 2 / 16 + 1;
            for (int x = tileCenterX - halfBox; x <= tileCenterX + halfBox; x++)
            {
                for (int y = tileCenterY - halfBox; y <= tileCenterY + halfBox; y++)
                {
                    if ((x == tileCenterX - halfBox || x == tileCenterX + halfBox || y == tileCenterY - halfBox || y == tileCenterY + halfBox)
                        && !Main.tile[x, y].active())
                    {
                        Main.tile[x, y].type = (ushort)ModContent.TileType<ProfanedRock>();
                        Main.tile[x, y].active(true);
                    }
                    Main.tile[x, y].lava(false);
                    Main.tile[x, y].liquid = 0;

                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                    else
                        WorldGen.SquareTileFrame(x, y, true);
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.npcTexture[npc.type];
			Texture2D textureGlow = ModContent.GetTexture("CalamityMod/NPCs/Providence/ProvidenceGlow");
			Texture2D textureGlow2 = ModContent.GetTexture("CalamityMod/NPCs/Providence/ProvidenceGlow2");

			if (npc.ai[0] == 2f || npc.ai[0] == 5f)
            {
				if (!useDefenseFrames)
				{
					texture = ModContent.GetTexture("CalamityMod/NPCs/Providence/ProvidenceDefense");
					textureGlow = ModContent.GetTexture("CalamityMod/NPCs/Providence/ProvidenceDefenseGlow");
					textureGlow2 = ModContent.GetTexture("CalamityMod/NPCs/Providence/ProvidenceDefenseGlow2");
				}
				else
				{
					texture = ModContent.GetTexture("CalamityMod/NPCs/Providence/ProvidenceDefenseAlt");
					textureGlow = ModContent.GetTexture("CalamityMod/NPCs/Providence/ProvidenceDefenseAltGlow");
					textureGlow2 = ModContent.GetTexture("CalamityMod/NPCs/Providence/ProvidenceDefenseAltGlow2");
				}
            }
            else
            {
				if (frameUsed == 0)
				{
					texture = Main.npcTexture[npc.type];
					textureGlow = ModContent.GetTexture("CalamityMod/NPCs/Providence/ProvidenceGlow");
					textureGlow2 = ModContent.GetTexture("CalamityMod/NPCs/Providence/ProvidenceGlow2");
				}
				else if (frameUsed == 1)
				{
					texture = ModContent.GetTexture("CalamityMod/NPCs/Providence/ProvidenceAlt");
					textureGlow = ModContent.GetTexture("CalamityMod/NPCs/Providence/ProvidenceAltGlow");
					textureGlow2 = ModContent.GetTexture("CalamityMod/NPCs/Providence/ProvidenceAltGlow2");
				}
				else if (frameUsed == 2)
				{
					texture = ModContent.GetTexture("CalamityMod/NPCs/Providence/ProvidenceAttack");
					textureGlow = ModContent.GetTexture("CalamityMod/NPCs/Providence/ProvidenceAttackGlow");
					textureGlow2 = ModContent.GetTexture("CalamityMod/NPCs/Providence/ProvidenceAttackGlow2");
				}
				else
				{
					texture = ModContent.GetTexture("CalamityMod/NPCs/Providence/ProvidenceAttackAlt");
					textureGlow = ModContent.GetTexture("CalamityMod/NPCs/Providence/ProvidenceAttackAltGlow");
					textureGlow2 = ModContent.GetTexture("CalamityMod/NPCs/Providence/ProvidenceAttackAltGlow2");
				}
            }

			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));
			Color color36 = Color.White;
			float amount9 = 0.5f;
			int num153 = 5;

			if (CalamityMod.CalamityConfig.Afterimages)
			{
				for (int num155 = 1; num155 < num153; num155 += 2)
				{
					Color color38 = lightColor;
					color38 = Color.Lerp(color38, color36, amount9);
					color38 = npc.GetAlpha(color38);
					color38 *= (float)(num153 - num155) / 15f;
					Vector2 vector41 = npc.oldPos[num155] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
					vector41 -= new Vector2((float)texture.Width, (float)(texture.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
					vector41 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
					spriteBatch.Draw(texture, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2((float)texture.Width, (float)(texture.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
			spriteBatch.Draw(texture, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			Color color37 = Color.Lerp(Color.White, Color.Yellow, 0.5f);
			Color color42 = Color.Lerp(Color.White, Color.Violet, 0.5f);

			if (CalamityMod.CalamityConfig.Afterimages)
			{
				for (int num163 = 1; num163 < num153; num163++)
				{
					Color color41 = color37;
					color41 = Color.Lerp(color41, color36, amount9);
					color41 = npc.GetAlpha(color41);
					color41 *= (float)(num153 - num163) / 15f;
					Vector2 vector44 = npc.oldPos[num163] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
					vector44 -= new Vector2((float)textureGlow.Width, (float)(textureGlow.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
					vector44 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
					spriteBatch.Draw(textureGlow, vector44, npc.frame, color41, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

					Color color43 = color42;
					color43 = Color.Lerp(color43, color36, amount9);
					color43 = npc.GetAlpha(color43);
					color43 *= (float)(num153 - num163) / 15f;
					spriteBatch.Draw(textureGlow2, vector44, npc.frame, color43, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			spriteBatch.Draw(textureGlow, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			spriteBatch.Draw(textureGlow2, vector43, npc.frame, color42, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
        }

        public override void FindFrame(int frameHeight) //9 total frames
        {
            if (npc.ai[0] == 2f || npc.ai[0] == 5f)
            {
                if (!useDefenseFrames)
                {
                    npc.frameCounter += 1.0;
                    if (npc.frameCounter > 5.0)
                    {
                        npc.frame.Y = npc.frame.Y + frameHeight;
                        npc.frameCounter = 0.0;
                    }
                    if (npc.frame.Y >= frameHeight * 3)
                    {
                        npc.frame.Y = 0;
                        useDefenseFrames = true;
                    }
                }
                else
                {
                    npc.frameCounter += 1.0;
                    if (npc.frameCounter > 5.0)
                    {
                        npc.frame.Y = npc.frame.Y + frameHeight;
                        npc.frameCounter = 0.0;
                    }
                    if (npc.frame.Y >= frameHeight * 2)
                        npc.frame.Y = frameHeight * 2;
                }
            }
            else
            {
                if (useDefenseFrames)
                    useDefenseFrames = false;

                npc.frameCounter += 1.0;
                if (npc.frameCounter > 5.0)
                {
                    npc.frameCounter = 0.0;
                    npc.frame.Y = npc.frame.Y + frameHeight;
                }
                if (npc.frame.Y >= frameHeight * 3) //6
                {
                    npc.frame.Y = 0;
                    frameUsed++;
                }
                if (frameUsed > 3)
                    frameUsed = 0;
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 2f;
            return null;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.8f);
        }

        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
			if (challenge)
			{
				bool goldenGun = projectile.type == ModContent.ProjectileType<GoldenGunProj>();
				bool allowedClass = projectile.minion || (!projectile.melee && !projectile.ranged && !projectile.magic && !projectile.thrown && !projectile.Calamity().rogue);
				bool allowedDamage = allowedClass && damage <= (npc.lifeMax * 0.005f); //0.5% max hp
				bool allowedBabs = Main.player[projectile.owner].Calamity().pArtifact && !Main.player[projectile.owner].Calamity().profanedCrystalBuffs;
				if ((!goldenGun && !allowedDamage && projectile.type != ModContent.ProjectileType<MiniGuardianDefense>() && projectile.type != ModContent.ProjectileType<MiniGuardianAttack>()) || !allowedBabs)
				{
					challenge = false;
				}
			}
        }

        public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
        {
			if (challenge)
				challenge = false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if(npc.soundDelay == 0)
            {
                npc.soundDelay = 8;
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/ProvidenceHurt"), npc.Center);
            }
            for (int k = 0; k < 15; k++)
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.ProfanedFire, hitDirection, -1f, 0, default, 1f);

            if (npc.life <= 0)
            {
                float randomSpread = (float)(Main.rand.Next(-50, 50) / 100);
                Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/Providence"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/Providence2"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/Providence3"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/Providence4"), 1f);
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 400;
                npc.height = 350;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 60; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.ProfanedFire, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 90; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.ProfanedFire, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.ProfanedFire, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }
    }
}
