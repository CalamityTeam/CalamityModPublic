using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Items.Armor;
using CalamityMod.Projectiles;
using CalamityMod.Projectiles.Healing;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.CalPlayer
{
	public class CalamityPlayerOnHit
	{
		#region Item
		public static void ItemOnHit(Player player, Mod mod, Item item, int damage, Vector2 position, bool crit, bool npcCheck)
		{
			CalamityPlayer modPlayer = player.Calamity();

            if (!item.melee && player.meleeEnchant == 7)
                Projectile.NewProjectile(position, player.velocity, ProjectileID.ConfettiMelee, 0, 0f, player.whoAmI);

			if (modPlayer.reaverDefense)
				player.lifeRegenTime += 1;

			if (modPlayer.desertProwler && crit && item.ranged)
			{
				if (player.ownedProjectileCounts[ProjectileType<DesertMark>()] < 1 && player.ownedProjectileCounts[ProjectileType<DesertTornado>()] < 1)
				{
					if (Main.rand.NextBool(15))
					{
						if (player.whoAmI == Main.myPlayer)
						{
							Projectile.NewProjectile(position, Vector2.Zero, ProjectileType<DesertMark>(), CalamityUtils.DamageSoftCap(damage * player.RangedDamage(), 50), item.knockBack, player.whoAmI);
						}
					}
				}
			}

			if (npcCheck)
			{
				if (item.melee && modPlayer.aBulwarkRare && modPlayer.aBulwarkRareTimer == 0)
				{
					modPlayer.aBulwarkRareTimer = 10;
					for (int n = 0; n < 3; n++)
						CalamityUtils.ProjectileRain(player.Center, 400f, 100f, 500f, 800f, 29f, ProjectileType<AstralStar>(), (int)(320 * player.AverageDamage()), 5f, player.whoAmI);
				}
				if (modPlayer.unstablePrism && crit && player.ownedProjectileCounts[ProjectileType<UnstableSpark>()] < 5)
				{
					for (int s = 0; s < 3; s++)
					{
						Vector2 velocity = CalamityUtils.RandomVelocity(50f, 30f, 60f);
						Projectile.NewProjectile(position, velocity, ProjectileType<UnstableSpark>(), (int)(15 * player.AverageDamage()), 0f, player.whoAmI);
					}
				}
                if (modPlayer.astralStarRain && crit && modPlayer.astralStarRainCooldown <= 0)
                {
                    modPlayer.astralStarRainCooldown = 60;
                    for (int n = 0; n < 3; n++)
                    {
						int projectileType = Utils.SelectRandom(Main.rand, new int[]
						{
							ProjectileType<AstralStar>(),
							ProjectileID.HallowStar,
							ProjectileType<FallenStarProj>()
						});
						Projectile star = CalamityUtils.ProjectileRain(position, 400f, 100f, 500f, 800f, 12f, projectileType, (int)(120 * player.AverageDamage()), 5f, player.whoAmI);
						if (star.whoAmI.WithinBounds(Main.maxProjectiles))
							star.Calamity().forceTypeless = true;
                    }
                }
			}

			if (item.melee)
			{
				modPlayer.titanBoost = 600;
				if (npcCheck)
				{
					if (modPlayer.ataxiaGeyser && player.ownedProjectileCounts[ProjectileType<ChaosGeyser>()] < 3)
					{
						Projectile.NewProjectile(position, Vector2.Zero, ProjectileType<ChaosGeyser>(), CalamityUtils.DamageSoftCap(damage * 0.15, 45), 2f, player.whoAmI, 0f, 0f);
					}
					if (modPlayer.soaring)
					{
						double useTimeMultiplier = 0.85 + (item.useTime * item.useAnimation / 3600D); //28 * 28 = 784 is average so that equals 784 / 3600 = 0.217777 + 1 = 21.7% boost
						double wingTimeFraction = player.wingTimeMax / 20D;
						double meleeStatMultiplier = (double)(player.meleeDamage * (float)(player.meleeCrit / 10D));

						if (player.wingTime < player.wingTimeMax)
							player.wingTime += (int)(useTimeMultiplier * (wingTimeFraction + meleeStatMultiplier));

						if (player.wingTime > player.wingTimeMax)
							player.wingTime = player.wingTimeMax;
					}
					if (modPlayer.bloodflareMelee && item.melee)
					{
						if (modPlayer.bloodflareMeleeHits < 15 && !modPlayer.bloodflareFrenzy && !modPlayer.bloodFrenzyCooldown)
						{
							modPlayer.bloodflareMeleeHits++;
						}
					}
				}
			}
		}
		#endregion

		#region Proj On Hit
		public static void ProjOnHit(Player player, Mod mod, Projectile proj, Vector2 position, bool crit, bool npcCheck)
		{
			CalamityPlayer modPlayer = player.Calamity();
			CalamityGlobalProjectile modProj = proj.Calamity();
			bool hasClass = proj.melee || proj.ranged || proj.magic || proj.IsSummon() || modProj.rogue;

			//flask of party affects all types of weapons, !proj.melee is to prevent double flask effects
			if (!proj.melee && player.meleeEnchant == 7)
				Projectile.NewProjectile(position, proj.velocity, ProjectileID.ConfettiMelee, 0, 0f, proj.owner);

			if (modPlayer.alchFlask && player.ownedProjectileCounts[ProjectileType<PlagueSeeker>()] < 3 && hasClass)
			{
				Projectile projectile = CalamityGlobalProjectile.SpawnOrb(proj, (int)(30 * player.AverageDamage()), ProjectileType<PlagueSeeker>(), 400f, 12f);
				if (projectile.whoAmI.WithinBounds(Main.maxProjectiles))
					Main.projectile[projectile.whoAmI].Calamity().forceTypeless = true;
			}

			if (modPlayer.theBee && player.statLife >= player.statLifeMax2)
				Main.PlaySound(SoundID.Item110, proj.Center);

			if (modPlayer.reaverDefense)
				player.lifeRegenTime += 1;

			if (npcCheck)
			{
                if (modPlayer.unstablePrism && crit && player.ownedProjectileCounts[ProjectileType<UnstableSpark>()] < 5)
                {
                    for (int s = 0; s < 3; s++)
                    {
						Vector2 velocity = CalamityUtils.RandomVelocity(50f, 30f, 60f);
                        Projectile.NewProjectile(position, velocity, ProjectileType<UnstableSpark>(), (int)(15 * player.AverageDamage()), 0f, player.whoAmI);
                    }
                }

                if (modPlayer.astralStarRain && crit && modPlayer.astralStarRainCooldown <= 0)
                {
                    modPlayer.astralStarRainCooldown = 60;
                    for (int n = 0; n < 3; n++)
                    {
						int projectileType = Utils.SelectRandom(Main.rand, new int[]
						{
							ProjectileType<AstralStar>(),
							ProjectileID.HallowStar,
							ProjectileType<FallenStarProj>()
						});
						Projectile star = CalamityUtils.ProjectileRain(position, 400f, 100f, 500f, 800f, 25f, projectileType, (int)(120 * player.AverageDamage()), 5f, player.whoAmI);
						if (star.whoAmI.WithinBounds(Main.maxProjectiles))
							star.Calamity().forceTypeless = true;
                    }
                }
			}

			if (proj.melee)
				MeleeOnHit(player, modPlayer, mod, proj, modProj, position, crit, npcCheck);
			if (proj.ranged)
				RangedOnHit(player, modPlayer, mod, proj, modProj, position, crit, npcCheck);
			if (proj.magic)
				MagicOnHit(player, modPlayer, mod, proj, modProj, position, crit, npcCheck);
			if (proj.IsSummon())
				SummonOnHit(player, modPlayer, mod, proj, modProj, position, crit, npcCheck);
			if (modProj.rogue)
				RogueOnHit(player, modPlayer, mod, proj, modProj, position, crit, npcCheck);
		}

		#region Melee
		private static void MeleeOnHit(Player player, CalamityPlayer modPlayer, Mod mod, Projectile proj, CalamityGlobalProjectile modProj, Vector2 position, bool crit, bool npcCheck)
		{
            Item heldItem = player.ActiveItem();

            if (modProj.trueMelee)
            {
				modPlayer.titanBoost = 600;
				if (modPlayer.soaring)
				{
					double useTimeMultiplier = 0.85 + (heldItem.useTime * heldItem.useAnimation / 3600D); //28 * 28 = 784 is average so that equals 784 / 3600 = 0.217777 + 1 = 21.7% boost
					double wingTimeFraction = player.wingTimeMax / 20D;
					double meleeStatMultiplier = player.meleeDamage * (float)(player.meleeCrit / 10D);

					if (player.wingTime < player.wingTimeMax)
						player.wingTime += (int)(useTimeMultiplier * (wingTimeFraction + meleeStatMultiplier));

					if (player.wingTime > player.wingTimeMax)
						player.wingTime = player.wingTimeMax;
				}
				if (modPlayer.aBulwarkRare && modPlayer.aBulwarkRareTimer == 0)
				{
					modPlayer.aBulwarkRareTimer = 10;
					for (int n = 0; n < 3; n++)
						CalamityUtils.ProjectileRain(player.Center, 400f, 100f, 500f, 800f, 29f, ProjectileType<AstralStar>(), (int)(320 * player.AverageDamage()), 5f, player.whoAmI);
				}
            }

			if (npcCheck)
			{
				if (modPlayer.ataxiaGeyser && player.ownedProjectileCounts[ProjectileType<ChaosGeyser>()] < 3)
				{
					Projectile.NewProjectile(proj.Center, Vector2.Zero, ProjectileType<ChaosGeyser>(), CalamityUtils.DamageSoftCap(proj.damage * 0.15, 35), 0f, player.whoAmI, 0f, 0f);
				}
                if (modPlayer.bloodflareMelee && modProj.trueMelee)
                {
                    if (modPlayer.bloodflareMeleeHits < 15 && !modPlayer.bloodflareFrenzy && !modPlayer.bloodFrenzyCooldown)
                    {
                        modPlayer.bloodflareMeleeHits++;
                    }
                }
			}
		}
		#endregion

		#region Ranged
		private static void RangedOnHit(Player player, CalamityPlayer modPlayer, Mod mod, Projectile proj, CalamityGlobalProjectile modProj, Vector2 position, bool crit, bool npcCheck)
		{
			if (modPlayer.desertProwler && crit)
			{
				if (player.ownedProjectileCounts[ProjectileType<DesertMark>()] < 1 && player.ownedProjectileCounts[ProjectileType<DesertTornado>()] < 1)
				{
					if (Main.rand.NextBool(15))
					{
						if (player.whoAmI == Main.myPlayer)
						{
							Projectile.NewProjectile(position, Vector2.Zero, ProjectileType<DesertMark>(), CalamityUtils.DamageSoftCap(proj.damage * player.RangedDamage(), 50), proj.knockBack, player.whoAmI);
						}
					}
				}
			}
			if (modPlayer.tarraRanged && Main.rand.Next(0, 100) >= 88 && player.ownedProjectileCounts[ProjectileType<TarraEnergy>()] < 5 && (proj.timeLeft <= 2 || proj.penetrate <= 1))
			{
				int projAmt = Main.rand.Next(2, 4);
				for (int projCount = 0; projCount < projAmt; projCount++)
				{
					Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
					Projectile.NewProjectile(proj.Center, velocity, ProjectileType<TarraEnergy>(), CalamityUtils.DamageSoftCap(proj.damage * 0.33, 65), 0f, proj.owner);
				}
			}
			if (npcCheck)
			{
                if (modPlayer.tarraRanged && crit && proj.ranged)
                {
                    int leafAmt = Main.rand.Next(2, 4);
                    for (int l = 0; l < leafAmt; l++)
                    {
						Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                        int FUCKYOU = Projectile.NewProjectile(position, velocity, ProjectileID.Leaf, CalamityUtils.DamageSoftCap(proj.damage * 0.25, 60), 0f, player.whoAmI);
						if (FUCKYOU.WithinBounds(Main.maxProjectiles))
						{
							Main.projectile[FUCKYOU].Calamity().forceTypeless = true;
							Main.projectile[FUCKYOU].netUpdate = true;
						}
                    }
                }
                if (proj.type == ProjectileType<PolarStar>())
                {
                    modPlayer.polarisBoostCounter += 1;
                }
			}
		}
		#endregion

		#region Magic
		private static void MagicOnHit(Player player, CalamityPlayer modPlayer, Mod mod, Projectile proj, CalamityGlobalProjectile modProj, Vector2 position, bool crit, bool npcCheck)
		{
			if (modPlayer.ataxiaMage && modPlayer.ataxiaDmg <= 0)
			{
				int projDamage = CalamityUtils.DamageSoftCap(proj.damage * 0.625, 100);
				CalamityGlobalProjectile.SpawnOrb(proj, projDamage, ProjectileType<AtaxiaOrb>(), 800f, 20f);
				int cooldown = (int)(projDamage * 0.5);
				modPlayer.ataxiaDmg += cooldown;
			}
            if (modPlayer.tarraMage && crit)
            {
                modPlayer.tarraCrits++;
            }
			if (npcCheck)
			{
                if (modPlayer.bloodflareMage && modPlayer.bloodflareMageCooldown <= 0 && crit)
                {
                    modPlayer.bloodflareMageCooldown = 120;
                    for (int i = 0; i < 3; i++)
                    {
						Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                        int fire = Projectile.NewProjectile(position, velocity, ProjectileID.BallofFire, CalamityUtils.DamageSoftCap(proj.damage * 0.5, 120), 0f, player.whoAmI);
						if (fire.WithinBounds(Main.maxProjectiles))
						{
							Main.projectile[fire].Calamity().forceTypeless = true;
							Main.projectile[fire].netUpdate = true;
						}
                    }
                }
			}
			if (modPlayer.silvaMage && proj.penetrate == 1 && Main.rand.Next(0, 100) >= 97)
			{
				Main.PlaySound(SoundID.Zombie, (int)proj.position.X, (int)proj.position.Y, 103);
				CalamityGlobalProjectile.ExpandHitboxBy(proj, 96);
				for (int d = 0; d < 3; d++)
				{
					Dust.NewDust(proj.position, proj.width, proj.height, 157, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1.5f);
				}
				for (int d = 0; d < 30; d++)
				{
					int explode = Dust.NewDust(proj.position, proj.width, proj.height, 157, 0f, 0f, 0, new Color(Main.DiscoR, 203, 103), 2.5f);
					Main.dust[explode].noGravity = true;
					Main.dust[explode].velocity *= 3f;
					explode = Dust.NewDust(proj.position, proj.width, proj.height, 157, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1.5f);
					Main.dust[explode].velocity *= 2f;
					Main.dust[explode].noGravity = true;
				}
				proj.damage += CalamityUtils.DamageSoftCap(proj.damage * 2, 720);
				proj.localNPCHitCooldown = 10;
				proj.usesLocalNPCImmunity = true;
				proj.Damage();
			}
		}
		#endregion

		#region Summon
		private static void SummonOnHit(Player player, CalamityPlayer modPlayer, Mod mod, Projectile proj, CalamityGlobalProjectile modProj, Vector2 position, bool crit, bool npcCheck)
		{
			if (npcCheck)
			{
				if (modPlayer.phantomicArtifact)
				{
					int restoreBuff = BuffType<PhantomicRestorationBuff>();
					int empowerBuff = BuffType<PhantomicEmpowermentBuff>();
					int shieldBuff = BuffType<PhantomicArmourBuff>();
					int buffType = Utils.SelectRandom(Main.rand, new int[]
					{
						restoreBuff,
						empowerBuff,
						shieldBuff
					});
					player.AddBuff(buffType, 60);
					if (buffType == restoreBuff)
					{
						if (modPlayer.phantomicHeartRegen == 1000 && player.ownedProjectileCounts[ProjectileType<PhantomicHeart>()] == 0 && Main.rand.NextBool(20))
						{
							Vector2 target = proj.Center;
							target.Y += Main.rand.Next(-50, 50);
							target.X += Main.rand.Next(-50, 50);
							Projectile.NewProjectile(target, Vector2.Zero, ProjectileType<PhantomicHeart>(), 0, 0f, player.whoAmI, 0f);
						}
					}
					else if (buffType == empowerBuff)
					{
						if (player.ownedProjectileCounts[ProjectileType<PhantomicDagger>()] < 3 && Main.rand.NextBool(10))
						{
							int damage = (int)(75 * player.MinionDamage());
							int dagger = Projectile.NewProjectile(proj.position, proj.velocity, ProjectileType<PhantomicDagger>(), damage, 1f, player.whoAmI, 0f);
							if (dagger.WithinBounds(Main.maxProjectiles))
								Main.projectile[dagger].Calamity().forceTypeless = true;
						}
					}
					else
					{
						if (player.ownedProjectileCounts[ProjectileType<PhantomicShield>()] == 0 && modPlayer.phantomicBulwarkCooldown == 0)
							Projectile.NewProjectile(player.position, Vector2.Zero, ProjectileType<PhantomicShield>(), 0, 0f, player.whoAmI, 0f);
					}
				}
				else if (modPlayer.hallowedRune)
				{
					int buffType = Utils.SelectRandom(Main.rand, new int[]
					{
						BuffType<HallowedRuneAtkBuff>(),
						BuffType<HallowedRuneRegenBuff>(),
						BuffType<HallowedRuneDefBuff>()
					});
					player.AddBuff(buffType, 60);
				}
				else if (modPlayer.sGenerator)
				{
					int buffType = Utils.SelectRandom(Main.rand, new int[]
					{
						BuffType<SpiritGeneratorAtkBuff>(),
						BuffType<SpiritGeneratorRegenBuff>(),
						BuffType<SpiritGeneratorDefBuff>()
					});
					player.AddBuff(buffType, 60);
				}
			}

			// Fearmonger set's colossal life regeneration
			if (modPlayer.fearmongerSet)
			{
				modPlayer.fearmongerRegenFrames += 10;
				if (modPlayer.fearmongerRegenFrames > 90)
					modPlayer.fearmongerRegenFrames = 90;
			}

			//Priorities: Nucleogenesis => Starbuster Core => Nuclear Rod => Jelly-Charged Battery
			List<int> summonExceptionList = new List<int>()
			{
				ProjectileType<EnergyOrb>(),
				ProjectileType<IrradiatedAura>(),
				ProjectileType<SummonAstralExplosion>(),
				ProjectileType<ApparatusExplosion>(),
				ProjectileType<HallowedStarSummon>()
			};

			if (summonExceptionList.TrueForAll(x => proj.type != x))
			{
				if (modPlayer.jellyDmg <= 0)
				{
					if (modPlayer.nucleogenesis)
					{
						int projectile = Projectile.NewProjectile(proj.Center, Vector2.Zero, ProjectileType<ApparatusExplosion>(), (int)(60 * player.MinionDamage()), 4f, proj.owner);
						if (projectile.WithinBounds(Main.maxProjectiles))
							Main.projectile[projectile].Calamity().forceTypeless = true;
						modPlayer.jellyDmg = 100f;
					}
					else if (modPlayer.starbusterCore)
					{
						int projectile = Projectile.NewProjectile(proj.Center, Vector2.Zero, ProjectileType<SummonAstralExplosion>(), (int)(40 * player.MinionDamage()), 3.5f, proj.owner);
						if (projectile.WithinBounds(Main.maxProjectiles))
							Main.projectile[projectile].Calamity().forceTypeless = true;
						modPlayer.jellyDmg = 60f;
					}
					else if (modPlayer.nuclearRod)
					{
						int projectile = Projectile.NewProjectile(proj.Center, Vector2.Zero, ProjectileType<IrradiatedAura>(), (int)(20 * player.MinionDamage()), 0f, proj.owner);
						if (projectile.WithinBounds(Main.maxProjectiles))
							Main.projectile[projectile].Calamity().forceTypeless = true;
						modPlayer.jellyDmg = 60f;
					}
					else if (modPlayer.jellyChargedBattery)
					{
						CalamityGlobalProjectile.SpawnOrb(proj, (int)(15 * player.MinionDamage()), ProjectileType<EnergyOrb>(), 800f, 15f);
						modPlayer.jellyDmg = 60f;
					}
				}

				if (modPlayer.hallowedPower)
				{
					if (modPlayer.hallowedRuneCooldown <= 0)
					{
						modPlayer.hallowedRuneCooldown = 180;
						Vector2 spawnPosition = position - new Vector2(0f, 920f).RotatedByRandom(0.3f);
						float speed = Main.rand.NextFloat(17f, 23f);
						int projectile = Projectile.NewProjectile(spawnPosition, Vector2.Normalize(position - spawnPosition) * speed, ProjectileType<HallowedStarSummon>(), (int)(30 * player.MinionDamage()), 3f, proj.owner);
						if (projectile.WithinBounds(Main.maxProjectiles))
							Main.projectile[projectile].Calamity().forceTypeless = true;
					}
				}
			}
		}
		#endregion

		#region Rogue
		private static void RogueOnHit(Player player, CalamityPlayer modPlayer, Mod mod, Projectile proj, CalamityGlobalProjectile modProj, Vector2 position, bool crit, bool npcCheck)
		{
			if (modProj.stealthStrike && modPlayer.dragonScales && CalamityUtils.CountProjectiles(ProjectileType<InfernadoFriendly>()) < 1)
			{
				int projTileX = (int)(proj.Center.X / 16f);
				int projTileY = (int)(proj.Center.Y / 16f);
				int distance = 100;
				if (projTileX < 10)
				{
					projTileX = 10;
				}
				if (projTileX > Main.maxTilesX - 10)
				{
					projTileX = Main.maxTilesX - 10;
				}
				if (projTileY < 10)
				{
					projTileY = 10;
				}
				if (projTileY > Main.maxTilesY - distance - 10)
				{
					projTileY = Main.maxTilesY - distance - 10;
				}
				for (int x = projTileX; x < projTileX + distance; x++)
				{
					Tile tile = Main.tile[projTileX, projTileY];
					if (tile.active() && (Main.tileSolid[tile.type] || tile.liquid != 0))
					{
						projTileX = x;
						break;
					}
				}
				int projectileIndex = Projectile.NewProjectile(projTileX * 16 + 8, projTileY * 16 - 24, 0f, 0f, ProjectileType<InfernadoFriendly>(), (int)(400 * player.RogueDamage()), 15f, Main.myPlayer, 16f, 16f);
				if (projectileIndex.WithinBounds(Main.maxProjectiles))
				{
					Main.projectile[projectileIndex].Calamity().forceTypeless = true;
					Main.projectile[projectileIndex].netUpdate = true;
					Main.projectile[projectileIndex].localNPCHitCooldown = 10;
				}
			}
            if (modPlayer.tarraThrowing && !modPlayer.tarragonImmunity && !modPlayer.tarragonImmunityCooldown && modPlayer.tarraThrowingCrits < 25 && crit)
            {
                modPlayer.tarraThrowingCrits++;
            }
			if (modPlayer.xerocSet && modPlayer.xerocDmg <= 0 && player.ownedProjectileCounts[ProjectileType<XerocFire>()] < 3 && player.ownedProjectileCounts[ProjectileType<XerocBlast>()] < 3)
			{
				switch (Main.rand.Next(5))
				{
					case 0:
						int projDamage = CalamityUtils.DamageSoftCap(proj.damage * 0.8, 120);
						CalamityGlobalProjectile.SpawnOrb(proj, projDamage, ProjectileType<XerocStar>(), 800f, Main.rand.Next(15, 30));
						modPlayer.xerocDmg += (int)(projDamage * 0.5);
						break;

					case 1:
						int projDamage2 = CalamityUtils.DamageSoftCap(proj.damage * 0.625, 100);
						CalamityGlobalProjectile.SpawnOrb(proj, projDamage2, ProjectileType<XerocOrb>(), 800f, 30f);
						modPlayer.xerocDmg += (int)(projDamage2 * 0.5);
						break;

					case 2:
						Projectile.NewProjectile(proj.Center, Vector2.Zero, ProjectileType<XerocFire>(), CalamityUtils.DamageSoftCap(proj.damage * 0.15, 30), 0f, proj.owner, 0f, 0f);
						break;

					case 3:
						Projectile.NewProjectile(proj.Center, Vector2.Zero, ProjectileType<XerocBlast>(), CalamityUtils.DamageSoftCap(proj.damage * 0.2, 40), 0f, proj.owner, 0f, 0f);
						break;

					case 4:
						int projDamage3 = CalamityUtils.DamageSoftCap(proj.damage * 0.6, 90);
						CalamityGlobalProjectile.SpawnOrb(proj, projDamage3, ProjectileType<XerocBubble>(), 800f, 15f);
						modPlayer.xerocDmg += (int)(projDamage3 * 0.5);
						break;

					default:
						break;
				}
			}

			if (modPlayer.featherCrown && modProj.stealthStrike && modPlayer.featherCrownCooldown <= 0 && modProj.stealthStrikeHitCount < 5)
			{
				for (int i = 0; i < 3; i++)
				{
					Vector2 source = new Vector2(position.X + Main.rand.Next(-201, 201), Main.screenPosition.Y - 600f - Main.rand.Next(50));
					float speedX = (position.X - source.X) / 30f;
					float speedY = (position.Y - source.Y) * 8;
					Vector2 velocity = new Vector2(speedX, speedY);
					int feather = Projectile.NewProjectile(source, velocity, ProjectileType<StickyFeather>(), (int)(15 * player.RogueDamage()), 3f, proj.owner);
					if (feather.WithinBounds(Main.maxProjectiles))
						Main.projectile[feather].Calamity().forceTypeless = true;
					modPlayer.featherCrownCooldown = 15;
				}
			}

			if (modPlayer.moonCrown && modProj.stealthStrike && modPlayer.moonCrownCooldown <= 0 && modProj.stealthStrikeHitCount < 5)
			{
				for (int i = 0; i < 3; i++)
				{
					Vector2 source = new Vector2(position.X + Main.rand.Next(-201, 201), Main.screenPosition.Y - 600f - Main.rand.Next(50));
					Vector2 velocity = (position - source) / 10f;
					int flare = Projectile.NewProjectile(source, velocity, ProjectileID.LunarFlare, (int)(60 * player.RogueDamage()), 3, proj.owner);
					if (flare.WithinBounds(Main.maxProjectiles))
						Main.projectile[flare].Calamity().forceTypeless = true;
					modPlayer.moonCrownCooldown = 15;
				}
			}

			if (modPlayer.nanotech && modProj.stealthStrike && modPlayer.nanoFlareCooldown <= 0 && modProj.stealthStrikeHitCount < 5)
			{
				for (int i = 0; i < 3; i++)
				{
					Vector2 source = new Vector2(position.X + Main.rand.Next(-201, 201), Main.screenPosition.Y - 600f - Main.rand.Next(50));
					Vector2 velocity = (position - source) / 40f;
					Projectile.NewProjectile(source, velocity, ProjectileType<NanoFlare>(), (int)(120 * player.RogueDamage()), 3f, proj.owner);
					modPlayer.nanoFlareCooldown = 15;
				}
			}

			if (modPlayer.forbiddenCirclet && modProj.stealthStrike && modPlayer.forbiddenCooldown <= 0 && modProj.stealthStrikeHitCount < 5)
			{
				for (int index2 = 0; index2 < 6; index2++)
				{
					float xVector = Main.rand.Next(-35, 36) * 0.02f;
					float yVector = Main.rand.Next(-35, 36) * 0.02f;
					xVector *= 10f;
					yVector *= 10f;
					int eater = Projectile.NewProjectile(proj.Center.X, proj.Center.Y, xVector, yVector, ProjectileType<ForbiddenCircletEater>(), (int)(40 * player.RogueDamage()), proj.knockBack, proj.owner);
					if (eater.WithinBounds(Main.maxProjectiles))
						Main.projectile[eater].Calamity().forceTypeless = true;
					modPlayer.forbiddenCooldown = 15;
				}
			}

			if (modPlayer.titanHeartSet && modProj.stealthStrike && modPlayer.titanCooldown <= 0 && modProj.stealthStrikeHitCount < 5)
			{
				Projectile.NewProjectile(proj.Center, Vector2.Zero, ProjectileType<SabatonBoom>(), (int)(50 * player.RogueDamage()), proj.knockBack, proj.owner, 1f, 0f);
				Main.PlaySound(SoundID.Item14, proj.Center);
				for (int dustexplode = 0; dustexplode < 360; dustexplode++)
				{
					Vector2 dustd = new Vector2(17f, 17f).RotatedBy(MathHelper.ToRadians(dustexplode));
					int d = Dust.NewDust(proj.Center, proj.width, proj.height, Main.rand.NextBool(2) ? DustType<AstralBlue>() : DustType<AstralOrange>(), dustd.X, dustd.Y, 100, default, 1f);
					Main.dust[d].noGravity = true;
					Main.dust[d].position = proj.Center;
					Main.dust[d].velocity *= 0.1f;
				}
				modPlayer.titanCooldown = 15;
			}

			if (modPlayer.corrosiveSpine && modProj.stealthStrikeHitCount < 5)
			{
				for (int i = 0; i < 3; i++)
				{
					if (Main.rand.NextBool(2))
					{
						int type = -1;
						switch (Main.rand.Next(15))
						{
							case 0:
								type = ProjectileType<Corrocloud1>();
								break;
							case 1:
								type = ProjectileType<Corrocloud2>();
								break;
							case 2:
								type = ProjectileType<Corrocloud3>();
								break;
						}
						// Should never happen, but just in case-
						if (type != -1)
						{
							float speed = Main.rand.NextFloat(5f, 11f);
							int cloud = Projectile.NewProjectile(position, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * speed, type, (int)(35 * player.RogueDamage()), proj.knockBack, player.whoAmI);
							if (cloud.WithinBounds(Main.maxProjectiles))
								Main.projectile[cloud].Calamity().forceTypeless = true;
						}
					}
				}
			}

			if (modPlayer.shadow && modPlayer.shadowPotCooldown <= 0 && modProj.stealthStrikeHitCount < 5)
			{
				if (CalamityLists.javelinProjList.Contains(proj.type))
				{
					int randrot = Main.rand.Next(-30, 391);
					Vector2 SoulSpeed = new Vector2(13f, 13f).RotatedBy(MathHelper.ToRadians(randrot));
					int soul = Projectile.NewProjectile(proj.Center, SoulSpeed, ProjectileType<PenumbraSoul>(), (int)(proj.damage * 0.1), 3f, proj.owner, 0f, 0f);
					if (soul.WithinBounds(Main.maxProjectiles))
						Main.projectile[soul].Calamity().forceTypeless = true;
					modPlayer.shadowPotCooldown = 30;
				}
				if (CalamityLists.spikyBallProjList.Contains(proj.type))
				{
					int scythe = Projectile.NewProjectile(proj.Center, Vector2.Zero, ProjectileType<CosmicScythe>(), (int)(proj.damage * 0.05), 3f, proj.owner, 1f, 0f);
					if (scythe.WithinBounds(Main.maxProjectiles))
						Main.projectile[scythe].Calamity().forceTypeless = true;
					Main.projectile[scythe].usesLocalNPCImmunity = true;
					Main.projectile[scythe].localNPCHitCooldown = 10;
					Main.projectile[scythe].penetrate = 2;
					modPlayer.shadowPotCooldown = 30;
				}
				if (CalamityLists.daggerProjList.Contains(proj.type))
				{
					Vector2 shardVelocity = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
					shardVelocity.Normalize();
					shardVelocity *= 5f;
					int shard = Projectile.NewProjectile(proj.Center, shardVelocity, ProjectileType<EquanimityDarkShard>(), (int)(proj.damage * 0.15), 0f, proj.owner);
					if (shard.WithinBounds(Main.maxProjectiles))
						Main.projectile[shard].Calamity().forceTypeless = true;
					Main.projectile[shard].timeLeft = 150;
					modPlayer.shadowPotCooldown = 30;
				}
				if (CalamityLists.boomerangProjList.Contains(proj.type))
				{
					int spiritDamage = (int)(proj.damage * 0.2);
					Projectile ghost = CalamityGlobalProjectile.SpawnOrb(proj, spiritDamage, ProjectileID.SpectreWrath, 800f, 4f);
					if (ghost.whoAmI.WithinBounds(Main.maxProjectiles))
					{
						ghost.Calamity().forceTypeless = true;
						ghost.penetrate = 1;
					}
					modPlayer.shadowPotCooldown = 30;
				}
				if (CalamityLists.flaskBombProjList.Contains(proj.type))
				{
					int blackhole = Projectile.NewProjectile(proj.Center, Vector2.Zero, ProjectileType<ShadowBlackhole>(), (int)(proj.damage * 0.05), 3f, proj.owner, 0f, 0f);
					if (blackhole.WithinBounds(Main.maxProjectiles))
						Main.projectile[blackhole].Calamity().forceTypeless = true;
					Main.projectile[blackhole].Center = proj.Center;
					modPlayer.shadowPotCooldown = 30;
				}
			}

			if (npcCheck)
			{
                if (modPlayer.umbraphileSet && (Main.rand.NextBool(4) || (modProj.stealthStrike && modProj.stealthStrikeHitCount < 5)) && proj.type != ProjectileType<UmbraphileBoom>())
                {
                    int boom = Projectile.NewProjectile(proj.Center, Vector2.Zero, ProjectileType<UmbraphileBoom>(), CalamityUtils.DamageSoftCap(proj.damage * 0.25, 50), 0f, player.whoAmI);
					if (boom.WithinBounds(Main.maxProjectiles))
						Main.projectile[boom].Calamity().forceTypeless = true;
				}
                if (modPlayer.raiderTalisman && modPlayer.raiderStack < 150 && crit && modPlayer.raiderCooldown <= 0)
                {
                    modPlayer.raiderStack++;
                    modPlayer.raiderCooldown = 30;
                }
				if (modPlayer.electricianGlove && modProj.stealthStrike && modProj.stealthStrikeHitCount < 5)
				{
					for (int s = 0; s < 3; s++)
					{
						Vector2 velocity = CalamityUtils.RandomVelocity(50f, 30f, 60f);
						int spark = Projectile.NewProjectile(position, velocity, ProjectileType<Spark>(), (int)(20 * player.RogueDamage()), 0f, player.whoAmI);
						if (spark.WithinBounds(Main.maxProjectiles))
						{
							Main.projectile[spark].Calamity().forceTypeless = true;
							Main.projectile[spark].localNPCHitCooldown = -1;
						}
					}
				}
			}
			modProj.stealthStrikeHitCount++;
		}
		#endregion
		#endregion

		#region Debuffs
		public static void NPCDebuffs(Player player, Mod mod, NPC target, bool melee, bool ranged, bool magic, bool summon, bool rogue, bool proj)
		{
			CalamityPlayer modPlayer = player.Calamity();
            if (melee) //prevents Deep Sea Dumbell from snagging true melee debuff memes
            {
                if (modPlayer.eGauntlet)
                {
					int duration = 90;
                    target.AddBuff(BuffID.CursedInferno, duration / 2, false);
                    target.AddBuff(BuffID.Frostburn, duration, false);
                    target.AddBuff(BuffID.Ichor, duration, false);
                    target.AddBuff(BuffID.Venom, duration, false);
                    target.AddBuff(BuffType<GodSlayerInferno>(), duration, false);
                    target.AddBuff(BuffType<AbyssalFlames>(), duration, false);
                    target.AddBuff(BuffType<HolyFlames>(), duration, false);
                    target.AddBuff(BuffType<Plague>(), duration, false);
                    target.AddBuff(BuffType<BrimstoneFlames>(), duration, false);
                    if (Main.rand.NextBool(5))
                    {
                        target.AddBuff(BuffType<GlacialState>(), duration, false);
                    }
                }
                if (modPlayer.cryogenSoul || modPlayer.frostFlare)
                {
					CalamityUtils.Inflict246DebuffsNPC(target, BuffID.Frostburn);
                }
                if (modPlayer.yInsignia)
                {
					CalamityUtils.Inflict246DebuffsNPC(target, BuffType<HolyFlames>());
                }
                if (modPlayer.ataxiaFire)
                {
					CalamityUtils.Inflict246DebuffsNPC(target, BuffID.OnFire, 4f);
                }
				if (modPlayer.aWeapon)
				{
					CalamityUtils.Inflict246DebuffsNPC(target, BuffType<AbyssalFlames>());
				}

				if (modPlayer.auricSet && modPlayer.godSlayerDamage && Main.rand.NextBool(4) && proj)
					target.AddBuff(BuffType<SilvaStun>(), 20);
			}
            if (modPlayer.armorCrumbling || modPlayer.armorShattering)
            {
                if (melee || rogue)
                {
					CalamityUtils.Inflict246DebuffsNPC(target, BuffType<ArmorCrunch>());
                }
            }
            if (rogue)
            {
				switch (player.meleeEnchant)
				{
					case 1:
						target.AddBuff(BuffID.Venom, 60 * Main.rand.Next(5, 10), false);
						break;
					case 2:
						target.AddBuff(BuffID.CursedInferno, 60 * Main.rand.Next(3, 7), false);
						break;
					case 3:
						target.AddBuff(BuffID.OnFire, 60 * Main.rand.Next(3, 7), false);
						break;
					case 5:
						target.AddBuff(BuffID.Ichor, 60 * Main.rand.Next(10, 20), false);
						break;
					case 6:
						target.AddBuff(BuffID.Confused, 60 * Main.rand.Next(1, 4), false);
						break;
					case 8:
						target.AddBuff(BuffID.Poisoned, 60 * Main.rand.Next(5, 10), false);
						break;
					case 4:
						target.AddBuff(BuffID.Midas, 120, false);
						break;
				}
				if (modPlayer.titanHeartMask)
				{
					target.AddBuff(BuffType<AstralInfectionDebuff>(), 60 * Main.rand.Next(1,6), false); // 1 to 5 seconds
				}
				if (modPlayer.corrosiveSpine)
				{
					target.AddBuff(BuffID.Venom, 240);
				}
            }
			if (summon)
			{
				if (modPlayer.pArtifact && !modPlayer.profanedCrystal)
				{
					target.AddBuff(BuffType<HolyFlames>(), 300);
				}
				if (modPlayer.profanedCrystalBuffs)
				{
					target.AddBuff(Main.dayTime ? BuffType<HolyFlames>() : BuffType<Nightwither>(), 600);
				}

				if (modPlayer.tearMinions)
				{
					target.AddBuff(BuffType<TemporalSadness>(), 60);
				}

				if (modPlayer.shadowMinions)
				{
					target.AddBuff(BuffID.ShadowFlame, 300);
				}

				if (modPlayer.voltaicJelly)
				{
					//100% chance for Star Tainted Generator or Nucleogenesis
					//20% chance for Voltaic Jelly
					if (Main.rand.NextBool(modPlayer.starTaintedGenerator ? 1 : 5))
					{
						target.AddBuff(BuffID.Electrified, 60);
					}
				}

				if (modPlayer.starTaintedGenerator)
				{
					target.AddBuff(BuffType<AstralInfectionDebuff>(), 180);
					target.AddBuff(BuffType<Irradiated>(), 180);
				}
			}
            if (modPlayer.omegaBlueChestplate)
                target.AddBuff(BuffType<CrushDepth>(), 240);
            if (modPlayer.sulfurSet)
                target.AddBuff(BuffID.Poisoned, 120);
            if (modPlayer.abyssalAmulet)
            {
				CalamityUtils.Inflict246DebuffsNPC(target, BuffType<CrushDepth>());
            }
            if (modPlayer.dsSetBonus)
            {
				CalamityUtils.Inflict246DebuffsNPC(target, BuffType<DemonFlames>());
            }
            if (modPlayer.alchFlask)
            {
				CalamityUtils.Inflict246DebuffsNPC(target, BuffType<Plague>());
            }
            if (modPlayer.holyWrath)
            {
                target.AddBuff(BuffType<HolyFlames>(), 600, false);
            }
            if (modPlayer.vexation)
            {
				if ((player.armor[0].type == ItemType<ReaverHelm>() || player.armor[0].type == ItemType<ReaverHeadgear>() ||
					player.armor[0].type == ItemType<ReaverVisage>()) && player.armor[1].type == ItemType<ReaverScaleMail>() &&
					player.armor[2].type == ItemType<ReaverCuisses>())
				{
                    target.AddBuff(BuffID.CursedInferno, 90, false);
                    target.AddBuff(BuffID.Venom, 120, false);
                }
            }
		}

		public static void PvpDebuffs(Player player, Mod mod, Player target, bool melee, bool ranged, bool magic, bool summon, bool rogue, bool proj)
		{
			CalamityPlayer modPlayer = player.Calamity();
            if (melee)
            {
                if (modPlayer.eGauntlet)
                {
					int duration = 90;
                    target.AddBuff(BuffID.CursedInferno, duration / 2, false);
                    target.AddBuff(BuffID.Frostburn, duration, false);
                    target.AddBuff(BuffID.Ichor, duration, false);
                    target.AddBuff(BuffID.Venom, duration, false);
                    target.AddBuff(BuffType<GodSlayerInferno>(), duration, false);
                    target.AddBuff(BuffType<AbyssalFlames>(), duration, false);
                    target.AddBuff(BuffType<HolyFlames>(), duration, false);
                    target.AddBuff(BuffType<Plague>(), duration, false);
                    target.AddBuff(BuffType<BrimstoneFlames>(), duration, false);
                    if (Main.rand.NextBool(5))
                    {
                        target.AddBuff(BuffType<GlacialState>(), duration, false);
                    }
                }
                if (modPlayer.aWeapon)
                {
					CalamityUtils.Inflict246DebuffsPvp(target, BuffType<AbyssalFlames>());
                }
                if (modPlayer.cryogenSoul || modPlayer.frostFlare)
                {
					CalamityUtils.Inflict246DebuffsPvp(target, BuffID.Frostburn);
                }
                if (modPlayer.yInsignia)
                {
					CalamityUtils.Inflict246DebuffsPvp(target, BuffType<HolyFlames>());
                }
                if (modPlayer.ataxiaFire)
                {
					CalamityUtils.Inflict246DebuffsPvp(target, BuffID.OnFire, 4f);
                }
				if (modPlayer.auricSet && modPlayer.godSlayerDamage && Main.rand.NextBool(4) && proj)
					target.AddBuff(BuffType<SilvaStun>(), 20);
			}
            if (modPlayer.armorCrumbling || modPlayer.armorShattering)
            {
                if (melee || rogue)
                {
					CalamityUtils.Inflict246DebuffsPvp(target, BuffType<ArmorCrunch>());
                }
            }
            if (rogue)
            {
				switch (player.meleeEnchant)
				{
					case 1:
						target.AddBuff(BuffID.Venom, 60 * Main.rand.Next(5, 10), false);
						break;
					case 2:
						target.AddBuff(BuffID.CursedInferno, 60 * Main.rand.Next(3, 7), false);
						break;
					case 3:
						target.AddBuff(BuffID.OnFire, 60 * Main.rand.Next(3, 7), false);
						break;
					case 5:
						target.AddBuff(BuffID.Ichor, 60 * Main.rand.Next(10, 20), false);
						break;
					case 6:
						target.AddBuff(BuffID.Confused, 60 * Main.rand.Next(1, 4), false);
						break;
					case 8:
						target.AddBuff(BuffID.Poisoned, 60 * Main.rand.Next(5, 10), false);
						break;
				}
				if (modPlayer.titanHeartMask)
				{
					target.AddBuff(BuffType<AstralInfectionDebuff>(), 60 * Main.rand.Next(1,6), false); // 1 to 5 seconds
				}
				if (modPlayer.corrosiveSpine)
				{
					target.AddBuff(BuffID.Venom, 240);
				}
            }
			if (summon)
			{
				if (modPlayer.pArtifact && !modPlayer.profanedCrystal)
				{
					target.AddBuff(BuffType<HolyFlames>(), 300);
				}
				if (modPlayer.profanedCrystalBuffs)
				{
					target.AddBuff(Main.dayTime ? BuffType<HolyFlames>() : BuffType<Nightwither>(), 600);
				}

				if (modPlayer.tearMinions)
				{
					target.AddBuff(BuffType<TemporalSadness>(), 60);
				}

				if (modPlayer.shadowMinions)
				{
					target.AddBuff(BuffID.ShadowFlame, 300);
				}

				if (modPlayer.voltaicJelly)
				{
					//100% chance for Star Tainted Generator or Nucleogenesis
					//20% chance for Voltaic Jelly
					if (Main.rand.NextBool(modPlayer.starTaintedGenerator ? 1 : 5))
					{
						target.AddBuff(BuffID.Electrified, 60);
					}
				}

				if (modPlayer.starTaintedGenerator)
				{
					target.AddBuff(BuffType<AstralInfectionDebuff>(), 180);
					target.AddBuff(BuffType<Irradiated>(), 180);
				}
			}
            if (modPlayer.omegaBlueChestplate)
                target.AddBuff(BuffType<CrushDepth>(), 240);
            if (modPlayer.sulfurSet)
                target.AddBuff(BuffID.Poisoned, 120);
            if (modPlayer.alchFlask)
            {
				CalamityUtils.Inflict246DebuffsPvp(target, BuffType<Plague>());
            }
			if (modPlayer.abyssalAmulet)
			{
				CalamityUtils.Inflict246DebuffsPvp(target, BuffType<CrushDepth>());
			}
			if (modPlayer.holyWrath)
			{
				target.AddBuff(BuffType<HolyFlames>(), 600, false);
			}
            if (modPlayer.vexation)
            {
				if ((player.armor[0].type == ItemType<ReaverHelm>() || player.armor[0].type == ItemType<ReaverHeadgear>() ||
					player.armor[0].type == ItemType<ReaverVisage>()) && player.armor[1].type == ItemType<ReaverScaleMail>() &&
					player.armor[2].type == ItemType<ReaverCuisses>())
				{
                    target.AddBuff(BuffID.CursedInferno, 90, false);
                    target.AddBuff(BuffID.Venom, 120, false);
                }
            }
		}
		#endregion

		#region Lifesteal
		public static void ProjLifesteal(Player player, Mod mod, NPC target, Projectile proj, int damage, bool crit)
		{
			CalamityPlayer modPlayer = player.Calamity();
			CalamityGlobalProjectile modProj = proj.Calamity();

			// Spectre Damage set and Nebula set work on enemies which are "immune to lifesteal"
			if (!target.canGhostHeal)
			{
				if (player.ghostHurt)
				{
					proj.ghostHurt(damage, target.Center);
				}

				if (player.setNebula && player.nebulaCD == 0 && Main.rand.NextBool(3))
				{
					player.nebulaCD = 30;
					int boosterType = Utils.SelectRandom(Main.rand, new int[]
					{
						ItemID.NebulaPickup1,
						ItemID.NebulaPickup2,
						ItemID.NebulaPickup3
					});
					int nebulaBooster = Item.NewItem(target.Center, target.Size, boosterType, 1, false, 0, false, false);
					Main.item[nebulaBooster].velocity.Y = Main.rand.Next(-20, 1) * 0.2f;
					Main.item[nebulaBooster].velocity.X = Main.rand.Next(10, 31) * 0.2f * proj.direction;
					if (Main.netMode == NetmodeID.MultiplayerClient)
					{
						NetMessage.SendData(MessageID.SyncItem, -1, -1, null, nebulaBooster, 0f, 0f, 0f, 0, 0, 0);
					}
				}
			}

            if (modPlayer.bloodflareSet && !target.SpawnedFromStatue && (target.damage > 0 || target.boss) && !player.moonLeech)
            {
                if ((target.life < target.lifeMax * 0.5) && modPlayer.bloodflareHeartTimer <= 0)
                {
                    modPlayer.bloodflareHeartTimer = 180;
                    DropHelper.DropItem(target, ItemID.Heart);
                }
                else if ((target.life > target.lifeMax * 0.5) && modPlayer.bloodflareManaTimer <= 0)
                {
                    modPlayer.bloodflareManaTimer = 180;
                    DropHelper.DropItem(target, ItemID.Star);
                }
            }

			if (Main.player[Main.myPlayer].lifeSteal > 0f && target.canGhostHeal && !player.moonLeech)
			{
				// Increases the degree to which Spectre Healing set contributes to the lifesteal cap
				if (player.ghostHeal)
				{
					float cooldownMult = 0.1f;
					cooldownMult -= proj.numHits * 0.025f;
					if (cooldownMult < 0f)
						cooldownMult = 0f;

					float cooldown = damage * cooldownMult;
					Main.player[Main.myPlayer].lifeSteal -= cooldown;
				}

				// Increases the degree to which Vampire Knives contribute to the lifesteal cap
				if (proj.type == ProjectileID.VampireKnife)
				{
					float cooldown = damage * 0.075f;
					if (cooldown < 0f)
						cooldown = 0f;

					Main.player[Main.myPlayer].lifeSteal -= cooldown;
				}

				if (modPlayer.vampiricTalisman && modProj.rogue && crit)
				{
					float heal = MathHelper.Clamp(damage * 0.015f, 0f, 6f);
					if ((int)heal > 0)
					{
						CalamityGlobalProjectile.SpawnLifeStealProjectile(proj, player, heal, ProjectileID.VampireHeal, 1200f, 2f);
					}
				}

				if ((modPlayer.bloodyGlove || modPlayer.electricianGlove) && modProj.rogue && modProj.stealthStrike)
				{
					player.statLife += 1;
					player.HealEffect(1);
				}

				if ((target.damage > 5 || target.boss) && !target.SpawnedFromStatue)
				{
					if (modPlayer.bloodflareThrowing && modProj.rogue && crit && Main.rand.NextBool(2))
					{
						float projHitMult = 0.03f;
						projHitMult -= (float)proj.numHits * 0.015f;
						if (projHitMult < 0f)
						{
							projHitMult = 0f;
						}
						float cooldownMult = damage * projHitMult;
						if (cooldownMult < 0f)
						{
							cooldownMult = 0f;
						}
						if (player.lifeSteal > 0f)
						{
							player.statLife += 1;
							player.HealEffect(1);
							player.lifeSteal -= cooldownMult * 2f;
						}
					}
					if (modPlayer.bloodflareMelee && modProj.trueMelee)
					{
						int healAmount = Main.rand.Next(3) + 1;
						player.statLife += healAmount;
						player.HealEffect(healAmount);
					}
				}

				bool otherHealTypes = modPlayer.auricSet || modPlayer.silvaSet || modPlayer.tarraMage || modPlayer.ataxiaMage;

				if (proj.magic && player.ActiveItem().magic)
				{
					if (modPlayer.manaOverloader && otherHealTypes)
					{
						if (Main.rand.NextBool(2))
						{
							float healMult = 0.2f;
							healMult -= proj.numHits * 0.05f;
							float heal = damage * healMult * (player.statMana / (float)player.statManaMax2);

							if (heal > 50)
								heal = 50;

							if (!CalamityGlobalProjectile.CanSpawnLifeStealProjectile(proj, healMult, heal))
								return;

							CalamityGlobalProjectile.SpawnLifeStealProjectile(proj, player, heal, ProjectileType<ManaOverloaderHealOrb>(), 1200f, 2f);
						}
					}
				}

				if (modPlayer.auricSet)
				{
					float healMult = 0.05f;
					healMult -= proj.numHits * 0.025f;
					float heal = damage * healMult;

					if (heal > 50)
						heal = 50;

					if (!CalamityGlobalProjectile.CanSpawnLifeStealProjectile(proj, healMult, heal))
						return;

					CalamityGlobalProjectile.SpawnLifeStealProjectile(proj, player, heal, ProjectileType<AuricOrb>(), 1200f, 3f);
				}
				else if (modPlayer.silvaSet)
				{
					float healMult = 0.03f;
					healMult -= proj.numHits * 0.015f;
					float heal = damage * healMult;

					if (heal > 50)
						heal = 50;

					if (!CalamityGlobalProjectile.CanSpawnLifeStealProjectile(proj, healMult, heal))
						return;

					CalamityGlobalProjectile.SpawnLifeStealProjectile(proj, player, heal, ProjectileType<SilvaOrb>(), 1200f, 3f);
				}
				else if (proj.magic && player.ActiveItem().magic)
				{
					if (modPlayer.manaOverloader)
					{
						float healMult = 0.2f;
						healMult -= proj.numHits * 0.05f;
						float heal = damage * healMult * (player.statMana / (float)player.statManaMax2);

						if (heal > 50)
							heal = 50;

						if (!CalamityGlobalProjectile.CanSpawnLifeStealProjectile(proj, healMult, heal))
							return;

						CalamityGlobalProjectile.SpawnLifeStealProjectile(proj, player, heal, ProjectileType<ManaOverloaderHealOrb>(), 1200f, 2f);
					}
					if (modPlayer.tarraMage)
					{
						if (modPlayer.tarraMageHealCooldown <= 0)
						{
							modPlayer.tarraMageHealCooldown = 90;

							float healMult = 0.1f;
							healMult -= proj.numHits * 0.05f;
							float heal = damage * healMult;

							if (heal > 50)
								heal = 50;

							if (!CalamityGlobalProjectile.CanSpawnLifeStealProjectile(proj, healMult, heal))
								return;

							Main.player[Main.myPlayer].lifeSteal -= heal * 6f;
							int healAmount = (int)heal;
							player.statLife += healAmount;
							player.HealEffect(healAmount);

							if (player.statLife > player.statLifeMax2)
								player.statLife = player.statLifeMax2;
						}
					}
					else if (modPlayer.ataxiaMage)
					{
						float healMult = 0.1f;
						healMult -= proj.numHits * 0.05f;
						float heal = damage * healMult;

						if (heal > 50)
							heal = 50;

						if (!CalamityGlobalProjectile.CanSpawnLifeStealProjectile(proj, healMult, heal))
							return;

						CalamityGlobalProjectile.SpawnLifeStealProjectile(proj, player, heal, ProjectileType<AtaxiaHealOrb>(), 1200f, 2f);
					}
				}
				if (modPlayer.reaverDefense)
				{
					float healMult = 0.2f;
					healMult -= proj.numHits * 0.05f;
					float heal = damage * healMult;

					if (heal > 50)
						heal = 50;
					if (Main.rand.Next(10) > 0)
						heal = 0;

					if (!CalamityGlobalProjectile.CanSpawnLifeStealProjectile(proj, healMult, heal))
						return;

					CalamityGlobalProjectile.SpawnLifeStealProjectile(proj, player, heal, ProjectileType<ReaverHealOrb>(), 1200f, 2f);
				}
				if (modProj.rogue)
				{
					if (modPlayer.xerocSet && modPlayer.xerocDmg <= 0 && player.ownedProjectileCounts[ProjectileType<XerocFire>()] < 3 && player.ownedProjectileCounts[ProjectileType<XerocBlast>()] < 3)
					{
						float healMult = 0.06f;
						healMult -= proj.numHits * 0.015f;
						float heal = damage * healMult;

						if (!CalamityGlobalProjectile.CanSpawnLifeStealProjectile(proj, healMult, heal))
							return;

						CalamityGlobalProjectile.SpawnLifeStealProjectile(proj, player, heal, ProjectileType<XerocHealOrb>(), 1200f, 1.5f);
					}
				}
			}
		}

		public static void ItemLifesteal(Player player, Mod mod, NPC target, Item item, int damage)
		{
			CalamityPlayer modPlayer = player.Calamity();

            if (modPlayer.bloodflareSet && !target.SpawnedFromStatue && (target.damage > 0 || target.boss))
            {
                if ((target.life < target.lifeMax * 0.5) && modPlayer.bloodflareHeartTimer <= 0)
                {
                    modPlayer.bloodflareHeartTimer = 180;
                    DropHelper.DropItem(target, ItemID.Heart);
                }
                else if ((target.life > target.lifeMax * 0.5) && modPlayer.bloodflareManaTimer <= 0)
                {
                    modPlayer.bloodflareManaTimer = 180;
                    DropHelper.DropItem(target, ItemID.Star);
                }
            }

            if ((target.damage > 5 || target.boss) && !target.SpawnedFromStatue && target.canGhostHeal && !player.moonLeech)
            {
                if (modPlayer.bloodflareMelee && item.melee)
                {
					int healAmount = Main.rand.Next(3) + 1;
					player.statLife += healAmount;
					player.HealEffect(healAmount);
                }
			}

			if (modPlayer.reaverDefense)
			{
                if (Main.player[Main.myPlayer].lifeSteal > 0f && target.canGhostHeal && !player.moonLeech)
                {
					float healMult = 0.2f;
					float heal = damage * healMult;

					if (heal > 50)
						heal = 50;
					if (Main.rand.Next(10) > 0)
						heal = 0;

					if ((int)heal > 0 && !Main.player[Main.myPlayer].moonLeech)
					{
						Main.player[Main.myPlayer].lifeSteal -= heal * 2f;
						float lowestHealthCheck = 0f;
						int healTarget = player.whoAmI;
						for (int i = 0; i < Main.maxPlayers; i++)
						{
							Player otherPlayer = Main.player[i];
							if (otherPlayer.active && !otherPlayer.dead && ((!player.hostile && !otherPlayer.hostile) || player.team == otherPlayer.team))
							{
								float playerDist = Vector2.Distance(target.Center, otherPlayer.Center);
								if (playerDist < 1200f && (otherPlayer.statLifeMax2 - otherPlayer.statLife) > lowestHealthCheck)
								{
									lowestHealthCheck = otherPlayer.statLifeMax2 - otherPlayer.statLife;
									healTarget = i;
								}
							}
						}
						Projectile.NewProjectile(target.Center, Vector2.Zero, ProjectileType<ReaverHealOrb>(), 0, 0f, player.whoAmI, healTarget, heal);
					}
				}
			}
		}
		#endregion

        #region The Horseman's Blade
        public static void HorsemansBladeOnHit(Player player, int targetIdx, int damage, float knockback, int extraUpdateAmt = 0, int type = ProjectileID.FlamingJack)
        {
            int logicCheckScreenHeight = Main.LogicCheckScreenHeight;
            int logicCheckScreenWidth = Main.LogicCheckScreenWidth;
            int x = Main.rand.Next(100, 300);
            int y = Main.rand.Next(100, 300);
            switch (Main.rand.Next(4))
            {
                case 0:
                    x -= logicCheckScreenWidth / 2 + x;
                    break;
                case 1:
                    x += logicCheckScreenWidth / 2 - x;
                    break;
                case 2:
                    y -= logicCheckScreenHeight / 2 + y;
                    break;
                case 3:
                    y += logicCheckScreenHeight / 2 - y;
                    break;
                default:
                    break;
            }
            x += (int)player.position.X;
            y += (int)player.position.Y;
            float speed = 8f;
            Vector2 spawnPos = new Vector2((float)x, (float)y);
			Vector2 velocity = Main.npc[targetIdx].DirectionFrom(spawnPos);
			velocity *= speed;
            int projectile = Projectile.NewProjectile(spawnPos, velocity, type, damage, knockback, player.whoAmI, targetIdx, 0f);
			Main.projectile[projectile].extraUpdates += extraUpdateAmt;
        }
        #endregion
	}
}
