using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalamityMod.Balancing;
using CalamityMod.ChatTags;
using CalamityMod.CustomRecipes;
using CalamityMod.DataStructures;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.Armor.Demonshade;
using CalamityMod.Items.Tools;
using CalamityMod.Items.VanillaArmorChanges;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Prefixes;
using CalamityMod.Systems.Collections;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Items
{
    public partial class CalamityGlobalItem : GlobalItem
    {
        #region Backup Tooltip Insertion Positions
        /// <summary>
        /// This array contains (almost) every single vanilla tooltip in reverse order starting at "Tooltip0".<br />
        /// Because "Tooltip0" is the first typical tooltip line, this is where Calamity tends to insert its tooltips.<br />
        /// When this line is not present, Calamity needs to insert tooltips in an <i>equivalent</i> position.<br />
        /// The best way to do this is to iterate backwards through all possible vanilla tooltip lines and pick the first one that is present.
        /// </summary>
        private static string[] MainTooltipBackupInsertionPositions =
        {
            "Material",
            "Consumable",
            "Ammo",
            "Placeable",
            "UseMana",
            "HealMana",
            "HealLife",
            "TileBoost",
            "HammerPower",
            "AxePower",
            "PickPower",
            "Defense",
            "Vanity",
            "Quest",
            "WandConsumes",
            "Equipable",
            "BaitPower",
            "NeedsBait",
            "FishingPower",
            "Knockback",
            "NoTransfer",
            "FavoriteDesc",
            "ItemName",
        };

        /// <summary>
        /// This array contains (almost) every single vanilla tooltip in reverse order starting at "Expert" and ending at "Tooltip0".<br />
        /// Because "Tooltip0" is the first typical tooltip line, this is the earliest conceivable place where a "Revengeance" marker can be inserted.<br />
        /// Since none of these tooltip lines are guaranteed to exist, Calamity needs to iterate through all of them to find a suitable insertion point.<br />
        /// The best way to do this is to iterate backwards through all possible vanilla tooltip lines and pick the first one that is present.
        /// </summary>
        private static string[] RevTooltipInsertionPositions =
        {
            "Expert",
            "SetBonus",
            RogueAccessoryPrefix.StealthTooltipID,
            "PrefixAccMeleeSpeed",
            "PrefixAccMoveSpeed",
            "PrefixAccDamage",
            "PrefixAccCritChance",
            "PrefixAccMaxMana",
            "PrefixAccDefense",
            RogueWeaponPrefix.StealthTooltipID,
            "PrefixKnockback",
            "PrefixShootSpeed",
            "PrefixSize",
            "PrefixUseMana",
            "PrefixCritChance",
            "PrefixSpeed",
            "PrefixDamage",
            "OneDropLogo",
            "BuffTime",
            "WellFedExpert",
            "EtherianManaWarning",
        };
        #endregion

        #region Main ModifyTooltips Function
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            // Get the first index, last index and total count of standard vanilla tooltip lines.
            // The first index and count are used to delete all vanilla tooltips when holding SHIFT, if requested.
            // The last index is used to insert various extra tooltip lines in the right position.
            //
            // This code used to be in the HoldShiftTooltip utility, but is needed to correctly place other tooltips.
            int firstTooltipIndex = -1;
            int lastTooltipIndex = -1;
            int standardTooltipCount = 0;
            for (int i = 0; i < tooltips.Count; i++)
            {
                if (tooltips[i]?.Name?.StartsWith("Tooltip") == true)
                {
                    if (firstTooltipIndex == -1)
                        firstTooltipIndex = i;
                    lastTooltipIndex = i;
                    standardTooltipCount++;
                }
            }

            // If there are no standard vanilla tooltip lines (e.g. Flintlock Pistol, which has no tooltip)
            // then a different position needs to be selected for typical insertion.
            bool noStandardTooltips = false;
            if (firstTooltipIndex == -1)
            {
                noStandardTooltips = true;
                foreach (string lineName in MainTooltipBackupInsertionPositions)
                {
                    int idx = tooltips.FindIndex((line) => line.Name == lineName);
                    if (idx != -1)
                    {
                        firstTooltipIndex = lastTooltipIndex = idx;
                        break;
                    }
                }
            }

            // Apply custom rarity coloration to the item's name if applicable.
            TooltipLine nameLine = tooltips.FirstOrDefault(x => x.Name == "ItemName" && x.Mod == "Terraria");
            if (nameLine != null)
                ApplyRarityColor(item, nameLine);

            // Modify all vanilla tooltips before appending mod mechanics (if any).
            ModifyVanillaTooltips(item, tooltips);

            // If an item has an enchantment, show its prefix in the first tooltip line and append its description to the tooltip list.
            EnchantmentTooltips(item, tooltips);

            WhipAutomaticTooltips(item, tooltips, ref lastTooltipIndex);
            // In GFB, replace all instances of "rogue" with "rouge".
            string[] rogueKey = [CalamityUtils.GetTextValue($"Misc.GFBRogueUppercase"), CalamityUtils.GetTextValue($"Misc.GFBRogueLowercase")];
            string[] rougeKey = [CalamityUtils.GetTextValue($"Misc.GFBRougeUppercase"), CalamityUtils.GetTextValue($"Misc.GFBRougeLowercase")];
            for (int n = 0; n < rogueKey.Length; n++)
            {
                if (Main.zenithWorld && rogueKey[n] != "")
                {
                    tooltips.FindAndReplace(rogueKey[n], rougeKey[n]);
                }
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
                TooltipLine line = new TooltipLine(Mod, "CalamityMod:Charge", CalamityUtils.GetText("Misc.Charge").Format(displayedPercent.ToString("N1")));
                tooltips.Insert(++lastTooltipIndex, line);
            }

            // Generic mechanical implementation of any and all Hold SHIFT tooltips.
            // For more information, see IHoldShiftTooltipItem.
            //
            // Original code lifted from Iban's extended armor tooltips.
            if (item.ModItem is IHoldShiftTooltipItem holdShiftItem)
            {
                bool holdingShift = Main.keyState.PressingShift();

                // If holding SHIFT, actually display the extended tooltip.
                if (holdingShift && firstTooltipIndex != -1)
                {
                    string holdShiftText = item.ModItem.GetLocalizedValue(holdShiftItem.TooltipExtensionKey);
                    TooltipLine holdShiftLine = new TooltipLine(Mod, IHoldShiftTooltipItem.ExtensionTooltipID, holdShiftText);
                    if (holdShiftItem.TooltipExtensionColor is not null)
                        holdShiftLine.OverrideColor = holdShiftItem.TooltipExtensionColor;

                    // If asked to, remove all standard tooltip lines. This moves the last tooltip index.
                    // This only occurs if the standard tooltip lines are ACTUALLY standard tooltips. Otherwise, don't remove anything!
                    if (holdShiftItem.HidesNormalTooltip && !noStandardTooltips)
                    {
                        tooltips.RemoveRange(firstTooltipIndex, standardTooltipCount);
                        lastTooltipIndex -= standardTooltipCount;
                    }

                    // Append the "Hold SHIFT" tooltip at the end of standard tooltips.
                    tooltips.Insert(++lastTooltipIndex, holdShiftLine);
                }

                // If not holding SHIFT, display the extension indicator if appropriate.
                if (!holdingShift && holdShiftItem.ShowExtensionIndicator)
                {
                    LocalizedText indicatorText = CalamityUtils.GetText(holdShiftItem.ExtensionIndicatorKey);
                    TooltipLine indicator = new TooltipLine(Mod, IHoldShiftTooltipItem.ExtensionIndicatorTooltipID, indicatorText.Value);
                    if (holdShiftItem.ExtensionIndicatorColor is not null)
                        indicator.OverrideColor = holdShiftItem.ExtensionIndicatorColor;

                    // Append the extension indicator tooltip at the end of standard tooltips.
                    tooltips.Insert(++lastTooltipIndex, indicator);
                }

                // Generic support for flavor tooltips.
                // This is only necessary on items with Hold SHIFT tooltips.
                // The extended tooltip and tooltip extension indicator are placed above flavor tooltips for vanilla consistency.
                //
                // Flavor tooltips display unconditionally if defined. They are visible both when holding SHIFT and when not.
                if (holdShiftItem.HasFlavorTooltip && holdShiftItem.FlavorTooltipKey is not null)
                {
                    string flavorText = item.ModItem.GetLocalizedValue(holdShiftItem.FlavorTooltipKey);
                    TooltipLine flavorLine = new TooltipLine(Mod, IHoldShiftTooltipItem.FlavorTooltipID, flavorText);
                    if (holdShiftItem.FlavorTooltipColor is not null)
                        flavorLine.OverrideColor = holdShiftItem.FlavorTooltipColor;

                    // Append the flavor tooltip at the end of standard tooltips, after all Hold SHIFT tooltips and reminders.
                    tooltips.Insert(++lastTooltipIndex, flavorLine);
                }
            }

            //
            // "Late" tooltips are all inserted after vanilla's "Expert" and "Master" markers.
            //

            // The best possible position is identified using a separate backwards search.
            int difficultyTooltipIndex = -1;
            foreach (string lineName in RevTooltipInsertionPositions)
            {
                int idx = tooltips.FindIndex((line) => line.Name == lineName);
                if (idx != -1)
                {
                    difficultyTooltipIndex = idx;
                    break;
                }
            }

            // If the backwards search fails, it defaults to the last known tooltip index from the previous search.
            if (difficultyTooltipIndex == -1)
                difficultyTooltipIndex = lastTooltipIndex;

            // Adds "Revengeance" to all items which are Revengeance exclusive, like how vanilla does it for Expert and Master items.
            if (revengeanceItem)
            {
                LocalizedText revText = CalamityUtils.GetText("UI.Revengeance");
                TooltipLine revLine = new TooltipLine(Mod, "CalamityMod:RevengeanceItem", revText.Value);
                tooltips.Insert(++difficultyTooltipIndex, revLine);
            }

            // Adds "Donor Item" and "Developer Item" to donor items and developer items respectively.
            // This is intentionally at the bottom, below everything else.
            if (devItem)
            {
                LocalizedText devText = CalamityUtils.GetText("UI.DevItemTooltip");
                string coloredText = CalamityUtils.ColorMessage(devText.Value, CalamityUtils.DevItemColor);
                TooltipLine devLine = new TooltipLine(Mod, "CalamityMod:DevItem", coloredText);
                tooltips.Insert(++difficultyTooltipIndex, devLine);
            }
            else if (donorItem)
            {
                LocalizedText donorText = CalamityUtils.GetText("UI.DonorItemTooltip");
                string coloredText = CalamityUtils.ColorMessage(donorText.Value, CalamityUtils.DonatorItemColor);
                TooltipLine donorLine = new TooltipLine(Mod, "CalamityMod:DonorItem", coloredText);
                tooltips.Insert(++difficultyTooltipIndex, donorLine);
            }

            var buffIdsInTooltip = new HashSet<int>();

            foreach (var tooltip in tooltips)
            {
                // Parse the tags of each line of text to find our buff tags'
                // snippets (since they store the buff IDs).
                var snippets = ChatManager.ParseMessage(tooltip.Text, Color.White);
                foreach (var snippet in snippets)
                {
                    if (snippet is CalamityBuffTagHandler.Snippet buffSnippet)
                    {
                        buffIdsInTooltip.Add(buffSnippet.BuffId);
                    }
                }
            }

            if (buffIdsInTooltip.Count > 0)
            {
                bool showTheTip = false;
                bool foundDebuff = false;
                foreach (int buffId in buffIdsInTooltip)
                {
                    string tooltipKey = "";
                    if (buffId < BuffID.Count)
                    {
                        tooltipKey = $"Mods.Terraria.Buffs.{BuffID.Search.GetName(buffId)}.ItemTooltip";
                    }
                    else
                    {
                        var modBuff = BuffLoader.GetBuff(buffId);
                        tooltipKey = $"Mods.{modBuff.Mod.Name}.Buffs.{modBuff.Name}.ItemTooltip";
                    }

                    if (!Language.Exists(tooltipKey))
                    {
                        continue;
                    }

                    var text = Language.GetTextValue(tooltipKey);
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        continue;
                    }

                    foundDebuff = true;
                    if (!PlayerInput.Triggers.Current.SmartCursor)
                    {
                        showTheTip = true;
                        break;
                    }

                    tooltips.Insert(++lastTooltipIndex, new TooltipLine(Mod, "CalamityMod:AltExpandTooltip" + buffId, $"[cbuff:{buffId}]\n{text}"));
                }

                if (showTheTip)
                {
                    var str = PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard].KeyStatus["SmartCursor"].First().ToString();
                    tooltips.Insert(++lastTooltipIndex, (new TooltipLine(Mod, "CalamityMod:AltExpandTooltip", CalamityUtils.GetTextValue("Misc.AltExpand").Replace("{0}", str))));
                    tooltips[lastTooltipIndex].OverrideColor = new Color(170, 170, 170);
                }
                else if (foundDebuff)
                {
                    foreach (var item1 in tooltips)
                    {
                        if (item1.Name.Contains("Tooltip") && !item1.Name.Contains("AltExpandTooltip"))
                            item1.Hide();
                    }
                }
            }
        }
        #endregion

        #region Rarity Coloration
        private static void ApplyRarityColor(Item item, TooltipLine nameLine)
        {
            if (item.type == ModContent.ItemType<TheCommunity>())
                nameLine.OverrideColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);

            // Developer items
            if (item.type == ModContent.ItemType<Sylvestaff>())
                nameLine.OverrideColor = new Color(249, 197, 255);
            if (item.type == ModContent.ItemType<StaffofBlushie>())
                nameLine.OverrideColor = new Color(0, 0, 255);
            if (item.type == ModContent.ItemType<TheDanceofLight>())
                nameLine.OverrideColor = TheDanceofLight.GetSyncedLightColor();
            if (item.type == ModContent.ItemType<NanoblackReaper>())
                nameLine.OverrideColor = new Color(0.34f, 0.34f + 0.66f * Main.DiscoG / 255f, 0.34f + 0.5f * Main.DiscoG / 255f);
            if (item.type == ModContent.ItemType<ShatteredCommunity>())
                nameLine.OverrideColor = ShatteredCommunity.GetRarityColor();
            if (item.type == ModContent.ItemType<Ozzathoth>())
                nameLine.OverrideColor = ShatteredCommunity.GetRarityColor();
            if (item.type == ModContent.ItemType<ProfanedSoulCrystal>())
                nameLine.OverrideColor = CalamityUtils.ColorSwap(new Color(255, 166, 0), new Color(25, 250, 25), 6f); //alternates between emerald green and amber (BanditHueh)
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
            if (item.type == ModContent.ItemType<TriactisTruePaladinianMageHammerofMight>())
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
                    Color.OrangeRed,
                    Color.MediumTurquoise,
                    Color.LimeGreen
                };
                if (nameLine != null)
                {
                    int colorIndex = (int)(Main.GlobalTimeWrappedHourly / 2 % earthColors.Count);
                    Color currentColor = earthColors[colorIndex];
                    Color nextColor = earthColors[(colorIndex + 1) % earthColors.Count];
                    nameLine.OverrideColor = Color.Lerp(currentColor, nextColor, Main.GlobalTimeWrappedHourly % 2f > 1f ? 1f : Main.GlobalTimeWrappedHourly % 1f);
                }
            }
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

        #region Whip Tooltips
        private void WhipAutomaticTooltips(Item item, IList<TooltipLine> tooltips, ref int lastTooltipIndex)
        {
            // Multiplicative tag changes
            string FlatTagTooltip(int dmg) => (CalamityUtils.GetText($"Common.SummonTagDamageFlat").Format(dmg.ToString()));
            string MultTagTooltip(float mult) => (CalamityUtils.GetText($"Common.SummonTagDamageMult").Format((mult + 1).ToString("0.##")));
            string CritTagTooltip(float crit) => (CalamityUtils.GetText($"Common.SummonTagCrit").Format((crit * 100).ToString("0.#")));

            Dictionary<int, SummonTag> TagByItem = new();
            foreach (SummonTag tag1 in CalamityBuffSets.SummonTagDebuff.Values)
            {
                if (tag1.TagItem > -1) TagByItem.Add(tag1.TagItem, tag1);
            }
            if (TagByItem.TryGetValue(item.type, out SummonTag tag))
            {
                if (!tag.AutoDrawTooltip) return;
                var modPlayer = Main.LocalPlayer.Calamity();
                if (tag.FlatTagDamage != 0)
                {
                    TooltipLine line = new TooltipLine(Mod, "CalamityMod:FlatSummonTag", FlatTagTooltip(tag.FlatTagDamage));
                    tooltips.Insert(++lastTooltipIndex, line);
                }
                if (!modPlayer.forceSummonTagCrit && (tag.MultiplicativeTagDamage != 0 || (modPlayer.forceSummonTagMultiplicative && tag.TagCritChance != 0)))
                {
                    TooltipLine line = new TooltipLine(Mod, "CalamityMod:MultiplicativeSummonTag", MultTagTooltip(tag.MultiplicativeTagDamage + (modPlayer.forceSummonTagMultiplicative ? tag.TagCritChance : 0)));
                    tooltips.Insert(++lastTooltipIndex, line);
                }
                if (!modPlayer.forceSummonTagMultiplicative && (tag.TagCritChance != 0 || (modPlayer.forceSummonTagCrit && tag.MultiplicativeTagDamage != 0)))
                {
                    TooltipLine line = new TooltipLine(Mod, "CalamityMod:CritSummonTag", CritTagTooltip(tag.TagCritChance + (modPlayer.forceSummonTagCrit ? tag.MultiplicativeTagDamage : 0)));
                    tooltips.Insert(++lastTooltipIndex, line);
                }
                if (modPlayer.forceSummonTagMultiplicative && modPlayer.forceSummonTagCrit) //when both tag and crit are forced, swaps between the two
                {
                    TooltipLine line = new TooltipLine(Mod, "CalamityMod:CritSummonTag", ((int)(Main.GlobalTimeWrappedHourly / 5) % 2 == 0 ? CritTagTooltip(tag.MultiplicativeTagDamage + tag.TagCritChance) : MultTagTooltip(tag.MultiplicativeTagDamage + tag.TagCritChance)));
                    tooltips.Insert(++lastTooltipIndex, line);
                }
            }
        }
        #endregion

        #region Vanilla Item Tooltip Modification
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
            string EditedTooltip(string key) => CalamityUtils.GetTextValue($"Vanilla.EditedTooltip.{key}");
            LocalizedText GetEditedTooltip(string key) => CalamityUtils.GetText($"Vanilla.EditedTooltip.{key}");

            // For items such as a Copper Helmet which literally have no tooltips at all, add a custom "Tooltip0" which mimics the vanilla Tooltip0.
            void AddTooltip(string key)
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
                tooltips.Insert(defenseIndex + 1, new TooltipLine(CalamityMod.Instance, "Tooltip0", CalamityUtils.GetTextValue($"Vanilla.AddedTooltip.{key}")));
            }
            string AddedTooltip(string key) => "\n" + CalamityUtils.GetTextValue($"Vanilla.AddedTooltip.{key}");
            LocalizedText GetAddedTooltip(string key) => CalamityUtils.GetText($"Vanilla.AddedTooltip.{key}");
            #endregion

            // Applies to various item categories to clarify exact regen effects
            #region Life Regen Clarity Tooltips
            bool isCampfire = item.type == ItemID.Campfire || item.type == ItemID.CursedCampfire || item.type == ItemID.DemonCampfire || item.type == ItemID.FrozenCampfire || item.type == ItemID.IchorCampfire || item.type == ItemID.RainbowCampfire || item.type == ItemID.UltraBrightCampfire || item.type == ItemID.BoneCampfire || item.type == ItemID.DesertCampfire || item.type == ItemID.CoralCampfire || item.type == ItemID.CorruptCampfire || item.type == ItemID.CrimsonCampfire || item.type == ItemID.HallowedCampfire || item.type == ItemID.JungleCampfire || item.type == ItemID.MushroomCampfire || item.type == ItemID.ShimmerCampfire;
            if (isCampfire)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("Campfires"));

            if (item.type == ItemID.HeartLantern)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("HeartLantern"));

            if (item.type == ItemID.BottledHoney)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("BottledHoney"));

            if (item.type == ItemID.ShinyStone)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("ShinyStone"));

            if (item.type == ItemID.BandofRegeneration)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("BandofRegeneration"));

            if (item.type == ItemID.CharmofMyths)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("CharmofMyths"));

            if (item.type == ItemID.RegenerationPotion)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("RegenerationPotion"));

            if (item.type == ItemID.SoulDrain)
                EditTooltipByNum(0, (line) => line.Text += AddedTooltip("SoulDrain"));

            if (item.type == ItemID.HamBat)
                EditTooltipByNum(1, (line) => line.Text = EditedTooltip("HamBat"));

            if (item.type == ItemID.AegisCrystal) // Vital Crystal
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("AegisCrystal"));

            if (item.type == ItemID.SquireGreatHelm)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("SquireGreatHelm"));

            if (item.type == ItemID.SquireAltShirt) // Valhalla Knight's Breastplate
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("SquireAltShirt"));

            if (item.type == ItemID.SolarFlareHelmet || item.type == ItemID.SolarFlareBreastplate || item.type == ItemID.SolarFlareLeggings)
                EditTooltipByNum(1, (line) => line.Text = EditedTooltip("SolarFlarePieces"));
            #endregion

            // Applies to various item categories which carry their light/breath effects into the Abyss
            #region Abyss Light/Breath
            // +1 to Abyss light level
            if (item.type == ItemID.JellyfishNecklace ||
                item.type == ItemID.MiningHelmet || item.type == ItemID.UltrabrightHelmet)
                EditTooltipByNum(0, (line) => line.Text += "\n" + CalamityUtils.GetTextValue("Common.AbyssGlow"));
            if (item.type == ItemID.JellyfishDivingGear)
                EditTooltipByNum(1, (line) => line.Text += "\n" + CalamityUtils.GetTextValue("Common.AbyssGlow"));

            // +2 to Abyss light level
            if (item.type == ItemID.ShinePotion)
                EditTooltipByName("BuffTime", (line) => line.Text += "\n" + CalamityUtils.GetTextValue("Common.AbyssGlow"));

            // Moderate breath boost
            if (item.type == ItemID.DivingHelmet)
                EditTooltipByNum(0, (line) => line.Text += "\n" + CalamityUtils.GetTextValue("Common.AbyssBreathLevel2"));
            if (item.type == ItemID.ArcticDivingGear)
                EditTooltipByNum(1, (line) => line.Text += "\n" + CalamityUtils.GetTextValue("Common.AbyssLightLevel") + "\n" + CalamityUtils.GetTextValue("Common.AbyssBreathLevel2"));

            // Great breath boost
            if (item.type == ItemID.GillsPotion)
                EditTooltipByName("BuffTime", (line) => line.Text += "\n" + CalamityUtils.GetTextValue("Common.AbyssBreathLevel3"));

            if (item.type == ItemID.NeptunesShell || item.type == ItemID.MoonShell)
                EditTooltipByNum(1, (line) => line.Text += "\n" + CalamityUtils.GetTextValue("Common.AbyssBreathLevel3"));
            if (item.type == ItemID.CelestialShell)
                EditTooltipByNum(4, (line) => line.Text += "\n" + CalamityUtils.GetTextValue("Common.AbyssBreathLevel3"));
            #endregion

            // Spawn, despawn, and enrage conditions + Non-consumable line
            #region Boss Summons
            if (item.type == ItemID.Abeemination)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("Abeemination"));

            if (item.type == ItemID.BloodySpine)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("BloodySpine"));

            if (item.type == ItemID.ClothierVoodooDoll)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("ClothierVoodooDoll"));

            if (item.type == ItemID.DeerThing)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("DeerThing"));

            if (item.type == ItemID.GuideVoodooDoll)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("GuideVoodooDoll"));

            if (item.type == ItemID.LihzahrdPowerCell)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("LihzahrdPowerCell"));

            if (item.type == ItemID.MechanicalEye)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("MechanicalEye"));

            if (item.type == ItemID.MechanicalSkull)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("MechanicalSkull"));

            if (item.type == ItemID.MechanicalWorm)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("MechanicalWorm"));

            if (item.type == ItemID.QueenSlimeCrystal)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("QueenSlimeCrystal"));

            if (item.type == ItemID.SuspiciousLookingEye)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("SuspiciousLookingEye"));

            if (item.type == ItemID.TruffleWorm)
                EditTooltipByName("Consumable", (line) => line.Text += AddedTooltip("TruffleWorm"));

            if (item.type == ItemID.WormFood)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("WormFood"));

            if (item.type == ItemID.SlimeCrown || item.type == ItemID.SuspiciousLookingEye || item.type == ItemID.WormFood || item.type == ItemID.BloodySpine || item.type == ItemID.Abeemination || item.type == ItemID.DeerThing
                || item.type == ItemID.QueenSlimeCrystal || item.type == ItemID.MechanicalEye || item.type == ItemID.MechanicalWorm || item.type == ItemID.MechanicalSkull || item.type == ItemID.CelestialSigil)
                EditTooltipByNum(0, (line) => line.Text += "\n" + CalamityUtils.GetTextValue("Common.NotConsumable"));
            #endregion
            // Brain of Confusion, Black Belt and Master Ninja Gear have guaranteed dodges with a fixed cooldown.
            #region Guaranteed Dodge Tooltips
            if (item.type == ItemID.BlackBelt)
                EditTooltipByNum(0, (line) => line.Text = CalamityUtils.GetTextValue("Common.DodgeProvided") + "\n" + CalamityUtils.GetTextValue("Common.DodgeInformation"));
            if (item.type == ItemID.MasterNinjaGear)
                EditTooltipByNum(1, (line) => line.Text = CalamityUtils.GetTextValue("Common.DodgeProvided") + "\n" + CalamityUtils.GetTextValue("Common.DodgeInformation"));
            if (item.type == ItemID.BrainOfConfusion)
            {
                EditTooltipByNum(0, (line) => line.Text = CalamityUtils.GetTextValue("Common.DodgeProvided"));

                EditTooltipByNum(2, (line) => line.Text += "\n" + CalamityUtils.GetTextValue("Common.DodgeInformation"));
            }
            #endregion

            // Whip tag is dynamically generated for all whips based on the SummonTagDebuffDict, so we'll remove the vanilla tag tootlips.
            #region Whip Tag removal
            // Additive tag changes
            if (item.type == ItemID.BlandWhip)
                EditTooltipByNum(0, (line) => line.Text = string.Empty);
            if (item.type == ItemID.ThornWhip)
                EditTooltipByNum(0, (line) => line.Text = string.Empty);
            if (item.type == ItemID.BoneWhip)
                EditTooltipByNum(0, (line) => line.Text = string.Empty);
            if (item.type == ItemID.CoolWhip)
                EditTooltipByNum(0, (line) => line.Text = string.Empty);
            if (item.type == ItemID.SwordWhip)
                EditTooltipByNum(0, (line) => line.Text = string.Empty);
            if (item.type == ItemID.MaceWhip)
            {
                EditTooltipByNum(0, (line) => line.Text = string.Empty);
                EditTooltipByNum(1, (line) => line.Text = string.Empty);
            }
            if (item.type == ItemID.RainbowWhip)
            {
                EditTooltipByNum(0, (line) => line.Text = string.Empty);
                EditTooltipByNum(1, (line) => line.Text = string.Empty);
            }
            #endregion

            #region Accessories

            // Nerfed Ancient Chisel and its upgrade.
            if (item.type == ItemID.AncientChisel)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("25%", "15%"));
            if (item.type == ItemID.HandOfCreation)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("25%", "15%"));

            // Melee speed removed from the Celestial Stone line.
            if (item.type == ItemID.MoonStone)
                EditTooltipByNum(1, (line) => line.Text = EditedTooltip("SunMoonStones"));
            if (item.type == ItemID.SunStone)
                EditTooltipByNum(1, (line) => line.Text = EditedTooltip("SunMoonStones"));
            if (item.type == ItemID.CelestialStone)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("CelestialStoneShell"));
            if (item.type == ItemID.CelestialShell)
                EditTooltipByNum(2, (line) => line.Text = EditedTooltip("CelestialStoneShell"));

            // Feral Claws line melee speed and true melee damage changes
            if (item.type == ItemID.FeralClaws)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("12%", "10%"));
            if (item.type == ItemID.TitanGlove)
                EditTooltipByNum(0, (line) => line.Text += AddedTooltip("TitanGloveLine"));
            if (item.type == ItemID.PowerGlove)
            {
                EditTooltipByNum(1, (line) => line.Text = EditedTooltip("PowerGlove"));
                EditTooltipByNum(0, (line) => line.Text += AddedTooltip("TitanGloveLine"));
            }
            if (item.type == ItemID.BerserkerGlove)
                EditTooltipByNum(1, (line) => line.Text = EditedTooltip("BerserkerGlove"));
            if (item.type == ItemID.MechanicalGlove)
                EditTooltipByNum(1, (line) => line.Text = EditedTooltip("MechanicalGlove") + AddedTooltip("TitanGloveLine"));
            if (item.type == ItemID.FireGauntlet)
            {
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("FireGauntlet1"));
                EditTooltipByNum(1, (line) => line.Text = EditedTooltip("FireGauntlet2") + AddedTooltip("TitanGloveLine"));
            }

            // Yoyo Glove/Bag apply a 0.5x damage multiplier on the second yoyo
            if (item.type == ItemID.YoyoBag || item.type == ItemID.YoYoGlove)
                EditTooltipByNum(0, (line) => line.Text += AddedTooltip("YoyoGlove"));

            // Molten Quiver sets Hellfire on all arrows.
            if (item.type == ItemID.MoltenQuiver)
                EditTooltipByNum(2, (line) => line.Text = EditedTooltip("MoltenQuiver"));

            // Eye of the Golem gains a new effect, also applies to specifically Sniper Scope.
            if (item.type == ItemID.EyeoftheGolem)
                EditTooltipByNum(0, (line) => line.Text += AddedTooltip("EyeoftheGolem"));

            // Scope effects can now be visibility toggled.
            if (item.type == ItemID.RifleScope)
            {
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("RifleScope1"));
                EditTooltipByNum(1, (line) => line.Text = EditedTooltip("RifleScope2"));
            }
            if (item.type == ItemID.ReconScope)
                EditTooltipByNum(0, (line) => line.Text += AddedTooltip("RifleScope"));
            if (item.type == ItemID.SniperScope)
            {
                EditTooltipByNum(1, (line) => line.Text = EditedTooltip("SniperScope"));
                EditTooltipByNum(0, (line) => line.Text += AddedTooltip("RifleScope"));
                EditTooltipByNum(1, (line) => line.Text += AddedTooltip("EyeoftheGolem"));
            }

            // Mana Flower tinker buffs.
            if (item.type == ItemID.MagnetFlower)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("8%", "10%"));
            if (item.type == ItemID.ArcaneFlower || item.type == ItemID.ManaCloak)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("8%", "12%"));
            if (item.type == ItemID.ArcaneFlower)
                EditTooltipByNum(2, (line) => line.Text += AddedTooltip("ArcaneFlower"));

            // Magiluminescence nerf and clarify the movement effects given.
            if (item.type == ItemID.Magiluminescence)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("Magiluminescence"));

            // Frog Leg and its upgrades clarify the amount of jump speed given.
            if (item.type == ItemID.FrogLeg)
                EditTooltipByNum(0, (line) => line.Text = GetEditedTooltip("FrogLeg").Format(BalancingConstants.VanillaFrogLegJumpSpeedBoost.ToJumpSpeedPercent()));
            if (item.type == ItemID.FrogFlipper || item.type == ItemID.FrogWebbing)
                EditTooltipByNum(1, (line) => line.Text = GetEditedTooltip("FrogLeg").Format(BalancingConstants.VanillaFrogLegJumpSpeedBoost.ToJumpSpeedPercent()));
            if (item.type == ItemID.FrogGear)
                EditTooltipByNum(2, (line) => line.Text = GetEditedTooltip("FrogLeg").Format(BalancingConstants.VanillaFrogLegJumpSpeedBoost.ToJumpSpeedPercent()));
            if (item.type == ItemID.AmphibianBoots)
                EditTooltipByNum(1, (line) => line.Text = GetEditedTooltip("FrogLeg").Format(BalancingConstants.AmphibianBootsJumpSpeedBoost.ToJumpSpeedPercent()));

            // Soaring Insignia nerf and clarify the movement effects given.
            if (item.type == ItemID.EmpressFlightBooster)
            {
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("EmpressFlightBooster1"));
                EditTooltipByNum(1, (line) => line.Text = EditedTooltip("EmpressFlightBooster2"));
            }

            // Reworked Gravity Globe
            if (item.type == ItemID.GravityGlobe)
                EditTooltipByNum(1, (line) => line.Text = EditedTooltip("GravityGlobe"));

            // Flame Waker Boots now has a functional effect which inherits to Hellfire Treads.
            if (item.type == ItemID.FlameWakerBoots)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("FlameWakerBoots"));
            if (item.type == ItemID.HellfireTreads)
                EditTooltipByNum(2, (line) => line.Text += AddedTooltip("HellfireTreads"));

            // Fairy Boots gains a new functional effect.
            if (item.type == ItemID.FairyBoots)
                EditTooltipByNum(2, (line) => line.Text += AddedTooltip("FairyBoots"));

            // Ankh Shield now provides sandstorm wind push immunity.
            if (item.type == ItemID.AnkhShield)
                EditTooltipByNum(1, (line) => line.Text = EditedTooltip("AnkhShield"));

            // Flesh Knuckles now gives increased max life.
            if (item.type == ItemID.FleshKnuckles || item.type == ItemID.HeroShield || item.type == ItemID.BerserkerGlove)
                EditTooltipByNum(0, (line) => line.Text += AddedTooltip("FleshKnucklesLine"));

            // Hand Warmer now has a side bonus with Snow armor.
            if (item.type == ItemID.HandWarmer)
                EditTooltipByNum(0, (line) => line.Text += AddedTooltip("HandWarmer"));
            #endregion

            #region Pre-Hardmode Armor
            // Gladiator
            if (item.type == ItemID.GladiatorHelmet)
                EditTooltipByName("Defense", (line) => line.Text += "\n" + CalamityUtils.GetText("Common.RogueDamage").Format(GladiatorArmorSetChange.HelmetRogueDamageBoostPercent));
            if (item.type == ItemID.GladiatorBreastplate)
                EditTooltipByName("Defense", (line) => line.Text += "\n" + CalamityUtils.GetText("Common.RogueCrit").Format(GladiatorArmorSetChange.ChestplateRogueCritBoostPercent));
            if (item.type == ItemID.GladiatorLeggings)
                EditTooltipByName("Defense", (line) => line.Text += "\n" + CalamityUtils.GetText("Common.RogueVelocity").Format(GladiatorArmorSetChange.LeggingRogueVelocityBoostPercent));

            // Jungle
            if (item.type == ItemID.JungleHat || item.type == ItemID.AncientCobaltHelmet)
            {
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("40", "20"));
                EditTooltipByNum(1, (line) => line.Text = line.Text.Replace("6%", "3%"));
            }
            if (item.type == ItemID.JunglePants || item.type == ItemID.AncientCobaltLeggings)
                EditTooltipByNum(1, (line) => line.Text = line.Text.Replace("6%", "3%"));

            // Crimson
            if (item.type == ItemID.CrimsonHelmet || item.type == ItemID.CrimsonScalemail || item.type == ItemID.CrimsonGreaves)
            {
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("3%", "6%"));
                EditTooltipByNum(0, (line) => line.Text += AddedTooltip("CrimsonArmorPieces"));
            }

            // Magic Hat nerf
            if (item.type == ItemID.MagicHat)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("MagicHat"));

            // Gem Robe nerfs
            if (item.type == ItemID.AmethystRobe)
                EditTooltipByNum(1, (line) => line.Text = line.Text.Replace("5%", "4%"));
            if (item.type == ItemID.TopazRobe)
            {
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("40", "20"));
                EditTooltipByNum(1, (line) => line.Text = line.Text.Replace("7%", "5%"));
            }
            if (item.type == ItemID.SapphireRobe)
                EditTooltipByNum(1, (line) => line.Text = line.Text.Replace("9%", "6%"));
            if (item.type == ItemID.EmeraldRobe)
            {
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("60", "40"));
                EditTooltipByNum(1, (line) => line.Text = line.Text.Replace("11%", "7%"));
            }
            if (item.type == ItemID.RubyRobe || item.type == ItemID.AmberRobe)
                EditTooltipByNum(1, (line) => line.Text = line.Text.Replace("13%", "8%"));
            if (item.type == ItemID.DiamondRobe)
            {
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("80", "60"));
                EditTooltipByNum(1, (line) => line.Text = line.Text.Replace("15%", "9%"));
            }

            //Gi 10% melee speed into 10% jump speed replacement
            if (item.type == ItemID.Gi)
                EditTooltipByNum(1, (line) => line.Text = EditedTooltip("Gi"));
            #endregion

            #region Hardmode Armor
            // Cobalt
            if (item.type == ItemID.CobaltHat)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("40", $"{CobaltArmorSetChange.MaxManaBoost + 40}"));

            // Palladium
            if (item.type == ItemID.PalladiumBreastplate)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("3%", $"{PalladiumArmorSetChange.ChestplateDamagePercentageBoost + 3}%"));
            if (item.type == ItemID.PalladiumLeggings)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("2%", $"{PalladiumArmorSetChange.LeggingsDamagePercentageBoost + 2}%"));

            // Mythril
            if (item.type == ItemID.MythrilHood)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("60", $"{MythrilArmorSetChange.MaxManaBoost + 60}"));

            // Orichalcum
            if (item.type == ItemID.OrichalcumBreastplate)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("6%", $"{OrichalcumArmorSetChange.ChestplateCritChanceBoost + 6}%"));

            // Adamantite
            if (item.type == ItemID.AdamantiteHeadgear)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("80", $"{AdamantiteArmorSetChange.MaxManaBoost + 80}"));

            // Titanium
            if (item.type == ItemID.TitaniumMask)
                EditTooltipByNum(1, (line) => line.Text = line.Text.Replace("9%", "14%"));

            // Shroomite
            if (item.type == ItemID.ShroomiteBreastplate)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("13%", "8%"));

            // Solar Flare
            if (item.type == ItemID.SolarFlareHelmet)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("26%", "20%"));

            // Vortex
            if (item.type == ItemID.VortexHelmet)
            {
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("16%", "10%"));
                EditTooltipByNum(1, (line) => line.Text = line.Text.Replace("7%", "5%"));
            }
            #endregion

            #region DD2 Armor
            // Nerf sets that are too strong

            if (item.type == ItemID.SquireAltHead)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("10%", "15%"));
            if (item.type == ItemID.SquireAltShirt)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("30%", "20%"));
            if (item.type == ItemID.SquireAltPants)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("20%", "15%"));

            // Tweaks into Rogue
            // Monk armor
            if (item.type == ItemID.MonkBrows)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("MonkBrows"));
            if (item.type == ItemID.MonkShirt)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("MonkShirt"));
            if (item.type == ItemID.MonkPants)
                EditTooltipByNum(1, (line) => line.Text = EditedTooltip("MonkPants"));

            // Shinobi Infiltrator armor
            if (item.type == ItemID.MonkAltHead)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("MonkAltHead"));
            if (item.type == ItemID.MonkAltShirt)
            {
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("MonkAltShirt0"));
                EditTooltipByNum(1, (line) => line.Text = EditedTooltip("MonkAltShirt1"));
            }
            if (item.type == ItemID.MonkAltPants)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("MonkAltPants"));
            #endregion

            #region Vanilla Set Bonus Tooltips
            EditTooltipByName("SetBonus", (line) => VanillaArmorChangeManager.ApplySetBonusTooltipChanges(item, ref line.Text));

            // Forbidden (UNLESS you are wearing the Circlet, which is Summon/Rogue and does not get this line)
            if ((item.type == ItemID.AncientBattleArmorHat || item.type == ItemID.AncientBattleArmorShirt || item.type == ItemID.AncientBattleArmorPants)
                && !Main.LocalPlayer.Calamity().forbiddenCirclet)
                EditTooltipByName("SetBonus", (line) => line.Text = CalamityUtils.GetText($"Vanilla.Armor.SetBonus.Forbidden").Format(CalamityUtils.GetArmorSetBonusKey()));

            // Vortex (hotkey spoof)
            if (item.type == ItemID.VortexHelmet || item.type == ItemID.VortexBreastplate || item.type == ItemID.VortexLeggings)
                EditTooltipByName("SetBonus", (line) => line.Text = CalamityUtils.GetText($"Vanilla.Armor.SetBonus.Vortex").Format(CalamityUtils.GetArmorSetBonusKey()));
            #endregion

            #region Potions
            // Nerfed Archery Potion
            if (item.type == ItemID.ArcheryPotion)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("ArcheryPotion"));

            // Nerfed Swiftness Potion
            if (item.type == ItemID.SwiftnessPotion)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("25%", "15%"));

            // Nerfed Magic Power Potion
            if (item.type == ItemID.MagicPowerPotion)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("20%", "10%"));

            // Nerfed Mining Potion
            if (item.type == ItemID.MiningPotion)
                EditTooltipByNum(0, (line) => line.Text = line.Text.Replace("25%", "15%"));

            // Ale and Sake rebalance and Alcohol Poisoning.
            if (item.type == ItemID.Ale || item.type == ItemID.Sake)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("AleSake"));

            // Featherfall Potion being stupid broken with Aero Stone
            if (item.type == ItemID.FeatherfallPotion)
                EditTooltipByNum(0, (line) => line.Text += AddedTooltip("FeatherfallPotion"));

            // Flasks apply to Rogue weapons (Party applies to all)
            if (item.type == ItemID.FlaskofCursedFlames)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("FlaskofCursedFlames"));
            if (item.type == ItemID.FlaskofFire)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("FlaskofFire"));
            if (item.type == ItemID.FlaskofGold)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("FlaskofGold"));
            if (item.type == ItemID.FlaskofIchor)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("FlaskofIchor"));
            if (item.type == ItemID.FlaskofNanites)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("FlaskofNanites"));
            if (item.type == ItemID.FlaskofParty)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("FlaskofParty"));
            if (item.type == ItemID.FlaskofPoison)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("FlaskofPoison"));
            if (item.type == ItemID.FlaskofVenom)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("FlaskofVenom"));
            #endregion

            #region Yoyo Stat Tooltips
            // This function is shorthand for appending a stat sheet to a yoyo.
            void AddYoyoStats(float d, float r, float s)
            {
                EditTooltipByName("Knockback", (line) => line.Text += "\n" + (d == -1f ? CalamityUtils.GetText("Common.YoyoStatsInfinite").Format(r.ToTiles(), s.ToString())
                : CalamityUtils.GetText("Common.YoyoStats").Format(r.ToTiles(), s.ToString(), d.ToString())));
            }

            if (item.type == ItemID.Amarok)
                AddYoyoStats(-1f, 432f, 28f);
            if (item.type == ItemID.Cascade)
                AddYoyoStats(30f, 384f, 28f);
            if (item.type == ItemID.Chik)
                AddYoyoStats(-1f, 400f, 32f);
            if (item.type == ItemID.Code1)
                AddYoyoStats(21f, 320f, 25f);
            if (item.type == ItemID.Code2)
                AddYoyoStats(-1f, 432f, 42f);
            if (item.type == ItemID.CorruptYoyo)
                AddYoyoStats(18f, 288f, 22f);
            if (item.type == ItemID.CrimsonYoyo)
                AddYoyoStats(18f, 288f, 22f);
            if (item.type == ItemID.FormatC)
                AddYoyoStats(-1f, 384f, 36f);
            if (item.type == ItemID.Gradient)
                AddYoyoStats(-1f, 384f, 36f);
            if (item.type == ItemID.HelFire)
                AddYoyoStats(-1f, 368f, 42f);
            if (item.type == ItemID.HiveFive)
                AddYoyoStats(24f, 320f, 20f);
            if (item.type == ItemID.JungleYoyo)
                AddYoyoStats(20f, 288f, 17f);
            if (item.type == ItemID.Kraken)
                AddYoyoStats(-1f, 480f, 54f);
            if (item.type == ItemID.Rally)
                AddYoyoStats(16f, 272f, 20f);
            if (item.type == ItemID.RedsYoyo)
                AddYoyoStats(-1f, 480f, 42f);
            if (item.type == ItemID.Terrarian)
                AddYoyoStats(-1f, 512f, 54f);
            if (item.type == ItemID.TheEyeOfCthulhu)
                AddYoyoStats(-1f, 480f, 36f);
            if (item.type == ItemID.ValkyrieYoyo)
                AddYoyoStats(-1f, 480f, 42f);
            if (item.type == ItemID.Valor)
                AddYoyoStats(30f, 400f, 36f);
            if (item.type == ItemID.WoodYoyo)
                AddYoyoStats(15f, 240f, 14f);
            if (item.type == ItemID.Yelets)
                AddYoyoStats(-1f, 400f, 36f);
            #endregion

            #region Wing Stat Tooltips

            // This function produces a "stat sheet" for a pair of wings from the raw stats.
            string WingStatsTooltip(WingStats stats, float fall, float rise, float rMax, float tMax, float asc, string extraKey = null)
            {
                int time = stats.FlyTime;
                float run = stats.AccRunSpeedOverride;
                float rAcc = stats.AccRunAccelerationMult * 0.08f;
                bool hover = stats.HasDownHoverStats;
                float hSpeed = stats.DownHoverSpeedOverride;
                float hAcc = stats.DownHoverAccelerationMult * 0.08f;
                float baseJumpSpeed = (CalamityServerConfig.Instance.FasterJumpSpeed ? BalancingConstants.ConfigBoostedBaseJumpSpeed : 5.01f) + 1f;
                StringBuilder sb = new StringBuilder(512);
                sb.Append('\n');
                sb.Append(CalamityUtils.GetText($"Common.WingStats").Format(time.FramesToSeconds(), run.ToMph(), (tMax * baseJumpSpeed).ToMph()));
                sb.Append('\n');
                if (Main.keyState.PressingShift())
                {
                    sb.Append(CalamityUtils.GetText($"Common.WingStatsAcceleration").Format(rAcc.ToMphps(), asc.ToMphps(), (asc + rise).ToMphps(), (rMax * baseJumpSpeed).ToMph(), (asc + fall).ToMphps()));
                    if (hover)
                    {
                        sb.Append('\n');
                        sb.Append(CalamityUtils.GetText($"Common.WingStatsHover").Format(hSpeed.ToMph(), hAcc.ToMphps()));
                    }
                }
                else
                    sb.Append($"[c/B8B8B8:{CalamityUtils.GetTextValue("UI.HoldShiftTooltipExtensionIndicator")}]");

                if (extraKey != null)
                {
                    sb.Append('\n');
                    sb.Append(CalamityUtils.GetTextValue($"Vanilla.Wings.{extraKey}"));
                }
                return sb.ToString();
            }

            // This function is shorthand for appending a stat sheet to a pair of wings.
            void AddWingStats(int slot, float fall, float rise, float rMax, float tMax, float asc, string extraKey = null) => EditTooltipByNum(0, (line) => line.Text += WingStatsTooltip(ArmorIDs.Wing.Sets.Stats[slot], fall, rise, rMax, tMax, asc, extraKey));

            if (item.type == ItemID.CreativeWings)
                AddWingStats(item.wingSlot, 0.5f, 0.1f, 0.5f, 1.5f, 0.1f);

            if (item.type == ItemID.AngelWings)
                AddWingStats(item.wingSlot, 0.5f, 0.1f, 0.5f, 1.95f, 0.15f);

            if (item.type == ItemID.DemonWings)
                AddWingStats(item.wingSlot, 1f, 0.2f, 1f, 1.5f, 0.1f, "DemonWings");

            if (item.type == ItemID.Jetpack)
                AddWingStats(item.wingSlot, 0.5f, 0.1f, 0.5f, 1.5f, 0.1f);

            if (item.type == ItemID.ButterflyWings)
                AddWingStats(item.wingSlot, 0.5f, 0.1f, 0.5f, 1.35f, 0.5f);

            if (item.type == ItemID.FairyWings)
                AddWingStats(item.wingSlot, 0.5f, 0.1f, 0.5f, 1.5f, 0.1f);

            if (item.type == ItemID.BeeWings)
                AddWingStats(item.wingSlot, 0.5f, 0.1f, 0.5f, 1.5f, 0.1f, "BeeWings");

            if (item.type == ItemID.HarpyWings)
                AddWingStats(item.wingSlot, 0.5f, 0.1f, 0.5f, 1.5f, 0.1f);

            if (item.type == ItemID.BoneWings)
                AddWingStats(item.wingSlot, 0.5f, 0.1f, 0.5f, 1.66f, 0.1f, "BoneWings");

            if (item.type == ItemID.FlameWings)
                AddWingStats(item.wingSlot, 0.5f, 0.1f, 0.5f, 1.8f, 0.135f);

            if (item.type == ItemID.FrozenWings)
                AddWingStats(item.wingSlot, 0.5f, 0.1f, 0.5f, 1.5f, 0.1f);

            if (item.type == ItemID.GhostWings)
                AddWingStats(item.wingSlot, 0.5f, 0.1f, 0.5f, 1.5f, 0.5f);

            if (item.type == ItemID.BeetleWings)
                AddWingStats(item.wingSlot, 0.5f, 0.1f, 0.5f, 1.66f, 0.1f);

            if (item.type == ItemID.FinWings)
                AddWingStats(item.wingSlot, 0.5f, 0.1f, 0.5f, 1.5f, 0.1f, "FinWings");

            if (item.type == ItemID.FishronWings)
                AddWingStats(item.wingSlot, 0.75f, 0.15f, 1f, 2.5f, 0.125f);

            if (item.type == ItemID.SteampunkWings)
                AddWingStats(item.wingSlot, 0.5f, 0.1f, 0.5f, 1.805f, 0.1f);

            if (item.type == ItemID.LeafWings)
                AddWingStats(item.wingSlot, 0.5f, 0.1f, 0.5f, 1.5f, 0.1f);

            if (item.type == ItemID.BatWings)
                AddWingStats(item.wingSlot, 0.5f, 0.1f, 0.5f, 1.5f, 0.1f);

            // All developer wings have identical stats and no special effects
            if (item.type == ItemID.Yoraiz0rWings || item.type == ItemID.JimsWings || item.type == ItemID.LokisWings || 
                item.type == ItemID.ArkhalisWings || item.type == ItemID.LeinforsWings || item.type == ItemID.RedsWings || 
                item.type == ItemID.DTownsWings || item.type == ItemID.WillsWings || item.type == ItemID.CrownosWings || 
                item.type == ItemID.CenxsWings || item.type == ItemID.FoodBarbarianWings || item.type == ItemID.GroxTheGreatWings || 
                item.type == ItemID.GhostarsWings || item.type == ItemID.SafemanWings)
            {
                AddWingStats(item.wingSlot, 0.5f, 0.1f, 0.5f, 1.5f, 0.1f);
            }

            // Except these ones that hover
            if (item.type == ItemID.SkiphsWings || item.type == ItemID.BejeweledValkyrieWing)
                AddWingStats(item.wingSlot, 0.5f, 0.1f, 0.5f, 1.5f, 0.1f);

            if (item.type == ItemID.TatteredFairyWings)
                AddWingStats(item.wingSlot, 0.5f, 0.1f, 0.5f, 1.805f, 0.1f, "TatteredFairyWings");

            if (item.type == ItemID.SpookyWings)
                AddWingStats(item.wingSlot, 0.5f, 0.1f, 0.5f, 1.805f, 0.1f);

            if (item.type == ItemID.Hoverboard)
                AddWingStats(item.wingSlot, 0.5f, 0.1f, 0.5f, 1.66f, 0.1f);

            if (item.type == ItemID.FestiveWings)
                AddWingStats(item.wingSlot, 0.5f, 0.1f, 0.5f, 1.805f, 0.1f, "FestiveWings");

            if (item.type == ItemID.MothronWings)
                AddWingStats(item.wingSlot, 0.5f, 0.1f, 0.5f, 1.66f, 0.1f);

            if (item.type == ItemID.WingsSolar)
                AddWingStats(item.wingSlot, 0.85f, 0.15f, 1f, 3f, 0.135f, "WingsSolar");

            if (item.type == ItemID.WingsStardust)
                AddWingStats(item.wingSlot, 0.85f, 0.15f, 1f, 3f, 0.135f, "WingsStardust");

            if (item.type == ItemID.WingsVortex)
                AddWingStats(item.wingSlot, 0.5f, 0.1f, 1f, 2.45f, 0.15f, "WingsVortex");

            if (item.type == ItemID.WingsNebula)
                AddWingStats(item.wingSlot, 0.5f, 0.1f, 1f, 2.45f, 0.15f, "WingsNebula");

            if (item.type == ItemID.BetsyWings)
                AddWingStats(item.wingSlot, 0.75f, 0.15f, 1f, 2.5f, 0.125f);

            if (item.type == ItemID.RainbowWings)
                AddWingStats(item.wingSlot, 0.85f, 0.15f, 1f, 2.5f, 0.125f);

            if (item.type == ItemID.LongRainbowTrailWings)
                AddWingStats(item.wingSlot, 0.95f, 0.15f, 1f, 4.5f, 0.1f);
            #endregion

            #region Grappling Hook Stat Tooltips

            // This function is shorthand for appending a stat sheet to a grappling hook.
            void AddGrappleStats(float r, float l, float e, float p) => EditTooltipByName("Equipable", (line) => line.Text += "\n" + CalamityUtils.GetText("Common.GrappleStats").Format(r.ToString(), l.ToString(), e.ToString(), p.ToString()));

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

            #region Herbs and Seeds Tooltips

            void AddHerbTooltips(string key)
            {
                int materialIndex = 0;
                for (int i = 0; i < tooltips.Count; ++i)
                    if (tooltips[i].Name == "Material")
                    {
                        materialIndex = i;
                        break;
                    }
                tooltips.Insert(materialIndex + 1, new TooltipLine(CalamityMod.Instance, "Tooltip0", CalamityUtils.GetTextValue($"Vanilla.HerbTooltips.{key}")));
            }

            if (item.type == ItemID.Daybloom)
                AddHerbTooltips("Daybloom");
            if (item.type == ItemID.Moonglow)
                AddHerbTooltips("Moonglow");
            if (item.type == ItemID.Waterleaf)
                AddHerbTooltips("Waterleaf");
            if (item.type == ItemID.Blinkroot)
                AddHerbTooltips("Blinkroot");
            if (item.type == ItemID.Shiverthorn)
                AddHerbTooltips("Shiverthorn");
            if (item.type == ItemID.Deathweed)
                AddHerbTooltips("Deathweed");
            if (item.type == ItemID.Fireblossom)
                AddHerbTooltips("Fireblossom");

            void AddSeedTooltips(string key)
            {
                int materialIndex = 0;
                for (int i = 0; i < tooltips.Count; ++i)
                    if (tooltips[i].Name == "Placeable")
                    {
                        materialIndex = i;
                        break;
                    }
                tooltips.Insert(materialIndex + 1, new TooltipLine(CalamityMod.Instance, "Tooltip0", CalamityUtils.GetTextValue($"Vanilla.SeedTooltips.{key}")));
            }

            if (item.type == ItemID.DaybloomSeeds)
                AddSeedTooltips("Daybloom");
            if (item.type == ItemID.MoonglowSeeds)
                AddSeedTooltips("Moonglow");
            if (item.type == ItemID.WaterleafSeeds)
                AddSeedTooltips("Waterleaf");
            if (item.type == ItemID.BlinkrootSeeds)
                AddSeedTooltips("Blinkroot");
            if (item.type == ItemID.ShiverthornSeeds)
                AddSeedTooltips("Shiverthorn");
            if (item.type == ItemID.DeathweedSeeds)
                AddSeedTooltips("Deathweed");
            if (item.type == ItemID.FireblossomSeeds)
                AddSeedTooltips("Fireblossom");

            #endregion

            // Add mentions of what Calamity ores vanilla pickaxes can mine
            #region Pickaxe New Ore Tooltips
            if (item.type == ItemID.GoldPickaxe || item.type == ItemID.PlatinumPickaxe)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("GoldPickaxe"));

            if (item.type == ItemID.Picksaw)
                EditTooltipByNum(0, (line) => line.Text += AddedTooltip("Picksaw"));

            if (item.type == ItemID.SolarFlarePickaxe || item.type == ItemID.VortexPickaxe || item.type == ItemID.NebulaPickaxe || item.type == ItemID.StardustPickaxe)
                EditTooltipByName("Material", (line) => line.Text += "\n" + CalamityUtils.GetTextValue("Common.CanMineUelibloom"));

            if (item.type == ItemID.SolarFlareDrill || item.type == ItemID.VortexDrill || item.type == ItemID.NebulaDrill || item.type == ItemID.StardustDrill)
                EditTooltipByName("TileBoost", (line) => line.Text += "\n" + CalamityUtils.GetTextValue("Common.CanMineUelibloom"));
            #endregion

            // Numerous random tooltip edits which don't fit into another category
            #region Miscellaneous Tooltip Edits
            // Apparently 612 is a homestuck reference
            if (item.type == ModContent.ItemType<Respiteblock>())
                EditTooltipByName("AxePower", (line) => line.Text = line.Text.Replace("610%", "612%"));

            // Master Mode items also drop in Revengeance
            // Only affects vanilla and Calamity items
            if (item.master && (item.type < ItemID.Count || item.ModItem?.Mod is CalamityMod))
                EditTooltipByName("Master", (line) => line.Text = EditedTooltip("MasterExclusive"));

            // Add a tooltip about Slimed's effects
            if (item.type == ItemID.SlimeGun)
                EditTooltipByNum(0, (line) => line.Text += AddedTooltip("SlimeGun"));
            // Replace the meme tooltip with a useful one.
            if (item.type == ItemID.GelBalloon)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("GelBalloon"));

            // Aerial Bane is no longer the real bane of aerial enemies (50% dmg bonus removed)
            if (item.type == ItemID.DD2BetsyBow)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("DD2BetsyBow"));

            // Rod of Discord cannot be used multiple times to hurt yourself
            if (item.type == ItemID.RodofDiscord)
                EditTooltipByNum(1, (line) => line.Text += AddedTooltip("RodofDiscord"));

            // If Early Hardmode Rework is enabled: Remind users that ores will NOT spawn when an altar is smashed.
            if (CalamityServerConfig.Instance.EarlyHardmodeProgressionRework && (item.type == ItemID.Pwnhammer || item.type == ItemID.Hammush))
                EditTooltipByNum(0, (line) => line.Text += AddedTooltip("Pwnhammer"));

            // Golden Fishing Rod inherently contains High Test Fishing Line
            if (item.type == ItemID.GoldenFishingRod)
                EditTooltipByName("NeedsBait", (line) => line.Text += AddedTooltip("GoldenFishingRod"));

            // Information about graveyards
            // There are no item sets for tombstones wtf
            if (item.type == ItemID.Tombstone || item.type == ItemID.GraveMarker || item.type == ItemID.CrossGraveMarker || item.type == ItemID.Headstone || item.type == ItemID.Gravestone || item.type == ItemID.Obelisk
                || item.type == ItemID.RichGravestone1 || item.type == ItemID.RichGravestone2 || item.type == ItemID.RichGravestone3 || item.type == ItemID.RichGravestone4 || item.type == ItemID.RichGravestone5)
                EditTooltipByName("Material", (line) => line.Text += AddedTooltip("Tombstones"));

            // Modify item speed tooltips to use a new scale designed to more accurately reflect practical distributions of item speeds.
            // Due to the higher complexity of the action, the actual logic is delegated to its own method.
            // I think this fits the miscellaneous category? Not seeing anything like this elsewhere. - Tomat
            EditTooltipByName("Speed", (line) => RedistributeSpeedTooltips(item, line));

            if (item.healLife > 0 && Main.LocalPlayer.Calamity().healingPotionMultiplier != 1f)
                EditTooltipByName("HealLife", (line) => line.Text = Language.GetOrRegister("CommonItemTooltip.RestoresLife").Format((int)(item.healLife * Main.LocalPlayer.Calamity().healingPotionMultiplier)));

            // Ancient Manipulator also crafts stuff with Astral Bars
            if (item.type == ItemID.LunarCraftingStation)
                EditTooltipByNum(0, (line) => line.Text = EditedTooltip("LunarCraftingStation"));

            // Replace the double tap line if double tap dash is overridden
            if ((item.type == ItemID.EoCShield || item.type == ItemID.Tabi) && CalamityKeybinds.DashHotkey.GetAssignedKeysOrEmpty().Count != 0)
                EditTooltipByNum(1, (line) => line.Text = CalamityUtils.GetText("Vanilla.DashKey").Format(CalamityKeybinds.DashHotkey.TooltipHotkeyString()));
            #endregion
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
            if (line.Name == "ItemName" && line.Mod == "Terraria" && item.type == ModContent.ItemType<XyksBlessingBlue>() && CalamityClientConfig.Instance.TextEffects)
            {
                Color rarityColor = Color.White;
                Vector2 basePosition = new Vector2(line.X, line.Y);

                float rate = Main.GlobalTimeWrappedHourly * 6;
                List<Color> eColors = new List<Color>()
                {
                    Color.DodgerBlue,
                    Color.Cyan,
                    Color.RoyalBlue
                };
                int colorIndex = (int)(rate / 2 % eColors.Count);
                Color currentColor = eColors[colorIndex];
                Color nextColor = eColors[(colorIndex + 1) % eColors.Count];
                Color usedColor = Color.Lerp(currentColor, nextColor, rate % 2f > 1f ? 1f : rate % 1f);

                Vector2 backScale = line.BaseScale;
                Color backColor = usedColor;

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.UIScaleMatrix);

                int draws = 20;
                for (int i = 0; i < draws; i++)
                {
                    Vector2 backPosition = basePosition + (MathHelper.TwoPi * i / 20f).ToRotationVector2() * (3.5f);
                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, backPosition, backColor, line.Rotation, line.Origin, backScale, line.MaxWidth, line.Spread);
                }
                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
                Texture2D square = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomRing").Value;

                Vector2 drawPosition = basePosition;
                Color drawColor = backColor;
                Vector2 rotationPoint = texture.Size() * 0.5f;
                int length = line.Text.Length;
                for (int i = 0; i < 8; i++)
                {
                    Main.EntitySpriteDraw(texture, basePosition + Vector2.UnitX * length * 4 + Vector2.UnitY * 23 + Vector2.UnitX * (i % 2 == 0 ? -10 * i : 10 * i), null, drawColor, (MathHelper.PiOver2), rotationPoint, new Vector2(0.3f - 0.02f * i, 1 + 0.35f * i) * 0.3f * Main.rand.NextFloat(0.9f, 1f), SpriteEffects.None);
                }
                for (int i = 0; i < 10; i++)
                {
                    float sine = (float)Math.Sin(Main.GlobalTimeWrappedHourly * (0 + i * 0.8f) / MathHelper.Pi);
                    float sine2 = (float)Math.Sin(Main.GlobalTimeWrappedHourly * (5) / MathHelper.Pi);
                    float squareSine = (float)Math.Sin(i * 2.5f / MathHelper.Pi);
                    float clockSine = (float)Math.Sin((float)Math.Pow(Utils.GetLerpValue(0, 110, ((int)(Main.GlobalTimeWrappedHourly * (60 + i * 2)) % 120)), 5));
                    Vector2 weirdPos = Vector2.UnitX * 53 * i * (Utils.GetLerpValue(20, 0, i, true)) + Vector2.UnitY * -23 * squareSine * sine2 + new Vector2(5, 10);
                    for (int t = 0; t < 3; t++)
                        Main.EntitySpriteDraw(square, basePosition + weirdPos, null, drawColor * (1 - 0.03f * i), 0, square.Size() * 0.5f, (1.2f - 0.07f * t) * new Vector2(1, 1) * (0.25f * ((float)Math.Pow(Utils.GetLerpValue(11, 0, i, true), 3) + 0.2f)), SpriteEffects.None);
                    for (int t = 0; t < 3; t++)
                        Main.EntitySpriteDraw(texture, basePosition + weirdPos, null, drawColor * (1 - 0.03f * i), 0, texture.Size() * 0.5f, (0.65f - 0.07f * t) * new Vector2(1, 1) * (0.25f * ((float)Math.Pow(Utils.GetLerpValue(11, 0, i, true), 3) + 0.2f)), SpriteEffects.None);
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);

                // Draw the front text as usual.
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, basePosition, rarityColor, line.Rotation, line.Origin, line.BaseScale, line.MaxWidth, line.Spread);

                return false;
            }
            if (line.Name == "ItemName" && line.Mod == "Terraria" && item.type == ModContent.ItemType<XyksBlessingOrange>() && CalamityClientConfig.Instance.TextEffects)
            {
                Color rarityColor = Color.White;
                Vector2 basePosition = new Vector2(line.X, line.Y);

                float rate = Main.GlobalTimeWrappedHourly * 6;
                List<Color> eColors = new List<Color>()
                {
                    new Color(248, 117, 52),
                    Color.Gold,
                    Color.Orange
                };
                int colorIndex = (int)(rate / 2 % eColors.Count);
                Color currentColor = eColors[colorIndex];
                Color nextColor = eColors[(colorIndex + 1) % eColors.Count];
                Color usedColor = Color.Lerp(currentColor, nextColor, rate % 2f > 1f ? 1f : rate % 1f);

                Vector2 backScale = line.BaseScale;
                Color backColor = usedColor;

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.UIScaleMatrix);

                int draws = 20;
                for (int i = 0; i < draws; i++)
                {
                    Vector2 backPosition = basePosition + (MathHelper.TwoPi * i / 20f).ToRotationVector2() * (3.5f);
                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, backPosition, backColor, line.Rotation, line.Origin, backScale, line.MaxWidth, line.Spread);
                }
                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
                Texture2D texture2 = ModContent.Request<Texture2D>("CalamityMod/Particles/SquareRotated").Value;
                Texture2D square = ModContent.Request<Texture2D>("CalamityMod/Particles/GlowSquareParticleThick").Value;

                Vector2 drawPosition = basePosition;
                Color drawColor = backColor;
                Vector2 rotationPoint = texture.Size() * 0.5f;
                int length = line.Text.Length;
                for (int i = 0; i < 8; i++)
                {
                    Main.EntitySpriteDraw(texture, basePosition + Vector2.UnitX * length * 4 + Vector2.UnitY * 23 + Vector2.UnitX * (i % 2 == 0 ? -10 * i : 10 * i), null, drawColor, (MathHelper.PiOver2), rotationPoint, new Vector2(0.3f - 0.02f * i, 1 + 0.35f * i) * 0.3f * Main.rand.NextFloat(0.9f, 1f), SpriteEffects.None);
                }
                for (int i = 0; i < 10; i++)
                {
                    float sine = (float)Math.Sin(Main.GlobalTimeWrappedHourly * (0 + i * 0.8f) / MathHelper.Pi);
                    float squareSine = (float)Math.Sin(i * 2.5f / MathHelper.Pi);
                    float clockSine = (float)Math.Sin((float)Math.Pow(Utils.GetLerpValue(0, 110, ((int)(Main.GlobalTimeWrappedHourly * (60 + i * 2)) % 120)), 5));
                    Vector2 weirdPos = Vector2.UnitX * 53 * i * (Utils.GetLerpValue(20, 0, i, true)) + Vector2.UnitY * 23 * squareSine + Vector2.UnitY * 5.5f * sine + new Vector2(5, 10);
                    for (int t = 0; t < 3; t++)
                        Main.EntitySpriteDraw(square, basePosition + weirdPos, null, drawColor * (1 - 0.03f * i), clockSine * MathHelper.PiOver2 + MathHelper.PiOver4, square.Size() * 0.5f, (1 - 0.07f * t) * new Vector2(1, 1) * (0.25f * ((float)Math.Pow(Utils.GetLerpValue(11, 0, i, true), 3) + 0.2f)), SpriteEffects.None);
                    for (int t = 0; t < 3; t++)
                        Main.EntitySpriteDraw(texture2, basePosition + weirdPos, null, drawColor * (1 - 0.03f * i), clockSine * MathHelper.PiOver2, texture2.Size() * 0.5f, (0.6f - 0.07f * t) * new Vector2(1, 1) * (0.25f * ((float)Math.Pow(Utils.GetLerpValue(11, 0, i, true), 3) + 0.2f)), SpriteEffects.None);
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);

                // Draw the front text as usual.
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, basePosition, rarityColor, line.Rotation, line.Origin, line.BaseScale, line.MaxWidth, line.Spread);

                return false;
            }
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
            // IV Drip tooltip FX
            if (line.Mod == "Terraria" && item.type == ModContent.ItemType<IVDripOnTheRocks>() && line.Name == "Tooltip4")
            {
                Vector2 basePosition = new Vector2(line.X, line.Y);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.UIScaleMatrix);

                for (int i = 0; i < 3; i++) // Draw 3 lines of differing opacity, color, and displacement.
                {
                    float timer = (float)Math.Sin(Main.GlobalTimeWrappedHourly + 1f) / 2f;

                    float angle = (Main.GlobalTimeWrappedHourly * 2f) + (i * MathHelper.TwoPi / 3f);
                    float radius = timer * (3f + i * 6f);
                    Vector2 offset = new Vector2((float)Math.Cos(angle) * 1.7f, (float)Math.Sin(angle) * 1f) * radius; // Offset in an elliptical pattern
                    Vector2 pos = basePosition + offset;

                    Color color = MulticolorLerp(Math.Abs(timer + (i * 0.2f)) % 1f, Color.LightBlue, Color.LightGreen, (i == 3) ? Color.Blue : (i == 2) ? Color.Red : Color.White);
                    float opacity = 1f - (i * 0.2f);
                    float rotation = line.Rotation + (timer * (Main.GlobalTimeWrappedHourly * 0.00001f)); // This precision is intended

                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, pos, color * opacity, rotation, line.Origin, line.BaseScale, line.MaxWidth, line.Spread);
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);

                return false;
            }
            if (line.Mod == "Terraria" && item.type == ModContent.ItemType<OntologicalDespoiler>() && (line.Name == "Tooltip1" || line.Name == "Tooltip2" || line.Name == "Tooltip4" || line.Name == "Tooltip5" || line.Name == "Tooltip7"))
            {
                Color rarityColor = Color.Black;
                Vector2 basePosition = new Vector2(line.X, line.Y);
                Vector2 backScale = line.BaseScale;
                Player Owner = Main.LocalPlayer;
                if (Owner is null)
                    return false;

                float sine = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 5 / MathHelper.Pi);
                int draws = 20;
                Color usedColor = Color.White;
                if (line.Name == "Tooltip1" || line.Name == "Tooltip4" || line.Name == "Tooltip7") // Give the shifting color to the lines that need it
                {
                    float rate = (Main.GlobalTimeWrappedHourly * 3);
                    List<Color> eColors = new List<Color>()
                    {
                        Owner.shirtColor,
                        Color.Lerp(Owner.shirtColor, Color.Black, 0.15f),
                        Color.Lerp(Owner.shirtColor, Color.White, 0.05f),
                        Color.Lerp(Owner.shirtColor, Color.White, 0.25f)
                    };
                    int colorIndex = (int)(rate / 2 % eColors.Count);
                    Color currentColor = eColors[colorIndex];
                    Color nextColor = eColors[(colorIndex + 1) % eColors.Count];
                    usedColor = Color.Lerp(currentColor, nextColor, rate % 2f >= 1f ? 1f : rate % 1f);
                    if (Owner.shirtColor == Color.White)
                        usedColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
                }

                if (line.Name == "Tooltip5") // Shaky black outline
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 shake = Main.rand.NextVector2Circular(5, 5);
                        Vector2 backPosition = basePosition + shake;
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, backPosition, rarityColor, line.Rotation, line.Origin, backScale, line.MaxWidth, line.Spread);
                    }
                }
                if (line.Name == "Tooltip2" || line.Name == "Tooltip5") // Negative lines (inverted colors)
                {
                    for (int i = 0; i < draws; i++)
                    {
                        Color clr = (line.Name == "Tooltip5") ? Color.Lerp(Color.White, Color.Black, sine) : Color.White;
                        Vector2 backPosition = basePosition + (MathHelper.TwoPi * i / draws).ToRotationVector2() * (1.5f + 0.2f * sine);
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, backPosition, clr with { A = 0 }, line.Rotation, line.Origin, backScale, line.MaxWidth, line.Spread);
                    }
                    Color clr2 = (line.Name == "Tooltip5") ? Color.Lerp(Color.Black, Color.White, sine) : Color.Black;
                    ChatManager.DrawColorCodedString(Main.spriteBatch, line.Font, line.Text, basePosition, clr2, line.Rotation, line.Origin, backScale);
                    return false;

                }
                else if (line.Name == "Tooltip4") // Double outline
                {
                    for (int i = 0; i < draws; i++)
                    {
                        Vector2 backPosition = basePosition + (MathHelper.TwoPi * i / draws).ToRotationVector2() * (4.5f + 0.2f * sine);
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, backPosition, usedColor with { A = 0 }, line.Rotation, line.Origin, backScale, line.MaxWidth, line.Spread);
                    }
                    for (int i = 0; i < draws; i++)
                    {
                        Vector2 backPosition = basePosition + (MathHelper.TwoPi * i / draws).ToRotationVector2() * (2.5f + 0.2f * sine);
                        ChatManager.DrawColorCodedString(Main.spriteBatch, line.Font, line.Text, backPosition, Color.Black, line.Rotation, line.Origin, backScale);
                    }

                    ChatManager.DrawColorCodedString(Main.spriteBatch, line.Font, line.Text, basePosition, Color.White, line.Rotation, line.Origin, backScale);
                    return false;
                }
                if (line.Name == "Tooltip7") // Dark horizon thing
                {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Particles/Light").Value;

                    Vector2 drawPosition = basePosition;
                    Color drawColor = Color.Black;
                    Vector2 rotationPoint = texture.Size() * 0.5f;
                    for (int i = 0; i < 6; i++)
                    {
                        int length = line.Text.Length;
                        Main.EntitySpriteDraw(texture, basePosition + Vector2.UnitX * length * 4 + Vector2.UnitY * 10 + Vector2.UnitX * (i % 2 == 0 ? -7 * i : 7 * i), null, usedColor with { A = 0 }, (MathHelper.PiOver2), rotationPoint, new Vector2(0.9f - 0.085f * i, 1 + 2.7f * i * 1f) * 0.7f * Main.rand.NextFloat(0.95f, 1f), SpriteEffects.None);
                        Main.EntitySpriteDraw(texture, basePosition + Vector2.UnitX * length * 4 + Vector2.UnitY * 10 + Vector2.UnitX * (i % 2 == 0 ? -7 * i : 7 * i), null, drawColor, (MathHelper.PiOver2), rotationPoint, new Vector2(0.9f - 0.05f * i, 1 + 4.5f * i * 1f) * 0.55f * Main.rand.NextFloat(0.95f, 1f), SpriteEffects.None);
                    }

                    for (int i = 0; i < draws; i++)
                    {
                        Vector2 backPosition = basePosition + (MathHelper.TwoPi * i / draws).ToRotationVector2() * (1.5f);
                        ChatManager.DrawColorCodedString(Main.spriteBatch, line.Font, line.Text, backPosition, usedColor with { A = 0 } * 0.6f, line.Rotation, line.Origin, backScale);
                    }
                    ChatManager.DrawColorCodedString(Main.spriteBatch, line.Font, line.Text, basePosition, Color.Black, line.Rotation, line.Origin, backScale);
                    return false;
                }
                if (line.Name == "Tooltip1")
                {
                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, basePosition, usedColor, line.Rotation, line.Origin, backScale, line.MaxWidth, line.Spread);
                    return false;
                }

                // Draw the front text as usual as a backup just in case.
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, basePosition, Color.White, line.Rotation, line.Origin, line.BaseScale, line.MaxWidth, line.Spread);

                return false;
            }

            // Rainbow effect originally made for Orderbringer because it used to have a special rarity
            // But why did it have one in the first place??
            // Might be used for Miracle stuff later or something idk
            /*if (line.Name == "ItemName" && line.Mod == "Terraria" && item.type == ModContent.ItemType<Orderbringer>())
            {
                Color rarityColor = Color.White;
                Vector2 basePosition = new Vector2(line.X, line.Y);

                float rate = Main.GlobalTimeWrappedHourly * 29;
                List<Color> eColors = new List<Color>()
                {
                Color.PaleVioletRed,
                Color.Coral,
                Color.Khaki,
                Color.PaleGreen,
                Color.Turquoise,
                Color.Violet
                };

                int colorIndex = (int)(rate / 2 % eColors.Count);
                Color currentColor = eColors[colorIndex];
                Color nextColor = eColors[(colorIndex + 1) % eColors.Count];
                Color usedColor = Color.Lerp(currentColor, nextColor, rate % 2f > 1f ? 1f : rate % 1f);
                

                float sine = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3 / MathHelper.Pi);
                sine = (float)Math.Pow(MathHelper.Lerp(sine, 0, 0.35f), 5);
                Vector2 backScale = line.BaseScale;
                Color backColor = usedColor;
                Vector2 shake = Main.rand.NextVector2Circular(15, 15) * sine;

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.UIScaleMatrix);

                int draws = 20;
                for (int i = 0; i < draws; i++)
                {
                    shake = Main.rand.NextVector2Circular(15, 15) * sine;
                    Vector2 backPosition = basePosition + (MathHelper.TwoPi * i / 20f).ToRotationVector2() * (5 + 0.5f * sine);
                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, shake * 4 + backPosition, backColor, line.Rotation, line.Origin, backScale, line.MaxWidth, line.Spread);
                }
                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;

                Vector2 drawPosition = basePosition;
                Color drawColor = backColor;
                Vector2 rotationPoint = texture.Size() * 0.5f;
                for (int i = 0; i < 6; i++)
                {
                    int length = line.Text.Length;
                    Main.EntitySpriteDraw(texture, basePosition + Vector2.UnitX * length * 4 + Vector2.UnitY * 10 + Vector2.UnitX * (i % 2 == 0 ? -7 * i : 7 * i), null, drawColor, (MathHelper.PiOver2), rotationPoint, new Vector2(1 - 0.05f * i * (1 + 0.2f * -sine), 1 + 0.75f * i * (1 + 0.2f * sine) * 1f) * 0.3f * Main.rand.NextFloat(0.9f, 1f), SpriteEffects.None);
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);

                // Draw the front text as usual.
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, shake + basePosition, rarityColor, line.Rotation, line.Origin, line.BaseScale, line.MaxWidth, line.Spread);

                return false;
            }*/
            return true;
        }
        #endregion

        #region Schematic Knowledge Tooltip Utility
        public static void InsertKnowledgeTooltip(List<TooltipLine> tooltips, int tier, bool allowOldWorlds = false)
        {
            TooltipLine line = new TooltipLine(CalamityMod.Instance, "SchematicKnowledge1", CalamityUtils.GetTextValue("Misc.SchematicKnowledgeTooltip"));
            TooltipLine line2 = new TooltipLine(CalamityMod.Instance, "SchematicKnowledge2", CalamityUtils.GetTextValue("Misc.SchematicKnowledgeTooltip2"));
            switch (tier)
            {
                case 1:
                    line2 = new TooltipLine(CalamityMod.Instance, "SchematicKnowledge2", CalamityUtils.GetTextValue("Misc.Tier1KnowledgeTooltip"));
                    break;
                case 2:
                    line2 = new TooltipLine(CalamityMod.Instance, "SchematicKnowledge2", CalamityUtils.GetTextValue("Misc.Tier2KnowledgeTooltip"));
                    break;
                case 3:
                    line2 = new TooltipLine(CalamityMod.Instance, "SchematicKnowledge2", CalamityUtils.GetTextValue("Misc.Tier3KnowledgeTooltip"));
                    break;
                case 4:
                    line2 = new TooltipLine(CalamityMod.Instance, "SchematicKnowledge2", CalamityUtils.GetTextValue("Misc.Tier4KnowledgeTooltip"));
                    break;
                case 5:
                    line2 = new TooltipLine(CalamityMod.Instance, "SchematicKnowledge2", CalamityUtils.GetTextValue("Misc.Tier5KnowledgeTooltip"));
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
