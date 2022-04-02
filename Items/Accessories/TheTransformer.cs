using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;

namespace CalamityMod.Items.Accessories
{
    public class TheTransformer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Transformer");
            Tooltip.SetDefault("Taking damage releases a blast of sparks\n" +
                                "Immunity to Electrified and you resist all electrical projectile and enemy damage\n" +
                                "Enemy bullets do half damage to you and are reflected back at the enemy for 800% their original damage");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 16));
        }

        public override void SetDefaults()
        {
            item.width = 42;
            item.height = 56;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = ItemRarityID.LightRed;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.aSparkRare = true;
            modPlayer.aSpark = true;
        }
    }
}
