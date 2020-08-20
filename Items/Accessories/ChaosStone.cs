using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class ChaosStone : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chaos Stone");
            Tooltip.SetDefault("One of the ancient relics\n" +
                "Increases max mana by 50, all damage by 3%, and reduces mana usage by 5%");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(8, 7));
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = CalamityGlobalItem.Rarity7BuyPrice;
            item.rare = 7;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, 0.85f, 0f, 0f);
            player.statManaMax2 += 50;
            player.manaCost *= 0.95f;
            player.allDamage += 0.03f;
        }
    }
}
