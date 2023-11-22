using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class MonkT2ArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.MonkBrows;

        public override int? BodyPieceID => ItemID.MonkShirt;

        public override int? LegPieceID => ItemID.MonkPants;

        public override string ArmorSetName => "MonkTier2";

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText += $"\n{CalamityUtils.GetTextValue($"Vanilla.Armor.SetBonus.{ArmorSetName}")}";
        }

        public override void ApplyHeadPieceEffect(Player player)
        {
            player.GetAttackSpeed<MeleeDamageClass>() -= 0.1f;
        }

        public override void ApplyBodyPieceEffect(Player player)
        {
            player.GetDamage<SummonDamageClass>() -= 0.1f;
            player.GetDamage<MeleeDamageClass>() -= 0.1f;
        }

        public override void ApplyLegPieceEffect(Player player)
        {
            player.GetDamage<SummonDamageClass>() -= 0.05f;
            player.GetCritChance<MeleeDamageClass>() -= 10;
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            player.GetDamage<SummonDamageClass>() += 0.15f;
            player.GetDamage<MeleeDamageClass>() += 0.1f;
            player.GetAttackSpeed<MeleeDamageClass>() += 0.1f;
            player.GetCritChance<MeleeDamageClass>() += 10;
        }
    }
}
