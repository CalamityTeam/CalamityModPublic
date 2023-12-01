using System.Text;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class IronArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.IronHelmet;

        public override int? BodyPieceID => ItemID.IronChainmail;

        public override int? LegPieceID => ItemID.IronGreaves;

        public override int[] AlternativeHeadPieceIDs => new int[] { ItemID.AncientIronHelmet };

        public override string ArmorSetName => "Iron";

        public const float ArmorPieceDR = 0.03f;
        public const float SetBonusDR = 0.06f;
        public const int SetBonusLifeRegen = 2;

        public override void ApplyHeadPieceEffect(Player player) => player.endurance += ArmorPieceDR;

        public override void ApplyBodyPieceEffect(Player player) => player.endurance += ArmorPieceDR;

        public override void ApplyLegPieceEffect(Player player) => player.endurance += ArmorPieceDR;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText += $"\n{CalamityUtils.GetTextValue($"Vanilla.Armor.SetBonus.{ArmorSetName}")}";
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            player.endurance += SetBonusDR;
            player.lifeRegen += SetBonusLifeRegen;
        }
    }
}
