using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class CrimsonArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.CrimsonHelmet;

        public override int? BodyPieceID => ItemID.CrimsonScalemail;

        public override int? LegPieceID => ItemID.CrimsonGreaves;

        public override string ArmorSetName => "Crimson";

        public const int ArmorPieceLifeRegen = 1;

        // Set bonus clarification
        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText = $"{CalamityUtils.GetTextValue($"Vanilla.Armor.SetBonus.{ArmorSetName}")}";
        }

        private static void ApplyAnyPieceEffect(Player player)
        {
            // Give life regen
            player.lifeRegen += ArmorPieceLifeRegen;
        }

        public override void ApplyHeadPieceEffect(Player player) => ApplyAnyPieceEffect(player);

        public override void ApplyBodyPieceEffect(Player player) => ApplyAnyPieceEffect(player);

        public override void ApplyLegPieceEffect(Player player) => ApplyAnyPieceEffect(player);
    }
}
