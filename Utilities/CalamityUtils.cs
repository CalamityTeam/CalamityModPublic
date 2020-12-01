using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.Items;
using CalamityMod.Items.Tools.ClimateChange;
using CalamityMod.NPCs;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.Providence;
using CalamityMod.Projectiles;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Tiles;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.Astral;
using CalamityMod.Tiles.AstralDesert;
using CalamityMod.Tiles.AstralSnow;
using CalamityMod.Tiles.Crags;
using CalamityMod.Tiles.FurnitureAbyss;
using CalamityMod.Tiles.FurnitureAshen;
using CalamityMod.Tiles.FurnitureEutrophic;
using CalamityMod.Tiles.FurnitureOccult;
using CalamityMod.Tiles.FurnitureProfaned;
using CalamityMod.Tiles.FurnitureVoid;
using CalamityMod.Tiles.Ores;
using CalamityMod.Tiles.SunkenSea;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using Terraria.UI.Chat;

namespace CalamityMod
{
	public static class CalamityUtils
	{
		public static readonly CalamityRarity[] postMLRarities =
		{
			CalamityRarity.Turquoise,
			CalamityRarity.PureGreen,
			CalamityRarity.DarkBlue,
			CalamityRarity.Violet,
			CalamityRarity.Developer,
			CalamityRarity.Rainbow
		};
		
		#region Object Extensions
		public static CalamityPlayer Calamity(this Player player) => player.GetModPlayer<CalamityPlayer>();
		public static CalamityGlobalNPC Calamity(this NPC npc) => npc.GetGlobalNPC<CalamityGlobalNPC>();
		public static CalamityGlobalItem Calamity(this Item item) => item.GetGlobalItem<CalamityGlobalItem>();
		public static CalamityGlobalProjectile Calamity(this Projectile proj) => proj.GetGlobalProjectile<CalamityGlobalProjectile>();
		public static Item ActiveItem(this Player player) => Main.mouseItem.IsAir ? player.HeldItem : Main.mouseItem;
		#endregion

		#region Player Utilities
		// These functions factor in TML 0.11 allDamage to get the player's total damage boost which affects the specified class.
		public static float MeleeDamage(this Player player) => player.allDamage + player.meleeDamage - 1f;
		public static float RangedDamage(this Player player) => player.allDamage + player.rangedDamage - 1f;
		public static float MagicDamage(this Player player) => player.allDamage + player.magicDamage - 1f;
		public static float MinionDamage(this Player player) => player.allDamage + player.minionDamage - 1f;
		public static float ThrownDamage(this Player player) => player.allDamage + player.thrownDamage - 1f;
		public static float RogueDamage(this Player player) => player.allDamage + player.thrownDamage + player.Calamity().throwingDamage - 2f;
		public static float AverageDamage(this Player player) => player.allDamage + (player.meleeDamage + player.rangedDamage + player.magicDamage + player.minionDamage + player.Calamity().throwingDamage - 5f) / 5f;

		public static bool IsUnderwater(this Player player) => Collision.DrownCollision(player.position, player.width, player.height, player.gravDir);
		public static bool StandingStill(this Player player, float velocity = 0.05f) => player.velocity.Length() < velocity;
		public static bool InSpace(this Player player)
		{
			float x = Main.maxTilesX / 4200f;
			x *= x;
			float spaceGravityMult = (float)((player.position.Y / 16f - (60f + 10f * x)) / (Main.worldSurface / 6.0));
			return spaceGravityMult < 1f;
		}
		public static bool PillarZone(this Player player) => player.ZoneTowerStardust || player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula;
		public static bool InCalamity(this Player player) => player.Calamity().ZoneCalamity;
		public static bool InSunkenSea(this Player player) => player.Calamity().ZoneSunkenSea;
		public static bool InSulphur(this Player player) => player.Calamity().ZoneSulphur;
		public static bool InAstral(this Player player, int biome = 0) //1 is above ground, 2 is underground, 3 is desert
		{
			switch (biome)
			{
				case 1:
					return player.Calamity().ZoneAstral && (player.ZoneOverworldHeight || player.ZoneSkyHeight);

				case 2:
					return player.Calamity().ZoneAstral && (player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight || player.ZoneUnderworldHeight);

				case 3:
					return player.Calamity().ZoneAstral && player.ZoneDesert;

				default:
					return player.Calamity().ZoneAstral;
			}
		}
		public static bool InAbyss(this Player player, int layer = 0)
		{
			switch (layer)
			{
				case 1:
					return player.Calamity().ZoneAbyssLayer1;

				case 2:
					return player.Calamity().ZoneAbyssLayer2;

				case 3:
					return player.Calamity().ZoneAbyssLayer3;

				case 4:
					return player.Calamity().ZoneAbyssLayer4;

				default:
					return player.Calamity().ZoneAbyss;
			}
		}
		public static bool InventoryHas(this Player player, params int[] items)
		{
			return player.inventory.Any(item => items.Contains(item.type));
		}
		public static bool PortableStorageHas(this Player player, params int[] items)
		{
			bool hasItem = false;
			if (player.bank.item.Any(item => items.Contains(item.type)))
				hasItem = true;
			if (player.bank2.item.Any(item => items.Contains(item.type)))
				hasItem = true;
			if (player.bank3.item.Any(item => items.Contains(item.type)))
				hasItem = true;
			return hasItem;
		}

