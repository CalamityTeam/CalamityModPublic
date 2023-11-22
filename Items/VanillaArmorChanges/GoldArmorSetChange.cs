using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class GoldArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.GoldHelmet;

        public override int? BodyPieceID => ItemID.GoldChainmail;

        public override int? LegPieceID => ItemID.GoldGreaves;

        public override int[] AlternativeHeadPieceIDs => new int[] { ItemID.AncientGoldHelmet };

        public override string ArmorSetName => "Gold";

        public const float HeadDamage = 0.06f;
        public const float ChestDR = 0.05f;
        public const float LegsMoveSpeed = 0.1f;
        public const float GoldDropChanceFromEnemies = 0.04f;
        public const int GoldFromBosses = 3;
        public const float SetBonusCritPerGoldCoin = 0.2f; // 5 gold coins = +1% crit chance

        public override void ApplyHeadPieceEffect(Player player) => player.GetDamage<GenericDamageClass>() += HeadDamage;

        public override void ApplyBodyPieceEffect(Player player) => player.endurance += ChestDR;

        public override void ApplyLegPieceEffect(Player player) => player.moveSpeed += LegsMoveSpeed;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText += $"\n{CalamityUtils.GetTextValue($"Vanilla.Armor.SetBonus.{ArmorSetName}")}";
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            player.Calamity().goldArmorGoldDrops = true;
            float critFromGold;

            // Give the crit chance from gold in inventory.
            // If you have any platinum, this guarantees the max boost.
            if (player.InventoryHas(ItemID.PlatinumCoin))
                critFromGold = 10f;
            else
            {
                int goldCoins = player.CountItem(ItemID.GoldCoin, 50);
                critFromGold = goldCoins * SetBonusCritPerGoldCoin;
            }

            player.GetCritChance<GenericDamageClass>() += critFromGold;
        }
    }
}
