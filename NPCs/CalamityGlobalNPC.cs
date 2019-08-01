using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Projectiles;
using CalamityMod.World;

namespace CalamityMod.NPCs
{
    public class CalamityGlobalNPC : GlobalNPC
	{
        #region Instance Per Entity
		public override bool InstancePerEntity
		{
			get
			{
				return true;
			}
		}
		#endregion

		#region Variables
		//Damage reduction
		private float protection = 0f;
		private float defProtection = 0f;

		//NewAI
		private const int maxAIMod = 4;
		public float[] newAI = new float[maxAIMod];

		//Town NPC Patreon
		private bool setNewName = true;

		//Draedons Remote
		public static bool DraedonMayhem = false;

		//Lunatic Cultist Rev+ attack
		public int CultProjectiles = 2;
		public float CultAngleSpread = 170f;
		public int CultCountdown = 0;

		//Debuffs
		public bool wCleave = false;
		public bool bBlood = false;
		public bool dFlames = false;
		public bool marked = false;
		public bool irradiated = false;
		public bool bFlames = false;
		public bool hFlames = false;
		public bool pFlames = false;
		public bool gState = false;
		public bool aCrunch = false;
		public bool tSad = false;
		public bool pShred = false;
		public bool cDepth = false;
		public bool gsInferno = false;
		public bool aFlames = false;
		public bool eFreeze = false;
		public bool wDeath = false;
		public bool nightwither = false;
		public bool silvaStun = false;
		public bool enraged = false;
		public bool yellowCandle = false;
		public bool pearlAura = false;
		public bool shellfishVore = false;
		public bool clamDebuff = false;

		//whoAmI Variables
		public static int bobbitWormBottom = -1;
		public static int hiveMind = -1;
		public static int perfHive = -1;
		public static int slimeGodPurple = -1;
		public static int slimeGodRed = -1;
		public static int slimeGod = -1;
		public static int laserEye = -1;
		public static int fireEye = -1;
		public static int brimstoneElemental = -1;
		public static int cataclysm = -1;
		public static int catastrophe = -1;
		public static int calamitas = -1;
		public static int leviathan = -1;
		public static int siren = -1;
		public static int scavenger = -1;
		public static int astrumDeusHeadMain = -1;
		public static int energyFlame = -1;
		public static int doughnutBoss = -1;
		public static int holyBossAttacker = -1;
		public static int holyBossDefender = -1;
		public static int holyBossHealer = -1;
		public static int holyBoss = -1;
		public static int voidBoss = -1;
		public static int ghostBossClone = -1;
		public static int ghostBoss = -1;
		public static int DoGHead = -1;
		public static int SCalCataclysm = -1;
		public static int SCalCatastrophe = -1;
		public static int SCal = -1;
		public static int SCalWorm = -1;
		#endregion

		#region Reset Effects
		public override void ResetEffects(NPC npc)
		{
			if (bobbitWormBottom >= 0 && !Main.npc[bobbitWormBottom].active)
				bobbitWormBottom = -1;
			if (hiveMind >= 0 && !Main.npc[hiveMind].active)
				hiveMind = -1;
			if (perfHive >= 0 && !Main.npc[perfHive].active)
				perfHive = -1;
			if (slimeGodPurple >= 0 && !Main.npc[slimeGodPurple].active)
				slimeGodPurple = -1;
			if (slimeGodRed >= 0 && !Main.npc[slimeGodRed].active)
				slimeGodRed = -1;
			if (slimeGod >= 0 && !Main.npc[slimeGod].active)
				slimeGod = -1;
			if (laserEye >= 0 && !Main.npc[laserEye].active)
				laserEye = -1;
			if (fireEye >= 0 && !Main.npc[fireEye].active)
				fireEye = -1;
			if (brimstoneElemental >= 0 && !Main.npc[brimstoneElemental].active)
				brimstoneElemental = -1;
			if (cataclysm >= 0 && !Main.npc[cataclysm].active)
				cataclysm = -1;
			if (catastrophe >= 0 && !Main.npc[catastrophe].active)
				catastrophe = -1;
			if (calamitas >= 0 && !Main.npc[calamitas].active)
				calamitas = -1;
			if (leviathan >= 0 && !Main.npc[leviathan].active)
				leviathan = -1;
			if (siren >= 0 && !Main.npc[siren].active)
				siren = -1;
			if (scavenger >= 0 && !Main.npc[scavenger].active)
				scavenger = -1;
			if (astrumDeusHeadMain >= 0 && !Main.npc[astrumDeusHeadMain].active)
				astrumDeusHeadMain = -1;
			if (energyFlame >= 0 && !Main.npc[energyFlame].active)
				energyFlame = -1;
			if (doughnutBoss >= 0 && !Main.npc[doughnutBoss].active)
				doughnutBoss = -1;
			if (holyBossAttacker >= 0 && !Main.npc[holyBossAttacker].active)
				holyBossAttacker = -1;
			if (holyBossDefender >= 0 && !Main.npc[holyBossDefender].active)
				holyBossDefender = -1;
			if (holyBossHealer >= 0 && !Main.npc[holyBossHealer].active)
				holyBossHealer = -1;
			if (holyBoss >= 0 && !Main.npc[holyBoss].active)
				holyBoss = -1;
			if (voidBoss >= 0 && !Main.npc[voidBoss].active)
				voidBoss = -1;
			if (ghostBossClone >= 0 && !Main.npc[ghostBossClone].active)
				ghostBossClone = -1;
			if (ghostBoss >= 0 && !Main.npc[ghostBoss].active)
				ghostBoss = -1;
			if (DoGHead >= 0 && !Main.npc[DoGHead].active)
				DoGHead = -1;
			if (SCalCataclysm >= 0 && !Main.npc[SCalCataclysm].active)
				SCalCataclysm = -1;
			if (SCalCatastrophe >= 0 && !Main.npc[SCalCatastrophe].active)
				SCalCatastrophe = -1;
			if (SCal >= 0 && !Main.npc[SCal].active)
				SCal = -1;
			if (SCalWorm >= 0 && !Main.npc[SCalWorm].active)
				SCalWorm = -1;

			wCleave = false;
			bBlood = false;
			dFlames = false;
			marked = false;
			irradiated = false;
			bFlames = false;
			hFlames = false;
			pFlames = false;
			gState = false;
			aCrunch = false;
			tSad = false;
			pShred = false;
			cDepth = false;
			gsInferno = false;
			aFlames = false;
			eFreeze = false;
			wDeath = false;
			nightwither = false;
			silvaStun = false;
			enraged = false;
			yellowCandle = false;
			pearlAura = false;
			shellfishVore = false;
			clamDebuff = false;
		}
		#endregion

		#region Life Regen
		public override void UpdateLifeRegen(NPC npc, ref int damage)
		{
			int genLimit = Main.maxTilesX / 2;
			int abyssChasmX = (CalamityWorld.abyssSide ? genLimit - (genLimit - 135) : genLimit + (genLimit - 135));
			bool abyssPosX = false;

			if (CalamityWorld.abyssSide)
			{
				if ((double)(npc.position.X / 16f) < abyssChasmX + 80)
					abyssPosX = true;
			}
			else
			{
				if ((double)(npc.position.X / 16f) > abyssChasmX - 80)
					abyssPosX = true;
			}

			bool hurtByAbyss = (npc.wet && npc.damage > 0 && !npc.boss && !npc.friendly && !npc.dontTakeDamage &&
				((((double)(npc.position.Y / 16f) > (Main.rockLayer - (double)Main.maxTilesY * 0.05)) &&
				((double)(npc.position.Y / 16f) <= Main.maxTilesY - 250) && abyssPosX) || CalamityWorld.abyssTiles > 200) &&
				!npc.buffImmune[mod.BuffType("CrushDepth")]);
			if (hurtByAbyss)
			{
				npc.AddBuff(mod.BuffType("CrushDepth"), 2);

				if (npc.DeathSound != null)
					npc.DeathSound = null;

				if (npc.HitSound != null)
					npc.HitSound = null;
			}

			if (npc.wet && npc.damage > 0 && !npc.boss && !npc.friendly && !npc.dontTakeDamage && CalamityWorld.sulphurTiles > 30 &&
				!npc.buffImmune[BuffID.Poisoned] && !npc.buffImmune[mod.BuffType("CrushDepth")])
			{
				npc.AddBuff(BuffID.Poisoned, 2);
			}

			if (Main.raining && npc.damage > 0 && !npc.boss && !npc.friendly && !npc.dontTakeDamage && CalamityWorld.sulphurTiles > 30 &&
				!npc.buffImmune[BuffID.Poisoned] && !npc.buffImmune[mod.BuffType("CrushDepth")])
			{
				npc.AddBuff(mod.BuffType("Irradiated"), 2);
			}

			if (npc.venom)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				int num7 = 0;
				for (int j = 0; j < 1000; j++)
				{
					if (Main.projectile[j].active &&
						(Main.projectile[j].type == mod.ProjectileType("Lionfish") || Main.projectile[j].type == mod.ProjectileType("SulphuricAcidCannon2")) &&
						Main.projectile[j].ai[0] == 1f && Main.projectile[j].ai[1] == (float)npc.whoAmI)
					{
						num7++;
					}
				}
				if (num7 > 0)
				{
					npc.lifeRegen -= num7 * 30;

					if (damage < num7 * 6)
						damage = num7 * 6;
				}
			}
			if (shellfishVore)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				int amount = 0;
				for (int j = 0; j < 1000; j++)
				{
					if (Main.projectile[j].active &&
						(Main.projectile[j].type == mod.ProjectileType("Shellfish")) &&
						Main.projectile[j].ai[0] == 1f && Main.projectile[j].ai[1] == (float)npc.whoAmI &&
						amount < 5)
					{
						amount++;
					}
				}
				npc.lifeRegen -= amount * 350;

				if (damage < amount * 35)
					damage = amount * 35;
			}
			if (clamDebuff)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				int amount = 0;
				for (int j = 0; j < 1000; j++)
				{
					if (Main.projectile[j].active &&
						(Main.projectile[j].type == mod.ProjectileType("SnapClamProj")) &&
						Main.projectile[j].ai[0] == 1f && Main.projectile[j].ai[1] == (float)npc.whoAmI &&
						amount < 2)
					{
						amount++;
					}
				}
				npc.lifeRegen -= amount * 25;

