using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Neck)]
    public class BloodyWormScarf : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloody Worm Scarf");
            Tooltip.SetDefault("10% increased damage reduction and increased melee stats");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 42;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.expert = true;
            item.rare = ItemRarityID.LightRed;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.meleeDamage += 0.1f;
            player.meleeSpeed += 0.1f;
            player.endurance += 0.1f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BloodyWormTooth>());
            recipe.AddIngredient(ItemID.WormScarf);
            recipe.AddIngredient(ItemID.SoulofNight, 3);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
