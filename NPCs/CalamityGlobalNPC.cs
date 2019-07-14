using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.Events;
using Terraria.GameContent.Tile_Entities;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.World.Generation;
using Terraria.GameContent.Generation;
using CalamityMod.Tiles;
using CalamityMod.Projectiles;
using CalamityMod;
using CalamityMod.Items;
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
		private float[] newAI = new float[maxAIMod];

		//Town NPC Patreon
		private bool setNewName = true;

		//Draedons Remote
		public static bool DraedonMayhem = false;

		//Lunatic Cultist Rev+ attack
		private int CultProjectiles = 2;
		private const float CultAngleSpread = 170;
		private int CultCountdown = 0;

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
					npc.lifeMax = 780000;
					break;
				case NPCID.PrimeCannon:
					npc.lifeMax = 350000;
					break;
				case NPCID.PrimeVice:
					npc.lifeMax = 440000;
					break;
				case NPCID.PrimeSaw:
					npc.lifeMax = 450000;
					break;
				case NPCID.PrimeLaser:
					npc.lifeMax = 280000;
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
					npc.lifeMax = 1000000;
					break;
				case NPCID.Retinazer:
					npc.lifeMax = 700000;
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

		#region DraedonMechaMayhemStatChanges
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
					npc.lifeMax = (int)((double)npc.lifeMax * 2.9);
				else
					npc.lifeMax = (int)((double)npc.lifeMax * 1.9);

				npc.npcSlots = 32f;
			}
			else if (npc.type == NPCID.PlanterasTentacle)
			{
				npc.lifeMax = (int)((double)npc.lifeMax * 1.25);
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
						npc.lifeMax = (int)((double)npc.lifeMax * 1.9);
						npc.scale *= 1.2f;
					}
				}
				else if (npc.type == NPCID.SkeletronPrime || npc.type == NPCID.PrimeVice || npc.type == NPCID.PrimeCannon || npc.type == NPCID.PrimeSaw || npc.type == NPCID.PrimeLaser)
				{
					if (CalamityWorld.death)
						npc.lifeMax = (int)((double)npc.lifeMax * 1.9);
					else
						npc.lifeMax = (int)((double)npc.lifeMax * 1.25);

					if (npc.type == NPCID.SkeletronPrime)
						npc.npcSlots = 12f;
				}
				else if (npc.type == NPCID.Retinazer)
				{
					if (CalamityWorld.death)
						npc.lifeMax = (int)((double)npc.lifeMax * 1.7);
					else
						npc.lifeMax = (int)((double)npc.lifeMax * 1.45);

					npc.npcSlots = 10f;
				}
				else if (npc.type == NPCID.Spazmatism)
				{
					if (CalamityWorld.death)
						npc.lifeMax = (int)((double)npc.lifeMax * 1.8);
					else
						npc.lifeMax = (int)((double)npc.lifeMax * 1.55);

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

			if (npc.type == NPCID.BrainofCthulhu && npc.alpha > 0)
				return false;

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
					QueenBeeLoreEffect(npc);
					return false;
				}
			}

			if (CalamityWorld.bossRushActive && !npc.friendly && !npc.townNPC)
			{
				BossRushForceDespawnOtherNPCs(npc, mod);
				switch (npc.type)
				{
					case NPCID.KingSlime:
						return BossRushKingSlimeAI(npc);
					case NPCID.BrainofCthulhu:
						return BossRushBrainofCthulhuAI(npc);
					case NPCID.EaterofWorldsHead:
						return BossRushEaterofWorldsAI(npc);
					case NPCID.QueenBee:
						return BossRushQueenBeeAI(npc);
					case NPCID.DukeFishron:
						return BossRushDukeFishronAI(npc);
					default:
						break;
				}
			}

			if (CalamityWorld.revenge || CalamityWorld.bossRushActive)
			{
				switch (npc.type)
				{
					case NPCID.Mothron:
						if (CalamityWorld.buffedEclipse)
							return BuffedMothronAI(npc);
						break;
					case NPCID.Pumpking:
						if (CalamityWorld.downedDoG)
							return BuffedPumpkingAI(npc);
						break;
					case NPCID.PumpkingBlade:
						if (CalamityWorld.downedDoG)
							return BuffedPumpkingBladeAI(npc);
						break;
					case NPCID.IceQueen:
						if (CalamityWorld.downedDoG)
							return BuffedIceQueenAI(npc);
						break;
					case NPCID.EyeofCthulhu:
						if (CalamityWorld.revenge || CalamityWorld.bossRushActive)
							return BuffedEyeofCthulhuAI(npc);
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

		#region Queen Bee Lore Effect
		private void QueenBeeLoreEffect(NPC npc)
		{
			npc.damage = 0;

			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
				npc.TargetClosest(true);

			float num = 5f;
			float num2 = 0.1f;
			npc.localAI[0] += 1f;

			float num3 = (npc.localAI[0] - 60f) / 60f;
			if (num3 > 1f)
				num3 = 1f;
			else
			{
				if (npc.velocity.X > 6f)
					npc.velocity.X = 6f;
				if (npc.velocity.X < -6f)
					npc.velocity.X = -6f;
				if (npc.velocity.Y > 6f)
					npc.velocity.Y = 6f;
				if (npc.velocity.Y < -6f)
					npc.velocity.Y = -6f;
			}

			num2 *= num3;
			Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
			float num4 = (float)npc.direction * num / 2f;
			float num5 = -num / 2f;

			if (npc.velocity.X < num4)
			{
				npc.velocity.X = npc.velocity.X + num2;
				if (npc.velocity.X < 0f && num4 > 0f)
					npc.velocity.X = npc.velocity.X + num2;
			}
			else if (npc.velocity.X > num4)
			{
				npc.velocity.X = npc.velocity.X - num2;
				if (npc.velocity.X > 0f && num4 < 0f)
					npc.velocity.X = npc.velocity.X - num2;
			}
			if (npc.velocity.Y < num5)
			{
				npc.velocity.Y = npc.velocity.Y + num2;
				if (npc.velocity.Y < 0f && num5 > 0f)
					npc.velocity.Y = npc.velocity.Y + num2;
			}
			else if (npc.velocity.Y > num5)
			{
				npc.velocity.Y = npc.velocity.Y - num2;
				if (npc.velocity.Y > 0f && num5 < 0f)
					npc.velocity.Y = npc.velocity.Y - num2;
			}

			if (npc.type != NPCID.Bee && npc.type != NPCID.BeeSmall)
			{
				if (npc.velocity.X > 0f)
					npc.spriteDirection = 1;
				if (npc.velocity.X < 0f)
					npc.spriteDirection = -1;

				npc.rotation = npc.velocity.X * 0.1f;
			}
			else
				npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) - 1.57f;

			float num11 = 0.7f;
			if (npc.collideX)
			{
				npc.netUpdate = true;
				npc.velocity.X = npc.oldVelocity.X * -num11;

				if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
					npc.velocity.X = 2f;
				if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
					npc.velocity.X = -2f;
			}
			if (npc.collideY)
			{
				npc.netUpdate = true;
				npc.velocity.Y = npc.oldVelocity.Y * -num11;

				if (npc.velocity.Y > 0f && (double)npc.velocity.Y < 1.5)
					npc.velocity.Y = 2f;
				if (npc.velocity.Y < 0f && (double)npc.velocity.Y > -1.5)
					npc.velocity.Y = -2f;
			}

			if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
				npc.netUpdate = true;
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

		#region Boss Rush King Slime AI
		private bool BossRushKingSlimeAI(NPC npc)
		{
			float num234 = 1f;
			bool flag8 = false;
			bool flag9 = false;
			npc.aiAction = 0;

			if (npc.ai[3] == 0f && npc.life > 0)
				npc.ai[3] = (float)npc.lifeMax;

			if (npc.localAI[3] == 0f && Main.netMode != 1)
			{
				npc.ai[0] = -100f;
				npc.localAI[3] = 1f;
				npc.TargetClosest(true);
				npc.netUpdate = true;
			}

			if (Main.player[npc.target].dead)
			{
				npc.TargetClosest(true);
				if (Main.player[npc.target].dead)
				{
					npc.timeLeft = 0;

					if (Main.player[npc.target].Center.X < npc.Center.X)
						npc.direction = 1;
					else
						npc.direction = -1;
				}
			}

			if (!Main.player[npc.target].dead && npc.ai[2] >= 300f && npc.ai[1] < 5f && npc.velocity.Y == 0f)
			{
				npc.ai[2] = 0f;
				npc.ai[0] = 0f;
				npc.ai[1] = 5f;

				if (Main.netMode != 1)
				{
					npc.TargetClosest(false);
					Vector2 vector30 = Main.player[npc.target].Center - npc.Center;

					int num238 = 0;
					bool flag10 = false;
					if (vector30.Length() > ((enraged || Config.BossRushXerocCurse) ? 1000f : 2000f))
					{
						flag10 = true;
						num238 = 100;
					}

					Point point3 = npc.Center.ToTileCoordinates();
					Point point4 = Main.player[npc.target].Center.ToTileCoordinates();
					int num235 = 10;
					int num236 = 0;
					int num237 = 7;

					while (!flag10 && num238 < 100)
					{
						num238++;
						int num239 = Main.rand.Next(point4.X - num235, point4.X + num235 + 1);
						int num240 = Main.rand.Next(point4.Y - num235, point4.Y + 1);

						if ((num240 < point4.Y - num237 || num240 > point4.Y + num237 || num239 < point4.X - num237 || num239 > point4.X + num237) && (num240 < point3.Y - num236 || num240 > point3.Y + num236 || num239 < point3.X - num236 || num239 > point3.X + num236) && !Main.tile[num239, num240].nactive())
						{
							int num241 = num240;
							int num242 = 0;

							bool flag11 = Main.tile[num239, num241].nactive() && Main.tileSolid[(int)Main.tile[num239, num241].type] && !Main.tileSolidTop[(int)Main.tile[num239, num241].type];
							if (flag11)
								num242 = 1;
							else
							{
								while (num242 < 150 && num241 + num242 < Main.maxTilesY)
								{
									int num243 = num241 + num242;
									bool flag12 = Main.tile[num239, num243].nactive() && Main.tileSolid[(int)Main.tile[num239, num243].type] && !Main.tileSolidTop[(int)Main.tile[num239, num243].type];
									if (flag12)
									{
										num242--;
										break;
									}
									int num = num242;
									num242 = num + 1;
								}
							}
							num240 += num242;

							bool flag13 = true;
							if (flag13 && Main.tile[num239, num240].lava())
								flag13 = false;
							if (flag13 && !Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
								flag13 = false;

							if (flag13)
							{
								npc.localAI[1] = (float)(num239 * 16 + 8);
								npc.localAI[2] = (float)(num240 * 16 + 16);
								break;
							}
						}
					}
					if (num238 >= 100)
					{
						Vector2 bottom = Main.player[(int)Player.FindClosest(npc.position, npc.width, npc.height)].Bottom;
						npc.localAI[1] = bottom.X;
						npc.localAI[2] = bottom.Y;
					}
				}
			}

			if (!Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
				npc.ai[2] += 1f;
			if (Math.Abs(npc.Top.Y - Main.player[npc.target].Bottom.Y) > 320f)
				npc.ai[2] += 1f;

			Dust dust;
			if (npc.ai[1] == 5f)
			{
				flag8 = true;
				npc.aiAction = 1;
				npc.ai[0] += ((enraged || Config.BossRushXerocCurse) ? 2f : 1f);
				num234 = MathHelper.Clamp((60f - npc.ai[0]) / 60f, 0f, 1f);
				num234 = 0.5f + num234 * 0.5f;

				if (npc.ai[0] >= 60f)
					flag9 = true;

				if (!flag9)
				{
					int num;
					for (int num245 = 0; num245 < 10; num245 = num + 1)
					{
						int num246 = Dust.NewDust(npc.position + Vector2.UnitX * -20f, npc.width + 40, npc.height, 4, npc.velocity.X, npc.velocity.Y, 150, new Color(78, 136, 255, 80), 2f);
						Main.dust[num246].noGravity = true;
						dust = Main.dust[num246];
						dust.velocity *= 0.5f;
						num = num245;
					}
				}

				if (npc.ai[0] == 60f)
					Gore.NewGore(npc.Center + new Vector2(-40f, (float)(-(float)npc.height / 2)), npc.velocity, 734, 1f);

				if (npc.ai[0] >= 60f && Main.netMode != 1)
				{
					npc.Bottom = new Vector2(npc.localAI[1], npc.localAI[2]);
					npc.ai[1] = 6f;
					npc.ai[0] = 0f;
					npc.netUpdate = true;
				}

				if (Main.netMode == 1 && npc.ai[0] >= 120f)
				{
					npc.ai[1] = 6f;
					npc.ai[0] = 0f;
				}
			}
			else if (npc.ai[1] == 6f)
			{
				flag8 = true;
				npc.aiAction = 0;
				npc.ai[0] += ((enraged || Config.BossRushXerocCurse) ? 2f : 1f);
				num234 = MathHelper.Clamp(npc.ai[0] / 30f, 0f, 1f);
				num234 = 0.5f + num234 * 0.5f;

				if (npc.ai[0] >= 30f && Main.netMode != 1)
				{
					npc.ai[1] = 0f;
					npc.ai[0] = 0f;
					npc.netUpdate = true;
					npc.TargetClosest(true);
				}

				if (Main.netMode == 1 && npc.ai[0] >= 60f)
				{
					npc.ai[1] = 0f;
					npc.ai[0] = 0f;
					npc.TargetClosest(true);
				}

				int num;
				for (int num247 = 0; num247 < 10; num247 = num + 1)
				{
					int num248 = Dust.NewDust(npc.position + Vector2.UnitX * -20f, npc.width + 40, npc.height, 4, npc.velocity.X, npc.velocity.Y, 150, new Color(78, 136, 255, 80), 2f);
					Main.dust[num248].noGravity = true;
					dust = Main.dust[num248];
					dust.velocity *= 2f;
					num = num247;
				}
			}

			npc.dontTakeDamage = (npc.hide = flag9);

			if (npc.velocity.Y == 0f)
			{
				npc.velocity.X = npc.velocity.X * 0.8f;

				if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
					npc.velocity.X = 0f;

				if (!flag8)
				{
					npc.ai[0] += (enraged || Config.BossRushXerocCurse) ? 4f : 2f;
					if ((double)npc.life < (double)npc.lifeMax * 0.8)
						npc.ai[0] += 1f;
					if ((double)npc.life < (double)npc.lifeMax * 0.6)
						npc.ai[0] += 1f;
					if ((double)npc.life < (double)npc.lifeMax * 0.4)
						npc.ai[0] += 2f;
					if ((double)npc.life < (double)npc.lifeMax * 0.2)
						npc.ai[0] += 3f;
					if ((double)npc.life < (double)npc.lifeMax * 0.1)
						npc.ai[0] += 4f;

					if (npc.ai[0] >= 0f)
					{
						npc.netUpdate = true;
						npc.TargetClosest(true);

						if (npc.ai[1] == 3f)
						{
							npc.velocity.Y = -26f;
							npc.velocity.X = npc.velocity.X + 7f * (float)npc.direction;
							npc.ai[0] = -200f;
							npc.ai[1] = 0f;
						}
						else if (npc.ai[1] == 2f)
						{
							npc.velocity.Y = -12f;
							npc.velocity.X = npc.velocity.X + 9f * (float)npc.direction;
							npc.ai[0] = -120f;
							npc.ai[1] += 1f;
						}
						else
						{
							npc.velocity.Y = -16f;
							npc.velocity.X = npc.velocity.X + 8f * (float)npc.direction;
							npc.ai[0] = -120f;
							npc.ai[1] += 1f;
						}
					}
					else if (npc.ai[0] >= -30f)
						npc.aiAction = 1;
				}
			}
			else if (npc.target < 255 && ((npc.direction == 1 && npc.velocity.X < 3f) || (npc.direction == -1 && npc.velocity.X > -3f)))
			{
				if ((npc.direction == -1 && (double)npc.velocity.X < 0.1) || (npc.direction == 1 && (double)npc.velocity.X > -0.1))
					npc.velocity.X = npc.velocity.X + 0.2f * (float)npc.direction;
				else
					npc.velocity.X = npc.velocity.X * 0.93f;
			}

			int num249 = Dust.NewDust(npc.position, npc.width, npc.height, 4, npc.velocity.X, npc.velocity.Y, 255, new Color(0, 80, 255, 80), npc.scale * 1.2f);
			Main.dust[num249].noGravity = true;
			dust = Main.dust[num249];
			dust.velocity *= 0.5f;

			if (npc.life > 0)
			{
				float num250 = (float)npc.life / (float)npc.lifeMax;
				num250 = num250 * 0.5f + 0.75f;
				num250 *= num234;
				if (num250 != npc.scale)
				{
					npc.position.X = npc.position.X + (float)(npc.width / 2);
					npc.position.Y = npc.position.Y + (float)npc.height;
					npc.scale = num250;
					npc.width = (int)(98f * npc.scale);
					npc.height = (int)(92f * npc.scale);
					npc.position.X = npc.position.X - (float)(npc.width / 2);
					npc.position.Y = npc.position.Y - (float)npc.height;
				}

				if (Main.netMode != 1)
				{
					int num251 = (int)((double)npc.lifeMax * 0.05);
					if ((float)(npc.life + num251) < npc.ai[3])
					{
						npc.ai[3] = (float)npc.life;
						int num252 = Main.rand.Next(1, 4);
						int num;
						for (int num253 = 0; num253 < num252; num253 = num + 1)
						{
							int x = (int)(npc.position.X + (float)Main.rand.Next(npc.width - 32));
							int y = (int)(npc.position.Y + (float)Main.rand.Next(npc.height - 32));

							int num254 = 1;
							if (Main.rand.Next(4) == 0)
								num254 = 535;

							int num255 = NPC.NewNPC(x, y, num254, 0, 0f, 0f, 0f, 0f, 255);
							Main.npc[num255].SetDefaults(num254, -1f);
							Main.npc[num255].velocity.X = (float)Main.rand.Next(-15, 16) * 0.1f;
							Main.npc[num255].velocity.Y = (float)Main.rand.Next(-30, 1) * 0.1f;
							Main.npc[num255].ai[0] = (float)(-1000 * Main.rand.Next(3));
							Main.npc[num255].ai[1] = 0f;

							if (Main.netMode == 2 && num255 < 200)
								NetMessage.SendData(23, -1, -1, null, num255, 0f, 0f, 0f, 0, 0, 0);

							num = num253;
						}
					}
				}
			}
			return false;
		}
		#endregion

		#region Boss Rush Brain of Cthulhu AI
		private bool BossRushBrainofCthulhuAI(NPC npc)
		{
			NPC.crimsonBoss = npc.whoAmI;
			npc.dontTakeDamage = false;

			if (Main.netMode != 1 && npc.localAI[0] == 0f)
			{
				npc.localAI[0] = 1f;
				int num;
				for (int num789 = 0; num789 < 20; num789 = num + 1)
				{
					float num790 = npc.Center.X;
					float num791 = npc.Center.Y;
					num790 += (float)Main.rand.Next(-npc.width, npc.width);
					num791 += (float)Main.rand.Next(-npc.height, npc.height);
					int num792 = NPC.NewNPC((int)num790, (int)num791, 267, 0, 0f, 0f, 0f, 0f, 255);
					Main.npc[num792].velocity = new Vector2((float)Main.rand.Next(-30, 31) * 0.1f, (float)Main.rand.Next(-30, 31) * 0.1f);
					Main.npc[num792].netUpdate = true;
					num = num789;
				}
			}

			if (npc.ai[0] < 0f)
			{
				if (npc.localAI[2] == 0f)
				{
					Main.PlaySound(3, (int)npc.position.X, (int)npc.position.Y, 1, 1f, 0f);
					npc.localAI[2] = 1f;

					Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 392, 1f);
					Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 393, 1f);
					Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 394, 1f);
					Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 395, 1f);

					int num;
					for (int num794 = 0; num794 < 20; num794 = num + 1)
					{
						Dust.NewDust(npc.position, npc.width, npc.height, 5, (float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f, 0, default(Color), 1f);
						num = num794;
					}

					Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
				}

				newAI[0] += 1f;
				if (newAI[0] >= 60f)
				{
					newAI[0] = 0f;
					if (Main.netMode != 1 && NPC.CountNPCS(NPCID.Creeper) < 15)
					{
						int creeper = NPC.NewNPC((int)(npc.position.X + (float)(npc.width / 2) + npc.velocity.X), (int)(npc.position.Y + (float)(npc.height / 2) + npc.velocity.Y), 267, 0, 0f, 0f, 0f, 0f, 255);
						Main.npc[creeper].netUpdate = true;
					}
					npc.netUpdate = true;
				}

				npc.knockBackResist = 0f;
				npc.TargetClosest(true);

				Vector2 vector98 = new Vector2(npc.Center.X, npc.Center.Y);
				float num795 = Main.player[npc.target].Center.X - vector98.X;
				float num796 = Main.player[npc.target].Center.Y - vector98.Y;
				float num797 = (float)Math.Sqrt((double)(num795 * num795 + num796 * num796));
				float num798 = ((enraged || Config.BossRushXerocCurse) ? 21f : 14f);

				num797 = num798 / num797;
				num795 *= num797;
				num796 *= num797;

				npc.velocity.X = (npc.velocity.X * 50f + num795) / 51f;
				npc.velocity.Y = (npc.velocity.Y * 50f + num796) / 51f;

				if (npc.ai[0] == -1f)
				{
					if (Main.netMode != 1)
					{
						npc.localAI[1] += 1f;
						if (npc.justHit)
							npc.localAI[1] -= (float)Main.rand.Next(5);

						int num799 = 60 + Main.rand.Next(120);
						if (Main.netMode != 0)
							num799 += Main.rand.Next(30, 90);

						if (npc.localAI[1] >= (float)num799)
						{
							npc.localAI[1] = 0f;
							npc.TargetClosest(true);

							int num800 = 0;
							int num801;
							int num802;

							while (true)
							{
								num800++;
								num801 = (int)Main.player[npc.target].Center.X / 16;
								num802 = (int)Main.player[npc.target].Center.Y / 16;

								if (Main.rand.Next(2) == 0)
									num801 += Main.rand.Next(7, 13);
								else
									num801 -= Main.rand.Next(7, 13);

								if (Main.rand.Next(2) == 0)
									num802 += Main.rand.Next(7, 13);
								else
									num802 -= Main.rand.Next(7, 13);

								if (!WorldGen.SolidTile(num801, num802))
									break;
								if (num800 > 100)
									goto Block_2784;
							}

							npc.ai[3] = 0f;
							npc.ai[0] = -2f;
							npc.ai[1] = (float)num801;
							npc.ai[2] = (float)num802;

							npc.netUpdate = true;
							npc.netSpam = 0;
							Block_2784:;
						}
					}
				}
				else if (npc.ai[0] == -2f)
				{
					npc.velocity *= 0.9f;

					if (Main.netMode != 0)
						npc.ai[3] += 15f;
					else
						npc.ai[3] += 25f;

					if (npc.ai[3] >= 255f)
					{
						npc.ai[3] = 255f;
						npc.ai[0] = -3f;

						npc.position.X = npc.ai[1] * 16f - (float)(npc.width / 2);
						npc.position.Y = npc.ai[2] * 16f - (float)(npc.height / 2);

						Main.PlaySound(SoundID.Item8, npc.Center);
						npc.netUpdate = true;
						npc.netSpam = 0;
					}
					npc.alpha = (int)npc.ai[3];
				}
				else if (npc.ai[0] == -3f)
				{
					if (Main.netMode != 0)
						npc.ai[3] -= 15f;
					else
						npc.ai[3] -= 25f;

					if (npc.ai[3] <= 0f)
					{
						npc.ai[3] = 0f;
						npc.ai[0] = -1f;
						npc.netUpdate = true;
						npc.netSpam = 0;
					}
					npc.alpha = (int)npc.ai[3];
				}
			}
			else
			{
				npc.TargetClosest(true);

				Vector2 vector99 = new Vector2(npc.Center.X, npc.Center.Y);
				float num803 = Main.player[npc.target].Center.X - vector99.X;
				float num804 = Main.player[npc.target].Center.Y - vector99.Y;
				float num805 = (float)Math.Sqrt((double)(num803 * num803 + num804 * num804));
				float num806 = ((enraged || Config.BossRushXerocCurse) ? 3f : 2f);

				if (num805 < num806)
				{
					npc.velocity.X = num803;
					npc.velocity.Y = num804;
				}
				else
				{
					num805 = num806 / num805;
					npc.velocity.X = num803 * num805;
					npc.velocity.Y = num804 * num805;
				}

				if (npc.ai[0] == 0f)
				{
					if (Main.netMode != 1)
					{
						int num807 = 0;
						int num;
						for (int num808 = 0; num808 < 200; num808 = num + 1)
						{
							if (Main.npc[num808].active && Main.npc[num808].type == 267)
								num807++;

							num = num808;
						}
						if (num807 == 0)
						{
							npc.ai[0] = -1f;
							npc.localAI[1] = 0f;
							npc.alpha = 0;
							npc.netUpdate = true;
						}

						npc.localAI[1] += 1f;
						if (npc.localAI[1] >= (float)(120 + Main.rand.Next(120)))
						{
							npc.localAI[1] = 0f;
							npc.TargetClosest(true);

							int num809 = 0;
							int num810;
							int num811;

							while (true)
							{
								num809++;
								num810 = (int)Main.player[npc.target].Center.X / 16;
								num811 = (int)Main.player[npc.target].Center.Y / 16;
								num810 += Main.rand.Next(-50, 51);
								num811 += Main.rand.Next(-50, 51);

								if (!WorldGen.SolidTile(num810, num811) && Collision.CanHit(new Vector2((float)(num810 * 16), (float)(num811 * 16)), 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
									break;
								if (num809 > 100)
									goto Block_2801;
							}

							npc.ai[0] = 1f;
							npc.ai[1] = (float)num810;
							npc.ai[2] = (float)num811;

							npc.netUpdate = true;
							Block_2801:;
						}
					}
				}
				else if (npc.ai[0] == 1f)
				{
					npc.alpha += 5;
					if (npc.alpha >= 255)
					{
						Main.PlaySound(SoundID.Item8, npc.Center);
						npc.alpha = 255;
						npc.position.X = npc.ai[1] * 16f - (float)(npc.width / 2);
						npc.position.Y = npc.ai[2] * 16f - (float)(npc.height / 2);
						npc.ai[0] = 2f;
					}
				}
				else if (npc.ai[0] == 2f)
				{
					npc.alpha -= 5;
					if (npc.alpha <= 0)
					{
						npc.alpha = 0;
						npc.ai[0] = 0f;
					}
				}
			}

			if (Main.player[npc.target].dead)
			{
				if (npc.localAI[3] < 120f)
					npc.localAI[3] += 1f;

				if (npc.localAI[3] > 60f)
					npc.velocity.Y = npc.velocity.Y + (npc.localAI[3] - 60f) * 0.25f;

				npc.ai[0] = 2f;
				npc.alpha = 10;
				return false;
			}
			if (npc.localAI[3] > 0f)
				npc.localAI[3] -= 1f;

			return false;
		}
		#endregion

		#region Boss Rush Eater of Worlds AI
		private bool BossRushEaterofWorldsAI(NPC npc)
		{
			Main.player[npc.target].ZoneCorrupt = true;
			npc.reflectingProjectiles = true;

			if (!Main.player[npc.target].dead)
				newAI[0] += ((enraged || Config.BossRushXerocCurse) ? 6f : 3f);
			if (newAI[0] >= 180f)
			{
				newAI[0] = 0f;
				if (Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					Vector2 vector34 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num349 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector34.X;
					float num350 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector34.Y;
					float num351 = (float)Math.Sqrt((double)(num349 * num349 + num350 * num350));
					num349 *= num351;
					num350 *= num351;

					if (Main.netMode != 1)
					{
						float num418 = ((enraged || Config.BossRushXerocCurse) ? 18f : 12f);
						int num419 = 12;
						int num420 = 96;
						num351 = (float)Math.Sqrt((double)(num349 * num349 + num350 * num350));
						num351 = num418 / num351;
						num349 *= num351;
						num350 *= num351;
						num349 += (float)Main.rand.Next(-40, 41) * 0.05f;
						num350 += (float)Main.rand.Next(-40, 41) * 0.05f;
						vector34.X += num349 * 4f;
						vector34.Y += num350 * 4f;
						Projectile.NewProjectile(vector34.X, vector34.Y, num349, num350, num420, num419, 0f, Main.myPlayer, 0f, 0f);
					}
				}
			}
			return false;
		}
		#endregion

		#region Boss Rush Queen Bee AI
		private bool BossRushQueenBeeAI(NPC npc)
		{
			int num592 = 0;
			int variable;
			for (int num593 = 0; num593 < 255; num593 = variable + 1)
			{
				if (Main.player[num593].active && !Main.player[num593].dead && (npc.Center - Main.player[num593].Center).Length() < 1000f)
					num592++;

				variable = num593;
			}

			int num594 = (int)(20f * (1f - (float)npc.life / (float)npc.lifeMax));
			npc.defense = npc.defDefense + num594;

			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest(true);

			bool dead4 = Main.player[npc.target].dead;
			if (dead4)
			{
				if ((double)npc.position.Y < Main.worldSurface * 16.0 + 2000.0)
					npc.velocity.Y = npc.velocity.Y + 0.04f;

				if (npc.position.X < (float)(Main.maxTilesX * 8))
					npc.velocity.X = npc.velocity.X - 0.04f;
				else
					npc.velocity.X = npc.velocity.X + 0.04f;

				if (npc.timeLeft > 10)
					npc.timeLeft = 10;
			}
			else if (npc.ai[0] == -1f)
			{
				if (Main.netMode != 1)
				{
					float num595 = npc.ai[1];
					int num596;
					do
					{
						num596 = Main.rand.Next(3);
						if (num596 == 1)
							num596 = 2;
						else if (num596 == 2)
							num596 = 3;
					}
					while ((float)num596 == num595);

					npc.ai[0] = (float)num596;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
				}
			}
			else if (npc.ai[0] == 0f)
			{
				if (npc.ai[1] > 10f && npc.ai[1] % 2f == 0f)
				{
					npc.ai[0] = -1f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
					return false;
				}

				if (npc.ai[1] % 2f == 0f)
				{
					npc.TargetClosest(true);

					if (Math.Abs(npc.position.Y + (float)(npc.height / 2) - (Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2))) < 20f)
					{
						npc.localAI[0] = 1f;
						npc.ai[1] += 1f;
						npc.ai[2] = 0f;

						float num598 = 24f;
						Vector2 vector74 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						float num599 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector74.X;
						float num600 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector74.Y;
						float num601 = (float)Math.Sqrt((double)(num599 * num599 + num600 * num600));

						num601 = num598 / num601;
						npc.velocity.X = num599 * num601;
						npc.velocity.Y = num600 * num601;

						npc.spriteDirection = npc.direction;
						Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
						return false;
					}

					npc.localAI[0] = 0f;
					float num602 = 18f;
					float num603 = 0.4f;

					if (npc.position.Y + (float)(npc.height / 2) < Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2))
						npc.velocity.Y = npc.velocity.Y + num603;
					else
						npc.velocity.Y = npc.velocity.Y - num603;

					if (npc.velocity.Y < -12f)
						npc.velocity.Y = -num602;
					if (npc.velocity.Y > 12f)
						npc.velocity.Y = num602;

					if (Math.Abs(npc.position.X + (float)(npc.width / 2) - (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))) > 600f)
						npc.velocity.X = npc.velocity.X + 0.15f * (float)npc.direction;
					else if (Math.Abs(npc.position.X + (float)(npc.width / 2) - (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))) < 300f)
						npc.velocity.X = npc.velocity.X - 0.15f * (float)npc.direction;
					else
						npc.velocity.X = npc.velocity.X * 0.8f;

					if (npc.velocity.X < -16f)
						npc.velocity.X = -16f;
					if (npc.velocity.X > 16f)
						npc.velocity.X = 16f;

					npc.spriteDirection = npc.direction;
				}
				else
				{
					if (npc.velocity.X < 0f)
						npc.direction = -1;
					else
						npc.direction = 1;
					npc.spriteDirection = npc.direction;

					int num605 = 1;
					if (npc.position.X + (float)(npc.width / 2) < Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))
						num605 = -1;

					int num604 = ((enraged || Config.BossRushXerocCurse) ? 150 : 250);
					if (npc.direction == num605 && Math.Abs(npc.position.X + (float)(npc.width / 2) - (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))) > (float)num604)
						npc.ai[2] = 1f;

					if (npc.ai[2] != 1f)
					{
						npc.localAI[0] = 1f;
						return false;
					}

					npc.TargetClosest(true);
					npc.spriteDirection = npc.direction;
					npc.localAI[0] = 0f;

					npc.velocity *= 0.9f;
					if (npc.life < npc.lifeMax / 2)
						npc.velocity *= 0.9f;
					if (npc.life < npc.lifeMax / 3)
						npc.velocity *= 0.9f;
					if (npc.life < npc.lifeMax / 5)
						npc.velocity *= 0.9f;

					if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < 0.25f)
					{
						npc.ai[2] = 0f;
						npc.ai[1] += 1f;
					}
				}
			}
			else if (npc.ai[0] == 2f)
			{
				npc.TargetClosest(true);
				npc.spriteDirection = npc.direction;

				float num607 = 12f;
				float num608 = 0.1f;
				Vector2 vector75 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num609 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector75.X;
				float num610 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 200f - vector75.Y;

				float num611 = (float)Math.Sqrt((double)(num609 * num609 + num610 * num610));
				if (num611 < 200f)
				{
					npc.ai[0] = 1f;
					npc.ai[1] = 0f;
					npc.netUpdate = true;
					return false;
				}
				num611 = num607 / num611;

				if (npc.velocity.X < num609)
				{
					npc.velocity.X = npc.velocity.X + num608;
					if (npc.velocity.X < 0f && num609 > 0f)
						npc.velocity.X = npc.velocity.X + num608;
				}
				else if (npc.velocity.X > num609)
				{
					npc.velocity.X = npc.velocity.X - num608;
					if (npc.velocity.X > 0f && num609 < 0f)
						npc.velocity.X = npc.velocity.X - num608;
				}
				if (npc.velocity.Y < num610)
				{
					npc.velocity.Y = npc.velocity.Y + num608;
					if (npc.velocity.Y < 0f && num610 > 0f)
						npc.velocity.Y = npc.velocity.Y + num608;
				}
				else if (npc.velocity.Y > num610)
				{
					npc.velocity.Y = npc.velocity.Y - num608;
					if (npc.velocity.Y > 0f && num610 < 0f)
						npc.velocity.Y = npc.velocity.Y - num608;
				}
			}
			else if (npc.ai[0] == 1f)
			{
				npc.localAI[0] = 0f;
				npc.TargetClosest(true);

				Vector2 vector76 = new Vector2(npc.position.X + (float)(npc.width / 2) + (float)(Main.rand.Next(20) * npc.direction), npc.position.Y + (float)npc.height * 0.8f);
				Vector2 vector77 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num612 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector77.X;
				float num613 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector77.Y;
				float num614 = (float)Math.Sqrt((double)(num612 * num612 + num613 * num613));

				npc.ai[1] += 1f;
				npc.ai[1] += (float)(num592 / 2);
				bool flag38 = false;

				if (npc.ai[1] > 10f)
				{
					npc.ai[1] = 0f;
					npc.ai[2] += 1f;
					flag38 = true;
				}

				if (Collision.CanHit(vector76, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) && flag38)
				{
					Main.PlaySound(3, (int)npc.position.X, (int)npc.position.Y, 1, 1f, 0f);
					if (Main.netMode != 1)
					{
						int num615 = Main.rand.Next(210, 212);
						int num616 = NPC.NewNPC((int)vector76.X, (int)vector76.Y, num615, 0, 0f, 0f, 0f, 0f, 255);
						Main.npc[num616].velocity.X = (float)Main.rand.Next(-200, 201) * 0.002f;
						Main.npc[num616].velocity.Y = (float)Main.rand.Next(-200, 201) * 0.002f;
						Main.npc[num616].localAI[0] = 60f;
						Main.npc[num616].netUpdate = true;
					}
				}

				if (num614 > 400f || !Collision.CanHit(new Vector2(vector76.X, vector76.Y - 30f), 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					float num617 = 14f;
					float num618 = 0.1f;
					vector77 = vector76;

					num612 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector77.X;
					num613 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector77.Y;
					num614 = (float)Math.Sqrt((double)(num612 * num612 + num613 * num613));
					num614 = num617 / num614;

					if (npc.velocity.X < num612)
					{
						npc.velocity.X = npc.velocity.X + num618;
						if (npc.velocity.X < 0f && num612 > 0f)
							npc.velocity.X = npc.velocity.X + num618;
					}
					else if (npc.velocity.X > num612)
					{
						npc.velocity.X = npc.velocity.X - num618;
						if (npc.velocity.X > 0f && num612 < 0f)
							npc.velocity.X = npc.velocity.X - num618;
					}
					if (npc.velocity.Y < num613)
					{
						npc.velocity.Y = npc.velocity.Y + num618;
						if (npc.velocity.Y < 0f && num613 > 0f)
							npc.velocity.Y = npc.velocity.Y + num618;
					}
					else if (npc.velocity.Y > num613)
					{
						npc.velocity.Y = npc.velocity.Y - num618;
						if (npc.velocity.Y > 0f && num613 < 0f)
							npc.velocity.Y = npc.velocity.Y - num618;
					}
				}
				else
					npc.velocity *= 0.9f;

				npc.spriteDirection = npc.direction;

				if (npc.ai[2] > 5f)
				{
					npc.ai[0] = -1f;
					npc.ai[1] = 1f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 3f)
			{
				float num619 = 6f;
				float num620 = 0.075f;

				Vector2 vector78 = new Vector2(npc.position.X + (float)(npc.width / 2) + (float)(Main.rand.Next(20) * npc.direction), npc.position.Y + (float)npc.height * 0.8f);
				Vector2 vector79 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num621 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector79.X;
				float num622 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 300f - vector79.Y;
				float num623 = (float)Math.Sqrt((double)(num621 * num621 + num622 * num622));

				npc.ai[1] += 1f;
				bool flag39 = false;
				if (npc.ai[1] % 15f == 14f)
					flag39 = true;

				if (flag39 && npc.position.Y + (float)npc.height < Main.player[npc.target].position.Y && Collision.CanHit(vector78, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					Main.PlaySound(SoundID.Item17, npc.position);
					if (Main.netMode != 1)
					{
						float num624 = ((enraged || Config.BossRushXerocCurse) ? 24f : 16f);
						float num625 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector78.X + (float)Main.rand.Next(-80, 81);
						float num626 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector78.Y + (float)Main.rand.Next(-40, 41);
						float num627 = (float)Math.Sqrt((double)(num625 * num625 + num626 * num626));

						num627 = num624 / num627;
						num625 *= num627;
						num626 *= num627;

						int num628 = 11;
						int num629 = 55;
						int num630 = Projectile.NewProjectile(vector78.X, vector78.Y, num625, num626, num629, num628, 0f, Main.myPlayer, 0f, 0f);
						Main.projectile[num630].timeLeft = 300;
					}
				}

				if (!Collision.CanHit(new Vector2(vector78.X, vector78.Y - 30f), 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					num619 = 14f;
					num620 = 0.1f;
					vector79 = vector78;

					num621 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector79.X;
					num622 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector79.Y;
					num623 = (float)Math.Sqrt((double)(num621 * num621 + num622 * num622));
					num623 = num619 / num623;

					if (npc.velocity.X < num621)
					{
						npc.velocity.X = npc.velocity.X + num620;
						if (npc.velocity.X < 0f && num621 > 0f)
							npc.velocity.X = npc.velocity.X + num620;
					}
					else if (npc.velocity.X > num621)
					{
						npc.velocity.X = npc.velocity.X - num620;
						if (npc.velocity.X > 0f && num621 < 0f)
							npc.velocity.X = npc.velocity.X - num620;
					}
					if (npc.velocity.Y < num622)
					{
						npc.velocity.Y = npc.velocity.Y + num620;
						if (npc.velocity.Y < 0f && num622 > 0f)
							npc.velocity.Y = npc.velocity.Y + num620;
					}
					else if (npc.velocity.Y > num622)
					{
						npc.velocity.Y = npc.velocity.Y - num620;
						if (npc.velocity.Y > 0f && num622 < 0f)
							npc.velocity.Y = npc.velocity.Y - num620;
					}
				}
				else if (num623 > 100f)
				{
					npc.TargetClosest(true);
					npc.spriteDirection = npc.direction;

					num623 = num619 / num623;
					if (npc.velocity.X < num621)
					{
						npc.velocity.X = npc.velocity.X + num620;
						if (npc.velocity.X < 0f && num621 > 0f)
							npc.velocity.X = npc.velocity.X + num620 * 2f;
					}
					else if (npc.velocity.X > num621)
					{
						npc.velocity.X = npc.velocity.X - num620;
						if (npc.velocity.X > 0f && num621 < 0f)
							npc.velocity.X = npc.velocity.X - num620 * 2f;
					}
					if (npc.velocity.Y < num622)
					{
						npc.velocity.Y = npc.velocity.Y + num620;
						if (npc.velocity.Y < 0f && num622 > 0f)
							npc.velocity.Y = npc.velocity.Y + num620 * 2f;
					}
					else if (npc.velocity.Y > num622)
					{
						npc.velocity.Y = npc.velocity.Y - num620;
						if (npc.velocity.Y > 0f && num622 < 0f)
							npc.velocity.Y = npc.velocity.Y - num620 * 2f;
					}
				}

				if (npc.ai[1] > 400f)
				{
					npc.ai[0] = -1f;
					npc.ai[1] = 3f;
					npc.netUpdate = true;
				}
			}
			return false;
		}
		#endregion

		#region Boss Rush Duke Fishron AI
		private bool BossRushDukeFishronAI(NPC npc)
		{
			if (npc.ai[0] == -1f || npc.ai[0] == 4f || npc.ai[0] == 9f)
				npc.dontTakeDamage = true;
			else if (npc.ai[0] <= 8f)
				npc.dontTakeDamage = false;

			float newDamage = (0.6f * Main.damageMultiplier);
			bool flag = (double)npc.life <= (double)npc.lifeMax * 0.99;
			bool flag2 = (double)npc.life <= (double)npc.lifeMax * 0.15;
			bool flag3 = npc.ai[0] > 4f;
			bool flag4 = npc.ai[0] > 9f;
			bool flag5 = npc.ai[3] < 10f;
			Vector2 vector = npc.Center;

			if (flag4)
			{
				newAI[0] += 1f;
				if (newAI[0] >= 480f)
				{
					newAI[0] = 0f;

					if (Main.netMode != 1)
						Projectile.NewProjectile(vector.X, vector.Y, 0f, 0f, 385, 0, 0f, Main.myPlayer, 1f, (float)(npc.target + 1));

					npc.netUpdate = true;
				}
				npc.damage = (int)((float)npc.defDamage * 1.5f * newDamage);
				npc.defense = 38;
			}
			else if (flag3)
			{
				npc.damage = (int)((float)npc.defDamage * 2.2f * newDamage);
				npc.defense = (int)((float)npc.defDefense * 0.8f);
			}
			else
			{
				npc.damage = npc.defDamage;
				npc.defense = npc.defDefense;
			}

			int num2 = 35;
			float num3 = 0.65f;
			float scaleFactor = 9.5f;
			if (flag4)
			{
				num3 = ((enraged || Config.BossRushXerocCurse) ? 0.9f : 0.8f);
				scaleFactor = ((enraged || Config.BossRushXerocCurse) ? 15f : 13f);
				num2 = ((enraged || Config.BossRushXerocCurse) ? 22 : 25);
			}
			else if (flag3 & flag5)
			{
				num3 = ((enraged || Config.BossRushXerocCurse) ? 0.8f : 0.7f);
				scaleFactor = ((enraged || Config.BossRushXerocCurse) ? 13f : 11f);
				num2 = ((enraged || Config.BossRushXerocCurse) ? 30 : 35);
			}
			else if (flag5 && !flag3 && !flag4)
				num2 = 25;

			int num4 = 24;
			float num5 = 18f;
			if (flag4)
			{
				num4 = ((enraged || Config.BossRushXerocCurse) ? 18 : 20);
				num5 = ((enraged || Config.BossRushXerocCurse) ? 30f : 28f);
			}
			else if (flag5 & flag3)
			{
				num4 = ((enraged || Config.BossRushXerocCurse) ? 21 : 23);
				num5 = ((enraged || Config.BossRushXerocCurse) ? 25f : 22f);
			}

			int num6 = 80;
			int num7 = 4;
			float num8 = 0.3f;
			float scaleFactor2 = 5f;
			int num9 = 90;
			int num10 = 180;
			int num11 = 180;
			int num12 = 30;
			int num13 = 120;
			int num14 = 4;
			float scaleFactor3 = 6f;
			float scaleFactor4 = 20f;
			float num15 = 6.28318548f / (float)(num13 / 2);
			int num16 = 75;

			Player player = Main.player[npc.target];
			if (npc.target < 0 || npc.target == 255 || player.dead || !player.active)
			{
				npc.TargetClosest(true);
				player = Main.player[npc.target];
				npc.netUpdate = true;
			}

			if (player.dead || Vector2.Distance(player.Center, vector) > 5600f)
			{
				npc.velocity.Y = npc.velocity.Y - 0.4f;

				if (npc.timeLeft > 10)
					npc.timeLeft = 10;

				if (npc.ai[0] > 4f)
					npc.ai[0] = 5f;
				else
					npc.ai[0] = 0f;

				npc.ai[2] = 0f;
			}

			if (player.position.Y < 800f || (double)player.position.Y > Main.worldSurface * 16.0 || (player.position.X > 6400f && player.position.X < (float)(Main.maxTilesX * 16 - 6400)))
			{
				num2 = 20;
				num5 += 6f;
			}

			if (npc.localAI[0] == 0f)
			{
				npc.localAI[0] = 1f;
				npc.alpha = 255;
				npc.rotation = 0f;
				if (Main.netMode != 1)
				{
					npc.ai[0] = -1f;
					npc.netUpdate = true;
				}
			}

			float num17 = (float)Math.Atan2((double)(player.Center.Y - vector.Y), (double)(player.Center.X - vector.X));
			if (npc.spriteDirection == 1)
				num17 += 3.14159274f;
			if (num17 < 0f)
				num17 += 6.28318548f;
			if (num17 > 6.28318548f)
				num17 -= 6.28318548f;
			if (npc.ai[0] == -1f)
				num17 = 0f;
			if (npc.ai[0] == 3f)
				num17 = 0f;
			if (npc.ai[0] == 4f)
				num17 = 0f;
			if (npc.ai[0] == 8f)
				num17 = 0f;

			float num18 = 0.04f;
			if (npc.ai[0] == 1f || npc.ai[0] == 6f)
				num18 = 0f;
			if (npc.ai[0] == 7f)
				num18 = 0f;
			if (npc.ai[0] == 3f)
				num18 = 0.01f;
			if (npc.ai[0] == 4f)
				num18 = 0.01f;
			if (npc.ai[0] == 8f)
				num18 = 0.01f;

			if (npc.rotation < num17)
			{
				if ((double)(num17 - npc.rotation) > 3.1415926535897931)
					npc.rotation -= num18;
				else
					npc.rotation += num18;
			}

			if (npc.rotation > num17)
			{
				if ((double)(npc.rotation - num17) > 3.1415926535897931)
					npc.rotation += num18;
				else
					npc.rotation -= num18;
			}

			if (npc.rotation > num17 - num18 && npc.rotation < num17 + num18)
				npc.rotation = num17;
			if (npc.rotation < 0f)
				npc.rotation += 6.28318548f;
			if (npc.rotation > 6.28318548f)
				npc.rotation -= 6.28318548f;
			if (npc.rotation > num17 - num18 && npc.rotation < num17 + num18)
				npc.rotation = num17;

			if (npc.ai[0] != -1f && npc.ai[0] < 9f)
			{
				if (Collision.SolidCollision(npc.position, npc.width, npc.height))
					npc.alpha += 15;
				else
					npc.alpha -= 15;

				if (npc.alpha < 0)
					npc.alpha = 0;
				if (npc.alpha > 150)
					npc.alpha = 150;
			}
			if (npc.ai[0] == -1f)
			{
				npc.velocity *= 0.98f;

				int num19 = Math.Sign(player.Center.X - vector.X);
				if (num19 != 0)
				{
					npc.direction = num19;
					npc.spriteDirection = -npc.direction;
				}

				if (npc.ai[2] > 20f)
				{
					npc.velocity.Y = -2f;
					npc.alpha -= 5;

					if (Collision.SolidCollision(npc.position, npc.width, npc.height))
						npc.alpha += 15;
					if (npc.alpha < 0)
						npc.alpha = 0;
					if (npc.alpha > 150)
						npc.alpha = 150;
				}

				if (npc.ai[2] == (float)(num9 - 30))
				{
					int num20 = 36;
					for (int i = 0; i < num20; i++)
					{
						Vector2 expr_80F = (Vector2.Normalize(npc.velocity) * new Vector2((float)npc.width / 2f, (float)npc.height) * 0.75f * 0.5f).RotatedBy((double)((float)(i - (num20 / 2 - 1)) * 6.28318548f / (float)num20), default(Vector2)) + npc.Center;
						Vector2 vector2 = expr_80F - npc.Center;
						int num21 = Dust.NewDust(expr_80F + vector2, 0, 0, 172, vector2.X * 2f, vector2.Y * 2f, 100, default(Color), 1.4f);
						Main.dust[num21].noGravity = true;
						Main.dust[num21].noLight = true;
						Main.dust[num21].velocity = Vector2.Normalize(vector2) * 3f;
					}
					Main.PlaySound(29, (int)vector.X, (int)vector.Y, 20, 1f, 0f);
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num16)
				{
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 0f && !player.dead)
			{
				if (npc.ai[1] == 0f)
					npc.ai[1] = (float)(300 * Math.Sign((vector - player.Center).X));

				Vector2 vector3 = Vector2.Normalize(player.Center + new Vector2(npc.ai[1], -200f) - vector - npc.velocity) * scaleFactor;
				if (npc.velocity.X < vector3.X)
				{
					npc.velocity.X = npc.velocity.X + num3;
					if (npc.velocity.X < 0f && vector3.X > 0f)
						npc.velocity.X = npc.velocity.X + num3;
				}
				else if (npc.velocity.X > vector3.X)
				{
					npc.velocity.X = npc.velocity.X - num3;
					if (npc.velocity.X > 0f && vector3.X < 0f)
						npc.velocity.X = npc.velocity.X - num3;
				}
				if (npc.velocity.Y < vector3.Y)
				{
					npc.velocity.Y = npc.velocity.Y + num3;
					if (npc.velocity.Y < 0f && vector3.Y > 0f)
						npc.velocity.Y = npc.velocity.Y + num3;
				}
				else if (npc.velocity.Y > vector3.Y)
				{
					npc.velocity.Y = npc.velocity.Y - num3;
					if (npc.velocity.Y > 0f && vector3.Y < 0f)
						npc.velocity.Y = npc.velocity.Y - num3;
				}

				int num22 = Math.Sign(player.Center.X - vector.X);
				if (num22 != 0)
				{
					if (npc.ai[2] == 0f && num22 != npc.direction)
						npc.rotation += 3.14159274f;

					npc.direction = num22;

					if (npc.spriteDirection != -npc.direction)
						npc.rotation += 3.14159274f;

					npc.spriteDirection = -npc.direction;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num2)
				{
					int num23 = 0;
					switch ((int)npc.ai[3])
					{
						case 0:
						case 1:
						case 2:
						case 3:
						case 4:
						case 5:
						case 6:
						case 7:
						case 8:
						case 9:
							num23 = 1;
							break;
						case 10:
							npc.ai[3] = 1f;
							num23 = 2;
							break;
						case 11:
							npc.ai[3] = 0f;
							num23 = 3;
							break;
					}

					if (flag)
						num23 = 4;

					if (num23 == 1)
					{
						npc.ai[0] = 1f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.velocity = Vector2.Normalize(player.Center - vector) * num5;
						npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X);

						if (num22 != 0)
						{
							npc.direction = num22;

							if (npc.spriteDirection == 1)
								npc.rotation += 3.14159274f;

							npc.spriteDirection = -npc.direction;
						}
					}
					else if (num23 == 2)
					{
						npc.ai[0] = 2f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
					}
					else if (num23 == 3)
					{
						npc.ai[0] = 3f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
					}
					else if (num23 == 4)
					{
						npc.ai[0] = 4f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
					}
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 1f)
			{
				int num24 = 7;
				for (int j = 0; j < num24; j++)
				{
					Vector2 arg_E1C_0 = (Vector2.Normalize(npc.velocity) * new Vector2((float)(npc.width + 50) / 2f, (float)npc.height) * 0.75f).RotatedBy((double)(j - (num24 / 2 - 1)) * 3.1415926535897931 / (double)((float)num24), default(Vector2)) + vector;
					Vector2 vector4 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
					int num25 = Dust.NewDust(arg_E1C_0 + vector4, 0, 0, 172, vector4.X * 2f, vector4.Y * 2f, 100, default(Color), 1.4f);
					Main.dust[num25].noGravity = true;
					Main.dust[num25].noLight = true;
					Main.dust[num25].velocity /= 4f;
					Main.dust[num25].velocity -= npc.velocity;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num4)
				{
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] += 2f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 2f)
			{
				if (npc.ai[1] == 0f)
					npc.ai[1] = (float)(300 * Math.Sign((vector - player.Center).X));

				Vector2 vector5 = Vector2.Normalize(player.Center + new Vector2(npc.ai[1], -200f) - vector - npc.velocity) * scaleFactor2;
				if (npc.velocity.X < vector5.X)
				{
					npc.velocity.X = npc.velocity.X + num8;
					if (npc.velocity.X < 0f && vector5.X > 0f)
						npc.velocity.X = npc.velocity.X + num8;
				}
				else if (npc.velocity.X > vector5.X)
				{
					npc.velocity.X = npc.velocity.X - num8;
					if (npc.velocity.X > 0f && vector5.X < 0f)
						npc.velocity.X = npc.velocity.X - num8;
				}
				if (npc.velocity.Y < vector5.Y)
				{
					npc.velocity.Y = npc.velocity.Y + num8;
					if (npc.velocity.Y < 0f && vector5.Y > 0f)
						npc.velocity.Y = npc.velocity.Y + num8;
				}
				else if (npc.velocity.Y > vector5.Y)
				{
					npc.velocity.Y = npc.velocity.Y - num8;
					if (npc.velocity.Y > 0f && vector5.Y < 0f)
						npc.velocity.Y = npc.velocity.Y - num8;
				}

				if (npc.ai[2] == 0f)
					Main.PlaySound(29, (int)vector.X, (int)vector.Y, 20, 1f, 0f);

				if (npc.ai[2] % (float)num7 == 0f)
				{
					Main.PlaySound(4, (int)npc.Center.X, (int)npc.Center.Y, 19, 1f, 0f);
					if (Main.netMode != 1)
					{
						Vector2 vector6 = Vector2.Normalize(player.Center - vector) * (float)(npc.width + 20) / 2f + vector;
						NPC.NewNPC((int)vector6.X, (int)vector6.Y + 45, 371, 0, 0f, 0f, 0f, 0f, 255);
					}
				}

				int num26 = Math.Sign(player.Center.X - vector.X);
				if (num26 != 0)
				{
					npc.direction = num26;

					if (npc.spriteDirection != -npc.direction)
						npc.rotation += 3.14159274f;

					npc.spriteDirection = -npc.direction;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num6)
				{
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 3f)
			{
				npc.velocity *= 0.98f;
				npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

				if (npc.ai[2] == (float)(num9 - 30))
					Main.PlaySound(29, (int)vector.X, (int)vector.Y, 9, 1f, 0f);

				if (Main.netMode != 1 && npc.ai[2] == (float)(num9 - 30))
				{
					Vector2 vector7 = npc.rotation.ToRotationVector2() * (Vector2.UnitX * (float)npc.direction) * (float)(npc.width + 20) / 2f + vector;
					Projectile.NewProjectile(vector7.X, vector7.Y, (float)(npc.direction * 2), 8f, 385, 0, 0f, Main.myPlayer, 0f, 0f);
					Projectile.NewProjectile(vector7.X, vector7.Y, (float)(-(float)npc.direction * 2), 8f, 385, 0, 0f, Main.myPlayer, 0f, 0f);
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num9)
				{
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 4f)
			{
				npc.velocity *= 0.98f;
				npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

				if (npc.ai[2] == (float)(num10 - 60))
					Main.PlaySound(29, (int)vector.X, (int)vector.Y, 20, 1f, 0f);

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num10)
				{
					npc.ai[0] = 5f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 5f && !player.dead)
			{
				if (npc.ai[1] == 0f)
					npc.ai[1] = (float)(300 * Math.Sign((vector - player.Center).X));

				Vector2 vector8 = Vector2.Normalize(player.Center + new Vector2(npc.ai[1], -200f) - vector - npc.velocity) * scaleFactor;
				if (npc.velocity.X < vector8.X)
				{
					npc.velocity.X = npc.velocity.X + num3;
					if (npc.velocity.X < 0f && vector8.X > 0f)
						npc.velocity.X = npc.velocity.X + num3;
				}
				else if (npc.velocity.X > vector8.X)
				{
					npc.velocity.X = npc.velocity.X - num3;
					if (npc.velocity.X > 0f && vector8.X < 0f)
						npc.velocity.X = npc.velocity.X - num3;
				}
				if (npc.velocity.Y < vector8.Y)
				{
					npc.velocity.Y = npc.velocity.Y + num3;
					if (npc.velocity.Y < 0f && vector8.Y > 0f)
						npc.velocity.Y = npc.velocity.Y + num3;
				}
				else if (npc.velocity.Y > vector8.Y)
				{
					npc.velocity.Y = npc.velocity.Y - num3;
					if (npc.velocity.Y > 0f && vector8.Y < 0f)
						npc.velocity.Y = npc.velocity.Y - num3;
				}

				int num27 = Math.Sign(player.Center.X - vector.X);
				if (num27 != 0)
				{
					if (npc.ai[2] == 0f && num27 != npc.direction)
						npc.rotation += 3.14159274f;

					npc.direction = num27;

					if (npc.spriteDirection != -npc.direction)
						npc.rotation += 3.14159274f;

					npc.spriteDirection = -npc.direction;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num2)
				{
					int num28 = 0;
					switch ((int)npc.ai[3])
					{
						case 0:
						case 1:
						case 2:
						case 3:
						case 4:
						case 5:
							num28 = 1;
							break;
						case 6:
							npc.ai[3] = 1f;
							num28 = 2;
							break;
						case 7:
							npc.ai[3] = 0f;
							num28 = 3;
							break;
					}

					if (flag2)
						num28 = 4;

					if (num28 == 1)
					{
						npc.ai[0] = 6f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.velocity = Vector2.Normalize(player.Center - vector) * num5;
						npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X);

						if (num27 != 0)
						{
							npc.direction = num27;

							if (npc.spriteDirection == 1)
								npc.rotation += 3.14159274f;

							npc.spriteDirection = -npc.direction;
						}
					}
					else if (num28 == 2)
					{
						npc.velocity = Vector2.Normalize(player.Center - vector) * scaleFactor4;
						npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X);

						if (num27 != 0)
						{
							npc.direction = num27;

							if (npc.spriteDirection == 1)
								npc.rotation += 3.14159274f;

							npc.spriteDirection = -npc.direction;
						}

						npc.ai[0] = 7f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
					}
					else if (num28 == 3)
					{
						npc.ai[0] = 8f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
					}
					else if (num28 == 4)
					{
						npc.ai[0] = 9f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
					}
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 6f)
			{
				int num29 = 7;
				for (int k = 0; k < num29; k++)
				{
					Vector2 arg_1A97_0 = (Vector2.Normalize(npc.velocity) * new Vector2((float)(npc.width + 50) / 2f, (float)npc.height) * 0.75f).RotatedBy((double)(k - (num29 / 2 - 1)) * 3.1415926535897931 / (double)((float)num29), default(Vector2)) + vector;
					Vector2 vector9 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
					int num30 = Dust.NewDust(arg_1A97_0 + vector9, 0, 0, 172, vector9.X * 2f, vector9.Y * 2f, 100, default(Color), 1.4f);
					Main.dust[num30].noGravity = true;
					Main.dust[num30].noLight = true;
					Main.dust[num30].velocity /= 4f;
					Main.dust[num30].velocity -= npc.velocity;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num4)
				{
					npc.ai[0] = 5f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] += 2f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 7f)
			{
				if (npc.ai[2] == 0f)
					Main.PlaySound(29, (int)vector.X, (int)vector.Y, 20, 1f, 0f);

				if (npc.ai[2] % (float)num14 == 0f)
				{
					Main.PlaySound(4, (int)npc.Center.X, (int)npc.Center.Y, 19, 1f, 0f);
					if (Main.netMode != 1)
					{
						Vector2 vector10 = Vector2.Normalize(npc.velocity) * (float)(npc.width + 20) / 2f + vector;
						int num31 = NPC.NewNPC((int)vector10.X, (int)vector10.Y + 45, 371, 0, 0f, 0f, 0f, 0f, 255);
						Main.npc[num31].target = npc.target;
						Main.npc[num31].velocity = Vector2.Normalize(npc.velocity).RotatedBy((double)(1.57079637f * (float)npc.direction), default(Vector2)) * scaleFactor3;
						Main.npc[num31].netUpdate = true;
						Main.npc[num31].ai[3] = (float)Main.rand.Next(80, 121) / 100f;
					}
				}

				npc.velocity = npc.velocity.RotatedBy((double)(-(double)num15 * (float)npc.direction), default(Vector2));
				npc.rotation -= num15 * (float)npc.direction;

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num13)
				{
					npc.ai[0] = 5f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 8f)
			{
				npc.velocity *= 0.98f;
				npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

				if (npc.ai[2] == (float)(num9 - 30))
					Main.PlaySound(29, (int)vector.X, (int)vector.Y, 20, 1f, 0f);

				if (Main.netMode != 1 && npc.ai[2] == (float)(num9 - 30))
					Projectile.NewProjectile(vector.X, vector.Y, 0f, 0f, 385, 0, 0f, Main.myPlayer, 1f, (float)(npc.target + 1));

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num9)
				{
					npc.ai[0] = 5f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 9f)
			{
				if (npc.ai[2] < (float)(num11 - 90))
				{
					if (Collision.SolidCollision(npc.position, npc.width, npc.height))
						npc.alpha += 15;
					else
						npc.alpha -= 15;

					if (npc.alpha < 0)
						npc.alpha = 0;
					if (npc.alpha > 150)
						npc.alpha = 150;
				}
				else if (npc.alpha < 255)
				{
					npc.alpha += 4;
					if (npc.alpha > 255)
						npc.alpha = 255;
				}

				npc.velocity *= 0.98f;
				npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

				if (npc.ai[2] == (float)(num11 - 60))
					Main.PlaySound(29, (int)vector.X, (int)vector.Y, 20, 1f, 0f);

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num11)
				{
					npc.ai[0] = 10f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 10f && !player.dead)
			{
				npc.dontTakeDamage = false;
				npc.chaseable = false;

				if (npc.alpha < 255)
				{
					npc.alpha += 25;
					if (npc.alpha > 255)
						npc.alpha = 255;
				}

				if (npc.ai[1] == 0f)
					npc.ai[1] = (float)(360 * Math.Sign((vector - player.Center).X));

				Vector2 desiredVelocity = Vector2.Normalize(player.Center + new Vector2(npc.ai[1], -200f) - vector - npc.velocity) * scaleFactor;
				npc.SimpleFlyMovement(desiredVelocity, num3);

				int num32 = Math.Sign(player.Center.X - vector.X);
				if (num32 != 0)
				{
					if (npc.ai[2] == 0f && num32 != npc.direction)
					{
						npc.rotation += 3.14159274f;

						for (int l = 0; l < npc.oldPos.Length; l++)
							npc.oldPos[l] = Vector2.Zero;
					}

					npc.direction = num32;

					if (npc.spriteDirection != -npc.direction)
						npc.rotation += 3.14159274f;

					npc.spriteDirection = -npc.direction;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num2)
				{
					int num33 = 0;
					switch ((int)npc.ai[3])
					{
						case 0:
						case 2:
						case 3:
						case 5:
						case 6:
						case 7:
							num33 = 1;
							break;
						case 1:
						case 4:
						case 8:
							num33 = 2;
							break;
					}
					if (num33 == 1)
					{
						npc.ai[0] = 11f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.velocity = Vector2.Normalize(player.Center - vector) * num5;
						npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X);

						if (num32 != 0)
						{
							npc.direction = num32;

							if (npc.spriteDirection == 1)
								npc.rotation += 3.14159274f;

							npc.spriteDirection = -npc.direction;
						}
					}
					else if (num33 == 2)
					{
						npc.ai[0] = 12f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
					}
					else if (num33 == 3)
					{
						npc.ai[0] = 13f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
					}
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 11f)
			{
				npc.dontTakeDamage = false;
				npc.chaseable = true;

				npc.alpha -= 25;
				if (npc.alpha < 0)
					npc.alpha = 0;

				int num34 = 7;
				for (int m = 0; m < num34; m++)
				{
					Vector2 arg_2444_0 = (Vector2.Normalize(npc.velocity) * new Vector2((float)(npc.width + 50) / 2f, (float)npc.height) * 0.75f).RotatedBy((double)(m - (num34 / 2 - 1)) * 3.1415926535897931 / (double)((float)num34), default(Vector2)) + vector;
					Vector2 vector11 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
					int num35 = Dust.NewDust(arg_2444_0 + vector11, 0, 0, 172, vector11.X * 2f, vector11.Y * 2f, 100, default(Color), 1.4f);
					Main.dust[num35].noGravity = true;
					Main.dust[num35].noLight = true;
					Main.dust[num35].velocity /= 4f;
					Main.dust[num35].velocity -= npc.velocity;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num4)
				{
					npc.ai[0] = 10f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] += 1f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 12f)
			{
				npc.dontTakeDamage = true;
				npc.chaseable = false;

				if (npc.alpha < 255)
				{
					npc.alpha += 17;
					if (npc.alpha > 255)
						npc.alpha = 255;
				}

				npc.velocity *= 0.98f;
				npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

				if (npc.ai[2] == (float)(num12 / 2))
					Main.PlaySound(29, (int)vector.X, (int)vector.Y, 20, 1f, 0f);

				if (Main.netMode != 1 && npc.ai[2] == (float)(num12 / 2))
				{
					if (npc.ai[1] == 0f)
						npc.ai[1] = (float)(300 * Math.Sign((vector - player.Center).X));

					Vector2 center = player.Center + new Vector2(-npc.ai[1], -200f);
					vector = (npc.Center = center);

					int num36 = Math.Sign(player.Center.X - vector.X);
					if (num36 != 0)
					{
						if (npc.ai[2] == 0f && num36 != npc.direction)
						{
							npc.rotation += 3.14159274f;

							for (int n = 0; n < npc.oldPos.Length; n++)
								npc.oldPos[n] = Vector2.Zero;
						}

						npc.direction = num36;

						if (npc.spriteDirection != -npc.direction)
							npc.rotation += 3.14159274f;

						npc.spriteDirection = -npc.direction;
					}
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num12)
				{
					npc.ai[0] = 10f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] += 1f;

					if (npc.ai[3] >= 9f)
						npc.ai[3] = 0f;

					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 13f)
			{
				if (npc.ai[2] == 0f)
					Main.PlaySound(29, (int)vector.X, (int)vector.Y, 20, 1f, 0f);

				npc.velocity = npc.velocity.RotatedBy((double)(-(double)num15 * (float)npc.direction), default(Vector2));
				npc.rotation -= num15 * (float)npc.direction;

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num13)
				{
					npc.ai[0] = 10f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] += 1f;
					npc.netUpdate = true;
				}
			}
			return false;
		}
		#endregion

		#region Buffed Mothron AI
		private bool BuffedMothronAI(NPC npc)
		{
			npc.noTileCollide = false;
			npc.noGravity = true;
			npc.knockBackResist = 0.2f * Main.expertKnockBack;
			npc.damage = npc.defDamage;

			if (!Main.eclipse)
				npc.ai[0] = -1f;
			else if (npc.target < 0 || Main.player[npc.target].dead || !Main.player[npc.target].active)
			{
				npc.TargetClosest(true);
				Vector2 vector235 = Main.player[npc.target].Center - npc.Center;
				if (Main.player[npc.target].dead || vector235.Length() > 3000f)
					npc.ai[0] = -1f;
			}
			else
			{
				Vector2 vector236 = Main.player[npc.target].Center - npc.Center;
				if (npc.ai[0] > 1f && vector236.Length() > 1000f)
					npc.ai[0] = 1f;
			}

			if (npc.ai[0] == -1f)
			{
				Vector2 value37 = new Vector2(0f, -8f);
				npc.velocity = (npc.velocity * 22f + value37) / 10f;
				npc.noTileCollide = true;
				npc.dontTakeDamage = true;
				return false;
			}

			if (npc.ai[0] == 0f)
			{
				npc.TargetClosest(true);

				if (npc.Center.X < Main.player[npc.target].Center.X - 2f)
					npc.direction = 1;
				if (npc.Center.X > Main.player[npc.target].Center.X + 2f)
					npc.direction = -1;

				npc.spriteDirection = npc.direction;
				npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.025f) / 10f;

				if (npc.collideX)
				{
					npc.velocity.X = npc.velocity.X * (-npc.oldVelocity.X * 0.5f);

					if (npc.velocity.X > 30f)
						npc.velocity.X = 30f;
					if (npc.velocity.X < -30f)
						npc.velocity.X = -30f;
				}
				if (npc.collideY)
				{
					npc.velocity.Y = npc.velocity.Y * (-npc.oldVelocity.Y * 0.5f);

					if (npc.velocity.Y > 30f)
						npc.velocity.Y = 30f;
					if (npc.velocity.Y < -30f)
						npc.velocity.Y = -30f;
				}

				Vector2 value38 = Main.player[npc.target].Center - npc.Center;
				value38.Y -= 200f;
				if (value38.Length() > 2000f)
				{
					npc.ai[0] = 1f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
				}
				else if (value38.Length() > 80f)
				{
					float scaleFactor15 = 13f;
					float num1354 = 30f;
					value38.Normalize();
					value38 *= scaleFactor15;
					npc.velocity = (npc.velocity * (num1354 - 1f) + value38) / num1354;
				}
				else if (npc.velocity.Length() > 2f)
					npc.velocity *= 0.95f;
				else if (npc.velocity.Length() < 1f)
					npc.velocity *= 1.05f;

				npc.ai[1] += 1f;
				if (npc.justHit)
					npc.ai[1] += (float)Main.rand.Next(10, 30);

				if (npc.ai[1] >= 180f && Main.netMode != 1)
				{
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;

					while (npc.ai[0] == 0f)
					{
						int num1355 = Main.rand.Next(3);
						if (num1355 == 0 && Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
							npc.ai[0] = 2f;
						else if (num1355 == 1)
							npc.ai[0] = 3f;
						else if (num1355 == 2 && NPC.CountNPCS(478) + NPC.CountNPCS(479) < 3)
							npc.ai[0] = 4f;
					}
				}
			}
			else
			{
				if (npc.ai[0] == 1f)
				{
					npc.collideX = false;
					npc.collideY = false;
					npc.noTileCollide = true;
					npc.knockBackResist = 0f;

					if (npc.target < 0 || !Main.player[npc.target].active || Main.player[npc.target].dead)
						npc.TargetClosest(true);

					if (npc.velocity.X < 0f)
						npc.direction = -1;
					else if (npc.velocity.X > 0f)
						npc.direction = 1;

					npc.spriteDirection = npc.direction;
					npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.02f) / 10f;

					Vector2 value39 = Main.player[npc.target].Center - npc.Center;
					if (value39.Length() < 300f && !Collision.SolidCollision(npc.position, npc.width, npc.height))
					{
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
					}

					float scaleFactor16 = 14f + value39.Length() / 100f;
					float num1356 = 25f;
					value39.Normalize();
					value39 *= scaleFactor16;
					npc.velocity = (npc.velocity * (num1356 - 1f) + value39) / num1356;
					return false;
				}

				if (npc.ai[0] == 2f)
				{
					npc.damage = (int)((double)npc.defDamage * 0.75);
					npc.knockBackResist = 0f;

					if (npc.target < 0 || !Main.player[npc.target].active || Main.player[npc.target].dead)
					{
						npc.TargetClosest(true);
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
					}

					if (Main.player[npc.target].Center.X - 10f < npc.Center.X)
						npc.direction = -1;
					else if (Main.player[npc.target].Center.X + 10f > npc.Center.X)
						npc.direction = 1;

					npc.spriteDirection = npc.direction;
					npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.025f) / 5f;

					if (npc.collideX)
					{
						npc.velocity.X = npc.velocity.X * (-npc.oldVelocity.X * 0.5f);

						if (npc.velocity.X > 30f)
							npc.velocity.X = 30f;
						if (npc.velocity.X < -30f)
							npc.velocity.X = -30f;
					}
					if (npc.collideY)
					{
						npc.velocity.Y = npc.velocity.Y * (-npc.oldVelocity.Y * 0.5f);

						if (npc.velocity.Y > 30f)
							npc.velocity.Y = 30f;
						if (npc.velocity.Y < -30f)
							npc.velocity.Y = -30f;
					}

					Vector2 value40 = Main.player[npc.target].Center - npc.Center;
					value40.Y -= 20f;

					npc.ai[2] += 0.0222222228f;
					if (Main.expertMode)
						npc.ai[2] += 0.0166666675f;

					float scaleFactor17 = 13f + npc.ai[2] + value40.Length() / 120f;
					float num1357 = 20f;
					value40.Normalize();
					value40 *= scaleFactor17;
					npc.velocity = (npc.velocity * (num1357 - 1f) + value40) / num1357;

					npc.ai[1] += 1f;
					if (npc.ai[1] > 240f || !Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
					{
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
					}
				}
				else
				{
					if (npc.ai[0] == 3f)
					{
						npc.knockBackResist = 0f;
						npc.noTileCollide = true;

						if (npc.velocity.X < 0f)
							npc.direction = -1;
						else
							npc.direction = 1;

						npc.spriteDirection = npc.direction;
						npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.0175f) / 5f;

						Vector2 value41 = Main.player[npc.target].Center - npc.Center;
						value41.Y -= 12f;
						if (npc.Center.X > Main.player[npc.target].Center.X)
							value41.X += 400f;
						else
							value41.X -= 400f;

						if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) > 350f && Math.Abs(npc.Center.Y - Main.player[npc.target].Center.Y) < 20f)
						{
							npc.ai[0] = 3.1f;
							npc.ai[1] = 0f;
						}

						npc.ai[1] += 0.0333333351f;
						float scaleFactor18 = 20f + npc.ai[1];
						float num1358 = 4f;
						value41.Normalize();
						value41 *= scaleFactor18;
						npc.velocity = (npc.velocity * (num1358 - 1f) + value41) / num1358;
						return false;
					}

					if (npc.ai[0] == 3.1f)
					{
						npc.knockBackResist = 0f;
						npc.noTileCollide = true;
						npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.0175f) / 5f;

						Vector2 vector237 = Main.player[npc.target].Center - npc.Center;
						vector237.Y -= 12f;
						float scaleFactor19 = 30f;
						float num1359 = 8f;
						vector237.Normalize();
						vector237 *= scaleFactor19;
						npc.velocity = (npc.velocity * (num1359 - 1f) + vector237) / num1359;

						if (npc.velocity.X < 0f)
							npc.direction = -1;
						else
							npc.direction = 1;

						npc.spriteDirection = npc.direction;

						npc.ai[1] += 1f;
						if (npc.ai[1] > 10f)
						{
							npc.velocity = vector237;

							if (npc.velocity.X < 0f)
								npc.direction = -1;
							else
								npc.direction = 1;

							npc.ai[0] = 3.2f;
							npc.ai[1] = 0f;
							npc.ai[1] = (float)npc.direction;
						}
					}
					else
					{
						if (npc.ai[0] == 3.2f)
						{
							npc.damage = (int)((double)npc.defDamage * 1.5);
							npc.collideX = false;
							npc.collideY = false;
							npc.knockBackResist = 0f;
							npc.noTileCollide = true;
							npc.ai[2] += 0.0333333351f;
							npc.velocity.X = (20f + npc.ai[2]) * npc.ai[1];

							if ((npc.ai[1] > 0f && npc.Center.X > Main.player[npc.target].Center.X + 260f) || (npc.ai[1] < 0f && npc.Center.X < Main.player[npc.target].Center.X - 260f))
							{
								if (!Collision.SolidCollision(npc.position, npc.width, npc.height))
								{
									npc.ai[0] = 0f;
									npc.ai[1] = 0f;
									npc.ai[2] = 0f;
									npc.ai[3] = 0f;
								}
								else if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) > 1600f)
								{
									npc.ai[0] = 1f;
									npc.ai[1] = 0f;
									npc.ai[2] = 0f;
									npc.ai[3] = 0f;
								}
							}
							npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.0175f) / 5f;
							return false;
						}

						if (npc.ai[0] == 4f)
						{
							npc.ai[0] = 0f;
							npc.TargetClosest(true);

							if (Main.netMode != 1)
							{
								npc.ai[1] = -1f;
								npc.ai[2] = -1f;

								int num;
								for (int num1360 = 0; num1360 < 1000; num1360 = num + 1)
								{
									int num1361 = (int)Main.player[npc.target].Center.X / 16;
									int num1362 = (int)Main.player[npc.target].Center.Y / 16;
									int num1363 = 30 + num1360 / 50;
									int num1364 = 20 + num1360 / 75;

									num1361 += Main.rand.Next(-num1363, num1363 + 1);
									num1362 += Main.rand.Next(-num1364, num1364 + 1);

									if (!WorldGen.SolidTile(num1361, num1362))
									{
										while (!WorldGen.SolidTile(num1361, num1362) && (double)num1362 < Main.worldSurface)
											num1362++;

										if ((new Vector2((float)(num1361 * 16 + 8), (float)(num1362 * 16 + 8)) - Main.player[npc.target].Center).Length() < 2100f)
										{
											npc.ai[0] = 4.1f;
											npc.ai[1] = (float)num1361;
											npc.ai[2] = (float)num1362;
											break;
										}
									}
									num = num1360;
								}
							}
							npc.netUpdate = true;
							return false;
						}

						if (npc.ai[0] == 4.1f)
						{
							if (npc.velocity.X < -2f)
								npc.direction = -1;
							else if (npc.velocity.X > 2f)
								npc.direction = 1;

							npc.spriteDirection = npc.direction;
							npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.025f) / 10f;
							npc.noTileCollide = true;

							int num1365 = (int)npc.ai[1];
							int num1366 = (int)npc.ai[2];
							float x2 = (float)(num1365 * 16 + 8);
							float y2 = (float)(num1366 * 16 - 20);
							Vector2 vector238 = new Vector2(x2, y2);
							vector238 -= npc.Center;
							float num1367 = 6f + vector238.Length() / 150f;

							if (num1367 > 10f)
								num1367 = 10f;

							if (vector238.Length() < 10f)
								npc.ai[0] = 4.2f;

							vector238.Normalize();
							vector238 *= num1367;
							float num1368 = 10f;
							npc.velocity = (npc.velocity * (num1368 - 1f) + vector238) / num1368;
							return false;
						}

						if (npc.ai[0] == 4.2f)
						{
							npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.025f) / 10f;
							npc.knockBackResist = 0f;
							npc.noTileCollide = true;

							int num1369 = (int)npc.ai[1];
							int num1370 = (int)npc.ai[2];
							float x3 = (float)(num1369 * 16 + 8);
							float y3 = (float)(num1370 * 16 - 20);
							Vector2 vector239 = new Vector2(x3, y3);
							vector239 -= npc.Center;
							float num1371 = 4f;
							float num1372 = 2f;

							if (Main.netMode != 1 && vector239.Length() < 4f)
							{
								int num1373 = 70;
								if (Main.expertMode)
									num1373 = (int)((double)num1373 * 0.75);

								npc.ai[3] += 1f;

								if (npc.ai[3] == (float)num1373)
									NPC.NewNPC(num1369 * 16 + 8, num1370 * 16, 478, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
								else if (npc.ai[3] == (float)(num1373 * 2))
								{
									npc.ai[0] = 0f;
									npc.ai[1] = 0f;
									npc.ai[2] = 0f;
									npc.ai[3] = 0f;

									if (NPC.CountNPCS(478) + NPC.CountNPCS(479) < 3 && Main.rand.Next(3) != 0)
										npc.ai[0] = 4f;
									else if (Collision.SolidCollision(npc.position, npc.width, npc.height))
										npc.ai[0] = 1f;
								}
							}

							if (vector239.Length() > num1371)
							{
								vector239.Normalize();
								vector239 *= num1371;
							}
							npc.velocity = (npc.velocity * (num1372 - 1f) + vector239) / num1372;
						}
					}
				}
			}
			return false;
		}
		#endregion

		#region Buffed Pumpking and Pumpking Blade AI
		private bool BuffedPumpkingAI(NPC npc)
		{
			npc.localAI[0] += 1f;
			if (npc.localAI[0] > 6f)
			{
				npc.localAI[0] = 0f;
				npc.localAI[1] += 1f;

				if (npc.localAI[1] > 4f)
					npc.localAI[1] = 0f;
			}

			if (Main.netMode != 1)
			{
				npc.localAI[2] += 1f;
				if (npc.localAI[2] > 300f)
				{
					npc.ai[3] = (float)Main.rand.Next(3);
					npc.localAI[2] = 0f;
				}
				else if (npc.ai[3] == 0f && npc.localAI[2] % 30f == 0f && npc.localAI[2] > 30f)
				{
					float num856 = 10f;
					Vector2 vector109 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f + 30f);
					if (!WorldGen.SolidTile((int)vector109.X / 16, (int)vector109.Y / 16))
					{
						float num857 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector109.X;
						float num858 = Main.player[npc.target].position.Y - vector109.Y;
						num857 += (float)Main.rand.Next(-50, 51);
						num858 += (float)Main.rand.Next(50, 201);
						num858 *= 0.2f;
						float num859 = (float)Math.Sqrt((double)(num857 * num857 + num858 * num858));
						num859 = num856 / num859;
						num857 *= num859;
						num858 *= num859;
						num857 *= 1f + (float)Main.rand.Next(-30, 31) * 0.01f;
						num858 *= 1f + (float)Main.rand.Next(-30, 31) * 0.01f;
						Projectile.NewProjectile(vector109.X, vector109.Y, num857, num858, Main.rand.Next(326, 329), 60, 0f, Main.myPlayer, 0f, 0f);
					}
				}
			}

			if (npc.ai[0] == 0f && Main.netMode != 1)
			{
				npc.TargetClosest(true);
				npc.ai[0] = 1f;

				int num861 = NPC.NewNPC((int)(npc.position.X + (float)(npc.width / 2)), (int)npc.position.Y + npc.height / 2, 328, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
				Main.npc[num861].ai[0] = -1f;
				Main.npc[num861].ai[1] = (float)npc.whoAmI;
				Main.npc[num861].target = npc.target;
				Main.npc[num861].netUpdate = true;

				num861 = NPC.NewNPC((int)(npc.position.X + (float)(npc.width / 2)), (int)npc.position.Y + npc.height / 2, 328, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
				Main.npc[num861].ai[0] = 1f;
				Main.npc[num861].ai[1] = (float)npc.whoAmI;
				Main.npc[num861].ai[3] = 150f;
				Main.npc[num861].target = npc.target;
				Main.npc[num861].netUpdate = true;
			}

			if (Main.player[npc.target].dead || Math.Abs(npc.position.X - Main.player[npc.target].position.X) > 2000f || Math.Abs(npc.position.Y - Main.player[npc.target].position.Y) > 2000f)
			{
				npc.TargetClosest(true);

				if (Main.player[npc.target].dead || Math.Abs(npc.position.X - Main.player[npc.target].position.X) > 2000f || Math.Abs(npc.position.Y - Main.player[npc.target].position.Y) > 2000f)
					npc.ai[1] = 2f;
			}

			if (Main.dayTime)
			{
				npc.velocity.Y = npc.velocity.Y + 0.3f;
				npc.velocity.X = npc.velocity.X * 0.9f;
			}
			else if (npc.ai[1] == 0f)
			{
				npc.ai[2] += 1f;
				if (npc.ai[2] >= 300f)
				{
					if (npc.ai[3] != 1f)
					{
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
					}
					else
					{
						npc.ai[2] = 0f;
						npc.ai[1] = 1f;
						npc.TargetClosest(true);
						npc.netUpdate = true;
					}
				}

				Vector2 vector110 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num862 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector110.X;
				float num863 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 200f - vector110.Y;
				float num864 = (float)Math.Sqrt((double)(num862 * num862 + num863 * num863));
				float num865 = 8f;

				if (npc.ai[3] == 1f)
				{
					if (num864 > 900f)
						num865 = 14f;
					else if (num864 > 600f)
						num865 = 12f;
					else if (num864 > 300f)
						num865 = 10f;
				}

				if (num864 > 50f)
				{
					num864 = num865 / num864;
					npc.velocity.X = (npc.velocity.X * 14f + num862 * num864) / 15f;
					npc.velocity.Y = (npc.velocity.Y * 14f + num863 * num864) / 15f;
				}
			}
			else if (npc.ai[1] == 1f)
			{
				npc.ai[2] += 1f;
				if (npc.ai[2] >= 600f || npc.ai[3] != 1f)
				{
					npc.ai[2] = 0f;
					npc.ai[1] = 0f;
				}

				Vector2 vector111 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num866 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector111.X;
				float num867 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector111.Y;
				float num868 = (float)Math.Sqrt((double)(num866 * num866 + num867 * num867));
				num868 = 20f / num868;

				npc.velocity.X = (npc.velocity.X * 49f + num866 * num868) / 50f;
				npc.velocity.Y = (npc.velocity.Y * 49f + num867 * num868) / 50f;
			}
			else if (npc.ai[1] == 2f)
			{
				npc.ai[1] = 3f;
				npc.velocity.Y = npc.velocity.Y + 0.1f;

				if (npc.velocity.Y < 0f)
					npc.velocity.Y = npc.velocity.Y * 0.95f;

				npc.velocity.X = npc.velocity.X * 0.95f;

				if (npc.timeLeft > 500)
					npc.timeLeft = 500;
			}
			npc.rotation = npc.velocity.X * -0.02f;
			return false;
		}

		private bool BuffedPumpkingBladeAI(NPC npc)
		{
			npc.spriteDirection = -(int)npc.ai[0];

			if (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != 58)
			{
				npc.ai[2] += 10f;
				if (npc.ai[2] > 50f || Main.netMode != 2)
				{
					npc.life = -1;
					npc.HitEffect(0, 10.0);
					npc.active = false;
				}
			}

			if (Main.netMode != 1 && Main.npc[(int)npc.ai[1]].ai[3] == 2f)
			{
				npc.localAI[1] += 1f;
				if (npc.localAI[1] > 30f)
				{
					npc.localAI[1] = 0f;

					float num869 = 0.01f;
					Vector2 vector112 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f + 30f);
					float num870 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector112.X;
					float num871 = Main.player[npc.target].position.Y - vector112.Y;
					float num872 = (float)Math.Sqrt((double)(num870 * num870 + num871 * num871));

					num872 = num869 / num872;
					num870 *= num872;
					num871 *= num872;

					Projectile.NewProjectile(npc.Center.X, npc.Center.Y, num870, num871, 329, 70, 0f, Main.myPlayer, npc.rotation, (float)npc.spriteDirection);
				}
			}

			if (Main.dayTime)
			{
				npc.velocity.Y = npc.velocity.Y + 0.3f;
				npc.velocity.X = npc.velocity.X * 0.9f;
				return false;
			}

			if (npc.ai[2] == 0f || npc.ai[2] == 3f)
			{
				if (Main.npc[(int)npc.ai[1]].ai[1] == 3f && npc.timeLeft > 10)
					npc.timeLeft = 10;

				npc.ai[3] += 1f;
				if (npc.ai[3] >= 180f)
				{
					npc.ai[2] += 1f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
				}

				Vector2 vector113 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num874 = (Main.player[npc.target].Center.X + Main.npc[(int)npc.ai[1]].Center.X) / 2f;
				float num875 = (Main.player[npc.target].Center.Y + Main.npc[(int)npc.ai[1]].Center.Y) / 2f;
				num874 += -170f * npc.ai[0] - vector113.X;
				num875 += 90f - vector113.Y;

				float num876 = Math.Abs(Main.player[npc.target].Center.X - Main.npc[(int)npc.ai[1]].Center.X) + Math.Abs(Main.player[npc.target].Center.Y - Main.npc[(int)npc.ai[1]].Center.Y);
				if (num876 > 700f)
				{
					num874 = Main.npc[(int)npc.ai[1]].Center.X - 170f * npc.ai[0] - vector113.X;
					num875 = Main.npc[(int)npc.ai[1]].Center.Y + 90f - vector113.Y;
				}

				float num877 = (float)Math.Sqrt((double)(num874 * num874 + num875 * num875));
				float num878 = 8f;
				if (num877 > 1000f)
					num878 = 23f;
				else if (num877 > 800f)
					num878 = 20f;
				else if (num877 > 600f)
					num878 = 17f;
				else if (num877 > 400f)
					num878 = 14f;
				else if (num877 > 200f)
					num878 = 11f;

				if (npc.ai[0] < 0f && npc.Center.X > Main.npc[(int)npc.ai[1]].Center.X)
					num874 -= 4f;
				if (npc.ai[0] > 0f && npc.Center.X < Main.npc[(int)npc.ai[1]].Center.X)
					num874 += 4f;

				num877 = num878 / num877;
				npc.velocity.X = (npc.velocity.X * 14f + num874 * num877) / 15f;
				npc.velocity.Y = (npc.velocity.Y * 14f + num875 * num877) / 15f;
				num877 = (float)Math.Sqrt((double)(num874 * num874 + num875 * num875));

				if (num877 > 20f)
					npc.rotation = (float)Math.Atan2((double)num875, (double)num874) + 1.57f;
			}
			else if (npc.ai[2] == 1f)
			{
				Vector2 vector114 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num879 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector114.X;
				float num880 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector114.Y;
				float num881 = (float)Math.Sqrt((double)(num879 * num879 + num880 * num880));

				npc.rotation = (float)Math.Atan2((double)num880, (double)num879) + 1.57f;
				npc.velocity.X = npc.velocity.X * 0.95f;
				npc.velocity.Y = npc.velocity.Y - 0.3f;

				if (npc.velocity.Y < -18f)
					npc.velocity.Y = -18f;

				if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 200f)
				{
					npc.TargetClosest(true);
					npc.ai[2] = 2f;

					vector114 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					num879 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector114.X;
					num880 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector114.Y;
					num881 = (float)Math.Sqrt((double)(num879 * num879 + num880 * num880));
					num881 = 24f / num881;

					npc.velocity.X = num879 * num881;
					npc.velocity.Y = num880 * num881;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[2] == 2f)
			{
				float num882 = Math.Abs(npc.Center.X - Main.npc[(int)npc.ai[1]].Center.X) + Math.Abs(npc.Center.Y - Main.npc[(int)npc.ai[1]].Center.Y);

				if (npc.position.Y > Main.player[npc.target].position.Y || npc.velocity.Y < 0f || num882 > 800f)
					npc.ai[2] = 3f;
			}
			else if (npc.ai[2] == 4f)
			{
				Vector2 vector115 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num883 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector115.X;
				float num884 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector115.Y;
				float num885 = (float)Math.Sqrt((double)(num883 * num883 + num884 * num884));

				npc.rotation = (float)Math.Atan2((double)num884, (double)num883) + 1.57f;
				npc.velocity.Y = npc.velocity.Y * 0.95f;
				npc.velocity.X = npc.velocity.X + 0.3f * -npc.ai[0];

				if (npc.velocity.X < -18f)
					npc.velocity.X = -18f;
				if (npc.velocity.X > 18f)
					npc.velocity.X = 18f;

				if (npc.position.X + (float)(npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 500f || npc.position.X + (float)(npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) + 500f)
				{
					npc.TargetClosest(true);
					npc.ai[2] = 5f;

					vector115 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					num883 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector115.X;
					num884 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector115.Y;
					num885 = (float)Math.Sqrt((double)(num883 * num883 + num884 * num884));
					num885 = 17f / num885;

					npc.velocity.X = num883 * num885;
					npc.velocity.Y = num884 * num885;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[2] == 5f)
			{
				float num886 = Math.Abs(npc.Center.X - Main.npc[(int)npc.ai[1]].Center.X) + Math.Abs(npc.Center.Y - Main.npc[(int)npc.ai[1]].Center.Y);

				if ((npc.velocity.X > 0f && npc.position.X + (float)(npc.width / 2) > Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2)) || (npc.velocity.X < 0f && npc.position.X + (float)(npc.width / 2) < Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2)) || num886 > 800f)
					npc.ai[2] = 0f;
			}
			return false;
		}
		#endregion

		#region Buffed Ice Queen AI
		private bool BuffedIceQueenAI(NPC npc)
		{
			if (Main.dayTime)
			{
				if (npc.velocity.X > 0f)
					npc.velocity.X = npc.velocity.X + 0.25f;
				else
					npc.velocity.X = npc.velocity.X - 0.25f;

				npc.velocity.Y = npc.velocity.Y - 0.1f;
				npc.rotation = npc.velocity.X * 0.05f;
			}
			else if (npc.ai[0] == 0f)
			{
				if (npc.ai[2] == 0f)
				{
					npc.TargetClosest(true);

					if (npc.Center.X < Main.player[npc.target].Center.X)
						npc.ai[2] = 1f;
					else
						npc.ai[2] = -1f;
				}

				npc.TargetClosest(true);
				int num887 = 800;
				float num888 = Math.Abs(npc.Center.X - Main.player[npc.target].Center.X);

				if (npc.Center.X < Main.player[npc.target].Center.X && npc.ai[2] < 0f && num888 > (float)num887)
					npc.ai[2] = 0f;
				if (npc.Center.X > Main.player[npc.target].Center.X && npc.ai[2] > 0f && num888 > (float)num887)
					npc.ai[2] = 0f;

				float num889 = 0.6f;
				float num890 = 10f;
				if ((double)npc.life < (double)npc.lifeMax * 0.75)
				{
					num889 = 0.7f;
					num890 = 12f;
				}
				if ((double)npc.life < (double)npc.lifeMax * 0.5)
				{
					num889 = 0.8f;
					num890 = 14f;
				}
				if ((double)npc.life < (double)npc.lifeMax * 0.25)
				{
					num889 = 0.95f;
					num890 = 16f;
				}

				npc.velocity.X = npc.velocity.X + npc.ai[2] * num889;
				if (npc.velocity.X > num890)
					npc.velocity.X = num890;
				if (npc.velocity.X < -num890)
					npc.velocity.X = -num890;

				float num891 = Main.player[npc.target].position.Y - (npc.position.Y + (float)npc.height);
				if (num891 < 150f)
					npc.velocity.Y = npc.velocity.Y - 0.2f;
				if (num891 > 200f)
					npc.velocity.Y = npc.velocity.Y + 0.2f;
				if (npc.velocity.Y > 9f)
					npc.velocity.Y = 9f;
				if (npc.velocity.Y < -9f)
					npc.velocity.Y = -9f;

				npc.rotation = npc.velocity.X * 0.05f;

				if ((num888 < 500f || npc.ai[3] < 0f) && npc.position.Y < Main.player[npc.target].position.Y)
				{
					npc.ai[3] += 1f;
					int num892 = 8;
					if ((double)npc.life < (double)npc.lifeMax * 0.75)
						num892 = 7;
					if ((double)npc.life < (double)npc.lifeMax * 0.5)
						num892 = 6;
					if ((double)npc.life < (double)npc.lifeMax * 0.25)
						num892 = 5;

					num892++;
					if (npc.ai[3] > (float)num892)
						npc.ai[3] = (float)(-(float)num892);

					if (npc.ai[3] == 0f && Main.netMode != 1)
					{
						Vector2 vector116 = new Vector2(npc.Center.X, npc.Center.Y);
						vector116.X += npc.velocity.X * 7f;
						float num893 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector116.X;
						float num894 = Main.player[npc.target].Center.Y - vector116.Y;
						float num895 = (float)Math.Sqrt((double)(num893 * num893 + num894 * num894));

						float num896 = 8f;
						if ((double)npc.life < (double)npc.lifeMax * 0.75)
							num896 = 9f;
						if ((double)npc.life < (double)npc.lifeMax * 0.5)
							num896 = 10f;
						if ((double)npc.life < (double)npc.lifeMax * 0.25)
							num896 = 11f;

						num895 = num896 / num895;
						num893 *= num895;
						num894 *= num895;

						Projectile.NewProjectile(vector116.X, vector116.Y, num893, num894, 348, 50, 0f, Main.myPlayer, 0f, 0f);
					}
				}
				else if (npc.ai[3] < 0f)
					npc.ai[3] += 1f;

				if (Main.netMode != 1)
				{
					npc.ai[1] += (float)Main.rand.Next(1, 4);

					if (npc.ai[1] > 600f && num888 < 600f)
						npc.ai[0] = -1f;
				}
			}
			else if (npc.ai[0] == 1f)
			{
				npc.TargetClosest(true);

				float num898 = 0.2f;
				float num899 = 10f;
				if ((double)npc.life < (double)npc.lifeMax * 0.75)
				{
					num898 = 0.24f;
					num899 = 12f;
				}
				if ((double)npc.life < (double)npc.lifeMax * 0.5)
				{
					num898 = 0.28f;
					num899 = 14f;
				}
				if ((double)npc.life < (double)npc.lifeMax * 0.25)
				{
					num898 = 0.32f;
					num899 = 16f;
				}
				num898 -= 0.05f;
				num899 -= 1f;

				if (npc.Center.X < Main.player[npc.target].Center.X)
				{
					npc.velocity.X = npc.velocity.X + num898;
					if (npc.velocity.X < 0f)
						npc.velocity.X = npc.velocity.X * 0.98f;
				}
				if (npc.Center.X > Main.player[npc.target].Center.X)
				{
					npc.velocity.X = npc.velocity.X - num898;
					if (npc.velocity.X > 0f)
						npc.velocity.X = npc.velocity.X * 0.98f;
				}
				if (npc.velocity.X > num899 || npc.velocity.X < -num899)
					npc.velocity.X = npc.velocity.X * 0.95f;

				float num900 = Main.player[npc.target].position.Y - (npc.position.Y + (float)npc.height);
				if (num900 < 180f)
					npc.velocity.Y = npc.velocity.Y - 0.1f;
				if (num900 > 200f)
					npc.velocity.Y = npc.velocity.Y + 0.1f;

				if (npc.velocity.Y > 7f)
					npc.velocity.Y = 7f;
				if (npc.velocity.Y < -7f)
					npc.velocity.Y = -7f;

				npc.rotation = npc.velocity.X * 0.01f;

				if (Main.netMode != 1)
				{
					npc.ai[3] += 1f;
					int num901 = 10;
					if ((double)npc.life < (double)npc.lifeMax * 0.75)
						num901 = 8;
					if ((double)npc.life < (double)npc.lifeMax * 0.5)
						num901 = 6;
					if ((double)npc.life < (double)npc.lifeMax * 0.25)
						num901 = 4;
					if ((double)npc.life < (double)npc.lifeMax * 0.1)
						num901 = 2;

					num901 += 3;
					if (npc.ai[3] >= (float)num901)
					{
						npc.ai[3] = 0f;
						Vector2 vector117 = new Vector2(npc.Center.X, npc.position.Y + (float)npc.height - 14f);
						int i2 = (int)(vector117.X / 16f);
						int j2 = (int)(vector117.Y / 16f);
						if (!WorldGen.SolidTile(i2, j2))
						{
							float num902 = npc.velocity.Y;

							if (num902 < 0f)
								num902 = 0f;

							num902 += 3f;
							float speedX2 = npc.velocity.X * 0.25f;
							Projectile.NewProjectile(vector117.X, vector117.Y, speedX2, num902, 349, 44, 0f, Main.myPlayer, (float)Main.rand.Next(5), 0f);
						}
					}
				}

				if (Main.netMode != 1)
				{
					npc.ai[1] += (float)Main.rand.Next(1, 4);

					if (npc.ai[1] > 450f)
						npc.ai[0] = -1f;
				}
			}
			else if (npc.ai[0] == 2f)
			{
				npc.TargetClosest(true);

				Vector2 vector118 = new Vector2(npc.Center.X, npc.Center.Y - 20f);
				float num904 = (float)Main.rand.Next(-1000, 1001);
				float num905 = (float)Main.rand.Next(-1000, 1001);
				float num906 = (float)Math.Sqrt((double)(num904 * num904 + num905 * num905));
				float num907 = 20f;

				npc.velocity *= 0.95f;
				num906 = num907 / num906;
				num904 *= num906;
				num905 *= num906;
				npc.rotation += 0.2f;
				vector118.X += num904 * 4f;
				vector118.Y += num905 * 4f;

				npc.ai[3] += 1f;
				int num908 = 7;
				if ((double)npc.life < (double)npc.lifeMax * 0.75)
					num908--;
				if ((double)npc.life < (double)npc.lifeMax * 0.5)
					num908 -= 2;
				if ((double)npc.life < (double)npc.lifeMax * 0.25)
					num908 -= 3;
				if ((double)npc.life < (double)npc.lifeMax * 0.1)
					num908 -= 4;

				if (npc.ai[3] > (float)num908)
				{
					npc.ai[3] = 0f;
					int num909 = Projectile.NewProjectile(vector118.X, vector118.Y, num904, num905, 349, 40, 0f, Main.myPlayer, 0f, 0f);
				}

				if (Main.netMode != 1)
				{
					npc.ai[1] += (float)Main.rand.Next(1, 4);

					if (npc.ai[1] > 300f)
						npc.ai[0] = -1f;
				}
			}
			if (npc.ai[0] == -1f)
			{
				int num910 = Main.rand.Next(3);
				npc.TargetClosest(true);

				if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) > 1000f)
					num910 = 0;

				npc.ai[0] = (float)num910;
				npc.ai[1] = 0f;
				npc.ai[2] = 0f;
				npc.ai[3] = 0f;
			}
			return false;
		}
		#endregion

		#region Buffed Eye of Cthulhu AI
		private bool BuffedEyeofCthulhuAI(NPC npc)
		{
			bool configBossRushBoost = Config.BossRushXerocCurse && CalamityWorld.bossRushActive;
			float num5 = 20f;

			double getMad = 0.5; //0.5
			bool flag2 = false;
			if ((double)npc.life < (double)npc.lifeMax * getMad)
				flag2 = true;

			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest(true);

			bool dead = Main.player[npc.target].dead;
			float num6 = npc.position.X + (float)(npc.width / 2) - Main.player[npc.target].position.X - (float)(Main.player[npc.target].width / 2);
			float num7 = npc.position.Y + (float)npc.height - 59f - Main.player[npc.target].position.Y - (float)(Main.player[npc.target].height / 2);
			float num8 = (float)Math.Atan2((double)num7, (double)num6) + 1.57f;

			if (num8 < 0f)
				num8 += 6.283f;
			else if ((double)num8 > 6.283)
				num8 -= 6.283f;

			float num9 = 0f;
			if (npc.ai[0] == 0f && npc.ai[1] == 0f)
				num9 = 0.02f;
			if (npc.ai[0] == 0f && npc.ai[1] == 2f && npc.ai[2] > 40f)
				num9 = 0.05f;
			if (npc.ai[0] == 3f && npc.ai[1] == 0f)
				num9 = 0.05f;
			if (npc.ai[0] == 3f && npc.ai[1] == 2f && npc.ai[2] > 40f)
				num9 = 0.08f;
			if (npc.ai[0] == 3f && npc.ai[1] == 4f && npc.ai[2] > num5)
				num9 = 0.15f;
			if (npc.ai[0] == 3f && npc.ai[1] == 5f)
				num9 = 0.05f;
			num9 *= 1.5f;

			if (npc.rotation < num8)
			{
				if ((double)(num8 - npc.rotation) > 3.1415)
					npc.rotation -= num9;
				else
					npc.rotation += num9;
			}
			else if (npc.rotation > num8)
			{
				if ((double)(npc.rotation - num8) > 3.1415)
					npc.rotation += num9;
				else
					npc.rotation -= num9;
			}

			if (npc.rotation > num8 - num9 && npc.rotation < num8 + num9)
				npc.rotation = num8;
			if (npc.rotation < 0f)
				npc.rotation += 6.283f;
			else if ((double)npc.rotation > 6.283)
				npc.rotation -= 6.283f;
			if (npc.rotation > num8 - num9 && npc.rotation < num8 + num9)
				npc.rotation = num8;

			if (Main.rand.Next(5) == 0)
			{
				int num10 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + (float)npc.height * 0.25f), npc.width, (int)((float)npc.height * 0.5f), 5, npc.velocity.X, 2f, 0, default(Color), 1f);
				Dust var_9_825_cp_0_cp_0 = Main.dust[num10];
				var_9_825_cp_0_cp_0.velocity.X = var_9_825_cp_0_cp_0.velocity.X * 0.5f;
				Dust var_9_845_cp_0_cp_0 = Main.dust[num10];
				var_9_845_cp_0_cp_0.velocity.Y = var_9_845_cp_0_cp_0.velocity.Y * 0.1f;
			}

			if (Main.dayTime | dead)
			{
				npc.velocity.Y = npc.velocity.Y - 0.04f;

				if (npc.timeLeft > 10)
					npc.timeLeft = 10;
			}
			else if (npc.ai[0] == 0f)
			{
				if (npc.ai[1] == 0f)
				{
					float num11 = ((enraged || configBossRushBoost) ? 10f : 7f);
					float num12 = ((enraged || configBossRushBoost) ? 0.2f : 0.15f);

					Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num13 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector.X;
					float num14 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 200f - vector.Y;
					float num15 = (float)Math.Sqrt((double)(num13 * num13 + num14 * num14));
					float num16 = num15;

					num15 = num11 / num15;
					num13 *= num15;
					num14 *= num15;

					if (npc.velocity.X < num13)
					{
						npc.velocity.X = npc.velocity.X + num12;
						if (npc.velocity.X < 0f && num13 > 0f)
							npc.velocity.X = npc.velocity.X + num12;
					}
					else if (npc.velocity.X > num13)
					{
						npc.velocity.X = npc.velocity.X - num12;
						if (npc.velocity.X > 0f && num13 < 0f)
							npc.velocity.X = npc.velocity.X - num12;
					}
					if (npc.velocity.Y < num14)
					{
						npc.velocity.Y = npc.velocity.Y + num12;
						if (npc.velocity.Y < 0f && num14 > 0f)
							npc.velocity.Y = npc.velocity.Y + num12;
					}
					else if (npc.velocity.Y > num14)
					{
						npc.velocity.Y = npc.velocity.Y - num12;
						if (npc.velocity.Y > 0f && num14 < 0f)
							npc.velocity.Y = npc.velocity.Y - num12;
					}

					npc.ai[2] += 1f;
					float num17 = 180f;
					if (npc.ai[2] >= num17)
					{
						npc.ai[1] = 1f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.target = 255;
						npc.netUpdate = true;
					}
					else if (num16 < 500f)
					{
						if (!Main.player[npc.target].dead)
							npc.ai[3] += 1f;

						if (npc.ai[3] >= 44f)
						{
							npc.ai[3] = 0f;
							npc.rotation = num8;

							float num19 = 5f;
							float num20 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector.X;
							float num21 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector.Y;
							float num22 = (float)Math.Sqrt((double)(num20 * num20 + num21 * num21));

							num22 = num19 / num22;
							Vector2 vector2 = vector;
							Vector2 vector3;
							vector3.X = num20 * num22;
							vector3.Y = num21 * num22;
							vector2.X += vector3.X * 10f;
							vector2.Y += vector3.Y * 10f;

							if (Main.netMode != 1)
							{
								int num23 = NPC.NewNPC((int)vector2.X, (int)vector2.Y, 5, 0, 0f, 0f, 0f, 0f, 255);
								Main.npc[num23].velocity.X = vector3.X;
								Main.npc[num23].velocity.Y = vector3.Y;

								if (Main.netMode == 2 && num23 < 200)
									NetMessage.SendData(23, -1, -1, null, num23, 0f, 0f, 0f, 0, 0, 0);
							}

							Main.PlaySound(3, (int)vector2.X, (int)vector2.Y, 1, 1f, 0f);

							int num;
							for (int m = 0; m < 10; m = num + 1)
							{
								Dust.NewDust(vector2, 20, 20, 5, vector3.X * 0.4f, vector3.Y * 0.4f, 0, default(Color), 1f);
								num = m;
							}
						}
					}
				}
				else if (npc.ai[1] == 1f)
				{
					npc.rotation = num8;
					float num24 = ((enraged || configBossRushBoost) ? 11f : 7.25f);

					Vector2 vector4 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num25 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector4.X;
					float num26 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector4.Y;
					float num27 = (float)Math.Sqrt((double)(num25 * num25 + num26 * num26));

					num27 = num24 / num27;
					npc.velocity.X = num25 * num27;
					npc.velocity.Y = num26 * num27;

					npc.ai[1] = 2f;
					npc.netUpdate = true;

					if (npc.netSpam > 10)
						npc.netSpam = 10;
				}
				else if (npc.ai[1] == 2f)
				{
					npc.ai[2] += 1f;
					if (npc.ai[2] >= 40f)
					{
						npc.velocity *= 0.975f;
						if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
							npc.velocity.X = 0f;
						if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1)
							npc.velocity.Y = 0f;
					}
					else
						npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) - 1.57f;

					int num28 = 90;
					if (npc.ai[2] >= (float)num28)
					{
						npc.ai[3] += 1f;
						npc.ai[2] = 0f;
						npc.target = 255;
						npc.rotation = num8;

						if (npc.ai[3] >= 3f)
						{
							npc.ai[1] = 0f;
							npc.ai[3] = 0f;
						}
						else
							npc.ai[1] = 1f;
					}
				}

				float num29 = 0.9f;
				if (((float)npc.life < (float)npc.lifeMax * num29) || CalamityWorld.death)
				{
					npc.ai[0] = 1f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;

					if (npc.netSpam > 10)
						npc.netSpam = 10;
				}
			}
			else if (npc.ai[0] == 1f || npc.ai[0] == 2f)
			{
				if (npc.ai[0] == 1f)
				{
					npc.ai[2] += 0.005f;
					if ((double)npc.ai[2] > 0.5)
						npc.ai[2] = 0.5f;
				}
				else
				{
					npc.ai[2] -= 0.005f;
					if (npc.ai[2] < 0f)
						npc.ai[2] = 0f;
				}

				npc.rotation += npc.ai[2];
				npc.ai[1] += 1f;
				if (npc.ai[1] % 20f == 0f)
				{
					Vector2 vector5 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num31 = (float)Main.rand.Next(-200, 200);
					float num32 = (float)Main.rand.Next(-200, 200);
					float num33 = (float)Math.Sqrt((double)(num31 * num31 + num32 * num32));

					num33 = 5f / num33;
					Vector2 vector6 = vector5;
					Vector2 vector7;
					vector7.X = num31 * num33;
					vector7.Y = num32 * num33;
					vector6.X += vector7.X * 10f;
					vector6.Y += vector7.Y * 10f;

					if (Main.netMode != 1)
					{
						int num34 = NPC.NewNPC((int)vector6.X, (int)vector6.Y, 5, 0, 0f, 0f, 0f, 0f, 255);
						Main.npc[num34].velocity.X = vector7.X;
						Main.npc[num34].velocity.Y = vector7.Y;

						if (Main.netMode == 2 && num34 < 200)
							NetMessage.SendData(23, -1, -1, null, num34, 0f, 0f, 0f, 0, 0, 0);
					}

					int num;
					for (int n = 0; n < 10; n = num + 1)
					{
						Dust.NewDust(vector6, 20, 20, 5, vector7.X * 0.4f, vector7.Y * 0.4f, 0, default(Color), 1f);
						num = n;
					}
				}

				if (npc.ai[1] == 100f)
				{
					npc.ai[0] += 1f;
					npc.ai[1] = 0f;

					if (npc.ai[0] == 3f)
						npc.ai[2] = 0f;
					else
					{
						Main.PlaySound(3, (int)npc.position.X, (int)npc.position.Y, 1, 1f, 0f);

						int num;
						for (int num35 = 0; num35 < 2; num35 = num + 1)
						{
							Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 8, 1f);
							Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 7, 1f);
							Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 6, 1f);
							num = num35;
						}

						for (int num36 = 0; num36 < 20; num36 = num + 1)
						{
							Dust.NewDust(npc.position, npc.width, npc.height, 5, (float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f, 0, default(Color), 1f);
							num = num36;
						}

						Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
					}
				}
				Dust.NewDust(npc.position, npc.width, npc.height, 5, (float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f, 0, default(Color), 1f);
				npc.velocity.X = npc.velocity.X * 0.98f;
				npc.velocity.Y = npc.velocity.Y * 0.98f;

				if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
					npc.velocity.X = 0f;
				if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1)
					npc.velocity.Y = 0f;
			}
			else
			{
				if (flag2)
					npc.defense = -5;
				else
					npc.damage = (int)(20f * Main.expertDamage);

				if (npc.ai[1] == 0f & flag2)
					npc.ai[1] = 5f;

				if (npc.ai[1] == 0f)
				{
					float num37 = ((enraged || configBossRushBoost) ? 8f : 5.5f);
					float num38 = ((enraged || configBossRushBoost) ? 0.09f : 0.06f);

					Vector2 vector8 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num39 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector8.X;
					float num40 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 120f - vector8.Y;
					float num41 = (float)Math.Sqrt((double)(num39 * num39 + num40 * num40));

					if (num41 > 400f)
					{
						num37 += 1.25f;
						num38 += 0.075f;
						if (num41 > 600f)
						{
							num37 += 1.25f;
							num38 += 0.075f;
							if (num41 > 800f)
							{
								num37 += 1.25f;
								num38 += 0.075f;
							}
						}
					}

					num41 = num37 / num41;
					num39 *= num41;
					num40 *= num41;

					if (npc.velocity.X < num39)
					{
						npc.velocity.X = npc.velocity.X + num38;
						if (npc.velocity.X < 0f && num39 > 0f)
							npc.velocity.X = npc.velocity.X + num38;
					}
					else if (npc.velocity.X > num39)
					{
						npc.velocity.X = npc.velocity.X - num38;
						if (npc.velocity.X > 0f && num39 < 0f)
							npc.velocity.X = npc.velocity.X - num38;
					}
					if (npc.velocity.Y < num40)
					{
						npc.velocity.Y = npc.velocity.Y + num38;
						if (npc.velocity.Y < 0f && num40 > 0f)
							npc.velocity.Y = npc.velocity.Y + num38;
					}
					else if (npc.velocity.Y > num40)
					{
						npc.velocity.Y = npc.velocity.Y - num38;
						if (npc.velocity.Y > 0f && num40 < 0f)
							npc.velocity.Y = npc.velocity.Y - num38;
					}

					npc.ai[2] += 1f;
					if (npc.ai[2] >= 200f)
					{
						npc.ai[1] = 1f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;

						if ((double)npc.life < (double)npc.lifeMax * 0.4)
							npc.ai[1] = 3f;

						npc.target = 255;
						npc.netUpdate = true;
					}
				}
				else if (npc.ai[1] == 1f)
				{
					Main.PlaySound(36, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
					npc.rotation = num8;

					float num42 = ((enraged || configBossRushBoost) ? 9.5f : 6.2f);
					if (npc.ai[3] == 1f)
						num42 *= 1.15f;
					if (npc.ai[3] == 2f)
						num42 *= 1.3f;

					Vector2 vector9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num43 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector9.X;
					float num44 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector9.Y;
					float num45 = (float)Math.Sqrt((double)(num43 * num43 + num44 * num44));

					num45 = num42 / num45;
					npc.velocity.X = num43 * num45;
					npc.velocity.Y = num44 * num45;
					npc.ai[1] = 2f;
					npc.netUpdate = true;

					if (npc.netSpam > 10)
						npc.netSpam = 10;
				}
				else if (npc.ai[1] == 2f)
				{
					float num46 = CalamityWorld.death ? 70f : 60f;

					npc.ai[2] += 1f;
					if (npc.ai[2] >= num46)
					{
						npc.velocity *= 0.96f;
						if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
							npc.velocity.X = 0f;
						if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1)
							npc.velocity.Y = 0f;
					}
					else
						npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) - 1.57f;

					int num47 = CalamityWorld.death ? 70 : 80;
					if (npc.ai[2] >= (float)num47)
					{
						npc.ai[3] += 1f;
						npc.ai[2] = 0f;
						npc.target = 255;
						npc.rotation = num8;

						if (npc.ai[3] >= 3f)
						{
							npc.ai[1] = 0f;
							npc.ai[3] = 0f;
							if (Main.netMode != 1 && (double)npc.life < (double)npc.lifeMax * 0.6)
							{
								npc.ai[1] = 3f;
								npc.ai[3] += (float)Main.rand.Next(1, 4);
							}
							npc.netUpdate = true;

							if (npc.netSpam > 10)
								npc.netSpam = 10;
						}
						else
							npc.ai[1] = 1f;
					}
				}
				else if (npc.ai[1] == 3f)
				{
					if ((npc.ai[3] == 4f & flag2) && npc.Center.Y > Main.player[npc.target].Center.Y)
					{
						npc.TargetClosest(true);
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.netUpdate = true;

						if (npc.netSpam > 10)
							npc.netSpam = 10;
					}
					else if (Main.netMode != 1)
					{
						npc.TargetClosest(true);
						float num48 = ((enraged || configBossRushBoost) ? 26f : 18f);

						Vector2 vector10 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						float num49 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector10.X;
						float num50 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector10.Y;
						float num51 = Math.Abs(Main.player[npc.target].velocity.X) + Math.Abs(Main.player[npc.target].velocity.Y) / 4f;
						num51 += 10f - num51;

						if (num51 < 5f)
							num51 = 5f;
						if (num51 > 15f)
							num51 = 15f;

						if (npc.ai[2] == -1f)
						{
							num51 *= 4f;
							num48 *= 1.3f;
						}

						num49 -= Main.player[npc.target].velocity.X * num51;
						num50 -= Main.player[npc.target].velocity.Y * num51 / 4f;
						num49 *= 1f + (float)Main.rand.Next(-10, 11) * 0.01f;
						num50 *= 1f + (float)Main.rand.Next(-10, 11) * 0.01f;

						float num52 = (float)Math.Sqrt((double)(num49 * num49 + num50 * num50));
						float num53 = num52;

						num52 = num48 / num52;
						npc.velocity.X = num49 * num52;
						npc.velocity.Y = num50 * num52;
						npc.velocity.X = npc.velocity.X + (float)Main.rand.Next(-20, 21) * 0.1f;
						npc.velocity.Y = npc.velocity.Y + (float)Main.rand.Next(-20, 21) * 0.1f;

						if (num53 < 100f)
						{
							if (Math.Abs(npc.velocity.X) > Math.Abs(npc.velocity.Y))
							{
								float num56 = Math.Abs(npc.velocity.X);
								float num57 = Math.Abs(npc.velocity.Y);

								if (npc.Center.X > Main.player[npc.target].Center.X)
									num57 *= -1f;
								if (npc.Center.Y > Main.player[npc.target].Center.Y)
									num56 *= -1f;

								npc.velocity.X = num57;
								npc.velocity.Y = num56;
							}
						}
						else if (Math.Abs(npc.velocity.X) > Math.Abs(npc.velocity.Y))
						{
							float num58 = (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) / 2f;
							float num59 = num58;

							if (npc.Center.X > Main.player[npc.target].Center.X)
								num59 *= -1f;
							if (npc.Center.Y > Main.player[npc.target].Center.Y)
								num58 *= -1f;

							npc.velocity.X = num59;
							npc.velocity.Y = num58;
						}
						npc.ai[1] = 4f;
						npc.netUpdate = true;

						if (npc.netSpam > 10)
							npc.netSpam = 10;
					}
				}
				else if (npc.ai[1] == 4f)
				{
					if (npc.ai[2] == 0f)
						Main.PlaySound(36, (int)npc.position.X, (int)npc.position.Y, -1, 1f, 0f);

					float num60 = num5;
					npc.ai[2] += 1f;

					if (npc.ai[2] == num60 && Vector2.Distance(npc.position, Main.player[npc.target].position) < 200f)
						npc.ai[2] -= 1f;

					if (npc.ai[2] >= num60)
					{
						npc.velocity *= 0.95f;
						if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
							npc.velocity.X = 0f;
						if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1)
							npc.velocity.Y = 0f;
					}
					else
						npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) - 1.57f;

					float num61 = num60 + 13f;
					if (npc.ai[2] >= num61)
					{
						npc.netUpdate = true;

						if (npc.netSpam > 10)
							npc.netSpam = 10;

						npc.ai[3] += 1f;
						npc.ai[2] = 0f;

						if (npc.ai[3] >= 5f)
						{
							npc.ai[1] = 0f;
							npc.ai[3] = 0f;
						}
						else
							npc.ai[1] = 3f;
					}
				}
				else if (npc.ai[1] == 5f)
				{
					float num62 = 600f;
					float num63 = ((enraged || configBossRushBoost) ? 12f : 8f);
					float num64 = ((enraged || configBossRushBoost) ? 0.4f : 0.25f);

					Vector2 vector11 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num65 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector11.X;
					float num66 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) + num62 - vector11.Y;
					float num67 = (float)Math.Sqrt((double)(num65 * num65 + num66 * num66));

					num67 = num63 / num67;
					num65 *= num67;
					num66 *= num67;

					if (npc.velocity.X < num65)
					{
						npc.velocity.X = npc.velocity.X + num64;
						if (npc.velocity.X < 0f && num65 > 0f)
							npc.velocity.X = npc.velocity.X + num64;
					}
					else if (npc.velocity.X > num65)
					{
						npc.velocity.X = npc.velocity.X - num64;
						if (npc.velocity.X > 0f && num65 < 0f)
							npc.velocity.X = npc.velocity.X - num64;
					}
					if (npc.velocity.Y < num66)
					{
						npc.velocity.Y = npc.velocity.Y + num64;
						if (npc.velocity.Y < 0f && num66 > 0f)
							npc.velocity.Y = npc.velocity.Y + num64;
					}
					else if (npc.velocity.Y > num66)
					{
						npc.velocity.Y = npc.velocity.Y - num64;
						if (npc.velocity.Y > 0f && num66 < 0f)
							npc.velocity.Y = npc.velocity.Y - num64;
					}

					npc.ai[2] += 1f;
					if (npc.ai[2] >= 70f)
					{
						npc.TargetClosest(true);
						npc.ai[1] = 3f;
						npc.ai[2] = -1f;
						npc.ai[3] = (float)Main.rand.Next(-3, 1);
						npc.netUpdate = true;
					}
				}
			}
			return false;
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
					UpdateServerBoolean();
				}
			}

			if (CalamityWorld.revenge || CalamityWorld.bossRushActive)
			{
				bool configBossRushBoost = Config.BossRushXerocCurse && CalamityWorld.bossRushActive;

				switch (npc.type)
				{
					case NPCID.MoonLordFreeEye:
						RevengeanceMoonLordFreeEyeAI(npc);
						break;
					case NPCID.MoonLordCore:
						RevengeanceMoonLordCoreAI(npc);
						break;
					case NPCID.MoonLordHand:
						RevengeanceMoonLordHandAI(npc);
						break;
					case NPCID.MoonLordHead:
						RevengeanceMoonLordHandAI(npc);
						break;
					case NPCID.CultistBoss:
						RevengeanceCultistAI(npc, configBossRushBoost);
						break;
					case NPCID.DukeFishron:
						RevengeanceDukeFishronAI(npc);
						break;
					case NPCID.GolemHeadFree:
						if (!CalamityWorld.bossRushActive)
							RevengeanceGolemHeadFreeAI(npc);
						break;
					case NPCID.GolemHead:
						if (!CalamityWorld.bossRushActive)
							RevengeanceGolemHeadAI(npc);
						break;
					case NPCID.Plantera:
						RevengeancePlanteraAI(npc, configBossRushBoost);
						break;
					case NPCID.PlanterasTentacle:
						RevengeancePlanterasTentacleAI(npc);
						break;
					case NPCID.SkeletronPrime:
						RevengeanceSkeletronPrimeAI(npc, configBossRushBoost);
						break;
					case NPCID.PrimeLaser:
						RevengeancePrimeLaserAI(npc);
						break;
					case NPCID.PrimeCannon:
						RevengeancePrimeCannonAI(npc, configBossRushBoost);
						break;
					case NPCID.Retinazer:
						RevengeanceRetinazerAI(npc, configBossRushBoost);
						break;
					case NPCID.Spazmatism:
						RevengeanceSpazmatismAI(npc, configBossRushBoost);
						break;
					case NPCID.TheDestroyerBody:
						RevengeanceDestroyerAI(npc, configBossRushBoost, mod);
						break;
					case NPCID.WallofFlesh:
						RevengeanceWallofFleshAI(npc, configBossRushBoost);
						break;
					case NPCID.WallofFleshEye:
						RevengeanceWallofFleshEyeAI(npc, configBossRushBoost);
						break;
					case NPCID.SkeletronHand:
						RevengeanceSkeletronHandAI(npc, configBossRushBoost);
						break;
					case NPCID.SkeletronHead:
						RevengeanceSkeletronAI(npc, configBossRushBoost);
						break;
					case NPCID.DungeonGuardian:
						RevengeanceSkeletronAI(npc, configBossRushBoost);
						break;
					case NPCID.QueenBee:
						RevengeanceQueenBeeAI(npc);
						break;
					case NPCID.BrainofCthulhu:
						RevengeanceBrainofCthulhuAI(npc);
						break;
					case NPCID.Creeper:
						RevengeanceCreeperAI(npc);
						break;
					case NPCID.EaterofWorldsHead:
						if (!CalamityWorld.bossRushActive)
							RevengeanceEaterofWorldsAI(npc);
						break;
					case NPCID.KingSlime:
						RevengeanceKingSlimeAI(npc);
						break;
					case NPCID.Lihzahrd:
						RevengeanceLihzahrdAI(npc);
						break;
					case NPCID.IceGolem:
						RevengeanceIceGolemAI(npc);
						break;
					default:
						break;
				}
			}
		}
		#endregion

		#region Revengeance Moon Lord AI
		private void RevengeanceMoonLordFreeEyeAI(NPC npc)
		{
			npc.dontTakeDamage = NPC.CountNPCS(npc.type) < 4;
		}

		private void RevengeanceMoonLordCoreAI(NPC npc)
		{
			if (npc.ai[0] >= 0f && npc.ai[0] < 2f && Main.netMode != 1 && npc.Distance(Main.player[npc.target].Center) > 1800f) //2400
			{
				npc.ai[0] = -2f;
				npc.netUpdate = true;
				Vector2 value8 = Main.player[npc.target].Center - Vector2.UnitY * 150f - npc.Center;
				npc.position += value8;

				if (Main.npc[(int)npc.localAI[0]].active)
				{
					NPC nPC6 = Main.npc[(int)npc.localAI[0]];
					nPC6.position += value8;
					Main.npc[(int)npc.localAI[0]].netUpdate = true;
				}
				if (Main.npc[(int)npc.localAI[1]].active)
				{
					NPC nPC6 = Main.npc[(int)npc.localAI[1]];
					nPC6.position += value8;
					Main.npc[(int)npc.localAI[1]].netUpdate = true;
				}
				if (Main.npc[(int)npc.localAI[2]].active)
				{
					NPC nPC6 = Main.npc[(int)npc.localAI[2]];
					nPC6.position += value8;
					Main.npc[(int)npc.localAI[2]].netUpdate = true;
				}

				int num;
				for (int num1176 = 0; num1176 < 200; num1176 = num + 1)
				{
					NPC nPC7 = Main.npc[num1176];
					if (nPC7.active && nPC7.type == NPCID.MoonLordFreeEye)
					{
						NPC nPC6 = nPC7;
						nPC6.position += value8;
						nPC7.netUpdate = true;
					}
					num = num1176;
				}
			}
			npc.dontTakeDamage = NPC.CountNPCS(NPCID.MoonLordFreeEye) > 3;
		}

		private void RevengeanceMoonLordHandAI(NPC npc)
		{
			if (npc.ai[0] == -2f)
			{
				if ((double)Main.npc[(int)npc.ai[3]].life <= (double)Main.npc[(int)npc.ai[3]].lifeMax * 0.75)
				{
					if (newAI[2] < 90f)
						newAI[2] += 1f;

					if ((int)newAI[2] % 60 == 0)
					{
						npc.netUpdate = true;
						if (Main.netMode != 1)
						{
							int num = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, 400, 0, 0f, 0f, 0f, 0f, 255);
							Main.npc[num].ai[3] = npc.ai[3];
							Main.npc[num].netUpdate = true;
						}
					}
				}
				if ((CalamityWorld.death || CalamityWorld.bossRushActive) && npc.type == NPCID.MoonLordHead)
				{
					if ((double)Main.npc[(int)npc.ai[3]].life <= (double)Main.npc[(int)npc.ai[3]].lifeMax * 0.5)
					{
						if (newAI[2] < 150f)
							newAI[2] += 1f;

						if ((int)newAI[2] % 60 == 0)
						{
							npc.netUpdate = true;
							if (Main.netMode != 1)
							{
								int num = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, 400, 0, 0f, 0f, 0f, 0f, 255);
								Main.npc[num].ai[3] = npc.ai[3];
								Main.npc[num].netUpdate = true;
							}
						}
					}
				}
			}
		}
		#endregion

		#region Revengeance Cultist AI
		private void RevengeanceCultistAI(NPC npc, bool configBossRushBoost)
		{
			if (npc.buffImmune[mod.BuffType("Enraged")])
				npc.buffImmune[mod.BuffType("Enraged")] = false;

			if ((double)npc.life <= (double)npc.lifeMax * 0.5 || enraged || configBossRushBoost)
			{
				if (!NPC.AnyNPCs(mod.NPCType("Eidolist")))
				{
					if (newAI[0] < 120f)
						newAI[0] += 1f;

					if (newAI[0] >= 120f)
					{
						newAI[0] = 0f;
						npc.netUpdate = true;

						if (Main.netMode != 1)
							NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, mod.NPCType("Eidolist"), 0, 0f, 0f, 0f, 0f, 255);

						newAI[0] = -360f;
					}
				}
			}
			else
			{
				if (CultCountdown == 0)
				{
					if (CalamityWorld.death || CalamityWorld.bossRushActive)
						CultCountdown = 75;
					else if ((double)npc.life <= (double)npc.lifeMax * 0.5)
						CultCountdown = 150;
					else
						CultCountdown = 200;
				}
				if (CultCountdown > 0)
				{
					CultCountdown--;
					if (CultCountdown == 0)
					{
						if (Main.netMode != 1)
						{
							Player player2 = Main.player[npc.target];
							int speed2 = 8;
							float spawnX = Main.rand.Next(1000) - 500 + player2.Center.X;
							float spawnY = -1000 + player2.Center.Y;
							Vector2 baseSpawn = new Vector2(spawnX, spawnY);
							Vector2 baseVelocity = player2.Center - baseSpawn;
							baseVelocity.Normalize();
							baseVelocity = baseVelocity * speed2;
							int damage = 25; //100
							for (int i = 0; i < CultProjectiles; i++)
							{
								Vector2 spawn2 = baseSpawn;
								spawn2.X = spawn2.X + i * 30 - (CultProjectiles * 15);
								Vector2 velocity = baseVelocity;
								velocity = baseVelocity.RotatedBy(MathHelper.ToRadians(-CultAngleSpread / 2 + (CultAngleSpread * i / (float)CultProjectiles)));
								velocity.X = velocity.X + 3 * Main.rand.NextFloat() - 1.5f;
								int projectileType = Main.rand.Next(3);
								switch (projectileType)
								{
									case 0:
										projectileType = 467;
										break;
									case 1:
										projectileType = 348;
										break;
									case 2:
										projectileType = 593;
										break;
									default:
										break;
								}
								int projectileI = Projectile.NewProjectile(spawn2.X, spawn2.Y, velocity.X, velocity.Y, projectileType, damage, 0f, Main.myPlayer, 0f, 0f);
								Main.projectile[projectileI].tileCollide = false;
							}
						}
					}
				}
			}
		}
		#endregion

		#region Revengeance Duke Fishron AI
		private void RevengeanceDukeFishronAI(NPC npc)
		{
			float num = 0.6f * Main.damageMultiplier; //1.2
			if (npc.ai[0] > 9f)
			{
				npc.damage = (int)((float)npc.defDamage * 1.1f * num);
				npc.defense = 38;
				newAI[0] += 1f;
				if (newAI[0] >= 600f)
				{
					newAI[0] = 0f;

					if (Main.netMode != 1)
						Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0f, 0f, 385, 0, 0f, Main.myPlayer, 1f, (float)(npc.target + 1));

					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] > 4f)
			{
				npc.damage = (int)((float)npc.defDamage * 1.2f * num);
				npc.defense = (int)((float)npc.defDefense * 1.1f);
			}
			else
			{
				npc.damage = npc.defDamage;
				npc.defense = npc.defDefense;
			}

			if (npc.ai[0] == -1f || npc.ai[0] == 4f || npc.ai[0] == 9f || (Main.player[npc.target].position.Y < 800f ||
				(double)Main.player[npc.target].position.Y > Main.worldSurface * 16.0 || (Main.player[npc.target].position.X > 6400f && Main.player[npc.target].position.X < (float)(Main.maxTilesX * 16 - 6400))))
			{
				npc.dontTakeDamage = true;
			}
			else if (npc.ai[0] <= 8f)
				npc.dontTakeDamage = false;

			if (npc.ai[0] == 1f || npc.ai[0] == 6f || npc.ai[0] == 11f)
				npc.velocity *= 1.01f; //20, 24, 30

			if (npc.ai[0] == 0f && !Main.player[npc.target].dead)
			{
				if ((double)npc.life <= (double)npc.lifeMax * 0.75)
				{
					npc.ai[0] = 4f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 5f && !Main.player[npc.target].dead)
			{
				if ((double)npc.life <= (double)npc.lifeMax * ((CalamityWorld.death || CalamityWorld.bossRushActive) ? 0.3 : 0.2))
				{
					npc.ai[0] = 9f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
				}
			}
		}
		#endregion

		#region Revengeance Golem AI
		private void RevengeanceGolemHeadFreeAI(NPC npc)
		{
			bool enrage = true;
			if ((double)Main.player[npc.target].Center.Y > Main.worldSurface * 16.0)
			{
				int num = (int)Main.player[npc.target].Center.X / 16;
				int num2 = (int)Main.player[npc.target].Center.Y / 16;
				Tile tile = Framing.GetTileSafely(num, num2);

				if (tile.wall == 87)
					enrage = false;
			}
			if (NPC.CountNPCS(NPCID.Golem) > 0)
			{
				if (enrage)
				{
					npc.ai[1] += 3f;
					npc.ai[2] += 3f;
				}

				npc.ai[1] -= 0.25f; //0.75
				if ((double)Main.npc[NPC.golemBoss].life < (double)Main.npc[NPC.golemBoss].lifeMax * 0.8)
					npc.ai[1] -= 0.75f; //1
				if ((double)Main.npc[NPC.golemBoss].life < (double)Main.npc[NPC.golemBoss].lifeMax * 0.6)
					npc.ai[1] -= 0.75f; //1.25
				if ((double)Main.npc[NPC.golemBoss].life < (double)Main.npc[NPC.golemBoss].lifeMax * 0.2)
					npc.ai[1] -= 0.75f; //1.5
				if ((double)Main.npc[NPC.golemBoss].life < (double)Main.npc[NPC.golemBoss].lifeMax * 0.1)
					npc.ai[1] -= 0.75f; //1.75

				npc.ai[2] -= 0.25f; //0.75
				if (Main.npc[NPC.golemBoss].life < Main.npc[NPC.golemBoss].lifeMax / 2)
					npc.ai[2] -= 0.75f; //1
				if (Main.npc[NPC.golemBoss].life < Main.npc[NPC.golemBoss].lifeMax / 3)
					npc.ai[2] -= 0.75f; //1.25
				if (Main.npc[NPC.golemBoss].life < Main.npc[NPC.golemBoss].lifeMax / 4)
					npc.ai[2] -= 0.5f; //1.75
				if (Main.npc[NPC.golemBoss].life < Main.npc[NPC.golemBoss].lifeMax / 5)
					npc.ai[2] -= 0.5f; //1.25
				if (Main.npc[NPC.golemBoss].life < Main.npc[NPC.golemBoss].lifeMax / 6)
					npc.ai[2] -= 0.25f; //2
			}
		}

		private void RevengeanceGolemHeadAI(NPC npc)
		{
			bool enrage = true;
			if ((double)Main.player[npc.target].Center.Y > Main.worldSurface * 16.0)
			{
				int num = (int)Main.player[npc.target].Center.X / 16;
				int num2 = (int)Main.player[npc.target].Center.Y / 16;
				Tile tile = Framing.GetTileSafely(num, num2);

				if (tile.wall == 87)
					enrage = false;
			}
			if (npc.ai[0] == 1f)
			{
				if (enrage)
				{
					npc.ai[1] += 3f;
					npc.ai[2] += 3f;
				}

				npc.ai[1] -= 0.25f; //0.75
				if ((double)npc.life < (double)npc.lifeMax * 0.4)
					npc.ai[1] -= 0.75f; //1
				if ((double)npc.life < (double)npc.lifeMax * 0.2)
					npc.ai[1] -= 0.75f; //1.25

				npc.ai[2] -= 0.25f; //0.75
				if (npc.life < npc.lifeMax / 3)
					npc.ai[2] -= 0.75f; //1
				if (npc.life < npc.lifeMax / 4)
					npc.ai[2] -= 0.75f; //1.25
				if (npc.life < npc.lifeMax / 5)
					npc.ai[2] -= 0.75f; //1.5
			}
		}
		#endregion

		#region Revengeance Plantera AI
		private void RevengeancePlanteraAI(NPC npc, bool configBossRushBoost)
		{
			bool jungle = Main.player[npc.target].ZoneJungle;

			if (npc.life < npc.lifeMax / 8 || CalamityWorld.bossRushActive)
			{
				npc.velocity.X *= 1.003f;
				npc.velocity.Y *= 1.003f;
			}
			else if (npc.life < npc.lifeMax / 4)
			{
				npc.velocity.X *= 1.001f;
				npc.velocity.Y *= 1.001f;
			}
			else if (npc.life < npc.lifeMax / 2)
			{
				npc.velocity.X *= 1.0005f;
				npc.velocity.Y *= 1.0005f;
			}

			if (npc.life > npc.lifeMax / 2)
			{
				npc.defense = 42;
				if (!jungle && !CalamityWorld.bossRushActive)
				{
					npc.defense = 200;
					npc.damage = (int)(120f * Main.damageMultiplier);
				}
				if (Main.netMode != 1)
					npc.localAI[1] += ((enraged || configBossRushBoost) ? 3f : 1.5f);
			}
			else
			{
				npc.localAI[1] += 1f;

				npc.defense = 22;
				if (!jungle && !CalamityWorld.bossRushActive)
				{
					npc.defense = 400;
					npc.damage = (int)(200f * Main.damageMultiplier);
				}

				newAI[0] += 1f;
				if (CalamityWorld.death || CalamityWorld.bossRushActive)
					newAI[0] += 0.5f;

				if (newAI[0] >= ((enraged || configBossRushBoost) ? 150f : 270f))
				{
					newAI[0] = 0f;

					if (Main.netMode != 1 && NPC.CountNPCS(NPCID.PlanterasTentacle) < 20)
						NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.PlanterasTentacle, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
				}

				if (CalamityWorld.death || (double)npc.life < (double)npc.lifeMax * 0.25 || CalamityWorld.bossRushActive)
				{
					newAI[1] += 1f;
					if (CalamityWorld.death || CalamityWorld.bossRushActive)
						newAI[1] += 0.5f;
					if (!jungle)
						newAI[1] += 2f;

					if (newAI[1] >= ((enraged || configBossRushBoost) ? 180f : 240f))
					{
						newAI[1] = 0f;
						if (Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
						{
							Vector2 vector34 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							float num349 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector34.X;
							float num350 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector34.Y;
							float num351 = (float)Math.Sqrt((double)(num349 * num349 + num350 * num350));
							num349 *= num351;
							num350 *= num351;
							if (Main.netMode != 1)
							{
								float num418 = (!jungle ? 24f : 12f);
								int num419 = 32;
								int num420 = 277;
								num351 = (float)Math.Sqrt((double)(num349 * num349 + num350 * num350));
								num351 = num418 / num351;
								num349 *= num351;
								num350 *= num351;
								num349 += (float)Main.rand.Next(-10, 11) * 0.05f;
								num350 += (float)Main.rand.Next(-10, 11) * 0.05f;
								vector34.X += num349 * 4f;
								vector34.Y += num350 * 4f;
								Projectile.NewProjectile(vector34.X, vector34.Y, num349, num350, num420, num419, 0f, Main.myPlayer, 0f, 0f);
							}
						}
					}
				}
			}
		}

		private void RevengeancePlanterasTentacleAI(NPC npc)
		{
			bool jungle = Main.player[npc.target].ZoneJungle;

			newAI[0] += 1f;
			if (CalamityWorld.death || !jungle)
				newAI[0] += 1f;

			if (newAI[0] >= 480f)
			{
				newAI[0] = 0f;

				if (Main.netMode != 1 && NPC.CountNPCS(NPCID.Spore) < 20)
					NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.Spore, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
			}
		}
		#endregion

		#region Revengeance Skeletron Prime AI
		private void RevengeanceSkeletronPrimeAI(NPC npc, bool configBossRushBoost)
		{
			bool targetFloatingUp = Main.player[npc.target].gravDir == -1f;

			if (Main.netMode != 1)
			{
				if (npc.ai[1] != 1f || (!CalamityWorld.death && (double)npc.life > (double)npc.lifeMax * 0.25) || CalamityWorld.bossRushActive || targetFloatingUp)
				{
					npc.localAI[0] += 1f;
					if ((double)npc.life <= (double)npc.lifeMax * 0.5 || CalamityWorld.bossRushActive)
						npc.localAI[0] += 1f;
					if ((double)npc.life <= (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive)
						npc.localAI[0] += 1f;

					if (npc.localAI[0] >= ((enraged || configBossRushBoost) ? 90f : 120f))
					{
						npc.localAI[0] = 0f;
						Vector2 vector16 = npc.Center;
						float num157 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector16.X;
						float num158 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector16.Y;
						Math.Sqrt((double)(num157 * num157 + num158 * num158));
						if (Collision.CanHit(vector16, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
						{
							float num159 = 7f;
							float num160 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector16.X + (float)Main.rand.Next(-20, 21);
							float num161 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector16.Y + (float)Main.rand.Next(-20, 21);
							float num162 = (float)Math.Sqrt((double)(num160 * num160 + num161 * num161));
							num162 = num159 / num162;
							num160 *= num162;
							num161 *= num162;
							Vector2 value = new Vector2(num160 * 1f + (float)Main.rand.Next(-50, 51) * 0.01f, num161 * 1f + (float)Main.rand.Next(-50, 51) * 0.01f);
							value.Normalize();
							value *= num159;
							value += npc.velocity;
							num160 = value.X;
							num161 = value.Y;
							int num163 = 27;
							int num164 = 270;
							vector16 += value * 5f;
							int num165 = Projectile.NewProjectile(vector16.X, vector16.Y, num160, num161, num164, num163, 0f, Main.myPlayer, -1f, 0f);
							Main.projectile[num165].timeLeft = 300;
						}
					}
				}
				if (CalamityWorld.death || (double)npc.life <= (double)npc.lifeMax * 0.25 || CalamityWorld.bossRushActive || targetFloatingUp)
				{
					newAI[0] += 1f;
					if (npc.ai[1] == 1f || CalamityWorld.bossRushActive)
						newAI[0] += 3f;

					if (newAI[0] >= ((enraged || configBossRushBoost) ? 180f : 240f)) //4 seconds or 1 second
					{
						newAI[0] = 0f;
						Vector2 shootFromVector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						float spread = 45f * 0.0174f;
						double startAngle = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spread / 2;
						double deltaAngle = spread / 8f;
						double offsetAngle;
						int damage = 30;
						int i;
						for (i = 0; i < 6; i++)
						{
							offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i;
							int projectile = Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, (float)(Math.Sin(offsetAngle) * 6f), (float)(Math.Cos(offsetAngle) * 6f), 100, damage, 0f, Main.myPlayer, 0f, 0f);
							int projectile2 = Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, (float)(-Math.Sin(offsetAngle) * 6f), (float)(-Math.Cos(offsetAngle) * 6f), 100, damage, 0f, Main.myPlayer, 0f, 0f);
							Main.projectile[projectile].timeLeft = 300;
							Main.projectile[projectile2].timeLeft = 300;
							Main.projectile[projectile].tileCollide = false;
							Main.projectile[projectile2].tileCollide = false;
						}
					}
				}
			}
			if (npc.ai[1] == 1f)
			{
				int speed = CalamityWorld.bossRushActive ? 7 : 3;
				if ((double)npc.life <= (double)npc.lifeMax * 0.7)
					speed++;
				if ((double)npc.life <= (double)npc.lifeMax * 0.3)
					speed++;
				if (enraged || configBossRushBoost)
					speed += 3;

				float speed2 = (float)speed;
				float speedBuff = 3f + (3f * (1f - (float)npc.life / (float)npc.lifeMax));
				float speedBuff2 = 10f + (10f * (1f - (float)npc.life / (float)npc.lifeMax));
				Vector2 vector45 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num444 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector45.X;
				float num445 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector45.Y;
				float num446 = (float)Math.Sqrt((double)(num444 * num444 + num445 * num445));
				speed2 += num446 / 100f;

				if (speed2 < speedBuff)
					speed2 = speedBuff;

				if (speed2 > speedBuff2)
					speed2 = speedBuff2;

				if (targetFloatingUp)
					speed2 += 5f;

				num446 = speed2 / num446;
				npc.velocity.X = num444 * num446;
				npc.velocity.Y = num445 * num446;
			}
		}

		private void RevengeancePrimeLaserAI(NPC npc)
		{
			if (Main.netMode != 1)
			{
				npc.localAI[0] += 2f;
				if (CalamityWorld.death || CalamityWorld.bossRushActive)
					npc.localAI[0] += 2f;
			}
		}

		private void RevengeancePrimeCannonAI(NPC npc, bool configBossRushBoost)
		{
			npc.TargetClosest(true);
			Vector2 vector61 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
			float num499 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector61.X;
			float num500 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector61.Y;
			npc.rotation = (float)Math.Atan2((double)num500, (double)num499) - 1.57f;

			if (Main.netMode != 1)
			{
				npc.localAI[0] = 0f;

				npc.localAI[1] += 1f;
				if (CalamityWorld.death || CalamityWorld.bossRushActive)
					npc.localAI[1] += 1f;

				if (npc.ai[2] == 1f)
				{
					npc.localAI[1] += 2f;
					if (CalamityWorld.death || CalamityWorld.bossRushActive)
						npc.localAI[1] += 2f;
				}

				if (npc.localAI[1] > ((enraged || configBossRushBoost) ? 90f : 120f))
				{
					npc.localAI[1] = 0f;
					npc.TargetClosest(true);
					if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
					{
						float num941 = 9f; //speed
						Vector2 vector104 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)(npc.height / 2));
						float num942 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector104.X + (float)Main.rand.Next(-20, 21);
						float num943 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector104.Y + (float)Main.rand.Next(-20, 21);
						float num944 = (float)Math.Sqrt((double)(num942 * num942 + num943 * num943));
						num944 = num941 / num944;
						num942 *= num944;
						num943 *= num944;
						num942 += (float)Main.rand.Next(-5, 6) * 0.05f;
						num943 += (float)Main.rand.Next(-5, 6) * 0.05f;
						int num945 = 27;
						int num946 = 303;
						vector104.X += num942 * 5f;
						vector104.Y += num943 * 5f;
						int num947 = Projectile.NewProjectile(vector104.X, vector104.Y, num942, num943, num946, num945, 0f, Main.myPlayer, 0f, 0f);
						Main.projectile[num947].timeLeft = 180;
						npc.netUpdate = true;
					}
				}
			}
		}
		#endregion

		#region Revengeance Twins AI
		private void RevengeanceRetinazerAI(NPC npc, bool configBossRushBoost)
		{
			bool targetFloatingUp = Main.player[npc.target].gravDir == -1f;

			laserEye = npc.whoAmI;

			bool spazAlive = false;
			if (fireEye != -1)
				spazAlive = Main.npc[fireEye].active;

			if (npc.ai[0] == 0f)
			{
				double healthMult = ((CalamityWorld.death || CalamityWorld.bossRushActive) ? 0.8 : 0.6);
				if ((double)npc.life < (double)npc.lifeMax * healthMult || targetFloatingUp)
				{
					npc.ai[0] = 1f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
					return;
				}
			}
			else if (npc.ai[0] != 1f && npc.ai[0] != 2f && npc.ai[0] != 0f)
			{
				if (spazAlive)
					npc.dontTakeDamage = Main.npc[fireEye].ai[0] == 1f || Main.npc[fireEye].ai[0] == 2f || Main.npc[fireEye].ai[0] == 0f;
				else
					npc.dontTakeDamage = false;

				npc.defense = npc.defDefense + 20;

				if (npc.ai[1] == 0f)
				{
					npc.ai[2] += (spazAlive ? 0.25f : 0.5f);
					npc.localAI[1] += (spazAlive ? 0.5f : 1f);
				}
				else
				{
					npc.ai[2] += (spazAlive ? 0.25f : 0.5f);
					npc.localAI[1] += (spazAlive ? 0.5f : 1f);
				}

				if (targetFloatingUp)
				{
					npc.ai[2] += 1f;
					npc.localAI[1] += 2f;
					newAI[0] += 2f;
				}

				newAI[0] += (spazAlive ? 1f : 2f);
				if (CalamityWorld.death || CalamityWorld.bossRushActive)
					newAI[0] += 1f;

				if (newAI[0] >= ((enraged || configBossRushBoost) ? 180f : 240f))
				{
					newAI[0] = 0f;
					Vector2 vector34 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num349 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector34.X;
					float num350 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector34.Y;
					float num351 = (float)Math.Sqrt((double)(num349 * num349 + num350 * num350));
					num349 *= num351;
					num350 *= num351;
					if (Main.netMode != 1)
					{
						float num353 = 8f;
						int num354 = 29;
						int num355 = mod.ProjectileType("ScavengerLaser");
						vector34 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						num349 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector34.X;
						num350 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector34.Y;
						num351 = (float)Math.Sqrt((double)(num349 * num349 + num350 * num350));
						num351 = num353 / num351;
						num349 *= num351;
						num350 *= num351;
						vector34.X += num349;
						vector34.Y += num350;
						Projectile.NewProjectile(vector34.X, vector34.Y, num349, num350, num355, num354, 0f, Main.myPlayer, 0f, 0f);
					}
				}
			}
		}

		private void RevengeanceSpazmatismAI(NPC npc, bool configBossRushBoost)
		{
			bool targetFloatingUp = Main.player[npc.target].gravDir == -1f;

			fireEye = npc.whoAmI;

			bool retAlive = false;
			if (laserEye != -1)
				retAlive = Main.npc[laserEye].active;

			if (npc.ai[0] == 0f)
			{
				double healthMult = ((CalamityWorld.death || CalamityWorld.bossRushActive) ? 0.8 : 0.6);
				if ((double)npc.life < (double)npc.lifeMax * healthMult || targetFloatingUp)
				{
					npc.ai[0] = 1f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
					return;
				}
			}
			else if (npc.ai[0] != 1f && npc.ai[0] != 2f && npc.ai[0] != 0f)
			{
				if (retAlive)
					npc.dontTakeDamage = Main.npc[laserEye].ai[0] == 1f || Main.npc[laserEye].ai[0] == 2f || Main.npc[laserEye].ai[0] == 0f;
				else
					npc.dontTakeDamage = false;

				npc.defense = npc.defDefense + 30;

				if (npc.ai[1] == 0f)
				{
					if (!Main.player[npc.target].dead)
						newAI[0] += 1f;

					if (newAI[0] >= ((enraged || configBossRushBoost) ? 20f : 30f))
					{
						newAI[0] = 0f;
						if (Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
						{
							Vector2 vector34 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							float num349 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector34.X;
							float num350 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector34.Y;
							float num351 = (float)Math.Sqrt((double)(num349 * num349 + num350 * num350));
							num349 *= num351;
							num350 *= num351;
							if (Main.netMode != 1)
							{
								float num418 = ((enraged || configBossRushBoost) ? 25f : 18f);
								int num419 = 30;
								int num420 = 96;
								num351 = (float)Math.Sqrt((double)(num349 * num349 + num350 * num350));
								num351 = num418 / num351;
								num349 *= num351;
								num350 *= num351;
								num349 += (float)Main.rand.Next(-30, 31) * 0.05f;
								num350 += (float)Main.rand.Next(-30, 31) * 0.05f;
								vector34.X += num349 * 4f;
								vector34.Y += num350 * 4f;
								Projectile.NewProjectile(vector34.X, vector34.Y, num349, num350, num420, num419, 0f, Main.myPlayer, 0f, 0f);
							}
						}
					}
					npc.ai[2] += (retAlive ? 1.5f : 2f);

					if (targetFloatingUp)
					{
						npc.ai[2] += 1f;
						npc.velocity.X *= 1.005f;
						npc.velocity.Y *= 1.005f;
					}

					npc.velocity.X *= 1.005f;
					npc.velocity.Y *= 1.005f;

					if (enraged || configBossRushBoost)
					{
						npc.velocity.X *= 1.01f;
						npc.velocity.Y *= 1.01f;
					}
				}
				else
				{
					npc.ai[2] += (retAlive ? 0f : 0.5f);

					if (targetFloatingUp)
					{
						npc.ai[2] += 0.25f;
						npc.velocity.X *= 1.001f;
						npc.velocity.Y *= 1.001f;
					}

					npc.velocity.X *= (retAlive ? 1.001f : 1.0025f);
					npc.velocity.Y *= (retAlive ? 1.001f : 1.0025f);

					if (CalamityWorld.death || CalamityWorld.bossRushActive)
					{
						npc.velocity.X *= (retAlive ? 1f : 1.001f);
						npc.velocity.Y *= (retAlive ? 1f : 1.001f);
					}

					if (enraged || configBossRushBoost)
					{
						npc.velocity.X *= 1.003f;
						npc.velocity.Y *= 1.003f;
					}
				}
			}
		}
		#endregion

		#region Revengeance Destroyer AI
		private void RevengeanceDestroyerAI(NPC npc, bool configBossRushBoost, Mod mod)
		{
			bool targetFloatingUp = Main.player[npc.target].gravDir == -1f;

			int defenseUp = (int)(20f * (1f - (float)npc.life / (float)npc.lifeMax));
			npc.defense = npc.defDefense + defenseUp;

			if (targetFloatingUp)
			{
				if (newAI[2] < 480f) //8 seconds
					newAI[2] += 1f;
			}
			else
			{
				if (newAI[2] > 0f)
					newAI[2] -= 1f;
			}

			npc.localAI[0] = 0f; //Disable normal laser code

			int shootTime = ((enraged || configBossRushBoost) ? 8 : 4);
			if (CalamityWorld.bossRushActive)
				shootTime += 1;
			if (CalamityWorld.death || CalamityWorld.bossRushActive)
				shootTime += 1;

			if (Main.netMode != 1)
			{
				newAI[0] += (float)Main.rand.Next(shootTime);
				if (newAI[0] >= (float)Main.rand.Next(1400, 26000))
				{
					newAI[0] = 0f;
					npc.TargetClosest(true);
					if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
					{
						float laserSpeedBoost = System.Math.Abs(Main.player[npc.target].velocity.X);
						if (System.Math.Abs(Main.player[npc.target].velocity.X) < System.Math.Abs(Main.player[npc.target].velocity.Y))
							laserSpeedBoost = System.Math.Abs(Main.player[npc.target].velocity.Y);

						float speed = 2f + laserSpeedBoost;

						if (speed < 7f)
							speed = 7f;
						if (speed > 10f)
							speed = 10f;

						if ((double)npc.life <= (double)npc.lifeMax * 0.7)
							speed += 0.25f;
						if ((double)npc.life <= (double)npc.lifeMax * 0.4 || CalamityWorld.bossRushActive)
							speed += CalamityWorld.death ? 0.33f : 0.25f;
						if ((double)npc.life <= (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive)
							speed += CalamityWorld.death ? 0.33f : 0.25f;

						int damage = 28;
						int projectileType = 100;
						int value = (CalamityWorld.bossRushActive ? 3 : 4);

						if (Main.rand.Next(value) == 0 || enraged || configBossRushBoost)
						{
							int secondValue = (newAI[2] > 0f ? 3 : 5);
							projectileType = (Main.rand.Next(secondValue) == 0 ? mod.ProjectileType("DestroyerHomingLaser") : 257);
							damage = 31;
						}

						Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)(npc.height / 2));
						float num6 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector.X;
						float num7 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector.Y;
						float num8 = (float)Math.Sqrt((double)(num6 * num6 + num7 * num7));
						num8 = speed / num8;
						num6 *= num8;
						num7 *= num8;
						vector.X += num6 * 5f;
						vector.Y += num7 * 5f;
						int proj = Projectile.NewProjectile(vector.X, vector.Y, num6, num7, projectileType, damage, 0f, Main.myPlayer, 0f, 0f);

						if (projectileType != mod.ProjectileType("DestroyerHomingLaser"))
							Main.projectile[proj].timeLeft = 300;

						npc.netUpdate = true;
					}
				}
			}
		}
		#endregion

		#region Revengeance Wall of Flesh AI
		private void RevengeanceWallofFleshAI(NPC npc, bool configBossRushBoost)
		{
			if (enraged || configBossRushBoost)
				npc.velocity.X *= 1.7f;
			else if (CalamityWorld.bossRushActive)
				npc.velocity.X *= 1.3f;
			else if (CalamityWorld.death)
				npc.velocity.X *= 1.18f;
			else
				npc.velocity.X *= 1.12f;
		}

		private void RevengeanceWallofFleshEyeAI(NPC npc, bool configBossRushBoost)
		{
			if (NPC.CountNPCS(NPCID.WallofFlesh) > 0)
			{
				Vector2 vector34 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num349 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector34.X;
				float num350 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector34.Y;
				float num351 = (float)Math.Sqrt((double)(num349 * num349 + num350 * num350));
				num349 *= num351;
				num350 *= num351;

				bool flag30 = true;
				if (npc.direction > 0)
				{
					if (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) < npc.position.X + (float)(npc.width / 2))
						flag30 = false;
				}
				else if (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) > npc.position.X + (float)(npc.width / 2))
					flag30 = false;

				if (Main.netMode != 1)
				{
					int num352 = ((enraged || configBossRushBoost) ? 7 : 4);
					npc.localAI[1] = 0f;
					npc.localAI[3] += ((enraged || configBossRushBoost) ? 4f : 2f);

					if ((double)Main.npc[Main.wof].life < (double)Main.npc[Main.wof].lifeMax * 0.75)
					{
						npc.localAI[3] += 1f;
						num352++;
					}
					if ((double)Main.npc[Main.wof].life < (double)Main.npc[Main.wof].lifeMax * 0.5)
					{
						npc.localAI[3] += 1f;
						num352++;
					}
					if ((double)Main.npc[Main.wof].life < (double)Main.npc[Main.wof].lifeMax * 0.25)
					{
						npc.localAI[3] += 1f;
						num352 += 2;
					}
					if ((double)Main.npc[Main.wof].life < (double)Main.npc[Main.wof].lifeMax * 0.1 || CalamityWorld.bossRushActive)
					{
						npc.localAI[3] += 2f;
						num352 += 3;
					}

					if (newAI[0] == 0f)
					{
						if (npc.localAI[3] > 600f)
						{
							newAI[0] = 1f;
							npc.localAI[3] = 0f;
						}
					}
					else if (npc.localAI[3] > 45f && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
					{
						npc.localAI[3] = 0f;

						newAI[0] += 1f;
						if (newAI[0] >= (float)num352)
							newAI[0] = 0f;

						if (flag30)
						{
							float num353 = ((enraged || configBossRushBoost) ? 14f : 10f);
							int num354 = 18;
							int num355 = 83;
							if ((double)Main.npc[Main.wof].life < (double)Main.npc[Main.wof].lifeMax * 0.5)
							{
								num354++;
								num353 += 1f;
							}
							if ((double)Main.npc[Main.wof].life < (double)Main.npc[Main.wof].lifeMax * 0.25)
							{
								num354++;
								num353 += 1f;
							}
							if ((double)Main.npc[Main.wof].life < (double)Main.npc[Main.wof].lifeMax * 0.1 || CalamityWorld.bossRushActive)
							{
								num354 += 2;
								num353 += 2f;
							}
							if (CalamityWorld.death || CalamityWorld.bossRushActive)
								num353 += 1f;

							vector34 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							num349 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector34.X;
							num350 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector34.Y;
							num351 = (float)Math.Sqrt((double)(num349 * num349 + num350 * num350));
							num351 = num353 / num351;
							num349 *= num351;
							num350 *= num351;
							vector34.X += num349;
							vector34.Y += num350;
							Projectile.NewProjectile(vector34.X, vector34.Y, num349, num350, num355, num354, 0f, Main.myPlayer, 0f, 0f);
						}
					}
				}
			}
		}
		#endregion

		#region Revengeance Skeletron AI
		private void RevengeanceSkeletronHandAI(NPC npc, bool configBossRushBoost)
		{
			if (npc.ai[2] == 0f || npc.ai[2] == 3f)
			{
				if (Main.npc[(int)npc.ai[1]].ai[1] == 0f)
				{
					npc.ai[3] += ((enraged || configBossRushBoost) ? 1.5f : 0.5f);
					if (CalamityWorld.death || CalamityWorld.bossRushActive)
						npc.ai[3] += 1f;
				}
			}
		}

		private void RevengeanceSkeletronAI(NPC npc, bool configBossRushBoost)
		{
			if (npc.ai[1] == 1f)
			{
				if (Main.netMode != 1)
				{
					npc.localAI[1] += ((enraged || configBossRushBoost) ? 6f : 3f);
					if ((double)npc.life <= (double)npc.lifeMax * 0.5 || CalamityWorld.bossRushActive)
						npc.localAI[1] += 3f;
					if ((double)npc.life <= (double)npc.lifeMax * 0.15 || CalamityWorld.bossRushActive)
						npc.localAI[1] += 3f;
					if (CalamityWorld.death || CalamityWorld.bossRushActive)
						npc.localAI[1] += 3f;

					if (npc.localAI[1] >= 500f)
					{
						npc.localAI[1] = 0f;
						Vector2 vector16 = npc.Center;
						if (Collision.CanHit(vector16, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
						{
							float num159 = 5f;
							float num160 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector16.X + (float)Main.rand.Next(-20, 21);
							float num161 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector16.Y + (float)Main.rand.Next(-20, 21);
							float num162 = (float)Math.Sqrt((double)(num160 * num160 + num161 * num161));
							num162 = num159 / num162;
							num160 *= num162;
							num161 *= num162;
							Vector2 value = new Vector2(num160 * 1f + (float)Main.rand.Next(-50, 51) * 0.01f, num161 * 1f + (float)Main.rand.Next(-50, 51) * 0.01f);
							value.Normalize();
							value *= num159;
							value += npc.velocity;
							num160 = value.X;
							num161 = value.Y;
							int num163 = 20;
							int num164 = 270;
							vector16 += value * 5f;
							int num165 = Projectile.NewProjectile(vector16.X, vector16.Y, num160, num161, num164, num163, 0f, Main.myPlayer, -1f, 0f);
							Main.projectile[num165].timeLeft = 300;
						}
					}
				}
				Vector2 vector20 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num173 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector20.X;
				float num174 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector20.Y;
				float num175 = (float)Math.Sqrt((double)(num173 * num173 + num174 * num174));

				float num176 = CalamityWorld.bossRushActive ? 10f : 5f;
				if (enraged || configBossRushBoost)
					num176 += 3f;

				if (num175 > 150f)
					num176 *= 1.05f;
				if (num175 > 200f)
					num176 *= 1.1f;
				if (num175 > 250f)
					num176 *= 1.1f;
				if (num175 > 300f)
					num176 *= 1.1f;
				if (num175 > 350f)
					num176 *= 1.1f;
				if (num175 > 400f)
					num176 *= 1.1f;
				if (num175 > 450f)
					num176 *= 1.1f;
				if (num175 > 500f)
					num176 *= 1.1f;
				if (num175 > 550f)
					num176 *= 1.1f;
				if (num175 > 600f)
					num176 *= 1.1f;

				num175 = num176 / num175;
				npc.velocity.X = num173 * num175;
				npc.velocity.Y = num174 * num175;
			}
			else if (npc.ai[1] == 2f)
			{
				Vector2 vector21 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num177 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector21.X;
				float num178 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector21.Y;
				float num179 = (float)Math.Sqrt((double)(num177 * num177 + num178 * num178));
				num179 = 12f / num179;
				npc.velocity.X = num177 * num179;
				npc.velocity.Y = num178 * num179;
				if (Main.netMode != 1)
				{
					npc.localAI[1] += 1f;
					if (npc.localAI[1] >= 60f)
					{
						npc.localAI[1] = 0f;
						Vector2 vector16 = npc.Center;
						if (Collision.CanHit(vector16, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
						{
							float num159 = 5f;
							float num160 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector16.X + (float)Main.rand.Next(-20, 21);
							float num161 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector16.Y + (float)Main.rand.Next(-20, 21);
							float num162 = (float)Math.Sqrt((double)(num160 * num160 + num161 * num161));
							num162 = num159 / num162;
							num160 *= num162;
							num161 *= num162;
							Vector2 value = new Vector2(num160 * 1f + (float)Main.rand.Next(-50, 51) * 0.01f, num161 * 1f + (float)Main.rand.Next(-50, 51) * 0.01f);
							value.Normalize();
							value *= num159;
							value += npc.velocity;
							num160 = value.X;
							num161 = value.Y;
							int num163 = 2500;
							int num164 = 270;
							vector16 += value * 5f;
							int num165 = Projectile.NewProjectile(vector16.X, vector16.Y, num160, num161, num164, num163, 0f, Main.myPlayer, -1f, 0f);
							Main.projectile[num165].timeLeft = 300;
						}
					}
				}
			}
		}
		#endregion

		#region Revengeance Queen Bee AI
		private void RevengeanceQueenBeeAI(NPC npc)
		{
			Vector2 vector74 = new Vector2(npc.position.X + (float)(npc.width / 2) + (float)(Main.rand.Next(20) * npc.direction), npc.position.Y + (float)npc.height * 0.8f);

			if (npc.ai[0] == 1f)
				npc.ai[1] += 1f;
			else if (npc.ai[0] == 3f)
			{
				newAI[0] += 1f;

				bool flag39 = false;
				if (CalamityWorld.death)
				{
					if (newAI[0] % 10f == 9f)
						flag39 = true;
				}
				else if ((double)npc.life < (double)npc.lifeMax * 0.1)
				{
					if (newAI[0] % 20f == 19f)
						flag39 = true;
				}
				else if (npc.life < npc.lifeMax / 3)
				{
					if (newAI[0] % 35f == 34f)
						flag39 = true;
				}
				else if (npc.life < npc.lifeMax / 2)
				{
					if (newAI[0] % 50f == 49f)
						flag39 = true;
				}
				else if (newAI[0] % 55f == 54f)
					flag39 = true;

				if (flag39 && npc.position.Y + (float)npc.height < Main.player[npc.target].position.Y && Collision.CanHit(vector74, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					Main.PlaySound(SoundID.Item17, npc.position);
					if (Main.netMode != 1)
					{
						float num602 = 13f;
						if ((double)npc.life < (double)npc.lifeMax * 0.1)
							num602 += 3f;
						if (CalamityWorld.death)
							num602 += 2f;

						float num603 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector74.X + (float)Main.rand.Next(-80, 81);
						float num604 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector74.Y + (float)Main.rand.Next(-40, 41);
						float num605 = (float)Math.Sqrt((double)(num603 * num603 + num604 * num604));
						num605 = num602 / num605;
						num603 *= num605;
						num604 *= num605;
						int num606 = 13;
						int num607 = 55;
						int num608 = Projectile.NewProjectile(vector74.X, vector74.Y, num603, num604, num607, num606, 0f, Main.myPlayer, 0f, 0f);
						Main.projectile[num608].timeLeft = 300;
					}
				}
			}
		}
		#endregion

		#region Revengeance Brain of Cthulhu AI
		private void RevengeanceBrainofCthulhuAI(NPC npc)
		{
			if (CalamityWorld.death)
			{
				if (npc.ai[0] < 0f)
				{
					newAI[0] += 1f;
					if (newAI[0] >= 480f)
					{
						newAI[0] = 0f;
						if (Main.netMode != 1 && NPC.CountNPCS(NPCID.Creeper) < 15)
						{
							int creeper = NPC.NewNPC((int)(npc.position.X + (float)(npc.width / 2) + npc.velocity.X), (int)(npc.position.Y + (float)(npc.height / 2) + npc.velocity.Y), 267, 0, 0f, 0f, 0f, 0f, 255);
							Main.npc[creeper].netUpdate = true;
						}
						npc.netUpdate = true;
					}
				}
			}
			else
			{
				newAI[0] += 1f;
				if (newAI[0] >= 600f)
				{
					newAI[0] = 0f;
					if (Main.netMode != 1 && NPC.CountNPCS(NPCID.Creeper) < 10)
					{
						int creeper = NPC.NewNPC((int)(npc.position.X + (float)(npc.width / 2) + npc.velocity.X), (int)(npc.position.Y + (float)(npc.height / 2) + npc.velocity.Y), 267, 0, 0f, 0f, 0f, 0f, 255);
						Main.npc[creeper].netUpdate = true;
					}
					npc.netUpdate = true;
				}
			}
			if (npc.ai[0] < 0f)
			{
				npc.knockBackResist = (CalamityWorld.death ? 0.05f : 0.1f) * Main.expertKnockBack;

				npc.velocity.X *= 1.006f;
				npc.velocity.Y *= 1.006f;

				if (CalamityWorld.death)
				{
					npc.velocity.X *= 1.003f;
					npc.velocity.Y *= 1.003f;
				}
			}
		}

		private void RevengeanceCreeperAI(NPC npc)
		{
			if (npc.ai[0] != 0f)
			{
				Vector2 value = Main.player[npc.target].Center - npc.Center;
				value.Normalize();
				value *= 9f;
				npc.velocity = (npc.velocity * 99f + value) / 98f; //100
			}
		}
		#endregion

		#region Revengeance Eater of Worlds AI
		private void RevengeanceEaterofWorldsAI(NPC npc)
		{
			if (!Main.player[npc.target].dead)
			{
				newAI[0] += 1f;
				if (CalamityWorld.death)
					newAI[0] += 2f;
			}
			if (newAI[0] >= 180f)
			{
				newAI[0] = 0f;
				if (Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					Vector2 vector34 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num349 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector34.X;
					float num350 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector34.Y;
					float num351 = (float)Math.Sqrt((double)(num349 * num349 + num350 * num350));
					num349 *= num351;
					num350 *= num351;
					if (Main.netMode != 1)
					{
						float num418 = 12f;
						int num419 = 12;
						int num420 = 96;
						num351 = (float)Math.Sqrt((double)(num349 * num349 + num350 * num350));
						num351 = num418 / num351;
						num349 *= num351;
						num350 *= num351;
						num349 += (float)Main.rand.Next(-40, 41) * 0.05f;
						num350 += (float)Main.rand.Next(-40, 41) * 0.05f;
						vector34.X += num349 * 4f;
						vector34.Y += num350 * 4f;
						Projectile.NewProjectile(vector34.X, vector34.Y, num349, num350, num420, num419, 0f, Main.myPlayer, 0f, 0f);
					}
				}
			}
		}
		#endregion

		#region Revengeance King Slime AI
		private void RevengeanceKingSlimeAI(NPC npc)
		{
			bool move = npc.ai[1] == 5f || npc.ai[1] == 6f;

			if (move)
			{
				npc.ai[0] += 3f;
				if (CalamityWorld.death)
					npc.ai[0] += 2f;
			}

			if (npc.velocity.Y == 0f)
			{
				if (!move)
				{
					npc.ai[0] += 4f;
					if (CalamityWorld.death)
						npc.ai[0] += 4f;
				}
			}
		}
		#endregion

		#region Revengeance Basic NPC AI
		private void RevengeanceLihzahrdAI(NPC npc)
		{
			if (Main.netMode != 1 && (double)npc.life <= (double)npc.lifeMax * 0.9)
				npc.Transform(NPCID.LihzahrdCrawler);
		}

		private void RevengeanceIceGolemAI(NPC npc)
		{
			float num63 = 2f;
			float num64 = 0.14f;
			num63 += (1f - (float)npc.life / (float)npc.lifeMax) * 1.5f;
			num64 += (1f - (float)npc.life / (float)npc.lifeMax) * 0.15f;

			if (npc.velocity.X < -num63 || npc.velocity.X > num63)
				return;
			else if (npc.velocity.X < num63 && npc.direction == 1)
			{
				npc.velocity.X = npc.velocity.X + num64;
				if (npc.velocity.X > num63)
					npc.velocity.X = num63;
			}
			else if (npc.velocity.X > -num63 && npc.direction == -1)
			{
				npc.velocity.X = npc.velocity.X - num64;
				if (npc.velocity.X < -num63)
					npc.velocity.X = -num63;
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
			if (npc.type == NPCID.TheDestroyerBody || npc.type == mod.NPCType("AquaticScourgeBody") || npc.type == mod.NPCType("AquaticScourgeBodyAlt"))
			{
				if (projectile.type == ProjectileID.RainFriendly || projectile.type == ProjectileID.BloodRain ||
					projectile.type == mod.ProjectileType("Shaderain") || projectile.aiStyle == 4 ||
					projectile.type == ProjectileID.CrystalPulse2 || projectile.type == ProjectileID.HallowStar ||
					projectile.type == mod.ProjectileType("FossilShardThrown"))
					damage = (int)((double)damage * 0.65);
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
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, 58, 1, false, 0, false, false);
				}
				else if (!npc.SpawnedFromStatue && npc.damage > 0 && ((double)npc.life > (double)npc.lifeMax * 0.5) &&
					player.GetModPlayer<CalamityPlayer>(mod).bloodflareManaTimer <= 0)
				{
					player.GetModPlayer<CalamityPlayer>(mod).bloodflareManaTimer = 180;
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, 184, 1, false, 0, false, false);
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
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, 58, 1, false, 0, false, false);
				}
				else if (!npc.SpawnedFromStatue && npc.damage > 0 && ((double)npc.life > (double)npc.lifeMax * 0.5) &&
					Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).bloodflareManaTimer <= 0)
				{
					Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).bloodflareManaTimer = 180;
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, 184, 1, false, 0, false, false);
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

		#region Stop Rain
		public static void StopRain()
		{
			Main.raining = false;
			UpdateServerBoolean();
		}
		#endregion

		#region Update Server Boolean
		public static void UpdateServerBoolean()
		{
			if (Main.netMode == 2)
				NetMessage.SendData(7, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
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