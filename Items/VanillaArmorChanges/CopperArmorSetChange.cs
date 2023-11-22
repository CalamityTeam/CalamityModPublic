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

        public override void ApplyHeadPieceEffect(Player player) => player.GetDamage<GenericDamageClass>() += HeadDamage;

        public override void ApplyBodyPieceEffect(Player player) => player.GetCritChance<GenericDamageClass>() += ChestCrit;

        public override void ApplyLegPieceEffect(Player player) => player.moveSpeed += LegsMoveSpeed;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText += $"\n{CalamityUtils.GetTextValue($"Vanilla.Armor.SetBonus.{ArmorSetName}")}";
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            player.GetDamage<GenericDamageClass>().Flat += SetBonusFlatDamage;
        }
    }
}
