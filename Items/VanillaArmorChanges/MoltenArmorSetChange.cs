using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class MoltenArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.MoltenHelmet;

        public override int? BodyPieceID => ItemID.MoltenBreastplate;

        public override int? LegPieceID => ItemID.MoltenGreaves;

        public override string ArmorSetName => "Molten";

        public const int MiningSpeedPercentSetBonus = 10;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText += $"\n{CalamityUtils.GetTextValue($"Vanilla.Armor.SetBonus.{ArmorSetName}")}";
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            player.fireWalk = true;
            player.lavaMax += 300;
            player.GetDamage<TrueMeleeDamageClass>() += 0.2f;
        }
    }
}
