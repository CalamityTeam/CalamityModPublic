using CalamityMod.CalPlayer;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor;
using CalamityMod.Items.Placeables.Furniture.Fountains;
using CalamityMod.Items.Tools;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
	public partial class CalamityGlobalItem : GlobalItem
	{
		#region Main ModifyTooltips Function
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			// Apply rarity coloration to the item's name.
			TooltipLine nameLine = tooltips.FirstOrDefault(x => x.Name == "ItemName" && x.mod == "Terraria");
			if (nameLine != null)
				ApplyRarityColor(item, nameLine);

			// If the item is melee, add a true melee damage number adjacent to the standard damage number.
			if (item.melee && item.damage > 0 && Main.LocalPlayer.Calamity().trueMeleeDamage > 0D)
				TrueMeleeDamageTooltip(item, tooltips);

			// Modify all vanilla tooltips before appending mod mechanics (if any).
			ModifyVanillaTooltips(item, tooltips);

			// If the item has a stealth generation prefix, show that on the tooltip.
			// This is placed between vanilla tooltip edits and mod mechanics because it can apply to vanilla items.
			StealthGenAccessoryTooltip(item, tooltips);

			// Everything below this line can only apply to modded items. If the item is vanilla, stop here for efficiency.
			if (item.type < ItemID.Count)
				return;

			// Adds tooltips to Calamity fountains which match Fargo's fountain tooltips.
			FargoFountainTooltip(item, tooltips);

			// Adds a Current Charge tooltip to all items which use charge.
			CalamityGlobalItem modItem = item.Calamity();
			if (modItem?.UsesCharge ?? false)
			{
				// Convert current charge ratio into a percentage.
				float displayedPercent = ChargeRatio * 100f;
				TooltipLine line = new TooltipLine(mod, "CalamityCharge", $"Current Charge: {displayedPercent:N1}%");
				tooltips.Add(line);
			}

			// Adds "Donor Item" and "Developer Item" to donor items and developer items respectively.
			if (donorItem)
			{
				TooltipLine line = new TooltipLine(mod, "CalamityDonor", CalamityUtils.ColorMessage("- Donor Item -", CalamityUtils.DonatorItemColor));
				tooltips.Add(line);
			}
			if (devItem)
			{
				TooltipLine line = new TooltipLine(mod, "CalamityDev", CalamityUtils.ColorMessage("- Developer Item -", CalamityUtils.HotPinkRarityColor));
				tooltips.Add(line);
			}

			// Adds "Challenge Drop" or "Legendary Challenge Drop" to Malice Mode drops.
			// For Legendary Challenge Drops, this tooltip matches their unique rarity color.
			if (challengeDrop)
				ChallengeDropTooltip(item, tooltips);
		}
		#endregion

		#region Rarity Coloration
		private void ApplyRarityColor(Item item, TooltipLine nameLine)
		{
			// Apply standard post-ML rarities to the item's color first.
			Color? standardRarityColor = CalamityUtils.GetRarityColor(customRarity);
			if (standardRarityColor.HasValue)
				nameLine.overrideColor = standardRarityColor.Value;

			#region Uniquely Colored Developer Items
			if (item.type == ModContent.ItemType<Fabstaff>())
				nameLine.overrideColor = new Color(Main.DiscoR, 100, 255);
			if (item.type == ModContent.ItemType<BlushieStaff>())
				nameLine.overrideColor = new Color(0, 0, 255);
			if (item.type == ModContent.ItemType<Judgement>())
				nameLine.overrideColor = Judgement.GetSyncedLightColor();
			if (item.type == ModContent.ItemType<NanoblackReaperRogue>())
				nameLine.overrideColor = new Color(0.34f, 0.34f + 0.66f * Main.DiscoG / 255f, 0.34f + 0.5f * Main.DiscoG / 255f);
			if (item.type == ModContent.ItemType<ShatteredCommunity>())
				nameLine.overrideColor = ShatteredCommunity.GetRarityColor();
			if (item.type == ModContent.ItemType<ProfanedSoulCrystal>())
				nameLine.overrideColor = CalamityUtils.ColorSwap(new Color(255, 166, 0), new Color(25, 250, 25), 4f); //alternates between emerald green and amber (BanditHueh)
			if (item.type == ModContent.ItemType<BensUmbrella>())
				nameLine.overrideColor = CalamityUtils.ColorSwap(new Color(210, 0, 255), new Color(255, 248, 24), 4f);
			if (item.type == ModContent.ItemType<Endogenesis>())
				nameLine.overrideColor = CalamityUtils.ColorSwap(new Color(131, 239, 255), new Color(36, 55, 230), 4f);
			if (item.type == ModContent.ItemType<DraconicDestruction>())
				nameLine.overrideColor = CalamityUtils.ColorSwap(new Color(255, 69, 0), new Color(139, 0, 0), 4f);
			if (item.type == ModContent.ItemType<ScarletDevil>())
				nameLine.overrideColor = CalamityUtils.ColorSwap(new Color(191, 45, 71), new Color(185, 187, 253), 4f);
			if (item.type == ModContent.ItemType<RedSun>())
				nameLine.overrideColor = CalamityUtils.ColorSwap(new Color(204, 86, 80), new Color(237, 69, 141), 4f);
			if (item.type == ModContent.ItemType<GaelsGreatsword>())
				nameLine.overrideColor = new Color(146, 0, 0);
			if (item.type == ModContent.ItemType<CrystylCrusher>())
				nameLine.overrideColor = new Color(129, 29, 149);
			if (item.type == ModContent.ItemType<Svantechnical>())
				nameLine.overrideColor = new Color(220, 20, 60);
			if (item.type == ModContent.ItemType<SomaPrime>())
				nameLine.overrideColor = new Color(254, 253, 235);
			if (item.type == ModContent.ItemType<Contagion>())
				nameLine.overrideColor = new Color(207, 17, 117);
			if (item.type == ModContent.ItemType<TriactisTruePaladinianMageHammerofMightMelee>())
				nameLine.overrideColor = new Color(227, 226, 180);
			if (item.type == ModContent.ItemType<RoyalKnivesMelee>())
				nameLine.overrideColor = CalamityUtils.ColorSwap(new Color(154, 255, 151), new Color(228, 151, 255), 4f);
			if (item.type == ModContent.ItemType<DemonshadeHelm>() || item.type == ModContent.ItemType<DemonshadeBreastplate>() || item.type == ModContent.ItemType<DemonshadeGreaves>())
				nameLine.overrideColor = CalamityUtils.ColorSwap(new Color(255, 132, 22), new Color(221, 85, 7), 4f);

			// TODO -- for cleanliness, ALL color math should either be a one-line color swap or inside the item's own file
			// The items that currently violate this are all below:
			// Eternity, Flamsteed Ring, Earth
			if (item.type == ModContent.ItemType<Eternity>())
			{
				List<Color> colorSet = new List<Color>()
					{
						new Color(188, 192, 193), // white
						new Color(157, 100, 183), // purple
						new Color(249, 166, 77), // honey-ish orange
						new Color(255, 105, 234), // pink
						new Color(67, 204, 219), // sky blue
						new Color(249, 245, 99), // bright yellow
						new Color(236, 168, 247), // purplish pink
					};
				if (nameLine != null)
				{
					int colorIndex = (int)(Main.GlobalTime / 2 % colorSet.Count);
					Color currentColor = colorSet[colorIndex];
					Color nextColor = colorSet[(colorIndex + 1) % colorSet.Count];
					nameLine.overrideColor = Color.Lerp(currentColor, nextColor, Main.GlobalTime % 2f > 1f ? 1f : Main.GlobalTime % 1f);
				}
			}
			if (item.type == ModContent.ItemType<PrototypeAndromechaRing>())
			{
				if (Main.GlobalTime % 1f < 0.6f)
				{
					nameLine.overrideColor = new Color(89, 229, 255);
				}
				else if (Main.GlobalTime % 1f < 0.8f)
				{
					nameLine.overrideColor = Color.Lerp(new Color(89, 229, 255), Color.White, (Main.GlobalTime % 1f - 0.6f) / 0.2f);
				}
				else
				{
					nameLine.overrideColor = Color.Lerp(Color.White, new Color(89, 229, 255), (Main.GlobalTime % 1f - 0.8f) / 0.2f);
				}
			}
			if (item.type == ModContent.ItemType<Earth>())
			{
				List<Color> earthColors = new List<Color>()
							{
								new Color(255, 99, 146),
								new Color(255, 228, 94),
								new Color(127, 200, 248)
							};
				if (nameLine != null)
				{
					int colorIndex = (int)(Main.GlobalTime / 2 % earthColors.Count);
					Color currentColor = earthColors[colorIndex];
					Color nextColor = earthColors[(colorIndex + 1) % earthColors.Count];
					nameLine.overrideColor = Color.Lerp(currentColor, nextColor, Main.GlobalTime % 2f > 1f ? 1f : Main.GlobalTime % 1f);
				}
			}
			#endregion
		}
		#endregion

		#region Challenge Drop Tooltip
		private void ChallengeDropTooltip(Item item, IList<TooltipLine> tooltips)
		{
			Color? legendaryColor = null;
			if (item.type == ModContent.ItemType<AegisBlade>() || item.type == ModContent.ItemType<YharimsCrystal>())
				legendaryColor = new Color(255, Main.DiscoG, 53);
			if (item.type == ModContent.ItemType<BlossomFlux>() || item.type == ModContent.ItemType<Malachite>())
				legendaryColor = new Color(Main.DiscoR, 203, 103);
			if (item.type == ModContent.ItemType<BrinyBaron>() || item.type == ModContent.ItemType<ColdDivinity>())
				legendaryColor = new Color(53, Main.DiscoG, 255);
			if (item.type == ModContent.ItemType<CosmicDischarge>())
				legendaryColor = new Color(150, Main.DiscoG, 255);
			if (item.type == ModContent.ItemType<SeasSearing>())
				legendaryColor = new Color(60, Main.DiscoG, 190);
			if (item.type == ModContent.ItemType<SHPC>())
				legendaryColor = new Color(255, Main.DiscoG, 155);
			if (item.type == ModContent.ItemType<Vesuvius>() || item.type == ModContent.ItemType<GoldBurdenBreaker>())
				legendaryColor = new Color(255, Main.DiscoG, 0);
			if (item.type == ModContent.ItemType<PristineFury>())
				legendaryColor = CalamityUtils.ColorSwap(new Color(255, 168, 53), new Color(255, 249, 0), 2f);
			if (item.type == ModContent.ItemType<LeonidProgenitor>())
				legendaryColor = CalamityUtils.ColorSwap(LeonidProgenitor.blueColor, LeonidProgenitor.purpleColor, 3f);
			if (item.type == ModContent.ItemType<TheCommunity>())
				legendaryColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);

			Color lineColor = legendaryColor.GetValueOrDefault(CalamityUtils.ChallengeDropColor);
			string text = legendaryColor.HasValue ? "- Legendary Challenge Drop -" : "- Challenge Drop";
			TooltipLine line = new TooltipLine(mod, "CalamityChallengeDrop", text)
			{
				overrideColor = lineColor
			};
			tooltips.Add(line);
		}
		#endregion

		#region Vanilla Item Tooltip Modification
		private void ModifyVanillaTooltips(Item item, IList<TooltipLine> tooltips)
		{
			#region Modular Tooltip Editing Code
			// This is a modular tooltip editor which loops over all tooltip lines of an item,
			// selects all those which match an arbitrary function you provide,
			// then edits them using another arbitrary function you provide.
			void ApplyTooltipEdits(IList<TooltipLine> lines, Func<Item, TooltipLine, bool> predicate, Action<TooltipLine> action)
			{
				foreach (TooltipLine line in lines)
					if (predicate.Invoke(item, line))
						action.Invoke(line);
			}

			// This function produces simple predicates to match a specific line of a tooltip, by number/index.
			Func<Item, TooltipLine, bool> LineNum(int n) => (Item i, TooltipLine l) => l.mod == "Terraria" && l.Name == $"Tooltip{n}";
			// This function produces simple predicates to match a specific line of a tooltip, by name.
			Func<Item, TooltipLine, bool> LineName(string s) => (Item i, TooltipLine l) => l.mod == "Terraria" && l.Name == s;

			// These functions are shorthand to invoke ApplyTooltipEdits using the above predicates.
			void EditTooltipByNum(int lineNum, Action<TooltipLine> action) => ApplyTooltipEdits(tooltips, LineNum(lineNum), action);
			void EditTooltipByName(string lineName, Action<TooltipLine> action) => ApplyTooltipEdits(tooltips, LineName(lineName), action);
			#endregion

			// Numerous random tooltip edits which don't fit into another category
			#region Various Tooltip Edits

			// Mirrors and Recall Potions cannot be used while a boss is alive.
			if (item.type == ItemID.MagicMirror || item.type == ItemID.IceMirror || item.type == ItemID.CellPhone || item.type == ItemID.RecallPotion)
				ApplyTooltipEdits(tooltips,
					(i, l) => l.mod == "Terraria" && l.Name == (i.type == ItemID.CellPhone ? "Tooltip1" : "Tooltip0"),
					(line) => line.text += "\nCannot be used while a boss is alive");

			// Rod of Discord cannot be used multiple times to hurt yourself
			if (item.type == ItemID.RodofDiscord)
				EditTooltipByNum(0, (line) => line.text += "\nTeleportation is disabled while Chaos State is active");

			// Water removing items cannot be used in the Abyss
			string noAbyssLine = "\nCannot be used in the Abyss";
			if (item.type == ItemID.SuperAbsorbantSponge)
				EditTooltipByNum(0, (line) => line.text += noAbyssLine);
			if (item.type == ItemID.EmptyBucket)
				EditTooltipByName("Defense", (line) => line.text += noAbyssLine);

			// If Early Hardmode Rework is enabled: Remind users that ores will NOT spawn when an altar is smashed.
			if (CalamityConfig.Instance.EarlyHardmodeProgressionRework && (item.type == ItemID.Pwnhammer || item.type == ItemID.Hammush))
				EditTooltipByNum(0, (line) => line.text += "\nDemon Altars no longer spawn ores when destroyed");

			// Bottled Honey gives the Honey buff
			if (item.type == ItemID.BottledHoney)
				EditTooltipByName("HealLife", (line) => line.text += "\nGrants the Honey buff for 2 minutes");

			// Warmth Potion provides debuff immunities and Death Mode cold protection
			if (item.type == ItemID.WarmthPotion)
			{
				string immunityLine = "\nMakes you immune to the Chilled, Frozen, and Glacial State debuffs";
				if (CalamityWorld.death)
					immunityLine += "\nProvides cold protection in Death Mode";
				EditTooltipByNum(0, (line) => line.text += immunityLine);
			}

			// Hand Warmer provides Death Mode cold protection and has a side bonus with Eskimo armor
			if (item.type == ItemID.HandWarmer)
			{
				string extraLine = "\nProvides a regeneration boost while wearing the Eskimo armor";
				if (CalamityWorld.death)
					extraLine += "\nProvides cold protection in Death Mode";
				EditTooltipByNum(0, (line) => line.text += extraLine);
			}

			// Invisibility Potion provides various rogue boosts
			if (item.type == ItemID.InvisibilityPotion)
				EditTooltipByNum(0, (line) => line.text += "\nBoosts various rogue stats depending on held weapon");

			// Golden Fishing Rod inherently contains High Test Fishing Line
			if (item.type == ItemID.GoldenFishingRod)
				EditTooltipByName("NeedsBait", (line) => line.text += "\nIts fishing line will never break");

			// Eternity Crystal notifies the player that they can accelerate the invasion
			if (item.type == ItemID.DD2ElderCrystal)
				EditTooltipByNum(0, (line) => line.text += "\nOnce placed you can right click the crystal to skip waves or increase the spawn rate of the invaders");

			// Fix a vanilla mistake in Magic Quiver's tooltip
			// TODO -- in 1.4 this mistake is already corrected
			if (item.type == ItemID.MagicQuiver)
				EditTooltipByNum(0, (line) => line.text = line.text.Replace(" damage", " arrow damage"));
			#endregion

			// Black Belt and Master Ninja Gear have guaranteed dodges on a 60 second cooldown.
			#region Dodging Belt Tooltips
			string beltDodgeLine = "Grants the ability to dodge attacks\n" +
				$"The dodge has a {CalamityPlayer.BeltDodgeCooldown} second cooldown which is shared with all other dodges and reflects";
			if (item.type == ItemID.BlackBelt)
				EditTooltipByNum(0, (line) => line.text = beltDodgeLine);
			if (item.type == ItemID.MasterNinjaGear)
				EditTooltipByNum(1, (line) => line.text = beltDodgeLine);
			#endregion

			// Early Hardmode ore melee weapons have new on-hit effects.
			#region Early Hardmode Melee Tooltips

			// Cobalt
			if (item.type == ItemID.CobaltSword || item.type == ItemID.CobaltNaginata)
				EditTooltipByName("Knockback", (line) => line.text += "\nDecreases enemy defense by 10% on hit");

			// Palladium
			if (item.type == ItemID.PalladiumSword || item.type == ItemID.PalladiumPike)
				EditTooltipByName("Knockback", (line) => line.text += "\nIncreases life regen on hit");

			// Mythril
			if (item.type == ItemID.MythrilSword || item.type == ItemID.MythrilHalberd)
				EditTooltipByName("Knockback", (line) => line.text += "\nDecreases enemy contact damage by 10% on hit");

			// Orichalcum
			if (item.type == ItemID.OrichalcumSword || item.type == ItemID.OrichalcumHalberd)
				EditTooltipByName("Knockback", (line) => line.text += "\nIncreases how frequently the Orichalcum set bonus triggers on hit");

			// Adamantite
			if (item.type == ItemID.AdamantiteSword || item.type == ItemID.AdamantiteGlaive)
				EditTooltipByName("Knockback", (line) => line.text += "\nSlows enemies on hit");

			// Titanium
			if (item.type == ItemID.TitaniumSword || item.type == ItemID.TitaniumTrident)
				EditTooltipByName("Knockback", (line) => line.text += "\nDeals increased damage to enemies with high knockback resistance");

			// Hallowed (and True Excalibur)
			if (item.type == ItemID.Excalibur || item.type == ItemID.Gungnir || item.type == ItemID.TrueExcalibur)
				EditTooltipByName("Knockback", (line) => line.text += "\nInflicts Holy Flames\nDeals double damage to enemies above 75% life");
			#endregion

			// Other melee weapon tooltips
			#region Other Melee Tooltips

			if (item.type == ItemID.CandyCaneSword || item.type == ItemID.FruitcakeChakram)
				EditTooltipByName("Knockback", (line) => line.text += "\nHeals you on hit");

			// Stylish Scissors, all Phaseblades, and all Phasesabers
			if (item.type == ItemID.StylistKilLaKillScissorsIWish || (item.type >= ItemID.BluePhaseblade && item.type <= ItemID.YellowPhaseblade) || (item.type >= ItemID.BluePhasesaber && item.type <= ItemID.YellowPhasesaber))
				EditTooltipByName("Knockback", (line) => line.text += "\nIgnores 100% of enemy defense");

			if (item.type == ItemID.AntlionClaw || item.type == ItemID.BoneSword || item.type == ItemID.BreakerBlade)
				EditTooltipByName("Knockback", (line) => line.text += "\nIgnores 50% of enemy defense");

			if (item.type == ItemID.LightsBane || item.type == ItemID.NightsEdge || item.type == ItemID.TrueNightsEdge)
				EditTooltipByName("Knockback", (line) => line.text += "\nInflicts Shadowflame on hit");

			if (item.type == ItemID.BloodButcherer || item.type == ItemID.TheRottedFork)
				EditTooltipByName("Knockback", (line) => line.text += "\nInflicts Burning Blood on hit");
			#endregion

			// Light pets, accessories, and other items which boost the player's Abyss light stat
			#region Abyss Light Tooltips

			// +1 to Abyss light level
			string abyssSmallLightLine = "\nProvides a small amount of light in the abyss";

			if (item.type == ItemID.CrimsonHeart || item.type == ItemID.ShadowOrb || item.type == ItemID.MagicLantern || item.type == ItemID.JellyfishNecklace || item.type == ItemID.MiningHelmet)
				EditTooltipByNum(0, (line) => line.text += abyssSmallLightLine);
			if (item.type == ItemID.JellyfishDivingGear)
				EditTooltipByNum(1, (line) => line.text += abyssSmallLightLine);

			// +2 to Abyss light level
			string abyssMediumLightLine = "\nProvides a moderate amount of light in the abyss";

			if (item.type == ItemID.ShinePotion)
				EditTooltipByName("BuffTime", (line) => line.text += abyssMediumLightLine);

			if (item.type == ItemID.FairyBell || item.type == ItemID.DD2PetGhost)
				EditTooltipByNum(0, (line) => line.text += abyssMediumLightLine);

			// +3 to Abyss light level
			string abyssLargeLightLine = "\nProvides a large amount of light in the abyss";

			if (item.type == ItemID.WispinaBottle)
				EditTooltipByNum(0, (line) => line.text += abyssLargeLightLine);
			if (item.type == ItemID.SuspiciousLookingTentacle)
				EditTooltipByNum(1, (line) => line.text += abyssLargeLightLine);
			#endregion

			// Accessories and other items which boost the player's ability to breathe in the Abyss
			#region Abyss Breath Tooltips

			// Moderate breath boost
			string abyssModerateBreathLine = "\nModerately reduces breath loss in the abyss";

			if (item.type == ItemID.DivingHelmet)
				EditTooltipByNum(0, (line) => line.text += abyssModerateBreathLine);
			if (item.type == ItemID.ArcticDivingGear)
				EditTooltipByNum(1, (line) => line.text += abyssSmallLightLine + abyssModerateBreathLine);

			// Great breath boost
			string abyssGreatBreathLine = "\nGreatly reduces breath loss in the abyss";

			if (item.type == ItemID.GillsPotion)
				EditTooltipByName("BuffTime", (line) => line.text += abyssGreatBreathLine);

			if (item.type == ItemID.NeptunesShell || item.type == ItemID.MoonShell)
				EditTooltipByNum(0, (line) => line.text += abyssGreatBreathLine);
			if (item.type == ItemID.CelestialShell)
				EditTooltipByNum(1, (line) => line.text += abyssModerateBreathLine);
			#endregion

			// Flasks apply to Rogue weapons
			#region Rogue Flask Tooltips
			if (item.type == ItemID.FlaskofCursedFlames)
				EditTooltipByNum(0, (line) => line.text += "\nRogue attacks inflict enemies with cursed flames");
			if (item.type == ItemID.FlaskofFire)
				EditTooltipByNum(0, (line) => line.text += "\nRogue attacks set enemies on fire");
			if (item.type == ItemID.FlaskofGold)
				EditTooltipByNum(0, (line) => line.text += "\nRogue attacks make enemies drop more gold");
			if (item.type == ItemID.FlaskofIchor)
				EditTooltipByNum(0, (line) => line.text += "\nRogue attacks decrease enemy defense");
			if (item.type == ItemID.FlaskofNanites)
				EditTooltipByNum(0, (line) => line.text += "\nRogue attacks confuse enemies");
			// party flask is unique because it affects ALL projectiles in Calamity, not just "also rogue ones"
			if (item.type == ItemID.FlaskofParty)
				EditTooltipByNum(0, (line) => line.text = "All attacks cause confetti to appear");
			if (item.type == ItemID.FlaskofPoison)
				EditTooltipByNum(0, (line) => line.text += "\nRogue attacks poison enemies");
			if (item.type == ItemID.FlaskofVenom)
				EditTooltipByNum(0, (line) => line.text += "\nRogue attacks inflict Venom on enemies");
			#endregion

			// Rebalances to vanilla item stats
			#region Vanilla Item Rebalance Tooltips

			// Worm Scarf only gives 10% DR instead of 17%
			if (item.type == ItemID.WormScarf)
				EditTooltipByNum(0, (line) => line.text = line.text = line.text.Replace("17%", "10%"));

			if (item.type == ItemID.TitanGlove)
				EditTooltipByNum(0, (line) => line.text += "\n10% increased true melee damage");
			if (item.type == ItemID.PowerGlove || item.type == ItemID.MechanicalGlove)
				EditTooltipByNum(1, (line) => line.text += "\n10% increased true melee damage");
			if (item.type == ItemID.FireGauntlet)
			{
				string extraLine = "\n10% increased true melee damage";
				if (CalamityWorld.death)
					extraLine += "\nProvides heat and cold protection in Death Mode";
				EditTooltipByNum(1, (line) => line.text = line.text.Replace("12%", "14%") + extraLine);
			}

			// Spectre Hood's lifesteal is heavily nerfed, so it only reduces magic damage by 20% instead of 40%
			if (item.type == ItemID.SpectreHood)
				EditTooltipByNum(0, (line) => line.text = line.text.Replace("40%", "20%"));
			#endregion

			// Items which provide immunity to either heat or cold in Death Mode
			#region Death Mode Environmental Immunity Tooltips
			if (item.type == ItemID.ObsidianSkinPotion)
			{
				string heatImmunity = CalamityWorld.death ? "\nProvides heat protection in Death Mode" : "";
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Provides immunity to direct damage from touching lava\n" +
							"Provides temporary immunity to lava burn damage\n" +
							"Greatly increases lava immunity time regeneration\n" +
							"Reduces lava burn damage" + heatImmunity;
					}
				}
			}
			if (item.type == ItemID.ObsidianRose)
			{
				string heatImmunity = CalamityWorld.death ? "\nProvides heat protection in Death Mode" : "";
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Reduced direct damage from touching lava\n" +
							"Greatly reduces lava burn damage" + heatImmunity;
					}
				}
			}
			if (item.type == ItemID.MagmaStone && CalamityWorld.death)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text += "\nProvides heat and cold protection in Death Mode";
					}
				}
			}
			if (item.type == ItemID.LavaCharm && CalamityWorld.death)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text += "\nProvides heat protection in Death Mode";
					}
				}
			}
			if (item.type == ItemID.LavaWaders && CalamityWorld.death)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
					{
						line2.text += "\nProvides heat protection in Death Mode";
					}
				}
			}
			#endregion

			// Add mentions of what Calamity ores vanilla pickaxes can mine
			#region Pickaxe New Ore Tooltips
			if (item.type == ItemID.Picksaw)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Capable of mining Lihzahrd Bricks and Scoria Ore";
					}
				}
			}
			if (item.type == ItemID.SolarFlarePickaxe)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Material")
					{
						line2.text = "Material\n" +
							"Can mine Uelibloom Ore";
					}
				}
			}
			if (item.type == ItemID.VortexPickaxe)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Material")
					{
						line2.text = "Material\n" +
							"Can mine Uelibloom Ore";
					}
				}
			}
			if (item.type == ItemID.NebulaPickaxe)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Material")
					{
						line2.text = "Material\n" +
							"Can mine Uelibloom Ore";
					}
				}
			}
			if (item.type == ItemID.StardustPickaxe)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Material")
					{
						line2.text = "Material\n" +
							"Can mine Uelibloom Ore";
					}
				}
			}
			#endregion

			// Rebalances and information about vanilla set bonuses
			#region Vanilla Set Bonus Tooltips
			if (item.type == ItemID.MeteorHelmet || item.type == ItemID.MeteorSuit || item.type == ItemID.MeteorLeggings)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "SetBonus")
					{
						line2.text = "Set Bonus: Reduces the mana cost of the Space Gun by 50%";
					}
				}
			}
			if (item.type == ItemID.CopperHelmet || item.type == ItemID.CopperChainmail || item.type == ItemID.CopperGreaves)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "SetBonus")
					{
						line2.text = "Set Bonus: +2 defense and 15% increased mining speed";
					}
				}
			}
			if (item.type == ItemID.TinHelmet || item.type == ItemID.TinChainmail || item.type == ItemID.TinGreaves)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "SetBonus")
					{
						line2.text = "Set Bonus: +2 defense and 10% increased mining speed";
					}
				}
			}
			if (item.type == ItemID.AncientIronHelmet || item.type == ItemID.IronHelmet || item.type == ItemID.IronChainmail || item.type == ItemID.IronGreaves)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "SetBonus")
					{
						line2.text = "Set Bonus: +2 defense and 25% increased mining speed";
					}
				}
			}
			if (item.type == ItemID.LeadHelmet || item.type == ItemID.LeadChainmail || item.type == ItemID.LeadGreaves)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "SetBonus")
					{
						line2.text = "Set Bonus: +3 defense and 20% increased mining speed";
					}
				}
			}
			if (item.type == ItemID.SilverHelmet || item.type == ItemID.SilverChainmail || item.type == ItemID.SilverGreaves)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "SetBonus")
					{
						line2.text = "Set Bonus: +3 defense and 35% increased mining speed";
					}
				}
			}
			if (item.type == ItemID.TungstenHelmet || item.type == ItemID.TungstenChainmail || item.type == ItemID.TungstenGreaves)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "SetBonus")
					{
						line2.text = "Set Bonus: +3 defense and 30% increased mining speed";
					}
				}
			}
			if (item.type == ItemID.AncientGoldHelmet || item.type == ItemID.GoldHelmet || item.type == ItemID.GoldChainmail || item.type == ItemID.GoldGreaves)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "SetBonus")
					{
						line2.text = "Set Bonus: +3 defense and 45% increased mining speed";
					}
				}
			}
			if (item.type == ItemID.PlatinumHelmet || item.type == ItemID.PlatinumChainmail || item.type == ItemID.PlatinumGreaves)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "SetBonus")
					{
						line2.text = "Set Bonus: +4 defense and 40% increased mining speed";
					}
				}
			}
			if (item.type == ItemID.AncientBattleArmorHat || item.type == ItemID.AncientBattleArmorShirt || item.type == ItemID.AncientBattleArmorPants)
			{
				if (!Main.player[Main.myPlayer].Calamity().forbiddenCirclet)
				{
					foreach (TooltipLine line2 in tooltips)
					{
						if (line2.mod == "Terraria" && line2.Name == "SetBonus")
						{
							line2.text += "\nThe minion damage nerf is reduced while wielding magic weapons";
						}
					}
				}
			}
			if (item.type == ItemID.StardustBreastplate || item.type == ItemID.StardustLeggings)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Increases your max number of minions";
					}
				}
			}
			if (item.type == ItemID.StardustHelmet || item.type == ItemID.StardustBreastplate || item.type == ItemID.StardustLeggings)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "SetBonus")
					{
						line2.text += "\nIncreases your max number of minions by 2";
					}
				}
			}
			if (item.type == ItemID.GladiatorHelmet)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Defense")
					{
						line2.text = "3 defense\n" +
							"3% increased rogue damage";
					}
				}
			}
			if (item.type == ItemID.GladiatorBreastplate)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Defense")
					{
						line2.text = "5 defense\n" +
							"3% increased rogue critical strike chance";
					}
				}
			}
			if (item.type == ItemID.GladiatorLeggings)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Defense")
					{
						line2.text = "4 defense\n" +
							"3% increased rogue velocity";
					}
				}
			}
			if (item.type == ItemID.ObsidianHelm)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Defense")
					{
						line2.text = "4 defense\n" +
							"3% increased rogue damage";
					}
				}
			}
			if (item.type == ItemID.ObsidianShirt)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Defense")
					{
						line2.text = "5 defense\n" +
							"3% increased rogue critical strike chance";
					}
				}
			}
			if (item.type == ItemID.ObsidianPants)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Defense")
					{
						line2.text = "4 defense\n" +
							"3% increased rogue velocity";
					}
				}
			}
			if (item.type == ItemID.MoltenHelmet || item.type == ItemID.MoltenBreastplate || item.type == ItemID.MoltenGreaves)
			{
				if (CalamityWorld.death)
				{
					foreach (TooltipLine line2 in tooltips)
					{
						if (line2.mod == "Terraria" && line2.Name == "SetBonus")
						{
							line2.text = @"Set Bonus: 17% extra melee damage
20% extra true melee damage
Grants immunity to fire blocks, and temporary immunity to lava
Provides heat and cold protection in Death Mode";
						}
					}
				}
				else
				{
					foreach (TooltipLine line2 in tooltips)
					{
						if (line2.mod == "Terraria" && line2.Name == "SetBonus")
						{
							line2.text = @"Set Bonus: 17% extra melee damage
20% extra true melee damage
Grants immunity to fire blocks, and temporary immunity to lava";
						}
					}
				}
			}
			if (item.type == ItemID.FrostHelmet || item.type == ItemID.FrostBreastplate || item.type == ItemID.FrostLeggings)
			{
				if (CalamityWorld.death)
				{
					foreach (TooltipLine line2 in tooltips)
					{
						if (line2.mod == "Terraria" && line2.Name == "SetBonus")
						{
							line2.text += "\nProvides heat and cold protection in Death Mode";
						}
					}
				}
			}
			#endregion

			// Provide the full, raw stats of every vanilla set of wings
			#region Wing Stat Tooltips
			if (item.type == ItemID.AngelWings)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Allows flight and slow fall\n" +
							"Horizontal speed: 6.25\n" +
							"Acceleration multiplier: 1\n" +
							"Average vertical speed\n" +
							"Flight time: 100\n" +
							"+20 max life, +10 defense and +2 life regen";
					}
				}
			}
			if (item.type == ItemID.DemonWings)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Allows flight and slow fall\n" +
							"Horizontal speed: 6.25\n" +
							"Acceleration multiplier: 1\n" +
							"Average vertical speed\n" +
							"Flight time: 100\n" +
							"5% increased damage and critical strike chance";
					}
				}
			}
			if (item.type == ItemID.Jetpack)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Allows flight and slow fall\n" +
							"Horizontal speed: 6.5\n" +
							"Acceleration multiplier: 1\n" +
							"Average vertical speed\n" +
							"Flight time: 115";
					}
				}
			}
			if (item.type == ItemID.ButterflyWings)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Allows flight and slow fall\n" +
							"Horizontal speed: 6.75\n" +
							"Acceleration multiplier: 1\n" +
							"Average vertical speed\n" +
							"Flight time: 130\n" +
							"+20 max mana, 5% decreased mana usage,\n" +
							"5% increased magic damage and magic critical strike chance";
					}
				}
			}
			if (item.type == ItemID.FairyWings)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Allows flight and slow fall\n" +
							"Horizontal speed: 6.75\n" +
							"Acceleration multiplier: 1\n" +
							"Average vertical speed\n" +
							"Flight time: 130\n" +
							"+60 max life";
					}
				}
			}
			if (item.type == ItemID.BeeWings)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Allows flight and slow fall\n" +
							"Horizontal speed: 6.75\n" +
							"Acceleration multiplier: 1\n" +
							"Average vertical speed\n" +
							"Flight time: 130\n" +
							"Honey buff at all times";
					}
				}
			}
			if (item.type == ItemID.HarpyWings)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Allows flight and slow fall\n" +
							"Horizontal speed: 7\n" +
							"Acceleration multiplier: 1\n" +
							"Average vertical speed\n" +
							"Flight time: 140\n" +
							"20% increased movement speed\n" +
							"Most attacks have a chance to fire a feather on swing if Harpy Ring or Angel Treads are equipped";
					}
				}
			}
			if (item.type == ItemID.BoneWings)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Allows flight and slow fall\n" +
							"Horizontal speed: 7\n" +
							"Acceleration multiplier: 1\n" +
							"Average vertical speed\n" +
							"Flight time: 140\n" +
							"10% increased movement speed, ranged damage and critical strike chance\n" +
							"and +30 defense while wearing the Necro Armor";
					}
				}
			}
			if (item.type == ItemID.FlameWings)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Allows flight and slow fall\n" +
							"Horizontal speed: 7.5\n" +
							"Acceleration multiplier: 1\n" +
							"Average vertical speed\n" +
							"Flight time: 160\n" +
							"5% increased melee damage and critical strike chance";
					}
				}
			}
			if (item.type == ItemID.FrozenWings)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Allows flight and slow fall\n" +
							"Horizontal speed: 7.5\n" +
							"Acceleration multiplier: 1\n" +
							"Average vertical speed\n" +
							"Flight time: 160\n" +
							"2% increased melee and ranged damage\n" +
							"and 1% increased melee and ranged critical strike chance\n" +
							"while wearing the Frost Armor";
					}
				}
			}
			if (item.type == ItemID.GhostWings)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Allows flight and slow fall\n" +
							"Horizontal speed: 7.5\n" +
							"Acceleration multiplier: 1\n" +
							"Average vertical speed\n" +
							"Flight time: 160\n" +
							"+10 defense and 5% increased damage reduction while wearing the Spectre Armor and Hood\n" +
							"5% increased magic damage and critical strike chance while wearing the Spectre Armor and Mask";
					}
				}
			}
			if (item.type == ItemID.BeetleWings)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Allows flight and slow fall\n" +
							"Horizontal speed: 7.5\n" +
							"Acceleration multiplier: 1\n" +
							"Average vertical speed\n" +
							"Flight time: 160\n" +
							"+10 defense and 5% increased damage reduction while wearing the Beetle Armor and Shell\n" +
							"5% increased melee damage and critical strike chance while wearing the Beetle Armor and Scale Mail";
					}
				}
			}
			if (item.type == ItemID.FinWings)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Allows flight and slow fall\n" +
							"Horizontal speed: 0\n" +
							"Acceleration multiplier: 0\n" +
							"Average vertical speed\n" +
							"Flight time: 100\n" +
							"Gills effect and you can move freely through liquids\n" +
							"You fall faster while submerged in liquid\n" +
							"15% increased movement speed and 18% increased jump speed";
					}
				}
			}
			if (item.type == ItemID.FishronWings)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Allows flight and slow fall\n" +
							"Horizontal speed: 8\n" +
							"Acceleration multiplier: 2\n" +
							"Good vertical speed\n" +
							"Flight time: 180";
					}
				}
			}
			if (item.type == ItemID.SteampunkWings)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Allows flight and slow fall\n" +
							"Horizontal speed: 7.75\n" +
							"Acceleration multiplier: 1\n" +
							"Average vertical speed\n" +
							"Flight time: 180\n" +
							"+8 defense, 10% increased movement speed,\n" +
							"4% increased damage, and 2% increased critical strike chance";
					}
				}
			}
			if (item.type == ItemID.LeafWings)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Allows flight and slow fall\n" +
							"Horizontal speed: 6.75\n" +
							"Acceleration multiplier: 1\n" +
							"Average vertical speed\n" +
							"Flight time: 160\n" +
							"+5 defense, 5% increased damage reduction,\n" +
							"and the Dryad's permanent blessing while wearing the Tiki Armor";
					}
				}
			}
			if (item.type == ItemID.BatWings)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Allows flight and slow fall\n" +
							"Horizontal speed: 0\n" +
							"Acceleration multiplier: 0\n" +
							"Average vertical speed\n" +
							"Flight time: 140\n" +
							"At night or during an eclipse, you will gain the following boosts:\n" +
							"10% increased movement speed, 10% increased jump speed,\n" +
							"7% increased damage and 3% increased critical strike chance";
					}
				}
			}
			if (item.type == ItemID.Yoraiz0rWings || item.type == ItemID.JimsWings || item.type == ItemID.SkiphsWings ||
				item.type == ItemID.LokisWings || item.type == ItemID.ArkhalisWings || item.type == ItemID.LeinforsWings ||
				item.type == ItemID.BejeweledValkyrieWing || item.type == ItemID.RedsWings || item.type == ItemID.DTownsWings ||
				item.type == ItemID.WillsWings || item.type == ItemID.CrownosWings || item.type == ItemID.CenxsWings)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "'Great for impersonating devs!'\n" +
							"Horizontal speed: 7\n" +
							"Acceleration multiplier: 1\n" +
							"Average vertical speed\n" +
							"Flight time: 150";
					}
				}
			}
			if (item.type == ItemID.TatteredFairyWings)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Allows flight and slow fall\n" +
							"Horizontal speed: 7.5\n" +
							"Acceleration multiplier: 1\n" +
							"Average vertical speed\n" +
							"Flight time: 180\n" +
							"5% increased damage and critical strike chance";
					}
				}
			}
			if (item.type == ItemID.SpookyWings)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Allows flight and slow fall\n" +
							"Horizontal speed: 7.5\n" +
							"Acceleration multiplier: 1\n" +
							"Average vertical speed\n" +
							"Flight time: 180\n" +
							"Increased minion knockback and 5% increased minion damage while wearing the Spooky Armor";
					}
				}
			}
			if (item.type == ItemID.Hoverboard)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
					{
						line2.text = "Hold DOWN and JUMP to hover\n" +
							"Horizontal speed: 6.25\n" +
							"Acceleration multiplier: 1\n" +
							"Average vertical speed\n" +
							"Flight time: 170\n" +
							"10% increased damage to bows, guns, rocket launchers, and flamethrowers while wearing the Shroomite Armor\n" +
							"Boosted weapon type depends on the Shroomite Helmet worn";
					}
				}
			}
			if (item.type == ItemID.FestiveWings)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Allows flight and slow fall\n" +
							"Horizontal speed: 7.5\n" +
							"Acceleration multiplier: 1\n" +
							"Average vertical speed\n" +
							"Flight time: 170\n" +
							"+40 max life\n" +
							"Ornaments rain down as you fly";
					}
				}
			}
			if (item.type == ItemID.MothronWings)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Allows flight and slow fall\n" +
							"Horizontal speed: 0\n" +
							"Acceleration multiplier: 0\n" +
							"Average vertical speed\n" +
							"Flight time: 160\n" +
							"+5 defense, 5% increased damage,\n" +
							"10% increased movement speed and 12% increased jump speed";
					}
				}
			}
			if (item.type == ItemID.WingsSolar)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Allows flight and slow fall\n" +
							"Horizontal speed: 9\n" +
							"Acceleration multiplier: 2.5\n" +
							"Great vertical speed\n" +
							"Flight time: 180\n" +
							"7% increased melee damage and 3% increased melee critical strike chance\n" +
							"while wearing the Solar Flare Armor";
					}
				}
			}
			if (item.type == ItemID.WingsStardust)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Allows flight and slow fall\n" +
							"Horizontal speed: 9\n" +
							"Acceleration multiplier: 2.5\n" +
							"Great vertical speed\n" +
							"Flight time: 180\n" +
							"+1 max minion and 5% increased minion damage while wearing the Stardust Armor";
					}
				}
			}
			if (item.type == ItemID.WingsVortex)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Allows flight and slow fall\n" +
							"Hold DOWN and JUMP to hover\n" +
							"Horizontal speed: 6.5\n" +
							"Acceleration multiplier: 1.5\n" +
							"Good vertical speed\n" +
							"Flight time: 160\n" +
							"3% increased ranged damage and 7% increased ranged critical strike chance\n" +
							"while wearing the Vortex Armor";
					}
				}
			}
			if (item.type == ItemID.WingsNebula)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Allows flight and slow fall\n" +
							"Hold DOWN and JUMP to hover\n" +
							"Horizontal speed: 6.5\n" +
							"Acceleration multiplier: 1.5\n" +
							"Good vertical speed\n" +
							"Flight time: 160\n" +
							"+20 max mana, 5% increased magic damage and critical strike chance,\n" +
							"and 5% decreased mana usage while wearing the Nebula Armor";
					}
				}
			}
			if (item.type == ItemID.BetsyWings)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Equipable")
					{
						line2.text = "Equipable\n" +
							"Allows flight and slow fall\n" +
							"Hold DOWN and JUMP to hover\n" +
							"Horizontal speed: 6\n" +
							"Acceleration multiplier: 2.5\n" +
							"Good vertical speed\n" +
							"Flight time: 150";
					}
				}
			}
			#endregion

			#region Grappling Hook Stat Tooltips
			if (item.type == ItemID.GrapplingHook)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Equipable")
					{
						line2.text = "Equipable\n" +
							"Reach: 18.75\n" +
							"Launch Velocity: 11.5\n" +
							"Pull Velocity: 11";
					}
				}
			}
			if (item.type == ItemID.AmethystHook)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Equipable")
					{
						line2.text = "Equipable\n" +
							"Reach: 18.75\n" +
							"Launch Velocity: 10\n" +
							"Pull Velocity: 11";
					}
				}
			}
			if (item.type == ItemID.TopazHook)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Equipable")
					{
						line2.text = "Equipable\n" +
							"Reach: 20.625\n" +
							"Launch Velocity: 10.5\n" +
							"Pull Velocity: 11.75";
					}
				}
			}
			if (item.type == ItemID.SapphireHook)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Equipable")
					{
						line2.text = "Equipable\n" +
							"Reach: 22.5\n" +
							"Launch Velocity: 11\n" +
							"Pull Velocity: 12.5";
					}
				}
			}
			if (item.type == ItemID.EmeraldHook)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Equipable")
					{
						line2.text = "Equipable\n" +
							"Reach: 24.375\n" +
							"Launch Velocity: 11.5\n" +
							"Pull Velocity: 13.25";
					}
				}
			}
			if (item.type == ItemID.RubyHook)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Equipable")
					{
						line2.text = "Equipable\n" +
							"Reach: 26.25\n" +
							"Launch Velocity: 12\n" +
							"Pull Velocity: 14";
					}
				}
			}
			if (item.type == ItemID.DiamondHook)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Equipable")
					{
						line2.text = "Equipable\n" +
							"Reach: 28.125\n" +
							"Launch Velocity: 12.5\n" +
							"Pull Velocity: 14.75";
					}
				}
			}
			if (item.type == ItemID.WebSlinger)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Equipable")
					{
						line2.text = "Equipable\n" +
							"Reach: 15.625\n" +
							"Launch Velocity: 10\n" +
							"Pull Velocity: 11";
					}
				}
			}
			if (item.type == ItemID.SkeletronHand)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Equipable")
					{
						line2.text = "Equipable\n" +
							"Reach: 21.875\n" +
							"Launch Velocity: 15\n" +
							"Pull Velocity: 11";
					}
				}
			}
			if (item.type == ItemID.SlimeHook)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Equipable")
					{
						line2.text = "Equipable\n" +
							"Reach: 18.75\n" +
							"Launch Velocity: 13\n" +
							"Pull Velocity: 11";
					}
				}
			}
			if (item.type == ItemID.FishHook)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Equipable")
					{
						line2.text = "Equipable\n" +
							"Reach: 25\n" +
							"Launch Velocity: 13\n" +
							"Pull Velocity: 11";
					}
				}
			}
			if (item.type == ItemID.IvyWhip)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Equipable")
					{
						line2.text = "Equipable\n" +
							"Reach: 25\n" +
							"Launch Velocity: 13\n" +
							"Pull Velocity: 15";
					}
				}
			}
			if (item.type == ItemID.BatHook)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Equipable")
					{
						line2.text = "Equipable\n" +
							"Reach: 31.25\n" +
							"Launch Velocity: 15.5\n" +
							"Pull Velocity: 20";
					}
				}
			}
			if (item.type == ItemID.CandyCaneHook)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Equipable")
					{
						line2.text = "Equipable\n" +
							"Reach: 25\n" +
							"Launch Velocity: 11.5\n" +
							"Pull Velocity: 11";
					}
				}
			}
			if (item.type == ItemID.DualHook)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Equipable")
					{
						line2.text = "Equipable\n" +
							"Reach: 27.5\n" +
							"Launch Velocity: 14\n" +
							"Pull Velocity: 17";
					}
				}
			}
			if (item.type == ItemID.ThornHook)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Equipable")
					{
						line2.text = "Equipable\n" +
							"Reach: 30\n" +
							"Launch Velocity: 15\n" +
							"Pull Velocity: 18";
					}
				}
			}
			if (item.type == ItemID.WormHook)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Equipable")
					{
						line2.text = "Equipable\n" +
							"Reach: 30\n" +
							"Launch Velocity: 15\n" +
							"Pull Velocity: 18";
					}
				}
			}
			if (item.type == ItemID.TendonHook)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Equipable")
					{
						line2.text = "Equipable\n" +
							"Reach: 30\n" +
							"Launch Velocity: 15\n" +
							"Pull Velocity: 18";
					}
				}
			}
			if (item.type == ItemID.IlluminantHook)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Equipable")
					{
						line2.text = "Equipable\n" +
							"Reach: 30\n" +
							"Launch Velocity: 15\n" +
							"Pull Velocity: 18";
					}
				}
			}
			if (item.type == ItemID.AntiGravityHook)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Equipable")
					{
						line2.text = "Equipable\n" +
							"Reach: 31.25\n" +
							"Launch Velocity: 14\n" +
							"Pull Velocity: 20";
					}
				}
			}
			if (item.type == ItemID.SpookyHook)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Equipable")
					{
						line2.text = "Equipable\n" +
							"Reach: 34.375\n" +
							"Launch Velocity: 15.5\n" +
							"Pull Velocity: 22";
					}
				}
			}
			if (item.type == ItemID.ChristmasHook)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Equipable")
					{
						line2.text = "Equipable\n" +
							"Reach: 34.375\n" +
							"Launch Velocity: 15.5\n" +
							"Pull Velocity: 17";
					}
				}
			}
			if (item.type == ItemID.LunarHook)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Equipable")
					{
						line2.text = "Equipable\n" +
							"Reach: 34.375\n" +
							"Launch Velocity: 16\n" +
							"Pull Velocity: 24";
					}
				}
			}
			if (item.type == ItemID.StaticHook)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Equipable")
					{
						line2.text = "Equipable\n" +
							"Reach: 37.5\n" +
							"Launch Velocity: 16\n" +
							"Pull Velocity: 24";
					}
				}
			}
			if (item.type == ItemID.StaticHook)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Equipable")
					{
						line2.text = "Equipable\n" +
							"Reach: 37.5\n" +
							"Launch Velocity: 16\n" +
							"Pull Velocity: 24";
					}
				}
			}
			#endregion

			#region Accessory Prefix Rebalance Tooltips
			if (item.accessory)
			{
				if (item.prefix == PrefixID.Brisk)
				{
					foreach (TooltipLine line2 in tooltips)
					{
						if (line2.mod == "Terraria" && line2.Name == "PrefixAccMoveSpeed")
							line2.text = "+2% movement speed";
					}
				}
				if (item.prefix == PrefixID.Fleeting)
				{
					foreach (TooltipLine line2 in tooltips)
					{
						if (line2.mod == "Terraria" && line2.Name == "PrefixAccMoveSpeed")
							line2.text = "+4% movement speed";
					}
				}
				if (item.prefix == PrefixID.Hasty2)
				{
					foreach (TooltipLine line2 in tooltips)
					{
						if (line2.mod == "Terraria" && line2.Name == "PrefixAccMoveSpeed")
							line2.text = "+6% movement speed";
					}
				}
				if (item.prefix == PrefixID.Quick2)
				{
					foreach (TooltipLine line2 in tooltips)
					{
						if (line2.mod == "Terraria" && line2.Name == "PrefixAccMoveSpeed")
							line2.text = "+8% movement speed";
					}
				}
				if (item.prefix == PrefixID.Precise || item.prefix == PrefixID.Lucky)
				{
					foreach (TooltipLine line2 in tooltips)
					{
						if (line2.mod == "Terraria" && line2.Name == "PrefixAccCritChance")
							line2.text += "\n+1 armor penetration";
					}
				}
				if (item.prefix == PrefixID.Hard)
				{
					string defenseBoost = "+1 defense\n";
					if (CalamityWorld.downedDoG)
						defenseBoost = "+4 defense\n";
					else if (CalamityWorld.downedProvidence || CalamityWorld.downedPolterghast)
						defenseBoost = "+3 defense\n";
					else if (NPC.downedGolemBoss || NPC.downedMoonlord)
						defenseBoost = "+2 defense\n";

					foreach (TooltipLine line2 in tooltips)
					{
						if (line2.mod == "Terraria" && line2.Name == "PrefixAccDefense")
							line2.text = defenseBoost + "+0.25% damage reduction";
					}
				}
				if (item.prefix == PrefixID.Guarding)
				{
					string defenseBoost = "+2 defense\n";
					if (CalamityWorld.downedDoG)
						defenseBoost = "+8 defense\n";
					else if (CalamityWorld.downedPolterghast)
						defenseBoost = "+7 defense\n";
					else if (CalamityWorld.downedProvidence)
						defenseBoost = "+6 defense\n";
					else if (NPC.downedMoonlord)
						defenseBoost = "+5 defense\n";
					else if (NPC.downedGolemBoss)
						defenseBoost = "+4 defense\n";
					else if (Main.hardMode)
						defenseBoost = "+3 defense\n";

					foreach (TooltipLine line2 in tooltips)
					{
						if (line2.mod == "Terraria" && line2.Name == "PrefixAccDefense")
							line2.text = defenseBoost + "+0.5% damage reduction";
					}
				}
				if (item.prefix == PrefixID.Armored)
				{
					string defenseBoost = "+3 defense\n";
					if (CalamityWorld.downedDoG)
						defenseBoost = "+12 defense\n";
					else if (CalamityWorld.downedPolterghast)
						defenseBoost = "+10 defense\n";
					else if (CalamityWorld.downedProvidence)
						defenseBoost = "+9 defense\n";
					else if (NPC.downedMoonlord)
						defenseBoost = "+7 defense\n";
					else if (NPC.downedGolemBoss)
						defenseBoost = "+6 defense\n";
					else if (Main.hardMode)
						defenseBoost = "+4 defense\n";

					foreach (TooltipLine line2 in tooltips)
					{
						if (line2.mod == "Terraria" && line2.Name == "PrefixAccDefense")
							line2.text = defenseBoost + "+0.75% damage reduction";
					}
				}
				if (item.prefix == PrefixID.Warding)
				{
					string defenseBoost = "+4 defense\n";
					if (CalamityWorld.downedDoG)
						defenseBoost = "+16 defense\n";
					else if (CalamityWorld.downedPolterghast)
						defenseBoost = "+14 defense\n";
					else if (CalamityWorld.downedProvidence)
						defenseBoost = "+12 defense\n";
					else if (NPC.downedMoonlord)
						defenseBoost = "+10 defense\n";
					else if (NPC.downedGolemBoss)
						defenseBoost = "+8 defense\n";
					else if (Main.hardMode)
						defenseBoost = "+6 defense\n";

					foreach (TooltipLine line2 in tooltips)
					{
						if (line2.mod == "Terraria" && line2.Name == "PrefixAccDefense")
							line2.text = defenseBoost + "+1% damage reduction";
					}
				}
			}
			#endregion
		}
		#endregion

		#region True Melee Damage Tooltip
		private void TrueMeleeDamageTooltip(Item item, IList<TooltipLine> tooltips)
		{
			TooltipLine line = tooltips.FirstOrDefault((l) => l.mod == "Terraria" && l.Name == "Damage");

			// If there somehow isn't a damage tooltip line, do not try to perform any edits.
			if (line is null)
				return;

			// Start with the existing line of melee damage.
			StringBuilder sb = new StringBuilder(64);
			sb.Append(line.text).Append(" : ");

			Player p = Main.LocalPlayer;
			float itemCurrentDamage = item.damage * p.MeleeDamage();
			double trueMeleeBoost = 1D + p.Calamity().trueMeleeDamage;
			double imprecisionRoundingCorrection = 5E-06D;
			int damageToDisplay = (int)(itemCurrentDamage * trueMeleeBoost + imprecisionRoundingCorrection);
			sb.Append(damageToDisplay);

			// These two pieces are split apart for ease of translation
			sb.Append(' ');
			sb.Append("true melee damage");
			line.text = sb.ToString();
		}
		#endregion

		#region Stealth Generation Prefix Accessory Tooltip
		private void StealthGenAccessoryTooltip(Item item, IList<TooltipLine> tooltips)
		{
			if (!item.accessory || item.social || item.prefix <= 0)
				return;

			float stealthGenBoost = item.Calamity().StealthGenBonus - 1f;
			if (stealthGenBoost > 0)
			{
				TooltipLine StealthGen = new TooltipLine(mod, "PrefixStealthGenBoost", "+" + Math.Round(stealthGenBoost * 100f) + "% stealth generation")
				{
					isModifier = true
				};
				tooltips.Add(StealthGen);
			}
		}
		#endregion

		#region Fargo Biome Fountain Tooltip
		private void FargoFountainTooltip(Item item, IList<TooltipLine> tooltips)
		{
			if (CalamityMod.Instance.fargos is null)
				return;

			if (item.type == ModContent.ItemType<SunkenSeaFountain>())
			{
				TooltipLine line = new TooltipLine(mod, "FargoFountain", "Forces surrounding biome state to Sunken Sea upon activation");
				tooltips.Add(line);
			}
			if (item.type == ModContent.ItemType<SulphurousFountainItem>())
			{
				TooltipLine line = new TooltipLine(mod, "FargoFountain", "Forces surrounding biome state to Sulphurous Sea upon activation");
				tooltips.Add(line);
			}
			if (item.type == ModContent.ItemType<AstralFountainItem>())
			{
				TooltipLine line = new TooltipLine(mod, "FargoFountain", "Forces surrounding biome state to Astral upon activation");
				tooltips.Add(line);
			}
		}
		#endregion
	}
}
