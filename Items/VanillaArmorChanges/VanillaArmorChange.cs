using System.Linq;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.VanillaArmorChanges
{
    // Certain things are virtual instead of abstract because not all pieces of armor are explicitly a part of any particular set.
    // The Wizard Hat is an example of this. The IDs are left abstract so that you don't mistakenly forget to override one of them.
    public abstract class VanillaArmorChange
    {
        public abstract int? HeadPieceID { get; }

        public abstract int? BodyPieceID { get; }

        public abstract int? LegPieceID { get; }

        public virtual int[] AlternativeHeadPieceIDs => new int[0];

        public virtual int[] AlternativeBodyPieceIDs => new int[0];

        public virtual int[] AlternativeLegPieceIDs => new int[0];

        public virtual string ArmorSetName => null;

        public virtual bool NeedsToCreateSetBonusTextManually => true;

        public virtual void UpdateSetBonusText(ref string setBonusText) { }

        public virtual void ApplyHeadPieceEffect(Player player) { }

        public virtual void ApplyBodyPieceEffect(Player player) { }

        public virtual void ApplyLegPieceEffect(Player player) { }

        public virtual void ApplyArmorSetBonus(Player player) { }

        public bool IsWearingEntireSet(Player player)
        {
            // Check each individual piece of armor on the player with the respective expected item ID.
            // If no ID is applied/it's null, that signals that it doesn't matter, and to just fall through anyway.
            if ((HeadPieceID ?? player.armor[0].type) != player.armor[0].type && !AlternativeHeadPieceIDs.Contains(player.armor[0].type))
                return false;
            if ((BodyPieceID ?? player.armor[1].type) != player.armor[1].type && !AlternativeBodyPieceIDs.Contains(player.armor[1].type))
                return false;
            if ((LegPieceID ?? player.armor[2].type) != player.armor[2].type && !AlternativeLegPieceIDs.Contains(player.armor[2].type))
                return false;

            return true;
        }

        public void ApplyIndividualPieceEffects(Player player)
        {
            if ((HeadPieceID ?? ItemID.None) == player.armor[0].type || AlternativeHeadPieceIDs.Contains(player.armor[0].type))
                ApplyHeadPieceEffect(player);
            if ((BodyPieceID ?? ItemID.None) == player.armor[1].type || AlternativeBodyPieceIDs.Contains(player.armor[1].type))
                ApplyBodyPieceEffect(player);
            if ((LegPieceID ?? ItemID.None) == player.armor[2].type || AlternativeLegPieceIDs.Contains(player.armor[2].type))
                ApplyLegPieceEffect(player);
        }
    }
}
