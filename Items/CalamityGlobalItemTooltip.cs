using CalamityMod.CalPlayer;
using CalamityMod.CustomRecipes;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor;
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

            // If the item is true melee, add a true melee damage number adjacent to the standard damage number.
            if (item.DamageType == DamageClass.Melee && (item.shoot == ProjectileID.None || trueMelee) && item.damage > 0 && Main.LocalPlayer.Calamity().trueMeleeDamage > 0D)
                TrueMeleeDamageTooltip(item, tooltips);

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

            // If an item has an enchantment, show its prefix in the first tooltip line and append its description to the
            // tooltip list.
            EnchantmentTooltips(item, tooltips);

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
                TooltipLine line = new TooltipLine(Mod, "CalamityDev", CalamityUtils.ColorMessage("- Developer Item -", CalamityUtils.HotPinkRarityColor));
                tooltips.Add(line);
            }
        }
        #endregion

        #region Rarity Coloration
        private void ApplyRarityColor(Item item, TooltipLine nameLine)
        {
            // Apply standard post-ML rarities to the item's color first.
            Color? standardRarityColor = CalamityUtils.GetRarityColor(customRarity);
            if (!item.expert && standardRarityColor.HasValue)
                nameLine.OverrideColor = standardRarityColor.Value;

            #region Uniquely Colored Developer Items
            if (item.type == ModContent.ItemType<Fabstaff>())
                nameLine.OverrideColor = new Color(Main.DiscoR, 100, 255);
            if (item.type == ModContent.ItemType<BlushieStaff>())
                nameLine.OverrideColor = new Color(0, 0, 255);
            if (item.type == ModContent.ItemType<Judgement>())
                nameLine.OverrideColor = Judgement.GetSyncedLightColor();
            if (item.type == ModContent.ItemType<NanoblackReaperRogue>())
                nameLine.OverrideColor = new Color(0.34f, 0.34f + 0.66f * Main.DiscoG / 255f, 0.34f + 0.5f * Main.DiscoG / 255f);
            if (item.type == ModContent.ItemType<ShatteredCommunity>())
                nameLine.OverrideColor = ShatteredCommunity.GetRarityColor();
            if (item.type == ModContent.ItemType<ProfanedSoulCrystal>())
                nameLine.OverrideColor = CalamityUtils.ColorSwap(new Color(255, 166, 0), new Color(25, 250, 25), 4f); //alternates between emerald green and amber (BanditHueh)
            if (item.type == ModContent.ItemType<BensUmbrella>())
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
            if (item.type == ModContent.ItemType<RoyalKnivesMelee>())
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
            if (item.type == ModContent.ItemType<PrototypeAndromechaRing>())
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
                foreach (string line in AppliedEnchantment.Value.Description.Split('\n'))
                {
                    TooltipLine descriptionLine = new TooltipLine(Mod, "Enchantment", CalamityUtils.ColorMessage(line, Color.DarkRed));
                    tooltips.Add(descriptionLine);
                }
            }
        }
        #endregion

        #region Vanilla Item Tooltip Modification

        // Turns a number into a string of increased mining speed.
        public static string MiningSpeedString(int percent) => $"\n{percent}% increased mining speed";

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
            Func<Item, TooltipLine, bool> LineNum(int n) => (Item i, TooltipLine l) => l.Mod == "Terraria" && l.Name == $"Tooltip{n}";
            // This function produces simple predicates to match a specific line of a tooltip, by name.
            Func<Item, TooltipLine, bool> LineName(string s) => (Item i, TooltipLine l) => l.Mod == "Terraria" && l.Name == s;

            // These functions are shorthand to invoke ApplyTooltipEdits using the above predicates.
            void EditTooltipByNum(int lineNum, Action<TooltipLine> action) => ApplyTooltipEdits(tooltips, LineNum(lineNum), action);
            void EditTooltipByName(string lineName, Action<TooltipLine> action) => ApplyTooltipEdits(tooltips, LineName(lineName), action);
            #endregion

            // Numerous random tooltip edits which don't fit into another category
            #region Various Tooltip Edits

            // Destroy this disgusting meme tooltip.
            if (item.type == ItemID.GelBalloon)
                EditTooltipByNum(0, (line) => line.Text = "");

            // Teleporters not working while a boss is alive.
            if (item.type == ItemID.Teleporter)
                EditTooltipByName("Placeable", (line) => line.Text += "\nCannot be used while a boss is alive");

            // Flesh Knuckles giving extra max life.
            if (item.type == ItemID.FleshKnuckles || item.type == ItemID.HeroShield || item.type == ItemID.BerserkerGlove)
                EditTooltipByNum(0, (line) => line.Text += "\nMax life increased by 45");

            // Mirrors and Recall Potions cannot be used while a boss is alive.
            if (item.type == ItemID.MagicMirror || item.type == ItemID.IceMirror || item.type == ItemID.CellPhone || item.type == ItemID.RecallPotion)
                ApplyTooltipEdits(tooltips,
                    (i, l) => l.Mod == "Terraria" && l.Name == (i.type == ItemID.CellPhone ? "Tooltip1" : "Tooltip0"),
                    (line) => line.Text += "\nCannot be used while you have the Boss Effects buff");

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
                EditTooltipByNum(0, (line) => line.Text += "\nDemon Altars no longer spawn ores when destroyed");

            // Bottled Honey gives the Honey buff
            if (item.type == ItemID.BottledHoney)
                EditTooltipByName("HealLife", (line) => line.Text += "\nGrants the Honey buff for 2 minutes");

            // Warmth Potion provides debuff immunities
            if (item.type == ItemID.WarmthPotion)
            {
                string immunityLine = "\nMakes you immune to the Chilled, Frozen, and Glacial State debuffs";
                EditTooltipByNum(0, (line) => line.Text += immunityLine);
            }

            // Nerfed Archery Potion tooltip
            if (item.type == ItemID.ArcheryPotion)
                EditTooltipByNum(0, (line) => line.Text = "20% increased arrow speed and 1.05x arrow damage");

            // Nerfed Swiftness Potion tooltip
            if (item.type == ItemID.SwiftnessPotion)
                EditTooltipByNum(0, (line) => line.Text = "15% increased movement speed");

            // Nerfed Endurance Potion tooltip
            if (item.type == ItemID.EndurancePotion)
                EditTooltipByNum(0, (line) => line.Text = "Reduces damage taken by 5%");

            // Hand Warmer has a side bonus with Eskimo armor
            if (item.type == ItemID.HandWarmer)
            {
                string extraLine = "\nProvides a regeneration boost while wearing the Eskimo armor";
                EditTooltipByNum(0, (line) => line.Text += extraLine);
            }

            // Invisibility Potion provides various rogue boosts
            if (item.type == ItemID.InvisibilityPotion)
                EditTooltipByNum(0, (line) => line.Text += "\nBoosts various rogue stats depending on held weapon");

            // Golden Fishing Rod inherently contains High Test Fishing Line
            if (item.type == ItemID.GoldenFishingRod)
                EditTooltipByName("NeedsBait", (line) => line.Text += "\nIts fishing line will never break");

            // Eternity Crystal notifies the player that they can accelerate the invasion
            if (item.type == ItemID.DD2ElderCrystal)
                EditTooltipByNum(0, (line) => line.Text += "\nOnce placed you can right click the crystal to skip waves or increase the spawn rate of the invaders");

            // Aerial Bane is no longer the real bane of aerial enemies (50% dmg bonus removed)
            if (item.type == ItemID.DD2BetsyBow)
                EditTooltipByNum(0, (line) => line.Text = "Shoots splitting arrows");
            #endregion

            // For boss summon item clarity
            #region Boss Summon Tooltip Edits

            if (item.type == ItemID.Abeemination)
                EditTooltipByNum(0, (line) => line.Text += " when used in the jungle");

            if (item.type == ItemID.BloodySpine)
                EditTooltipByNum(0, (line) => line.Text += " when used in the crimson");

            if (item.type == ItemID.ClothierVoodooDoll)
                EditTooltipByNum(0, (line) => line.Text += "\nWhile equipped, summons Skeletron when the Clothier is killed during nighttime");

            if (item.type == ItemID.DeerThing)
                EditTooltipByNum(0, (line) => line.Text += " when used in the snow or ice biome");

            if (item.type == ItemID.GuideVoodooDoll)
                EditTooltipByNum(0, (line) => line.Text += "\nSummons the Wall of Flesh if thrown into lava in the underworld while the Guide is alive");

            if (item.type == ItemID.LihzahrdPowerCell)
                EditTooltipByNum(0, (line) => line.Text += " to summon the Golem");

            if (item.type == ItemID.MechanicalEye || item.type == ItemID.MechanicalSkull || item.type == ItemID.MechanicalWorm || item.type == ItemID.SuspiciousLookingEye)
                EditTooltipByNum(0, (line) => line.Text += " when used during nighttime");

            if (item.type == ItemID.QueenSlimeCrystal)
                EditTooltipByNum(0, (line) => line.Text += " when used in the hallow");

            if (item.type == ItemID.TruffleWorm)
                EditTooltipByName("Consumable", (line) => line.Text += "\nSummons Duke Fishron if used as bait in the ocean");

            if (item.type == ItemID.WormFood)
                EditTooltipByNum(0, (line) => line.Text += " when used in the corruption");
            #endregion

            // Black Belt and Master Ninja Gear have guaranteed dodges on a 90 second cooldown.
            #region Dodging Belt Tooltips
            string beltDodgeLine = "Grants the ability to dodge attacks\n" +
                $"The dodge has a {CalamityPlayer.BeltDodgeCooldown / 60} second cooldown which is shared with all other dodges and reflects";
            if (item.type == ItemID.BlackBelt)
                EditTooltipByNum(0, (line) => line.Text = beltDodgeLine);
            if (item.type == ItemID.MasterNinjaGear)
                EditTooltipByNum(1, (line) => line.Text = beltDodgeLine);
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
                EditTooltipByName("Knockback", (line) => line.Text += "\nInflicts Holy Flames\nDeals double damage to enemies above 75% life");
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

            if (item.type == ItemID.LightsBane || item.type == ItemID.NightsEdge || item.type == ItemID.TrueNightsEdge || item.type == ItemID.BallOHurt || item.type == ItemID.CorruptYoyo)
                EditTooltipByName("Knockback", (line) => line.Text += "\nInflicts Shadowflame on hit");

            if (item.type == ItemID.BloodButcherer || item.type == ItemID.TheRottedFork || item.type == ItemID.TheMeatball || item.type == ItemID.CrimsonYoyo || item.type == ItemID.CrimsonRod)
                EditTooltipByName("Knockback", (line) => line.Text += "\nInflicts Burning Blood on hit");

            if (item.type == ItemID.DeathSickle)
                EditTooltipByNum(0, (line) => line.Text += "\nInflicts Whispering Death on hit");
            #endregion

            // Light pets, accessories, and other items which boost the player's Abyss light stat
            #region Abyss Light Tooltips

            // +1 to Abyss light level
            string abyssSmallLightLine = "\nProvides a small amount of light in the abyss";

            if (item.type == ItemID.CrimsonHeart || item.type == ItemID.ShadowOrb || item.type == ItemID.MagicLantern || item.type == ItemID.JellyfishNecklace || item.type == ItemID.MiningHelmet)
                EditTooltipByNum(0, (line) => line.Text += abyssSmallLightLine);
            if (item.type == ItemID.JellyfishDivingGear)
                EditTooltipByNum(1, (line) => line.Text += abyssSmallLightLine);

            // +2 to Abyss light level
            string abyssMediumLightLine = "\nProvides a moderate amount of light in the abyss";

            if (item.type == ItemID.ShinePotion)
                EditTooltipByName("BuffTime", (line) => line.Text += abyssMediumLightLine);

            if (item.type == ItemID.FairyBell || item.type == ItemID.DD2PetGhost)
                EditTooltipByNum(0, (line) => line.Text += abyssMediumLightLine);

            // +3 to Abyss light level
            string abyssLargeLightLine = "\nProvides a large amount of light in the abyss";

            if (item.type == ItemID.WispinaBottle)
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
                EditTooltipByNum(0, (line) => line.Text += abyssGreatBreathLine);
            if (item.type == ItemID.CelestialShell)
                EditTooltipByNum(1, (line) => line.Text += abyssModerateBreathLine);
            #endregion

            // Flasks apply to Rogue weapons
            #region Rogue Flask Tooltips
            if (item.type == ItemID.FlaskofCursedFlames)
                EditTooltipByNum(0, (line) => line.Text += "\nRogue attacks inflict enemies with cursed flames");
            if (item.type == ItemID.FlaskofFire)
                EditTooltipByNum(0, (line) => line.Text += "\nRogue attacks set enemies on fire");
            if (item.type == ItemID.FlaskofGold)
                EditTooltipByNum(0, (line) => line.Text += "\nRogue attacks make enemies drop more gold");
            if (item.type == ItemID.FlaskofIchor)
                EditTooltipByNum(0, (line) => line.Text += "\nRogue attacks decrease enemy defense");
            if (item.type == ItemID.FlaskofNanites)
                EditTooltipByNum(0, (line) => line.Text += "\nRogue attacks confuse enemies");
            // party flask is unique because it affects ALL projectiles in Calamity, not just "also rogue ones"
            if (item.type == ItemID.FlaskofParty)
                EditTooltipByNum(0, (line) => line.Text = "All attacks cause confetti to appear");
            if (item.type == ItemID.FlaskofPoison)
                EditTooltipByNum(0, (line) => line.Text += "\nRogue attacks poison enemies");
            if (item.type == ItemID.FlaskofVenom)
                EditTooltipByNum(0, (line) => line.Text += "\nRogue attacks inflict Venom on enemies");
            #endregion

            // Rebalances to vanilla item stats
            #region Vanilla Item Rebalance Tooltips

            // Arcane and Magnet Flower buffs.
            if (item.type == ItemID.ArcaneFlower || item.type == ItemID.MagnetFlower)
                EditTooltipByNum(0, (line) => line.Text = "20% reduced mana usage");

            // Magiluminescence nerf and clear explanation of what it actually does.
            if (item.type == ItemID.Magiluminescence)
                EditTooltipByNum(0, (line) => line.Text = "Increases movement acceleration and deceleration by 1.25x");

            // Soaring Insignia nerf and clear explanation of what it actually does.
            if (item.type == ItemID.EmpressFlightBooster)
            {
                EditTooltipByNum(0, (line) => line.Text = "Increases wing flight time by 50%");
                EditTooltipByNum(1, (line) => line.Text = "Increases movement and jump speed by 10% and acceleration by 1.1x");
            }

            // Magic Power Potion nerf
            if (item.type == ItemID.MagicPowerPotion)
                EditTooltipByNum(0, (line) => line.Text = "10% increased magic damage");

            // Magic and Wizard Hat nerfs
            // Magic Hat
            if (item.type == ItemID.MagicHat)
                EditTooltipByNum(0, (line) => line.Text = "5% increased magic damage and critical strike chance");

            // NOTE -- This is where Fab stopped for the day to play Elden Ring :^)
            // Edit individual tooltips for early hardmode armor sets.
            // Cobalt Hat.
            if (item.type == ItemID.CobaltHat)
                EditTooltipByNum(0, (line) => line.Text = $"Increases maximum mana by {CobaltArmorSetChange.MaxManaBoost + 40}");

            // Palladium Breastplate.
            if (item.type == ItemID.PalladiumBreastplate)
                EditTooltipByNum(0, (line) => line.Text = $"{PalladiumArmorSetChange.ChestplateDamagePercentageBoost + 3}% increased damage.");

            // Palladium Leggings.
            if (item.type == ItemID.PalladiumLeggings)
                EditTooltipByNum(0, (line) => line.Text = $"{PalladiumArmorSetChange.LeggingsDamagePercentageBoost + 2}% increased damage.");

            // Mythril Hood.
            if (item.type == ItemID.MythrilHood)
                EditTooltipByNum(0, (line) => line.Text = $"Increases maximum mana by {MythrilArmorSetChange.MaxManaBoost + 60}");

            // Mythril Chainmail.
            if (item.type == ItemID.MythrilChainmail)
                EditTooltipByNum(0, (line) => line.Text = $"{MythrilArmorSetChange.ChestplateDamagePercentageBoost + 7}% increased damage");

            // Mythril Hood.
            if (item.type == ItemID.MythrilGreaves)
                EditTooltipByNum(0, (line) => line.Text = $"{MythrilArmorSetChange.LeggingsCritChanceBoost + 10}% increased critical strike chance");

            // Orichalcum Breastplate.
            if (item.type == ItemID.OrichalcumBreastplate)
                EditTooltipByNum(0, (line) => line.Text = $"{OrichalcumArmorSetChange.ChestplateCritChanceBoost + 6}% increased critical strike chance");

            // Adamantite Headgear.
            if (item.type == ItemID.AdamantiteHeadgear)
                EditTooltipByNum(0, (line) => line.Text = $"Increases maximum mana by {AdamantiteArmorSetChange.MaxManaBoost + 80}");

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
                EditTooltipByNum(0, (line) => line.Text = "10% increased minion damage and melee critical strike chance\n" +
                "20% increased movement speed");

            // Shinobi Infiltrator armor
            if (item.type == ItemID.MonkAltHead)
                EditTooltipByNum(0, (line) => line.Text = "Increases your max number of sentries by 2\n" +
                "10% increased melee and minion damage");
            if (item.type == ItemID.MonkAltShirt)
                EditTooltipByNum(0, (line) => line.Text = "10% increased minion damage and melee speed\n" +
                "5% increased melee critical strike chance");
            if (item.type == ItemID.MonkAltPants)
                EditTooltipByNum(0, (line) => line.Text = "10% increased minion damage and melee critical strike chance\n" +
                "30% increased movement speed");

            // Red Riding armor
            if (item.type == ItemID.HuntressAltShirt)
                EditTooltipByNum(0, (line) => line.Text = "15% increased minion and ranged damage and 20% chance to not consume ammo");

            // Dark Artist armor
            if (item.type == ItemID.ApprenticeAltPants)
                EditTooltipByNum(0, (line) => line.Text = "10% increased minion damage and magic critical strike chance\n" +
                "20% increased movement speed");

            // Worm Scarf only gives 10% DR instead of 17%
            if (item.type == ItemID.WormScarf)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("17%", "10%"));

            if (item.type == ItemID.TitanGlove)
                EditTooltipByNum(0, (line) => line.Text += "\n10% increased true melee damage");
            if (item.type == ItemID.PowerGlove || item.type == ItemID.MechanicalGlove || item.type == ItemID.BerserkerGlove)
                EditTooltipByNum(1, (line) => line.Text += "\n10% increased true melee damage");
            if (item.type == ItemID.FireGauntlet)
            {
                string extraLine = "\n10% increased true melee damage";
                EditTooltipByNum(1, (line) => line.Text = "14% increased melee damage and speed" + extraLine);
            }

            // On Fire! debuff immunities
            if (item.type == ItemID.ObsidianSkull || item.type == ItemID.AnkhShield)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("fire blocks", "the Burning and On Fire! debuffs"));

            if (item.type == ItemID.ObsidianHorseshoe || item.type == ItemID.ObsidianShield || item.type == ItemID.ObsidianWaterWalkingBoots || item.type == ItemID.LavaWaders)
                EditTooltipByNum(1, (line) => line.Text = line.Text.Replace("fire blocks", "the Burning and On Fire! debuffs"));

            // Yoyo Glove/Bag apply a 0.66x damage multiplier on yoyos
            if (item.type == ItemID.YoyoBag || item.type == ItemID.YoYoGlove)
                EditTooltipByNum(0, (line) => line.Text += "\nYoyos will do 33% less damage");
            #endregion

            // Non-consumable boss summon items
            #region Vanilla Boss Summon Non-consumable Tooltips
            if (item.type == ItemID.SlimeCrown || item.type == ItemID.SuspiciousLookingEye || item.type == ItemID.BloodMoonStarter || item.type == ItemID.GoblinBattleStandard ||
                item.type == ItemID.WormFood || item.type == ItemID.BloodySpine || item.type == ItemID.Abeemination || item.type == ItemID.DeerThing || item.type == ItemID.QueenSlimeCrystal ||
                item.type == ItemID.PirateMap || item.type == ItemID.SnowGlobe || item.type == ItemID.MechanicalEye || item.type == ItemID.MechanicalWorm || item.type == ItemID.MechanicalSkull ||
                item.type == ItemID.NaughtyPresent ||item.type == ItemID.PumpkinMoonMedallion || item.type == ItemID.SolarTablet ||item.type == ItemID.SolarTablet || item.type == ItemID.CelestialSigil)

                EditTooltipByNum(0, (line) => line.Text += "\nNot consumable");
            #endregion

            // Add mentions of what Calamity ores vanilla pickaxes can mine
            #region Pickaxe New Ore Tooltips
            if (item.type == ItemID.Picksaw)
                EditTooltipByNum(0, (line) => line.Text += "\nCan mine Scoria Ore located in the Abyss");

            if (item.type == ItemID.SolarFlarePickaxe || item.type == ItemID.VortexPickaxe || item.type == ItemID.NebulaPickaxe || item.type == ItemID.StardustPickaxe)
                EditTooltipByName("Material", (line) => line.Text += "\nCan mine Uelibloom Ore");
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

            // Obsidian
            if (item.type == ItemID.ObsidianHelm)
                EditTooltipByName("Defense", (line) => line.Text += $"\n{ObsidianArmorSetChange.HelmetRogueDamageBoostPercent}% increased rogue damage");
            if (item.type == ItemID.ObsidianShirt)
                EditTooltipByName("Defense", (line) => line.Text += $"\n{ObsidianArmorSetChange.ChestplateRogueCritBoostPercent}% increased rogue critical strike chance");
            if (item.type == ItemID.ObsidianPants)
                EditTooltipByName("Defense", (line) => line.Text += $"\n{ObsidianArmorSetChange.LeggingRogueVelocityBoostPercent}% increased rogue velocity");

            // Forbidden (UNLESS you are wearing the Circlet, which is Summon/Rogue and does not get this line)
            if (item.type == ItemID.AncientBattleArmorHat || item.type == ItemID.AncientBattleArmorShirt || item.type == ItemID.AncientBattleArmorPants
                && !Main.LocalPlayer.Calamity().forbiddenCirclet)
                EditTooltipByName("SetBonus", (line) => line.Text += "\nThe minion damage nerf is reduced while wielding magic weapons");

            // Stardust
            // 1) Chest and Legs only give 1 minion slot each instead of 2 each.
            if (item.type == ItemID.StardustBreastplate || item.type == ItemID.StardustLeggings)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace('2', '1'));
            #endregion

            // Provide the full stats of every vanilla set of wings
            #region Wing Stat Tooltips

            // This function produces a "stat sheet" for a pair of wings from the raw stats.
            // For "vertical speed", 0 = Average, 1 = Good, 2 = Great.
            string[] vertSpeedStrings = new string[] { "Average vertical speed", "Good vertical speed", "Great vertical speed" };
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

            if (item.type == ItemID.AngelWings)
                AddWingStats(6.25f, 1f, 0, 100, "+20 max life, +10 defense and +2 life regen");

            if (item.type == ItemID.DemonWings)
                AddWingStats(6.25f, 1f, 0, 100, "5% increased damage and critical strike chance");

            if (item.type == ItemID.Jetpack)
                AddWingStats(6.5f, 1f, 0, 115);

            if (item.type == ItemID.ButterflyWings)
                AddWingStats(6.75f, 1f, 0, 130, "+20 max mana, 5% decreased mana usage,\n" +
                    "5% increased magic damage and magic critical strike chance");

            if (item.type == ItemID.FairyWings)
                AddWingStats(6.75f, 1f, 0, 130, "+60 max life");

            if (item.type == ItemID.BeeWings)
                AddWingStats(6.75f, 1f, 0, 130, "Permanently gives the Honey buff");

            if (item.type == ItemID.HarpyWings)
                AddWingStats(7f, 1f, 0, 140, "20% increased movement speed\n" +
                    "With Harpy Ring or Angel Treads equipped, most attacks sometimes launch feathers");

            if (item.type == ItemID.BoneWings)
                AddWingStats(7f, 1f, 0, 140, "10% increased movement speed, ranged damage and critical strike chance\n" +
                    "and +30 defense while wearing the Necro Armor");

            if (item.type == ItemID.FlameWings)
                AddWingStats(7.5f, 1f, 0, 160, "5% increased melee damage and critical strike chance");

            if (item.type == ItemID.FrozenWings)
                AddWingStats(7.5f, 1f, 0, 160, "2% increased melee and ranged damage\n" +
                    "and 1% increased melee and ranged critical strike chance\n" +
                    "while wearing the Frost Armor");

            if (item.type == ItemID.GhostWings)
                AddWingStats(7.5f, 1f, 0, 160, "+10 defense and 5% increased damage reduction while wearing the Spectre Hood set\n" +
                    "5% increased magic damage and critical strike chance while wearing the Spectre Mask set");

            if (item.type == ItemID.BeetleWings)
                AddWingStats(7.5f, 1f, 0, 160, "+10 defense and 5% increased damage reduction while wearing the Beetle Shell set\n" +
                    "5% increased melee damage and critical strike chance while wearing the Beetle Scale Mail set");

            if (item.type == ItemID.FinWings)
                AddWingStats(0f, 0f, 0, 100, "Gills effect and you can move freely through liquids\n" +
                    "You fall faster while submerged in liquid\n" +
                    "15% increased movement speed and 18% increased jump speed");

            if (item.type == ItemID.FishronWings)
                AddWingStats(8f, 2f, 1, 180);

            if (item.type == ItemID.SteampunkWings)
                AddWingStats(7.75f, 1f, 0, 180, "+8 defense, 10% increased movement speed,\n" + "4% increased damage, and 2% increased critical strike chance");

            if (item.type == ItemID.LeafWings)
                AddWingStats(6.75f, 1f, 0, 160, "+5 defense, 5% increased damage reduction,\n" + "and permanent Dryad's Blessing while wearing the Tiki Armor");

            if (item.type == ItemID.BatWings)
                AddWingStats(0f, 0f, 0, 140, "At night or during an eclipse, you will gain the following boosts:\n" +
                    "10% increased movement speed, 10% increased jump speed,\n" +
                    "7% increased damage and 3% increased critical strike chance");

            // All developer wings have identical stats and no special effects
            if (item.type == ItemID.Yoraiz0rWings || item.type == ItemID.JimsWings || item.type == ItemID.SkiphsWings ||
                item.type == ItemID.LokisWings || item.type == ItemID.ArkhalisWings || item.type == ItemID.LeinforsWings ||
                item.type == ItemID.BejeweledValkyrieWing || item.type == ItemID.RedsWings || item.type == ItemID.DTownsWings ||
                item.type == ItemID.WillsWings || item.type == ItemID.CrownosWings || item.type == ItemID.CenxsWings)
            {
                AddWingStats(7f, 1f, 0, 150);
            }

            if (item.type == ItemID.TatteredFairyWings)
                AddWingStats(7.5f, 1f, 0, 180, "5% increased damage and critical strike chance");

            if (item.type == ItemID.SpookyWings)
                AddWingStats(7.5f, 1f, 0, 180, "Increased minion knockback and 5% increased minion damage while wearing the Spooky Armor");

            if (item.type == ItemID.Hoverboard)
                AddWingStats(6.25f, 1f, 0, 170, "10% increased weapon-type damage while wearing the Shroomite Armor\n" +
                    "The weapon type boosted matches which Shroomite helmet is worn");

            if (item.type == ItemID.FestiveWings)
                AddWingStats(7.5f, 1f, 0, 170, "+40 max life\nOrnaments rain down as you fly");

            if (item.type == ItemID.MothronWings)
                AddWingStats(0f, 0f, 0, 160, "+5 defense, 5% increased damage,\n" +
                    "10% increased movement speed and 12% increased jump speed");

            if (item.type == ItemID.WingsSolar)
                AddWingStats(9f, 2.5f, 2, 180, "7% increased melee damage and 3% increased melee critical strike chance\n" +
                    "while wearing the Solar Flare Armor");

            if (item.type == ItemID.WingsStardust)
                AddWingStats(9f, 2.5f, 2, 180, "+1 max minion and 5% increased minion damage while wearing the Stardust Armor");

            if (item.type == ItemID.WingsVortex)
                AddWingStats(6.5f, 1.5f, 1, 160, "3% increased ranged damage and 7% increased ranged critical strike chance\n" +
                    "while wearing the Vortex Armor");

            if (item.type == ItemID.WingsNebula)
                AddWingStats(6.5f, 1.5f, 1, 160, "+20 max mana, 5% increased magic damage and critical strike chance,\n" +
                    "and 5% decreased mana usage while wearing the Nebula Armor");

            // Betsy's Wings (and dev wings) are the only wings without "Allows flight and free fall"
            if (item.type == ItemID.BetsyWings)
                AddWingStats2(6f, 2.5f, 1, 150, null, "Equipable");
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
            if (item.type == ItemID.TopazHook)
                AddGrappleStats(20.625f, 10.5f, 11.75f, 11f);
            if (item.type == ItemID.SapphireHook)
                AddGrappleStats(22.5f, 11f, 12.5f, 11f);
            if (item.type == ItemID.EmeraldHook)
                AddGrappleStats(24.375f, 11.5f, 13.25f, 11f);
            if (item.type == ItemID.RubyHook)
                AddGrappleStats(26.25f, 12f, 14f, 11f);
            if (item.type == ItemID.DiamondHook)
                AddGrappleStats(28.125f, 12.5f, 14.75f, 11f);
            if (item.type == ItemID.WebSlinger)
                AddGrappleStats(15.625f, 10f, 11f, 11f);
            if (item.type == ItemID.SkeletronHand)
                AddGrappleStats(21.875f, 15f, 11f, 11f);
            if (item.type == ItemID.SlimeHook)
                AddGrappleStats(18.75f, 13f, 11f, 11f);
            if (item.type == ItemID.FishHook)
                AddGrappleStats(25f, 13f, 11f, 11f);
            if (item.type == ItemID.IvyWhip)
                AddGrappleStats(25f, 13f, 15f, 11f);
            if (item.type == ItemID.BatHook) // TODO -- This item should be dropped by Vampires in the Eclipse. It is very overpowered.
                AddGrappleStats(31.25f, 15.5f, 20f, 16f);
            if (item.type == ItemID.CandyCaneHook)
                AddGrappleStats(25f, 11.5f, 11f, 11f);
            if (item.type == ItemID.DualHook)
                AddGrappleStats(27.5f, 14f, 17f, 11f);
            // these four grapple hooks are all functionally identical
            if (item.type == ItemID.ThornHook || item.type == ItemID.WormHook || item.type == ItemID.TendonHook || item.type == ItemID.IlluminantHook)
                AddGrappleStats(30f, 15f, 18f, 11f);
            if (item.type == ItemID.AntiGravityHook)
                AddGrappleStats(31.25f, 14f, 20f, 11f);
            if (item.type == ItemID.SpookyHook)
                AddGrappleStats(34.375f, 15.5f, 22f, 11f);
            if (item.type == ItemID.ChristmasHook)
                AddGrappleStats(34.375f, 15.5f, 17f, 11f);
            if (item.type == ItemID.LunarHook)
                AddGrappleStats(34.375f, 16f, 24f, 13f);
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
            }
            #endregion
        }
        #endregion

        #region True Melee Damage Tooltip
        private void TrueMeleeDamageTooltip(Item item, IList<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault((l) => l.Mod == "Terraria" && l.Name == "Damage");

            // If there somehow isn't a damage tooltip line, do not try to perform any edits.
            if (line is null)
                return;

            // Start with the existing line of melee damage.
            StringBuilder sb = new StringBuilder(64);
            sb.Append(line.Text).Append(" : ");

            Player p = Main.LocalPlayer;
            float itemCurrentDamage = item.damage * p.MeleeDamage();
            double trueMeleeBoost = 1D + p.Calamity().trueMeleeDamage;
            double imprecisionRoundingCorrection = 5E-06D;
            int damageToDisplay = (int)(itemCurrentDamage * trueMeleeBoost + imprecisionRoundingCorrection);
            sb.Append(damageToDisplay);

            // These two pieces are split apart for ease of translation
            sb.Append(' ');
            sb.Append("true melee damage");
            line.Text = sb.ToString();
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
                TooltipLine StealthGen = new TooltipLine(Mod, "PrefixStealthGenBoost", "+" + Math.Round(stealthGenBoost * 100f) + "% stealth generation")
                {
                    IsModifier = true
                };
                tooltips.Add(StealthGen);
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
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

                // Draw the back text as an ominous pulse.
                for (int i = 0; i < 2; i++)
                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, backPosition, backColor, line.Rotation, line.Origin, backScale, line.MaxWidth, line.Spread);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin();

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
            line.OverrideColor = line2.OverrideColor = Color.Cyan;

            bool allowedDueToOldWorld = allowOldWorlds && CalamityWorld.IsWorldAfterDraedonUpdate;
            tooltips.AddWithCondition(line, !ArsenalTierGatedRecipe.HasTierBeenLearned(tier) && !allowedDueToOldWorld);
            tooltips.AddWithCondition(line2, !ArsenalTierGatedRecipe.HasTierBeenLearned(tier) && !allowedDueToOldWorld);
        }
        #endregion
    }
}
