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
                                "You move faster and lose 18 defense for 3 seconds if you take damage");
        }

        public override void SetDefaults()
        {
            item.defense = 36;
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
			float moveSpeedDecrease = modPlayer.shellBoost ? 0.2f : 0.5f;
            player.moveSpeed -= moveSpeedDecrease;
            player.thorns += 0.25f;
			if (modPlayer.shellBoost)
				player.statDefense -= 18;
        }
    }
}
