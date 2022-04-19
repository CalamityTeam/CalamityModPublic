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

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText = "Increases whip range by 50% and speed by 35%\n" +
                        "Increases minion damage by 15%\n" +
                        "Grants immunity to fire blocks and temporary immunity to lava";
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            player.fireWalk = true;
            player.lavaMax += 180;
        }
    }
}
