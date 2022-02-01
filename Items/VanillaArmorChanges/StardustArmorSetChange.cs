using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class StardustArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.StardustHelmet;

        public override int? BodyPieceID => ItemID.StardustBreastplate;

        public override int? LegPieceID => ItemID.StardustLeggings;

        public override string ArmorSetName => "Stardust";

        public override void ApplyBodyPieceEffect(Player player)
        {
            player.maxMinions--;
        }

        public override void ApplyLegPieceEffect(Player player)
        {
            player.maxMinions--;
        }
    }
}
