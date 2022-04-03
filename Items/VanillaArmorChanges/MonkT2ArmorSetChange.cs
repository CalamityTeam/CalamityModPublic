using Terraria;
using Terraria.ID;

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
            setBonusText += "\n10% increased melee speed, crit, and damage\n" +
                        "15% increased minion damage";
        }

        public override void ApplyHeadPieceEffect(Player player)
        {
            player.meleeSpeed -= 0.1f;
        }

        public override void ApplyBodyPieceEffect(Player player)
        {
            player.GetDamage(DamageClass.Summon) -= 0.1f;
            player.GetDamage(DamageClass.Melee) -= 0.1f;
        }

        public override void ApplyLegPieceEffect(Player player)
        {
            player.GetDamage(DamageClass.Summon) -= 0.05f;
            player.GetCritChance(DamageClass.Melee) -= 10;
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            player.GetDamage(DamageClass.Summon) += 0.15f;
            player.GetDamage(DamageClass.Melee) += 0.1f;
            player.meleeSpeed += 0.1f;
            player.GetCritChance(DamageClass.Melee) += 10;
        }
    }
}
