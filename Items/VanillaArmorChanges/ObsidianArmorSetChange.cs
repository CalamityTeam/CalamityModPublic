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

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText = $"{CalamityUtils.GetTextValue($"Vanilla.Armor.SetBonus.{ArmorSetName}")}";
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            player.fireWalk = true;
            player.lavaMax += 180;
        }
    }
}
