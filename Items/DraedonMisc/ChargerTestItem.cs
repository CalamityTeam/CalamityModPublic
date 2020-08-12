using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.DraedonMisc
{
    public class ChargerTestItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Charger Test Item");
            Tooltip.SetDefault("Testing/Cheat Item\n" +
                               "Charges all Draedon's Arsenal weapons in your inventory");
        }

        public override void SetDefaults()
        {
            item.width = 42;
            item.height = 42;
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.DraedonRust;

            item.consumable = false;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.useTime = 29;
            item.useAnimation = 29;
            item.autoReuse = false;
            item.useTurn = true;
        }

        public override bool UseItem(Player player)
        {
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].type > ItemID.Count && player.inventory[i].Calamity().Chargeable)
                    player.inventory[i].Calamity().CurrentCharge = player.inventory[i].Calamity().ChargeMax;
            }
            return true;
        }

    }
}
