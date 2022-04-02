using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.VanillaArmorChanges
{
	public class ObsidianArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.ObsidianHelm;

        public override int? BodyPieceID => ItemID.ObsidianShirt;

        public override int? LegPieceID => ItemID.ObsidianPants;

        public override string ArmorSetName => "Obsidian";

        public override bool NeedsToCreateSetBonusTextManually => true;

        public const int HelmetRogueDamageBoostPercent = 3;
        public const int ChestplateRogueCritBoostPercent = 3;
        public const int LeggingRogueVelocityBoostPercent = 3;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText = "+2 defense\n" +
                        "5% increased rogue damage and critical strike chance\n" +
                        "Grants immunity to fire blocks and temporary immunity to lava\n" +
                        "Rogue stealth builds while not attacking and not moving, up to a max of 80\n" +
                        "Once you have built max stealth, you will be able to perform a Stealth Strike\n" +
                        "Rogue stealth only reduces when you attack, it does not reduce while moving\n" +
                        "The higher your rogue stealth the higher your rogue damage, crit, and movement speed";
        }

        public override void ApplyHeadPieceEffect(Player player)
        {
            player.Calamity().throwingDamage += HelmetRogueDamageBoostPercent * 0.01f;
        }

        public override void ApplyBodyPieceEffect(Player player)
        {
            player.Calamity().throwingCrit += ChestplateRogueCritBoostPercent;
        }

        public override void ApplyLegPieceEffect(Player player)
        {
            player.Calamity().throwingVelocity += LeggingRogueVelocityBoostPercent * 0.01f;
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            player.Calamity().rogueStealthMax += 0.8f;
            player.Calamity().wearingRogueArmor = true;
            player.Calamity().throwingDamage += 0.05f;
            player.Calamity().throwingCrit += 5;
            player.statDefense += 2;
            player.fireWalk = true;
            player.lavaMax += 180;
        }
    }
}
