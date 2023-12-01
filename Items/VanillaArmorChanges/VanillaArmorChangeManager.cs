using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.VanillaArmorChanges
{
    // TODO -- This can be made into a ModSystem with simple OnModLoad and Unload hooks.
    public static class VanillaArmorChangeManager
    {
        internal static List<VanillaArmorChange> ArmorChanges;

        internal static void Load()
        {
            ArmorChanges = new List<VanillaArmorChange>();
            foreach (Type type in CalamityMod.Instance.Code.GetTypes())
            {
                if (!type.IsSubclassOf(typeof(VanillaArmorChange)) || type.IsAbstract)
                    continue;

                ArmorChanges.Add((VanillaArmorChange)FormatterServices.GetUninitializedObject(type));
            }
        }

        internal static void Unload() => ArmorChanges = null;

        public static void ApplySetBonusTooltipChanges(Item checkItem, ref string setBonusText)
        {
            for (int i = 0; i < ArmorChanges.Count; i++)
            {
                bool isValidHeadPiece = (ArmorChanges[i].HeadPieceID ?? ItemID.None) == checkItem.type ||
                    ArmorChanges[i].AlternativeHeadPieceIDs.Contains(checkItem.type);
                bool isValidBodyPiece = (ArmorChanges[i].BodyPieceID ?? ItemID.None) == checkItem.type ||
                    ArmorChanges[i].AlternativeBodyPieceIDs.Contains(checkItem.type);
                bool isValidLegPiece = (ArmorChanges[i].LegPieceID ?? ItemID.None) == checkItem.type ||
                    ArmorChanges[i].AlternativeLegPieceIDs.Contains(checkItem.type);
                if ((isValidHeadPiece || isValidBodyPiece || isValidLegPiece) && !ArmorChanges[i].NeedsToCreateSetBonusTextManually)
                    ArmorChanges[i].UpdateSetBonusText(ref setBonusText);
            }
        }

        public static void CreateTooltipManuallyAsNecessary(Player player)
        {
            for (int i = 0; i < ArmorChanges.Count; i++)
            {
                if (ArmorChanges[i].IsWearingEntireSet(player) && ArmorChanges[i].NeedsToCreateSetBonusTextManually)
                {
                    ArmorChanges[i].UpdateSetBonusText(ref player.setBonus);
                    return;
                }
            }
        }

        public static string GetSetBonusName(Player player)
        {
            for (int i = 0; i < ArmorChanges.Count; i++)
            {
                if (ArmorChanges[i].IsWearingEntireSet(player))
                    return ArmorChanges[i].ArmorSetName;
            }
            return string.Empty;
        }

        public static void ApplyPotentialEffectsTo(Player player)
        {
            // Look through every armor change, apply individual set piece effects if pieces are being worn, and
            // if the entire set is worn, apply the set bonus.
            for (int i = 0; i < ArmorChanges.Count; i++)
            {
                ArmorChanges[i].ApplyIndividualPieceEffects(player);
                if (ArmorChanges[i].IsWearingEntireSet(player))
                    ArmorChanges[i].ApplyArmorSetBonus(player);
            }
        }
    }
}
