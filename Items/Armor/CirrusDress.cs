using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body, EquipType.Legs)]
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
            item.width = 18;
            item.height = 18;
			item.value = CalamityGlobalItem.Rarity16BuyPrice;
			item.Calamity().customRarity = CalamityRarity.HotPink;
			item.Calamity().devItem = true;
            item.defense = 8;
        }

        public override void UpdateEquip(Player player)
        {
			player.Calamity().cirrusDress = true;
            player.magicDamage += 0.05f;
            player.magicCrit += 5;
        }
    }
}
