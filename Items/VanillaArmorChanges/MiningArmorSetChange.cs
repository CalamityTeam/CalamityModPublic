using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class MiningArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.MiningHelmet;
        public override int[] AlternativeHeadPieceIDs => new int[] { ItemID.UltrabrightHelmet };

        public override int? BodyPieceID => ItemID.MiningShirt;

        public override int? LegPieceID => ItemID.MiningPants;

        public override string ArmorSetName => "Mining";

        public const int BonusOreChance = 4;
        public const int CooldownMin = 180;
        public const int CooldownMax = 360;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText += $"\n{CalamityUtils.GetTextValue($"Vanilla.Armor.SetBonus.{ArmorSetName}")}";
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            player.Calamity().miningSet = true;
        }
    }
}
