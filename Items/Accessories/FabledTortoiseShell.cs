using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class FabledTortoiseShell : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fabled Tortoise Shell");
            Tooltip.SetDefault("50% reduced movement speed\n" +
                                "Enemies take damage when they hit you\n" +
                                "You move quickly for a short time if you take damage");
        }

        public override void SetDefaults()
        {
            item.defense = 35;
            item.width = 20;
            item.height = 24;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = 5;
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.RareVariant;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.fabledTortoise = true;
            player.moveSpeed -= 0.5f;
            player.thorns += 0.25f;
        }
    }
}
