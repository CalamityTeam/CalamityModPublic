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

        public override bool NeedsToCreateSetBonusTextManually => true;

        public const int HelmetRogueDamageBoostPercent = 3;
        public const int ChestplateRogueCritBoostPercent = 3;
        public const int LeggingRogueVelocityBoostPercent = 3;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText = "+3 defense\n" +
                        "5% increased rogue damage and 10% increased velocity\n" +
                        "Rogue stealth builds while not attacking and not moving, up to a max of 70\n" +
                        "Once you have built max stealth, you will be able to perform a Stealth Strike\n" +
                        "Rogue stealth only reduces when you attack, it does not reduce while moving\n" +
                        "The higher your rogue stealth the higher your rogue damage, crit, and movement speed";
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
            player.Calamity().rogueStealthMax += 0.7f;
            player.Calamity().wearingRogueArmor = true;
            player.GetDamage<ThrowingDamageClass>() += 0.05f;
            player.Calamity().rogueVelocity += 0.1f;
            player.statDefense += 3;
        }
    }
}
