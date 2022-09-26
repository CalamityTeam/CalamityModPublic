using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class MeteorArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.MeteorHelmet;

        public override int? BodyPieceID => ItemID.MeteorSuit;

        public override int? LegPieceID => ItemID.MeteorLeggings;

        public override string ArmorSetName => "Meteor";

        // Meteor Armor only makes space gun cost 50% instead of zero mana.
        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText = setBonusText.Replace("0", "50%");
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            player.Calamity().meteorSet = true;
        }
    }
}
