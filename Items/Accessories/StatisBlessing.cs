using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Neck)]
    public class StatisBlessing : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Statis' Blessing");
            Tooltip.SetDefault("Increased max minions by 2 and 10% increased minion damage\n" +
                "Increased minion knockback\n" +
                "Minions inflict holy flames on hit");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.holyMinions = true;
            player.minionKB += 2.5f;
            player.GetDamage(DamageClass.Summon) += 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.PapyrusScarab).AddIngredient(ItemID.PygmyNecklace).AddIngredient(ItemID.SummonerEmblem).AddIngredient(ItemID.HolyWater, 30).AddIngredient(ModContent.ItemType<CoreofCinder>(), 5).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
