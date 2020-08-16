using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Dyes;
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
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Tiles.FurnitureProfaned;
using CalamityMod.Tiles.Ores;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

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
        internal bool challenge = Main.expertMode/* && Main.netMode == NetmodeID.SinglePlayer*/; //Used to determine if Profaned Soul Crystal should drop, couldn't figure out mp mems always dropping it so challenge is singleplayer only.
		internal bool hasTakenDaytimeDamage = false;

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
			npc.DR_NERD(normalDR, null, null, null, true);
			CalamityGlobalNPC global = npc.Calamity();
            global.flatDRReductions.Add(BuffID.CursedInferno, 0.05f);
            npc.LifeMaxNERB(440000, 500000, 12500000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.value = Item.buyPrice(0, 50, 0, 0);
            npc.boss = true;
			npc.Opacity = 0f;
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
            npc.buffImmune[ModContent.BuffType<Shred>()] = false;
            npc.buffImmune[ModContent.BuffType<WarCleave>()] = false;
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
			writer.Write(npc.localAI[2]);
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
			npc.localAI[2] = reader.ReadSingle();
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

            // Target's current biome
            bool isHoly = player.ZoneHoly;
            bool isHell = player.ZoneUnderworldHeight;

            // Fire projectiles at normal rate or not
            bool normalAttackRate = true;

			// Is in spawning animation
			float spawnAnimationTime = 180f;
			bool spawnAnimation = calamityGlobalNPC.newAI[3] < spawnAnimationTime;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Night bool
			bool nightTime = !Main.dayTime;

			// Play enrage animation if night starts
			if (nightTime && calamityGlobalNPC.newAI[3] == spawnAnimationTime)
			{
				npc.ai[0] = 0f;
				npc.ai[1] = 0f;
				npc.ai[2] = 0f;
				npc.ai[3] = 0f;
				calamityGlobalNPC.newAI[1] = 0f;
				calamityGlobalNPC.newAI[2] = 0f;
				calamityGlobalNPC.newAI[3] = 0f;
			}

			// Difficulty bools
			bool death = CalamityWorld.death || CalamityWorld.bossRushActive || nightTime;
			bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive || nightTime;
			bool expertMode = Main.expertMode || CalamityWorld.bossRushActive || nightTime;
			bool enraged = npc.Calamity().enraged > 0 || (CalamityConfig.Instance.BossRushXerocCurse && CalamityWorld.bossRushActive);

			// Projectile damage values
			bool scaleExpertProjectileDamage = Main.expertMode && !nightTime;
			int holyLaserDamage = 100;
			int crystalDamage = scaleExpertProjectileDamage ? 57 : 70;
			int holySpearDamage = scaleExpertProjectileDamage ? 52 : 65;
			int holyBombDamage = scaleExpertProjectileDamage ? 48 : 60;
			int moltenBlastDamage = scaleExpertProjectileDamage ? 42 : 55;
			int holyFireDamage = scaleExpertProjectileDamage ? 48 : 60;
			int holyBlastDamage = scaleExpertProjectileDamage ? 52 : 65;

			// Increase all projectile damage at night
			int projectileDamageNight = 100;
			if (nightTime)
			{
				holyLaserDamage = crystalDamage = holySpearDamage = holyBombDamage = moltenBlastDamage = holyFireDamage = holyBlastDamage = projectileDamageNight;
			}

			// Change dust type at night
			int dustType = Main.dayTime ? (int)CalamityDusts.ProfanedFire : (int)CalamityDusts.Nightwither;

			// Phase times
			float phaseTime = nightTime ? 240f : 300f;
			float crystalPhaseTime = nightTime ? 60f : 120f;
			int nightCrystalTime = 210;
			float attackDelayAfterCocoon = 90f;

			// Phases
			bool ignoreGuardianAmt = lifeRatio < (death ? 0.2f : 0.15f);
            bool phase2 = (lifeRatio < 0.75f || death) && !nightTime;
			bool delayAttacks = npc.localAI[2] > 0f;

			// Spear phase
			float spearRateIncrease = death ? 1f : 1f * (1f - lifeRatio);
			float enragedSpearRateIncrease = 0.5f;
			float bossRushSpearRateIncrease = 0.25f;
			float baseSpearRate = 18f;
			float spearRate = 1f + spearRateIncrease;

			if (enraged)
				spearRate += enragedSpearRateIncrease;

			if (CalamityWorld.bossRushActive)
				spearRate += bossRushSpearRateIncrease;

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
            if (!player.active || player.dead)
            {
				if (!player.active || player.dead)
				{
					npc.TargetClosest(false);
					player = Main.player[npc.target];
				}
				if (!player.active || player.dead)
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
			if (!nightTime)
			{
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
							NPC.NewNPC(x - 100, y - 100, ModContent.NPCType<ProvSpawnDefense>());
							NPC.NewNPC(x + 100, y - 100, ModContent.NPCType<ProvSpawnHealer>());
							NPC.NewNPC(x, y + 100, ModContent.NPCType<ProvSpawnOffense>());
						}
					}
				}
			}

            // Set DR based on current attack phase
            npc.Calamity().DR = (npc.ai[0] == 2f || npc.ai[0] == 5f || npc.ai[0] == 7f || spawnAnimation) ?
				cocoonDR : delayAttacks ?
				MathHelper.Lerp(normalDR, cocoonDR, npc.localAI[2] / attackDelayAfterCocoon) : normalDR;

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
                if (CalamityWorld.bossRushActive || nightTime)
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

				// Slowly drift down when spawning
				if (spawnAnimation)
				{
					float minSpawnVelocity = 0.4f;
					float maxSpawnVelocity = 4f;
					float velocityY = maxSpawnVelocity - MathHelper.Lerp(minSpawnVelocity, maxSpawnVelocity, calamityGlobalNPC.newAI[3] / spawnAnimationTime);
					npc.velocity = new Vector2(0f, velocityY);
				}
			}

			// Phase switch
			if (npc.ai[0] == -1f)
			{
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
							phase = (useCrystal || nightTime) ? 6 : 3;
							break;
						case 4:
							phase = useCrystal ? 3 : 2;
							break; // 600
						case 5:
							phase = useCrystal ? 2 : 1;
							break; // 900
						case 6:
							phase = (useLaser || nightTime) ? 7 : 4; // 1875 or 1800
							if (useLaser || nightTime)
							{
								npc.TargetClosest(false);
							}
							break; // 1200
						case 7:
							phase = (useLaser || nightTime) ? 4 : 3;
							break; // 1500
						case 8:
							phase = (useLaser || nightTime) ? 3 : 5;
							break;
						case 9:
							phase = 0;
							break; // 2175 or 2100
						case 10:
							phase = (useCrystal || nightTime) ? 6 : 2;
							break;
						case 11:
							phase = nightTime ? 2 : 3;
							break; // 300
						case 12:
							phase = (useLaser || nightTime) ? 7 : 4; // 675 or 600
							if (useLaser || nightTime)
							{
								npc.TargetClosest(false);
							}
							break;
						case 13:
							phase = 5;
							break; // 975 or 900
						case 14:
							phase = (useLaser || nightTime) ? 4 : 0;
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

				// Reset attack delay for laser
				if (phase == 7)
					npc.localAI[2] = 0f;

				// Reset arrays
				npc.ai[0] = phase;
				npc.ai[1] = 0f;
				npc.ai[2] = 0f;
				npc.ai[3] = 0f;
				calamityGlobalNPC.newAI[1] = 0f;
				calamityGlobalNPC.newAI[2] = 0f;
			}

			// Holy blasts
			else if (npc.ai[0] == 0f)
			{
				if (spawnAnimation)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient && calamityGlobalNPC.newAI[3] == 0f)
						Projectile.NewProjectile(vector + new Vector2(0f, -80f), Vector2.Zero, ModContent.ProjectileType<HolyAura>(), 0, 0f, Main.myPlayer, biomeType, 0f);

					if (calamityGlobalNPC.newAI[3] == 10f && nightTime)
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ProvidenceHolyRay"), (int)npc.position.X, (int)npc.position.Y);

					if (calamityGlobalNPC.newAI[3] > 10f && calamityGlobalNPC.newAI[3] < 150f)
					{
						int dustAmt = (int)MathHelper.Lerp(4f, 8f, calamityGlobalNPC.newAI[3] / spawnAnimationTime);
						for (int m = 0; m < dustAmt; m++)
						{
							float fade = MathHelper.Lerp(1.3f, 0.7f, npc.Opacity) * CalamityUtils.GetLerpValue(0f, 120f, calamityGlobalNPC.newAI[3], clamped: true);
							Color newColor = Main.hslToRgb(calamityGlobalNPC.newAI[3] / 180f, 1f, 0.5f);

							if (!nightTime)
							{
								newColor.R = 255;
								if (biomeType == 2)
									newColor.B = 0;
							}
							else
							{
								newColor.B = 255;
								if (biomeType == 2)
									newColor.G = 0;
								else
									newColor.R = 0;
							}

							int dust = Dust.NewDust(npc.position, npc.width, npc.height, 267, 0f, 0f, 0, newColor);
							Main.dust[dust].position = npc.Center + Main.rand.NextVector2Circular(npc.width * 2f, npc.height * 2f) + new Vector2(0f, -150f);
							Main.dust[dust].velocity *= Main.rand.NextFloat() * 0.8f;
							Main.dust[dust].noGravity = true;
							Main.dust[dust].fadeIn = 0.6f + Main.rand.NextFloat() * 0.7f * fade;
							Main.dust[dust].velocity += Vector2.UnitY * 3f;
							Main.dust[dust].scale = 1.2f;

							if (dust != 6000)
							{
								Dust dust2 = Dust.CloneDust(dust);
								dust2.scale /= 2f;
								dust2.fadeIn *= 0.85f;
								dust2.color = new Color(255, 255, 0, 255);
							}
						}
					}

					npc.Opacity = MathHelper.Clamp(calamityGlobalNPC.newAI[3] / spawnAnimationTime, 0f, 1f);

					calamityGlobalNPC.newAI[3] += 1f;

					if (nightTime && calamityGlobalNPC.newAI[3] >= spawnAnimationTime)
						calamityGlobalNPC.newAI[3] += 1f;

					return;
				}

				// Attack delay after cocoon phase
				if (delayAttacks)
				{
					npc.localAI[2] -= 1f;
					return;
				}

				float num852 = Math.Abs(vector.X - player.Center.X);
				if (num852 > distanceNeededToShoot && npc.position.Y < player.position.Y)
				{
					npc.ai[3] += 1f;

					int shootBoost = death ? 3 : (int)(4f * (1f - lifeRatio));
					int num856 = (expertMode ? 24 : 26) - shootBoost;
					if (enraged)
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
						if (enraged)
							num860 = 12.75f;

						if (revenge)
							num860 *= 1.15f;

						num859 = num860 / num859;
						num857 *= num859;
						num858 *= num859;

						Projectile.NewProjectile(vector.X, vector.Y, num857, num858, ModContent.ProjectileType<HolyBlast>(), holyBlastDamage, 0f, Main.myPlayer, 0f, 0f);
					}
				}
				else if (npc.ai[3] < 0f)
					npc.ai[3] += 1f;

				npc.ai[1] += 1f;
				if (npc.ai[1] >= phaseTime)
					npc.ai[0] = -1f;
			}

			// Holy fire
			else if (npc.ai[0] == 1f)
			{
				// Attack delay after cocoon phase
				if (delayAttacks)
				{
					npc.localAI[2] -= 1f;
					return;
				}

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

						Projectile.NewProjectile(vector113.X, vector113.Y, npc.velocity.X * 0.25f, num865, ModContent.ProjectileType<HolyFire>(), holyFireDamage, 0f, Main.myPlayer, 0f, 0f);
					}
				}

				npc.ai[1] += 1f;
				if (npc.ai[1] >= phaseTime)
					npc.ai[0] = -1f;
			}

			// Cocoon flames
			else if (npc.ai[0] == 2f)
			{
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
							bool normalSpread = calamityGlobalNPC.newAI[1] % 2f == 0f;
							Vector2 spinningPoint = normalSpread ? new Vector2(0f, -velocity) : Vector2.Normalize(new Vector2(-velocity, -velocity));
							double radians = MathHelper.TwoPi / chains;
							Main.PlaySound(SoundID.Item20, npc.position);
							for (int i = 0; i < chains; i++)
							{
								Vector2 vector2 = spinningPoint.RotatedBy(radians * i + MathHelper.ToRadians(npc.ai[2]));

								if (!normalSpread)
									vector2 *= velocity;

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
							Main.PlaySound(SoundID.Item20, npc.position);
							for (int i = 0; i < totalProjectiles; i++)
							{
								Vector2 vector2 = new Vector2(0f, -velocity).RotatedBy(radians * i);

								int projectileType = ModContent.ProjectileType<HolyBurnOrb>();
								if (Main.rand.NextBool(4) && !death)
									projectileType = ModContent.ProjectileType<HolyLight>();

								Projectile.NewProjectile(fireFrom, vector2, projectileType, 0, 0f, Main.myPlayer, 0f, 0f);
							}

							Vector2 velocity2 = Vector2.Normalize(player.Center - fireFrom) * velocity;
							Projectile.NewProjectile(fireFrom, velocity2, ModContent.ProjectileType<HolyBurnOrb>(), 0, 0f, Main.myPlayer, 0f, 0f);
						}
					}
				}

				if (npc.ai[3] == 0f)
					DespawnSpecificProjectiles();

				// Air is burning text
				npc.ai[3] += 1f;
				if (npc.ai[3] >= (phaseTime * 1.5f) && !text)
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
				if (npc.ai[3] >= (phaseTime * 2f))
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
									player2.width, player2.height, dustType, 0f, 0f, 100, default, 2f);
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
									player2.width, player2.height, dustType, 0f, 0f, 100, default, 3f);
								Main.dust[num624].noGravity = true;
								Main.dust[num624].velocity *= 5f;
								num624 = Dust.NewDust(new Vector2(player2.position.X, player2.position.Y),
									player2.width, player2.height, dustType, 0f, 0f, 100, default, 2f);
								Main.dust[num624].velocity *= 2f;
							}
						}
					}

					text = false;
					npc.ai[0] = -1f;
					npc.localAI[2] = attackDelayAfterCocoon;
				}
			}

			// Molten blasts
			else if (npc.ai[0] == 3f)
			{
				// Attack delay after cocoon phase
				if (delayAttacks)
				{
					npc.localAI[2] -= 1f;
					return;
				}

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

						Projectile.NewProjectile(vector.X, vector.Y, num857 * 0.1f, num858, ModContent.ProjectileType<MoltenBlast>(), moltenBlastDamage, 0f, Main.myPlayer, 0f, 0f);
					}
				}
				else if (npc.ai[3] < 0f)
					npc.ai[3] += 1f;

				npc.ai[1] += 1f;
				if (npc.ai[1] >= phaseTime)
					npc.ai[0] = -1f;
			}

			// Holy bombs
			else if (npc.ai[0] == 4f)
			{
				// Attack delay after cocoon phase
				if (delayAttacks)
				{
					npc.localAI[2] -= 1f;
					return;
				}

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.ai[3] += 1f;

					int shootBoost = death ? 9 : (int)(10f * (1f - lifeRatio));
					int num864 = (expertMode ? 73 : 77) - shootBoost;
					if (enraged)
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

						Projectile.NewProjectile(vector113.X, vector113.Y, npc.velocity.X * 0.25f, num865, ModContent.ProjectileType<HolyBomb>(), holyBombDamage, 0f, Main.myPlayer, 0f, 0f);
					}
				}

				npc.ai[1] += 1f;
				if (npc.ai[1] >= phaseTime)
					npc.ai[0] = -1f;
			}

			// Cocoon spears
			else if (npc.ai[0] == 5f)
			{
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
					DespawnSpecificProjectiles();

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.ai[2] += spearRate;
					if (npc.ai[2] >= (float)(baseSpearRate * attackRateMult))
					{
						npc.ai[2] = 0f;

						Vector2 fireFrom = new Vector2(vector.X, vector.Y + 20f);

						Main.PlayTrackedSound(SoundID.DD2_BetsyFireballShot, fireFrom);

						float velocity = 3f;
						int projectileType = ModContent.ProjectileType<HolySpear>();

						if (calamityGlobalNPC.newAI[2] % 2f == 0f)
						{
							int totalProjectiles = 12;
							double radians = MathHelper.TwoPi / totalProjectiles;
							Vector2 spinningPoint = Vector2.Normalize(new Vector2(-calamityGlobalNPC.newAI[1], -velocity));

							for (int i = 0; i < totalProjectiles; i++)
							{
								Vector2 vector2 = spinningPoint.RotatedBy(radians * i) * velocity;
								Projectile.NewProjectile(fireFrom, vector2, projectileType, holySpearDamage, 0f, Main.myPlayer, 0f, 0f);
							}

							float radialOffset = MathHelper.Lerp(0.2f, 0.4f, spearRateIncrease);
							calamityGlobalNPC.newAI[1] += radialOffset;
						}

						calamityGlobalNPC.newAI[2] += 1f;

						velocity = expertMode ? 12f : 10f;
						Vector2 velocity2 = Vector2.Normalize(player.Center - fireFrom) * velocity;
						Projectile.NewProjectile(fireFrom, velocity2, projectileType, holySpearDamage, 0f, Main.myPlayer, 1f, 0f);
					}
				}

				npc.ai[1] += 1f;
				if (npc.ai[1] >= phaseTime)
				{
					npc.ai[0] = -1f;
					npc.localAI[2] = attackDelayAfterCocoon;
				}
			}

			// Crystal
			else if (npc.ai[0] == 6f)
			{
				npc.TargetClosest(true);

				if (!targetDead)
					npc.velocity *= 0.9f;

				npc.ai[1] += 1f;
				if (npc.ai[1] >= crystalPhaseTime)
				{
					if (npc.ai[1] == crystalPhaseTime && Main.netMode != NetmodeID.MultiplayerClient)
					{
						int proj = Projectile.NewProjectile(player.Center.X, player.Center.Y - 360f, 0f, 0f, ModContent.ProjectileType<ProvidenceCrystal>(), crystalDamage, 0f, player.whoAmI, 0f, 0f);

						if (nightTime)
							Main.projectile[proj].timeLeft = nightCrystalTime;
					}

					if (npc.ai[1] >= crystalPhaseTime + nightCrystalTime || !nightTime)
						npc.ai[0] = -1f;
				}
			}

			// Holy ray
			else if (npc.ai[0] == 7f)
			{
				Vector2 value19 = new Vector2(27f, 59f);

				float rotation = 450f + (guardianAmt * 18);

				npc.ai[2] += 1f;
				if (npc.ai[2] < 120f)
				{
					if (npc.ai[2] >= 40f)
					{
						int num1220 = 0;
						if (npc.ai[2] >= 80f)
							num1220 = 1;

						for (int d = 0; d < 1 + num1220; d++)
						{
							float scalar = 1.2f;
							if (d % 2 == 1)
								scalar = 2.8f;

							Vector2 vector199 = new Vector2(vector.X, vector.Y + 32f) + ((float)Main.rand.NextDouble() * MathHelper.TwoPi).ToRotationVector2() * value19 / 2f;
							int index = Dust.NewDust(vector199 - Vector2.One * 8f, 16, 16, dustType, npc.velocity.X / 2f, npc.velocity.Y / 2f, 0, default, 1f);
							Main.dust[index].velocity = Vector2.Normalize(vector - vector199) * 3.5f * (10f - num1220 * 2f) / 10f;
							Main.dust[index].noGravity = true;
							Main.dust[index].scale = scalar;
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

							Vector2 velocity = player.Center - vector;
							velocity.Normalize();

							float num1225 = -1f;
							if (velocity.X < 0f)
								num1225 = 1f;

							velocity = velocity.RotatedBy(-(double)num1225 * MathHelper.TwoPi / 6f, default);
							Projectile.NewProjectile(vector.X, vector.Y + 32f, velocity.X, velocity.Y, ModContent.ProjectileType<ProvidenceHolyRay>(), holyLaserDamage, 0f, Main.myPlayer, num1225 * MathHelper.TwoPi / rotation, npc.whoAmI);

							if (revenge)
								Projectile.NewProjectile(vector.X, vector.Y + 32f, -velocity.X, -velocity.Y, ModContent.ProjectileType<ProvidenceHolyRay>(), holyLaserDamage, 0f, Main.myPlayer, -num1225 * MathHelper.TwoPi / rotation, npc.whoAmI);

							npc.ai[3] = (velocity.ToRotation() + MathHelper.TwoPi + MathHelper.Pi) * num1225;
							npc.netUpdate = true;
						}
					}
				}

				npc.ai[1] += 1f;
				if (npc.ai[1] >= (revenge ? 235f : 315f))
					npc.ai[0] = -1f;
			}
        }

		private void DespawnSpecificProjectiles()
		{
			for (int x = 0; x < Main.maxProjectiles; x++)
			{
				Projectile projectile = Main.projectile[x];
				if (projectile.active)
				{
					if (projectile.type == ModContent.ProjectileType<HolyFire2>() || projectile.type == ModContent.ProjectileType<HolyFlare>())
						projectile.Kill();
					else if (projectile.type == ModContent.ProjectileType<HolyBlast>() || projectile.type == ModContent.ProjectileType<HolyFire>())
						projectile.active = false;
				}
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

			CalamityGlobalTownNPC.SetNewShopVariable(new int[] { ModContent.NPCType<THIEF>() }, CalamityWorld.downedProvidence);

			// Accessories clientside only in Expert. Both drop if she is defeated at night.
			DropHelper.DropItemCondition(npc, ModContent.ItemType<ElysianWings>(), Main.expertMode, biomeType != 2 || !hasTakenDaytimeDamage);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<ElysianAegis>(), Main.expertMode, biomeType == 2 || !hasTakenDaytimeDamage);

			// Drops pre-scal, cannot be sold, does nothing aka purely vanity. Requires at least expert for consistency with other post scal dev items.
			bool shouldDrop = challenge/* || (Main.expertMode && Main.rand.NextBool(CalamityWorld.downedSCal ? 10 : 200))*/;
			DropHelper.DropItemCondition(npc, ModContent.ItemType<ProfanedSoulCrystal>(), true, shouldDrop);

			// Special drop for defeating her at night
			DropHelper.DropItemCondition(npc, ModContent.ItemType<ProfanedMoonlightDye>(), true, !hasTakenDaytimeDamage, 3, 4);

			// All other drops are contained in the bag, so they only drop directly on Normal
			if (!Main.expertMode)
            {
                // Materials
                DropHelper.DropItemSpray(npc, ModContent.ItemType<UnholyEssence>(), 20, 30);
                DropHelper.DropItemSpray(npc, ModContent.ItemType<DivineGeode>(), 15, 20);

                // Weapons
				DropHelper.DropWeaponSet(npc, 4,
					ModContent.ItemType<HolyCollider>(),
					ModContent.ItemType<SolarFlare>(),
					ModContent.ItemType<TelluricGlare>(),
					ModContent.ItemType<BlissfulBombardier>(),
					ModContent.ItemType<PurgeGuzzler>(),
					ModContent.ItemType<MoltenAmputator>(),
					ModContent.ItemType<DazzlingStabberStaff>());

                // Equipment
                DropHelper.DropItemChance(npc, ModContent.ItemType<SamuraiBadge>(), 40);

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
            int tileCenterX = (int)npc.Center.X / 16;
            int tileCenterY = (int)npc.Center.Y / 16;
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
			bool nightTime = !Main.dayTime;

			string baseTextureString = "CalamityMod/NPCs/Providence/";
			string baseGlowTextureString = baseTextureString + "Glowmasks/";

			string getTextureString = baseTextureString + "Providence";
			string getTextureGlowString = baseGlowTextureString + "ProvidenceGlow";
			string getTextureGlow2String = baseGlowTextureString + "ProvidenceGlow2";

			if (npc.ai[0] == 2f || npc.ai[0] == 5f)
            {
				if (!useDefenseFrames)
				{
					getTextureString = baseTextureString + "ProvidenceDefense";
					getTextureGlowString = baseGlowTextureString + "ProvidenceDefenseGlow";
					getTextureGlow2String = baseGlowTextureString + "ProvidenceDefenseGlow2";
				}
				else
				{
					getTextureString = baseTextureString + "ProvidenceDefenseAlt";
					getTextureGlowString = baseGlowTextureString + "ProvidenceDefenseAltGlow";
					getTextureGlow2String = baseGlowTextureString + "ProvidenceDefenseAltGlow2";
				}
            }
            else
            {
				if (frameUsed == 0)
				{
					getTextureGlowString = baseGlowTextureString + "ProvidenceGlow";
					getTextureGlow2String = baseGlowTextureString + "ProvidenceGlow2";
				}
				else if (frameUsed == 1)
				{
					getTextureString = baseTextureString + "ProvidenceAlt";
					getTextureGlowString = baseGlowTextureString + "ProvidenceAltGlow";
					getTextureGlow2String = baseGlowTextureString + "ProvidenceAltGlow2";
				}
				else if (frameUsed == 2)
				{
					getTextureString = baseTextureString + "ProvidenceAttack";
					getTextureGlowString = baseGlowTextureString + "ProvidenceAttackGlow";
					getTextureGlow2String = baseGlowTextureString + "ProvidenceAttackGlow2";
				}
				else
				{
					getTextureString = baseTextureString + "ProvidenceAttackAlt";
					getTextureGlowString = baseGlowTextureString + "ProvidenceAttackAltGlow";
					getTextureGlow2String = baseGlowTextureString + "ProvidenceAttackAltGlow2";
				}
            }

			if (nightTime)
			{
				getTextureString += "Night";
				getTextureGlowString += "Night";
				getTextureGlow2String += "Night";
			}

			Texture2D texture = ModContent.GetTexture(getTextureString);
			Texture2D textureGlow = ModContent.GetTexture(getTextureGlowString);
			Texture2D textureGlow2 = ModContent.GetTexture(getTextureGlow2String);

			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Vector2 vector11 = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2);
			Color color36 = Color.White;
			float amount9 = 0.5f;
			int num153 = 5;

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num155 = 1; num155 < num153; num155 += 2)
				{
					Color color38 = lightColor;
					color38 = Color.Lerp(color38, color36, amount9);
					color38 = npc.GetAlpha(color38);
					color38 *= (num153 - num155) / 15f;
					Vector2 vector41 = npc.oldPos[num155] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
					vector41 -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
					vector41 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
					spriteBatch.Draw(texture, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
			spriteBatch.Draw(texture, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			Color color37 = Color.Lerp(Color.White, Main.dayTime ? Color.Yellow : Color.Cyan, 0.5f) * npc.Opacity;
			Color color42 = Color.Lerp(Color.White, Main.dayTime ? Color.Violet : Color.BlueViolet, 0.5f) * npc.Opacity;

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num163 = 1; num163 < num153; num163++)
				{
					Color color41 = color37;
					color41 = Color.Lerp(color41, color36, amount9);
					color41 = npc.GetAlpha(color41);
					color41 *= (num153 - num163) / 15f;
					Vector2 vector44 = npc.oldPos[num163] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
					vector44 -= new Vector2(textureGlow.Width, textureGlow.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
					vector44 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
					spriteBatch.Draw(textureGlow, vector44, npc.frame, color41, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

					Color color43 = color42;
					color43 = Color.Lerp(color43, color36, amount9);
					color43 = npc.GetAlpha(color43);
					color43 *= (num153 - num163) / 15f;
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
                    if (npc.frameCounter > 8.0)
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
                    if (npc.frameCounter > 8.0)
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
                if (npc.frameCounter > (npc.Calamity().newAI[3] < 180f ? 8.0 : 5.0))
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
			if (!hasTakenDaytimeDamage)
			{
				if (Main.dayTime)
				{
					hasTakenDaytimeDamage = true;

					if (Main.netMode != NetmodeID.SinglePlayer)
					{
						var netMessage = mod.GetPacket();
						netMessage.Write((byte)CalamityModMessageType.ProvidenceDyeConditionSync);
						netMessage.Write((byte)npc.whoAmI);
						netMessage.Write(hasTakenDaytimeDamage);
						netMessage.Send();
					}
				}
			}

			if (challenge)
			{
				List<int> exceptionList = new List<int>()
				{
					ModContent.ProjectileType<GoldenGunProj>(),
					ModContent.ProjectileType<MiniGuardianDefense>(),
					ModContent.ProjectileType<MiniGuardianAttack>(),
					ModContent.ProjectileType<SilvaCrystalExplosion>(),
					ModContent.ProjectileType<GhostlyMine>()
				};

				bool allowedClass = projectile.IsSummon() || (!projectile.melee && !projectile.ranged && !projectile.magic && !projectile.thrown && !projectile.Calamity().rogue);
				bool allowedDamage = allowedClass && damage <= (npc.lifeMax * 0.001f); //0.1% max hp
				bool allowedBabs = Main.player[projectile.owner].Calamity().pArtifact && !Main.player[projectile.owner].Calamity().profanedCrystalBuffs;

				if ((exceptionList.TrueForAll(x => projectile.type != x) && !allowedDamage) || !allowedBabs)
				{
					challenge = false;

					if (Main.netMode != NetmodeID.SinglePlayer)
					{
						var netMessage = mod.GetPacket();
						netMessage.Write((byte)CalamityModMessageType.PSCChallengeSync);
						netMessage.Write((byte)npc.whoAmI);
						netMessage.Write(challenge);
						netMessage.Send();
					}
				}
			}
        }

        public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
		{
			if (!hasTakenDaytimeDamage)
			{
				if (Main.dayTime)
				{
					hasTakenDaytimeDamage = true;

					if (Main.netMode != NetmodeID.SinglePlayer)
					{
						var netMessage = mod.GetPacket();
						netMessage.Write((byte)CalamityModMessageType.ProvidenceDyeConditionSync);
						netMessage.Write((byte)npc.whoAmI);
						netMessage.Write(hasTakenDaytimeDamage);
						netMessage.Send();
					}
				}
			}

			if (challenge)
			{
				challenge = false;

				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					var netMessage = mod.GetPacket();
					netMessage.Write((byte)CalamityModMessageType.PSCChallengeSync);
					netMessage.Write((byte)npc.whoAmI);
					netMessage.Write(challenge);
					netMessage.Send();
				}
			}
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if(npc.soundDelay == 0)
            {
                npc.soundDelay = 8;
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/ProvidenceHurt"), npc.Center);
            }

			int dustType = Main.dayTime ? (int)CalamityDusts.ProfanedFire : (int)CalamityDusts.Nightwither;
			for (int k = 0; k < 15; k++)
                Dust.NewDust(npc.position, npc.width, npc.height, dustType, hitDirection, -1f, 0, default, 1f);

            if (npc.life <= 0)
            {
                float randomSpread = Main.rand.Next(-50, 50) / 100;
                Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/Providence"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/Providence2"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/Providence3"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/Providence4"), 1f);
                npc.position = npc.Center;
                npc.width = 400;
                npc.height = 350;
				npc.position -= npc.Size * 0.5f;
                for (int d = 0; d < 60; d++)
                {
                    int fire = Dust.NewDust(npc.position, npc.width, npc.height, dustType, 0f, 0f, 100, default, 2f);
                    Main.dust[fire].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[fire].scale = 0.5f;
                        Main.dust[fire].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int d = 0; d < 90; d++)
                {
                    int fire = Dust.NewDust(npc.position, npc.width, npc.height, dustType, 0f, 0f, 100, default, 3f);
                    Main.dust[fire].noGravity = true;
                    Main.dust[fire].velocity *= 5f;
                    fire = Dust.NewDust(npc.position, npc.width, npc.height, dustType, 0f, 0f, 100, default, 2f);
                    Main.dust[fire].velocity *= 2f;
                }
            }
        }
    }
}
