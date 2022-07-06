using System;
using System.Collections.Generic;
using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using static Microsoft.Xna.Framework.Input.Keys;

namespace CalamityMod.Items.BaseItems
{
    public abstract class StealthGrantingArmorPiece : ModItem
    {
        /// <summary>
        /// Checks if the arlmor is part of a set.
        /// </summary>
        public abstract bool HasArmorSet(Player player);
        /// <summary>
        /// How much stealth is granted by this armor set
        /// </summary>
        public abstract float StealthBoost { get; }
        /// <summary>
        /// The text that appears to query about the extra stealth details. If shift is meant to show more than just stealth, it can be edited.
        /// </summary>
        public virtual string TooltipExpansionQueryText => "Hold SHIFT for an explanation of the stealth mechanics";
        public static string StealthSalesPitch = " Rogue stealth builds while not attacking and slower while moving\n" +
                " Your maximum stealth depends on the armor and accessories you are wearing\n" +
                " Once you have built max stealth, you will be able to perform a Stealth Strike\n" +
                " Rogue stealth only reduces when you attack, it does not reduce while moving\n" +
                " The higher your rogue stealth the higher your rogue damage, crit, and movement speed";
        public static string StealthColorHex => Color.Lerp(new Color(227, 180, 253), new Color(205, 236, 253), 0.5f + 0.5f * (float)Math.Sin(Main.GlobalTimeWrappedHourly)).ToHex();
        public static Color StealthSalesPitchColor => Color.Lerp(Color.Lerp(new Color(227, 180, 253), new Color(205, 236, 253), 0.5f + 0.5f * (float)Math.Sin(Main.GlobalTimeWrappedHourly)), new Color(230, 230, 230), 0.5f);


        /// <summary>
        /// Takes care of the required stealth set bonus effects.
        /// Call this after setting the players setBonus.
        /// </summary>
        /// <param name="player"></param>
        public void StealthSetBonus(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.rogueStealthMax += StealthBoost;
            modPlayer.wearingRogueArmor = true;
        }

        public void EditStealthDescription(Item item, List<TooltipLine> tooltips)
        {
            StealthSalesPitch = "  Rogue stealth builds while not attacking and slower while moving\n" +
                "  Your maximum stealth depends on the armor and accessories you are wearing\n" +
                "  Once you have built max stealth, you will be able to perform a Stealth Strike\n" +
                "  Rogue stealth only reduces when you attack, it does not reduce while moving\n" +
                "  The higher your rogue stealth the higher your rogue damage, crit, and movement speed";

            if (HasArmorSet(Main.LocalPlayer))
            {
                if (!Main.keyState.IsKeyDown(LeftShift))
                {
                    TooltipLine stealthInfoQuery = new TooltipLine(item.ModItem.Mod, "CalamityMod:ExpandedStealthDisplayQuery", TooltipExpansionQueryText);
                    stealthInfoQuery.OverrideColor = new Color(190, 190, 190);
                    tooltips.Add(stealthInfoQuery);
                }

                else
                {
                    TooltipLine stealthInfoTitle = new TooltipLine(item.ModItem.Mod, "CalamityMod:ExpandedStealthDisplayTitle", "[- Stealth -]");
                    stealthInfoTitle.OverrideColor = StealthSalesPitchColor;
                    tooltips.Add(stealthInfoTitle);

                    TooltipLine stealthInfo = new TooltipLine(item.ModItem.Mod, "CalamityMod:ExpandedStealthDisplay", StealthSalesPitch);
                    stealthInfo.OverrideColor = Color.White;
                    tooltips.Add(stealthInfo);
                }

            }
        }

        public override sealed void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (HasArmorSet(Main.LocalPlayer))
            {
                int setBonusIndex = tooltips.FindIndex(x => x.Name == "SetBonus" && x.Mod == "Terraria");

                if (setBonusIndex != -1)
                {
                    setBonusIndex++;
                    TooltipLine stealthCount = new TooltipLine(Item.ModItem.Mod, "CalamityMod:ArmorSetStealth", $"Grants {StealthBoost * 100} base [c/" + StealthColorHex + ":Stealth].");
                    tooltips.Insert(setBonusIndex, stealthCount);

                    EditStealthDescription(Item, tooltips);
                }
            }
        }
    }
}
