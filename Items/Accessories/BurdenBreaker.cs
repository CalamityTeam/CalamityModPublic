using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class BurdenBreaker : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Determination Breaker");
            Tooltip.SetDefault("The bad time\n" +
                "Removes immunity frames\n" +
                "If you want a crazy challenge, equip this\n" +
                "Touching lava kills you instantly");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = ItemRarityID.Orange;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.lavaImmune = false;
            if (Collision.LavaCollision(player.position, player.width, player.waterWalk ? (player.height - 6) : player.height))
            {
                player.Calamity().KillPlayer();
            }
            else if (player.immune)
            {
                player.immune = false;
                player.immuneTime = 0;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Bone, 50);
            recipe.AddIngredient(ItemID.IronBar, 7);
            recipe.anyIronBar = true;
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