				if (damage < amount * 5)
					damage = amount * 5;
			}
			if (irradiated)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				npc.lifeRegen -= 20;

				if (damage < 4)
					damage = 4;
			}
			if (cDepth)
			{
				if (npc.defense < 0)
					npc.defense = 0;

				int depthDamage = Main.hardMode ? 80 : 12;
				if (hurtByAbyss)
					depthDamage = 300;

				int calcDepthDamage = depthDamage - npc.defense;
				if (calcDepthDamage < 0)
					calcDepthDamage = 0;

				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				npc.lifeRegen -= calcDepthDamage * 5;

				if (damage < calcDepthDamage)
					damage = calcDepthDamage;
			}
			if (bFlames)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				npc.lifeRegen -= 40;

				if (damage < 8)
					damage = 8;
			}
			if (hFlames)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				npc.lifeRegen -= 50;

				if (damage < 10)
					damage = 10;
			}
			if (pFlames)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				npc.lifeRegen -= 100;

				if (damage < 20)
					damage = 20;
			}
			if (gsInferno)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				npc.lifeRegen -= 250;

				if (damage < 50)
					damage = 50;
			}
			if (aFlames)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				npc.lifeRegen -= 125;

				if (damage < 25)
					damage = 25;
			}
			if (pShred)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				npc.lifeRegen -= 1500;

				if (damage < 300)
					damage = 300;
			}
			if (gState)
			{
				npc.velocity.X = 0f;
				npc.velocity.Y = npc.velocity.Y + 0.05f;

				if (npc.velocity.Y > 15f)
					npc.velocity.Y = 15f;
			}
			if (eFreeze && !CalamityWorld.bossRushActive)
			{
				npc.velocity.X = 0f;
				npc.velocity.Y = npc.velocity.Y + 0.1f;

				if (npc.velocity.Y > 15f)
					npc.velocity.Y = 15f;
			}
			if (tSad)
			{
				npc.velocity.Y /= 2;
				npc.velocity.X /= 2;
			}
			if (nightwither)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				npc.lifeRegen -= 200;

				if (damage < 40)
					damage = 40;
			}
			if (dFlames)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				npc.lifeRegen -= 2500;

				if (damage < 500)
					damage = 500;
			}
			if (bBlood)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				npc.lifeRegen -= 50;

				if (damage < 10)
					damage = 10;
			}
		}
		#endregion

		#region Set Defaults
		public override void SetDefaults(NPC npc)
		{
			for (int m = 0; m < maxAIMod; m++)
				newAI[m] = 0f;

			if (npc.boss && CalamityWorld.revenge)
			{
				if (npc.type != mod.NPCType("HiveMindP2") && npc.type != mod.NPCType("Leviathan") && npc.type != mod.NPCType("StormWeaverHeadNaked") &&
					npc.type != mod.NPCType("StormWeaverBodyNaked") && npc.type != mod.NPCType("StormWeaverTailNaked") &&
					npc.type != mod.NPCType("DevourerofGodsHeadS") && npc.type != mod.NPCType("DevourerofGodsBodyS") &&
					npc.type != mod.NPCType("DevourerofGodsTailS"))
				{
					if (Main.netMode != 2)
					{
						if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active)
							Main.player[Main.myPlayer].GetModPlayer<CalamityPlayer>(mod).adrenaline = 0;
					}
				}
			}

			if (CalamityWorld.bossRushActive)
			{
				BossRushStatChanges(npc, mod);
			}
			else
			{
				if (CalamityMod.enemyImmunityList.Contains(npc.type))
				{
					npc.buffImmune[mod.BuffType("GlacialState")] = true;
					npc.buffImmune[mod.BuffType("TemporalSadness")] = true;
				}
				if (npc.type == NPCID.TheDestroyer ||
					npc.type == NPCID.TheDestroyerBody ||
					npc.type == NPCID.TheDestroyerTail ||
					npc.type == NPCID.DD2EterniaCrystal ||
					npc.townNPC)
				{
					for (int k = 0; k < npc.buffImmune.Length; k++)
						npc.buffImmune[k] = true;

					if (npc.townNPC)
					{
						npc.buffImmune[BuffID.Wet] = false;
						npc.buffImmune[BuffID.Slimed] = false;
						npc.buffImmune[BuffID.Lovestruck] = false;
						npc.buffImmune[BuffID.Stinky] = false;
					}
				}
				if (npc.buffImmune[mod.BuffType("Enraged")])
					npc.buffImmune[mod.BuffType("Enraged")] = false;
			}

			BossValueChanges(npc);

			if (CalamityWorld.defiled)
				npc.value = (float)((int)((double)npc.value * 1.5));

			if (DraedonMayhem)
				DraedonMechaMayhemStatChanges(npc);

			if (CalamityWorld.revenge)
				RevengeanceStatChanges(npc, mod);

			OtherStatChanges(npc);

			if (protection > 0f)
				defProtection = protection;
		}
		#endregion

		#region Boss Rush Stat Changes
		private void BossRushStatChanges(NPC npc, Mod mod)
		{
			if (!npc.friendly)
			{
				for (int k = 0; k < npc.buffImmune.Length; k++)
					npc.buffImmune[k] = true;

				if (npc.type != mod.NPCType("DevourerofGodsHeadS") && npc.type != mod.NPCType("DevourerofGodsBodyS") && npc.type != mod.NPCType("DevourerofGodsTailS"))
				{
					npc.buffImmune[BuffID.Ichor] = false;
					npc.buffImmune[BuffID.CursedInferno] = false;
				}
				npc.buffImmune[mod.BuffType("Enraged")] = false;
			}

			switch (npc.type)
			{
				case NPCID.QueenBee: //Tier 1
					npc.lifeMax = 420000;
					break;
				case NPCID.BrainofCthulhu:
					npc.lifeMax = 200000;
					break;
				case NPCID.Creeper:
					npc.lifeMax = 20000;
					break;
				case NPCID.KingSlime:
					npc.lifeMax = 500000;
					break;
				case NPCID.BlueSlime:
					npc.lifeMax = 12000;
					break;
				case NPCID.SlimeSpiked:
					npc.lifeMax = 24000;
					break;
				case NPCID.EyeofCthulhu:
					npc.lifeMax = 600000;
					break;
				case NPCID.ServantofCthulhu:
					npc.lifeMax = 60000;
					break;
				case NPCID.SkeletronPrime:
					npc.lifeMax = 980000;
					break;
				case NPCID.PrimeCannon:
					npc.lifeMax = 450000;
					break;
				case NPCID.PrimeVice:
					npc.lifeMax = 540000;
					break;
				case NPCID.PrimeSaw:
					npc.lifeMax = 450000;
					break;
				case NPCID.PrimeLaser:
					npc.lifeMax = 380000;
					break;
				case NPCID.Golem:
					npc.lifeMax = 450000;
					break;
				case NPCID.GolemHead:
					npc.lifeMax = 300000;
					break;
				case NPCID.GolemFistLeft:
					npc.lifeMax = 250000;
					break;
				case NPCID.GolemFistRight:
					npc.lifeMax = 250000;
					break;
				case NPCID.EaterofWorldsHead:
					npc.lifeMax = 20000;
					break;
				case NPCID.EaterofWorldsBody:
					npc.lifeMax = 30000;
					break;
				case NPCID.EaterofWorldsTail:
					npc.lifeMax = 40000;
					break;
				case NPCID.TheDestroyer: //Tier 2
					npc.lifeMax = 2500000;
					break;
				case NPCID.TheDestroyerBody:
					npc.lifeMax = 2500000;
					break;
				case NPCID.TheDestroyerTail:
					npc.lifeMax = 2500000;
					break;
				case NPCID.Probe:
					npc.lifeMax = 50000;
					break;
				case NPCID.Spazmatism:
					npc.lifeMax = 1300000;
					break;
				case NPCID.Retinazer:
					npc.lifeMax = 900000;
					break;
				case NPCID.WallofFlesh:
					npc.lifeMax = 2400000;
					break;
				case NPCID.WallofFleshEye:
					npc.lifeMax = 2400000;
					break;
				case NPCID.SkeletronHead:
					npc.lifeMax = 1300000;
					break;
				case NPCID.SkeletronHand:
					npc.lifeMax = 500000;
					break;
				case NPCID.CultistBoss: //Tier 3
					npc.lifeMax = 1100000;
					break;
				case NPCID.CultistDragonHead:
					npc.lifeMax = 400000;
					break;
				case NPCID.CultistDragonBody1:
					npc.lifeMax = 400000;
					break;
				case NPCID.CultistDragonBody2:
					npc.lifeMax = 400000;
					break;
				case NPCID.CultistDragonBody3:
					npc.lifeMax = 400000;
					break;
				case NPCID.CultistDragonBody4:
					npc.lifeMax = 400000;
					break;
				case NPCID.CultistDragonTail:
					npc.lifeMax = 400000;
					break;
				case NPCID.AncientCultistSquidhead:
					npc.lifeMax = 300000;
					break;
				case NPCID.Plantera:
					npc.lifeMax = 2100000;
					break;
				case NPCID.PlanterasTentacle:
					npc.lifeMax = 90000;
					break;
				case NPCID.DukeFishron: //Tier 4
					npc.lifeMax = 2500000;
					break;
				case NPCID.MoonLordCore:
					npc.lifeMax = 1400000;
					break;
				case NPCID.MoonLordHand:
					npc.lifeMax = 450000;
					break;
				case NPCID.MoonLordHead:
					npc.lifeMax = 600000;
					break;
				case NPCID.MoonLordFreeEye:
					npc.lifeMax = 1000;
					break;
				default:
					break;
			}
		}
		#endregion

		#region Boss Value Changes
		private void BossValueChanges(NPC npc)
		{
			switch (npc.type)
			{
				case NPCID.QueenBee:
					npc.value = Item.buyPrice(0, 5, 0, 0);
					break;
				case NPCID.SkeletronHead:
					npc.value = Item.buyPrice(0, 7, 0, 0);
					break;
				case NPCID.DukeFishron:
					npc.value = Item.buyPrice(0, 25, 0, 0);
					break;
				case NPCID.CultistBoss:
					npc.value = Item.buyPrice(0, 25, 0, 0);
					break;
				case NPCID.MoonLordCore:
					npc.value = Item.buyPrice(0, 30, 0, 0);
					break;
				default:
					break;
			}
		}
		#endregion

		#region Draedon Mecha Mayhem Stat Changes
		private void DraedonMechaMayhemStatChanges(NPC npc)
		{
			if (npc.type == NPCID.TheDestroyer || npc.type == NPCID.TheDestroyerBody || npc.type == NPCID.TheDestroyerTail)
			{
				if (CalamityWorld.death)
				{
					npc.lifeMax = (int)((double)npc.lifeMax * 2.7);
					npc.scale *= 1.25f;
				}
				else
				{
					npc.lifeMax = (int)((double)npc.lifeMax * 1.8);
					npc.scale *= 1.2f;
				}
				npc.npcSlots = 10f;
			}
			else if (npc.type == NPCID.Probe)
			{
				if (CalamityWorld.death)
				{
					npc.lifeMax = (int)((double)npc.lifeMax * 1.9);
					npc.scale *= 1.25f;
				}
				else
				{
					npc.lifeMax = (int)((double)npc.lifeMax * 1.6);
					npc.scale *= 1.2f;
				}
			}
			else if (npc.type == NPCID.SkeletronPrime || npc.type == NPCID.PrimeVice || npc.type == NPCID.PrimeCannon || npc.type == NPCID.PrimeSaw || npc.type == NPCID.PrimeLaser)
			{
				if (CalamityWorld.death)
					npc.lifeMax = (int)((double)npc.lifeMax * 2.9);
				else
					npc.lifeMax = (int)((double)npc.lifeMax * 1.6);

				if (npc.type == NPCID.SkeletronPrime)
					npc.npcSlots = 12f;
			}
			else if (npc.type == NPCID.Retinazer)
			{
				if (CalamityWorld.death)
					npc.lifeMax = (int)((double)npc.lifeMax * 3.0);
				else
					npc.lifeMax = (int)((double)npc.lifeMax * 1.8);

				npc.npcSlots = 10f;
			}
			else if (npc.type == NPCID.Spazmatism)
			{
				if (CalamityWorld.death)
					npc.lifeMax = (int)((double)npc.lifeMax * 3.0);
				else
					npc.lifeMax = (int)((double)npc.lifeMax * 1.8);

				npc.npcSlots = 10f;
			}
		}
		#endregion

		#region Revengeance Stat Changes
		private void RevengeanceStatChanges(NPC npc, Mod mod)
		{
			npc.value = (float)((int)((double)npc.value * 1.5));

			if (npc.type == NPCID.MoonLordFreeEye)
			{
				npc.lifeMax = (int)((double)npc.lifeMax * 150.0);
			}
			else if (npc.type == NPCID.Mothron)
			{
				npc.scale = 1.25f;
			}
			else if (npc.type == NPCID.MoonLordCore)
			{
				if (CalamityWorld.death)
					npc.lifeMax = (int)((double)npc.lifeMax * 2.4);
				else
					npc.lifeMax = (int)((double)npc.lifeMax * 1.9);

				npc.npcSlots = 12f;
			}
			else if (npc.type == NPCID.MoonLordHand || npc.type == NPCID.MoonLordHead)
			{
				if (CalamityWorld.death)
					npc.lifeMax = (int)((double)npc.lifeMax * 1.4);
				else
					npc.lifeMax = (int)((double)npc.lifeMax * 1.2);

				npc.npcSlots = 12f;
			}
			else if (npc.type >= NPCID.CultistDragonHead && npc.type <= NPCID.CultistDragonTail)
			{
				if (CalamityWorld.death)
					npc.lifeMax = (int)((double)npc.lifeMax * 10.0);
				else
					npc.lifeMax = (int)((double)npc.lifeMax * 5.0);
			}
			else if (npc.type == NPCID.DukeFishron)
			{
				if (CalamityWorld.death)
					npc.lifeMax = (int)((double)npc.lifeMax * 3.525);
				else
					npc.lifeMax = (int)((double)npc.lifeMax * 1.85);

				npc.npcSlots = 20f;
			}
			else if (npc.type == NPCID.Sharkron || npc.type == NPCID.Sharkron2)
			{
				npc.lifeMax = (int)((double)npc.lifeMax * 30.0);
			}
			else if (npc.type == NPCID.Golem)
			{
				if (CalamityWorld.death)
					npc.lifeMax = (int)((double)npc.lifeMax * 8.5);
				else
					npc.lifeMax = (int)((double)npc.lifeMax * 3.5);

				npc.npcSlots = 64f;
			}
			else if (npc.type == NPCID.Plantera)
			{
				if (CalamityWorld.death)
					npc.lifeMax = (int)((double)npc.lifeMax * 3.4);
				else
					npc.lifeMax = (int)((double)npc.lifeMax * 2.3);

				npc.npcSlots = 32f;
			}
			else if (npc.type == NPCID.WallofFlesh || npc.type == NPCID.WallofFleshEye)
			{
				if (CalamityWorld.death)
					npc.lifeMax = (int)((double)npc.lifeMax * 2.6);
				else
					npc.lifeMax = (int)((double)npc.lifeMax * 1.9);

				if (npc.type == NPCID.WallofFlesh)
					npc.npcSlots = 20f;
			}
			else if (npc.type == NPCID.TheHungryII)
			{
				if (CalamityWorld.death)
				{
					npc.noTileCollide = true;
					npc.lifeMax = (int)((double)npc.lifeMax * 1.5);
				}
				else
					npc.lifeMax = (int)((double)npc.lifeMax * 1.25);
			}
			else if (npc.type == NPCID.LeechHead || npc.type == NPCID.LeechBody || npc.type == NPCID.LeechTail)
			{
				if (CalamityWorld.death)
					npc.lifeMax = (int)((double)npc.lifeMax * 1.5);
				else
					npc.lifeMax = (int)((double)npc.lifeMax * 1.25);
			}
			else if (npc.type == NPCID.SkeletronHead)
			{
				if (CalamityWorld.death)
					npc.lifeMax = (int)((double)npc.lifeMax * 1.4);
				else
					npc.lifeMax = (int)((double)npc.lifeMax * 1.25);

				npc.npcSlots = 12f;
			}
			else if (npc.type == NPCID.SkeletronHand)
			{
				if (CalamityWorld.death)
					npc.lifeMax = (int)((double)npc.lifeMax * 1.4);
				else
					npc.lifeMax = (int)((double)npc.lifeMax * 1.3);
			}
			else if (npc.type == NPCID.QueenBee)
			{
				if (CalamityWorld.death)
					npc.lifeMax = (int)((double)npc.lifeMax * 1.65);
				else
					npc.lifeMax = (int)((double)npc.lifeMax * 1.15);

				npc.npcSlots = 14f;
			}
			else if (npc.type == NPCID.BrainofCthulhu)
			{
				if (CalamityWorld.death)
					npc.lifeMax = (int)((double)npc.lifeMax * 1.9);
				else
					npc.lifeMax = (int)((double)npc.lifeMax * 1.4);

				npc.npcSlots = 12f;
			}
			else if (npc.type == NPCID.Creeper)
			{
				if (CalamityWorld.death)
					npc.lifeMax = (int)((double)npc.lifeMax * 1.2);
				else
					npc.lifeMax = (int)((double)npc.lifeMax * 1.1);
			}
			else if (npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail)
			{
				if (npc.type == NPCID.EaterofWorldsHead)
				{
					npc.npcSlots = 10f;

					if (CalamityWorld.death)
						npc.reflectingProjectiles = true;
				}

				if (CalamityWorld.death)
					npc.lifeMax = (int)((double)npc.lifeMax * 1.4);
				else
					npc.lifeMax = (int)((double)npc.lifeMax * 1.3);
			}
			else if (npc.type == NPCID.EyeofCthulhu)
			{
				if (CalamityWorld.death)
					npc.lifeMax = (int)((double)npc.lifeMax * 2.3);
				else
					npc.lifeMax = (int)((double)npc.lifeMax * 1.25);

				npc.npcSlots = 10f;
			}
			else if (npc.type == NPCID.KingSlime)
			{
				if (CalamityWorld.death)
					npc.lifeMax = (int)((double)npc.lifeMax * 1.85);
			}
			else if (npc.type == NPCID.Wraith || npc.type == NPCID.Mimic || npc.type == NPCID.Reaper || npc.type == NPCID.PresentMimic || npc.type == NPCID.SandElemental)
			{
				npc.knockBackResist = 0f;
			}

			if (!DraedonMayhem)
			{
				if (npc.type == NPCID.TheDestroyer || npc.type == NPCID.TheDestroyerBody || npc.type == NPCID.TheDestroyerTail)
				{
					if (CalamityWorld.death)
					{
						npc.lifeMax = (int)((double)npc.lifeMax * 1.6);
						npc.scale *= 1.2f;
					}
					else
						npc.lifeMax = (int)((double)npc.lifeMax * 1.25);

					npc.npcSlots = 10f;
				}
				else if (npc.type == NPCID.Probe)
				{
					if (CalamityWorld.death)
					{
						npc.lifeMax = (int)((double)npc.lifeMax * 1.25);
						npc.scale *= 1.2f;
					}
				}
				else if (npc.type == NPCID.SkeletronPrime || npc.type == NPCID.PrimeVice || npc.type == NPCID.PrimeCannon || npc.type == NPCID.PrimeSaw || npc.type == NPCID.PrimeLaser)
				{
					if (CalamityWorld.death)
						npc.lifeMax = (int)((double)npc.lifeMax * 1.5);
					else
						npc.lifeMax = (int)((double)npc.lifeMax * 1.15);

					if (npc.type == NPCID.SkeletronPrime)
						npc.npcSlots = 12f;
				}
				else if (npc.type == NPCID.Retinazer)
				{
					if (CalamityWorld.death)
						npc.lifeMax = (int)((double)npc.lifeMax * 1.4);
					else
						npc.lifeMax = (int)((double)npc.lifeMax * 1.25);

					npc.npcSlots = 10f;
				}
				else if (npc.type == NPCID.Spazmatism)
				{
					if (CalamityWorld.death)
						npc.lifeMax = (int)((double)npc.lifeMax * 1.45);
					else
						npc.lifeMax = (int)((double)npc.lifeMax * 1.3);

					npc.npcSlots = 10f;
				}
			}

			if (npc.type == NPCID.Probe || npc.type == NPCID.MoonLordFreeEye || (npc.type >= 454 && npc.type <= 459) ||
				npc.type == NPCID.Sharkron || npc.type == NPCID.Sharkron2 || npc.type == NPCID.PlanterasTentacle ||
				npc.type == NPCID.Spore || npc.type == NPCID.TheHungryII || npc.type == NPCID.LeechHead ||
				npc.type == NPCID.LeechBody || npc.type == NPCID.LeechTail || npc.type == NPCID.TheDestroyerBody ||
				npc.type == NPCID.TheDestroyerTail || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail ||
				npc.type == NPCID.GolemHead || npc.type == NPCID.GolemFistRight || npc.type == NPCID.GolemFistLeft ||
				npc.type == NPCID.MoonLordCore)
			{
				npc.canGhostHeal = false;
			}

			RevengeanceDamageReduction(npc, mod);
		}
		#endregion

		#region Revengeance Damage Reduction
		private void RevengeanceDamageReduction(NPC npc, Mod mod)
		{
			if (Config.RevengeanceAndDeathThoriumBossBuff)
			{
				Mod thorium = ModLoader.GetMod("ThoriumMod");
				if (thorium != null)
				{
					if (npc.type == thorium.NPCType("Viscount") || npc.type == thorium.NPCType("BoreanStrider") || npc.type == thorium.NPCType("FallenDeathBeholder") ||
						npc.type == thorium.NPCType("Lich") || npc.type == thorium.NPCType("AbyssionReleased"))
					{
						protection = 0.05f;
					}
					else if (npc.type == thorium.NPCType("CryoCore") || npc.type == thorium.NPCType("BioCore") || npc.type == thorium.NPCType("PyroCore") ||
						npc.type == thorium.NPCType("Aquaius"))
					{
						protection = 0.1f;
					}
					else if (npc.type == thorium.NPCType("ThePrimeScouter") || npc.type == thorium.NPCType("FallenDeathBeholder2") || npc.type == thorium.NPCType("SlagFury") ||
						npc.type == thorium.NPCType("Aquaius2") || npc.type == thorium.NPCType("GraniteEnergyStorm"))
					{
						protection = 0.2f;
					}
					else if (npc.type == thorium.NPCType("TheBuriedWarrior") || npc.type == thorium.NPCType("TheBuriedWarrior1") || npc.type == thorium.NPCType("TheBuriedWarrior2") ||
						npc.type == thorium.NPCType("LichHeadless") || npc.type == thorium.NPCType("AbyssionCracked"))
					{
						protection = 0.25f;
					}
					else if (npc.type == thorium.NPCType("Omnicide"))
					{
						protection = 0.3f;
					}
					else if (npc.type == thorium.NPCType("Abyssion"))
					{
						protection = 0.35f;
					}
				}
			}

			if (npc.type == mod.NPCType("Cnidrion") || npc.type == mod.NPCType("DesertScourgeBody") || npc.type == mod.NPCType("ColossalSquid") ||
				npc.type == mod.NPCType("Siren") || npc.type == mod.NPCType("ThiccWaifu") || npc.type == mod.NPCType("ProfanedGuardianBoss3") ||
				npc.type == mod.NPCType("ScornEater") || npc.type == mod.NPCType("AquaticScourgeBody") || npc.type == mod.NPCType("AquaticScourgeBodyAlt") ||
				npc.type == mod.NPCType("Mauler") || npc.type == mod.NPCType("EutrophicRay"))
			{
				protection = 0.05f;
			}
			else if (npc.type == mod.NPCType("AstrumDeusBody") || npc.type == mod.NPCType("SoulSeeker") || npc.type == mod.NPCType("DesertScourgeTail") ||
				npc.type == mod.NPCType("Horse") || npc.type == mod.NPCType("ProfanedEnergyBody") || npc.type == mod.NPCType("ScavengerClawLeft") ||
				npc.type == mod.NPCType("ScavengerClawRight") || npc.type == mod.NPCType("ScavengerHead") || npc.type == mod.NPCType("MantisShrimp") ||
				npc.type == mod.NPCType("PhantomDebris") || npc.type == mod.NPCType("AstrumDeusHead") || npc.type == mod.NPCType("AquaticScourgeHead") ||
				npc.type == mod.NPCType("Cryon") || npc.type == mod.NPCType("Cryogen"))
			{
				protection = 0.1f;
			}
			else if (npc.type == mod.NPCType("ArmoredDiggerHead") || npc.type == mod.NPCType("AstralProbe") || npc.type == mod.NPCType("Calamitas") ||
				npc.type == mod.NPCType("CalamitasRun") || npc.type == mod.NPCType("CalamitasRun2") || npc.type == mod.NPCType("CalamitasRun3") ||
				npc.type == mod.NPCType("SoulSlurper") || npc.type == mod.NPCType("ProvSpawnHealer") || npc.type == mod.NPCType("Gnasher") ||
				npc.type == mod.NPCType("ScavengerLegLeft") || npc.type == mod.NPCType("ScavengerLegRight") || npc.type == mod.NPCType("ShockstormShuttle") ||
				npc.type == mod.NPCType("Reaper") || npc.type == mod.NPCType("OverloadedSoldier") || npc.type == mod.NPCType("AquaticScourgeTail") ||
				npc.type == mod.NPCType("EidolonWyrmHead") || npc.type == mod.NPCType("Aries") || npc.type == mod.NPCType("AstralachneaGround") ||
				npc.type == mod.NPCType("AstralachneaWall") || npc.type == mod.NPCType("Astrageldon") || npc.type == mod.NPCType("Atlas") ||
				npc.type == mod.NPCType("BigSightseer") || npc.type == mod.NPCType("FusionFeeder") || npc.type == mod.NPCType("Hadarian") ||
				npc.type == mod.NPCType("Hive") || npc.type == mod.NPCType("Mantis") || npc.type == mod.NPCType("Nova") || npc.type == mod.NPCType("SmallSightseer") ||
				npc.type == mod.NPCType("StellarCulex"))
			{
				protection = 0.15f;
			}
			else if (npc.type == mod.NPCType("AstrumDeusProbe3") || npc.type == mod.NPCType("PlaguebringerShade") || npc.type == mod.NPCType("BlindedAngler"))
			{
				protection = 0.2f;
			}
			else if (npc.type == mod.NPCType("PlaguebringerGoliath") || npc.type == mod.NPCType("ProfanedGuardianBoss2") || npc.type == mod.NPCType("SandTortoise") ||
				npc.type == mod.NPCType("StasisProbe") || npc.type == mod.NPCType("BobbitWormHead") || npc.type == mod.NPCType("GreatSandShark") ||
				npc.type == mod.NPCType("Clam") || npc.type == mod.NPCType("PrismTurtle"))
			{
				protection = 0.25f;
			}
			else if (npc.type == mod.NPCType("ProvSpawnOffense") || npc.type == mod.NPCType("GiantClam"))
			{
				protection = 0.3f;
			}
			else if (npc.type == mod.NPCType("ArmoredDiggerBody") || npc.type == mod.NPCType("AstrumDeusTail") || npc.type == mod.NPCType("DespairStone") ||
				npc.type == mod.NPCType("SoulSeekerSupreme") || npc.type == mod.NPCType("Leviathan"))
			{
				protection = 0.35f;
			}
			else if (npc.type == mod.NPCType("CryogenIce") || npc.type == mod.NPCType("ProfanedGuardianBoss") || npc.type == mod.NPCType("ProvSpawnDefense") ||
				npc.type == mod.NPCType("ScavengerBody"))
			{
				protection = 0.4f;
			}
			else if (npc.type == mod.NPCType("ArmoredDiggerTail"))
			{
				protection = 0.45f;
			}
			else if (npc.type == mod.NPCType("SirenIce"))
			{
				protection = 0.5f;
			}
			else
			{
				switch (npc.type)
				{
					case NPCID.SkeletronHand:
					case NPCID.SkeletronHead:
					case NPCID.QueenBee:
					case NPCID.HeadlessHorseman:
					case NPCID.FlyingAntlion:
					case NPCID.PirateCaptain:
					case NPCID.MoonLordHead:
					case NPCID.MoonLordHand:
					case NPCID.MoonLordCore:
					case NPCID.MoonLordFreeEye:
					case NPCID.CultistBoss:
					case NPCID.Mothron:
					case NPCID.Crab:
					case NPCID.SeaSnail:
						protection = 0.05f;
						break;
					case NPCID.Antlion:
					case NPCID.TheHungry:
					case NPCID.TheDestroyer:
					case NPCID.UndeadViking:
					case NPCID.MourningWood:
					case NPCID.Everscream:
					case NPCID.GreekSkeleton:
					case NPCID.GraniteFlyer:
					case NPCID.WalkingAntlion:
					case NPCID.Pumpking:
					case NPCID.IceQueen:
					case NPCID.IceGolem:
					case NPCID.AnomuraFungus:
					case NPCID.SkeletonArcher:
					case NPCID.SandElemental:
					case NPCID.Arapaima:
					case NPCID.ArmoredViking:
					case NPCID.DD2Betsy:
					case NPCID.DD2OgreT2:
						protection = 0.1f;
						break;
					case NPCID.ElfCopter:
					case NPCID.GraniteGolem:
					case NPCID.ArmoredSkeleton:
					case NPCID.PirateShipCannon:
					case NPCID.DD2OgreT3:
					case NPCID.Golem:
						protection = 0.15f;
						break;
					case NPCID.Retinazer:
					case NPCID.Spazmatism:
					case NPCID.PrimeCannon:
					case NPCID.PrimeLaser:
					case NPCID.TheDestroyerBody:
					case NPCID.RustyArmoredBonesAxe:
					case NPCID.RustyArmoredBonesFlail:
					case NPCID.RustyArmoredBonesSword:
					case NPCID.RustyArmoredBonesSwordNoArmor:
					case NPCID.BlueArmoredBones:
					case NPCID.BlueArmoredBonesMace:
					case NPCID.BlueArmoredBonesNoPants:
					case NPCID.BlueArmoredBonesSword:
					case NPCID.HellArmoredBones:
					case NPCID.HellArmoredBonesSpikeShield:
					case NPCID.HellArmoredBonesMace:
					case NPCID.HellArmoredBonesSword:
					case NPCID.RaggedCaster:
					case NPCID.RaggedCasterOpenCoat:
					case NPCID.Necromancer:
					case NPCID.NecromancerArmored:
					case NPCID.DiabolistRed:
					case NPCID.DiabolistWhite:
					case NPCID.BoneLee:
					case NPCID.DungeonSpirit:
					case NPCID.GiantCursedSkull:
					case NPCID.SkeletonSniper:
					case NPCID.TacticalSkeleton:
					case NPCID.SkeletonCommando:
					case NPCID.AngryBonesBig:
					case NPCID.AngryBonesBigMuscle:
					case NPCID.AngryBonesBigHelmet:
					case NPCID.MartianSaucerTurret:
					case NPCID.MartianSaucerCannon:
					case NPCID.MartianTurret:
					case NPCID.MartianDrone:
					case NPCID.MartianSaucer:
					case NPCID.MartianSaucerCore:
					case NPCID.Crawdad:
					case NPCID.Crawdad2:
					case NPCID.GiantShelly:
					case NPCID.GiantShelly2:
						protection = 0.2f;
						break;
					case NPCID.PrimeSaw:
					case NPCID.PrimeVice:
					case NPCID.SkeletronPrime:
					case NPCID.Probe:
					case NPCID.PossessedArmor:
						protection = 0.25f;
						break;
					case NPCID.Mimic:
					case NPCID.PresentMimic:
					case NPCID.BigMimicCorruption:
					case NPCID.BigMimicCrimson:
					case NPCID.BigMimicHallow:
					case NPCID.BigMimicJungle:
						protection = 0.3f;
						break;
					case NPCID.GiantTortoise:
					case NPCID.IceTortoise:
					case NPCID.SantaNK1:
					case NPCID.MartianWalker:
					case NPCID.TheDestroyerTail:
						protection = 0.35f;
						break;
					case NPCID.DeadlySphere:
						protection = 0.4f;
						break;
					case NPCID.Paladin:
						protection = 0.45f;
						break;
					case NPCID.WallofFlesh:
					case NPCID.MothronEgg:
						protection = 0.5f;
						break;
					case NPCID.DungeonGuardian:
						protection = 0.999999f;
						break;
					default:
						break;
				}
			}
		}
		#endregion

		#region Other Stat Changes
		private void OtherStatChanges(NPC npc)
		{
			if (npc.type == NPCID.CultistBoss)
			{
				if (CalamityWorld.death)
					npc.lifeMax = (int)((double)npc.lifeMax * 2.25);
				else
					npc.lifeMax = (int)((double)npc.lifeMax * (CalamityWorld.revenge ? 1.55 : 1.15));

				npc.npcSlots = 20f;
			}

			if (npc.type >= NPCID.TombCrawlerHead && npc.type <= NPCID.TombCrawlerTail && !Main.hardMode)
				npc.lifeMax = (int)((double)npc.lifeMax * 0.6);

			if (Main.bloodMoon && NPC.downedMoonlord && !npc.boss && !npc.friendly && !npc.dontTakeDamage && npc.lifeMax <= 2000)
			{
				npc.lifeMax = (int)((double)npc.lifeMax * 3.5);
				npc.damage += 100;
				npc.life = npc.lifeMax;
				npc.defDamage = npc.damage;
			}

			if (CalamityWorld.downedDoG)
			{
				if (CalamityMod.pumpkinMoonBuffList.Contains(npc.type))
				{
					npc.lifeMax = (int)((double)npc.lifeMax * 7.5);
					npc.damage += 200;
					npc.life = npc.lifeMax;
					npc.defDamage = npc.damage;
				}
				else if (CalamityMod.frostMoonBuffList.Contains(npc.type))
				{
					npc.lifeMax = (int)((double)npc.lifeMax * 6.0);
					npc.damage += 200;
					npc.life = npc.lifeMax;
					npc.defDamage = npc.damage;
				}
			}

			if (CalamityMod.eclipseBuffList.Contains(npc.type) && CalamityWorld.buffedEclipse)
			{
				npc.lifeMax = (int)((double)npc.lifeMax * 32.5);
				npc.damage += 250;
				npc.life = npc.lifeMax;
				npc.defDamage = npc.damage;
			}

			if (NPC.downedMoonlord)
			{
				if (CalamityMod.dungeonEnemyBuffList.Contains(npc.type))
				{
					npc.lifeMax = (int)((double)npc.lifeMax * 2.5);
					npc.damage += 150;
					npc.life = npc.lifeMax;
					npc.defDamage = npc.damage;
				}
			}

			if (CalamityWorld.revenge)
			{
				if (CalamityMod.revengeanceEnemyBuffList.Contains(npc.type))
				{
					npc.damage = (int)((double)npc.damage * 1.25);
					npc.defDamage = npc.damage;
				}
			}

			if ((npc.boss && npc.type != NPCID.MartianSaucerCore && npc.type < NPCID.Count) ||
				npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail ||
				npc.type == NPCID.SkeletronHand || npc.type == NPCID.WallofFleshEye || npc.type == NPCID.TheDestroyerBody || npc.type == NPCID.TheDestroyerTail ||
				npc.type == NPCID.PrimeCannon || npc.type == NPCID.PrimeLaser || npc.type == NPCID.PrimeVice || npc.type == NPCID.PrimeSaw ||
				npc.type == NPCID.GolemHead || npc.type == NPCID.GolemFistRight || npc.type == NPCID.GolemFistLeft || npc.type == NPCID.MoonLordHead ||
				npc.type == NPCID.MoonLordHand)
			{
				double HPBoost = (double)Config.BossHealthPercentageBoost * 0.01;
				npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
			}
		}
		#endregion

		#region Scale Expert Multiplayer Stats
		public override void ScaleExpertStats(NPC npc, int numPlayers, float bossLifeScale)
		{
			if (CalamityWorld.revenge)
				ScaleThoriumBossHealth(npc, mod);

			if (Main.netMode != 0)
			{
				if (numPlayers > 1)
				{
					if (((npc.boss || CalamityMod.bossScaleList.Contains(npc.type)) && npc.type < NPCID.Count) ||
						(npc.modNPC != null && npc.modNPC.mod.Name.Equals("CalamityMod")))
					{
						double scalar = 1.0;
						switch (numPlayers) //Decrease HP in multiplayer before vanilla scaling
						{
							case 2:
								scalar = 0.76;
								break;
							case 3:
								scalar = 0.63;
								break;
							case 4:
								scalar = 0.525;
								break;
							case 5:
								scalar = 0.43;
								break;
							case 6:
								scalar = 0.36;
								break;
							default:
								scalar = 0.295;
								break;
						}
						npc.lifeMax = (int)((double)npc.lifeMax * scalar);
					}
				}
			}
		}
		#endregion

		#region Scale Thorium Boss Health
		private void ScaleThoriumBossHealth(NPC npc, Mod mod)
		{
			if (Config.RevengeanceAndDeathThoriumBossBuff)
			{
				Mod thorium = ModLoader.GetMod("ThoriumMod");
				if (thorium != null)
				{
					if (npc.type == thorium.NPCType("Hatchling") || npc.type == thorium.NPCType("DistractJelly") || npc.type == thorium.NPCType("ViscountBaby") ||
						npc.type == thorium.NPCType("BoreanHopper") || npc.type == thorium.NPCType("BoreanMyte1") || npc.type == thorium.NPCType("EnemyBeholder") ||
						npc.type == thorium.NPCType("ThousandSoulPhalactry") || npc.type == thorium.NPCType("AbyssalSpawn"))
					{
						if (CalamityWorld.death)
							npc.lifeMax = (int)((double)npc.lifeMax * 2.3);
						else
							npc.lifeMax = (int)((double)npc.lifeMax * 1.5);
					}
					else if (npc.type == thorium.NPCType("TheGrandThunderBirdv2"))
					{
						npc.buffImmune[mod.BuffType("GlacialState")] = true;
						npc.buffImmune[mod.BuffType("TemporalSadness")] = true;

						if (CalamityWorld.death)
							npc.lifeMax = (int)((double)npc.lifeMax * 2.3);
						else
							npc.lifeMax = (int)((double)npc.lifeMax * 1.5);
					}
					else if (npc.type == thorium.NPCType("QueenJelly"))
					{
						npc.buffImmune[mod.BuffType("GlacialState")] = true;
						npc.buffImmune[mod.BuffType("TemporalSadness")] = true;

						if (CalamityWorld.death)
							npc.lifeMax = (int)((double)npc.lifeMax * 1.9);
						else
							npc.lifeMax = (int)((double)npc.lifeMax * 1.35);
					}
					else if (npc.type == thorium.NPCType("Viscount"))
					{
						npc.buffImmune[mod.BuffType("GlacialState")] = true;
						npc.buffImmune[mod.BuffType("TemporalSadness")] = true;

						if (CalamityWorld.death)
							npc.lifeMax = (int)((double)npc.lifeMax * 1.65);
						else
							npc.lifeMax = (int)((double)npc.lifeMax * 1.25);
					}
					else if (npc.type == thorium.NPCType("GraniteEnergyStorm"))
					{
						npc.buffImmune[mod.BuffType("GlacialState")] = true;
						npc.buffImmune[mod.BuffType("TemporalSadness")] = true;

						if (CalamityWorld.death)
							npc.lifeMax = (int)((double)npc.lifeMax * 1.25);
						else
							npc.lifeMax = (int)((double)npc.lifeMax * 1.1);
					}
					else if (npc.type == thorium.NPCType("TheBuriedWarrior") || npc.type == thorium.NPCType("TheBuriedWarrior1") || npc.type == thorium.NPCType("TheBuriedWarrior2"))
					{
						npc.buffImmune[mod.BuffType("GlacialState")] = true;
						npc.buffImmune[mod.BuffType("TemporalSadness")] = true;

						if (CalamityWorld.death)
							npc.lifeMax = (int)((double)npc.lifeMax * 1.5);
						else
							npc.lifeMax = (int)((double)npc.lifeMax * 1.2);
					}
					else if (npc.type == thorium.NPCType("ThePrimeScouter") || npc.type == thorium.NPCType("CryoCore") || npc.type == thorium.NPCType("BioCore") ||
						npc.type == thorium.NPCType("PyroCore"))
					{
						npc.buffImmune[mod.BuffType("GlacialState")] = true;
						npc.buffImmune[mod.BuffType("TemporalSadness")] = true;

						if (CalamityWorld.death)
							npc.lifeMax = (int)((double)npc.lifeMax * 1.75);
						else
							npc.lifeMax = (int)((double)npc.lifeMax * 1.3);
					}
					else if (npc.type == thorium.NPCType("BoreanStrider") || npc.type == thorium.NPCType("BoreanStriderPopped"))
					{
						npc.buffImmune[mod.BuffType("GlacialState")] = true;
						npc.buffImmune[mod.BuffType("TemporalSadness")] = true;

						if (CalamityWorld.death)
							npc.lifeMax = (int)((double)npc.lifeMax * 2.0);
						else
							npc.lifeMax = (int)((double)npc.lifeMax * 1.4);
					}
					else if (npc.type == thorium.NPCType("FallenDeathBeholder") || npc.type == thorium.NPCType("FallenDeathBeholder2"))
					{
						npc.buffImmune[mod.BuffType("GlacialState")] = true;
						npc.buffImmune[mod.BuffType("TemporalSadness")] = true;

						if (CalamityWorld.death)
							npc.lifeMax = (int)((double)npc.lifeMax * 2.5);
						else
							npc.lifeMax = (int)((double)npc.lifeMax * 1.6);
					}
					else if (npc.type == thorium.NPCType("Lich") || npc.type == thorium.NPCType("LichHeadless"))
					{
						npc.buffImmune[mod.BuffType("GlacialState")] = true;
						npc.buffImmune[mod.BuffType("TemporalSadness")] = true;

						if (CalamityWorld.death)
							npc.lifeMax = (int)((double)npc.lifeMax * 2.5);
						else
							npc.lifeMax = (int)((double)npc.lifeMax * 1.6);
					}
					else if (npc.type == thorium.NPCType("Abyssion") || npc.type == thorium.NPCType("AbyssionCracked") || npc.type == thorium.NPCType("AbyssionReleased"))
					{
						npc.buffImmune[mod.BuffType("GlacialState")] = true;
						npc.buffImmune[mod.BuffType("TemporalSadness")] = true;

						if (CalamityWorld.death)
							npc.lifeMax = (int)((double)npc.lifeMax * 2.3);
						else
							npc.lifeMax = (int)((double)npc.lifeMax * 1.5);
					}
					else if (npc.type == thorium.NPCType("SlagFury") || npc.type == thorium.NPCType("Omnicide") || npc.type == thorium.NPCType("RealityBreaker") ||
						npc.type == thorium.NPCType("Aquaius") || npc.type == thorium.NPCType("Aquaius2"))
					{
						npc.buffImmune[mod.BuffType("GlacialState")] = true;
						npc.buffImmune[mod.BuffType("TemporalSadness")] = true;

						if (CalamityWorld.death)
							npc.lifeMax = (int)((double)npc.lifeMax * 1.75);
						else
							npc.lifeMax = (int)((double)npc.lifeMax * 1.3);
					}
				}
			}
		}
		#endregion

		#region Can Be Hit By
		public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
		{
			if (npc.type == NPCID.TargetDummy || npc.type == mod.NPCType("SuperDummy"))
				return !CalamityPlayer.areThereAnyDamnBosses;

			return null;
		}

		public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
		{
			if (npc.type == NPCID.TargetDummy || npc.type == mod.NPCType("SuperDummy"))
				return !CalamityPlayer.areThereAnyDamnBosses;

			return null;
		}
		#endregion

		#region Can Hit Player
		public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
		{
			if (Main.pumpkinMoon && CalamityWorld.downedDoG && !npc.boss && !npc.friendly && !npc.dontTakeDamage)
				cooldownSlot = 1;
			else if (Main.snowMoon && CalamityWorld.downedDoG && !npc.boss && !npc.friendly && !npc.dontTakeDamage)
				cooldownSlot = 1;
			else if (Main.eclipse && CalamityWorld.buffedEclipse && !npc.boss && !npc.friendly && !npc.dontTakeDamage)
				cooldownSlot = 1;

			if (npc.type == NPCID.BrainofCthulhu)
				return npc.ai[0] < 0f;

			return true;
		}
		#endregion

		#region Modify Hit Player
		public override void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit)
		{
			if (tSad)
				damage /= 2;

			if (target.GetModPlayer<CalamityPlayer>(mod).beeResist)
			{
				if (CalamityMod.beeEnemyList.Contains(npc.type))
					damage = (int)((double)damage * 0.75);
			}
		}
		#endregion

		#region Strike NPC
		public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
		{
			if (npc.type == NPCID.TheDestroyer || npc.type == NPCID.TheDestroyerBody || npc.type == NPCID.TheDestroyerTail ||
				npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail)
			{
				if (newAI[1] < 480f || newAI[2] > 0f)
					damage *= 0.01;
			}
			else if (npc.type == mod.NPCType("SCalWormBody") || npc.type == mod.NPCType("SCalWormHead") || npc.type == mod.NPCType("SCalWormBodyWeak") || npc.type == mod.NPCType("SCalWormTail") || npc.type == mod.NPCType("EidolonWyrmHeadHuge"))
				damage *= 0.000001;

			double yellowCandleDamageBoost = damage * 0.05; //get value before DR

			int newDefense = npc.defense -
					(pFlames ? 4 : 0) -
					(wDeath ? 50 : 0) -
					(gsInferno ? 20 : 0) -
					(aFlames ? 10 : 0) -
					(wCleave ? 15 : 0);

			if (gState)
				newDefense /= 2;
			if (aCrunch)
				newDefense /= 3;

			if (newDefense < 0)
				newDefense = 0;

			defense = newDefense;

			if (protection > 0f)
			{
				double newDamage = damage + ((double)defense * 0.25); //defense damage boost 150 * .25 = 45 + 150 = 195 damage  180 defense

				if (marked)
					protection *= 0.5f;
				if (npc.betsysCurse)
					protection *= 0.66f;
				if (wCleave)
					protection *= 0.75f;

				if (npc.ichor)
					protection *= 0.75f;
				else if (npc.onFire2)
					protection *= 0.8f;

				if (protection < 0f)
					protection = 0f;

				if (newDamage >= 1.0)
				{
					newDamage = (double)(1f - protection) * newDamage; //DR calc 195 * 0.4 = 78 damage 0.6 DR

					if (newDamage < 1.0)
						newDamage = 1.0;
				}

				damage = newDamage;
				protection = defProtection;
			}

			if (protection < 0.99f)
			{
				if (yellowCandle)
					damage += yellowCandleDamageBoost;
			}

			return true; //vanilla defense calc 78 - (180 / 2 = 90) = 0, boosted to 1 by calc
		}
		#endregion

		#region Pre AI
		public override bool PreAI(NPC npc)
		{
			SetPatreonTownNPCName(npc);

			if (npc.type == NPCID.TargetDummy || npc.type == mod.NPCType("SuperDummy"))
				npc.chaseable = !CalamityPlayer.areThereAnyDamnBosses;

			if (npc.type == NPCID.TheDestroyer || npc.type == NPCID.TheDestroyerBody || npc.type == NPCID.TheDestroyerTail ||
				npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail)
			{
				if (npc.buffImmune[mod.BuffType("Enraged")])
					npc.buffImmune[mod.BuffType("Enraged")] = false;

				if (npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail)
				{
					if (newAI[3] == 0f)
					{
						newAI[3] = 1f;
						newAI[1] = 300f;
					}
				}
				if (newAI[1] < 480f)
					newAI[1] += 1f;
			}

			if (npc.type == NPCID.Bee || npc.type == NPCID.BeeSmall || npc.type == NPCID.Hornet || npc.type == NPCID.HornetFatty || npc.type == NPCID.HornetHoney ||
				npc.type == NPCID.HornetLeafy || npc.type == NPCID.HornetSpikey || npc.type == NPCID.HornetStingy || npc.type == NPCID.BigHornetStingy || npc.type == NPCID.LittleHornetStingy ||
				npc.type == NPCID.BigHornetSpikey || npc.type == NPCID.LittleHornetSpikey || npc.type == NPCID.BigHornetLeafy || npc.type == NPCID.LittleHornetLeafy ||
				npc.type == NPCID.BigHornetHoney || npc.type == NPCID.LittleHornetHoney || npc.type == NPCID.BigHornetFatty || npc.type == NPCID.LittleHornetFatty)
			{
				if (Main.player[npc.target].GetModPlayer<CalamityPlayer>(mod).queenBeeLore)
				{
					CalamityGlobalAI.QueenBeeLoreEffect(npc);
					return false;
				}
			}

			if (CalamityWorld.bossRushActive && !npc.friendly && !npc.townNPC)
			{
				BossRushForceDespawnOtherNPCs(npc, mod);
				switch (npc.type)
				{
					case NPCID.KingSlime:
						return CalamityGlobalAI.BossRushKingSlimeAI(npc, enraged);
					case NPCID.BrainofCthulhu:
						return CalamityGlobalAI.BossRushBrainofCthulhuAI(npc, enraged, mod);
					case NPCID.EaterofWorldsHead:
						return CalamityGlobalAI.BossRushEaterofWorldsAI(npc, enraged, mod);
					case NPCID.QueenBee:
						return CalamityGlobalAI.BossRushQueenBeeAI(npc, enraged);
					case NPCID.DukeFishron:
						return CalamityGlobalAI.BossRushDukeFishronAI(npc, enraged, mod);
					default:
						break;
				}
			}

			if (CalamityWorld.revenge || CalamityWorld.bossRushActive)
			{
				switch (npc.type)
				{
					case NPCID.QueenBee:
						return CalamityGlobalAI.BuffedQueenBeeAI(npc, enraged, mod);
						break;
					case NPCID.TheDestroyer:
					case NPCID.TheDestroyerBody:
					case NPCID.TheDestroyerTail:
						return CalamityGlobalAI.BuffedDestroyerAI(npc, enraged, mod);
						break;
					case NPCID.Mothron:
						if (CalamityWorld.buffedEclipse)
							return CalamityGlobalAI.BuffedMothronAI(npc);
						break;
					case NPCID.Pumpking:
						if (CalamityWorld.downedDoG)
							return CalamityGlobalAI.BuffedPumpkingAI(npc);
						break;
					case NPCID.PumpkingBlade:
						if (CalamityWorld.downedDoG)
							return CalamityGlobalAI.BuffedPumpkingBladeAI(npc);
						break;
					case NPCID.IceQueen:
						if (CalamityWorld.downedDoG)
							return CalamityGlobalAI.BuffedIceQueenAI(npc);
						break;
					case NPCID.EyeofCthulhu:
						if (CalamityWorld.revenge || CalamityWorld.bossRushActive)
							return CalamityGlobalAI.BuffedEyeofCthulhuAI(npc, enraged);
						break;
					default:
						break;
				}
			}
			return true;
		}
		#endregion

		#region Set Patreon Town NPC Name
		private void SetPatreonTownNPCName(NPC npc)
		{
			if (setNewName)
			{
				setNewName = false;
				switch (npc.type)
				{
					case NPCID.Guide:
						switch (Main.rand.Next(36)) //34 guide names
						{
							case 0:
								npc.GivenName = "Lapp";
								break;
							case 1:
								npc.GivenName = "Ben Shapiro";
								break;
							default:
								break;
						}
						break;
					case NPCID.Wizard:
						switch (Main.rand.Next(24)) //23 wizard names
						{
							case 0:
								npc.GivenName = "Mage One-Trick";
								break;
							default:
								break;
						}
						break;
					case NPCID.Steampunker:
						switch (Main.rand.Next(22)) //21 steampunker names
						{
							case 0:
								npc.GivenName = "Vorbis";
								break;
							default:
								break;
						}
						break;
					case NPCID.Stylist:
						switch (Main.rand.Next(21)) //20 stylist names
						{
							case 0:
								npc.GivenName = "Amber";
								break;
							default:
								break;
						}
						break;
					case NPCID.WitchDoctor:
						switch (Main.rand.Next(11)) //10 witch doctor names
						{
							case 0:
								npc.GivenName = "Sok'ar";
								break;
							default:
								break;
						}
						break;
					default:
						break;
				}
			}
		}
		#endregion

		#region Boss Rush Force Despawn Other NPCs
		private void BossRushForceDespawnOtherNPCs(NPC npc, Mod mod)
		{
			switch (CalamityWorld.bossRushStage)
			{
				case 0:
					if (npc.type != NPCID.QueenBee && npc.type != NPCID.Bee && npc.type != NPCID.BeeSmall)
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 1:
					if (npc.type != NPCID.BrainofCthulhu && npc.type != NPCID.Creeper)
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 2:
					if (npc.type != NPCID.KingSlime && npc.type != NPCID.BlueSlime && npc.type != NPCID.SlimeSpiked)
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 3:
					if (npc.type != NPCID.EyeofCthulhu && npc.type != NPCID.ServantofCthulhu)
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 4:
					if (npc.type != NPCID.SkeletronPrime && npc.type != NPCID.PrimeSaw && npc.type != NPCID.PrimeVice &&
						npc.type != NPCID.PrimeCannon && npc.type != NPCID.PrimeLaser)
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 5:
					if (npc.type != NPCID.Golem && npc.type != NPCID.GolemFistLeft && npc.type != NPCID.GolemFistRight &&
						npc.type != NPCID.GolemHead && npc.type != NPCID.GolemHeadFree)
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 6:
					if (npc.type != mod.NPCType("ProfanedGuardianBoss") && npc.type != mod.NPCType("ProfanedGuardianBoss2") &&
						npc.type != mod.NPCType("ProfanedGuardianBoss3"))
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 7:
					if (npc.type != NPCID.EaterofWorldsHead && npc.type != NPCID.EaterofWorldsBody && npc.type != NPCID.EaterofWorldsTail &&
						npc.type != NPCID.VileSpit)
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 8:
					if (npc.type != mod.NPCType("Astrageldon") && npc.type != mod.NPCType("AstralSlime"))
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 9:
					if (npc.type != NPCID.TheDestroyer && npc.type != NPCID.TheDestroyerBody && npc.type != NPCID.TheDestroyerTail &&
						npc.type != NPCID.Probe)
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 10:
					if (npc.type != NPCID.Spazmatism && npc.type != NPCID.Retinazer)
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 11:
					if (npc.type != mod.NPCType("Bumblefuck") && npc.type != mod.NPCType("Bumblefuck2") &&
						npc.type != NPCID.Spazmatism && npc.type != NPCID.Retinazer)
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 12:
					if (npc.type != NPCID.WallofFlesh && npc.type != NPCID.WallofFleshEye && npc.type != NPCID.TheHungry &&
						npc.type != NPCID.TheHungryII && npc.type != NPCID.LeechHead && npc.type != NPCID.LeechBody &&
						npc.type != NPCID.LeechTail)
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 13:
					if (npc.type != mod.NPCType("HiveMind") && npc.type != mod.NPCType("HiveMindP2") &&
						npc.type != mod.NPCType("DarkHeart") && npc.type != mod.NPCType("HiveBlob") &&
						npc.type != mod.NPCType("DankCreeper") && npc.type != mod.NPCType("HiveBlob2"))
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 14:
					if (npc.type != NPCID.SkeletronHead && npc.type != NPCID.SkeletronHand)
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 15:
					if (npc.type != mod.NPCType("StormWeaverHead") && npc.type != mod.NPCType("StormWeaverBody") &&
						npc.type != mod.NPCType("StormWeaverTail") && npc.type != mod.NPCType("StormWeaverHeadNaked") &&
						npc.type != mod.NPCType("StormWeaverBodyNaked") && npc.type != mod.NPCType("StormWeaverTailNaked") &&
						npc.type != mod.NPCType("StasisProbe") && npc.type != mod.NPCType("StasisProbeNaked"))
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 16:
					if (npc.type != mod.NPCType("AquaticScourgeHead") && npc.type != mod.NPCType("AquaticScourgeBody") &&
						npc.type != mod.NPCType("AquaticScourgeBodyAlt") && npc.type != mod.NPCType("AquaticScourgeTail") &&
						npc.type != mod.NPCType("AquaticParasite") && npc.type != mod.NPCType("AquaticUrchin") &&
						npc.type != mod.NPCType("AquaticSeekerHead") && npc.type != mod.NPCType("AquaticSeekerBody") &&
						npc.type != mod.NPCType("AquaticSeekerTail"))
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 17:
					if (npc.type != mod.NPCType("DesertScourgeHead") && npc.type != mod.NPCType("DesertScourgeBody") &&
						npc.type != mod.NPCType("DesertScourgeTail") && npc.type != mod.NPCType("DesertScourgeHeadSmall") &&
						npc.type != mod.NPCType("DesertScourgeBodySmall") && npc.type != mod.NPCType("DesertScourgeTailSmall") &&
						npc.type != mod.NPCType("DriedSeekerHead") && npc.type != mod.NPCType("DriedSeekerBody") &&
						npc.type != mod.NPCType("DriedSeekerTail"))
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 18:
					if (npc.type != NPCID.CultistBoss && npc.type != NPCID.CultistBossClone && npc.type != NPCID.CultistDragonHead &&
						npc.type != NPCID.CultistDragonBody1 && npc.type != NPCID.CultistDragonBody2 && npc.type != NPCID.CultistDragonBody3 &&
						npc.type != NPCID.CultistDragonBody4 && npc.type != NPCID.CultistDragonTail && npc.type != NPCID.AncientCultistSquidhead &&
						npc.type != NPCID.AncientLight && npc.type != NPCID.AncientDoom && npc.type != mod.NPCType("Eidolist"))
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 19:
					if (npc.type != mod.NPCType("CrabulonIdle") && npc.type != mod.NPCType("CrabShroom"))
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 20:
					if (npc.type != NPCID.Plantera && npc.type != NPCID.PlanterasTentacle && npc.type != NPCID.PlanterasHook &&
						npc.type != NPCID.Spore)
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 21:
					if (npc.type != mod.NPCType("CeaselessVoid") && npc.type != mod.NPCType("DarkEnergy") &&
						npc.type != mod.NPCType("DarkEnergy2") && npc.type != mod.NPCType("DarkEnergy3"))
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 22:
					if (npc.type != mod.NPCType("PerforatorHive") && npc.type != mod.NPCType("PerforatorHeadLarge") &&
						npc.type != mod.NPCType("PerforatorBodyLarge") && npc.type != mod.NPCType("PerforatorTailLarge") &&
						npc.type != mod.NPCType("PerforatorHeadMedium") && npc.type != mod.NPCType("PerforatorBodyMedium") &&
						npc.type != mod.NPCType("PerforatorTailMedium") && npc.type != mod.NPCType("PerforatorHeadSmall") &&
						npc.type != mod.NPCType("PerforatorBodySmall") && npc.type != mod.NPCType("PerforatorTailSmall"))
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 23:
					if (npc.type != mod.NPCType("Cryogen") && npc.type != mod.NPCType("CryogenIce") &&
						npc.type != mod.NPCType("IceMass") && npc.type != mod.NPCType("Cryocore") &&
						npc.type != mod.NPCType("Cryocore2"))
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 24:
					if (npc.type != mod.NPCType("BrimstoneElemental") && npc.type != mod.NPCType("Brimling"))
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 25:
					if (npc.type != mod.NPCType("CosmicWraith") && npc.type != mod.NPCType("SignusBomb") &&
						npc.type != mod.NPCType("CosmicLantern"))
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 26:
					if (npc.type != mod.NPCType("ScavengerBody") && npc.type != mod.NPCType("ScavengerHead") &&
						npc.type != mod.NPCType("ScavengerClawLeft") && npc.type != mod.NPCType("ScavengerClawRight") &&
						npc.type != mod.NPCType("ScavengerLegLeft") && npc.type != mod.NPCType("ScavengerLegRight") &&
						npc.type != mod.NPCType("ScavengerHead2") && npc.type != mod.NPCType("RockPillar") &&
						npc.type != mod.NPCType("FlamePillar"))
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 27:
					if (npc.type != NPCID.DukeFishron && npc.type != NPCID.DetonatingBubble && npc.type != NPCID.Sharkron &&
						npc.type != NPCID.Sharkron2)
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 28:
					if (npc.type != NPCID.MoonLordCore && npc.type != NPCID.MoonLordHead && npc.type != NPCID.MoonLordHand &&
						npc.type != NPCID.MoonLordFreeEye && npc.type != mod.NPCType("Eidolist"))
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 29:
					if (npc.type != mod.NPCType("AstrumDeusHead") && npc.type != mod.NPCType("AstrumDeusBody") &&
						npc.type != mod.NPCType("AstrumDeusTail") && npc.type != mod.NPCType("AstrumDeusHeadSpectral") &&
						npc.type != mod.NPCType("AstrumDeusBodySpectral") && npc.type != mod.NPCType("AstrumDeusTailSpectral") &&
						npc.type != mod.NPCType("AstrumDeusProbe") && npc.type != mod.NPCType("AstrumDeusProbe2") &&
						npc.type != mod.NPCType("AstrumDeusProbe3"))
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 30:
					if (npc.type != mod.NPCType("Polterghast") && npc.type != mod.NPCType("PhantomFuckYou") &&
						npc.type != mod.NPCType("PolterghastHook") && npc.type != mod.NPCType("PolterPhantom"))
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 31:
					if (npc.type != mod.NPCType("PlaguebringerGoliath") && npc.type != mod.NPCType("PlagueBeeG") &&
						npc.type != mod.NPCType("PlagueBeeLargeG") && npc.type != mod.NPCType("PlagueHomingMissile") &&
						npc.type != mod.NPCType("PlagueMine") && npc.type != mod.NPCType("PlaguebringerShade"))
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 32:
					if (npc.type != mod.NPCType("Calamitas") && npc.type != mod.NPCType("CalamitasRun") &&
						npc.type != mod.NPCType("CalamitasRun2") && npc.type != mod.NPCType("CalamitasRun3") &&
						npc.type != mod.NPCType("LifeSeeker") && npc.type != mod.NPCType("SoulSeeker"))
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 33:
					if (npc.type != mod.NPCType("Siren") && npc.type != mod.NPCType("Leviathan") &&
						npc.type != mod.NPCType("AquaticAberration") && npc.type != mod.NPCType("Parasea") &&
						npc.type != mod.NPCType("SirenClone") && npc.type != mod.NPCType("SirenIce"))
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 34:
					if (npc.type != mod.NPCType("SlimeGod") && npc.type != mod.NPCType("SlimeGodRun") &&
						npc.type != mod.NPCType("SlimeGodCore") && npc.type != mod.NPCType("SlimeGodSplit") &&
						npc.type != mod.NPCType("SlimeGodRunSplit") && npc.type != mod.NPCType("SlimeSpawnCorrupt") &&
						npc.type != mod.NPCType("SlimeSpawnCorrupt2") && npc.type != mod.NPCType("SlimeSpawnCrimson") &&
						npc.type != mod.NPCType("SlimeSpawnCrimson2"))
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 35:
					if (npc.type != mod.NPCType("Providence") && npc.type != mod.NPCType("ProvSpawnDefense") &&
						npc.type != mod.NPCType("ProvSpawnOffense") && npc.type != mod.NPCType("ProvSpawnHealer"))
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 36:
					if (npc.type != mod.NPCType("SupremeCalamitas") && npc.type != mod.NPCType("SCalWormBody") &&
						npc.type != mod.NPCType("SCalWormBodyWeak") && npc.type != mod.NPCType("SCalWormHead") &&
						npc.type != mod.NPCType("SCalWormTail") && npc.type != mod.NPCType("SoulSeekerSupreme") &&
						npc.type != mod.NPCType("SCalWormHeart") && npc.type != mod.NPCType("SupremeCataclysm") &&
						npc.type != mod.NPCType("SupremeCatastrophe"))
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 37:
					if (npc.type != mod.NPCType("Yharon") && npc.type != mod.NPCType("DetonatingFlare") &&
						npc.type != mod.NPCType("DetonatingFlare2") && npc.type != mod.NPCType("DetonatingFlare3") &&
						npc.type != mod.NPCType("Bumblefuck3") && npc.type != mod.NPCType("Bumblefuck4"))
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
				case 38:
					if (npc.type != mod.NPCType("DevourerofGodsHeadS") && npc.type != mod.NPCType("DevourerofGodsBodyS") &&
						npc.type != mod.NPCType("DevourerofGodsTailS"))
					{
						npc.active = false;
						npc.netUpdate = true;
					}
					break;
			}
		}
		#endregion

		#region AI
		public override void AI(NPC npc)
		{
			if (!CalamityWorld.spawnedHardBoss)
			{
				if (npc.type == NPCID.TheDestroyer || npc.type == NPCID.Spazmatism || npc.type == NPCID.Retinazer || npc.type == NPCID.SkeletronPrime ||
					npc.type == NPCID.Plantera || npc.type == mod.NPCType("Cryogen") || npc.type == mod.NPCType("AquaticScourgeHead") ||
					npc.type == mod.NPCType("BrimstoneElemental") || npc.type == mod.NPCType("Astrageldon") || npc.type == mod.NPCType("AstrumDeusHeadSpectral") ||
					npc.type == mod.NPCType("Calamitas") || npc.type == mod.NPCType("Siren") || npc.type == mod.NPCType("PlaguebringerGoliath") ||
					npc.type == mod.NPCType("ScavengerBody") || npc.type == NPCID.DukeFishron || npc.type == NPCID.CultistBoss || npc.type == NPCID.Golem)
				{
					CalamityWorld.spawnedHardBoss = true;
					CalamityMod.UpdateServerBoolean();
				}
			}

			if (CalamityWorld.revenge || CalamityWorld.bossRushActive)
			{
				bool configBossRushBoost = Config.BossRushXerocCurse && CalamityWorld.bossRushActive;

				switch (npc.type)
				{
					case NPCID.MoonLordFreeEye:
						CalamityGlobalAI.RevengeanceMoonLordFreeEyeAI(npc);
						break;
					case NPCID.MoonLordCore:
						CalamityGlobalAI.RevengeanceMoonLordCoreAI(npc);
						break;
					case NPCID.MoonLordHand:
					case NPCID.MoonLordHead:
						CalamityGlobalAI.RevengeanceMoonLordHandAI(npc, mod);
						break;
					case NPCID.CultistBoss:
						CalamityGlobalAI.RevengeanceCultistAI(npc, configBossRushBoost, mod, enraged);
						break;
					case NPCID.DukeFishron:
						CalamityGlobalAI.RevengeanceDukeFishronAI(npc, mod);
						break;
					case NPCID.GolemHeadFree:
						if (!CalamityWorld.bossRushActive)
							CalamityGlobalAI.RevengeanceGolemHeadFreeAI(npc);
						break;
					case NPCID.GolemHead:
						if (!CalamityWorld.bossRushActive)
							CalamityGlobalAI.RevengeanceGolemHeadAI(npc);
						break;
					case NPCID.Plantera:
						CalamityGlobalAI.RevengeancePlanteraAI(npc, configBossRushBoost, mod, enraged);
						break;
					case NPCID.PlanterasTentacle:
						CalamityGlobalAI.RevengeancePlanterasTentacleAI(npc, mod);
						break;
					case NPCID.SkeletronPrime:
						CalamityGlobalAI.RevengeanceSkeletronPrimeAI(npc, configBossRushBoost, mod, enraged);
						break;
					case NPCID.PrimeLaser:
						CalamityGlobalAI.RevengeancePrimeLaserAI(npc);
						break;
					case NPCID.PrimeCannon:
						CalamityGlobalAI.RevengeancePrimeCannonAI(npc, configBossRushBoost, enraged);
						break;
					case NPCID.Retinazer:
						CalamityGlobalAI.RevengeanceRetinazerAI(npc, configBossRushBoost, mod, enraged);
						break;
					case NPCID.Spazmatism:
						CalamityGlobalAI.RevengeanceSpazmatismAI(npc, configBossRushBoost, mod, enraged);
						break;
					case NPCID.WallofFlesh:
						CalamityGlobalAI.RevengeanceWallofFleshAI(npc, configBossRushBoost, enraged);
						break;
					case NPCID.WallofFleshEye:
						CalamityGlobalAI.RevengeanceWallofFleshEyeAI(npc, configBossRushBoost, mod, enraged);
						break;
					case NPCID.SkeletronHand:
						CalamityGlobalAI.RevengeanceSkeletronHandAI(npc, configBossRushBoost, enraged);
						break;
					case NPCID.SkeletronHead:
						CalamityGlobalAI.RevengeanceSkeletronAI(npc, configBossRushBoost, enraged);
						break;
					case NPCID.DungeonGuardian:
						CalamityGlobalAI.RevengeanceSkeletronAI(npc, configBossRushBoost, enraged);
						break;
					case NPCID.BrainofCthulhu:
						CalamityGlobalAI.RevengeanceBrainofCthulhuAI(npc);
						break;
					case NPCID.Creeper:
						CalamityGlobalAI.RevengeanceCreeperAI(npc);
						break;
					case NPCID.EaterofWorldsHead:
						if (!CalamityWorld.bossRushActive)
							CalamityGlobalAI.RevengeanceEaterofWorldsAI(npc, mod);
						break;
					case NPCID.KingSlime:
						CalamityGlobalAI.RevengeanceKingSlimeAI(npc);
						break;
					case NPCID.Lihzahrd:
						CalamityGlobalAI.RevengeanceLihzahrdAI(npc);
						break;
					case NPCID.IceGolem:
						CalamityGlobalAI.RevengeanceIceGolemAI(npc);
						break;
					default:
						break;
				}
			}
		}
		#endregion

		#region Post AI
		public override void PostAI(NPC npc)
		{
			if (pearlAura && !CalamityPlayer.areThereAnyDamnBosses)
			{
				npc.velocity.X *= 0.95f;
				npc.velocity.Y *= 0.95f;
			}
			if (silvaStun && !CalamityWorld.bossRushActive)
			{
				npc.velocity.X = 0f;
				npc.velocity.Y = 0f;
			}
		}
		#endregion

		#region On Hit Player
		public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
		{
			if (target.GetModPlayer<CalamityPlayer>(mod).snowman)
			{
				if (npc.type == NPCID.Demon || npc.type == NPCID.VoodooDemon || npc.type == NPCID.RedDevil)
					target.AddBuff(mod.BuffType("PopoNoseless"), 36000);
			}

			switch (npc.type)
			{
				case NPCID.GolemHead:
					target.AddBuff(mod.BuffType("ArmorCrunch"), 180);
					break;
				case NPCID.GolemHeadFree:
					target.AddBuff(mod.BuffType("ArmorCrunch"), 180);
					break;
				case NPCID.Golem:
					target.AddBuff(mod.BuffType("ArmorCrunch"), 300);
					break;
				case NPCID.GolemFistRight:
					target.AddBuff(mod.BuffType("ArmorCrunch"), 180);
					break;
				case NPCID.GolemFistLeft:
					target.AddBuff(mod.BuffType("ArmorCrunch"), 180);
					break;
				default:
					break;
			}

			if (CalamityWorld.revenge)
			{
				if (Config.RevengeanceAndDeathThoriumBossBuff)
				{
					Mod thorium = ModLoader.GetMod("ThoriumMod");
					if (thorium != null)
					{
						if (npc.type == thorium.NPCType("GraniteEnergyStorm") || npc.type == thorium.NPCType("TheBuriedWarrior") || npc.type == thorium.NPCType("TheBuriedWarrior1") ||
							npc.type == thorium.NPCType("TheBuriedWarrior2") || npc.type == thorium.NPCType("ThePrimeScouter") || npc.type == thorium.NPCType("CryoCore") ||
							npc.type == thorium.NPCType("BioCore") || npc.type == thorium.NPCType("PyroCore") || npc.type == thorium.NPCType("SlagFury") ||
							npc.type == thorium.NPCType("Omnicide") || npc.type == thorium.NPCType("RealityBreaker") || npc.type == thorium.NPCType("Aquaius") ||
							npc.type == thorium.NPCType("Aquaius2"))
						{
							target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
						}
						else if (npc.type == thorium.NPCType("ViscountBaby") || npc.type == thorium.NPCType("EnemyBeholder") || npc.type == thorium.NPCType("AbyssalSpawn") ||
							npc.type == thorium.NPCType("Viscount") || npc.type == thorium.NPCType("FallenDeathBeholder") || npc.type == thorium.NPCType("FallenDeathBeholder2") ||
							npc.type == thorium.NPCType("Lich") || npc.type == thorium.NPCType("LichHeadless") || npc.type == thorium.NPCType("Abyssion") ||
							npc.type == thorium.NPCType("AbyssionCracked") || npc.type == thorium.NPCType("AbyssionReleased"))
						{
							target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
							target.AddBuff(mod.BuffType("Horror"), 180);
						}
					}
				}

				switch (npc.type)
				{
					case NPCID.DemonEye:
						target.AddBuff(mod.BuffType("Horror"), 60);
						break;
					case NPCID.EyeofCthulhu:
						target.AddBuff(mod.BuffType("Horror"), 180);
						break;
					case NPCID.ServantofCthulhu:
						target.AddBuff(mod.BuffType("Horror"), 180);
						break;
					case NPCID.EaterofSouls:
						target.AddBuff(mod.BuffType("Horror"), 60);
						break;
					case NPCID.DevourerHead:
						target.AddBuff(mod.BuffType("Horror"), 120);
						break;
					case NPCID.EaterofWorldsHead:
						target.AddBuff(mod.BuffType("Horror"), 180);
						target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
						break;
					case NPCID.EaterofWorldsBody:
						target.AddBuff(mod.BuffType("Horror"), 120);
						break;
					case NPCID.EaterofWorldsTail:
						target.AddBuff(mod.BuffType("Horror"), 60);
						break;
					case NPCID.BurningSphere:
						target.AddBuff(mod.BuffType("MarkedforDeath"), 120);
						break;
					case NPCID.ChaosBall:
						target.AddBuff(mod.BuffType("Horror"), 60);
						break;
					case NPCID.CursedSkull:
						target.AddBuff(mod.BuffType("Horror"), 120);
						target.AddBuff(mod.BuffType("MarkedforDeath"), 120);
						break;
					case NPCID.SkeletronHead:
						target.AddBuff(mod.BuffType("Horror"), 180);
						break;
					case NPCID.SkeletronHand:
						target.AddBuff(mod.BuffType("Horror"), 120);
						target.AddBuff(mod.BuffType("MarkedforDeath"), 120);
						break;
					case NPCID.ManEater:
						target.AddBuff(mod.BuffType("MarkedforDeath"), 120);
						break;
					case NPCID.Snatcher:
						target.AddBuff(mod.BuffType("MarkedforDeath"), 60);
						break;
					case NPCID.CorruptBunny:
						target.AddBuff(mod.BuffType("Horror"), 120);
						break;
					case NPCID.CorruptGoldfish:
						target.AddBuff(mod.BuffType("Horror"), 120);
						break;
					case NPCID.Demon:
						target.AddBuff(mod.BuffType("Horror"), 180);
						break;
					case NPCID.VoodooDemon:
						target.AddBuff(mod.BuffType("Horror"), 180);
						break;
					case NPCID.DungeonGuardian:
						target.AddBuff(mod.BuffType("Horror"), 1200);
						break;
					case NPCID.Mummy:
						target.AddBuff(mod.BuffType("MarkedforDeath"), 120);
						break;
					case NPCID.DarkMummy:
						target.AddBuff(mod.BuffType("Horror"), 180);
						target.AddBuff(mod.BuffType("MarkedforDeath"), 120);
						break;
					case NPCID.CorruptSlime:
						target.AddBuff(mod.BuffType("Horror"), 120);
						break;
					case NPCID.Wraith:
						target.AddBuff(mod.BuffType("Horror"), 240);
						target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
						break;
					case NPCID.EnchantedSword:
						target.AddBuff(mod.BuffType("MarkedforDeath"), 120);
						break;
					case NPCID.Mimic:
						target.AddBuff(mod.BuffType("MarkedforDeath"), 240);
						break;
					case NPCID.Werewolf:
						target.AddBuff(mod.BuffType("MarkedforDeath"), 120);
						break;
					case NPCID.CursedHammer:
						target.AddBuff(mod.BuffType("Horror"), 180);
						break;
					case NPCID.Corruptor:
						target.AddBuff(mod.BuffType("Horror"), 120);
						break;
					case NPCID.SeekerHead:
						target.AddBuff(mod.BuffType("Horror"), 180);
						break;
					case NPCID.Clinger:
						target.AddBuff(mod.BuffType("Horror"), 120);
						break;
					case NPCID.VileSpit:
						target.AddBuff(mod.BuffType("Horror"), 60);
						break;
					case NPCID.WallofFlesh:
						target.AddBuff(mod.BuffType("Horror"), 300);
						break;
					case NPCID.TheHungry:
						target.AddBuff(mod.BuffType("Horror"), 120);
						target.AddBuff(mod.BuffType("MarkedforDeath"), 120);
						break;
					case NPCID.TheHungryII:
						target.AddBuff(mod.BuffType("Horror"), 120);
						target.AddBuff(mod.BuffType("MarkedforDeath"), 120);
						break;
					case NPCID.LeechHead:
						target.AddBuff(mod.BuffType("Horror"), 60);
						break;
					case NPCID.Slimer:
						target.AddBuff(mod.BuffType("Horror"), 120);
						break;
					case NPCID.WanderingEye:
						target.AddBuff(mod.BuffType("Horror"), 120);
						break;
					case NPCID.Spazmatism:
						target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
						break;
					case NPCID.PrimeSaw:
						target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
						break;
					case NPCID.PrimeVice:
						target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
						break;
					case NPCID.Probe:
						target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
						break;
					case NPCID.RedDevil:
						target.AddBuff(mod.BuffType("Horror"), 180);
						break;
					case NPCID.VampireBat:
						target.AddBuff(mod.BuffType("Horror"), 240);
						target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
						break;
					case NPCID.Vampire:
						target.AddBuff(mod.BuffType("Horror"), 240);
						target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
						break;
					case NPCID.Frankenstein:
						target.AddBuff(mod.BuffType("Horror"), 120);
						break;
					case NPCID.BlackRecluse:
						target.AddBuff(mod.BuffType("Horror"), 180);
						break;
					case NPCID.WallCreeper:
						target.AddBuff(mod.BuffType("Horror"), 120);
						break;
					case NPCID.WallCreeperWall:
						target.AddBuff(mod.BuffType("Horror"), 120);
						break;
					case NPCID.SwampThing:
						target.AddBuff(mod.BuffType("Horror"), 120);
						break;
					case NPCID.AngryTrapper:
						target.AddBuff(mod.BuffType("MarkedforDeath"), 240);
						break;
					case NPCID.CorruptPenguin:
						target.AddBuff(mod.BuffType("Horror"), 120);
						break;
					case NPCID.Crimera:
						target.AddBuff(mod.BuffType("Horror"), 60);
						break;
					case NPCID.Herpling:
						target.AddBuff(mod.BuffType("Horror"), 120);
						break;
					case NPCID.CrimsonAxe:
						target.AddBuff(mod.BuffType("Horror"), 180);
						break;
					case NPCID.FaceMonster:
						target.AddBuff(mod.BuffType("Horror"), 60);
						break;
					case NPCID.FloatyGross:
						target.AddBuff(mod.BuffType("Horror"), 120);
						break;
					case NPCID.Crimslime:
						target.AddBuff(mod.BuffType("Horror"), 120);
						break;
					case NPCID.Nymph:
						target.AddBuff(mod.BuffType("Horror"), 180);
						break;
					case NPCID.BlackRecluseWall:
						target.AddBuff(mod.BuffType("Horror"), 180);
						break;
					case NPCID.BloodCrawler:
						target.AddBuff(mod.BuffType("Horror"), 60);
						break;
					case NPCID.BloodCrawlerWall:
						target.AddBuff(mod.BuffType("Horror"), 60);
						break;
					case NPCID.BloodFeeder:
						target.AddBuff(mod.BuffType("Horror"), 120);
						break;
					case NPCID.QueenBee:
						target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
						break;
					case NPCID.GolemFistLeft:
						target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
						break;
					case NPCID.GolemFistRight:
						target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
						break;
					case NPCID.BloodJelly:
						target.AddBuff(mod.BuffType("Horror"), 120);
						break;
					case NPCID.Eyezor:
						target.AddBuff(mod.BuffType("Horror"), 180);
						break;
					case NPCID.Reaper:
						target.AddBuff(mod.BuffType("Horror"), 240);
						target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
						break;
					case NPCID.BrainofCthulhu:
						target.AddBuff(mod.BuffType("Horror"), 180);
						break;
					case NPCID.Creeper:
						target.AddBuff(mod.BuffType("Horror"), 60);
						target.AddBuff(mod.BuffType("MarkedforDeath"), 120);
						break;
					case NPCID.IchorSticker:
						target.AddBuff(mod.BuffType("Horror"), 120);
						break;
					case NPCID.DungeonSpirit:
						target.AddBuff(mod.BuffType("Horror"), 300);
						target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
						break;
					case NPCID.GiantCursedSkull:
						target.AddBuff(mod.BuffType("Horror"), 180);
						target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
						break;
					case NPCID.Spore:
						target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
						break;
					case NPCID.BoneLee:
						target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
						break;
					case NPCID.HeadlessHorseman:
						target.AddBuff(mod.BuffType("Horror"), 180);
						break;
					case NPCID.Ghost:
						target.AddBuff(mod.BuffType("Horror"), 120);
						break;
					case NPCID.MourningWood:
						target.AddBuff(mod.BuffType("Horror"), 240);
						break;
					case NPCID.Splinterling:
						target.AddBuff(mod.BuffType("Horror"), 180);
						break;
					case NPCID.Pumpking:
						target.AddBuff(mod.BuffType("Horror"), 240);
						break;
					case NPCID.PumpkingBlade:
						target.AddBuff(mod.BuffType("Horror"), 120);
						target.AddBuff(mod.BuffType("MarkedforDeath"), 120);
						break;
					case NPCID.Hellhound:
						target.AddBuff(mod.BuffType("Horror"), 180);
						break;
					case NPCID.Poltergeist:
						target.AddBuff(mod.BuffType("Horror"), 180);
						break;
					case NPCID.Krampus:
						target.AddBuff(mod.BuffType("Horror"), 300);
						break;
					case NPCID.Flocko:
						target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
						break;
					case NPCID.DetonatingBubble:
						target.AddBuff(mod.BuffType("MarkedforDeath"), 360);
						break;
					case NPCID.DukeFishron:
						target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
						break;
					case NPCID.Sharkron:
						target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
						break;
					case NPCID.Sharkron2:
						target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
						break;
					case NPCID.Butcher:
						target.AddBuff(mod.BuffType("Horror"), 180);
						target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
						break;
					case NPCID.CreatureFromTheDeep:
						target.AddBuff(mod.BuffType("Horror"), 120);
						break;
					case NPCID.Fritz:
						target.AddBuff(mod.BuffType("Horror"), 120);
						break;
					case NPCID.Nailhead:
						target.AddBuff(mod.BuffType("Horror"), 180);
						break;
					case NPCID.CrimsonBunny:
						target.AddBuff(mod.BuffType("Horror"), 120);
						break;
					case NPCID.CrimsonGoldfish:
						target.AddBuff(mod.BuffType("Horror"), 120);
						break;
					case NPCID.Psycho:
						target.AddBuff(mod.BuffType("Horror"), 180);
						target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
						break;
					case NPCID.DrManFly:
						target.AddBuff(mod.BuffType("Horror"), 180);
						break;
					case NPCID.ThePossessed:
						target.AddBuff(mod.BuffType("Horror"), 180);
						break;
					case NPCID.CrimsonPenguin:
						target.AddBuff(mod.BuffType("Horror"), 120);
						break;
					case NPCID.Mothron:
						target.AddBuff(mod.BuffType("Horror"), 180);
						break;
					case NPCID.ShadowFlameApparition:
						target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
						break;
					case NPCID.MartianDrone:
						target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
						break;
					case NPCID.MoonLordFreeEye:
						target.AddBuff(mod.BuffType("Horror"), 180);
						target.AddBuff(mod.BuffType("MarkedforDeath"), 180);
						break;
					case NPCID.Medusa:
						target.AddBuff(mod.BuffType("Horror"), 180);
						break;
					case NPCID.BloodZombie:
						target.AddBuff(mod.BuffType("Horror"), 120);
						target.AddBuff(mod.BuffType("MarkedforDeath"), 120);
						break;
					case NPCID.Drippler:
						target.AddBuff(mod.BuffType("Horror"), 120);
						break;
					case NPCID.AncientCultistSquidhead:
						target.AddBuff(mod.BuffType("Horror"), 240);
						target.AddBuff(mod.BuffType("MarkedforDeath"), 240);
						break;
					case NPCID.AncientDoom:
						target.AddBuff(mod.BuffType("Horror"), 300);
						target.AddBuff(mod.BuffType("MarkedforDeath"), 300);
						break;
					case NPCID.SandsharkCorrupt:
						target.AddBuff(mod.BuffType("Horror"), 180);
						break;
					case NPCID.SandsharkCrimson:
						target.AddBuff(mod.BuffType("Horror"), 180);
						break;
					default:
						break;
				}
			}
		}
		#endregion

		#region Modify Hit By Projectile
		public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
            if (npc.type == NPCID.TheDestroyerBody)
            {
				if (projectile.penetrate == -1 && !projectile.minion)
					damage = (int)((double)damage * 0.2);
				else if (projectile.penetrate > 1)
					damage /= projectile.penetrate;
			}

            if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).eGauntlet)
			{
				if (projectile.melee && ShouldAffectNPC(npc) && Main.rand.Next(15) == 0)
				{
					if (!CalamityPlayer.areThereAnyDamnBosses)
						damage = npc.lifeMax * 3;
				}
			}
			if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).eTalisman)
			{
				if (projectile.magic && ShouldAffectNPC(npc) && Main.rand.Next(15) == 0)
				{
					if (!CalamityPlayer.areThereAnyDamnBosses)
						damage = npc.lifeMax * 3;
				}
			}
			if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).nanotech)
			{
				if (projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).rogue && ShouldAffectNPC(npc) && Main.rand.Next(15) == 0)
				{
					if (!CalamityPlayer.areThereAnyDamnBosses)
						damage = npc.lifeMax * 3;
				}
			}
			if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).eQuiver)
			{
				if (projectile.ranged && ShouldAffectNPC(npc) && Main.rand.Next(15) == 0)
				{
					if (!CalamityPlayer.areThereAnyDamnBosses)
						damage = npc.lifeMax * 3;
				}
			}
			if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).statisBeltOfCurses)
			{
				if ((projectile.minion || CalamityMod.projectileMinionList.Contains(projectile.type)) && ShouldAffectNPC(npc) && Main.rand.Next(15) == 0)
				{
					if (!CalamityPlayer.areThereAnyDamnBosses)
						damage = npc.lifeMax * 3;
				}
			}
		}
		#endregion

		#region On Hit By Item
		public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit)
		{
			if (player.GetModPlayer<CalamityPlayer>(mod).bloodflareSet)
			{
				if (!npc.SpawnedFromStatue && npc.damage > 0 && ((double)npc.life < (double)npc.lifeMax * 0.5) &&
					player.GetModPlayer<CalamityPlayer>(mod).bloodflareHeartTimer <= 0)
				{
					player.GetModPlayer<CalamityPlayer>(mod).bloodflareHeartTimer = 180;
                    DropHelper.DropItem(npc, ItemID.Heart);
				}
				else if (!npc.SpawnedFromStatue && npc.damage > 0 && ((double)npc.life > (double)npc.lifeMax * 0.5) &&
					player.GetModPlayer<CalamityPlayer>(mod).bloodflareManaTimer <= 0)
				{
					player.GetModPlayer<CalamityPlayer>(mod).bloodflareManaTimer = 180;
                    DropHelper.DropItem(npc, ItemID.Star);
				}
			}
		}
		#endregion

		#region On Hit By Projectile
		public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
		{
			if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).sGenerator)
			{
				if (projectile.minion && npc.damage > 0)
				{
					switch (Main.rand.Next(3))
					{
						case 0:
							Main.player[projectile.owner].AddBuff(mod.BuffType("SpiritGeneratorAtkBuff"), 120);
							break;
						case 1:
							Main.player[projectile.owner].AddBuff(mod.BuffType("SpiritGeneratorRegenBuff"), 120);
							break;
						case 2:
							Main.player[projectile.owner].AddBuff(mod.BuffType("SpiritGeneratorDefBuff"), 120);
							break;
						default:
							break;
					}
				}
			}

			if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).bloodflareSet)
			{
				if (!npc.SpawnedFromStatue && npc.damage > 0 && ((double)npc.life < (double)npc.lifeMax * 0.5) &&
					Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).bloodflareHeartTimer <= 0)
				{
					Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).bloodflareHeartTimer = 180;
                    DropHelper.DropItem(npc, ItemID.Heart);
				}
				else if (!npc.SpawnedFromStatue && npc.damage > 0 && ((double)npc.life > (double)npc.lifeMax * 0.5) &&
					Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).bloodflareManaTimer <= 0)
				{
					Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).bloodflareManaTimer = 180;
                    DropHelper.DropItem(npc, ItemID.Star);
				}
			}
		}
		#endregion

		#region Check Dead
		public override bool CheckDead(NPC npc)
		{
			if (npc.lifeMax > 1000 && npc.type != 288 &&
				npc.type != mod.NPCType("PhantomSpirit") &&
				npc.type != mod.NPCType("PhantomSpiritS") &&
				npc.type != mod.NPCType("PhantomSpiritM") &&
				npc.type != mod.NPCType("PhantomSpiritL") &&
				npc.value > 0f && npc.HasPlayerTarget &&
				NPC.downedMoonlord &&
				Main.player[npc.target].ZoneDungeon)
			{
				int maxValue = Main.expertMode ? 4 : 6;

				if (Main.rand.Next(maxValue) == 0 && Main.wallDungeon[(int)Main.tile[(int)npc.Center.X / 16, (int)npc.Center.Y / 16].wall])
				{
					int randomType = Main.rand.Next(4);
					switch (randomType)
					{
						case 0:
							randomType = mod.NPCType("PhantomSpirit");
							break;
						case 1:
							randomType = mod.NPCType("PhantomSpiritS");
							break;
						case 2:
							randomType = mod.NPCType("PhantomSpiritM");
							break;
						case 3:
							randomType = mod.NPCType("PhantomSpiritL");
							break;
						default:
							break;
					}
					NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, randomType, 0, 0f, 0f, 0f, 0f, 255);
				}
			}
			return true;
		}
		#endregion

		#region Hit Effect
		public override void HitEffect(NPC npc, int hitDirection, double damage)
		{
			if (CalamityWorld.revenge)
			{
				switch (npc.type)
				{
					case NPCID.MotherSlime:
						if (npc.life <= 0)
						{
							if (Main.netMode != 1)
							{
								int num261 = Main.rand.Next(2) + 2;
								int num;
								for (int num262 = 0; num262 < num261; num262 = num + 1)
								{
									int num263 = NPC.NewNPC((int)(npc.position.X + (float)(npc.width / 2)), (int)(npc.position.Y + (float)npc.height), 1, 0, 0f, 0f, 0f, 0f, 255);
									Main.npc[num263].SetDefaults(-5, -1f);
									Main.npc[num263].velocity.X = npc.velocity.X * 2f;
									Main.npc[num263].velocity.Y = npc.velocity.Y;
									NPC var_324_BB1A_cp_0_cp_0 = Main.npc[num263];
									var_324_BB1A_cp_0_cp_0.velocity.X = var_324_BB1A_cp_0_cp_0.velocity.X + ((float)Main.rand.Next(-20, 20) * 0.1f + (float)(num262 * npc.direction) * 0.3f);
									NPC var_324_BB6F_cp_0_cp_0 = Main.npc[num263];
									var_324_BB6F_cp_0_cp_0.velocity.Y = var_324_BB6F_cp_0_cp_0.velocity.Y - ((float)Main.rand.Next(0, 10) * 0.1f + (float)num262);
									Main.npc[num263].ai[0] = (float)(-1000 * Main.rand.Next(3));

									if (Main.netMode == 2 && num263 < 200)
										NetMessage.SendData(23, -1, -1, null, num263, 0f, 0f, 0f, 0, 0, 0);

									num = num262;
								}
							}
						}
						break;
					case NPCID.Demon:
					case NPCID.VoodooDemon:
						npc.ai[0] += 1f;
						break;
					case NPCID.CursedHammer:
					case NPCID.EnchantedSword:
					case NPCID.Clinger:
					case NPCID.Gastropod:
					case NPCID.GiantTortoise:
					case NPCID.IceTortoise:
					case NPCID.BlackRecluse:
					case NPCID.BlackRecluseWall:
					case NPCID.CrimsonAxe:
					case NPCID.Paladin:
						npc.justHit = false;
						break;
					case NPCID.Clown:
						if (Main.netMode != 1 && !Main.player[npc.target].dead)
							npc.ai[2] += 29f;
						break;
				}
			}
		}
		#endregion

		#region Edit Spawn Rate
		public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
		{
			if (player.GetModPlayer<CalamityPlayer>(mod).ZoneSulphur)
			{
				spawnRate = (int)((double)spawnRate * 1.1);
				maxSpawns = (int)((float)maxSpawns * 0.8f);
				if (Main.raining)
				{
					spawnRate = (int)((double)spawnRate * 0.7);
					maxSpawns = (int)((float)maxSpawns * 1.2f);
				}
			}
			else if (player.GetModPlayer<CalamityPlayer>(mod).ZoneAbyss)
			{
				spawnRate = (int)((double)spawnRate * 0.7);
				maxSpawns = (int)((float)maxSpawns * 1.1f);
			}
			else if (player.GetModPlayer<CalamityPlayer>(mod).ZoneCalamity)
			{
				spawnRate = (int)((double)spawnRate * 0.9);
				maxSpawns = (int)((float)maxSpawns * 1.1f);
			}
			else if (player.GetModPlayer<CalamityPlayer>(mod).ZoneAstral)
			{
				spawnRate = (int)((double)spawnRate * 0.6);
				maxSpawns = (int)((float)maxSpawns * 1.2f);
			}
			else if (player.GetModPlayer<CalamityPlayer>(mod).ZoneSunkenSea)
			{
				spawnRate = (int)((double)spawnRate * 0.9);
				maxSpawns = (int)((float)maxSpawns * 1.1f);
			}

			if (CalamityWorld.revenge)
				spawnRate = (int)((double)spawnRate * 0.85);
			if (CalamityWorld.death)
				spawnRate = (int)((double)spawnRate * 0.75);
			if (CalamityWorld.demonMode)
				spawnRate = (int)((double)spawnRate * 0.75);

			if (player.GetModPlayer<CalamityPlayer>(mod).clamity)
			{
				spawnRate = (int)((double)spawnRate * 0.02);
				maxSpawns = (int)((float)maxSpawns * 1.5f);
			}

			if (player.GetModPlayer<CalamityPlayer>(mod).zerg && player.GetModPlayer<CalamityPlayer>(mod).chaosCandle)
			{
				spawnRate = (int)((double)spawnRate * 0.005);
				maxSpawns = (int)((float)maxSpawns * 7.5f);
			}
			else if (player.GetModPlayer<CalamityPlayer>(mod).zerg)
			{
				spawnRate = (int)((double)spawnRate * 0.01);
				maxSpawns = (int)((float)maxSpawns * 5f);
			}
			else if (player.GetModPlayer<CalamityPlayer>(mod).chaosCandle)
			{
				spawnRate = (int)((double)spawnRate * 0.02);
				maxSpawns = (int)((float)maxSpawns * 2.5f);
			}

			if (player.GetModPlayer<CalamityPlayer>(mod).zen && player.GetModPlayer<CalamityPlayer>(mod).tranquilityCandle)
			{
				spawnRate = (int)((double)spawnRate * (player.GetModPlayer<CalamityPlayer>(mod).ZoneAbyss ? 1.75 : 75));
				maxSpawns = (int)((float)maxSpawns * (player.GetModPlayer<CalamityPlayer>(mod).ZoneAbyss ? 0.625f : 0.005f));
			}
			else if (CalamityPlayer.areThereAnyDamnBosses || player.GetModPlayer<CalamityPlayer>(mod).zen || (Config.DisableExpertEnemySpawnsNearHouse && player.townNPCs > 1f && Main.expertMode))
			{
				spawnRate = (int)((double)spawnRate * (player.GetModPlayer<CalamityPlayer>(mod).ZoneAbyss ? 1.5 : 50));
				maxSpawns = (int)((float)maxSpawns * (player.GetModPlayer<CalamityPlayer>(mod).ZoneAbyss ? 0.75f : 0.01f));
			}
			else if (player.GetModPlayer<CalamityPlayer>(mod).tranquilityCandle)
			{
				spawnRate = (int)((double)spawnRate * (player.GetModPlayer<CalamityPlayer>(mod).ZoneAbyss ? 1.25 : 25));
				maxSpawns = (int)((float)maxSpawns * (player.GetModPlayer<CalamityPlayer>(mod).ZoneAbyss ? 0.875f : 0.02f));
			}
		}
		#endregion

		#region Edit Spawn Range
		public override void EditSpawnRange(Player player, ref int spawnRangeX, ref int spawnRangeY, ref int safeRangeX, ref int safeRangeY)
		{
			if (player.GetModPlayer<CalamityPlayer>(mod).ZoneAbyss)
			{
				spawnRangeX = (int)((double)(1920 / 16) * 0.5); //0.7
				safeRangeX = (int)((double)(1920 / 16) * 0.32); //0.52
			}
		}
		#endregion

		#region Edit Spawn Pool
		public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneAbyss ||
				spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneCalamity ||
				spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneSulphur ||
				spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneSunkenSea ||
				(spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneAstral && !NPC.LunarApocalypseIsUp))
			{
				pool[0] = 0f;
			}
		}
		#endregion

		#region Drawing
		public override void DrawEffects(NPC npc, ref Color drawColor)
		{
			if (bBlood)
			{
				if (Main.rand.Next(5) < 4)
				{
					int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, 5, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color), 3f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity *= 1.8f;
					Main.dust[dust].velocity.Y -= 0.5f;
					if (Main.rand.Next(4) == 0)
					{
						Main.dust[dust].noGravity = false;
						Main.dust[dust].scale *= 0.5f;
					}
				}
				Lighting.AddLight(npc.position, 0.08f, 0f, 0f);
			}
			if (bFlames || enraged)
			{
				if (Main.rand.Next(5) < 4)
				{
					int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, mod.DustType("BrimstoneFlame"), npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color), 3.5f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity *= 1.8f;
					Main.dust[dust].velocity.Y -= 0.5f;
					if (Main.rand.Next(4) == 0)
					{
						Main.dust[dust].noGravity = false;
						Main.dust[dust].scale *= 0.5f;
					}
				}
				Lighting.AddLight(npc.position, 0.05f, 0.01f, 0.01f);
			}
			if (aFlames)
			{
				if (Main.rand.Next(5) < 4)
				{
					int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, mod.DustType("BrimstoneFlame"), npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color), 3.5f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity *= 1.8f;
					Main.dust[dust].velocity.Y -= 0.25f;
					if (Main.rand.Next(4) == 0)
					{
						Main.dust[dust].noGravity = false;
						Main.dust[dust].scale *= 0.35f;
					}
				}
				Lighting.AddLight(npc.position, 0.025f, 0f, 0f);
			}
			if (pShred)
			{
				if (Main.rand.Next(5) < 4)
				{
					int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, 5, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color), 1.5f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity *= 1.1f;
					Main.dust[dust].velocity.Y += 0.25f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[dust].noGravity = false;
						Main.dust[dust].scale *= 0.5f;
					}
				}
			}
			if (hFlames)
			{
				if (Main.rand.Next(5) < 4)
				{
					int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, mod.DustType("HolyFlame"), npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color), 3.5f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity *= 1.8f;
					Main.dust[dust].velocity.Y -= 0.5f;
					if (Main.rand.Next(4) == 0)
					{
						Main.dust[dust].noGravity = false;
						Main.dust[dust].scale *= 0.5f;
					}
				}
				Lighting.AddLight(npc.position, 0.25f, 0.25f, 0.1f);
			}
			if (pFlames)
			{
				if (Main.rand.Next(5) < 4)
				{
					int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, 89, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color), 3.5f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity *= 1.2f;
					Main.dust[dust].velocity.Y -= 0.15f;
					if (Main.rand.Next(4) == 0)
					{
						Main.dust[dust].noGravity = false;
						Main.dust[dust].scale *= 0.5f;
					}
				}
				Lighting.AddLight(npc.position, 0.07f, 0.15f, 0.01f);
			}
			if (gsInferno)
			{
				if (Main.rand.Next(5) < 4)
				{
					int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, 173, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color), 1.5f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity *= 1.2f;
					Main.dust[dust].velocity.Y -= 0.15f;
					if (Main.rand.Next(4) == 0)
					{
						Main.dust[dust].noGravity = false;
						Main.dust[dust].scale *= 0.5f;
					}
				}
				Lighting.AddLight(npc.position, 0.1f, 0f, 0.135f);
			}
			if (nightwither)
			{
				Rectangle hitbox = npc.Hitbox;
				if (Main.rand.Next(5) < 4)
				{
					int num3 = Utils.SelectRandom<int>(Main.rand, new int[]
					{
						173,
						27,
						234
					});
					int num4 = Dust.NewDust(hitbox.TopLeft(), npc.width, npc.height, num3, 0f, -2.5f, 0, default(Color), 1f);
					Main.dust[num4].noGravity = true;
					Main.dust[num4].alpha = 200;
					Main.dust[num4].velocity.Y -= 0.2f;
					Dust dust = Main.dust[num4];
					dust.velocity *= 1.2f;
					dust = Main.dust[num4];
					dust.scale += Main.rand.NextFloat();
				}
			}
			if (tSad || cDepth)
			{
				if (Main.rand.Next(6) < 3)
				{
					int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, 33, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color), 3.5f);
					Main.dust[dust].noGravity = false;
					Main.dust[dust].velocity *= 1.2f;
					Main.dust[dust].velocity.Y += 0.15f;
					if (Main.rand.Next(4) == 0)
					{
						Main.dust[dust].noGravity = false;
						Main.dust[dust].scale *= 0.5f;
					}
				}
			}
			if (dFlames)
			{
				if (Main.rand.Next(5) < 4)
				{
					int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, 173, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color), 1.5f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity *= 1.2f;
					Main.dust[dust].velocity.Y -= 0.15f;
					if (Main.rand.Next(4) == 0)
					{
						Main.dust[dust].noGravity = false;
						Main.dust[dust].scale *= 0.5f;
					}
				}
				Lighting.AddLight(npc.position, 0.1f, 0f, 0.135f);
			}

			if (gState || eFreeze)
				drawColor = Color.Cyan;
			if (marked)
				drawColor = Color.Fuchsia;
			if (pearlAura)
				drawColor = Color.White;
		}

		public override Color? GetAlpha(NPC npc, Color drawColor)
		{
			if (Main.player[Main.myPlayer].GetModPlayer<CalamityPlayer>(mod).trippy)
				return new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, npc.alpha);

			if (enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
				return new Color(200, 50, 50, npc.alpha);

			return null;
		}

		public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
		{
			if (Main.player[Main.myPlayer].GetModPlayer<CalamityPlayer>(mod).trippy)
			{
				SpriteEffects spriteEffects = SpriteEffects.None;
				if (npc.spriteDirection == 1)
					spriteEffects = SpriteEffects.FlipHorizontally;

				float num66 = 0f;
				Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));
				Microsoft.Xna.Framework.Color color9 = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0);
				Microsoft.Xna.Framework.Color alpha15 = npc.GetAlpha(color9);
				float num212 = 0.99f;
				alpha15.R = (byte)((float)alpha15.R * num212);
				alpha15.G = (byte)((float)alpha15.G * num212);
				alpha15.B = (byte)((float)alpha15.B * num212);
				alpha15.A = (byte)((float)alpha15.A * num212);
				for (int num213 = 0; num213 < 4; num213++)
				{
					Vector2 position9 = npc.position;
					float num214 = Math.Abs(npc.Center.X - Main.player[Main.myPlayer].Center.X);
					float num215 = Math.Abs(npc.Center.Y - Main.player[Main.myPlayer].Center.Y);

					if (num213 == 0 || num213 == 2)
						position9.X = Main.player[Main.myPlayer].Center.X + num214;
					else
						position9.X = Main.player[Main.myPlayer].Center.X - num214;

					position9.X -= (float)(npc.width / 2);

					if (num213 == 0 || num213 == 1)
						position9.Y = Main.player[Main.myPlayer].Center.Y + num215;
					else
						position9.Y = Main.player[Main.myPlayer].Center.Y - num215;

					position9.Y -= (float)(npc.height / 2);

					Main.spriteBatch.Draw(Main.npcTexture[npc.type], new Vector2(position9.X - Main.screenPosition.X + (float)(npc.width / 2) - (float)Main.npcTexture[npc.type].Width * npc.scale / 2f + vector11.X * npc.scale, position9.Y - Main.screenPosition.Y + (float)npc.height - (float)Main.npcTexture[npc.type].Height * npc.scale / (float)Main.npcFrameCount[npc.type] + 4f + vector11.Y * npc.scale + num66 + npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(npc.frame), alpha15, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}
			return true;
		}
		#endregion

		#region Get Chat
		public override void GetChat(NPC npc, ref string chat)
		{
			switch (npc.type)
			{
				case NPCID.Guide:
					if (Main.rand.Next(20) == 0 && Main.hardMode)
						chat = "Could you be so kind as to, ah...check hell for me...? I left someone I kind of care about down there.";
					if (Main.rand.Next(20) == 0 && CalamityWorld.spawnAstralMeteor)
						chat = "I have this sudden shiver up my spine, like a meteor just fell and thousands of innocent creatures turned into monsters from the stars.";
					if (Main.rand.Next(20) == 0 && NPC.downedMoonlord)
						chat = "The dungeon seems even more restless than usual, watch out for the powerful abominations stirring up in there.";
					if (Main.rand.Next(20) == 0 && CalamityWorld.downedProvidence)
						chat = "Seems like extinguishing that butterfly caused its life to seep into the hallowed areas, try taking a peek there and see what you can find!";
					if (Main.rand.Next(20) == 0 && CalamityWorld.downedProvidence)
						chat = "I've heard there is a portal of antimatter absorbing everything it can see in the dungeon, try using the Rune of Kos there!";
					break;
				case NPCID.PartyGirl:
					int fapsol = NPC.FindFirstNPC(mod.NPCType("FAP"));
					if (Main.rand.Next(10) == 0 && fapsol != -1)
						chat = "I have a feeling we're going to have absolutely fantastic parties with " + Main.npc[fapsol].GivenName + " around!";
					if (Main.rand.Next(5) == 0 && Main.eclipse)
						chat = "I think my light display is turning into an accidental bug zapper. At least the monsters are enjoying it.";
					if (Main.rand.Next(5) == 0 && Main.eclipse)
						chat = "Ooh! I love parties where everyone wears a scary costume!";
					break;
				case NPCID.Wizard:
					int permadong = NPC.FindFirstNPC(mod.NPCType("DILF"));
					if (Main.rand.Next(10) == 0 && permadong != -1)
						chat = "I'd let " + Main.npc[permadong].GivenName + " coldheart MY icicle.";
					if (Main.rand.Next(10) == 0 && CalamityWorld.spawnAstralMeteor)
						chat = "Space just got way too close for comfort.";
					break;
				case NPCID.Dryad:
					if (Main.rand.Next(5) == 0 && CalamityWorld.buffedEclipse && Main.eclipse)
						chat = "There's a dark solar energy emanating from the moths that appear during this time. Ah, the moths as you progress further get more powerful...hmm...what power was Yharon holding back?";
					if (Main.rand.Next(10) == 0 && CalamityWorld.spawnAstralMeteor)
						chat = "That starborne illness sits upon this land like a blister. Do even more vile forces of corruption exist in worlds beyond?";
					break;
				case NPCID.Stylist:
					int fapsol2 = NPC.FindFirstNPC(mod.NPCType("FAP"));
					string worldEvil = WorldGen.crimson ? "Crimson" : "Corruption";
					if (Main.rand.Next(15) == 0 && fapsol2 != -1)
						chat = "Sometimes I catch " + Main.npc[fapsol2].GivenName + " sneaking up from behind me.";
					if (Main.rand.Next(15) == 0 && fapsol2 != -1)
						chat = Main.npc[fapsol2].GivenName + " is always trying to brighten my mood...even if, deep down, I know she's sad.";
					if (Main.rand.Next(15) == 0 && CalamityWorld.spawnAstralMeteor)
						chat = "Please don't catch space lice. Or " + worldEvil + " lice. Or just lice in general.";
					break;
				case NPCID.GoblinTinkerer:
					if (Main.rand.Next(10) == 0 && NPC.downedMoonlord)
						chat = "You know...we haven't had an invasion in a while...";
					break;
				case NPCID.ArmsDealer:
					if (Main.rand.Next(5) == 0 && Main.eclipse)
						chat = "That's the biggest moth I've ever seen for sure. You'd need one big gun to take one of those down.";
					if (Main.rand.Next(10) == 0 && CalamityWorld.downedDoG)
						chat = "Is it me or are your weapons getting bigger and bigger?";
					break;
				case NPCID.Merchant:
					if (Main.rand.Next(5) == 0 && NPC.downedMoonlord)
						chat = "Each night seems only more foreboding than the last. I feel unthinkable terrors are watching your every move.";
					if (Main.rand.Next(5) == 0 && Main.eclipse)
						chat = "Are you daft?! Turn off those lamps!";
					if (Main.rand.Next(5) == 0 && Main.raining && CalamityWorld.sulphurTiles > 30)
						chat = "If this acid rain keeps up, there'll be a shortage of Dirt Blocks soon enough!";
					break;
				case NPCID.Mechanic:
					int fapsol3 = NPC.FindFirstNPC(mod.NPCType("FAP"));
					if (Main.rand.Next(5) == 0 && NPC.downedMoonlord)
						chat = "What do you mean your traps aren't making the cut? Don't look at me!";
					if (Main.rand.Next(5) == 0 && Main.eclipse)
						chat = "Um...should my nightlight be on?";
					if (Main.rand.Next(5) == 0 && fapsol3 != -1)
						chat = "Well, I like " + Main.npc[fapsol3].GivenName + ", but I, ah...I have my eyes on someone else.";
					break;
				case NPCID.DD2Bartender:
					int fapsol4 = NPC.FindFirstNPC(mod.NPCType("FAP"));
					if (Main.rand.Next(5) == 0 && !Main.dayTime && Main.moonPhase == 0)
						chat = "Care for a little Moonshine?";
					if (Main.rand.Next(10) == 0 && fapsol4 != -1)
						chat = "Sheesh, " + Main.npc[fapsol4].GivenName + " is a little cruel, isn't she? I never claimed to be an expert on anything but ale!";
					break;
				case NPCID.Pirate:
					if (Main.rand.Next(5) == 0 && !CalamityWorld.downedLeviathan)
						chat = "Aye, I’ve heard of a mythical creature in the oceans, singing with an alluring voice. Careful when yer fishin out there.";
					if (Main.rand.Next(5) == 0 && CalamityWorld.downedAquaticScourge)
						chat = "I have to thank ye again for takin' care of that sea serpent. Or was that another one...";
					break;
				case NPCID.Cyborg:
					if (Main.rand.Next(10) == 0 && Main.raining)
						chat = "All these moments will be lost in time. Like tears...in the rain.";
					if (Main.rand.Next(10) == 0 && NPC.downedMoonlord)
						chat = "Always shoot for the moon! It has clearly worked before.";
					if (Main.rand.Next(10) == 0 && NPC.downedMoonlord)
						chat = "Draedon? He's...a little 'high octane' if you know what I mean.";
					if (Main.rand.Next(10) == 0 && !CalamityWorld.downedPlaguebringer && NPC.downedGolemBoss)
						chat = "Those oversized bugs terrorizing the jungle... Surely you of all people could shut them down!";
					break;
				case NPCID.Clothier:
					if (Main.rand.Next(10) == 0 && NPC.downedMoonlord)
						chat = "Who you gonna call?";
					if (Main.rand.Next(10) == 0 && NPC.downedMoonlord)
						chat = "Those screams...I’m not sure why, but I feel like a nameless fear has awoken in my heart.";
					if (Main.rand.Next(10) == 0 && NPC.downedMoonlord)
						chat = "I can faintly hear ghostly shrieks from the dungeon...and not ones I’m familiar with at all. Just what is going on in there?";
					if (Main.rand.Next(10) == 0 && CalamityWorld.downedPolterghast)
						chat = "Whatever that thing was, I’m glad it’s gone now.";
					if (Main.rand.Next(5) == 0 && NPC.AnyNPCs(NPCID.MoonLordCore))
						chat = "Houston, we've had a problem.";
					break;
				case NPCID.Steampunker:
					bool hasPortalGun = false;
					for (int k = 0; k < 255; k++)
					{
						Player player = Main.player[k];
						if (player.active)
						{
							for (int j = 0; j < player.inventory.Length; j++)
							{
								if (player.inventory[j].type == ItemID.PortalGun)
									hasPortalGun = true;
							}
						}
					}
					if (Main.rand.Next(5) == 0 && NPC.downedMoonlord)
						chat = "Yep! I'm also considering being a space pirate now.";
					if (Main.rand.Next(5) == 0 && hasPortalGun)
						chat = "Just what is that contraption? It makes my Teleporters look like child's play!";
					break;
				case NPCID.DyeTrader:
					int permadong2 = NPC.FindFirstNPC(mod.NPCType("DILF"));
					if (Main.rand.Next(5) == 0)
						chat = "Have you seen those gemstone creatures in the caverns? Their colors are simply breathtaking!";
					if (Main.rand.Next(5) == 0 && permadong2 != -1)
						chat = "Do you think " + Main.npc[permadong2].GivenName + " knows how to 'let it go?'";
					break;
				case NPCID.TaxCollector:
					int platinumCoins = 0;
					for (int k = 0; k < 255; k++)
					{
						Player player = Main.player[k];
						if (player.active)
						{
							for (int j = 0; j < player.inventory.Length; j++)
							{
								if (player.inventory[j].type == ItemID.PlatinumCoin)
									platinumCoins = player.inventory[j].stack;
							}
						}
					}
					if (Main.rand.Next(5) == 0 && platinumCoins >= 100)
						chat = "BAH! Doesn't seem like I'll ever be able to quarrel with the debts of the town again!";
					if (Main.rand.Next(5) == 0 && platinumCoins >= 500)
						chat = "Where and how are you getting all of this money?";
					if (Main.rand.Next(5) == 0 && !CalamityWorld.downedBrimstoneElemental)
						chat = "Perhaps with all that time you've got you could check those old ruins? Certainly something of value in it for you!";
					if (Main.rand.Next(10) == 0 && CalamityWorld.downedDoG)
						chat = "Devourer of what, you said? Devourer of Funds, if its payroll is anything to go by!";
					break;
				case NPCID.Demolitionist:
					if (Main.rand.Next(5) == 0 && CalamityWorld.downedDoG)
						chat = "God Slayer Dynamite? Boy do I like the sound of that!";
					break;
				default:
					break;
			}
		}
		#endregion

		#region Shop Stuff
		public override void SetupShop(int type, Chest shop, ref int nextSlot)
		{
			if (type == NPCID.Merchant && (Main.player[Main.myPlayer].HasItem(mod.ItemType("FirestormCannon")) || Main.player[Main.myPlayer].HasItem(mod.ItemType("SpectralstormCannon"))))
			{
				shop.item[nextSlot].SetDefaults(ItemID.Flare);
				nextSlot++;
			}
			if (type == NPCID.Merchant)
			{
				if (NPC.downedBoss1)
				{
					shop.item[nextSlot].SetDefaults(ItemID.ApprenticeBait);
					nextSlot++;
				}
				if (NPC.downedBoss2)
				{
					shop.item[nextSlot].SetDefaults(ItemID.JourneymanBait);
					nextSlot++;
					if (WorldGen.crimson)
					{
						shop.item[nextSlot].SetDefaults(ItemID.Vilethorn);
						nextSlot++;
					}
					else
					{
						shop.item[nextSlot].SetDefaults(ItemID.TheRottedFork);
						nextSlot++;
					}
				}
				if (NPC.downedBoss3)
				{
					shop.item[nextSlot].SetDefaults(ItemID.MasterBait);
					nextSlot++;
				}
			}
			if (type == NPCID.ArmsDealer && (Main.player[Main.myPlayer].HasItem(mod.ItemType("Impaler"))))
			{
				shop.item[nextSlot].SetDefaults(ItemID.Stake);
				nextSlot++;
			}
			if (type == NPCID.ArmsDealer)
			{
				if (NPC.downedBoss2)
				{
					if (WorldGen.crimson)
					{
						shop.item[nextSlot].SetDefaults(ItemID.Musket);
						nextSlot++;
					}
					else
					{
						shop.item[nextSlot].SetDefaults(ItemID.TheUndertaker);
						nextSlot++;
					}
				}
			}
			if (type == NPCID.Dryad)
			{
				shop.item[nextSlot].SetDefaults(ItemID.JungleRose);
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 2, 0, 0);
				nextSlot++;
				shop.item[nextSlot].SetDefaults(ItemID.NaturesGift);
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 10, 0, 0);
				nextSlot++;
				shop.item[nextSlot].SetDefaults(ItemID.GoblinBattleStandard);
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 1, 0, 0);
				nextSlot++;
				if (NPC.downedSlimeKing)
				{
					shop.item[nextSlot].SetDefaults(ItemID.SlimeCrown);
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 2, 0, 0);
					nextSlot++;
				}
				if (CalamityWorld.downedDesertScourge)
				{
					shop.item[nextSlot].SetDefaults(mod.ItemType("DriedSeafood"));
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 2, 0, 0);
					nextSlot++;
				}
				if (NPC.downedBoss1)
				{
					shop.item[nextSlot].SetDefaults(ItemID.SuspiciousLookingEye);
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 3, 0, 0);
					nextSlot++;
				}
				if (CalamityWorld.downedCrabulon)
				{
					shop.item[nextSlot].SetDefaults(mod.ItemType("DecapoditaSprout"));
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 4, 0, 0);
					nextSlot++;
				}
				if (NPC.downedBoss2)
				{
					shop.item[nextSlot].SetDefaults(ItemID.BloodySpine);
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 6, 0, 0);
					nextSlot++;
					shop.item[nextSlot].SetDefaults(ItemID.WormFood);
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 6, 0, 0);
					nextSlot++;
					if (WorldGen.crimson)
					{
						if (Main.expertMode)
						{
							shop.item[nextSlot].SetDefaults(ItemID.WormScarf);
							nextSlot++;
						}
						shop.item[nextSlot].SetDefaults(ItemID.BandofStarpower);
						nextSlot++;
					}
					else
					{
						if (Main.expertMode)
						{
							shop.item[nextSlot].SetDefaults(ItemID.BrainOfConfusion);
							nextSlot++;
						}
						shop.item[nextSlot].SetDefaults(ItemID.PanicNecklace);
						nextSlot++;
					}
				}
				if (CalamityWorld.downedPerforator)
				{
					shop.item[nextSlot].SetDefaults(mod.ItemType("BloodyWormFood"));
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 10, 0, 0);
					nextSlot++;
					if (Main.expertMode)
					{
						shop.item[nextSlot].SetDefaults(mod.ItemType("RottenBrain"));
						nextSlot++;
					}
				}
				if (CalamityWorld.downedHiveMind)
				{
					shop.item[nextSlot].SetDefaults(mod.ItemType("Teratoma"));
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 10, 0, 0);
					nextSlot++;
					if (Main.expertMode)
					{
						shop.item[nextSlot].SetDefaults(mod.ItemType("BloodyWormTooth"));
						nextSlot++;
					}
				}
				if (CalamityWorld.downedSlimeGod)
				{
					shop.item[nextSlot].SetDefaults(mod.ItemType("OverloadedSludge"));
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 15, 0, 0);
					nextSlot++;
				}
				if (CalamityWorld.downedAquaticScourge)
				{
					shop.item[nextSlot].SetDefaults(mod.ItemType("Seafood"));
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 20, 0, 0);
					nextSlot++;
				}
				if (NPC.downedHalloweenKing)
				{
					shop.item[nextSlot].SetDefaults(ItemID.PumpkinMoonMedallion);
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 25, 0, 0);
					nextSlot++;
				}
				if (NPC.downedChristmasIceQueen)
				{
					shop.item[nextSlot].SetDefaults(ItemID.NaughtyPresent);
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 25, 0, 0);
					nextSlot++;
				}
				shop.item[nextSlot].SetDefaults(mod.ItemType("RomajedaOrchid"));
				nextSlot++;
			}
			if (type == NPCID.GoblinTinkerer)
			{
				shop.item[nextSlot].SetDefaults(mod.ItemType("MeleeLevelMeter"));
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 5, 0, 0);
				nextSlot++;
				shop.item[nextSlot].SetDefaults(mod.ItemType("RangedLevelMeter"));
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 5, 0, 0);
				nextSlot++;
				shop.item[nextSlot].SetDefaults(mod.ItemType("MagicLevelMeter"));
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 5, 0, 0);
				nextSlot++;
				shop.item[nextSlot].SetDefaults(mod.ItemType("SummonLevelMeter"));
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 5, 0, 0);
				nextSlot++;
				shop.item[nextSlot].SetDefaults(mod.ItemType("RogueLevelMeter"));
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 5, 0, 0);
				nextSlot++;
			}
			if (type == NPCID.Clothier)
			{
				shop.item[nextSlot].SetDefaults(mod.ItemType("BlueBrickWallUnsafe"));
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 0, 0, 10);
				nextSlot++;
				shop.item[nextSlot].SetDefaults(mod.ItemType("BlueSlabWallUnsafe"));
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 0, 0, 10);
				nextSlot++;
				shop.item[nextSlot].SetDefaults(mod.ItemType("BlueTiledWallUnsafe"));
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 0, 0, 10);
				nextSlot++;
				shop.item[nextSlot].SetDefaults(mod.ItemType("GreenBrickWallUnsafe"));
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 0, 0, 10);
				nextSlot++;
				shop.item[nextSlot].SetDefaults(mod.ItemType("GreenSlabWallUnsafe"));
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 0, 0, 10);
				nextSlot++;
				shop.item[nextSlot].SetDefaults(mod.ItemType("GreenTiledWallUnsafe"));
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 0, 0, 10);
				nextSlot++;
				shop.item[nextSlot].SetDefaults(mod.ItemType("PinkBrickWallUnsafe"));
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 0, 0, 10);
				nextSlot++;
				shop.item[nextSlot].SetDefaults(mod.ItemType("PinkSlabWallUnsafe"));
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 0, 0, 10);
				nextSlot++;
				shop.item[nextSlot].SetDefaults(mod.ItemType("PinkTiledWallUnsafe"));
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 0, 0, 10);
				nextSlot++;
				if (Main.hardMode)
				{
					shop.item[nextSlot].SetDefaults(ItemID.GoldenKey);
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 5, 0, 0);
					nextSlot++;
				}
			}
			if (type == NPCID.Steampunker)
			{
				if (NPC.downedMechBoss1)
				{
					shop.item[nextSlot].SetDefaults(ItemID.MechanicalWorm);
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 20, 0, 0);
					nextSlot++;
				}
				if (NPC.downedMechBoss2)
				{
					shop.item[nextSlot].SetDefaults(ItemID.MechanicalEye);
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 20, 0, 0);
					nextSlot++;
				}
				if (NPC.downedMechBoss3)
				{
					shop.item[nextSlot].SetDefaults(ItemID.MechanicalSkull);
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 20, 0, 0);
					nextSlot++;
				}
				if (Main.player[Main.myPlayer].GetModPlayer<CalamityPlayer>(mod).ZoneAstral && CalamityWorld.spawnAstralMeteor)
				{
					shop.item[nextSlot].SetDefaults(mod.ItemType("AstralSolution"));
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 0, 5, 0);
					nextSlot++;
				}
			}
			if (type == NPCID.Wizard)
			{
				if (CalamityWorld.downedCryogen)
				{
					shop.item[nextSlot].SetDefaults(mod.ItemType("CryoKey"));
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 15, 0, 0);
					nextSlot++;
				}
				if (CalamityWorld.downedBrimstoneElemental)
				{
					shop.item[nextSlot].SetDefaults(mod.ItemType("CharredIdol"));
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 20, 0, 0);
					nextSlot++;
				}
				if (CalamityWorld.downedAstrageldon)
				{
					shop.item[nextSlot].SetDefaults(mod.ItemType("AstralChunk"));
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 25, 0, 0);
					nextSlot++;
				}
				if (NPC.downedGolemBoss)
				{
					shop.item[nextSlot].SetDefaults(ItemID.SpectreStaff);
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 25, 0, 0);
					nextSlot++;
					shop.item[nextSlot].SetDefaults(ItemID.InfernoFork);
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 25, 0, 0);
					nextSlot++;
					shop.item[nextSlot].SetDefaults(ItemID.ShadowbeamStaff);
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 25, 0, 0);
					nextSlot++;
				}
				if (NPC.downedMoonlord)
				{
					shop.item[nextSlot].SetDefaults(ItemID.CelestialSigil);
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(3, 0, 0, 0);
					nextSlot++;
				}
				if (CalamityWorld.downedGuardians)
				{
					shop.item[nextSlot].SetDefaults(mod.ItemType("ProfanedShard"));
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(10, 0, 0, 0);
					nextSlot++;
				}
			}
			if (type == NPCID.WitchDoctor)
			{
				shop.item[nextSlot].SetDefaults(ItemID.Abeemination);
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 8, 0, 0);
				nextSlot++;
				if (NPC.downedPlantBoss)
				{
					shop.item[nextSlot].SetDefaults(mod.ItemType("BulbofDoom"));
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 20, 0, 0);
					nextSlot++;
				}
				if (NPC.downedGolemBoss)
				{
					shop.item[nextSlot].SetDefaults(ItemID.SolarTablet);
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 25, 0, 0);
					nextSlot++;
					shop.item[nextSlot].SetDefaults(ItemID.LihzahrdPowerCell);
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 30, 0, 0);
					nextSlot++;
				}
				if (CalamityWorld.downedScavenger)
				{
					shop.item[nextSlot].SetDefaults(mod.ItemType("AncientMedallion"));
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 50, 0, 0);
					nextSlot++;
				}
				if (CalamityWorld.downedPlaguebringer)
				{
					shop.item[nextSlot].SetDefaults(mod.ItemType("Abomination"));
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 50, 0, 0);
					nextSlot++;
				}
				if (CalamityWorld.downedBumble)
				{
					shop.item[nextSlot].SetDefaults(mod.ItemType("BirbPheromones"));
					shop.item[nextSlot].shopCustomPrice = Item.buyPrice(5, 0, 0, 0);
					nextSlot++;
				}
			}
			if (type == NPCID.TravellingMerchant && Main.moonPhase == 0)
			{
				shop.item[nextSlot].SetDefaults(mod.ItemType("FrostBarrier"));
				nextSlot++;
			}
		}
		#endregion

		#region Any Boss NPCs
		public static bool AnyBossNPCS()
		{
			Mod mod = ModLoader.GetMod("CalamityMod");

			for (int i = 0; i < 200; i++)
			{
				if (Main.npc[i].active && Main.npc[i].type != NPCID.MartianSaucerCore &&
					(Main.npc[i].boss || Main.npc[i].type == NPCID.EaterofWorldsHead || Main.npc[i].type == NPCID.EaterofWorldsTail || Main.npc[i].type == mod.NPCType("SlimeGodRun") ||
					Main.npc[i].type == mod.NPCType("SlimeGodRunSplit") || Main.npc[i].type == mod.NPCType("SlimeGod") || Main.npc[i].type == mod.NPCType("SlimeGodSplit")))
				{
					return true;
				}
			}
			return false;
		}
		#endregion

		#region Any Living Players
		public static bool AnyLivingPlayers()
		{
			for (int i = 0; i < 255; i++)
			{
				if (Main.player[i].active && !Main.player[i].dead && !Main.player[i].ghost)
					return true;
			}
			return false;
		}
		#endregion

		#region Should Affect NPC
		public static bool ShouldAffectNPC(NPC target)
		{
			Mod mod = ModLoader.GetMod("CalamityMod");

			if (target.damage > 0 && !target.boss && !target.friendly && !target.dontTakeDamage && target.type != NPCID.Mothron &&
				target.type != NPCID.Pumpking && target.type != NPCID.TheDestroyerBody && target.type != NPCID.TheDestroyerTail &&
				target.type != NPCID.MourningWood && target.type != NPCID.Everscream && target.type != NPCID.SantaNK1 && target.type != NPCID.IceQueen &&
				target.type != mod.NPCType("Reaper") && target.type != mod.NPCType("Mauler") && target.type != mod.NPCType("EidolonWyrmHead") &&
				target.type != mod.NPCType("EidolonWyrmHeadHuge") && target.type != mod.NPCType("ColossalSquid") && target.type != NPCID.DD2Betsy)
			{
				return true;
			}
			return false;
		}
		#endregion

		#region Old Duke Spawn
		public static void OldDukeSpawn(int plr, int Type)
		{
			Mod mod = ModLoader.GetMod("CalamityMod");
			Player player = Main.player[plr];

			if (!player.active || player.dead)
				return;

			int m = 0;
			while (m < 1000)
			{
				Projectile projectile = Main.projectile[m];
				if (projectile.active && projectile.bobber && projectile.owner == plr)
				{
					int num8 = NPC.NewNPC((int)projectile.Center.X, (int)projectile.Center.Y + 100, mod.NPCType("OldDuke"), 0, 0f, 0f, 0f, 0f, 255);
					string typeName2 = Main.npc[num8].TypeName;
					if (Main.netMode == 0)
					{
						Main.NewText(Language.GetTextValue("Announcement.HasAwoken", typeName2), 175, 75, 255, false);
						return;
					}
					if (Main.netMode == 2)
					{
						NetMessage.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", new object[]
						{
								Main.npc[num8].GetTypeNetName()
						}), new Color(175, 75, 255), -1);
						return;
					}
					break;
				}
				else
					m++;
			}
		}
		#endregion

		#region Astral things
		public static void DoHitDust(NPC npc, int hitDirection, int dustType = 5, float xSpeedMult = 1f, int numHitDust = 5, int numDeathDust = 20)
		{
			for (int k = 0; k < 5; k++)
				Dust.NewDust(npc.position, npc.width, npc.height, dustType, hitDirection * xSpeedMult, -1f, 0, default(Color), 1f);

			if (npc.life <= 0)
			{
				for (int k = 0; k < 20; k++)
					Dust.NewDust(npc.position, npc.width, npc.height, dustType, hitDirection * xSpeedMult, -1f, 0, default(Color), 1f);
			}
		}

		public static void DoFlyingAI(NPC npc, float maxSpeed, float acceleration, float circleTime, float minDistanceTarget = 150f, bool shouldAttackTarget = true)
		{
			//Pick a new target.
			if (npc.target < 0 || npc.target >= 255 || Main.player[npc.target].dead)
				npc.TargetClosest(true);

			Player myTarget = Main.player[npc.target];
			Vector2 toTarget = (myTarget.Center - npc.Center);
			float distanceToTarget = toTarget.Length();
			Vector2 maxVelocity = toTarget;

			if (distanceToTarget < 3f)
				maxVelocity = npc.velocity;
			else
			{
				float magnitude = maxSpeed / distanceToTarget;
				maxVelocity *= magnitude;
			}

			//Circular motion
			npc.ai[0]++;

			//y motion
			if (npc.ai[0] > circleTime * 0.5f)
				npc.velocity.Y += acceleration;
			else
				npc.velocity.Y -= acceleration;

			//x motion
			if (npc.ai[0] < circleTime * 0.25f || npc.ai[0] > circleTime * 0.75f)
				npc.velocity.X += acceleration;
			else
				npc.velocity.X -= acceleration;

			//reset
			if (npc.ai[0] > circleTime)
				npc.ai[0] = 0f;

			//if close enough
			if (shouldAttackTarget && distanceToTarget < minDistanceTarget)
				npc.velocity += maxVelocity * 0.007f;

			if (myTarget.dead)
			{
				maxVelocity.X = npc.direction * maxSpeed / 2f;
				maxVelocity.Y = -maxSpeed / 2f;
			}

			//maximise velocity
			if (npc.velocity.X < maxVelocity.X)
				npc.velocity.X += acceleration;
			if (npc.velocity.X > maxVelocity.X)
				npc.velocity.X -= acceleration;
			if (npc.velocity.Y < maxVelocity.Y)
				npc.velocity.Y += acceleration;
			if (npc.velocity.Y > maxVelocity.Y)
				npc.velocity.Y -= acceleration;

			//rotate towards player if alive
			if (!myTarget.dead)
				npc.rotation = toTarget.ToRotation();
			else //don't, do velocity instead
				npc.rotation = npc.velocity.ToRotation();

			npc.rotation += MathHelper.Pi;

			//tile collision
			float collisionDamp = 0.7f;
			if (npc.collideX)
			{
				npc.netUpdate = true;
				npc.velocity.X = npc.oldVelocity.X * -collisionDamp;

				if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
					npc.velocity.X = 2f;
				if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
					npc.velocity.X = -2f;
			}
			if (npc.collideY)
			{
				npc.netUpdate = true;
				npc.velocity.Y = npc.oldVelocity.Y * -collisionDamp;

				if (npc.velocity.Y > 0f && npc.velocity.Y < 1.5f)
					npc.velocity.Y = 1.5f;
				if (npc.velocity.Y < 0f && npc.velocity.Y > -1.5f)
					npc.velocity.Y = -1.5f;
			}

			//water collision
			if (npc.wet)
			{
				if (npc.velocity.Y > 0f)
					npc.velocity.Y *= 0.95f;

				npc.velocity.Y -= 0.3f;

				if (npc.velocity.Y < -2f)
					npc.velocity.Y = -2f;
			}

			//Taken from source. Important for net?
			if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
				npc.netUpdate = true;
		}

		public static void DoSpiderWallAI(NPC npc, int transformType, float chaseMaxSpeed = 2f, float chaseAcceleration = 0.08f)
		{
			//GET NEW TARGET
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
				npc.TargetClosest();

			Vector2 between = Main.player[npc.target].Center - npc.Center;
			float distance = between.Length();

			//modify vector depending on distance and speed.
			if (distance == 0f)
			{
				between.X = npc.velocity.X;
				between.Y = npc.velocity.Y;
			}
			else
			{
				distance = chaseMaxSpeed / distance;
				between.X *= distance;
				between.Y *= distance;
			}

			//update if target dead.
			if (Main.player[npc.target].dead)
			{
				between.X = (float)npc.direction * chaseMaxSpeed / 2f;
				between.Y = -chaseMaxSpeed / 2f;
			}
			npc.spriteDirection = -1;

			//If spider can't see target, circle around to attempt to find the target.
			if (!Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
			{
				//CIRCULAR MOTION, SIMILAR TO FLYING AI (Eater of Souls etc.)
				npc.ai[0]++;

				if (npc.ai[0] > 0f)
					npc.velocity.Y += 0.023f;
				else
					npc.velocity.Y -= 0.023f;

				if (npc.ai[0] < -100f || npc.ai[0] > 100f)
					npc.velocity.X += 0.023f;
				else
					npc.velocity.X -= 0.023f;

				if (npc.ai[0] > 200f)
					npc.ai[0] = -200f;

				npc.velocity.X += between.X * 0.007f;
				npc.velocity.Y += between.Y * 0.007f;
				npc.rotation = npc.velocity.ToRotation();

				if (npc.velocity.X > 1.5f)
					npc.velocity.X *= 0.9f;
				if (npc.velocity.X < -1.5f)
					npc.velocity.X *= 0.9f;
				if (npc.velocity.Y > 1.5f)
					npc.velocity.Y *= 0.9f;
				if (npc.velocity.Y < -1.5f)
					npc.velocity.Y *= 0.9f;

				npc.velocity.X = MathHelper.Clamp(npc.velocity.X, -3f, 3f);
				npc.velocity.Y = MathHelper.Clamp(npc.velocity.Y, -3f, 3f);
			}
			else //CHASE TARGET
			{
				if (npc.velocity.X < between.X)
				{
					npc.velocity.X = npc.velocity.X + chaseAcceleration;

					if (npc.velocity.X < 0f && between.X > 0f)
						npc.velocity.X = npc.velocity.X + chaseAcceleration;
				}
				else if (npc.velocity.X > between.X)
				{
					npc.velocity.X = npc.velocity.X - chaseAcceleration;

					if (npc.velocity.X > 0f && between.X < 0f)
						npc.velocity.X = npc.velocity.X - chaseAcceleration;
				}
				if (npc.velocity.Y < between.Y)
				{
					npc.velocity.Y = npc.velocity.Y + chaseAcceleration;

					if (npc.velocity.Y < 0f && between.Y > 0f)
						npc.velocity.Y = npc.velocity.Y + chaseAcceleration;
				}
				else if (npc.velocity.Y > between.Y)
				{
					npc.velocity.Y = npc.velocity.Y - chaseAcceleration;

					if (npc.velocity.Y > 0f && between.Y < 0f)
						npc.velocity.Y = npc.velocity.Y - chaseAcceleration;
				}
				npc.rotation = between.ToRotation();
			}

			//DAMP COLLISIONS OFF OF WALLS
			float collisionDamp = 0.5f;
			if (npc.collideX)
			{
				npc.netUpdate = true;
				npc.velocity.X = npc.oldVelocity.X * -collisionDamp;

				if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
					npc.velocity.X = 2f;
				if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
					npc.velocity.X = -2f;
			}
			if (npc.collideY)
			{
				npc.netUpdate = true;
				npc.velocity.Y = npc.oldVelocity.Y * -collisionDamp;

				if (npc.velocity.Y > 0f && npc.velocity.Y < 1.5f)
					npc.velocity.Y = 2f;
				if (npc.velocity.Y < 0f && npc.velocity.Y > -1.5f)
					npc.velocity.Y = -2f;
			}

			if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
				npc.netUpdate = true;

			if (Main.netMode != 1)
			{
				int x = (int)npc.Center.X / 16;
				int y = (int)npc.Center.Y / 16;
				bool flag = false;

				for (int i = x - 1; i <= x + 1; i++)
				{
					for (int j = y - 1; j <= y + 1; j++)
					{
						if (Main.tile[i, j] == null)
							return;

						if (Main.tile[i, j].wall > 0)
							flag = true;
					}
				}
				if (!flag)
				{
					npc.Transform(transformType);
					return;
				}
			}
		}

		public static void DoVultureAI(NPC npc, float acceleration = 0.1f, float maxSpeed = 3f, int sitWidth = 30, int flyWidth = 50, int rangeX = 100, int rangeY = 100)
		{
			npc.localAI[0]++;
			npc.noGravity = true;
			npc.TargetClosest(true);

			if (npc.ai[0] == 0f)
			{
				npc.width = sitWidth;
				npc.noGravity = false;
				if (Main.netMode != 1)
				{
					if (npc.velocity.X != 0f || npc.velocity.Y < 0f || (double)npc.velocity.Y > 0.3)
					{
						npc.ai[0] = 1f;
						npc.netUpdate = true;
					}
					else
					{
						Rectangle playerRect = Main.player[npc.target].getRect();
						Rectangle rangeRect = new Rectangle((int)npc.Center.X - rangeX, (int)npc.Center.Y - rangeY, rangeX * 2, rangeY * 2);
						if (npc.localAI[0] > 20f && (rangeRect.Intersects(playerRect) || npc.life < npc.lifeMax))
						{
							npc.ai[0] = 1f;
							npc.velocity.Y -= 6f;
							npc.netUpdate = true;
						}
					}
				}
			}
			else if (!Main.player[npc.target].dead)
			{
				npc.width = flyWidth;

				//Collision damping
				if (npc.collideX)
				{
					npc.velocity.X = npc.oldVelocity.X * -0.5f;

					if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
						npc.velocity.X = 2f;
					if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
						npc.velocity.X = -2f;
				}
				if (npc.collideY)
				{
					npc.velocity.Y = npc.oldVelocity.Y * -0.5f;

					if (npc.velocity.Y > 0f && npc.velocity.Y < 1f)
						npc.velocity.Y = 1f;
					if (npc.velocity.Y < 0f && npc.velocity.Y > -1f)
						npc.velocity.Y = -1f;
				}
				if (npc.direction == -1 && npc.velocity.X > -maxSpeed)
				{
					npc.velocity.X -= acceleration;

					if (npc.velocity.X > maxSpeed)
						npc.velocity.X = npc.velocity.X - acceleration;
					else if (npc.velocity.X > 0f)
						npc.velocity.X = npc.velocity.X - acceleration * 0.5f;

					if (npc.velocity.X < -maxSpeed)
						npc.velocity.X = -maxSpeed;
				}
				else if (npc.direction == 1 && npc.velocity.X < maxSpeed)
				{
					npc.velocity.X = npc.velocity.X + acceleration;

					if (npc.velocity.X < -maxSpeed)
						npc.velocity.X = npc.velocity.X + acceleration;
					else if (npc.velocity.X < 0f)
						npc.velocity.X = npc.velocity.X + acceleration * 0.5f;

					if (npc.velocity.X > maxSpeed)
						npc.velocity.X = maxSpeed;
				}
				float xDistance = Math.Abs(npc.Center.X - Main.player[npc.target].Center.X);
				float yLimiter = Main.player[npc.target].position.Y - (npc.height / 2f);

				if (xDistance > 50f)
					yLimiter -= 100f;

				if (npc.position.Y < yLimiter)
				{
					npc.velocity.Y = npc.velocity.Y + acceleration * 0.5f;

					if (npc.velocity.Y < 0f)
						npc.velocity.Y = npc.velocity.Y + acceleration * 0.1f;
				}
				else
				{
					npc.velocity.Y = npc.velocity.Y - acceleration * 0.5f;

					if (npc.velocity.Y > 0f)
						npc.velocity.Y = npc.velocity.Y - acceleration * 0.1f;
				}

				if (npc.velocity.Y < -maxSpeed)
					npc.velocity.Y = -maxSpeed;
				if (npc.velocity.Y > maxSpeed)
					npc.velocity.Y = maxSpeed;
			}
			//Change velocity if wet.
			if (npc.wet)
			{
				if (npc.velocity.Y > 0f)
					npc.velocity.Y = npc.velocity.Y * 0.95f;

				npc.velocity.Y = npc.velocity.Y - 0.5f;

				if (npc.velocity.Y < -4f)
					npc.velocity.Y = -4f;
			}
		}

		/// <summary>
		/// Allows you to spawn dust on the NPC in a certain place. Uses the npc.position value as the base point for the rectangle.
		/// Takes direction and rotation into account.
		/// </summary>
		/// <param name="frameWidth">The width of the sheet for the NPC.</param>
		/// <param name="rect">The place to put a dust.</param>
		/// <param name="chance">The chance to spawn a dust (0.3 = 30%)</param>
		public static Dust SpawnDustOnNPC(NPC npc, int frameWidth, int frameHeight, int dustType, Rectangle rect, Vector2 velocity = default(Vector2), float chance = 0.5f, bool useSpriteDirection = false)
		{
			Vector2 half = new Vector2(frameWidth / 2f, frameHeight / 2f);

			//"flip" the rectangle's position x-wise.
			if ((!useSpriteDirection && npc.direction == 1) || (useSpriteDirection && npc.spriteDirection == 1))
				rect.X = frameWidth - rect.Right;

			if (Main.rand.NextFloat(1f) < chance)
			{
				Vector2 offset = (npc.Center - half + new Vector2(Main.rand.NextFloat(rect.Left, rect.Right), Main.rand.NextFloat(rect.Top, rect.Bottom))) - npc.Center;
				offset = offset.RotatedBy(npc.rotation);
				Dust d = Dust.NewDustPerfect(npc.Center + offset, dustType, velocity);
				return d;
			}
			return null;
		}
		#endregion
	}
}
