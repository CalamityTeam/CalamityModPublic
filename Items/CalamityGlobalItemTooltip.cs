using CalamityMod.Balancing;
using CalamityMod.CustomRecipes;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Demonshade;
using CalamityMod.Items.Tools;
using CalamityMod.Items.VanillaArmorChanges;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace CalamityMod.Items
{
    public partial class CalamityGlobalItem : GlobalItem
    {
        #region Main ModifyTooltips Function
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            // Apply rarity coloration to the item's name.
            TooltipLine nameLine = tooltips.FirstOrDefault(x => x.Name == "ItemName" && x.Mod == "Terraria");
            if (nameLine != null)
                ApplyRarityColor(item, nameLine);

            // Modify all vanilla tooltips before appending mod mechanics (if any).
            ModifyVanillaTooltips(item, tooltips);

            // If the item has a stealth generation prefix, show that on the tooltip.
            // This is placed between vanilla tooltip edits and mod mechanics because it can apply to vanilla items.
            StealthGenAccessoryTooltip(item, tooltips);

            // Adds "Does extra damage to enemies shot at point-blank range" to weapons capable of it.
            if (canFirePointBlankShots)
            {
                TooltipLine line = new TooltipLine(Mod, "PointBlankShot", "Does extra damage to enemies shot at point-blank range");
                tooltips.Add(line);
            }

            // If the item has a stealth strike damage prefix, show that on the tooltip.
            StealthWeaponTooltip(item, tooltips);

            // If an item has an enchantment, show its prefix in the first tooltip line and append its description to the
            // tooltip list.
            EnchantmentTooltips(item, tooltips);

            // Replace rogue with rouge in gfb
            if (Main.zenithWorld)
            {
                tooltips.FindAndReplace("Rogue", "Rouge");
                tooltips.FindAndReplace("rogue", "rouge");
            }
            // Everything below this line can only apply to modded items. If the item is vanilla, stop here for efficiency.
            if (item.type < ItemID.Count)
                return;

            // Adds a Current Charge tooltip to all items which use charge.
            CalamityGlobalItem modItem = item.Calamity();
            if (modItem?.UsesCharge ?? false)
            {
                // Convert current charge ratio into a percentage.
                float displayedPercent = ChargeRatio * 100f;
                TooltipLine line = new TooltipLine(Mod, "CalamityCharge", $"Current Charge: {displayedPercent:N1}%");
                tooltips.Add(line);
            }

            // Adds "Donor Item" and "Developer Item" to donor items and developer items respectively.
            if (donorItem)
            {
                TooltipLine line = new TooltipLine(Mod, "CalamityDonor", CalamityUtils.ColorMessage("- Donor Item -", CalamityUtils.DonatorItemColor));
                tooltips.Add(line);
            }
            if (devItem)
            {
                TooltipLine line = new TooltipLine(Mod, "CalamityDev", CalamityUtils.ColorMessage("- Developer Item -", CalamityUtils.DevItemColor));
                tooltips.Add(line);
            }
        }
        #endregion

        #region Rarity Coloration
        private void ApplyRarityColor(Item item, TooltipLine nameLine)
        {
            #region Uniquely Colored Developer Items
            if (item.type == ModContent.ItemType<Fabstaff>())
                nameLine.OverrideColor = new Color(Main.DiscoR, 100, 255);
            if (item.type == ModContent.ItemType<StaffofBlushie>())
                nameLine.OverrideColor = new Color(0, 0, 255);
            if (item.type == ModContent.ItemType<TheDanceofLight>())
                nameLine.OverrideColor = TheDanceofLight.GetSyncedLightColor();
            if (item.type == ModContent.ItemType<NanoblackReaper>())
                nameLine.OverrideColor = new Color(0.34f, 0.34f + 0.66f * Main.DiscoG / 255f, 0.34f + 0.5f * Main.DiscoG / 255f);
            if (item.type == ModContent.ItemType<ShatteredCommunity>())
                nameLine.OverrideColor = ShatteredCommunity.GetRarityColor();
            if (item.type == ModContent.ItemType<ProfanedSoulCrystal>())
                nameLine.OverrideColor = CalamityUtils.ColorSwap(new Color(255, 166, 0), new Color(25, 250, 25), 4f); //alternates between emerald green and amber (BanditHueh)
            if (item.type == ModContent.ItemType<TemporalUmbrella>())
                nameLine.OverrideColor = CalamityUtils.ColorSwap(new Color(210, 0, 255), new Color(255, 248, 24), 4f);
            if (item.type == ModContent.ItemType<Endogenesis>())
                nameLine.OverrideColor = CalamityUtils.ColorSwap(new Color(131, 239, 255), new Color(36, 55, 230), 4f);
            if (item.type == ModContent.ItemType<DraconicDestruction>())
                nameLine.OverrideColor = CalamityUtils.ColorSwap(new Color(255, 69, 0), new Color(139, 0, 0), 4f);
            if (item.type == ModContent.ItemType<ScarletDevil>())
                nameLine.OverrideColor = CalamityUtils.ColorSwap(new Color(191, 45, 71), new Color(185, 187, 253), 4f);
            if (item.type == ModContent.ItemType<RedSun>())
                nameLine.OverrideColor = CalamityUtils.ColorSwap(new Color(204, 86, 80), new Color(237, 69, 141), 4f);
            if (item.type == ModContent.ItemType<CrystylCrusher>())
                nameLine.OverrideColor = new Color(129, 29, 149);
            if (item.type == ModContent.ItemType<SomaPrime>())
                nameLine.OverrideColor = CalamityUtils.ColorSwap(new Color(255, 255, 255), new Color(0xD1, 0xCC, 0x6F), 4f);
            if (item.type == ModContent.ItemType<Svantechnical>())
                nameLine.OverrideColor = new Color(220, 20, 60);
            if (item.type == ModContent.ItemType<Contagion>())
                nameLine.OverrideColor = new Color(207, 17, 117);
            if (item.type == ModContent.ItemType<TriactisTruePaladinianMageHammerofMightMelee>())
                nameLine.OverrideColor = new Color(227, 226, 180);
            if (item.type == ModContent.ItemType<IllustriousKnives>())
                nameLine.OverrideColor = CalamityUtils.ColorSwap(new Color(154, 255, 151), new Color(228, 151, 255), 4f);
            if (item.type == ModContent.ItemType<DemonshadeHelm>() || item.type == ModContent.ItemType<DemonshadeBreastplate>() || item.type == ModContent.ItemType<DemonshadeGreaves>())
                nameLine.OverrideColor = CalamityUtils.ColorSwap(new Color(255, 132, 22), new Color(221, 85, 7), 4f);
            if (item.type == ModContent.ItemType<AngelicAlliance>())
            {
                nameLine.OverrideColor = CalamityUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly / 2f % 1f, new Color[]
                {
                    new Color(255, 196, 55),
                    new Color(255, 231, 107),
                    new Color(255, 254, 243)
                });
            }

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
                    int colorIndex = (int)(Main.GlobalTimeWrappedHourly / 2 % colorSet.Count);
                    Color currentColor = colorSet[colorIndex];
                    Color nextColor = colorSet[(colorIndex + 1) % colorSet.Count];
                    nameLine.OverrideColor = Color.Lerp(currentColor, nextColor, Main.GlobalTimeWrappedHourly % 2f > 1f ? 1f : Main.GlobalTimeWrappedHourly % 1f);
                }
            }
            if (item.type == ModContent.ItemType<FlamsteedRing>())
            {
                if (Main.GlobalTimeWrappedHourly % 1f < 0.6f)
                {
                    nameLine.OverrideColor = new Color(89, 229, 255);
                }
                else if (Main.GlobalTimeWrappedHourly % 1f < 0.8f)
                {
                    nameLine.OverrideColor = Color.Lerp(new Color(89, 229, 255), Color.White, (Main.GlobalTimeWrappedHourly % 1f - 0.6f) / 0.2f);
                }
                else
                {
                    nameLine.OverrideColor = Color.Lerp(Color.White, new Color(89, 229, 255), (Main.GlobalTimeWrappedHourly % 1f - 0.8f) / 0.2f);
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
                    int colorIndex = (int)(Main.GlobalTimeWrappedHourly / 2 % earthColors.Count);
                    Color currentColor = earthColors[colorIndex];
                    Color nextColor = earthColors[(colorIndex + 1) % earthColors.Count];
                    nameLine.OverrideColor = Color.Lerp(currentColor, nextColor, Main.GlobalTimeWrappedHourly % 2f > 1f ? 1f : Main.GlobalTimeWrappedHourly % 1f);
                }
            }
            #endregion
        }
        #endregion

        #region Enchantment Tooltips
        private void EnchantmentTooltips(Item item, IList<TooltipLine> tooltips)
        {
            if (!item.IsAir && AppliedEnchantment.HasValue)
            {
                foreach (string line in AppliedEnchantment.Value.Description.ToString().Split('\n'))
                {
                    TooltipLine descriptionLine = new TooltipLine(Mod, "Enchantment", CalamityUtils.ColorMessage(line, CalamityUtils.DonatorItemColor));
                    tooltips.Add(descriptionLine);
                }
            }
        }
        #endregion

        #region Vanilla Item Tooltip Modification

        // Turns a number into a string of increased mining speed.
        public static string MiningSpeedString(int percent) => $"\n{percent}% increased mining speed";

        private static void ModifyVanillaTooltips(Item item, IList<TooltipLine> tooltips)
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
            Func<Item, TooltipLine, bool> LineNum(int n) => (Item i, TooltipLine l) => l.Mod == "Terraria" && l.Name == $"Tooltip{n}";
            // This function produces simple predicates to match a specific line of a tooltip, by name.
            Func<Item, TooltipLine, bool> LineName(string s) => (Item i, TooltipLine l) => l.Mod == "Terraria" && l.Name == s;

            // These functions are shorthand to invoke ApplyTooltipEdits using the above predicates.
            void EditTooltipByNum(int lineNum, Action<TooltipLine> action) => ApplyTooltipEdits(tooltips, LineNum(lineNum), action);
            void EditTooltipByName(string lineName, Action<TooltipLine> action) => ApplyTooltipEdits(tooltips, LineName(lineName), action);

            // For items such as a Copper Helmet which literally have no tooltips at all, add a custom "Tooltip0" which mimics the vanilla Tooltip0.
            void AddTooltip(string text)
            {
                // Don't add the tooltip if the item is in a social slot
                if (item.social)
                    return;

                int defenseIndex = -1;
                for (int i = 0; i < tooltips.Count; ++i)
                    if (tooltips[i].Name == "Defense")
                    {
                        defenseIndex = i;
                        break;
                    }
                tooltips.Insert(defenseIndex + 1, new TooltipLine(CalamityMod.Instance, "Tooltip0", text));
            }
            #endregion

            // Numerous random tooltip edits which don't fit into another category
            #region Various Tooltip Edits

            // Master Mode items also drop in Revengeance
            // Only affects vanilla and Calamity items
            if (item.master && (item.type < ItemID.Count || item.ModItem?.Mod is CalamityMod))
                EditTooltipByName("Master", (line) => line.Text += " or Revengeance");

            // Add a tooltip about Slimed's effects
            if (item.type == ItemID.SlimeGun)
                EditTooltipByNum(0, (line) => line.Text += "\nSlimed enemies take more damage from fire-based debuffs");
            // Replace the meme tooltip with a useful one.
            if (item.type == ItemID.GelBalloon)
                EditTooltipByNum(0, (line) => line.Text = "Throws a mixture of slime and sparkling crystals"
                + "\nSlimed enemies take more damage from fire-based debuffs");

            // Flesh Knuckles giving extra max life.
            if (item.type == ItemID.FleshKnuckles || item.type == ItemID.HeroShield || item.type == ItemID.BerserkerGlove)
                EditTooltipByNum(0, (line) => line.Text += "\nMax life increased by 45");

            // Rod of Discord cannot be used multiple times to hurt yourself
            if (item.type == ItemID.RodofDiscord)
                EditTooltipByNum(0, (line) => line.Text += "\nTeleportation is disabled while Chaos State is active");

            // Indicate that the Ankh Shield provides sandstorm wind push immunity
            if (item.type == ItemID.AnkhShield)
                EditTooltipByNum(1, (line) => line.Text += ", including Mighty Wind");

            // Water removing items cannot be used in the Abyss
            string noAbyssLine = "\nCannot be used in the Abyss";
            if (item.type == ItemID.SuperAbsorbantSponge)
                EditTooltipByNum(0, (line) => line.Text += noAbyssLine);
            if (item.type == ItemID.EmptyBucket)
                EditTooltipByName("Defense", (line) => line.Text += noAbyssLine);

            // If Early Hardmode Rework is enabled: Remind users that ores will NOT spawn when an altar is smashed.
            if (CalamityConfig.Instance.EarlyHardmodeProgressionRework && (item.type == ItemID.Pwnhammer || item.type == ItemID.Hammush))
                EditTooltipByNum(0, (line) => line.Text += "\nDemon Altars now drop Souls of Night instead of generating ores when destroyed" +
                "\nHardmode ores now generate after defeating Mechanical Bosses for the first time");

            // Bottled Honey gives the Honey buff
            if (item.type == ItemID.BottledHoney)
                EditTooltipByName("HealLife", (line) => line.Text += "\nGrants the Honey buff for 2 minutes");

            // Warmth Potion provides debuff immunities
            if (item.type == ItemID.WarmthPotion)
            {
                string immunityLine = "\nGrants immunity to Chilled, Frozen and Glacial State";
                EditTooltipByNum(0, (line) => line.Text += immunityLine);
            }

            // Nerfed Archery Potion tooltip
            if (item.type == ItemID.ArcheryPotion)
                EditTooltipByNum(0, (line) => line.Text = "20% increased arrow speed and 5% increased arrow damage");

            // Buffed Ironskin Potion tooltip
            if (item.type == ItemID.IronskinPotion)
                EditTooltipByNum(0, (line) => line.Text = "Increase defense by " + CalamityUtils.GetScalingDefense(-1));

            // Nerfed Swiftness Potion tooltip
            if (item.type == ItemID.SwiftnessPotion)
                EditTooltipByNum(0, (line) => line.Text = "15% increased movement speed");

            // Hand Warmer has a side bonus with Snow armor
            if (item.type == ItemID.HandWarmer)
            {
                string extraLine = "\nProvides a regeneration boost while wearing the Snow armor";
                EditTooltipByNum(0, (line) => line.Text += extraLine);
            }

            // Golden Fishing Rod inherently contains High Test Fishing Line
            if (item.type == ItemID.GoldenFishingRod)
                EditTooltipByName("NeedsBait", (line) => line.Text += "\nIts fishing line will never break");

            // Information about graveyards
            // There are no item sets for tombstones wtf
            if (item.type == ItemID.Tombstone || item.type == ItemID.GraveMarker || item.type == ItemID.CrossGraveMarker || item.type == ItemID.Headstone || item.type == ItemID.Gravestone || item.type == ItemID.Obelisk
                || item.type == ItemID.RichGravestone1 || item.type == ItemID.RichGravestone2 || item.type == ItemID.RichGravestone3 || item.type == ItemID.RichGravestone4 || item.type == ItemID.RichGravestone5)
                EditTooltipByName("Material", (line) => line.Text += "\n20 of any tombstone turns the surrounding area into a graveyard"
                + "\nGraveyards have various new item sales and recipes");

            // Eternity Crystal notifies the player that they can accelerate the invasion
            if (item.type == ItemID.DD2ElderCrystal)
                EditTooltipByNum(0, (line) => line.Text += "\nOnce placed you can right click the crystal to skip waves or increase the spawn rate of the invaders");

            // Aerial Bane is no longer the real bane of aerial enemies (50% dmg bonus removed)
            if (item.type == ItemID.DD2BetsyBow)
                EditTooltipByNum(0, (line) => line.Text = "Shoots splitting arrows");

            // Modify item speed tooltips to use a new scale designed to more accurately reflect practical distributions of item speeds.
            // Due to the higher complexity of the action, the actual logic is delegated to its own method.
            // I think this fits the miscellaneous category? Not seeing anything like this elsewhere. - Tomat
            EditTooltipByName("Speed", (line) => RedistributeSpeedTooltips(item, line));

            if (item.type == ItemID.SpaceGun)
            {
                int cost = (int)(item.mana * Main.LocalPlayer.manaCost * 0.5f);
                EditTooltipByName("UseMana", (line) => line.Text = $"Uses {cost} mana");
            }
            if (item.healLife > 0 && Main.LocalPlayer.Calamity().healingPotBonus != 1f)
            {
                int healAmt = (int)(item.healLife * Main.LocalPlayer.Calamity().healingPotBonus);
                EditTooltipByName("HealLife", (line) => line.Text = $"Restores {healAmt} life");
            }
            #endregion

            // For boss summon item clarity
            #region Boss Summon Tooltip Edits

            if (item.type == ItemID.Abeemination)
            {
                EditTooltipByNum(0, (line) => line.Text += " when used in the Jungle\n");
                EditTooltipByNum(0, (line) => line.Text += "Enrages outside the Underground Jungle");
            }

            if (item.type == ItemID.BloodySpine)
            {
                EditTooltipByNum(0, (line) => line.Text += " when used in the Crimson\n");
                EditTooltipByNum(0, (line) => line.Text += "Enrages outside the Underground Crimson");
            }

            if (item.type == ItemID.ClothierVoodooDoll)
            {
                EditTooltipByNum(0, (line) => line.Text += "\nWhile equipped, summons Skeletron when the Clothier is killed during nighttime\n");
                EditTooltipByNum(0, (line) => line.Text += "Enrages during the day");
            }

            if (item.type == ItemID.DeerThing)
                EditTooltipByNum(0, (line) => line.Text += " when used in the Snow or Ice biome");

            if (item.type == ItemID.GuideVoodooDoll)
                EditTooltipByNum(0, (line) => line.Text += "\nSummons the Wall of Flesh if thrown into lava in the underworld while the Guide is alive");

            if (item.type == ItemID.LihzahrdPowerCell)
            {
                EditTooltipByNum(0, (line) => line.Text += " to summon the Golem\n");
                EditTooltipByNum(0, (line) => line.Text += "Enrages outside the Jungle Temple");

            }

            if (item.type == ItemID.MechanicalEye || item.type == ItemID.MechanicalSkull || item.type == ItemID.MechanicalWorm || item.type == ItemID.SuspiciousLookingEye)
            {
                EditTooltipByNum(0, (line) => line.Text += " when used during nighttime\n");
                EditTooltipByNum(0, (line) => line.Text += "Enrages during the day");
            }

            if (item.type == ItemID.QueenSlimeCrystal)
                EditTooltipByNum(0, (line) => line.Text += " when used in the Hallow");

            if (item.type == ItemID.TruffleWorm)
            {
                EditTooltipByName("Consumable", (line) => line.Text += "\nSummons Duke Fishron if used as bait in the Ocean\n");
                EditTooltipByName("Consumable", (line) => line.Text += "Enrages outside the Ocean");
            }

            if (item.type == ItemID.WormFood)
            {
                EditTooltipByNum(0, (line) => line.Text += " when used in the Corruption\n");
                EditTooltipByNum(0, (line) => line.Text += "Enrages outside the Underground Corruption");
            }
            #endregion

            // Brain of Confusion, Black Belt and Master Ninja Gear have guaranteed dodges with a fixed cooldown.
            #region Guaranteed Dodge Tooltips
            string beltDodgeLine = "Grants the ability to dodge attacks\n" +
                $"The dodge has a {BalancingConstants.BeltDodgeCooldown / 60} second cooldown which is shared with all other dodges and reflects";
            if (item.type == ItemID.BlackBelt)
                EditTooltipByNum(0, (line) => line.Text = beltDodgeLine);
            if (item.type == ItemID.MasterNinjaGear)
                EditTooltipByNum(1, (line) => line.Text = beltDodgeLine);

            string brainDodgeLine = "Grants the ability to dodge attacks\n" +
                $"The dodge has a {BalancingConstants.BrainDodgeCooldown / 60} second cooldown which is shared with all other dodges and reflects";
            if (item.type == ItemID.BrainOfConfusion)
                EditTooltipByNum(0, (line) => line.Text = brainDodgeLine);
            #endregion

            // Early Hardmode ore melee weapons have new on-hit effects.
            #region Early Hardmode Melee Tooltips

            // Cobalt
            if (item.type == ItemID.CobaltSword || item.type == ItemID.CobaltNaginata)
                EditTooltipByName("Knockback", (line) => line.Text += "\nDecreases enemy defense by 25% on hit");

            // Palladium
            if (item.type == ItemID.PalladiumSword || item.type == ItemID.PalladiumPike)
                EditTooltipByName("Knockback", (line) => line.Text += "\nIncreases life regen on hit");

            // Mythril
            if (item.type == ItemID.MythrilSword || item.type == ItemID.MythrilHalberd)
                EditTooltipByName("Knockback", (line) => line.Text += "\nDecreases enemy contact damage by 10% on hit");

            // Orichalcum
            if (item.type == ItemID.OrichalcumSword || item.type == ItemID.OrichalcumHalberd)
                EditTooltipByName("Knockback", (line) => line.Text += "\nIncreases how frequently the Orichalcum set bonus triggers on hit");

            // Adamantite
            if (item.type == ItemID.AdamantiteSword || item.type == ItemID.AdamantiteGlaive)
                EditTooltipByName("Knockback", (line) => line.Text += "\nSlows enemies on hit");

            // Titanium
            if (item.type == ItemID.TitaniumSword || item.type == ItemID.TitaniumTrident)
                EditTooltipByName("Knockback", (line) => line.Text += "\nDeals increased damage to enemies with high knockback resistance");

            // Hallowed (and True Excalibur)
            if (item.type == ItemID.Excalibur || item.type == ItemID.Gungnir || item.type == ItemID.TrueExcalibur)
                EditTooltipByName("Knockback", (line) => line.Text += "\nDeals double damage to enemies above 75% life");
            #endregion

            // Other melee weapon tooltips
            #region Other Melee Tooltips

            if (item.type == ItemID.CandyCaneSword || item.type == ItemID.FruitcakeChakram)
                EditTooltipByName("Knockback", (line) => line.Text += "\nHeals you on hit");

            // Stylish Scissors, all Phaseblades, and all Phasesabers
            if (item.type == ItemID.StylistKilLaKillScissorsIWish || (item.type >= ItemID.BluePhaseblade && item.type <= ItemID.YellowPhaseblade) || (item.type >= ItemID.BluePhasesaber && item.type <= ItemID.YellowPhasesaber) || item.type == ItemID.OrangePhaseblade || item.type == ItemID.OrangePhasesaber)
                EditTooltipByName("Knockback", (line) => line.Text += "\nIgnores 100% of enemy defense");

            if (item.type == ItemID.AntlionClaw || item.type == ItemID.BoneSword || item.type == ItemID.BreakerBlade)
                EditTooltipByName("Knockback", (line) => line.Text += "\nIgnores 50% of enemy defense");

            if (item.type == ItemID.NightsEdge || item.type == ItemID.TrueNightsEdge)
                EditTooltipByName("Knockback", (line) => line.Text += "\nInflicts Shadowflame on hit");

            if (item.type == ItemID.DeathSickle)
                EditTooltipByNum(0, (line) => line.Text += "\nInflicts Whispering Death on hit");
            #endregion

            // Light pets, accessories, and other items which boost the player's Abyss light stat
            #region Abyss Light Tooltips

            // +1 to Abyss light level
            string abyssSmallLightLine = "\nProvides a small amount of light in the abyss";

            if (item.type == ItemID.CrimsonHeart || item.type == ItemID.ShadowOrb || item.type == ItemID.MagicLantern || item.type == ItemID.JellyfishNecklace ||
                item.type == ItemID.MiningHelmet || item.type == ItemID.UltrabrightHelmet)
                EditTooltipByNum(0, (line) => line.Text += abyssSmallLightLine);
            if (item.type == ItemID.JellyfishDivingGear || item.type == ItemID.Magiluminescence)
                EditTooltipByNum(1, (line) => line.Text += abyssSmallLightLine);

            // +2 to Abyss light level
            string abyssMediumLightLine = "\nProvides a moderate amount of light in the abyss";

            if (item.type == ItemID.ShinePotion)
                EditTooltipByName("BuffTime", (line) => line.Text += abyssMediumLightLine);

            if (item.type == ItemID.FairyBell || item.type == ItemID.DD2PetGhost)
                EditTooltipByNum(0, (line) => line.Text += abyssMediumLightLine);

            // +3 to Abyss light level
            string abyssLargeLightLine = "\nProvides a large amount of light in the abyss";

            if (item.type == ItemID.WispinaBottle || item.type == ItemID.PumpkingPetItem || item.type == ItemID.GolemPetItem || item.type == ItemID.FairyQueenPetItem)
                EditTooltipByNum(0, (line) => line.Text += abyssLargeLightLine);
            if (item.type == ItemID.SuspiciousLookingTentacle)
                EditTooltipByNum(1, (line) => line.Text += abyssLargeLightLine);
            #endregion

            // Accessories and other items which boost the player's ability to breathe in the Abyss
            #region Abyss Breath Tooltips

            // Moderate breath boost
            string abyssModerateBreathLine = "\nModerately reduces breath loss in the abyss";

            if (item.type == ItemID.DivingHelmet)
                EditTooltipByNum(0, (line) => line.Text += abyssModerateBreathLine);
            if (item.type == ItemID.ArcticDivingGear)
                EditTooltipByNum(1, (line) => line.Text += abyssSmallLightLine + abyssModerateBreathLine);

            // Great breath boost
            string abyssGreatBreathLine = "\nGreatly reduces breath loss in the abyss";

            if (item.type == ItemID.GillsPotion)
                EditTooltipByName("BuffTime", (line) => line.Text += abyssGreatBreathLine);

            if (item.type == ItemID.NeptunesShell || item.type == ItemID.MoonShell)
                EditTooltipByNum(1, (line) => line.Text += abyssGreatBreathLine);
            if (item.type == ItemID.CelestialShell)
                EditTooltipByNum(4, (line) => line.Text += abyssModerateBreathLine);
            #endregion

            // Flasks apply to Rogue weapons
            #region Rogue Flask Tooltips
            if (item.type == ItemID.FlaskofCursedFlames)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace(" and Whip", ", Whip, and Rogue"));
            if (item.type == ItemID.FlaskofFire)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace(" and Whip", ", Whip, and Rogue"));
            if (item.type == ItemID.FlaskofGold)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace(" and Whip", ", Whip, and Rogue"));
            if (item.type == ItemID.FlaskofIchor)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace(" and Whip", ", Whip, and Rogue"));
            if (item.type == ItemID.FlaskofNanites)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace(" and Whip", ", Whip, and Rogue"));
            // party flask is unique because it affects ALL projectiles in Calamity, not just "also rogue ones"
            if (item.type == ItemID.FlaskofParty)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("Melee and Whip", "All"));
            if (item.type == ItemID.FlaskofPoison)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace(" and Whip", ", Whip, and Rogue"));
            if (item.type == ItemID.FlaskofVenom)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace(" and Whip", ", Whip, and Rogue"));
            #endregion

            // Rebalances to vanilla item stats
            #region Vanilla Item Rebalance Tooltips

            // Ancient Chisel rebalance.
            if (item.type == ItemID.AncientChisel)
                EditTooltipByNum(0, (line) => line.Text = "Increases mining speed by 15%");

            // Frozen Turtle Shell rebalance.
            if (item.type == ItemID.FrozenTurtleShell)
                EditTooltipByNum(0, (line) => line.Text = "Puts a shell around the owner when below 50% life that reduces damage by 15%");

            if (item.type == ItemID.FrozenShield)
                EditTooltipByNum(1, (line) => line.Text = "Puts a shell around the owner when below 50% life that reduces damage by 15%");

            // Ale and Sake rebalance.
            if (item.type == ItemID.Ale || item.type == ItemID.Sake)
                EditTooltipByNum(0, (line) => line.Text = "Increases melee damage by 10% and reduces defense by 5%");

            // Hellfire Treads buff.
            if (item.type == ItemID.HellfireTreads)
            {
                EditTooltipByNum(1, (line) => line.Text = line.Text.Replace("fire blocks", "the Burning and On Fire! debuffs"));
                EditTooltipByNum(3, (line) => line.Text += "\nMultiplies all fire-based debuff damage by 1.5\n" +
                "All attacks inflict Hellfire");
            }

            // Fairy Boots buff.
            if (item.type == ItemID.FairyBoots)
                EditTooltipByNum(2, (line) => line.Text += "\nFairies can spawn at any time on the surface and spawn far more frequently\n" +
                "Nearby fairies grant increased life regen, defense and movement speed\n" +
                "Fairies are immune to damage and will no longer flee");

            // Armor Crunch immunity pre-Golem.
            if (item.type == ItemID.ArmorPolish || item.type == ItemID.ArmorBracing)
                EditTooltipByNum(0, (line) => line.Text += " and Armor Crunch");

            // Nightwither immunity pre-Moon Lord and Holy Flames immunity pre-Profaned Guardians and melee speed removal.
            if (item.type == ItemID.MoonStone)
            {
                EditTooltipByNum(2, (line) => line.Text += "\nGrants immunity to Nightwither");
                EditTooltipByNum(1, (line) => line.Text = line.Text.Replace("melee speed, ", ""));
            }
            if (item.type == ItemID.SunStone)
            {
                EditTooltipByNum(2, (line) => line.Text += "\nGrants immunity to Holy Flames");
                EditTooltipByNum(1, (line) => line.Text = line.Text.Replace("melee speed, ", ""));
            }
            if (item.type == ItemID.CelestialStone)
            {
                EditTooltipByNum(2, (line) => line.Text += "\nGrants immunity to Nightwither and Holy Flames");
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace(" melee speed,", ""));
            }
            if (item.type == ItemID.CelestialShell)
            {
                EditTooltipByNum(4, (line) => line.Text += "\nGrants immunity to Nightwither and Holy Flames");
                EditTooltipByNum(2, (line) => line.Text = line.Text.Replace(" melee speed,", ""));
            }

            // Arcane and Magnet Flower buffs.
            if (item.type == ItemID.ArcaneFlower || item.type == ItemID.MagnetFlower)
                EditTooltipByNum(0, (line) => line.Text = "12% reduced mana usage");

            // Magiluminescence nerf and clear explanation of what it actually does.
            if (item.type == ItemID.Magiluminescence)
                EditTooltipByNum(0, (line) => line.Text = "Increases movement acceleration and deceleration by 1.25x\n" +
                "Increases movement speed by 1.05x. This bonus applies to running boot accessories");

            // Soaring Insignia nerf and clear explanation of what it actually does.
            if (item.type == ItemID.EmpressFlightBooster)
            {
                EditTooltipByNum(0, (line) => line.Text = "Increases wing flight time by 25%");
                EditTooltipByNum(1, (line) => line.Text = "Increases movement and jump speed by 10% and acceleration by 1.1x");
            }

            // Sniper Scope
            if (item.type == ItemID.SniperScope)
                EditTooltipByNum(1, (line) => line.Text = "7% increased ranged damage and critical strike chance");

            // Magic Quiver
            if (item.type == ItemID.MagicQuiver)
                EditTooltipByNum(0, (line) => line.Text = "Increases arrow damage by 5% and greatly increases arrow speed");

            // Molten Quiver
            if (item.type == ItemID.MoltenQuiver)
            {
                EditTooltipByNum(0, (line) => line.Text = "Increases arrow damage by 7% and greatly increases arrow speed");
                EditTooltipByNum(2, (line) => line.Text += " and all arrows inflict Hellfire");
            }

            // Magic Power Potion nerf
            if (item.type == ItemID.MagicPowerPotion)
                EditTooltipByNum(0, (line) => line.Text = "10% increased magic damage");

            // Magic Hat nerf
            if (item.type == ItemID.MagicHat)
                EditTooltipByNum(0, (line) => line.Text = "5% increased magic damage and critical strike chance");

            // Worm Scarf only gives 10% DR instead of 17%
            if (item.type == ItemID.WormScarf)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("17%", "10%"));

            // Feral Claws line melee speed and true melee damage changes
            if (item.type == ItemID.FeralClaws)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("12%", "10%"));

            if (item.type == ItemID.TitanGlove)
                EditTooltipByNum(0, (line) => line.Text += "\n10% increased true melee damage");

            if (item.type == ItemID.PowerGlove)
            {
                EditTooltipByNum(1, (line) => line.Text = line.Text.Replace("12%", "10%"));
                EditTooltipByNum(0, (line) => line.Text += "\n10% increased true melee damage");
            }

            if (item.type == ItemID.BerserkerGlove)
            {
                EditTooltipByNum(1, (line) => line.Text = line.Text.Replace("12% increased melee speed", "10% increased true melee damage"));
            }

            if (item.type == ItemID.MechanicalGlove)
                EditTooltipByNum(0, (line) => line.Text += "\n10% increased true melee damage");

            if (item.type == ItemID.FireGauntlet)
            {
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("fire damage", "Hellfire"));
                string extraLine = "\n10% increased true melee damage";
                EditTooltipByNum(1, (line) => line.Text = "14% increased melee damage and speed" + extraLine);
            }

            // On Fire! debuff immunities
            if (item.type == ItemID.ObsidianSkull || item.type == ItemID.AnkhShield || item.type == ItemID.ObsidianSkullRose || item.type == ItemID.MoltenCharm)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("fire blocks", "the Burning and On Fire! debuffs"));

            if (item.type == ItemID.ObsidianHorseshoe || item.type == ItemID.ObsidianShield || item.type == ItemID.ObsidianWaterWalkingBoots || item.type == ItemID.LavaWaders || item.type == ItemID.LavaSkull || item.type == ItemID.MoltenSkullRose)
                EditTooltipByNum(1, (line) => line.Text = line.Text.Replace("fire blocks", "the Burning and On Fire! debuffs"));

            if (item.type == ItemID.TerrasparkBoots)
                EditTooltipByNum(3, (line) => line.Text += "\nImmunity to the On Fire! debuff");

            // IT'S HELLFIRE!!!
            if (item.type == ItemID.MagmaStone || item.type == ItemID.LavaSkull || item.type == ItemID.MoltenSkullRose)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("fire damage", "Hellfire"));

            // Yoyo Glove/Bag apply a 0.5x damage multiplier on the second yoyo
            if (item.type == ItemID.YoyoBag || item.type == ItemID.YoYoGlove)
                EditTooltipByNum(0, (line) => line.Text += "\nSecondary yoyos will do 50% less damage");

            // Falcon Blade +20% move speed while holding
            if (item.type == ItemID.FalconBlade)
                EditTooltipByName("Knockback", (line) => line.Text += "\nHolding this item grants +20% increased movement speed");

            //Gi 10% melee speed into 10% jump speed replacement
            if (item.type == ItemID.Gi)
                EditTooltipByNum(1, (line) => line.Text = line.Text.Replace("melee", "jump"));
            #endregion

            // Pre-Hardmode ore armor tooltip edits
            #region Pre-Hardmode Ore Armor
            // Copper
            if (item.type == ItemID.CopperHelmet)
                AddTooltip("5% increased damage");
            if (item.type == ItemID.CopperChainmail)
                AddTooltip("3% increased critical strike chance");
            if (item.type == ItemID.CopperGreaves)
                AddTooltip("5% increased movement speed");

            // Tin
            if (item.type == ItemID.TinHelmet)
                AddTooltip("3% increased critical strike chance");
            if (item.type == ItemID.TinChainmail)
                AddTooltip("+1 life regen");
            if (item.type == ItemID.TinGreaves)
                AddTooltip("5% increased movement speed");

            // Iron
            if (item.type == ItemID.IronHelmet || item.type == ItemID.AncientIronHelmet || item.type == ItemID.IronChainmail || item.type == ItemID.IronGreaves)
                AddTooltip("Increases damage reduction by 3%");

            // Lead
            if (item.type == ItemID.LeadHelmet || item.type == ItemID.LeadChainmail || item.type == ItemID.LeadGreaves)
                AddTooltip("Increases damage reduction by 3%");

            // Silver
            if (item.type == ItemID.SilverHelmet)
                AddTooltip("5% increased critical strike chance");
            if (item.type == ItemID.SilverChainmail)
                AddTooltip("+2 life regen");
            if (item.type == ItemID.SilverGreaves)
                AddTooltip("8% increased movement speed");

            // Tungsten
            if (item.type == ItemID.TungstenHelmet)
                AddTooltip("7% increased damage");
            if (item.type == ItemID.TungstenChainmail)
                AddTooltip("+1 life regen");
            if (item.type == ItemID.TungstenGreaves)
                AddTooltip("8% increased movement speed");

            // Gold
            if (item.type == ItemID.GoldHelmet || item.type == ItemID.AncientGoldHelmet)
                AddTooltip("6% increased damage");
            if (item.type == ItemID.GoldChainmail)
                AddTooltip("Increases damage reduction by 5%");
            if (item.type == ItemID.GoldGreaves)
                AddTooltip("10% increased movement speed");

            // Platinum
            if (item.type == ItemID.PlatinumHelmet)
                AddTooltip("6% increased damage");
            if (item.type == ItemID.PlatinumChainmail)
                AddTooltip("5% increased critical strike chance");
            if (item.type == ItemID.PlatinumGreaves)
                AddTooltip("10% increased movement speed");

            // Shadow
            if (item.type == ItemID.ShadowHelmet || item.type == ItemID.AncientShadowHelmet || item.type == ItemID.ShadowScalemail || item.type == ItemID.AncientShadowScalemail || item.type == ItemID.ShadowGreaves || item.type == ItemID.AncientShadowGreaves)
                EditTooltipByNum(0, (line) => line.Text = "5% increased damage and 7% increased jump speed");

            // Crimson
            if (item.type == ItemID.CrimsonHelmet || item.type == ItemID.CrimsonScalemail || item.type == ItemID.CrimsonGreaves)
            {
                EditTooltipByNum(0, (line) => {
                    string newTooltip = line.Text.Replace("2%", "5%");
                    newTooltip += "\n+1 life regen";
                    line.Text = newTooltip;
                });
            }
            #endregion

            // Hardmode ore armor tooltip edits
            #region Hardmode Ore Armor
            // Cobalt
            if (item.type == ItemID.CobaltHat)
                EditTooltipByNum(0, (line) => line.Text = $"Increases maximum mana by {CobaltArmorSetChange.MaxManaBoost + 40}");

            // Palladium
            if (item.type == ItemID.PalladiumBreastplate)
                EditTooltipByNum(0, (line) => line.Text = $"{PalladiumArmorSetChange.ChestplateDamagePercentageBoost + 3}% increased damage.");
            if (item.type == ItemID.PalladiumLeggings)
                EditTooltipByNum(0, (line) => line.Text = $"{PalladiumArmorSetChange.LeggingsDamagePercentageBoost + 2}% increased damage.");

            // Mythril
            if (item.type == ItemID.MythrilHood)
                EditTooltipByNum(0, (line) => line.Text = $"Increases maximum mana by {MythrilArmorSetChange.MaxManaBoost + 60}");

            // Orichalcum
            if (item.type == ItemID.OrichalcumBreastplate)
                EditTooltipByNum(0, (line) => line.Text = $"{OrichalcumArmorSetChange.ChestplateCritChanceBoost + 6}% increased critical strike chance");

            // Adamantite
            if (item.type == ItemID.AdamantiteHeadgear)
                EditTooltipByNum(0, (line) => line.Text = $"Increases maximum mana by {AdamantiteArmorSetChange.MaxManaBoost + 80}");

            // Titanium
            if (item.type == ItemID.TitaniumMask)
                EditTooltipByNum(1, (line) => line.Text = line.Text.Replace("9", "14"));
            #endregion

            // DD2 armor tooltip edits
            #region DD2 Armor
            // Reduce DD2 armor piece bonuses because they're overpowered
            // Squire armor
            if (item.type == ItemID.SquirePlating)
                EditTooltipByNum(0, (line) => line.Text = "10% increased minion and melee damage");
            if (item.type == ItemID.SquireGreaves)
                EditTooltipByNum(0, (line) => line.Text = "5% increased minion damage and melee critical strike chance\n" +
                "15% increased movement speed");

            // Monk armor
            if (item.type == ItemID.MonkBrows)
                EditTooltipByNum(0, (line) => line.Text = "Increases your max number of sentries by 1 and increases melee attack speed by 10%");
            if (item.type == ItemID.MonkShirt)
                EditTooltipByNum(0, (line) => line.Text = "10% increased minion and melee damage");
            if (item.type == ItemID.MonkPants)
            {
                EditTooltipByNum(0, (line) => line.Text = "5% increased minion damage and melee critical strike chance");
                EditTooltipByNum(1, (line) => line.Text = "20% increased movement speed");
            }

            // Huntress armor
            if (item.type == ItemID.HuntressJerkin)
                EditTooltipByNum(0, (line) => line.Text = "10% increased minion and ranged damage\n" +
                "10% chance to not consume ammo");

            // Apprentice armor
            if (item.type == ItemID.ApprenticeTrousers)
                EditTooltipByNum(0, (line) => line.Text = "5% increased minion damage and magic critical strike chance\n" +
                "20% increased movement speed");

            // Valhalla Knight armor
            if (item.type == ItemID.SquireAltShirt)
                EditTooltipByNum(0, (line) => line.Text = "30% increased minion damage and increased life regeneration");
            if (item.type == ItemID.SquireAltPants)
                EditTooltipByNum(0, (line) => line.Text = "10% increased minion damage and melee critical strike chance");

            // Shinobi Infiltrator armor
            if (item.type == ItemID.MonkAltHead)
                EditTooltipByNum(0, (line) => line.Text = "Increases your max number of sentries by 2\n" +
                "10% increased melee and minion damage");
            if (item.type == ItemID.MonkAltShirt)
                EditTooltipByNum(0, (line) => line.Text = "10% increased minion damage and melee speed");
            if (item.type == ItemID.MonkAltPants)
                EditTooltipByNum(0, (line) => line.Text = "10% increased minion damage and melee critical strike chance");

            // Red Riding armor
            if (item.type == ItemID.HuntressAltShirt)
                EditTooltipByNum(0, (line) => line.Text = "15% increased minion and ranged damage and 20% chance to not consume ammo");

            // Dark Artist armor
            if (item.type == ItemID.ApprenticeAltPants)
                EditTooltipByNum(0, (line) => line.Text = "10% increased minion damage and magic critical strike chance");
            #endregion

            // Non-consumable boss summon items
            #region Vanilla Boss Summon Non-consumable Tooltips
            if (item.type == ItemID.SlimeCrown || item.type == ItemID.SuspiciousLookingEye || item.type == ItemID.BloodMoonStarter || item.type == ItemID.GoblinBattleStandard ||
                item.type == ItemID.WormFood || item.type == ItemID.BloodySpine || item.type == ItemID.Abeemination || item.type == ItemID.DeerThing || item.type == ItemID.QueenSlimeCrystal ||
                item.type == ItemID.PirateMap || item.type == ItemID.SnowGlobe || item.type == ItemID.MechanicalEye || item.type == ItemID.MechanicalWorm || item.type == ItemID.MechanicalSkull ||
                item.type == ItemID.NaughtyPresent || item.type == ItemID.PumpkinMoonMedallion || item.type == ItemID.SolarTablet || item.type == ItemID.SolarTablet || item.type == ItemID.CelestialSigil)

                EditTooltipByNum(0, (line) => line.Text += "\nNot consumable");
            #endregion

            // Add mentions of what Calamity ores vanilla pickaxes can mine
            #region Pickaxe New Ore Tooltips
            if (item.type == ItemID.GoldPickaxe || item.type == ItemID.PlatinumPickaxe)
                EditTooltipByNum(0, (line) => line.Text = "Can mine Demonite, Crimtane, Meteorite, Sea Prisms and Sea Prism Crystals");

            if (item.type == ItemID.Picksaw)
                EditTooltipByNum(0, (line) => line.Text += "\nCan mine Scoria Ore located in the Abyss");

            if (item.type == ItemID.SolarFlarePickaxe || item.type == ItemID.VortexPickaxe || item.type == ItemID.NebulaPickaxe || item.type == ItemID.StardustPickaxe
                || item.type == ItemID.SolarFlareDrill || item.type == ItemID.VortexDrill || item.type == ItemID.NebulaDrill || item.type == ItemID.StardustDrill)
                EditTooltipByName("Knockback", (line) => line.Text += "\nCan mine Uelibloom Ore");
            #endregion

            // Rebalances and information about vanilla set bonuses
            #region Vanilla Set Bonus Tooltips

            EditTooltipByName("SetBonus", (line) => VanillaArmorChangeManager.ApplySetBonusTooltipChanges(item, ref line.Text));

            // Gladiator
            if (item.type == ItemID.GladiatorHelmet)
                EditTooltipByName("Defense", (line) => line.Text += $"\n{GladiatorArmorSetChange.HelmetRogueDamageBoostPercent}% increased rogue damage");
            if (item.type == ItemID.GladiatorBreastplate)
                EditTooltipByName("Defense", (line) => line.Text += $"\n{GladiatorArmorSetChange.ChestplateRogueCritBoostPercent}% increased rogue critical strike chance");
            if (item.type == ItemID.GladiatorLeggings)
                EditTooltipByName("Defense", (line) => line.Text += $"\n{GladiatorArmorSetChange.LeggingRogueVelocityBoostPercent}% increased rogue velocity");

            // Forbidden (UNLESS you are wearing the Circlet, which is Summon/Rogue and does not get this line)
            if ((item.type == ItemID.AncientBattleArmorHat || item.type == ItemID.AncientBattleArmorShirt || item.type == ItemID.AncientBattleArmorPants)
                && !Main.LocalPlayer.Calamity().forbiddenCirclet)
                EditTooltipByName("SetBonus", (line) => line.Text += "\nMinions no longer deal less damage while wielding magic weapons");
            #endregion

            // Provide the full stats of every vanilla set of wings
            #region Wing Stat Tooltips

            // This function produces a "stat sheet" for a pair of wings from the raw stats.
            // For "vertical speed", 0 = Bad, 1 = Average, 2 = Good, 3 = Great, 4 = Excellent.
            string[] vertSpeedStrings = new string[] { "Bad vertical speed", "Average vertical speed", "Good vertical speed", "Great vertical speed", "Excellent vertical speed" };
            string WingStatsTooltip(float hSpeed, float accelMult, int vertSpeed, int flightTime, string extraTooltip = null)
            {
                StringBuilder sb = new StringBuilder(512);
                sb.Append('\n');
                sb.Append($"Horizontal speed: {hSpeed:N2}\n");
                sb.Append($"Acceleration multiplier: {accelMult:N1}\n");
                sb.Append(vertSpeedStrings[vertSpeed]);
                sb.Append('\n');
                sb.Append($"Flight time: {flightTime}");
                if (extraTooltip != null)
                {
                    sb.Append('\n');
                    sb.Append(extraTooltip);
                }
                return sb.ToString();
            }

            // This function is shorthand for appending a stat sheet to a pair of wings.
            void AddWingStats(float h, float a, int v, int f, string s = null) => EditTooltipByNum(0, (line) => line.Text += WingStatsTooltip(h, a, v, f, s));
            void AddWingStats2(float h, float a, int v, int f, string s = null, string lineName = null) => EditTooltipByName(lineName, (line) => line.Text += WingStatsTooltip(h, a, v, f, s));

            if (item.type == ItemID.CreativeWings)
                AddWingStats(3f, 1f, 0, 25);

            if (item.type == ItemID.AngelWings)
                AddWingStats(6.25f, 1f, 1, 100, "+20 max life, +10 defense and +2 life regen");

            if (item.type == ItemID.DemonWings)
                AddWingStats(6.25f, 1f, 1, 100, "5% increased damage and critical strike chance");

            if (item.type == ItemID.Jetpack)
                AddWingStats(6.5f, 1f, 1, 150);

            if (item.type == ItemID.ButterflyWings)
                AddWingStats(7.5f, 1f, 1, 160, "+20 max mana, 5% decreased mana usage,\n" +
                    "5% increased magic damage and magic critical strike chance");

            if (item.type == ItemID.FairyWings)
                AddWingStats(6.75f, 1f, 1, 130, "+60 max life");

            if (item.type == ItemID.BeeWings)
                AddWingStats(7.5f, 1f, 1, 160, "Permanently gives the Honey buff");

            if (item.type == ItemID.HarpyWings)
                AddWingStats(6.75f, 1f, 1, 130, "20% increased movement speed\n" +
                    "With Harpy Ring or Angel Treads equipped, most attacks sometimes launch feathers");

            if (item.type == ItemID.BoneWings)
                AddWingStats(7.5f, 1f, 1, 240, "Halves flight time when taking a hit");

            if (item.type == ItemID.FlameWings)
                AddWingStats(7.5f, 1f, 1, 160, "5% increased melee damage and critical strike chance");

            if (item.type == ItemID.FrozenWings)
                AddWingStats(6.75f, 1f, 1, 130, "2% increased melee and ranged damage\n" +
                    "and 1% increased melee and ranged critical strike chance\n" +
                    "while wearing the Frost Armor");

            if (item.type == ItemID.GhostWings)
                AddWingStats(7.5f, 1f, 1, 170, "+10 defense and 5% increased damage reduction while wearing the Spectre Hood set\n" +
                    "5% increased magic damage and critical strike chance while wearing the Spectre Mask set");

            if (item.type == ItemID.BeetleWings)
                AddWingStats(7.5f, 1f, 1, 170, "+10 defense and 5% increased damage reduction while wearing the Beetle Shell set\n" +
                    "5% increased melee damage and critical strike chance while wearing the Beetle Scale Mail set");

            if (item.type == ItemID.FinWings)
                AddWingStats(6.75f, 1f, 1, 130, "Gills effect and you can move freely through liquids\n" +
                    "You fall faster while submerged in liquid");

            if (item.type == ItemID.FishronWings)
                AddWingStats(8f, 2f, 2, 180);

            if (item.type == ItemID.SteampunkWings)
                AddWingStats(7.5f, 1f, 1, 180, "+8 defense, 10% increased movement speed,\n" + "4% increased damage, and 2% increased critical strike chance");

            if (item.type == ItemID.LeafWings)
                AddWingStats(7.5f, 1f, 1, 160, "+5 defense, 5% increased damage reduction,\n" + "and permanent Dryad's Blessing while wearing the Tiki Armor");

            if (item.type == ItemID.BatWings)
                AddWingStats(7.5f, 1f, 1, 160, "At night or during an eclipse, you will gain the following boosts:\n" +
                    "7% increased damage and 3% increased critical strike chance");

            // All developer wings have identical stats and no special effects
            if (item.type == ItemID.Yoraiz0rWings || item.type == ItemID.JimsWings || item.type == ItemID.SkiphsWings ||
                item.type == ItemID.LokisWings || item.type == ItemID.ArkhalisWings || item.type == ItemID.LeinforsWings ||
                item.type == ItemID.BejeweledValkyrieWing || item.type == ItemID.RedsWings || item.type == ItemID.DTownsWings ||
                item.type == ItemID.WillsWings || item.type == ItemID.CrownosWings || item.type == ItemID.CenxsWings ||
                item.type == ItemID.FoodBarbarianWings || item.type == ItemID.GroxTheGreatWings || item.type == ItemID.GhostarsWings ||
                item.type == ItemID.SafemanWings)
            {
                AddWingStats(7f, 1f, 1, 150);
            }

            if (item.type == ItemID.TatteredFairyWings)
                AddWingStats(7.5f, 1f, 1, 180, "5% increased damage and critical strike chance");

            if (item.type == ItemID.SpookyWings)
                AddWingStats(7.5f, 1f, 1, 180, "Increased minion knockback and 5% increased minion damage while wearing the Spooky Armor");

            if (item.type == ItemID.Hoverboard)
                AddWingStats(6.5f, 1f, 1, 170, "5% increased weapon-type damage while wearing the Shroomite Armor\n" +
                    "The weapon type boosted matches which Shroomite helmet is worn");

            if (item.type == ItemID.FestiveWings)
                AddWingStats(7.5f, 1f, 1, 180, "+40 max life\nOrnaments rain down as you fly");

            if (item.type == ItemID.MothronWings)
                AddWingStats(7.5f, 1f, 1, 170, "+5 defense and 5% increased damage");

            if (item.type == ItemID.WingsSolar)
                AddWingStats(9f, 2.5f, 3, 180, "7% increased melee damage and 3% increased melee critical strike chance\n" +
                    "while wearing the Solar Flare Armor");

            if (item.type == ItemID.WingsStardust)
                AddWingStats(9f, 2.5f, 3, 180, "10% increased minion damage while wearing the Stardust Armor");

            if (item.type == ItemID.WingsVortex)
                AddWingStats(6.5f, 1.5f, 2, 180, "3% increased ranged damage and 7% increased ranged critical strike chance\n" +
                    "while wearing the Vortex Armor");

            if (item.type == ItemID.WingsNebula)
                AddWingStats(6.5f, 1.5f, 2, 180, "+20 max mana, 5% increased magic damage and critical strike chance,\n" +
                    "and 5% decreased mana usage while wearing the Nebula Armor");

            // Betsy's Wings (and dev wings) are the only wings without "Allows flight and free fall"
            if (item.type == ItemID.BetsyWings)
                AddWingStats2(6f, 2.5f, 2, 150, null, "Equipable");

            if (item.type == ItemID.RainbowWings)
                AddWingStats(7f, 2.5f, 2, 100);

            if (item.type == ItemID.LongRainbowTrailWings)
                AddWingStats(8f, 2.75f, 4, 180);
            #endregion

            // Provide the full stats of every vanilla grappling hook
            #region Grappling Hook Stat Tooltips

            // This function produces a "stat sheet" for a grappling hook from the raw stats.
            string HookStatsTooltip(float reach, float launch, float reel, float pull)
            {
                StringBuilder sb = new StringBuilder(128);
                sb.Append('\n');
                sb.Append($"Reach: {reach:N3} tiles\n");
                sb.Append($"Launch Velocity: {launch:N2}\n");
                sb.Append($"Reelback Velocity: {reel:N2}\n");
                sb.Append($"Pull Velocity: {pull:N2}");
                return sb.ToString();
            }

            // This function is shorthand for appending a stat sheet to a grappling hook.
            void AddGrappleStats(float r, float l, float e, float p) => EditTooltipByName("Equipable", (line) => line.Text += HookStatsTooltip(r, l, e, p));

            if (item.type == ItemID.GrapplingHook)
                AddGrappleStats(18.75f, 11.5f, 11f, 11f);
            if (item.type == ItemID.AmethystHook)
                AddGrappleStats(18.75f, 10f, 11f, 11f);
            if (item.type == ItemID.SquirrelHook)
                AddGrappleStats(19f, 11.5f, 11f, 11f);
            if (item.type == ItemID.TopazHook)
                AddGrappleStats(20.625f, 10.5f, 11.75f, 11f);
            if (item.type == ItemID.SapphireHook)
                AddGrappleStats(22.5f, 11f, 12.5f, 11f);
            if (item.type == ItemID.EmeraldHook)
                AddGrappleStats(24.375f, 11.5f, 13.25f, 11f);
            if (item.type == ItemID.RubyHook)
                AddGrappleStats(26.25f, 12f, 14f, 11f);
            if (item.type == ItemID.AmberHook)
                AddGrappleStats(27.5f, 12.5f, 15f, 11f);
            if (item.type == ItemID.DiamondHook)
                AddGrappleStats(29.125f, 12.5f, 14.75f, 11f);
            if (item.type == ItemID.WebSlinger)
                AddGrappleStats(22.625f, 10f, 11f, 11f);
            if (item.type == ItemID.SkeletronHand)
                AddGrappleStats(21.875f, 15f, 11f, 11f);
            if (item.type == ItemID.SlimeHook)
                AddGrappleStats(18.75f, 13f, 11f, 11f);
            if (item.type == ItemID.FishHook)
                AddGrappleStats(25f, 13f, 11f, 11f);
            if (item.type == ItemID.IvyWhip)
                AddGrappleStats(25f, 13f, 15f, 11f);
            if (item.type == ItemID.BatHook)
                AddGrappleStats(31.25f, 13.5f, 20f, 13f);
            if (item.type == ItemID.CandyCaneHook)
                AddGrappleStats(25f, 11.5f, 11f, 11f);
            if (item.type == ItemID.DualHook)
                AddGrappleStats(27.5f, 14f, 17f, 11f);
            if (item.type == ItemID.QueenSlimeHook)
                AddGrappleStats(30f, 16f, 18f, 11f);
            // these three grapple hooks are all functionally identical
            if (item.type == ItemID.WormHook || item.type == ItemID.TendonHook || item.type == ItemID.IlluminantHook)
                AddGrappleStats(30f, 15f, 18f, 11f);
            if (item.type == ItemID.ThornHook)
                AddGrappleStats(30f, 16f, 18f, 12f);
            if (item.type == ItemID.AntiGravityHook)
                AddGrappleStats(31.25f, 14f, 20f, 11f);
            if (item.type == ItemID.SpookyHook)
                AddGrappleStats(34.375f, 15.5f, 22f, 11f);
            if (item.type == ItemID.ChristmasHook)
                AddGrappleStats(34.375f, 15.5f, 17f, 11f);
            if (item.type == ItemID.LunarHook)
                AddGrappleStats(34.375f, 18f, 24f, 16f);
            if (item.type == ItemID.StaticHook)
                AddGrappleStats(37.5f, 16f, 24f, 0f);
            #endregion

            // Beyond this point all code only applies to accessories. Skip it all if the item is not an accessory.
            if (!item.accessory)
                return;

            // Display the stat changes to vanilla prefixes
            #region Accessory Prefix Rebalance Tooltips

            // Turns a number into a string of increased DR.
            string DRString(float percent) => $"\n+{percent:N2}% damage reduction";

            switch (item.prefix)
            {
                case PrefixID.Hard:
                    EditTooltipByName("PrefixAccDefense",
                        (line) => line.Text = line.Text.Replace("1", CalamityUtils.GetScalingDefense(item.prefix).ToString()) + DRString(0.25f));
                    return;
                case PrefixID.Guarding:
                    EditTooltipByName("PrefixAccDefense",
                        (line) => line.Text = line.Text.Replace("2", CalamityUtils.GetScalingDefense(item.prefix).ToString()) + DRString(0.5f));
                    return;
                case PrefixID.Armored:
                    EditTooltipByName("PrefixAccDefense",
                        (line) => line.Text = line.Text.Replace("3", CalamityUtils.GetScalingDefense(item.prefix).ToString()) + DRString(0.75f));
                    return;
                case PrefixID.Warding:
                    EditTooltipByName("PrefixAccDefense",
                        (line) => line.Text = line.Text.Replace("4", CalamityUtils.GetScalingDefense(item.prefix).ToString()) + DRString(1f));
                    return;
                case PrefixID.Lucky:
                    EditTooltipByName("PrefixAccCritChance", (line) => line.Text += "\n+0.05 luck");
                    return;
            }
            #endregion
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
                TooltipLine line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Expert");
                if (line == null)
                    line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Price");

                TooltipLine StealthGen = new TooltipLine(Mod, "PrefixStealthGenBoost", "+" + Math.Round(stealthGenBoost * 100f) + "% stealth generation")
                {
                    IsModifier = true
                };

                if (line == null)
                    tooltips.Add(StealthGen);
                else
                    tooltips.Insert(tooltips.IndexOf(line), StealthGen);
            }
        }
        #endregion

        #region Stealth Strike Damage Prefix Weapon Tooltip
        private void StealthWeaponTooltip(Item item, IList<TooltipLine> tooltips)
        {
            if (!item.CountsAsClass<RogueDamageClass>() || item.accessory || item.prefix <= 0)
                return;

            float stealthDmgBonus = item.Calamity().StealthStrikePrefixBonus - 1f;
            if (stealthDmgBonus > 0)
            {
                TooltipLine line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "PrefixShootSpeed");
                if (line == null)
                    line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "PrefixCritChance");
                else if (line == null)
                    line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "PrefixSpeed");
                else if (line == null)
                    line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "PrefixDamage");
                TooltipLine line2 = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Expert");
                if (line2 == null)
                    line2 = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Price");

                TooltipLine StealthDmg = new TooltipLine(Mod, "PrefixStealthDamageBoost", "+" + Math.Round(stealthDmgBonus * 100f) + "% stealth strike damage")
                {
                    IsModifier = true
                };

                // If price/expert line doesn't exist, just add it to the end
                if (line2 == null)
                    tooltips.Add(StealthDmg);
                // Otherwise, insert it right before the sell price (or expert line)
                else
                    tooltips.Insert(tooltips.IndexOf(line2), StealthDmg);
            }
        }
        #endregion

        #region Speed Tooltips
        
        // TODO: Investigate using a SortedDictionary instead? May be slower, but removes the need for carefully adding KVPs.
        /// <summary>
        /// This dictionary handles easily retrieving tooltip text based on a numerical threshold. <br />
        /// As items are added to the dictionary, the keys should only increase as they go down. <br />
        /// For example: <code>{ 2, x }, { 4, y }, { 7, z }, ...</code>. <br />
        /// When iterating with the threshold in mind, this essentially equates to: <br />
        /// <code>
        /// if (foo &lt;= 2) bar = x;
        /// else if (foo &lt;= 4) bar = y;
        /// else if (foo &lt;= 7) bar = z;
        /// </code>
        /// </summary>
        /// <remarks>
        /// Currently, the dictionary functions as follows: <br />
        /// 1-5   insanely fast <br />
        /// 6-9   very fast <br />
        /// 10-14 fast <br />
        /// 15-22 average <br />
        /// 23-29 slow <br />
        /// 30-37 very slow <br />
        /// 38-45 extremely slow <br />
        /// 46+   snail
        /// </remarks>
        private static readonly Dictionary<int, LocalizedText> SpeedTooltips = new Dictionary<int, LocalizedText>()
        {
            { 5, Language.GetText("LegacyTooltip.6") },
            { 9, Language.GetText("LegacyTooltip.7") },
            { 14, Language.GetText("LegacyTooltip.8") },
            { 22, Language.GetText("LegacyTooltip.9") },
            { 29, Language.GetText("LegacyTooltip.10") },
            { 37, Language.GetText("LegacyTooltip.11") },
            { 45, Language.GetText("LegacyTooltip.12") },
            // TODO: Using int.MaxValue here may be considered kind of strange - only alternatives I can think of require hardcoding.
            { int.MaxValue, Language.GetText("LegacyTooltip.13") }
        };

        private static void RedistributeSpeedTooltips(Item item, TooltipLine line)
        {
            // Iterate through each KeyValuePair in this dictionary.
            // See the summary of SpeedTooltips to understand the purpose and logic of this loop.
            foreach ((int threshold, LocalizedText tooltip) in SpeedTooltips)
                if (item.useAnimation <= threshold)
                {
                    line.Text = tooltip.Value;
                    break;
                }
        }
        #endregion

        #region Enchanted Rarity Text Drawing
        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            // Special enchantment line color.
            if (line.Name == "ItemName" && line.Mod == "Terraria" && item.IsEnchanted())
            {
                Color rarityColor = line.OverrideColor ?? line.Color;
                Vector2 basePosition = new Vector2(line.X, line.Y);

                float backInterpolant = (float)Math.Pow(Main.GlobalTimeWrappedHourly * 0.81f % 1f, 1.5f);
                Vector2 backScale = line.BaseScale * MathHelper.Lerp(1f, 1.2f, backInterpolant);
                Color backColor = Color.Lerp(rarityColor, Color.DarkRed, backInterpolant) * (float)Math.Pow(1f - backInterpolant, 0.46f);
                Vector2 backPosition = basePosition - new Vector2(1f, 0.1f) * backInterpolant * 10f;

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.UIScaleMatrix);

                // Draw the back text as an ominous pulse.
                for (int i = 0; i < 2; i++)
                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, backPosition, backColor, line.Rotation, line.Origin, backScale, line.MaxWidth, line.Spread);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);

                // Draw the front text as usual.
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, basePosition, rarityColor, line.Rotation, line.Origin, line.BaseScale, line.MaxWidth, line.Spread);

                return false;
            }
            return true;
        }
        #endregion

        #region Schematic Knowledge Tooltip Utility
        public static void InsertKnowledgeTooltip(List<TooltipLine> tooltips, int tier, bool allowOldWorlds = false)
        {
            TooltipLine line = new TooltipLine(CalamityMod.Instance, "SchematicKnowledge1", "You don't have sufficient knowledge to create this yet");
            TooltipLine line2 = new TooltipLine(CalamityMod.Instance, "SchematicKnowledge2", "A specific schematic must be deciphered first");
            switch (tier)
            {
                case 1:
                    line2 = new TooltipLine(CalamityMod.Instance, "SchematicKnowledge2", "The Sunken Sea schematic must be deciphered first");
                    break;
                case 2:
                    line2 = new TooltipLine(CalamityMod.Instance, "SchematicKnowledge2", "The Planetoid schematic must be deciphered first");
                    break;
                case 3:
                    line2 = new TooltipLine(CalamityMod.Instance, "SchematicKnowledge2", "The Jungle schematic must be deciphered first");
                    break;
                case 4:
                    line2 = new TooltipLine(CalamityMod.Instance, "SchematicKnowledge2", "The Underworld schematic must be deciphered first");
                    break;
                case 5:
                    line2 = new TooltipLine(CalamityMod.Instance, "SchematicKnowledge2", "The Ice biome schematic must be deciphered first");
                    break;
            }
            line.OverrideColor = line2.OverrideColor = Color.Cyan;

            bool allowedDueToOldWorld = allowOldWorlds && CalamityWorld.IsWorldAfterDraedonUpdate;
            tooltips.AddWithCondition(line, !ArsenalTierGatedRecipe.HasTierBeenLearned(tier) && !allowedDueToOldWorld);
            tooltips.AddWithCondition(line2, !ArsenalTierGatedRecipe.HasTierBeenLearned(tier) && !allowedDueToOldWorld);
        }
        #endregion
    }
}
