using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing
{
    public class EnchantedStarfish : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Fishing";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 10;
            ItemID.Sets.CanBePlacedOnWeaponRacks[Type] = true;
            // For some reason Life/Mana boosting items are in this set (along with Magic Mirror+)
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 21; // Mana Crystal
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.ArcaneCrystal;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 26;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item29;
            Item.maxStack = Item.CommonMaxStack;
            Item.autoReuse = true;
            Item.consumable = true;

            Item.value = Item.sellPrice(silver: 25);
            Item.rare = ItemRarityID.Green;
        }

        public override bool? UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.itemTime = Item.useTime;
                // Still has the holding animation if fully consumed Mana Crystals, but not consuming the item
                if (player.ConsumedManaCrystals >= Player.ManaCrystalMax)
                    return null;

                player.UseManaMaxIncreasingItem(20);
                player.ConsumedManaCrystals++;
                AchievementsHelper.HandleSpecialEvent(player, 1);
            }
            return true;
        }

        public override bool ConsumeItem(Player player) => player.ConsumedManaCrystals < Player.ManaCrystalMax;
    }
}
