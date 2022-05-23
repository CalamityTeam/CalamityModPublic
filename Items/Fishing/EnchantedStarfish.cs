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
            SacrificeTotal = 10;
        }

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Green;
            Item.width = 30;
            Item.height = 26;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item29;
            Item.consumable = true;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 50);
        }

        public override bool? UseItem(Player player)
        {
            if (player.itemAnimation > 0 && (player.statManaMax < 200 && player.itemTime == 0))
            {
                player.itemTime = Item.useTime;
                player.statManaMax += 20;
                player.statManaMax2 += 20;
                player.statMana += 20;
                if (Main.myPlayer == player.whoAmI)
                    player.ManaEffect(20);
                AchievementsHelper.HandleSpecialEvent(player, 1);
            }
            return false;
        }
    }
}
