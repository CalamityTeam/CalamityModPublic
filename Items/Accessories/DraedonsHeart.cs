using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class DraedonsHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Draedon's Heart");
            Tooltip.SetDefault("Reduces the amount of defense damage you take by 50%\n" +
                "Standing still regenerates your life quickly, reduces your damage by 50% and boosts your defense by 75%\n" +
                "Nanomachines, son");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 11));
        }

        public override void SetDefaults()
        {
            item.width = 52;
            item.height = 68;
            item.accessory = true;
            item.rare = ItemRarityID.Red;
            item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.draedonsHeart = true;
        }
    }
}
