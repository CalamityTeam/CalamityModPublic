using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod
{
	public static partial class CalamityUtils
	{
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
		/// Gives the player the specified number of immunity frames (or "iframes" for short).<br />If the player already has more iframes than you want to give them, this function does nothing.
		/// </summary>
		/// <param name="player">The player who should be given immunity frames.</param>
		/// <param name="frames">The number of immunity frames to give.</param>
		/// <param name="blink">Whether or not the player should be blinking during this time.</param>
		/// <returns>Whether or not any immunity frames were given.</returns>
		public static bool GiveIFrames(this Player player, int frames, bool blink = false)
		{
			// Check to see if there is any way for the player to get iframes from this operation.
			bool anyIFramesWouldBeGiven = false;
			for (int i = 0; i < player.hurtCooldowns.Length; ++i)
				if (player.hurtCooldowns[i] < frames)
					anyIFramesWouldBeGiven = true;

			// If they would get nothing, don't do it.
			if (!anyIFramesWouldBeGiven)
				return false;

			// Apply iframes thoroughly.
			player.immune = true;
			player.immuneNoBlink = !blink;
			player.immuneTime = frames;
			for (int i = 0; i < player.hurtCooldowns.Length; ++i)
				if (player.hurtCooldowns[i] < frames)
					player.hurtCooldowns[i] = frames;
			return true;
		}

		public static void RemoveAllIFrames(this Player player)
		{
			player.immune = false;
			player.immuneNoBlink = false;
			player.immuneTime = 0;
			for (int i = 0; i < player.hurtCooldowns.Length; ++i)
				player.hurtCooldowns[i] = 0;
		}


		/// <summary>
		/// Returns the damage multiplier Adrenaline Mode provides for the given player.
		/// </summary>
		/// <param name="mp">The player whose Adrenaline damage should be calculated.</param>
		/// <returns>Adrenaline damage multiplier. 1.0 would be no change.</returns>
		public static double GetAdrenalineDamage(this CalamityPlayer mp)
		{
			double adrenalineBoost = CalamityPlayer.AdrenalineDamageBoost;
			if (mp.adrenalineBoostOne)
				adrenalineBoost += CalamityPlayer.AdrenalineDamagePerBooster;
			if (mp.adrenalineBoostTwo)
				adrenalineBoost += CalamityPlayer.AdrenalineDamagePerBooster;
			if (mp.adrenalineBoostThree)
				adrenalineBoost += CalamityPlayer.AdrenalineDamagePerBooster;

			return adrenalineBoost;
		}

		/// <summary>
		/// Applies Rage and Adrenaline to the given damage multiplier. The values controlling the so-called "Rippers" can be found in CalamityPlayer.
		/// </summary>
		/// <param name="mp">The CalamityPlayer who may or may not be using Rage or Adrenaline.</param>
		/// <param name="damageMult">A reference to the current in-use damage multiplier. This will be increased in-place.</param>
		public static void ApplyRippersToDamage(CalamityPlayer mp, bool trueMelee, ref double damageMult)
		{
			// Reduce how much true melee benefits from Rage and Adrenaline.
			double rageAndAdrenalineTrueMeleeDamageMult = 0.5;

			// Rage and Adrenaline now stack additively with no special cases.
			if (mp.rageModeActive)
				damageMult += trueMelee ? mp.RageDamageBoost * rageAndAdrenalineTrueMeleeDamageMult : mp.RageDamageBoost;
			if (mp.adrenalineModeActive)
				damageMult += trueMelee ? mp.GetAdrenalineDamage() * rageAndAdrenalineTrueMeleeDamageMult : mp.GetAdrenalineDamage();
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

		/// <summary>
		/// Inflict typical exo weapon debuffs in pvp.
		/// </summary>
		/// <param name="target">The Player attacked.</param>
		/// <param name="multiplier">Debuff time multiplier if needed.</param>
		/// <returns>Inflicts debuffs if the target isn't immune.</returns>
		public static void ExoDebuffs(this Player target, float multiplier = 1f)
		{
			target.AddBuff(BuffType<ExoFreeze>(), (int)(30 * multiplier));
			target.AddBuff(BuffType<HolyFlames>(), (int)(120 * multiplier));
			target.AddBuff(BuffID.Frostburn, (int)(150 * multiplier));
			target.AddBuff(BuffID.OnFire, (int)(180 * multiplier));
		}

		/// <summary>
		/// Checks if the player is ontop of solid ground. May also check for solid ground for X tiles in front of them
		/// </summary>
		/// <param name="player">The Player whose position is being checked</param>
		/// <param name="solidGroundAhead">How many tiles in front of the player to check</param>
		/// <param name="airExposureNeeded">How many tiles above every checked tile are checked for non-solid ground</param>
		public static bool CheckSolidGround(this Player player, int solidGroundAhead = 0, int airExposureNeeded = 0)
		{
			if (player.velocity.Y != 0) //Player gotta be standing still in any case
				return false;

			Tile checkedTile;
			bool ConditionMet = true;

			for (int i = 0; i <= solidGroundAhead; i++) //Check i tiles in front of the player
			{
				ConditionMet = Main.tile[(int)player.Center.X / 16 + player.direction * i, (int)(player.position.Y + (float)player.height - 1f) / 16 + 1].IsTileSolidGround();
				if (!ConditionMet)
					return ConditionMet;

				for (int j = 1; j <= airExposureNeeded; j++) //Check j tiles ontop of each checked tiles for non-solid ground
				{
					checkedTile = Main.tile[(int)player.Center.X / 16 + player.direction * i, (int)(player.position.Y + (float)player.height - 1f) / 16 + 1 - j];

					ConditionMet = !(checkedTile != null && checkedTile.nactive() && Main.tileSolid[checkedTile.type]); //IsTileSolidGround minus the ground part, to avoid platforms and other half solid tiles messing it up
					if (!ConditionMet) 
						return ConditionMet;
				}
			}
			return ConditionMet;
		}

		/// <summary>
		/// Makes the given player send the given packet to all appropriate receivers.<br />
		/// If server is false, the packet is sent only to the multiplayer host.<br />
		/// If server is true, the packet is sent to all clients except the player it pertains to.
		/// </summary>
		/// <param name="player">The player to whom the packet's data pertains.</param>
		/// <param name="packet">The packet to send with certain parameters.</param>
		/// <param name="server">True if a dedicated server is broadcasting information to all players.</param>
		public static void SendPacket(this Player player, ModPacket packet, bool server)
		{
			// Client: Send the packet only to the host.
			if (!server)
				packet.Send();

			// Server: Send the packet to every OTHER client.
			else
				packet.Send(-1, player.whoAmI);
		}
	}
}
