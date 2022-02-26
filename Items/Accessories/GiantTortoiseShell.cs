using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
	public class GiantTortoiseShell : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Giant Tortoise Shell");
            Tooltip.SetDefault("10% decreased movement speed\n" +
                "Enemies take damage when they hit you");
        }

        public override void SetDefaults()
        {
            item.defense = 14;
            item.width = 20;
            item.height = 24;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = ItemRarityID.LightRed;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.moveSpeed -= 0.1f;
            player.thorns += 0.25f;
        }
    }
}