		/// <summary>
		/// Inflict typical exo weapon debuffs in pvp.
		/// </summary>
		/// <param name="target">The Player attacked.</param>
		/// <param name="multiplier">Debuff time multiplier if needed.</param>
		/// <returns>Inflicts debuffs if the target isn't immune.</returns>
		public static void ExoDebuffs(this Player target, float multiplier = 1f)
		{
			target.AddBuff(ModContent.BuffType<ExoFreeze>(), (int)(30 * multiplier));
			target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), (int)(120 * multiplier));
			target.AddBuff(ModContent.BuffType<GlacialState>(), (int)(120 * multiplier));
			target.AddBuff(ModContent.BuffType<Plague>(), (int)(120 * multiplier));
			target.AddBuff(ModContent.BuffType<HolyFlames>(), (int)(120 * multiplier));
			target.AddBuff(BuffID.CursedInferno, (int)(120 * multiplier));
			target.AddBuff(BuffID.Frostburn, (int)(120 * multiplier));
			target.AddBuff(BuffID.OnFire, (int)(120 * multiplier));
			target.AddBuff(BuffID.Ichor, (int)(120 * multiplier));
		}
		#endregion

		#region NPC Utilities
		/// <summary>
		/// Allows you to set the lifeMax value of a NPC to different values based on the mode. Called instead of npc.lifeMax = X.
		/// </summary>
		/// <param name="npc">The NPC whose lifeMax value you are trying to set.</param>
		/// <param name="normal">The value lifeMax will be set to in normal mode, this value gets doubled automatically in Expert mode.</param>
		/// <param name="revengeance">The value lifeMax will be set to in Revegeneance mode.</param>
		/// <param name="bossRush">The value lifeMax will be set to during the Boss Rush.</param>
		public static void LifeMaxNERB(this NPC npc, int normal, int? revengeance = null, int? bossRush = null)
		{
			npc.lifeMax = normal;

			if (bossRush.HasValue && BossRushEvent.BossRushActive)
			{
				npc.lifeMax = bossRush.Value;
			}
			else if (revengeance.HasValue && CalamityWorld.revenge)
			{
				npc.lifeMax = revengeance.Value;
			}
		}

		/// <summary>
		/// Allows you to set the DR value of a NPC to different values based on the mode.
		/// </summary>
		/// <param name="npc">The NPC whose DR value you are trying to set.</param>
		/// <param name="normal">The value DR will be set to in normal mode.</param>
		/// <param name="revengeance">The value DR will be set to in Revegeneance mode.</param>
		/// <param name="bossRush">The value DR will be set to during the Boss Rush.</param>
		public static void DR_NERD(this NPC npc, float normal, float? revengeance = null, float? death = null, float? bossRush = null, bool? customDR = null)
		{
			npc.Calamity().DR = normal;

			if (bossRush.HasValue && BossRushEvent.BossRushActive)
			{
				npc.Calamity().DR = bossRush.Value;
			}
			else if (revengeance.HasValue && CalamityWorld.revenge)
			{
				npc.Calamity().DR = CalamityWorld.death ? death.Value : revengeance.Value;
			}

			if (customDR.HasValue)
				npc.Calamity().customDR = true;
		}

		/// <summary>
		/// Get the aggression multiplier used for NPCs in Master Mode Calamity rev+
		/// Will also be used to modify certain boss size/scale
		/// </summary>
		/// <param name="NPCType">The NPC that is having its aggression increased, used to modify the base 1.5x aggression multiplier</param>
		/// <param name="newColor">Used to modify the color of this NPC</param>
		public static float GetMasterModeNPCAggressionMultiplier(ref Color newColor, int? NPCType = null)
		{
			/*if (!Main.masterMode)
				return 1f;*/

			/*if (NPCType == ModContent.NPCType<DesertScourgeHead>())
			{

			}
			else if (NPCType == ModContent.NPCType<CrabulonIdle>())
			{

			}
			else if (NPCType == ModContent.NPCType<HiveMind>() || NPCType == ModContent.NPCType<HiveMindP2>())
			{

			}
			else if (NPCType == ModContent.NPCType<PerforatorHive>())
			{

			}
			else if (NPCType == ModContent.NPCType<SlimeGodCore>() || NPCType == ModContent.NPCType<SlimeGod>() || NPCType == ModContent.NPCType<SlimeGodRun>() || NPCType == ModContent.NPCType<SlimeGodSplit>() || NPCType == ModContent.NPCType<SlimeGodRunSplit>())
			{

			}
			else if (NPCType == ModContent.NPCType<Cryogen>())
			{

			}
			else if (NPCType == ModContent.NPCType<AquaticScourgeHead>())
			{

			}
			else if (NPCType == ModContent.NPCType<BrimstoneElemental>())
			{

			}
			else if (NPCType == ModContent.NPCType<Calamitas>() || NPCType == ModContent.NPCType<CalamitasRun3>())
			{

			}
			else if (NPCType == ModContent.NPCType<Leviathan>() || NPCType == ModContent.NPCType<Siren>())
			{

			}
			else if (NPCType == ModContent.NPCType<AstrumAureus>())
			{

			}
			else if (NPCType == ModContent.NPCType<AstrumDeusHeadSpectral>())
			{

			}
			else if (NPCType == ModContent.NPCType<PlaguebringerGoliath>())
			{

			}
			else if (NPCType == ModContent.NPCType<RavagerBody>())
			{

			}
			else if (NPCType == ModContent.NPCType<ProfanedGuardianBoss>())
			{

			}
			else if (NPCType == ModContent.NPCType<Bumblefuck>())
			{

			}
			else if (NPCType == ModContent.NPCType<Providence>())
			{

			}
			else if (NPCType == ModContent.NPCType<CeaselessVoid>())
			{

			}
			else if (NPCType == ModContent.NPCType<StormWeaverHead>() || NPCType == ModContent.NPCType<StormWeaverHeadNaked>())
			{

			}
			else if (NPCType == ModContent.NPCType<Signus>())
			{

			}
			else if (NPCType == ModContent.NPCType<Polterghast>())
			{

			}
			else if (NPCType == ModContent.NPCType<OldDuke>())
			{

			}
			else if (NPCType == ModContent.NPCType<DevourerofGodsHead>() || NPCType == ModContent.NPCType<DevourerofGodsHeadS>())
			{

			}
			else if (NPCType == ModContent.NPCType<Yharon>())
			{

			}
			else if (NPCType == ModContent.NPCType<SupremeCalamitas>())
			{

			}
			else
			{
				switch (NPCType)
				{
					case NPCID.KingSlime:
					case NPCID.EyeofCthulhu:
					case NPCID.EaterofWorldsHead:
					case NPCID.BrainofCthulhu:
					case NPCID.Creeper:
					case NPCID.QueenBee:
					case NPCID.SkeletronHead:
					case NPCID.WallofFlesh:
					case NPCID.WallofFleshEye:
					case NPCID.Spazmatism:
					case NPCID.Retinazer:
					case NPCID.TheDestroyer:
					case NPCID.SkeletronPrime:
					case NPCID.Plantera:
					case NPCID.Golem:
					case NPCID.GolemHead:
					case NPCID.DukeFishron:
					case NPCID.CultistBoss:
					case NPCID.MoonLordCore:
					case NPCID.MoonLordHand:
					case NPCID.MoonLordHead:
						break;
				}
			}*/

			return 1.5f;
		}

		/// <summary>
		/// Detects nearby hostile NPCs from a given point
		/// </summary>
		/// <param name="origin">The position where we wish to check for nearby NPCs</param>
		/// <param name="maxDistanceToCheck">Maximum amount of pixels to check around the origin</param>
		/// <param name="ignoreTiles">Whether to ignore tiles when finding a target or not</param>
		/// <param name="bossPriority">Whether bosses should be prioritized in targetting or not</param>
		public static NPC ClosestNPCAt(this Vector2 origin, float maxDistanceToCheck, bool ignoreTiles = true, bool bossPriority = false)
		{
			NPC closestTarget = null;
			float distance = maxDistanceToCheck;
			if (bossPriority)
			{
				bool bossFound = false;
				for (int index = 0; index < Main.npc.Length; index++)
				{
					// If we've found a valid boss target, ignore ALL targets which aren't bosses.
					if (bossFound && !(Main.npc[index].boss || Main.npc[index].type == NPCID.WallofFleshEye))
						continue;
					if (Main.npc[index].CanBeChasedBy(null, false))
					{
						float extraDistance = (Main.npc[index].width / 2) + (Main.npc[index].height / 2);

						bool canHit = true;
						if (extraDistance < distance && !ignoreTiles)
							canHit = Collision.CanHit(origin, 1, 1, Main.npc[index].Center, 1, 1);

						if (Vector2.Distance(origin, Main.npc[index].Center) < (distance + extraDistance) && canHit)
						{
							if (Main.npc[index].boss || Main.npc[index].type == NPCID.WallofFleshEye)
								bossFound = true;
							distance = Vector2.Distance(origin, Main.npc[index].Center);
							closestTarget = Main.npc[index];
						}
					}
				}
			}
			else
			{
				for (int index = 0; index < Main.npc.Length; index++)
				{
					if (Main.npc[index].CanBeChasedBy(null, false))
					{
						float extraDistance = (Main.npc[index].width / 2) + (Main.npc[index].height / 2);

						bool canHit = true;
						if (extraDistance < distance && !ignoreTiles)
							canHit = Collision.CanHit(origin, 1, 1, Main.npc[index].Center, 1, 1);

						if (Vector2.Distance(origin, Main.npc[index].Center) < (distance + extraDistance) && canHit)
						{
							distance = Vector2.Distance(origin, Main.npc[index].Center);
							closestTarget = Main.npc[index];
						}
					}
				}
			}
			return closestTarget;
		}
		/// <summary>
		/// Detects nearby hostile NPCs from a given point with minion support
		/// </summary>
		/// <param name="origin">The position where we wish to check for nearby NPCs</param>
		/// <param name="maxDistanceToCheck">Maximum amount of pixels to check around the origin</param>
		/// <param name="owner">Owner of the minion</param>
		/// <param name="ignoreTiles">Whether to ignore tiles when finding a target or not</param>
		public static NPC MinionHoming(this Vector2 origin, float maxDistanceToCheck, Player owner, bool ignoreTiles = true)
		{
			if (owner is null || owner.whoAmI < 0 || owner.whoAmI > Main.maxPlayers || owner.MinionAttackTargetNPC < 0 || owner.MinionAttackTargetNPC > Main.maxNPCs)
				return ClosestNPCAt(origin, maxDistanceToCheck, ignoreTiles);
			NPC npc = Main.npc[owner.MinionAttackTargetNPC];
			bool canHit = true;
			if (!ignoreTiles)
				canHit = Collision.CanHit(origin, 1, 1, npc.Center, 1, 1);
			if (owner.HasMinionAttackTargetNPC && canHit)
			{
				return npc;
			}
			return ClosestNPCAt(origin, maxDistanceToCheck, ignoreTiles);
		}

		/// <summary>
		/// Crude anti-butcher logic based on % max health.
		/// </summary>
		/// <param name="npc">The NPC attacked.</param>
		/// <param name="damage">How much damage the attack would deal.</param>
		/// <returns>Whether or not the anti-butcher was triggered.</returns>
		public static bool AntiButcher(NPC npc, ref double damage, float healthPercent)
		{
			if (damage <= npc.lifeMax * healthPercent)
				return false;
			damage = 0D;
			return true;
		}

		/// <summary>
		/// Check if an NPC is organic
		/// </summary>
		/// <param name="target">The NPC attacked.</param>
		/// <returns>Whether or not the NPC is organic.</returns>
		public static bool Organic(this NPC target)
		{
			if ((target.HitSound != SoundID.NPCHit4 && target.HitSound != SoundID.NPCHit41 && target.HitSound != SoundID.NPCHit2 &&
				target.HitSound != SoundID.NPCHit5 && target.HitSound != SoundID.NPCHit11 && target.HitSound != SoundID.NPCHit30 &&
				target.HitSound != SoundID.NPCHit34 && target.HitSound != SoundID.NPCHit36 && target.HitSound != SoundID.NPCHit42 &&
				target.HitSound != SoundID.NPCHit49 && target.HitSound != SoundID.NPCHit52 && target.HitSound != SoundID.NPCHit53 &&
				target.HitSound != SoundID.NPCHit54 && target.HitSound != null) || target.type == ModContent.NPCType<Providence>() ||
				target.type == ModContent.NPCType<ScornEater>())
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Check if an NPC is inorganic
		/// </summary>
		/// <param name="target">The NPC attacked.</param>
		/// <returns>Whether or not the NPC is inorganic.</returns>
		public static bool Inorganic(this NPC target)
		{
			if (target.HitSound != SoundID.NPCHit1 && target.HitSound != SoundID.NPCHit6 && target.HitSound != SoundID.NPCHit7 &&
				target.HitSound != SoundID.NPCHit8 && target.HitSound != SoundID.NPCHit9 && target.HitSound != SoundID.NPCHit12 &&
				target.HitSound != SoundID.NPCHit13 && target.HitSound != SoundID.NPCHit14 && target.HitSound != SoundID.NPCHit18 &&
				target.HitSound != SoundID.NPCHit19 && target.HitSound != SoundID.NPCHit20 && target.HitSound != SoundID.NPCHit21 &&
				target.HitSound != SoundID.NPCHit22 && target.HitSound != SoundID.NPCHit23 && target.HitSound != SoundID.NPCHit24 &&
				target.HitSound != SoundID.NPCHit25 && target.HitSound != SoundID.NPCHit26 && target.HitSound != SoundID.NPCHit27 &&
				target.HitSound != SoundID.NPCHit28 && target.HitSound != SoundID.NPCHit29 && target.HitSound != SoundID.NPCHit31 &&
				target.HitSound != SoundID.NPCHit32 && target.HitSound != SoundID.NPCHit33 && target.HitSound != SoundID.NPCHit35 &&
				target.HitSound != SoundID.NPCHit37 && target.HitSound != SoundID.NPCHit38 && target.HitSound != SoundID.NPCHit40 &&
				target.HitSound != SoundID.NPCHit43 && target.HitSound != SoundID.NPCHit44 && target.HitSound != SoundID.NPCHit45 &&
				target.HitSound != SoundID.NPCHit46 && target.HitSound != SoundID.NPCHit47 && target.HitSound != SoundID.NPCHit48 &&
				target.HitSound != SoundID.NPCHit50 && target.HitSound != SoundID.NPCHit51 && target.HitSound != SoundID.NPCHit55 &&
				target.HitSound != SoundID.NPCHit56 && target.HitSound != SoundID.NPCHit57 && target.type != ModContent.NPCType<AngryDog>() &&
				target.type != ModContent.NPCType<Providence>() && target.type != ModContent.NPCType<ScornEater>())
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Shortcut for the generic boss summon message.
		/// </summary>
		/// <param name="npcIndex">The whoAmI index of the summoned npc.</param>
		public static void BossAwakenMessage(int npcIndex)
		{
			string typeName = Main.npc[npcIndex].TypeName;
			if (Main.netMode == NetmodeID.SinglePlayer)
			{
				Main.NewText(Language.GetTextValue("Announcement.HasAwoken", typeName), new Color(175, 75, 255));
			}
			else if (Main.netMode == NetmodeID.Server)
			{
				NetMessage.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", new object[]{Main.npc[npcIndex].GetTypeNetName()}), new Color(175, 75, 255));
			}
		}

		public static void DisplayLocalizedText(string key, Color? textColor = null)
		{
			// An attempt to bypass the need for a separate method and runtime/compile-time parameter
			// constraints by using nulls for defaults.
			if (!textColor.HasValue)
				textColor = Color.White;

			if (Main.netMode == NetmodeID.SinglePlayer)
				Main.NewText(Language.GetTextValue(key), textColor.Value);
			else if (Main.netMode == NetmodeID.Server)
				NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), textColor.Value);
		}

		/// Inflict typical exo weapon debuffs. Duration multiplier optional.
		/// </summary>
		/// <param name="target">The NPC attacked.</param>
		/// <param name="multiplier">Debuff time multiplier if needed.</param>
		/// <returns>Inflicts debuffs if they can.</returns>
		public static void ExoDebuffs(this NPC target, float multiplier = 1f)
		{
			target.AddBuff(BuffID.Ichor, (int)(120 * multiplier));
			target.AddBuff(BuffID.CursedInferno, (int)(120 * multiplier));
			target.AddBuff(ModContent.BuffType<ExoFreeze>(), (int)(30 * multiplier));
			target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), (int)(120 * multiplier));
			target.AddBuff(ModContent.BuffType<GlacialState>(), (int)(120 * multiplier));
			target.AddBuff(ModContent.BuffType<Plague>(), (int)(120 * multiplier));
			target.AddBuff(ModContent.BuffType<HolyFlames>(), (int)(120 * multiplier));
			target.AddBuff(BuffID.Frostburn, (int)(120 * multiplier));
			target.AddBuff(BuffID.OnFire, (int)(120 * multiplier));
		}
		#endregion

		#region Item Utilities
		public static bool IsPostML(this CalamityRarity calrare)
		{
			return calrare != CalamityRarity.NoEffect;
			// TODO -- separate out whether an item is post-ML from its custom rarity.
			// This is necessary because there are pre-ML rare variants, legendary weapons and dedicated items.
			/*
			for(int i = 0; i < postMLRarities.Length; ++i)
				if (postMLRarities[i] == calrare)
					return true;
			return false;
			*/
		}

		/// <summary>
		/// Converts the given ModHotKey into a string for insertion into item tooltips.<br></br>
		/// This allows the user's actual keybind choices to be shown to them in tooltips.
		/// </summary>
		/// <param name="mhk">The ModHotKey to convert to a string.</param>
		/// <returns></returns>
		public static string TooltipHotkeyString(this ModHotKey mhk)
		{
			if (Main.dedServ || mhk is null)
				return "";

			List<string> keys = mhk.GetAssignedKeys();
			if (keys.Count == 0)
				return "[NONE]";
			else
			{
				StringBuilder sb = new StringBuilder(16);
				sb.Append(keys[0]);

				// In almost all cases, this code won't run, because there won't be multiple bindings for the hotkey. But just in case...
				for (int i = 1; i < keys.Count; ++i)
					sb.Append(" / ").Append(keys[i]);
				return sb.ToString();
			}
		}

		private const float WorldInsertionOffset = 15f;
		/// <summary>
		/// If the given item is outside the world, force it to be within the world boundaries.
		/// </summary>
		/// <param name="item">The item to possibly relocate.</param>
		/// <param name="dist">The minimum distance in pixels the item can be from the world boundary.</param>
		/// <returns>Whether the item was relocated.</returns>
		public static bool ForceItemIntoWorld(Item item, float desiredDist = WorldInsertionOffset)
		{
			if (item is null || !item.active)
				return false;

			// The world edge needs to be accounted for regardless of the distance chosen as an argument.
			float worldEdge = Main.offLimitBorderTiles * 16f;
			float dist = worldEdge + desiredDist;

			float maxPosX = Main.maxTilesX * 16f;
			float maxPosY = Main.maxTilesY * 16f;
			bool moved = false;
			if (item.position.X < worldEdge)
			{
				item.position.X = dist;
				moved = true;
			}
			else if(item.position.X + item.width > maxPosX - worldEdge)
			{
				item.position.X = maxPosX - item.width - dist;
				moved = true;
			}
			if(item.position.Y < worldEdge)
			{
				item.position.Y = dist;
				moved = true;
			}
			else if(item.position.Y + item.height > maxPosY - worldEdge)
			{
				item.position.Y = maxPosY - item.height - dist;
				moved = true;
			}
			return moved;
		}

		public static Rectangle FixSwingHitbox(float hitboxWidth, float hitboxHeight)
		{
			Player player = Main.player[Main.myPlayer];
			Item item = player.ActiveItem();
			float hitbox_X, hitbox_Y;
			float mountOffsetY = player.mount.PlayerOffsetHitbox;

			// Third hitbox shifting values
			if (player.itemAnimation < player.itemAnimationMax * 0.333)
			{
				float shiftX = 10f;
				if (hitboxWidth >= 92)
					shiftX = 38f;
				else if (hitboxWidth >= 64)
					shiftX = 28f;
				else if (hitboxWidth >= 52)
					shiftX = 24f;
				else if (hitboxWidth > 32)
					shiftX = 14f;
				hitbox_X = player.position.X + player.width * 0.5f + (hitboxWidth * 0.5f - shiftX) * player.direction;
				hitbox_Y = player.position.Y + 24f + mountOffsetY;
			}

			// Second hitbox shifting values
			else if (player.itemAnimation < player.itemAnimationMax * 0.666)
			{
				float shift = 10f;
				if (hitboxWidth >= 92)
					shift = 38f;
				else if (hitboxWidth >= 64)
					shift = 28f;
				else if (hitboxWidth >= 52)
					shift = 24f;
				else if (hitboxWidth > 32)
					shift = 18f;
				hitbox_X = player.position.X + (player.width * 0.5f + (hitboxWidth * 0.5f - shift) * player.direction);

				shift = 10f;
				if (hitboxHeight > 64)
					shift = 14f;
				else if (hitboxHeight > 52)
					shift = 12f;
				else if (hitboxHeight > 32)
					shift = 8f;

				hitbox_Y = player.position.Y + shift + mountOffsetY;
			}

			// First hitbox shifting values
			else
			{
				float shift = 6f;
				if (hitboxWidth >= 92)
					shift = 38f;
				else if (hitboxWidth >= 64)
					shift = 28f;
				else if (hitboxWidth >= 52)
					shift = 24f;
				else if (hitboxWidth >= 48)
					shift = 18f;
				else if (hitboxWidth > 32)
					shift = 14f;
				hitbox_X = player.position.X + player.width * 0.5f - (hitboxWidth * 0.5f - shift) * player.direction;

				shift = 10f;
				if (hitboxHeight > 64)
					shift = 14f;
				else if (hitboxHeight > 52)
					shift = 12f;
				else if (hitboxHeight > 32)
					shift = 10f;
				hitbox_Y = player.position.Y + shift + mountOffsetY;
			}

			// Inversion due to grav potion
			if (player.gravDir == -1f)
			{
				hitbox_Y = player.position.Y + player.height + (player.position.Y - hitbox_Y);
			}

			// Hitbox size adjustments
			Rectangle hitbox = new Rectangle((int)hitbox_X, (int)hitbox_Y, 32, 32);
			if (item.damage >= 0 && item.type > ItemID.None && !item.noMelee && player.itemAnimation > 0)
			{
				if (!Main.dedServ)
				{
					hitbox = new Rectangle((int)hitbox_X, (int)hitbox_Y, (int)hitboxWidth, (int)hitboxHeight);
				}
				hitbox.Width = (int)(hitbox.Width * item.scale);
				hitbox.Height = (int)(hitbox.Height * item.scale);
				if (player.direction == -1)
				{
					hitbox.X -= hitbox.Width;
				}
				if (player.gravDir == 1f)
				{
					hitbox.Y -= hitbox.Height;
				}

				// Broadsword use style
				if (item.useStyle == ItemUseStyleID.SwingThrow)
				{
					// Third hitbox size adjustments
					if (player.itemAnimation < player.itemAnimationMax * 0.333)
					{
						if (player.direction == -1)
						{
							hitbox.X -= (int)(hitbox.Width * 1.4 - hitbox.Width);
						}
						hitbox.Width = (int)(hitbox.Width * 1.4);
						hitbox.Y += (int)(hitbox.Height * 0.5 * player.gravDir);
						hitbox.Height = (int)(hitbox.Height * 1.1);
					}

					// First hitbox size adjustments
					else if (player.itemAnimation >= player.itemAnimationMax * 0.666)
					{
						if (player.direction == 1)
						{
							hitbox.X -= (int)(hitbox.Width * 1.2);
						}
						hitbox.Width *= 2;
						hitbox.Y -= (int)((hitbox.Height * 1.4 - hitbox.Height) * player.gravDir);
						hitbox.Height = (int)(hitbox.Height * 1.4);
					}
				}
			}
			return hitbox;
		}

		public static void ConsumeItemViaQuickBuff(Player player, Item item, int buffType, int buffTime, bool reducedPotionSickness)
		{
			bool showsOver = false;
			//Fail if you have the buff
			for (int l = 0; l < Player.MaxBuffs; l++)
			{
				int hasBuff = player.buffType[l];
				if (player.buffTime[l] > 0 && hasBuff == buffType)
					showsOver = true;
			}
			//Fail if you have potion sickness
			if (player.potionDelay > 0 || player.Calamity().potionTimer > 0)
				showsOver = true;

			if (!showsOver)
			{
				Main.PlaySound(item.UseSound, player.Center);

				double healMult = 1D +
						(player.Calamity().coreOfTheBloodGod ? 0.15 : 0) +
						(player.Calamity().bloodPactBoost ? 0.5 : 0);
				int healAmt = (int)(item.healLife * healMult);
				if (CalamityWorld.ironHeart)
					healAmt = 0;
				if (healAmt > 0 && player.QuickHeal_GetItemToUse() != null)
				{
					if (player.QuickHeal_GetItemToUse().type != item.type)
						healAmt = 0;
				}

				player.statLife += healAmt;
				player.statMana += item.healMana;
				if (player.statMana > player.statManaMax2)
				{
					player.statMana = player.statManaMax2;
				}
				if (player.statLife > player.statLifeMax2)
				{
					player.statLife = player.statLifeMax2;
				}
				if (item.healMana > 0)
					player.AddBuff(BuffID.ManaSickness, Player.manaSickTime, true);
				if (Main.myPlayer == player.whoAmI)
				{
					if (healAmt > 0)
						player.HealEffect(healAmt, true);
					if (item.healMana > 0)
						player.ManaEffect(item.healMana);
				}
				if (item.potion)
				{
					int duration = reducedPotionSickness ? 3000 : 3600;
					if (player.pStone)
						duration = (int)(duration * 0.75);
					player.AddBuff(BuffID.PotionSickness, duration);
				}

				player.AddBuff(buffType, buffTime);

				--item.stack;
				if (item.stack <= 0)
					item.TurnToAir();
				Recipe.FindRecipes();
			}
		}
		#endregion

		#region Projectile Utilities
		public static int CountProjectiles(int Type) => Main.projectile.Count(proj => proj.type == Type && proj.active);

		public static int CountHookProj() => Main.projectile.Count(proj => Main.projHook[proj.type] && proj.ai[0] == 2f && proj.active && proj.owner == Main.myPlayer);

		public static bool FinalExtraUpdate(this Projectile proj) => proj.numUpdates == -1;

		public static bool IsSummon(this Projectile proj) => proj.minion || proj.sentry || CalamityLists.projectileMinionList.Contains(proj.type) || ProjectileID.Sets.MinionShot[proj.type] || ProjectileID.Sets.SentryShot[proj.type];

		public static void KillAllHostileProjectiles()
		{
			for (int x = 0; x < Main.maxProjectiles; x++)
			{
				Projectile projectile = Main.projectile[x];
				if (projectile.active && projectile.hostile && !projectile.friendly && projectile.damage > 0)
				{
					projectile.Kill();
				}
			}
		}

		public static void KillShootProjectiles(bool shouldBreak, int projType, Player player)
		{
			for (int x = 0; x < Main.maxProjectiles; x++)
			{
				Projectile proj = Main.projectile[x];
				if (proj.active && proj.owner == player.whoAmI && proj.type == projType)
				{
					proj.Kill();
					if (shouldBreak)
						break;
				}
			}
		}

		public static void KillShootProjectileMany(Player player, params int[] projTypes)
		{
			for (int x = 0; x < Main.maxProjectiles; x++)
			{
				Projectile proj = Main.projectile[x];
				if (proj.active && proj.owner == player.whoAmI && projTypes.Contains(proj.type))
				{
					proj.Kill();
				}
			}
		}

		public static int FindFirstProjectile(int Type)
		{
			int index = -1;
			for (int x = 0; x < Main.maxProjectiles; x++)
			{
				Projectile proj = Main.projectile[x];
				if (proj.active && proj.type == Type)
				{
					index = x;
					break;
				}
			}
			return index;
		}

		public static void OnlyOneSentry(Player player, int Type)
		{
			int existingTurrets = player.ownedProjectileCounts[Type];
			if (existingTurrets > 0)
			{
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					if (Main.projectile[i].type == Type &&
						Main.projectile[i].owner == player.whoAmI &&
						Main.projectile[i].active)
					{
						Main.projectile[i].Kill();
						existingTurrets--;
						if (existingTurrets <= 0)
							break;
					}
				}
			}
		}

		/// <summary>
		/// Call this function in the ai of your projectile so it can stick to enemies, also requires ModifyHitNPCSticky to be called in ModifyHitNPC
		/// </summary>
		/// <param name="projectile">The projectile you're adding sticky behaviour to</param>
		/// <param name="timeLeft">Number of seconds you want a projectile to cling to an NPC</param>
		public static void StickyProjAI (this Projectile projectile, int timeLeft)
		{
			if (projectile.ai[0] == 1f)
			{
				int seconds = timeLeft;
				bool killProj = false;
				bool spawnDust = false;

				//the projectile follows the NPC, even if it goes into blocks
				projectile.tileCollide = false;

				//timer for triggering hit effects
				projectile.localAI[0]++;
				if (projectile.localAI[0] % 30f == 0f)
				{
					spawnDust = true;
				}

				//So AI knows what NPC it is sticking to
				int npcIndex = (int)projectile.ai[1];
				NPC npc = Main.npc[npcIndex];

				//Kill projectile after so many seconds or if the NPC it is stuck to no longer exists
				if (projectile.localAI[0] >= (float)(60 * seconds))
				{
					killProj = true;
				}
				else if (npcIndex < 0 || npcIndex >= Main.maxNPCs)
				{
					killProj = true;
				}

				else if (npc.active && !npc.dontTakeDamage)
				{
					//follow the NPC
					projectile.Center = npc.Center - projectile.velocity * 2f;
					projectile.gfxOffY = npc.gfxOffY;

					//if attached to npc, trigger npc hit effects every half a second
					if (spawnDust)
					{
						npc.HitEffect(0, 1.0);
					}
				}
				else
				{
					killProj = true;
				}

				//Kill the projectile if needed
				if (killProj)
				{
					projectile.Kill();
				}
			}
		}

		/// <summary>
		/// Call this function in ModifyHitNPC to make your projectiles stick to enemies, needs StickyProjAI to be called in the AI of the projectile
		/// </summary>
		/// <param name="projectile">The projectile you're giving sticky behaviour to</param>
		/// <param name="maxStick">How many projectiles of this type can stick to one enemy</param>
		/// <param name="constantDamage">Decides if you want the projectile to deal damage while its sticked to enemies or not</param>
		public static void ModifyHitNPCSticky(this Projectile projectile, int maxStick, bool constantDamage)
		{
			Player player = Main.player[projectile.owner];
			Rectangle myRect = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);

			if (projectile.owner == Main.myPlayer)
			{
				for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
				{
					NPC npc = Main.npc[npcIndex];
					//covers most edge cases like voodoo dolls
					if (npc.active && !npc.dontTakeDamage &&
						((projectile.friendly && (!npc.friendly || (npc.type == NPCID.Guide && projectile.owner < Main.maxPlayers && player.killGuide) || (npc.type == NPCID.Clothier && projectile.owner < Main.maxPlayers && player.killClothier))) ||
						(projectile.hostile && npc.friendly && !npc.dontTakeDamageFromHostiles)) && (projectile.owner < 0 || npc.immune[projectile.owner] == 0 || projectile.maxPenetrate == 1))
					{
						if (npc.noTileCollide || !projectile.ownerHitCheck || projectile.CanHit(npc))
						{
							bool stickingToNPC;
							//Solar Crawltipede tail has special collision
							if (npc.type == NPCID.SolarCrawltipedeTail)
							{
								Rectangle rect = npc.getRect();
								int num5 = 8;
								rect.X -= num5;
								rect.Y -= num5;
								rect.Width += num5 * 2;
								rect.Height += num5 * 2;
								stickingToNPC = projectile.Colliding(myRect, rect);
							}
							else
							{
								stickingToNPC = projectile.Colliding(myRect, npc.getRect());
							}
							if (stickingToNPC)
							{
								//reflect projectile if the npc can reflect it (like Selenians)
								if (npc.reflectingProjectiles && projectile.CanReflect())
								{
									npc.ReflectProjectile(projectile.whoAmI);
									return;
								}

								//let the projectile know it is sticking and the npc it is sticking too
								projectile.ai[0] = 1f;
								projectile.ai[1] = (float)npcIndex;

								//follow the NPC
								projectile.velocity = (npc.Center - projectile.Center) * 0.75f;

								projectile.netUpdate = true;

								//Set projectile damage to 0 if desired
								if (!constantDamage)
									projectile.damage = 0;

								//Count how many projectiles are attached, delete as necessary
								Point[] array2 = new Point[maxStick];
								int projCount = 0;
								for (int projIndex = 0; projIndex < Main.maxProjectiles; projIndex++)
								{
									Projectile proj = Main.projectile[projIndex];
									if (projIndex != projectile.whoAmI && proj.active && proj.owner == Main.myPlayer && proj.type == projectile.type && proj.ai[0] == 1f && proj.ai[1] == (float)npcIndex)
									{
										array2[projCount++] = new Point(projIndex, proj.timeLeft);
										if (projCount >= array2.Length)
										{
											break;
										}
									}
								}
								if (projCount >= array2.Length)
								{
									int num30 = 0;
									for (int m = 1; m < array2.Length; m++)
									{
										if (array2[m].Y < array2[num30].Y)
										{
											num30 = m;
										}
									}
									Main.projectile[array2[num30].X].Kill();
								}
							}
						}
					}
				}
			}
		}

		public static void StickToTiles(this Projectile projectile, bool ignorePlatforms, bool stickToEverything)
		{
			try
			{
				int xLeft = (int)(projectile.position.X / 16f) - 1;
				int xRight = (int)((projectile.position.X + (float)projectile.width) / 16f) + 2;
				int yBottom = (int)(projectile.position.Y / 16f) - 1;
				int yTop = (int)((projectile.position.Y + (float)projectile.height) / 16f) + 2;
				if (xLeft < 0)
				{
					xLeft = 0;
				}
				if (xRight > Main.maxTilesX)
				{
					xRight = Main.maxTilesX;
				}
				if (yBottom < 0)
				{
					yBottom = 0;
				}
				if (yTop > Main.maxTilesY)
				{
					yTop = Main.maxTilesY;
				}
				for (int x = xLeft; x < xRight; x++)
				{
					for (int y = yBottom; y < yTop; y++)
					{
						Tile tile = Main.tile[x, y];
						bool platformCheck = true;
						if (ignorePlatforms)
							platformCheck = !TileID.Sets.Platforms[tile.type] && tile.type != TileID.PlanterBox;
						bool tableCheck = false;
						if (stickToEverything)
							tableCheck = Main.tileSolidTop[tile.type] && tile.frameY == 0;
						if (tile != null && tile.nactive() && platformCheck && (Main.tileSolid[tile.type] || tableCheck))
						{
							Vector2 tileSize;
							tileSize.X = (float)(x * 16);
							tileSize.Y = (float)(y * 16);
							if (projectile.position.X + (float)projectile.width - 4f > tileSize.X && projectile.position.X + 4f < tileSize.X + 16f && projectile.position.Y + (float)projectile.height - 4f > tileSize.Y && projectile.position.Y + 4f < tileSize.Y + 16f)
							{
								projectile.velocity.X = 0f;
								projectile.velocity.Y = -0.2f;
							}
						}
					}
				}
			} catch
			{
			}
		}

		public static Projectile ProjectileRain(Vector2 targetPos, float xLimit, float xVariance, float yLimitLower, float yLimitUpper, float projSpeed, int projType, int damage, float knockback, int owner, int forceType = 0, int immunitySetting = 0, int cooldown = 10, int extraUpdates = 0)
		{
			float x = targetPos.X + Main.rand.NextFloat(-xLimit, xLimit);
			if (projType == ModContent.ProjectileType<AstralStarMagic>())
				x = targetPos.X + xLimit;
			float y = targetPos.Y - Main.rand.NextFloat(yLimitLower, yLimitUpper);
			Vector2 source = new Vector2(x, y);
			Vector2 velocity = targetPos - source;
			velocity.X += Main.rand.NextFloat(-xVariance, xVariance);
			float speed = projSpeed;
			float targetDist = velocity.Length();
			targetDist = speed / targetDist;
			velocity.X *= targetDist;
			velocity.Y *= targetDist;
			Projectile proj = Projectile.NewProjectileDirect(source, velocity, projType, damage, knockback, owner, 0f, 0f);
			proj.extraUpdates += extraUpdates;
			CalamityGlobalProjectile modProj = proj.Calamity();
			if (forceType > 0)
			{
				switch (forceType)
				{
					case 1:
						modProj.forceMelee = true;
						break;
					case 2:
						modProj.forceRanged = true;
						break;
					case 3:
						modProj.forceMagic = true;
						break;
					case 4:
						modProj.forceMinion = true;
						break;
					case 5:
						modProj.forceRogue = true;
						break;
					case 6:
						modProj.forceTypeless = true;
						break;
				}
			}
			if (immunitySetting > 0)
			{
				switch (forceType)
				{
					case 1:
						proj.usesLocalNPCImmunity = true;
						proj.localNPCHitCooldown = cooldown;
						proj.usesIDStaticNPCImmunity = false;
						break;
					case 2:
						proj.usesLocalNPCImmunity = false;
						proj.idStaticNPCHitCooldown = cooldown;
						proj.usesIDStaticNPCImmunity = true;
						break;
				}
			}
			return proj;
		}

		public static Projectile ProjectileBarrage(Vector2 originVec, Vector2 targetPos, bool fromRight, float xOffsetMin, float xOffsetMax, float yOffsetMin, float yOffsetMax, float projSpeed, int projType, int damage, float knockback, int owner, bool clamped = false, float inaccuracyOffset = 5f)
		{
			float xPos = originVec.X + Main.rand.NextFloat(xOffsetMin, xOffsetMax) * fromRight.ToDirectionInt();
			float yPos = originVec.Y + Main.rand.NextFloat(yOffsetMin, yOffsetMax) * Main.rand.NextBool().ToDirectionInt();
			Vector2 spawnPosition = new Vector2(xPos, yPos);
			Vector2 velocity = targetPos - spawnPosition;
			velocity.X += Main.rand.NextFloat(-inaccuracyOffset, inaccuracyOffset);
			velocity.Y += Main.rand.NextFloat(-inaccuracyOffset, inaccuracyOffset);
			velocity.Normalize();
			velocity *= projSpeed * (clamped ? 150f : 1f);
			//This clamp means the spawned projectiles only go at diagnals and are not accurate
			if (clamped)
			{
				velocity.X = MathHelper.Clamp(velocity.X, -15f, 15f);
				velocity.Y = MathHelper.Clamp(velocity.Y, -15f, 15f);
			}
			Projectile proj = Projectile.NewProjectileDirect(spawnPosition, velocity, projType, damage, knockback, owner);
			return proj;
		}

		public static int DamageSoftCap(double dmgInput, int cap)
		{
			int newDamage = (int)dmgInput;
			if (newDamage > cap)
			{
				newDamage = (int)((dmgInput - cap) * 0.1) + cap;
			}
			if (newDamage < 1)
				newDamage = 1;
			return newDamage;
		}

		public static Vector2 RandomVelocity(float directionMult, float speedLowerLimit, float speedCap, float speedMult = 0.1f)
		{
			Vector2 velocity = new Vector2(Main.rand.NextFloat(-directionMult, directionMult), Main.rand.NextFloat(-directionMult, directionMult));
			//Rerolling to avoid dividing by zero
			while (velocity.X == 0f && velocity.Y == 0f)
			{
				velocity = new Vector2(Main.rand.NextFloat(-directionMult, directionMult), Main.rand.NextFloat(-directionMult, directionMult));
			}
			velocity.Normalize();
			velocity *= Main.rand.NextFloat(speedLowerLimit, speedCap) * speedMult;
			return velocity;
		}

		public static void SporeSacAI(this Projectile projectile)
		{
			Player player = Main.player[projectile.owner];

			float scaleAmt = 1f - (float)projectile.alpha / 255f;
			scaleAmt *= projectile.scale;
			Lighting.AddLight(projectile.Center, 0.25f * scaleAmt, 0.025f * scaleAmt, 0.275f * scaleAmt);

			projectile.localAI[0] += 1f;
			if (projectile.localAI[0] >= 90f)
			{
				projectile.localAI[0] *= -1f;
			}
			if (projectile.localAI[0] >= 0f)
			{
				projectile.scale += 0.003f;
			}
			else
			{
				projectile.scale -= 0.003f;
			}
			projectile.rotation += 0.0025f * projectile.scale;
			float yVel = 1f;
			float xVel = 1f;
			if (projectile.identity % 6 == 0)
			{
				xVel *= -1f;
			}
			if (projectile.identity % 6 == 1)
			{
				yVel *= -1f;
			}
			if (projectile.identity % 6 == 2)
			{
				xVel *= -1f;
				yVel *= -1f;
			}
			if (projectile.identity % 6 == 3)
			{
				xVel = 0f;
			}
			if (projectile.identity % 6 == 4)
			{
				yVel = 0f;
			}
			projectile.localAI[1] += 1f;
			if (projectile.localAI[1] > 60f)
			{
				projectile.localAI[1] = -180f;
			}
			if (projectile.localAI[1] >= -60f)
			{
				projectile.velocity.X += 0.002f * xVel;
				projectile.velocity.Y += 0.002f * yVel;
			}
			else
			{
				projectile.velocity.X -= 0.002f * xVel;
				projectile.velocity.Y -= 0.002f * yVel;
			}
			projectile.ai[0] += 1f;
			if (projectile.ai[0] > 5400f)
			{
				projectile.damage = 0;
				projectile.ai[1] = 1f;
				if (projectile.alpha < 255)
				{
					projectile.alpha += 5;
					if (projectile.alpha > 255)
					{
						projectile.alpha = 255;
					}
				}
				else if (projectile.owner == Main.myPlayer)
				{
					projectile.Kill();
				}
			}
			else
			{
				float playerDist = (projectile.Center - player.Center).Length() / 100f;
				if (playerDist > 4f)
				{
					playerDist *= 1.1f;
				}
				if (playerDist > 5f)
				{
					playerDist *= 1.2f;
				}
				if (playerDist > 6f)
				{
					playerDist *= 1.3f;
				}
				if (playerDist > 7f)
				{
					playerDist *= 1.4f;
				}
				if (playerDist > 8f)
				{
					playerDist *= 1.5f;
				}
				if (playerDist > 9f)
				{
					playerDist *= 1.6f;
				}
				if (playerDist > 10f)
				{
					playerDist *= 1.7f;
				}
				projectile.ai[0] += playerDist;
				if (projectile.alpha > 50)
				{
					projectile.alpha -= 10;
					if (projectile.alpha < 50)
					{
						projectile.alpha = 50;
					}
				}
			}
			if (projectile.Calamity().lineColor != 1 || projectile.timeLeft < (3300 + projectile.localAI[0]))
			{
				bool foundTarget = false;
				Vector2 targetPos = new Vector2(0f, 0f);
				float maxDist = 600f;
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC npc = Main.npc[i];
					if (npc.CanBeChasedBy(projectile, false))
					{
						float targetDist = Vector2.Distance(projectile.Center, npc.Center);
						if (targetDist < maxDist)
						{
							targetPos = npc.Center;
							foundTarget = true;
							break;
						}
					}
				}
				if (foundTarget)
				{
					if (projectile.Calamity().lineColor == 1)
						projectile.extraUpdates = 5;

					Vector2 targetVec = targetPos - projectile.Center;
					targetVec.Normalize();
					targetVec *= 0.75f;
					projectile.velocity = (projectile.velocity * 10f + targetVec) / 11f;
					return;
				}
			}
			if (projectile.velocity.Length() > 0.2f)
			{
				projectile.velocity *= 0.98f;
			}
		}

		public static void MinionAntiClump(this Projectile projectile, float pushForce = 0.05f)
		{
			for (int k = 0; k < Main.maxProjectiles; k++)
			{
				Projectile otherProj = Main.projectile[k];
				// Short circuits to make the loop as fast as possible
				if (!otherProj.active || otherProj.owner != projectile.owner || !otherProj.minion || k == projectile.whoAmI)
					continue;

				// If the other projectile is indeed the same owned by the same player and they're too close, nudge them away.
				bool sameProjType = otherProj.type == projectile.type;
				float taxicabDist = Math.Abs(projectile.position.X - otherProj.position.X) + Math.Abs(projectile.position.Y - otherProj.position.Y);
				if (sameProjType && taxicabDist < projectile.width)
				{
					if (projectile.position.X < otherProj.position.X)
						projectile.velocity.X -= pushForce;
					else
						projectile.velocity.X += pushForce;

					if (projectile.position.Y < otherProj.position.Y)
						projectile.velocity.Y -= pushForce;
					else
						projectile.velocity.Y += pushForce;
				}
			}
		}

		public static void ChargingMinionAI(this Projectile projectile, float range, float maxPlayerDist, float extraMaxPlayerDist, float safeDist, int initialUpdates, float chargeDelayTime, float goToSpeed, float goBackSpeed, Vector2 returnOffset, float chargeCounterMax, float chargeSpeed, bool tileVision, bool ignoreTilesWhenCharging, int updateDifference = 1)
		{
			Player player = Main.player[projectile.owner];
			CalamityPlayer modPlayer = player.Calamity();

			//Anti sticky movement to prevent stacking
			projectile.MinionAntiClump();

			//Breather time between charges as like a reset
			bool chargeDelay = false;
			if (projectile.ai[0] == 2f)
			{
				projectile.ai[1] += 1f;
				projectile.extraUpdates = initialUpdates + updateDifference;
				if (projectile.ai[1] > chargeDelayTime)
				{
					projectile.ai[1] = 1f;
					projectile.ai[0] = 0f;
					projectile.extraUpdates = initialUpdates;
					projectile.numUpdates = 0;
					projectile.netUpdate = true;
				}
				else
				{
					chargeDelay = true;
				}
			}
			if (chargeDelay)
			{
				return;
			}

			//Find a target
			float maxDist = range;
			Vector2 targetVec = projectile.position;
			bool foundTarget = false;
			bool isButterfly = projectile.type == ModContent.ProjectileType<PurpleButterfly>();
			//Prioritize the targeted enemy if possible
			if (player.HasMinionAttackTargetNPC)
			{
				NPC npc = Main.npc[player.MinionAttackTargetNPC];
				bool fishronCheck = npc.type == NPCID.DukeFishron && npc.active && isButterfly;
				if (npc.CanBeChasedBy(projectile, false) || fishronCheck)
				{
					//Check the size of the target to make it easier to hit fat targets like Levi
					float extraDist = (npc.width / 2) + (npc.height / 2);

					float targetDist = Vector2.Distance(npc.Center, projectile.Center);
					//Some minions will ignore tiles when choosing a target like Ice Claspers, others will not
					bool canHit = true;
					if (extraDist < maxDist && !tileVision)
						canHit = Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1);
					if (!foundTarget && targetDist < (maxDist + extraDist) && canHit)
					{
						maxDist = targetDist;
						targetVec = npc.Center;
						foundTarget = true;
					}
				}
			}
			//If no npc is specifically targetted or the selected enemy can't be found, check through the entire array
			if (!foundTarget)
			{
				for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
				{
					NPC npc = Main.npc[npcIndex];
					bool fishronCheck = npc.type == NPCID.DukeFishron && npc.active && isButterfly;
					if (npc.CanBeChasedBy(projectile, false) || fishronCheck)
					{
						float extraDist = (npc.width / 2) + (npc.height / 2);
						float targetDist = Vector2.Distance(npc.Center, projectile.Center);
						bool canHit = true;
						if (extraDist < maxDist && !tileVision)
							canHit = Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1);
						if (!foundTarget && targetDist < (maxDist + extraDist) && canHit)
						{
							maxDist = targetDist;
							targetVec = npc.Center;
							foundTarget = true;
						}
					}
				}
			}

			//If the player is too far, return to the player. Range is increased while attacking something.
			float distBeforeForcedReturn = maxPlayerDist;
			if (foundTarget)
			{
				distBeforeForcedReturn = extraMaxPlayerDist;
			}
			if (Vector2.Distance(player.Center, projectile.Center) > distBeforeForcedReturn)
			{
				projectile.ai[0] = 1f;
				projectile.netUpdate = true;
			}

			//Go to the target if you found one
			if (foundTarget && projectile.ai[0] == 0f)
			{
				//Some minions don't ignore tiles while charging like brittle stars
				projectile.tileCollide = !ignoreTilesWhenCharging;
				Vector2 targetSpot = targetVec - projectile.Center;
				float targetDist = targetSpot.Length();
				targetSpot.Normalize();
				//Tries to get the minion in the sweet spot of 200 pixels away but the minion also charges so idk what good it does
				if (targetDist > 200f)
				{
					float speed = goToSpeed; //8
					targetSpot *= speed;
					projectile.velocity = (projectile.velocity * 40f + targetSpot) / 41f;
				}
				else
				{
					float speed = -goBackSpeed; //-4
					targetSpot *= speed;
					projectile.velocity = (projectile.velocity * 40f + targetSpot) / 41f; //41
				}
			}

			//Movement for idle or returning to the player
			else
			{
				//Ignore tiles so they don't get stuck everywhere like Optic Staff
				projectile.tileCollide = false;

				bool returningToPlayer = false;
				if (!returningToPlayer)
				{
					returningToPlayer = projectile.ai[0] == 1f;
				}

				//Player distance calculations
				Vector2 playerVec = player.Center - projectile.Center + returnOffset;
				float playerDist = playerVec.Length();

				//If the minion is actively returning, move faster
				float playerHomeSpeed = 6f;
				if (returningToPlayer)
				{
					playerHomeSpeed = 15f;
				}
				//Move somewhat faster if the player is kinda far~ish
				if (playerDist > 200f && playerHomeSpeed < 8f)
				{
					playerHomeSpeed = 8f;
				}
				//Return to normal if close enough to the player
				if (playerDist < safeDist && returningToPlayer && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
				{
					projectile.ai[0] = 0f;
					projectile.netUpdate = true;
				}
				//Teleport to the player if abnormally far
				if (playerDist > 2000f)
				{
					projectile.position.X = player.Center.X - (float)(projectile.width / 2);
					projectile.position.Y = player.Center.Y - (float)(projectile.height / 2);
					projectile.netUpdate = true;
				}
				//If more than 70 pixels away, move toward the player
				if (playerDist > 70f)
				{
					playerVec.Normalize();
					playerVec *= playerHomeSpeed;
					projectile.velocity = (projectile.velocity * 40f + playerVec) / 41f;
				}
				//Minions never stay still
				else if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
				{
					projectile.velocity.X = -0.15f;
					projectile.velocity.Y = -0.05f;
				}
			}

			//Increment attack counter randomly
			if (projectile.ai[1] > 0f)
			{
				projectile.ai[1] += (float)Main.rand.Next(1, 4);
			}
			//If high enough, prepare to attack
			if (projectile.ai[1] > chargeCounterMax)
			{
				projectile.ai[1] = 0f;
				projectile.netUpdate = true;
			}

			//Charge at an enemy if not on cooldown
			if (projectile.ai[0] == 0f)
			{
				if (projectile.ai[1] == 0f && foundTarget && maxDist < 500f)
				{
					projectile.ai[1] += 1f;
					if (Main.myPlayer == projectile.owner)
					{
						projectile.ai[0] = 2f;
						Vector2 targetPos = targetVec - projectile.Center;
						targetPos.Normalize();
						projectile.velocity = targetPos * chargeSpeed; //8
						projectile.netUpdate = true;
					}
				}
			}
		}

		public static void FloatingPetAI(this Projectile projectile, bool faceRight, float tiltFloat, bool lightPet = false)
		{
			Player player = Main.player[projectile.owner];

			//anti sticking movement as a failsafe
			float SAImovement = 0.05f;
			for (int k = 0; k < Main.maxProjectiles; k++)
			{
				Projectile otherProj = Main.projectile[k];
				// Short circuits to make the loop as fast as possible
				if (!otherProj.active || otherProj.owner != projectile.owner || !Main.projPet[otherProj.type] || k == projectile.whoAmI)
					continue;

				// If the other projectile is indeed another pet owned by the same player and they're too close, nudge them away.
				bool isPet = Main.projPet[otherProj.type];
				float taxicabDist = Math.Abs(projectile.position.X - otherProj.position.X) + Math.Abs(projectile.position.Y - otherProj.position.Y);
				if (isPet && taxicabDist < projectile.width)
				{
					if (projectile.position.X < otherProj.position.X)
						projectile.velocity.X -= SAImovement;
					else
						projectile.velocity.X += SAImovement;

					if (projectile.position.Y < otherProj.position.Y)
						projectile.velocity.Y -= SAImovement;
					else
						projectile.velocity.Y += SAImovement;
				}
			}

			float passiveMvtFloat = 0.5f;
			projectile.tileCollide = false;
			float range = 100f;
			Vector2 projPos = new Vector2(projectile.Center.X, projectile.Center.Y);
			float xDist = player.Center.X - projPos.X;
			float yDist = player.Center.Y - projPos.Y;
			yDist += Main.rand.NextFloat(-10, 20);
			xDist += Main.rand.NextFloat(-10, 20);
			//Light pets lead the player, normal pets trail the player
			xDist += 60f * (lightPet ? (float)player.direction : -(float)player.direction);
			yDist -= 60f;
			Vector2 playerVector = new Vector2(xDist, yDist);
			float playerDist = playerVector.Length();
			float returnSpeed = 18f;

			//If player is close enough, resume normal
			if (playerDist < range && player.velocity.Y == 0f &&
				projectile.position.Y + projectile.height <= player.position.Y + player.height &&
				!Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
			{
				if (projectile.velocity.Y < -6f)
				{
					projectile.velocity.Y = -6f;
				}
			}

			//Teleport to player if too far
			if (playerDist > 2000f)
			{
				projectile.position.X = player.Center.X - projectile.width / 2;
				projectile.position.Y = player.Center.Y - projectile.height / 2;
				projectile.netUpdate = true;
			}

			if (playerDist < 50f)
			{
				if (Math.Abs(projectile.velocity.X) > 2f || Math.Abs(projectile.velocity.Y) > 2f)
				{
					projectile.velocity *= 0.99f;
				}
				passiveMvtFloat = 0.01f;
			}
			else
			{
				if (playerDist < 100f)
				{
					passiveMvtFloat = 0.1f;
				}
				if (playerDist > 300f)
				{
					passiveMvtFloat = 1f;
				}
				playerDist = returnSpeed / playerDist;
				playerVector.X *= playerDist;
				playerVector.Y *= playerDist;
			}
			if (projectile.velocity.X < playerVector.X)
			{
				projectile.velocity.X += passiveMvtFloat;
				if (passiveMvtFloat > 0.05f && projectile.velocity.X < 0f)
				{
					projectile.velocity.X += passiveMvtFloat;
				}
			}
			if (projectile.velocity.X > playerVector.X)
			{
				projectile.velocity.X -= passiveMvtFloat;
				if (passiveMvtFloat > 0.05f && projectile.velocity.X > 0f)
				{
					projectile.velocity.X -= passiveMvtFloat;
				}
			}
			if (projectile.velocity.Y < playerVector.Y)
			{
				projectile.velocity.Y += passiveMvtFloat;
				if (passiveMvtFloat > 0.05f && projectile.velocity.Y < 0f)
				{
					projectile.velocity.Y += passiveMvtFloat * 2f;
				}
			}
			if (projectile.velocity.Y > playerVector.Y)
			{
				projectile.velocity.Y -= passiveMvtFloat;
				if (passiveMvtFloat > 0.05f && projectile.velocity.Y > 0f)
				{
					projectile.velocity.Y -= passiveMvtFloat * 2f;
				}
			}
			if (projectile.velocity.X >= 0.25f)
			{
				projectile.direction = faceRight ? 1 : -1;
			}
			else if (projectile.velocity.X < -0.25f)
			{
				projectile.direction = faceRight ? -1 : 1;
			}
			//Tilting and change directions
			projectile.spriteDirection = projectile.direction;
			projectile.rotation = projectile.velocity.X * tiltFloat;
		}

		public static void HealingProjectile(this Projectile projectile, int healing, int playerToHeal, float homingVelocity, float N, bool autoHomes = true, int timeCheck = 120)
		{
			int target = playerToHeal;
			Player player = Main.player[target];
			float homingSpeed = homingVelocity;
			if (player.lifeMagnet)
				homingSpeed *= 1.5f;

			Vector2 playerVector = player.Center - projectile.Center;
			float playerDist = playerVector.Length();
			if (playerDist < 50f && projectile.position.X < player.position.X + player.width && projectile.position.X + projectile.width > player.position.X && projectile.position.Y < player.position.Y + player.height && projectile.position.Y + projectile.height > player.position.Y)
			{
				if (projectile.owner == Main.myPlayer && !Main.player[Main.myPlayer].moonLeech)
				{
					int healAmt = healing;
					player.HealEffect(healAmt, false);
					player.statLife += healAmt;
					if (player.statLife > player.statLifeMax2)
					{
						player.statLife = player.statLifeMax2;
					}
					NetMessage.SendData(MessageID.SpiritHeal, -1, -1, null, target, healAmt, 0f, 0f, 0, 0, 0);
				}
				projectile.Kill();
			}
			if (autoHomes)
			{
				playerDist = homingSpeed / playerDist;
				playerVector.X *= playerDist;
				playerVector.Y *= playerDist;
				projectile.velocity.X = (projectile.velocity.X * N + playerVector.X) / (N + 1f);
				projectile.velocity.Y = (projectile.velocity.Y * N + playerVector.Y) / (N + 1f);
			}
			else if (player.lifeMagnet && projectile.timeLeft < timeCheck)
			{
				playerDist = homingVelocity / playerDist;
				playerVector.X *= playerDist;
				playerVector.Y *= playerDist;
				projectile.velocity.X = (projectile.velocity.X * N + playerVector.X) / (N + 1f);
				projectile.velocity.Y = (projectile.velocity.Y * N + playerVector.Y) / (N + 1f);
			}
		}

		public static bool DrawBeam(this Projectile projectile, float length, float width, Color lightColor, Texture2D texture = null)
		{
			if (texture is null)
				texture = Main.projectileTexture[projectile.type];

			float widthOffset = (float)(texture.Width - projectile.width) * 0.5f + (float)projectile.width * 0.5f;
			float heightOffset = (float)(projectile.height / 2);
			Vector2 origin = new Vector2(widthOffset, heightOffset);
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (projectile.spriteDirection == -1)
			{
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
			Rectangle roughScreenBounds = new Rectangle((int)Main.screenPosition.X - 500, (int)Main.screenPosition.Y - 500, Main.screenWidth + 1000, Main.screenHeight + 1000);
			if (projectile.getRect().Intersects(roughScreenBounds))
			{
				Vector2 drawPos = projectile.position - Main.screenPosition + origin;
				drawPos.Y += projectile.gfxOffY;
				float maxTrailPoints = length;
				float wide = width;
				if (projectile.ai[1] == 1f)
				{
					maxTrailPoints = (int)projectile.localAI[0];
				}
				for (int i = 1; i <= (int)projectile.localAI[0]; i++)
				{
					Vector2 offset = Vector2.Normalize(projectile.velocity) * (float)i * wide;
					Color color = projectile.GetAlpha(lightColor);
					color *= (maxTrailPoints - (float)i) / maxTrailPoints;
					color.A = 0;
					Main.spriteBatch.Draw(texture, drawPos - offset, null, color, projectile.rotation, origin, projectile.scale, spriteEffects, 0f);
				}
			}
			return false;
		}

		public static void ExplodeandDestroyTiles(Projectile projectile, int explosionRadius, bool checkExplosions, List<int> tilesToCheck, List<int> wallsToCheck)
		{
			int minTileX = (int)projectile.position.X / 16 - explosionRadius;
			int maxTileX = (int)projectile.position.X / 16 + explosionRadius;
			int minTileY = (int)projectile.position.Y / 16 - explosionRadius;
			int maxTileY = (int)projectile.position.Y / 16 + explosionRadius;
			if (minTileX < 0)
			{
				minTileX = 0;
			}
			if (maxTileX > Main.maxTilesX)
			{
				maxTileX = Main.maxTilesX;
			}
			if (minTileY < 0)
			{
				minTileY = 0;
			}
			if (maxTileY > Main.maxTilesY)
			{
				maxTileY = Main.maxTilesY;
			}

			bool canKillWalls = false;
			for (int x = minTileX; x <= maxTileX; x++)
			{
				for (int y = minTileY; y <= maxTileY; y++)
				{
					Vector2 explodeArea = new Vector2(Math.Abs(x - projectile.position.X / 16f), Math.Abs(y - projectile.position.Y / 16f));
					float distance = explodeArea.Length();
					if (distance < explosionRadius && Main.tile[x, y] != null && Main.tile[x, y].wall == WallID.None)
					{
						canKillWalls = true;
						break;
					}
				}
			}

			List<int> tileExcludeList = new List<int>()
			{
				TileID.DemonAltar,
				TileID.ElderCrystalStand
			};
			for (int i = 0; i < tilesToCheck.Count; ++i)
				tileExcludeList.Add(tilesToCheck[i]);
			List<int> wallExcludeList = new List<int>();
			for (int i = 0; i < wallsToCheck.Count; ++i)
				wallExcludeList.Add(wallsToCheck[i]);

			List<int> explosionCheckList = new List<int>()
			{
				TileID.DemonAltar,
				TileID.Cobalt,
				TileID.Mythril,
				TileID.Adamantite,
				TileID.Palladium,
				TileID.Orichalcum,
				TileID.Titanium,
				TileID.Chlorophyte,
				TileID.LihzahrdBrick,
				TileID.LihzahrdAltar,
				TileID.DesertFossil
			};
			CalamityUtils.AddWithCondition<int>(explosionCheckList, TileID.Hellstone, !Main.hardMode);

			for (int i = minTileX; i <= maxTileX; i++)
			{
				for (int j = minTileY; j <= maxTileY; j++)
				{
					Tile tile = Main.tile[i, j];
					bool t = 1 == 1; bool f = 1 == 2;

					Vector2 explodeArea = new Vector2(Math.Abs(i - projectile.position.X / 16f), Math.Abs(j - projectile.position.Y / 16f));
					float distance = explodeArea.Length();
					if (distance < explosionRadius)
					{
						bool canKillTile = true;
						if (tile != null && tile.active())
						{
							if (checkExplosions)
							{
								if (Main.tileDungeon[tile.type] || explosionCheckList.Contains(tile.type))
								{
									canKillTile = false;
								}
								if (!TileLoader.CanExplode(i, j))
								{
									canKillTile = false;
								}
							}
							if (Main.tileContainer[tile.type])
								canKillTile = false;
							if (!TileLoader.CanKillTile(i, j, tile.type, ref t) || !TileLoader.CanKillTile(i, j, tile.type, ref f))
								canKillTile = false;
							if (tileExcludeList.Contains(tile.type))
								canKillTile = false;

							if (canKillTile)
							{
								WorldGen.KillTile(i, j, false, false, false);
								if (!tile.active() && Main.netMode != NetmodeID.SinglePlayer)
								{
									NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, i, j, 0f, 0, 0, 0);
								}
							}
						}
						if (canKillTile)
						{
							for (int x = i - 1; x <= i + 1; x++)
							{
								for (int y = j - 1; y <= j + 1; y++)
								{
									bool canExplode = true;
									if (checkExplosions)
										canExplode = WallLoader.CanExplode(x, y, Main.tile[x, y].wall);
									if (wallExcludeList.Any() && wallExcludeList.Contains(Main.tile[x, y].wall))
										canKillWalls = false;
									if (Main.tile[x, y] != null && Main.tile[x, y].wall > WallID.None && canKillWalls && canExplode)
									{
										WorldGen.KillWall(x, y, false);
										if (Main.tile[x, y].wall == WallID.None && Main.netMode != NetmodeID.SinglePlayer)
										{
											NetMessage.SendData(MessageID.TileChange, -1, -1, null, 2, x, y, 0f, 0, 0, 0);
										}
									}
								}
							}
						}
					}
				}
			}
		}
		#endregion

		#region Tile Utilities

		public static string GetMapChestName(string baseName, int x, int y)
		{
			// Bounds check.
			if (!WorldGen.InWorld(x, y, 2))
				return baseName;

			// Tile null check.
			Tile tile = Main.tile[x, y];
			if (tile is null)
				return baseName;

			int left = x;
			int top = y;
			if (tile.frameX % 36 != 0)
				left--;
			if (tile.frameY != 0)
				top--;

			int chest = Chest.FindChest(left, top);

			// Valid chest index check.
			if (chest < 0)
				return baseName;

			string name = baseName;

			// Concatenate the chest's custom name if it has one.
			if (!string.IsNullOrEmpty(Main.chest[chest].name))
				name += $": {Main.chest[chest].name}";

			return name;
		}

		public static void SafeSquareTileFrame(int x, int y, bool resetFrame = true)
		{
			if (Main.tile[x, y] is null)
				return;

			for (int xIter = x - 1; xIter <= x + 1; ++xIter)
			{
				if (xIter < 0 || xIter >= Main.maxTilesX)
					continue;
				for (int yIter = y - 1; yIter <= y + 1; yIter++)
				{
					if (yIter < 0 || yIter >= Main.maxTilesY)
						continue;
					if (xIter == x && yIter == y)
					{
						WorldGen.TileFrame(x, y, resetFrame, false);
					}
					else
					{
						WorldGen.TileFrame(xIter, yIter, false, false);
					}
				}
			}
		}

		public static void LightHitWire(int type, int i, int j, int tileX, int tileY)
		{
			int x = i - Main.tile[i, j].frameX / 18 % tileX;
			int y = j - Main.tile[i, j].frameY / 18 % tileY;
			for (int l = x; l < x + tileX; l++)
			{
				for (int m = y; m < y + tileY; m++)
				{
					if (Main.tile[l, m] == null)
					{
						Main.tile[l, m] = new Tile();
					}
					if (Main.tile[l, m].active() && Main.tile[l, m].type == type)
					{
						if (Main.tile[l, m].frameX < (18 * tileX))
						{
							Main.tile[l, m].frameX += (short)(18 * tileX);
						}
						else
						{
							Main.tile[l, m].frameX -= (short)(18 * tileX);
						}
					}
				}
			}
			if (Wiring.running)
			{
				for (int k = 0; k < tileX; k++)
				{
					for (int l = 0; l < tileY; l++)
					{
						Wiring.SkipWire(x + k, y + l);
					}
				}
			}
		}

		public static void DrawFlameEffect(Texture2D flameTexture, int i, int j, int offsetX = 0, int offsetY = 0)
		{
			Tile tile = Main.tile[i, j];
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

			int width = 16;
			int height = 16;
			int yOffset = TileObjectData.GetTileData(tile).DrawYOffset;

			ulong num190 = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (long)(uint)i);

			for (int c = 0; c < 7; c++)
			{
				float shakeX = Utils.RandomInt(ref num190, -10, 11) * 0.15f;
				float shakeY = Utils.RandomInt(ref num190, -10, 1) * 0.35f;
				Main.spriteBatch.Draw(flameTexture, new Vector2(i * 16 - (int)Main.screenPosition.X - (width - 16f) / 2f + shakeX, j * 16 - (int)Main.screenPosition.Y + shakeY + yOffset) + zero, new Rectangle(tile.frameX + offsetX, tile.frameY + offsetY, width, height), new Color(100, 100, 100, 0), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
			}
		}

		public static void DrawStaticFlameEffect(Texture2D flameTexture, int i, int j, int offsetX = 0, int offsetY = 0)
		{
			int xPos = Main.tile[i, j].frameX;
			int yPos = Main.tile[i, j].frameY;
			Color drawColour = new Color(100, 100, 100, 0);
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
			Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + zero;
			for (int x = -1; x < 2; x++)
			{
				for (int y = -1; y < 2; y++)
				{
					Vector2 flameOffset = new Vector2(x, y).SafeNormalize(Vector2.Zero);
					flameOffset *= 1.5f;
					Main.spriteBatch.Draw(flameTexture, drawOffset + flameOffset, new Rectangle?(new Rectangle(xPos + offsetX, yPos + offsetY, 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
				}
			}
		}

		public static void DrawFlameSparks(int dustType, int rarity, int i, int j)
		{
			Tile tile = Main.tile[i, j];
			if (!Main.gamePaused && Main.instance.IsActive && (!Lighting.UpdateEveryFrame || Main.rand.NextBool(4)))
			{
				if (Main.rand.NextBool(rarity))
				{
					int dust = Dust.NewDust(new Vector2(i * 16 + 4, j * 16 + 2), 4, 4, dustType, 0f, 0f, 100, default(Color), 1f);
					if (Main.rand.Next(3) != 0)
					{
						Main.dust[dust].noGravity = true;
					}
					Main.dust[dust].velocity *= 0.3f;
					Main.dust[dust].velocity.Y = Main.dust[dust].velocity.Y - 1.5f;
				}
			}
		}

		public static void DrawItemFlame(Texture2D flameTexture, Item item)
		{
			int width = flameTexture.Width;
			int height = flameTexture.Height;
			for (int c = 0; c < 7; c++)
			{
				float shakeX = Main.rand.Next(-10, 11) * 0.15f;
				float shakeY = Main.rand.Next(-10, 1) * 0.35f;
				Main.spriteBatch.Draw(flameTexture, new Vector2(item.position.X - Main.screenPosition.X + item.width * 0.5f + shakeX, item.position.Y - Main.screenPosition.Y + item.height - flameTexture.Height * 0.5f + 2f + shakeY), new Rectangle(0, 0, width, height), new Color(100, 100, 100, 0), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
			}
		}
		public static Tile ParanoidTileRetrieval(int x, int y)
		{
			if (!WorldGen.InWorld(x, y))
				return new Tile();
			Tile tile = Main.tile[x, y];
			if (tile is null)
			{
				tile = new Tile();
				Main.tile[x, y] = tile;
			}
			return tile;
		}
		public static bool TileSelectionSolid(int x, int y, int width, int height)
		{
			for (int i = x; i != x + width; i += Math.Sign(width))
			{
				for (int j = y; y != y + height; j += Math.Sign(height))
				{
					if (!WorldGen.InWorld(i, j))
						return false;
					if (!WorldGen.SolidTile(Framing.GetTileSafely(i, j)))
						return false;
				}
			}
			return true;
		}
		public static bool TileSelectionSolidSquare(int x, int y, int width, int height)
		{
			for (int i = x - width; i != x + width; i += Math.Sign(width))
			{
				for (int j = y - height; y != y + height; j += Math.Sign(height))
				{
					if (!WorldGen.InWorld(i, j))
						return false;
					if (!WorldGen.SolidTile(Framing.GetTileSafely(i, j)))
						return false;
				}
			}
			return true;
		}
		public static bool TileActiveAndOfType(int x, int y, int type)
		{
			return ParanoidTileRetrieval(x, y).active() && ParanoidTileRetrieval(x, y).type == type;
		}

		#region Tile Merge Utilities
		/// <summary>
		/// Sets the mergeability state of two tiles. By default, enables tile merging.
		/// </summary>
		/// <param name="type1">The first tile type which should merge (or not).</param>
		/// <param name="type2">The second tile type which should merge (or not).</param>
		/// <param name="merge">The mergeability state of the tiles. Defaults to true if omitted.</param>
		public static void SetMerge(int type1, int type2, bool merge = true)
		{
			if (type1 != type2)
			{
				Main.tileMerge[type1][type2] = merge;
				Main.tileMerge[type2][type1] = merge;
			}
		}

		/// <summary>
		/// Makes the first tile type argument merge with all the other tile type arguments. Also accepts arrays.
		/// </summary>
		/// <param name="myType">The tile whose merging properties will be set.</param>
		/// <param name="otherTypes">Every tile that should be merged with.</param>
		public static void MergeWithSet(int myType, params int[] otherTypes)
		{
			for (int i = 0; i < otherTypes.Length; ++i)
				SetMerge(myType, otherTypes[i]);
		}

		/// <summary>
		/// Makes the specified tile merge with the most common types of tiles found in world generation.<br></br>
		/// Notably excludes Ice.
		/// </summary>
		/// <param name="type">The tile whose merging properties will be set.</param>
		public static void MergeWithGeneral(int type) => MergeWithSet(type, new int[] {
			// Soils
			TileID.Dirt,
			TileID.Mud,
			TileID.ClayBlock,
			// Stones
			TileID.Stone,
			TileID.Ebonstone,
			TileID.Crimstone,
			TileID.Pearlstone,
			// Sands
			TileID.Sand,
			TileID.Ebonsand,
			TileID.Crimsand,
			TileID.Pearlsand,
			// Snows
			TileID.SnowBlock,
			// Calamity Tiles
			ModContent.TileType<AstralDirt>(),
			ModContent.TileType<AstralClay>(),
			ModContent.TileType<AstralStone>(),
			ModContent.TileType<AstralSand>(),
			ModContent.TileType<AstralSnow>(),
			ModContent.TileType<Navystone>(),
			ModContent.TileType<EutrophicSand>(),
			ModContent.TileType<AbyssGravel>(),
			ModContent.TileType<Voidstone>(),
		});

		/// <summary>
		/// Makes the specified tile merge with all ores, vanilla and Calamity. Particularly useful for stone blocks.
		/// </summary>
		/// <param name="type">The tile whose merging properties will be set.</param>
		public static void MergeWithOres(int type) => MergeWithSet(type, new int[] {
			// Vanilla Ores
			TileID.Copper,
			TileID.Tin,
			TileID.Iron,
			TileID.Lead,
			TileID.Silver,
			TileID.Tungsten,
			TileID.Gold,
			TileID.Platinum,
			TileID.Demonite,
			TileID.Crimtane,
			TileID.Cobalt,
			TileID.Palladium,
			TileID.Mythril,
			TileID.Orichalcum,
			TileID.Adamantite,
			TileID.Titanium,
			TileID.LunarOre,
			// Calamity Ores
			ModContent.TileType<AerialiteOre>(),
			ModContent.TileType<CryonicOre>(),
			ModContent.TileType<PerennialOre>(),
			ModContent.TileType<CharredOre>(),
			ModContent.TileType<ChaoticOre>(),
			ModContent.TileType<AstralOre>(),
			ModContent.TileType<UelibloomOre>(),
			ModContent.TileType<AuricOre>(),
		});

		/// <summary>
		/// Makes the specified tile merge with all types of desert tiles, including the Calamity Sunken Sea.
		/// </summary>
		/// <param name="type">The tile whose merging properties will be set.</param>
		public static void MergeWithDesert(int type) => MergeWithSet(type, new int[] {
			// Sands
			TileID.Sand,
			TileID.Ebonsand,
			TileID.Crimsand,
			TileID.Pearlsand,
			// Hardened Sands
			TileID.HardenedSand,
			TileID.CorruptHardenedSand,
			TileID.CrimsonHardenedSand,
			TileID.HallowHardenedSand,
			// Sandstones
			TileID.Sandstone,
			TileID.CorruptSandstone,
			TileID.CrimsonSandstone,
			TileID.HallowSandstone,
			// Miscellaneous Desert Tiles
			TileID.FossilOre,
			TileID.DesertFossil,
			// Astral Desert
			ModContent.TileType<AstralSand>(),
			ModContent.TileType<HardenedAstralSand>(),
			ModContent.TileType<AstralSandstone>(),
			ModContent.TileType<AstralFossil>(),
			// Sunken Sea
			ModContent.TileType<EutrophicSand>(),
			ModContent.TileType<Navystone>(),
			ModContent.TileType<SeaPrism>(),
		});

		/// <summary>
		/// Makes the specified tile merge with all types of snow and ice tiles.
		/// </summary>
		/// <param name="type">The tile whose merging properties will be set.</param>
		public static void MergeWithSnow(int type) => MergeWithSet(type, new int[] {
			// Snows
			TileID.SnowBlock,
			// Ices
			TileID.IceBlock,
			TileID.CorruptIce,
			TileID.FleshIce,
			TileID.HallowedIce,
			// Astral Snow
			ModContent.TileType<AstralIce>(),
			ModContent.TileType<AstralSnow>(),
			ModContent.TileType<AstralSilt>(),
		});

		/// <summary>
		/// Makes the specified tile merge with all tiles which generate in hell. Does not include Charred Ore.
		/// </summary>
		/// <param name="type">The tile whose merging properties will be set.</param>
		public static void MergeWithHell(int type) => MergeWithSet(type, new int[] {
			TileID.Ash,
			TileID.Hellstone,
			TileID.ObsidianBrick,
			TileID.HellstoneBrick,
			ModContent.TileType<BrimstoneSlag>(),
		});

		/// <summary>
		/// Makes the specified tile merge with all tiles which generate in the Abyss or the Sulphurous Sea. Includes Chaotic Ore.
		/// </summary>
		/// <param name="type">The tile whose merging properties will be set.</param>
		public static void MergeWithAbyss(int type) => MergeWithSet(type, new int[] {
			// Sulphurous Sea
			ModContent.TileType<SulphurousSand>(),
			ModContent.TileType<SulphurousSandstone>(),
			// Abyss
			ModContent.TileType<AbyssGravel>(),
			ModContent.TileType<Voidstone>(),
			ModContent.TileType<PlantyMush>(),
			ModContent.TileType<Tenebris>(),
			ModContent.TileType<ChaoticOre>(),
		});

		/// <summary>
		/// Makes the tile merge with all the tile types that generate within various types of astral tiles
		/// </summary>
		/// <param name="type"></param>
		public static void MergeAstralTiles(int type)
		{
			//Astral
			SetMerge(type, ModContent.TileType<AstralDirt>());
			SetMerge(type, ModContent.TileType<AstralStone>());
			SetMerge(type, ModContent.TileType<AstralMonolith>());
			SetMerge(type, ModContent.TileType<AstralClay>());
			//Astral Desert
			SetMerge(type, ModContent.TileType<AstralSand>());
			SetMerge(type, ModContent.TileType<HardenedAstralSand>());
			SetMerge(type, ModContent.TileType<AstralSandstone>());
			SetMerge(type, ModContent.TileType<AstralFossil>());
			//Astral Snow
			SetMerge(type, ModContent.TileType<AstralIce>());
			SetMerge(type, ModContent.TileType<AstralSnow>());
		}

		/// <summary>
		/// Makes the tile merge with all the decorative 'smooth' tiles
		/// </summary>
		/// <param name="type"></param>
		public static void MergeSmoothTiles(int type)
		{
			//Vanilla
			SetMerge(type, TileID.MarbleBlock);
			SetMerge(type, TileID.GraniteBlock);
			//Calam
			SetMerge(type, ModContent.TileType<SmoothNavystone>());
			SetMerge(type, ModContent.TileType<SmoothBrimstoneSlag>());
			SetMerge(type, ModContent.TileType<SmoothAbyssGravel>());
			SetMerge(type, ModContent.TileType<SmoothVoidstone>());
		}

		/// <summary>
		/// Makes the tile merge with other mergable decorative tiles
		/// </summary>
		/// <param name="type"></param>
		public static void MergeDecorativeTiles(int type)
		{
			//Vanilla decor
			Main.tileBrick[type] = true;
			//Calam
			SetMerge(type, ModContent.TileType<CryonicBrick>());
			SetMerge(type, ModContent.TileType<PerennialBrick>());
			SetMerge(type, ModContent.TileType<UelibloomBrick>());
			SetMerge(type, ModContent.TileType<OccultStone>());
			SetMerge(type, ModContent.TileType<ProfanedSlab>());
			SetMerge(type, ModContent.TileType<RunicProfanedBrick>());
			SetMerge(type, ModContent.TileType<AshenSlab>());
			SetMerge(type, ModContent.TileType<VoidstoneSlab>());
		}
		#endregion

		#region Furniture Interaction
		public static void RightClickBreak(int i, int j)
		{
			if (Main.tile[i, j] != null && Main.tile[i, j].active())
			{
				WorldGen.KillTile(i, j, false, false, false);
				if (!Main.tile[i, j].active() && Main.netMode != NetmodeID.SinglePlayer)
				{
					NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, (float)i, (float)j, 0f, 0, 0, 0);
				}
			}
		}

		public static bool BedRightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			int spawnX = i - tile.frameX / 18;
			int spawnY = j + 2;
			spawnX += tile.frameX >= 72 ? 5 : 2;
			if (tile.frameY % 38 != 0)
			{
				spawnY--;
			}
			player.FindSpawn();
			if (player.SpawnX == spawnX && player.SpawnY == spawnY)
			{
				player.RemoveSpawn();
				Main.NewText("Spawn point removed!", 255, 240, 20, false);
			}
			else if (Player.CheckSpawn(spawnX, spawnY))
			{
				player.ChangeSpawn(spawnX, spawnY);
				Main.NewText("Spawn point set!", 255, 240, 20, false);
			}
			return true;
		}

		public static bool ChestRightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			Main.mouseRightRelease = false;
			int left = i;
			int top = j;
			if (tile.frameX % 36 != 0)
			{
				left--;
			}
			if (tile.frameY != 0)
			{
				top--;
			}
			if (player.sign >= 0)
			{
				Main.PlaySound(SoundID.MenuClose);
				player.sign = -1;
				Main.editSign = false;
				Main.npcChatText = "";
			}
			if (Main.editChest)
			{
				Main.PlaySound(SoundID.MenuTick);
				Main.editChest = false;
				Main.npcChatText = "";
			}
			if (player.editedChestName)
			{
				NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f, 0f, 0f, 0, 0, 0);
				player.editedChestName = false;
			}

			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				if (left == player.chestX && top == player.chestY && player.chest >= 0)
				{
					player.chest = -1;
					Recipe.FindRecipes();
					Main.PlaySound(SoundID.MenuClose);
				}
				else
				{
					NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, (float)top, 0f, 0f, 0, 0, 0);
					Main.stackSplit = 600;
				}
			}
			else
			{
				int chest = Chest.FindChest(left, top);
				if (chest >= 0)
				{
					Main.stackSplit = 600;
					if (chest == player.chest)
					{
						player.chest = -1;
						Main.PlaySound(SoundID.MenuClose);
					}
					else
					{
						player.chest = chest;
						Main.playerInventory = true;
						Main.recBigList = false;
						player.chestX = left;
						player.chestY = top;
						Main.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
					}
					Recipe.FindRecipes();
				}
			}
			return true;
		}

		public static void ChestMouseOver<T>(string chestName, int i, int j) where T : ModItem
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			int left = i;
			int top = j;
			if (tile.frameX % 36 != 0)
			{
				left--;
			}
			if (tile.frameY != 0)
			{
				top--;
			}
			int chest = Chest.FindChest(left, top);
			player.showItemIcon2 = -1;
			if (chest < 0)
			{
				player.showItemIconText = Language.GetTextValue("LegacyChestType.0");
			}
			else
			{
				player.showItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : chestName;
				if (player.showItemIconText == chestName)
				{
					player.showItemIcon2 = ModContent.ItemType<T>();
					player.showItemIconText = "";
				}
			}
			player.noThrow = 2;
			player.showItemIcon = true;
		}

		public static void ChestMouseFar<T>(string name, int i, int j) where T : ModItem
		{
			ChestMouseOver<T>(name, i, j);
			Player player = Main.LocalPlayer;
			if (player.showItemIconText == "")
			{
				player.showItemIcon = false;
				player.showItemIcon2 = 0;
			}
		}

		public static bool ClockRightClick()
		{
			string text = "AM";

			// Get Terraria's current strange time variable
			double time = Main.time;

			// Correct for night time (which for some reason isn't just a different number) by adding 54000.
			if (!Main.dayTime)
				time += 54000D;

			// Divide by seconds in an hour
			time /= 3600D;

			// Terraria night starts at 7:30 PM, so offset accordingly
			time -= 19.5;

			// Offset time to ensure it is not negative, then change to PM if necessary
			if (time < 0D)
				time += 24D;
			if (time >= 12D)
				text = "PM";

			// Get the decimal (smaller than hours, so minutes) component of time.
			int intTime = (int)time;
			double deltaTime = time - intTime;

			// Convert decimal time into an exact number of minutes.
			deltaTime = (int)(deltaTime * 60D);

			string minuteText = deltaTime.ToString();

			// Ensure minutes has a leading zero
			if (deltaTime < 10D)
				minuteText = "0" + minuteText;

			// Convert from 24 to 12 hour time (PM already handled earlier)
			if (intTime > 12)
				intTime -= 12;
			// 12 AM is actually hour zero in 24 hour time
			if (intTime == 0)
				intTime = 12;

			// Create an overall time readout and send it to chat
			var newText = string.Concat("Time: ", intTime, ":", minuteText, " ", text);
			Main.NewText(newText, 255, 240, 20);
			return true;
		}

		public static bool DresserRightClick()
		{
			Player player = Main.LocalPlayer;
			if (Main.tile[Player.tileTargetX, Player.tileTargetY].frameY == 0)
			{
				Main.CancelClothesWindow(true);

				int left = (int)(Main.tile[Player.tileTargetX, Player.tileTargetY].frameX / 18);
				left %= 3;
				left = Player.tileTargetX - left;
				int top = Player.tileTargetY - (int)(Main.tile[Player.tileTargetX, Player.tileTargetY].frameY / 18);
				if (player.sign > -1)
				{
					Main.PlaySound(SoundID.MenuClose);
					player.sign = -1;
					Main.editSign = false;
					Main.npcChatText = string.Empty;
				}
				if (Main.editChest)
				{
					Main.PlaySound(SoundID.MenuTick);
					Main.editChest = false;
					Main.npcChatText = string.Empty;
				}
				if (player.editedChestName)
				{
					NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f, 0f, 0f, 0, 0, 0);
					player.editedChestName = false;
				}
				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					if (left == player.chestX && top == player.chestY && player.chest != -1)
					{
						player.chest = -1;
						Recipe.FindRecipes();
						Main.PlaySound(SoundID.MenuClose);
					}
					else
					{
						NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, (float)top, 0f, 0f, 0, 0, 0);
						Main.stackSplit = 600;
					}
					return true;
				}
				else
				{
					player.flyingPigChest = -1;
					int num213 = Chest.FindChest(left, top);
					if (num213 != -1)
					{
						Main.stackSplit = 600;
						if (num213 == player.chest)
						{
							player.chest = -1;
							Recipe.FindRecipes();
							Main.PlaySound(SoundID.MenuClose);
						}
						else if (num213 != player.chest && player.chest == -1)
						{
							player.chest = num213;
							Main.playerInventory = true;
							Main.recBigList = false;
							Main.PlaySound(SoundID.MenuOpen);
							player.chestX = left;
							player.chestY = top;
						}
						else
						{
							player.chest = num213;
							Main.playerInventory = true;
							Main.recBigList = false;
							Main.PlaySound(SoundID.MenuTick);
							player.chestX = left;
							player.chestY = top;
						}
						Recipe.FindRecipes();
						return true;
					}
				}
			}
			else
			{
				Main.playerInventory = false;
				player.chest = -1;
				Recipe.FindRecipes();
				Main.dresserX = Player.tileTargetX;
				Main.dresserY = Player.tileTargetY;
				Main.OpenClothesWindow();
				return true;
			}

			return false;
		}

		public static void DresserMouseFar<T>(string chestName) where T : ModItem
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
			int left = Player.tileTargetX;
			int top = Player.tileTargetY;
			left -= (int)(tile.frameX % 54 / 18);
			if (tile.frameY % 36 != 0)
			{
				top--;
			}
			int chestIndex = Chest.FindChest(left, top);
			player.showItemIcon2 = -1;
			if (chestIndex < 0)
			{
				player.showItemIconText = Language.GetTextValue("LegacyDresserType.0");
			}
			else
			{
				if (Main.chest[chestIndex].name != "")
				{
					player.showItemIconText = Main.chest[chestIndex].name;
				}
				else
				{
					player.showItemIconText = chestName;
				}
				if (player.showItemIconText == chestName)
				{
					player.showItemIcon2 = ModContent.ItemType<T>();
					player.showItemIconText = "";
				}
			}
			player.noThrow = 2;
			player.showItemIcon = true;
			if (player.showItemIconText == "")
			{
				player.showItemIcon = false;
				player.showItemIcon2 = 0;
			}
		}

		public static void DresserMouseOver<T>(string chestName) where T : ModItem
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
			int left = Player.tileTargetX;
			int top = Player.tileTargetY;
			left -= (int)(tile.frameX % 54 / 18);
			if (tile.frameY % 36 != 0)
			{
				top--;
			}
			int chestIndex = Chest.FindChest(left, top);
			player.showItemIcon2 = -1;
			if (chestIndex < 0)
			{
				player.showItemIconText = Language.GetTextValue("LegacyDresserType.0");
			}
			else
			{
				if (Main.chest[chestIndex].name != "")
				{
					player.showItemIconText = Main.chest[chestIndex].name;
				}
				else
				{
					player.showItemIconText = chestName;
				}
				if (player.showItemIconText == chestName)
				{
					player.showItemIcon2 = ModContent.ItemType<T>();
					player.showItemIconText = "";
				}
			}
			player.noThrow = 2;
			player.showItemIcon = true;
			if (Main.tile[Player.tileTargetX, Player.tileTargetY].frameY > 0)
			{
				player.showItemIcon2 = ItemID.FamiliarShirt;
			}
		}

		public static bool LockedChestRightClick(bool isLocked, int left, int top, int i, int j)
		{
			Player player = Main.LocalPlayer;

			// If the player right clicked the chest while editing a sign, finish that up
			if (player.sign >= 0)
			{
				Main.PlaySound(SoundID.MenuClose);
				player.sign = -1;
				Main.editSign = false;
				Main.npcChatText = "";
			}

			// If the player right clicked the chest while editing a chest, finish that up
			if (Main.editChest)
			{
				Main.PlaySound(SoundID.MenuTick);
				Main.editChest = false;
				Main.npcChatText = "";
			}

			// If the player right clicked the chest after changing another chest's name, finish that up
			if (player.editedChestName)
			{
				NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f, 0f, 0f, 0, 0, 0);
				player.editedChestName = false;
			}
			if (Main.netMode == NetmodeID.MultiplayerClient && !isLocked)
			{
				// Right clicking the chest you currently have open closes it. This counts as interaction.
				if (left == player.chestX && top == player.chestY && player.chest >= 0)
				{
					player.chest = -1;
					Recipe.FindRecipes();
					Main.PlaySound(SoundID.MenuClose);
				}

				// Right clicking this chest opens it if it's not already open. This counts as interaction.
				else
				{
					NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, (float)top, 0f, 0f, 0, 0, 0);
					Main.stackSplit = 600;
				}
				return true;
			}

			else
			{
				if (isLocked)
				{
					// If you right click the locked chest and you can unlock it, it unlocks itself but does not open. This counts as interaction.
					if (Chest.Unlock(left, top))
					{
						if (Main.netMode == NetmodeID.MultiplayerClient)
						{
							NetMessage.SendData(MessageID.Unlock, -1, -1, null, player.whoAmI, 1f, (float)left, (float)top);
						}
						return true;
					}
				}
				else
				{
					int chest = Chest.FindChest(left, top);
					if (chest >= 0)
					{
						Main.stackSplit = 600;

						// If you right click the same chest you already have open, it closes. This counts as interaction.
						if (chest == player.chest)
						{
							player.chest = -1;
							Main.PlaySound(SoundID.MenuClose);
						}

						// If you right click this chest when you have a different chest selected, that one closes and this one opens. This counts as interaction.
						else
						{
							player.chest = chest;
							Main.playerInventory = true;
							Main.recBigList = false;
							player.chestX = left;
							player.chestY = top;
							Main.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
						}

						Recipe.FindRecipes();
						return true;
					}
				}
			}

			// This only occurs when the chest is locked and cannot be unlocked. You did not interact with the chest.
			return false;
		}

		public static void LockedChestMouseOver<K, C>(string chestName, int i, int j)
			where K : ModItem where C : ModItem
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			int left = i;
			int top = j;
			if (tile.frameX % 36 != 0)
			{
				left--;
			}
			if (tile.frameY != 0)
			{
				top--;
			}
			int chest = Chest.FindChest(left, top);
			player.showItemIcon2 = -1;
			if (chest < 0)
			{
				player.showItemIconText = Language.GetTextValue("LegacyChestType.0");
			}
			else
			{
				player.showItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : chestName;
				if (player.showItemIconText == chestName)
				{
					player.showItemIcon2 = ModContent.ItemType<C>();
					if (Main.tile[left, top].frameX / 36 == 1)
						player.showItemIcon2 = ModContent.ItemType<K>();
					player.showItemIconText = "";
				}
			}
			player.noThrow = 2;
			player.showItemIcon = true;
		}

		public static void LockedChestMouseOverFar<K, C>(string chestName, int i, int j)
			where K : ModItem where C : ModItem
		{
			LockedChestMouseOver<K, C>(chestName, i, j);
			Player player = Main.LocalPlayer;
			if (player.showItemIconText == "")
			{
				player.showItemIcon = false;
				player.showItemIcon2 = 0;
			}
		}
		#endregion

		#region Furniture SetDefaults
		/// <summary>
		/// Extension which initializes a ModTile to be a bathtub.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpBathtub(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileLighted[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;
			TileObjectData.newTile.Width = 4;
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, 4, 0);
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(mt.Type);

			// All bathtubs count as tables.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a bed.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpBed(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;
			TileID.Sets.HasOutlines[mt.Type] = true;
			TileObjectData.newTile.Width = 4;
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, 4, 0);
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(mt.Type);

			// All beds count as chairs.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a bookcase.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		/// <param name="solidTop">Whether this tile is supposed to have a solid top. Defaults to true.</param>
		internal static void SetUpBookcase(this ModTile mt, bool lavaImmune = false, bool solidTop = true)
		{
			Main.tileSolidTop[mt.Type] = solidTop;
			Main.tileLighted[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileTable[mt.Type] = solidTop;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.addTile(mt.Type);

			// All bookcases count as tables.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a candelabra.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpCandelabra(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileLighted[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = true;
			Main.tileWaterDeath[mt.Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.addTile(mt.Type);

			// All candelabras count as light sources.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a candle.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpCandle(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileLighted[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
			TileObjectData.newTile.CoordinateHeights = new int[] { 20 };
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.newTile.DrawYOffset = -4;
			TileObjectData.addTile(mt.Type);

			// All candles count as light sources.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a chair.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpChair(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileNoAttach[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newTile.StyleWrapLimit = 2;
			TileObjectData.newTile.StyleMultiplier = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(mt.Type);

			// As you could probably guess, all chairs count as chairs.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a chandelier.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpChandelier(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileLighted[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileNoAttach[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;
			TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.Origin = new Point16(1, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.addTile(mt.Type);

			// All chandeliers count as light sources.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a chest.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		internal static void SetUpChest(this ModTile mt, bool offset = false)
		{
			Main.tileSpelunker[mt.Type] = true;
			Main.tileContainer[mt.Type] = true;
			Main.tileShine2[mt.Type] = true;
			Main.tileShine[mt.Type] = 1200;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileNoAttach[mt.Type] = true;
			Main.tileValue[mt.Type] = 500;
			TileID.Sets.HasOutlines[mt.Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			if (offset)
				TileObjectData.newTile.DrawYOffset = 4;
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.newTile.HookCheck = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.FindEmptyChest), -1, 0, true);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.AfterPlacement_Hook), -1, 0, false);
			TileObjectData.newTile.AnchorInvalidTiles = new int[] { 127 };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(mt.Type);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a clock.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpClock(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileNoAttach[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			TileID.Sets.HasOutlines[mt.Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 5;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16,
				16,
				16
			};
			TileObjectData.newTile.Origin = new Point16(0, 4);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.addTile(mt.Type);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a closed door.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpDoorClosed(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileBlockLight[mt.Type] = true;
			Main.tileSolid[mt.Type] = true;
			Main.tileNoAttach[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;
			TileID.Sets.NotReallySolid[mt.Type] = true;
			TileID.Sets.DrawsWalls[mt.Type] = true;
			TileID.Sets.HasOutlines[mt.Type] = true;
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(0, 0);
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(0, 1);
			TileObjectData.addAlternate(0);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(0, 2);
			TileObjectData.addAlternate(0);
			TileObjectData.addTile(mt.Type);

			// As you could probably guess, all closed doors count as doors.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be an open door.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpDoorOpen(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileSolid[mt.Type] = false;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;
			Main.tileNoSunLight[mt.Type] = true;
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(0, 0);
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 0);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.StyleMultiplier = 2;
			TileObjectData.newTile.StyleWrapLimit = 2;
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(0, 1);
			TileObjectData.addAlternate(0);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(0, 2);
			TileObjectData.addAlternate(0);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(1, 0);
			TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
			TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.addAlternate(1);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(1, 1);
			TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
			TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.addAlternate(1);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(1, 2);
			TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
			TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(mt.Type);
			TileID.Sets.HousingWalls[mt.Type] = true;
			TileID.Sets.HasOutlines[mt.Type] = true;

			// As you could probably guess, all open doors count as doors.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a dresser.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		internal static void SetUpDresser(this ModTile mt)
		{
			Main.tileSolidTop[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileNoAttach[mt.Type] = true;
			Main.tileTable[mt.Type] = true;
			Main.tileContainer[mt.Type] = true;
			Main.tileWaterDeath[mt.Type] = false;
			Main.tileLavaDeath[mt.Type] = false;
			TileID.Sets.HasOutlines[mt.Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
			TileObjectData.newTile.HookCheck = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.FindEmptyChest), -1, 0, true);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.AfterPlacement_Hook), -1, 0, false);
			TileObjectData.newTile.AnchorInvalidTiles = new int[] { 127 };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(mt.Type);

			// All dressers count as tables.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a floor lamp.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpLamp(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileLighted[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1xX);
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.addTile(mt.Type);

			// All floor lamps count as light sources.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a hanging lantern.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpLantern(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileLighted[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.addTile(mt.Type);

			// All hanging lanterns count as light sources.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a piano.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpPiano(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileLighted[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.addTile(mt.Type);

			// All pianos count as tables.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a platform.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpPlatform(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileLighted[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileSolidTop[mt.Type] = true;
			Main.tileSolid[mt.Type] = true;
			Main.tileNoAttach[mt.Type] = true;
			Main.tileTable[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			TileID.Sets.Platforms[mt.Type] = true;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.StyleMultiplier = 27;
			TileObjectData.newTile.StyleWrapLimit = 27;
			TileObjectData.newTile.UsesCustomCanPlace = false;
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.addTile(mt.Type);

			// All platforms count as doors (so that you may have top-or-bottom entry/exit rooms)
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a sink.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpSink(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileLighted[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.addTile(mt.Type);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a sofa.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpSofa(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileLighted[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.addTile(mt.Type);

			// All sofas count as chairs.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a table.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpTable(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileSolidTop[mt.Type] = true;
			Main.tileLighted[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileNoAttach[mt.Type] = true;
			Main.tileTable[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.addTile(mt.Type);

			// As you could probably guess, all tables count as tables.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a torch.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpTorch(this ModTile mt, bool lavaImmune = false, bool waterImmune = false)
		{
			Main.tileLighted[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileSolid[mt.Type] = false;
			Main.tileNoAttach[mt.Type] = true;
			Main.tileNoFail[mt.Type] = true;
			Main.tileWaterDeath[mt.Type] = !waterImmune;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleTorch);
			TileObjectData.newTile.WaterDeath = !waterImmune;
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);
			TileObjectData.newAlternate.WaterDeath = false;
			TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
			TileObjectData.newAlternate.AnchorAlternateTiles = new int[] { 124 };
			TileObjectData.addAlternate(1);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);
			TileObjectData.newAlternate.WaterDeath = false;
			TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
			TileObjectData.newAlternate.AnchorAlternateTiles = new int[] { 124 };
			TileObjectData.addAlternate(2);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);
			TileObjectData.newAlternate.WaterDeath = false;
			TileObjectData.newAlternate.AnchorWall = true;
			TileObjectData.addAlternate(0);
			TileObjectData.addTile(mt.Type);

			// All torches count as light sources.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a work bench.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpWorkBench(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileSolidTop[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileNoAttach[mt.Type] = true;
			Main.tileTable[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
			TileObjectData.newTile.CoordinateHeights = new int[] { 18 };
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.addTile(mt.Type);

			// All work benches count as tables.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a fountain.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		internal static void SetUpFountain(this ModTile mt)
		{
			//All fountains are immune to lava
			Main.tileLighted[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = false;
			Main.tileWaterDeath[mt.Type] = false;
			//TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
			//TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(mt.Type);
			TileID.Sets.HasOutlines[mt.Type] = true;

			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 4;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.Origin = new Point16(0, 3);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, 2, 0);
			TileObjectData.addTile(mt.Type);
		}
		#endregion
		#endregion

		#region Tile Entity Utilities
		/// <summary>
		/// A hitbox that should approximately represent the tip of the mouse cursor. Used for UI stuff.
		/// </summary>
		public static Rectangle MouseHitbox => new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 2, 2);

		/// <summary>
		/// Finds the Calamity tile entity that is associated with the tile at the given world coordinates (i, j).<br></br>
		/// Can return <b>null</b> if nothing is found.
		/// </summary>
		/// <typeparam name="T">The Type of Tile Entity to look for. Must be a modded tile entity.</typeparam>
		/// <param name="i">The X coordinate of the tile in the world.</param>
		/// <param name="j">The Y coordinate of the tile in the world.</param>
		/// <param name="width">The width of the <b>FrameImportant</b> Tile that hosts the tile entity.</param>
		/// <param name="height">The height of the <b>FrameImportant</b> Tile that hosts the tile entity.</param>
		/// <param name="sheetSquare">The width in pixels of each tile-square on the Tile's sprite sheet. Usually 16. Can be 18 if the sprite sheet has pink lines.</param>
		/// <returns>A <b>ModTileEntity</b> of the proper type, or <b>null</b>.</returns>
		public static T FindTileEntity<T>(int i, int j, int width, int height, int sheetSquare = 16) where T : ModTileEntity
		{
			// Find the top left corner of the FrameImportant tile that the player clicked on in the world.
			Tile t = Main.tile[i, j];
			int left = i - t.frameX % (width * sheetSquare) / sheetSquare;
			int top = j - t.frameY % (height * sheetSquare) / sheetSquare;

			byte chargerType = ModContent.GetInstance<T>().type;
			bool exists = TileEntity.ByPosition.TryGetValue(new Point16(left, top), out TileEntity te);
			return exists && te.type == chargerType ? (T)te : null;
		}

		/// <summary>
		/// Cancels any currently active sign or chest name edits, and closes any chest the player may have open.
		/// </summary>
		/// <param name="player">The player to act upon. Should basically always be Main.LocalPlayer.</param>
		public static void CancelSignsAndChests(this Player player)
		{
			Main.mouseRightRelease = false;

			// If a sign or chest was in use previously, close those GUIs.
			if (player.sign >= 0)
			{
				Main.PlaySound(SoundID.MenuClose);
				player.sign = -1;
				Main.editSign = false;
				Main.npcChatText = "";
			}
			if (Main.editChest)
			{
				Main.PlaySound(SoundID.MenuTick);
				Main.editChest = false;
				Main.npcChatText = "";
			}
			if (player.chest >= 0)
				player.chest = -1;
		}

		// Draws the Power Cell item slot in a UI. Used by both the Power Cell Factory and Charging Station.
		internal static void DrawPowercellSlot(SpriteBatch spriteBatch, Item item, Vector2 drawPosition, float iconScale = 0.7f)
		{
			Texture2D slotBackgroundTex = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/PowerCellSlot_Empty");

			// This check is done twice because the draw order matters. We want to draw the background icon before any text.
			if (item.stack > 0)
				slotBackgroundTex = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/PowerCellSlot_Filled");

			spriteBatch.Draw(slotBackgroundTex, drawPosition, null, Color.White, 0f, slotBackgroundTex.Size() * 0.5f, iconScale, SpriteEffects.None, 0f);
			if (item.stack > 0)
			{
				float inventoryScale = Main.inventoryScale;
				Vector2 numberOffset = slotBackgroundTex.Size() * 0.2f;
				numberOffset.X -= 17f;
				ChatManager.DrawColorCodedStringWithShadow(spriteBatch,
					Main.fontItemStack,
					item.stack.ToString(),
					drawPosition + numberOffset * inventoryScale,
					Color.White,
					0f,
					Vector2.Zero,
					new Vector2(inventoryScale),
					-1f,
					inventoryScale);
			}
		}
		#endregion

		#region Drawing Utilities
		public static void DrawItemGlowmaskSingleFrame(this Item item, SpriteBatch spriteBatch, float rotation, Texture2D glowmaskTexture)
		{
			Vector2 origin = new Vector2(glowmaskTexture.Width / 2f, glowmaskTexture.Height / 2f - 2f);
			spriteBatch.Draw(glowmaskTexture, item.Center - Main.screenPosition, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
		}

		public static Rectangle GetCurrentFrame(this Item item, ref int frame, ref int frameCounter, int frameDelay, int frameAmt, bool frameCounterUp = true)
		{
			if (frameCounter >= frameDelay)
			{
				frameCounter = -1;
				frame = frame == frameAmt - 1 ? 0 : frame + 1;
			}
			if (frameCounterUp)
				frameCounter++;
			return new Rectangle(0, item.height * frame, item.width, item.height);
		}

		public static bool DrawFishingLine(this Projectile projectile, int fishingRodType, Color poleColor, int xPositionAdditive = 45, float yPositionAdditive = 35f)
		{
			Player player = Main.player[projectile.owner];
			Item item = player.HeldItem;
			if (!projectile.bobber || item.holdStyle <= 0)
				return false;

			float originX = player.MountedCenter.X;
			float originY = player.MountedCenter.Y;
			originY += player.gfxOffY;
			//This variable is used to account for Gravitation Potions
			float gravity = player.gravDir;

			if (item.type == fishingRodType)
			{
				originX += (float)(xPositionAdditive * player.direction);
				if (player.direction < 0)
				{
					originX -= 13f;
				}
				originY -= yPositionAdditive * gravity;
			}

			if (gravity == -1f)
			{
				originY -= 12f;
			}
			Vector2 mountedCenter = new Vector2(originX, originY);
			mountedCenter = player.RotatedRelativePoint(mountedCenter + new Vector2(8f), true) - new Vector2(8f);
			Vector2 lineOrigin = projectile.Center - mountedCenter;
			bool canDraw = true;
			if (lineOrigin.X == 0f && lineOrigin.Y == 0f)
				return false;

			float projPosMagnitude = lineOrigin.Length();
			projPosMagnitude = 12f / projPosMagnitude;
			lineOrigin.X *= projPosMagnitude;
			lineOrigin.Y *= projPosMagnitude;
			mountedCenter -= lineOrigin;
			lineOrigin = projectile.Center - mountedCenter;

			while (canDraw)
			{
				float height = 12f;
				float positionMagnitude = lineOrigin.Length();
				if (float.IsNaN(positionMagnitude) || float.IsNaN(positionMagnitude))
					break;

				if (positionMagnitude < 20f)
				{
					height = positionMagnitude - 8f;
					canDraw = false;
				}
				positionMagnitude = 12f / positionMagnitude;
				lineOrigin.X *= positionMagnitude;
				lineOrigin.Y *= positionMagnitude;
				mountedCenter += lineOrigin;
				lineOrigin = projectile.Center - mountedCenter;
				if (positionMagnitude > 12f)
				{
					float positionInverseMultiplier = 0.3f;
					float absVelocitySum = Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y);
					if (absVelocitySum > 16f)
					{
						absVelocitySum = 16f;
					}
					absVelocitySum = 1f - absVelocitySum / 16f;
					positionInverseMultiplier *= absVelocitySum;
					absVelocitySum = positionMagnitude / 80f;
					if (absVelocitySum > 1f)
					{
						absVelocitySum = 1f;
					}
					positionInverseMultiplier *= absVelocitySum;
					if (positionInverseMultiplier < 0f)
					{
						positionInverseMultiplier = 0f;
					}
					absVelocitySum = 1f - projectile.localAI[0] / 100f;
					positionInverseMultiplier *= absVelocitySum;
					if (lineOrigin.Y > 0f)
					{
						lineOrigin.Y *= 1f + positionInverseMultiplier;
						lineOrigin.X *= 1f - positionInverseMultiplier;
					}
					else
					{
						absVelocitySum = Math.Abs(projectile.velocity.X) / 3f;
						if (absVelocitySum > 1f)
						{
							absVelocitySum = 1f;
						}
						absVelocitySum -= 0.5f;
						positionInverseMultiplier *= absVelocitySum;
						if (positionInverseMultiplier > 0f)
						{
							positionInverseMultiplier *= 2f;
						}
						lineOrigin.Y *= 1f + positionInverseMultiplier;
						lineOrigin.X *= 1f - positionInverseMultiplier;
					}
				}
				//This color decides the color of the fishing line.
				Color lineColor = Lighting.GetColor((int)mountedCenter.X / 16, (int)mountedCenter.Y / 16, poleColor);
				float rotation = lineOrigin.ToRotation() - MathHelper.PiOver2;

				Main.spriteBatch.Draw(Main.fishingLineTexture, new Vector2(mountedCenter.X - Main.screenPosition.X + Main.fishingLineTexture.Width * 0.5f, mountedCenter.Y - Main.screenPosition.Y + Main.fishingLineTexture.Height * 0.5f), new Rectangle?(new Rectangle(0, 0, Main.fishingLineTexture.Width, (int)height)), lineColor, rotation, new Vector2(Main.fishingLineTexture.Width * 0.5f, 0f), 1f, SpriteEffects.None, 0f);
			}
			return false;
		}

		public static void DrawHook(this Projectile projectile, Texture2D hookTexture, float angleAdditive = 0f)
		{
			Player player = Main.player[projectile.owner];
			Vector2 center = projectile.Center;
			float angleToMountedCenter = projectile.AngleTo(player.MountedCenter) - MathHelper.PiOver2;
			bool canShowHook = true;
			while (canShowHook)
			{
				float distanceMagnitude = (player.MountedCenter - center).Length(); //Exact same as using a Sqrt
				if (distanceMagnitude < hookTexture.Height + 1f)
				{
					canShowHook = false;
				}
				else if (float.IsNaN(distanceMagnitude))
				{
					canShowHook = false;
				}
				else
				{
					center += projectile.DirectionTo(player.MountedCenter) * hookTexture.Height;
					Color tileAtCenterColor = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16f));
					Main.spriteBatch.Draw(hookTexture, center - Main.screenPosition, 
						new Rectangle?(new Rectangle(0, 0, hookTexture.Width, hookTexture.Height)), 
						tileAtCenterColor, angleToMountedCenter + angleAdditive, 
						hookTexture.Size() / 2, 1f, SpriteEffects.None, 0f);
				}
			}
		}

		internal static void IterateDisco(ref Color c, ref float aiParam, in byte discoIter = 7)
		{
			switch (aiParam)
			{
				case 0f:
					c.G += discoIter;
					if (c.G >= 255)
					{
						c.G = 255;
						aiParam = 1f;
					}
					break;
				case 1f:
					c.R -= discoIter;
					if (c.R <= 0)
					{
						c.R = 0;
						aiParam = 2f;
					}
					break;
				case 2f:
					c.B += discoIter;
					if (c.B >= 255)
					{
						c.B = 255;
						aiParam = 3f;
					}
					break;
				case 3f:
					c.G -= discoIter;
					if (c.G <= 0)
					{
						c.G = 0;
						aiParam = 4f;
					}
					break;
				case 4f:
					c.R += discoIter;
					if (c.R >= 255)
					{
						c.R = 255;
						aiParam = 5f;
					}
					break;
				case 5f:
					c.B -= discoIter;
					if (c.B <= 0)
					{
						c.B = 0;
						aiParam = 0f;
					}
					break;
				default:
					aiParam = 0f;
					c = Color.Red;
					break;
			}
		}

		public static string ColorMessage(string msg, Color color)
		{
			StringBuilder sb = new StringBuilder(msg.Length + 12);
			sb.Append("[c/").Append(color.Hex3()).Append(':').Append(msg).Append(']');
			return sb.ToString();
		}

		/// <summary>
		/// Returns a color lerp that allows for smooth transitioning between two given colors
		/// </summary>
		/// <param name="firstColor">The first color you want it to switch between</param>
		/// <param name="secondColor">The second color you want it to switch between</param>
		/// <param name="seconds">How long you want it to take to swap between colors</param>
		public static Color ColorSwap(Color firstColor, Color secondColor, float seconds)
		{
			double timeMult = (double)(MathHelper.TwoPi / seconds);
			float colorMePurple = (float)((Math.Sin(timeMult * Main.GlobalTime) + 1) * 0.5f);
			return Color.Lerp(firstColor, secondColor, colorMePurple);
		}
		/// <summary>
		/// Returns a color lerp that supports multiple colors.
		/// </summary>
		/// <param name="increment">The 0-1 incremental value used when interpolating.</param>
		/// <param name="colors">The various colors to interpolate across.</param>
		/// <returns></returns>
		public static Color MulticolorLerp(float increment, params Color[] colors)
		{
			int currentColorIndex = (int)(increment * colors.Length);
			Color currentColor = colors[currentColorIndex];
			Color nextColor = colors[(currentColorIndex + 1) % colors.Length];
			return Color.Lerp(currentColor, nextColor, increment * colors.Length % 1f);
		}
		#endregion

		#region Mathematical Utilities
		/// <summary>
		/// Computes the Manhattan Distance between two points. This is typically used as a cheaper alternative to Euclidean Distance.
		/// </summary>
		/// <param name="a">The first point.</param>
		/// <param name="b">The second point.</param>
		public static float ManhattanDistance(this Vector2 a, Vector2 b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
		#endregion

		#region Miscellaneous Utilities
		public static T[] ShuffleArray<T>(T[] array, Random rand = null)
		{
			if (rand is null)
				rand = new Random();

			for (int i = array.Length; i > 0; --i)
			{
				int j = rand.Next(i);
				T tempElement = array[j];
				array[j] = array[i - 1];
				array[i - 1] = tempElement;
			}
			return array;
		}

		/// <summary>
		/// Retrieves all the colors from a <see cref="Texture2D"/> and returns them as a 2D <see cref="Color"/> array.
		/// </summary>
		/// <param name="texture">The texture to load.</param>
		/// <returns></returns>
		public static Color[,] GetColorsFromTexture(this Texture2D texture)
		{
			Color[] alignedColors = new Color[texture.Width * texture.Height];
			texture.GetData(alignedColors); // Fills the color array with all the colors in the texture

			Color[,] colors2D = new Color[texture.Width, texture.Height];
			for (int x = 0; x < texture.Width; x++)
			{
				for (int y = 0; y < texture.Height; y++)
				{
					colors2D[x, y] = alignedColors[x + y * texture.Width];
				}
			}
			return colors2D;
		}

		public static string CombineStrings(params object[] args)
		{
			StringBuilder result = new StringBuilder(1024);
			for(int i = 0; i < args.Length; ++i)
			{
				object o = args[i];
				result.Append(o.ToString());
				result.Append(' ');
			}
			return result.ToString();
		}

		/// <summary>
		/// Calculates the sound volume and panning for a sound which is played at the specified location in the game world.<br/>
		/// Note that sound does not play on dedicated servers or during world generation.
		/// </summary>
		/// <param name="soundPos">The position the sound is emitting from. If either X or Y is -1, the sound does not fade with distance.</param>
		/// <param name="ambient">Whether the sound is considered ambient, which makes it use the ambient sound slider in the options. Defaults to false.</param>
		/// <returns>Volume and pan, in that order. Volume is always between 0 and 1. Pan is always between -1 and 1.</returns>
		public static (float, float) CalculateSoundStats(Vector2 soundPos, bool ambient = false)
		{
			float volume = 0f;
			float pan = 0f;

			if (soundPos.X == -1f || soundPos.Y == -1f)
				volume = 1f;
			else if (WorldGen.gen || Main.dedServ || Main.netMode == NetmodeID.Server)
				volume = 0f;
			else
			{
				float topLeftX = Main.screenPosition.X - Main.screenWidth * 2f;
				float topLeftY = Main.screenPosition.Y - Main.screenHeight * 2f;

				// Sounds cannot be heard from more than ~2.5 screens away.
				// This rectangle is 5x5 screens centered on the current screen center position.
				Rectangle audibleArea = new Rectangle((int)topLeftX, (int)topLeftY, Main.screenWidth * 5, Main.screenHeight * 5);
				Rectangle soundHitbox = new Rectangle((int)soundPos.X, (int)soundPos.Y, 1, 1);
				Vector2 screenCenter = Main.screenPosition + new Vector2(Main.screenWidth * 0.5f, Main.screenHeight * 0.5f);
				if (audibleArea.Intersects(soundHitbox))
				{
					pan = (soundPos.X - screenCenter.X) / (Main.screenWidth * 0.5f);
					float dist = Vector2.Distance(soundPos, screenCenter);
					volume = 1f - (dist / (Main.screenWidth * 1.5f));
				}
			}

			pan = MathHelper.Clamp(pan, -1f, 1f);
			volume = MathHelper.Clamp(volume, 0f, 1f);
			if (ambient)
				volume = Main.gameInactive ? 0f : volume * Main.ambientVolume;
			else
				volume *= Main.soundVolume;

			// This is actually done by vanilla. I guess if the sound volume gets corrupted during gameplay, you can't blast your eardrums out.
			volume = MathHelper.Clamp(volume, 0f, 1f);
			return (volume, pan);
		}

		/// <summary>
		/// Convenience function to utilize CalculateSoundStats immediately on an existing sound effect.<br/>
		/// This allows updating a looping sound every single frame to have the correct volume and pan, even if the player drags the audio sliders around.
		/// </summary>
		/// <param name="sfx">The SoundEffectInstance which is having its values updated.</param>
		/// <param name="soundPos">The position the sound is emitting from. If either X or Y is -1, the sound does not fade with distance.</param>
		/// <param name="ambient">Whether the sound is considered ambient, which makes it use the ambient sound slider in the options. Defaults to false.</param>
		public static void ApplySoundStats(ref SoundEffectInstance sfx, Vector2 soundPos, bool ambient = false)
		{
			if (sfx is null || sfx.IsDisposed)
				return;
			(sfx.Volume, sfx.Pan) = CalculateSoundStats(soundPos, ambient);
		}

		public static void StartRain(bool torrentialTear = false)
		{
			int num = 86400;
			int num2 = num / 24;
			Main.rainTime = Main.rand.Next(num2 * 8, num);
			if (Main.rand.NextBool(3))
			{
				Main.rainTime += Main.rand.Next(0, num2);
			}
			if (Main.rand.NextBool(4))
			{
				Main.rainTime += Main.rand.Next(0, num2 * 2);
			}
			if (Main.rand.NextBool(5))
			{
				Main.rainTime += Main.rand.Next(0, num2 * 2);
			}
			if (Main.rand.NextBool(6))
			{
				Main.rainTime += Main.rand.Next(0, num2 * 3);
			}
			if (Main.rand.NextBool(7))
			{
				Main.rainTime += Main.rand.Next(0, num2 * 4);
			}
			if (Main.rand.NextBool(8))
			{
				Main.rainTime += Main.rand.Next(0, num2 * 5);
			}
			float num3 = 1f;
			if (Main.rand.NextBool(2))
			{
				num3 += 0.05f;
			}
			if (Main.rand.NextBool(3))
			{
				num3 += 0.1f;
			}
			if (Main.rand.NextBool(4))
			{
				num3 += 0.15f;
			}
			if (Main.rand.NextBool(5))
			{
				num3 += 0.2f;
			}
			Main.rainTime = (int)(Main.rainTime * num3);
			Main.raining = true;
			if (torrentialTear)
				TorrentialTear.AdjustRainSeverity(false);
			CalamityNetcode.SyncWorld();
		}

		public static void StartSandstorm()
		{
			typeof(Terraria.GameContent.Events.Sandstorm).GetMethod("StartSandstorm", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
		}

		public static void StopSandstorm()
		{
			Terraria.GameContent.Events.Sandstorm.Happening = false;
		}

		public static void AddWithCondition<T>(this List<T> list, T type, bool condition)
		{
			if (condition)
				list.Add(type);
		}

		public static void Inflict246DebuffsNPC(NPC target, int buff, float timeBase = 2f)
		{
			if (Main.rand.NextBool(4))
			{
				target.AddBuff(buff, SecondsToFrames(timeBase * 3f), false);
			}
			else if (Main.rand.NextBool(2))
			{
				target.AddBuff(buff, SecondsToFrames(timeBase * 2f), false);
			}
			else
			{
				target.AddBuff(buff, SecondsToFrames(timeBase), false);
			}
		}

		public static void Inflict246DebuffsPvp(Player target, int buff, float timeBase = 2f)
		{
			if (Main.rand.NextBool(4))
			{
				target.AddBuff(buff, SecondsToFrames(timeBase * 3f), false);
			}
			else if (Main.rand.NextBool(2))
			{
				target.AddBuff(buff, SecondsToFrames(timeBase * 2f), false);
			}
			else
			{
				target.AddBuff(buff, SecondsToFrames(timeBase), false);
			}
		}

		public static int SecondsToFrames(float seconds) => (int)(seconds * 60);

		/// <summary>
		/// Call this function to spawn explosion clouds at the specified location. Good for when NPCs or projectiles die and need to explode.
		/// </summary>
		/// <param name="goreSource">The spot to spawn the explosion clouds</param>
		/// <param name="goreAmt">Number of times it loops to spawn gores</param>
		public static void ExplosionGores (Vector2 goreSource, int goreAmt)
		{
			Vector2 source = new Vector2(goreSource.X - 24f, goreSource.Y - 24f);
			for (int goreIndex = 0; goreIndex < goreAmt; goreIndex++)
			{
				float velocityMult = 0.33f;
				if (goreIndex < (int)(goreAmt/3))
				{
					velocityMult = 0.66f;
				}
				if (goreIndex >= (int)((2*goreAmt)/3))
				{
					velocityMult = 1f;
				}
				int smoke = Gore.NewGore(source, default, Main.rand.Next(61, 64), 1f);
				Gore gore = Main.gore[smoke];
				gore.velocity *= velocityMult;
				gore.velocity.X += 1f;
				gore.velocity.Y += 1f;
				smoke = Gore.NewGore(source, default, Main.rand.Next(61, 64), 1f);
				gore.velocity *= velocityMult;
				gore.velocity.X -= 1f;
				gore.velocity.Y += 1f;
				smoke = Gore.NewGore(source, default, Main.rand.Next(61, 64), 1f);
				gore.velocity *= velocityMult;
				gore.velocity.X += 1f;
				gore.velocity.Y -= 1f;
				smoke = Gore.NewGore(source, default, Main.rand.Next(61, 64), 1f);
				gore.velocity *= velocityMult;
				gore.velocity.X -= 1f;
				gore.velocity.Y -= 1f;
			}
		}

		// REMOVE THIS IN CALAMITY 1.4, it's a 1.4 Main.cs function
		public static float GetLerpValue(float from, float to, float t, bool clamped = false)
		{
			if (clamped)
			{
				if (from < to)
				{
					if (t < from)
					{
						return 0f;
					}
					if (t > to)
					{
						return 1f;
					}
				}
				else
				{
					if (t < to)
					{
						return 1f;
					}
					if (t > from)
					{
						return 0f;
					}
				}
			}
			return (t - from) / (to - from);
		}

		/// <summary>
		/// Clamps the distance between vectors via normalization.
		/// </summary>
		/// <param name="start">The starting point.</param>
		/// <param name="end">The ending point.</param>
		/// <param name="maxDistance">The maximum possible distance between the two vectors before they get clamped.</param>
		public static void DistanceClamp(ref Vector2 start, ref Vector2 end, float maxDistance)
		{
			if (Vector2.Distance(end, start) > maxDistance)
			{
				end = start + Vector2.Normalize(end - start) * maxDistance;
			}
		}

		// REMOVE THIS IN CALAMITY 1.4, it's a 1.4 World.cs function
		public static Rectangle ClampToWorld(Rectangle tileRectangle)
		{
			int num = Math.Max(0, Math.Min(tileRectangle.Left, Main.maxTilesX));
			int num2 = Math.Max(0, Math.Min(tileRectangle.Top, Main.maxTilesY));
			int num3 = Math.Max(0, Math.Min(tileRectangle.Right, Main.maxTilesX));
			int num4 = Math.Max(0, Math.Min(tileRectangle.Bottom, Main.maxTilesY));
			return new Rectangle(num, num2, num3 - num, num4 - num2);
		}
		#endregion

		#region Saving, Loading, and Sending Utilities
		/// <summary>
		/// Saves a mod item and appends the data to a <see cref="TagCompound"/>.
		/// </summary>
		/// <param name="tag">The tag to append to.</param>
		/// <param name="itemToSave">The item to save.</param>
		/// <param name="itemIndex">A number used for discerning the item out of multiple items.</param>
		public static void SaveModItem(TagCompound tag, Item itemToSave, int itemIndex = 0)
		{
			tag.Add($"mod{itemIndex}", itemToSave.modItem?.mod?.Name ?? string.Empty);
			tag.Add($"name{itemIndex}", itemToSave.modItem?.Name ?? string.Empty);
		}

		/// <summary>
		/// Loads a mod item from a <see cref="TagCompound"/>.
		/// </summary>
		/// <param name="tag">The tag that contains the original item.</param>
		/// <param name="itemIndex">The number used to discern the item out of potentially multiple.</param>
		/// <returns>The original item.</returns>
		public static Item LoadModItem(TagCompound tag, int itemIndex = 0)
		{
			string modName = tag.GetString($"mod{itemIndex}");
			string itemName = tag.GetString($"name{itemIndex}");
			Item item = new Item();

			// Don't bother checking any further if something is an empty string.
			// Doing the checks below would result in checking for a mod with an empty string for a name.
			// This can cause highly unpredictable/unstable behavior, such as the game crashing when you hover
			// over the item in the GUI.
			if (string.IsNullOrEmpty(modName) || string.IsNullOrEmpty(itemName))
			{
				item.type = ItemID.None;
				return item;
			}

			int type = ModLoader.GetMod(modName)?.ItemType(itemName) ?? ItemID.None;
			if (type > ItemID.None)
			{
				item.netDefaults(type);
				item.modItem.Load(tag.GetCompound("data"));
				object[] loadGlobalsParameters = new object[] { item, tag.GetList<TagCompound>("globalData") };
				typeof(ItemIO).GetMethod("LoadGlobals", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(Item), typeof(IList<TagCompound>) }, null).Invoke(null, loadGlobalsParameters);
			}
			return item;
		}
		#endregion
	}
}
