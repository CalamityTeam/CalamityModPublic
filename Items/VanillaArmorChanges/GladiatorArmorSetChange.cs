using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class GladiatorArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.GladiatorHelmet;

        public override int? BodyPieceID => ItemID.GladiatorBreastplate;

        public override int? LegPieceID => ItemID.GladiatorLeggings;

        public override string ArmorSetName => "Gladiator";

        public const int HelmetRogueDamageBoostPercent = 3;
        public const int ChestplateRogueCritBoostPercent = 3;
        public const int LeggingRogueVelocityBoostPercent = 3;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText += $"\n{CalamityUtils.GetTextValue($"Vanilla.Armor.SetBonus.{ArmorSetName}")}";
        }

        public override void ApplyHeadPieceEffect(Player player)
        {
            player.GetDamage<ThrowingDamageClass>() += HelmetRogueDamageBoostPercent * 0.01f;
        }

        public override void ApplyBodyPieceEffect(Player player)
        {
            player.GetCritChance<ThrowingDamageClass>() += ChestplateRogueCritBoostPercent;
        }

        public override void ApplyLegPieceEffect(Player player)
        {
            player.Calamity().rogueVelocity += LeggingRogueVelocityBoostPercent * 0.01f;
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            player.Calamity().rogueStealthMax += 0.6f;
            player.Calamity().wearingRogueArmor = true;
            player.GetDamage<ThrowingDamageClass>() += 0.05f;
            player.Calamity().rogueVelocity += 0.1f;
            player.statDefense += 3;
        }
    }
}
