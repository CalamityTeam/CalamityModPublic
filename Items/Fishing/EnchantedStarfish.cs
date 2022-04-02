using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Achievements;
namespace CalamityMod.Items.Fishing
{
    public class EnchantedStarfish : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Enchanted Starfish");
            Tooltip.SetDefault("Permanently increases maximum mana by 20");
        }

        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Green;
            item.width = 30;
            item.height = 26;
            item.useAnimation = 30;
            item.useTime = 30;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item29;
            item.consumable = true;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 50);
        }

        public override bool UseItem(Player player)
        {
            if (player.itemAnimation > 0 && (player.statManaMax < 200 && player.itemTime == 0))
            {
                player.itemTime = item.useTime;
                player.statManaMax = player.statManaMax + 20;
                player.statManaMax2 = player.statManaMax2 + 20;
                player.statMana = player.statMana + 20;
                if (Main.myPlayer == player.whoAmI)
                    player.ManaEffect(20);
                AchievementsHelper.HandleSpecialEvent(player, 1);
            }
            return false;
        }
    }
}
