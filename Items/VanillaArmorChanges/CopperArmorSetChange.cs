using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class CopperArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.CopperHelmet;

        public override int? BodyPieceID => ItemID.CopperChainmail;

        public override int? LegPieceID => ItemID.CopperGreaves;

        public override string ArmorSetName => "Copper";

        public const float HeadDamage = 0.05f;
        public const float ChestCrit = 3f;
        public const float LegsMoveSpeed = 0.05f;
        public const float SetBonusFlatDamage = 2.0f;
        public const float SetBonusMoveSpeed = 0.1f;
        public const int SetBonusMiningSpeedPercent = 25;

        public override void ApplyHeadPieceEffect(Player player) => player.GetDamage<GenericDamageClass>() += HeadDamage;

        public override void ApplyBodyPieceEffect(Player player) => player.GetCritChance<GenericDamageClass>() += ChestCrit;

        public override void ApplyLegPieceEffect(Player player) => player.moveSpeed += LegsMoveSpeed;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            StringBuilder sb = new StringBuilder(256);
            sb.Append("\n+2 flat damage to all attacks, +10% movement speed");
            sb.Append(CalamityGlobalItem.MiningSpeedString(SetBonusMiningSpeedPercent));
            setBonusText += sb.ToString();
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            player.GetDamage<GenericDamageClass>().Flat += SetBonusFlatDamage;
            player.moveSpeed += SetBonusMoveSpeed;
            player.pickSpeed -= SetBonusMiningSpeedPercent * 0.01f;
        }
    }
}
