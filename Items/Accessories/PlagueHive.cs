using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class PlagueHive : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Plague Hive");
            Tooltip.SetDefault("All attacks inflict the Plague and grants immunity to the Plague\n" +
                   "Releases bees when damaged that inflict the Plague\n" +
                   "Projectiles spawn plague seekers on enemy hits");
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 48;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.rare = ItemRarityID.Cyan;
            Item.accessory = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AlchemicalFlask>().
                AddIngredient(ItemID.HoneyComb).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.strongBees = true;
            modPlayer.uberBees = true;
            modPlayer.alchFlask = true;
            player.buffImmune[ModContent.BuffType<Plague>()] = true;
        }
    }
}
