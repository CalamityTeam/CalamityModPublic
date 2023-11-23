using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class TinArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.TinHelmet;

        public override int? BodyPieceID => ItemID.TinChainmail;

        public override int? LegPieceID => ItemID.TinGreaves;

        public override string ArmorSetName => "Tin";

        public const float HeadCrit = 4f;
        public const int ChestLifeRegen = 1;
        public const float LegsMoveSpeed = 0.05f;
        public const float SetBonusArmorPen = 5.0f;

        public override void ApplyHeadPieceEffect(Player player) => player.GetCritChance<GenericDamageClass>() += HeadCrit;

        public override void ApplyBodyPieceEffect(Player player) => player.lifeRegen += ChestLifeRegen;

        public override void ApplyLegPieceEffect(Player player) => player.moveSpeed += LegsMoveSpeed;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText += $"\n{CalamityUtils.GetTextValue($"Vanilla.Armor.SetBonus.{ArmorSetName}")}";
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            player.GetArmorPenetration<GenericDamageClass>() += SetBonusArmorPen;
        }
    }
}
