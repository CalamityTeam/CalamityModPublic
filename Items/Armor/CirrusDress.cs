using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class CirrusDress : ModItem
    {
        /* How to obtain
         * 1 - Have alcohol poisoning
         * 2 - Visit the Stylist while Cirrus is alive in the world
         * 3 - Open her shop to find the dress
         */

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cirrus' Dress");
            Tooltip.SetDefault("Here, this should help you drink as much alcohol as you want!\n" +
                "5% increased magic damage and critical strike chance\n" +
                "You feel thick...");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity16BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.HotPink;
            Item.Calamity().devItem = true;
            Item.defense = 8;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().cirrusDress = true;
            player.GetDamage(DamageClass.Magic) += 0.05f;
            player.GetCritChance(DamageClass.Magic) += 5;
        }

        public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
        {
            robes = true;
            // The equipSlot is added in CalamityMod.cs --> Load hook
            equipSlot = Mod.GetEquipSlot("CirrusDress_Legs", EquipType.Legs);
        }

        public override void DrawHands(ref bool drawHands, ref bool drawArms)
        {
            drawHands = true;
        }
    }
}
