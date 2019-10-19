using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.FurnitureAbyss
{
    public class AbyssTorch : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Can be placed in water");
        }

        public override void SetDefaults()
        {
            item.width = 10;
            item.height = 12;
            item.maxStack = 99;
            item.holdStyle = 1;
            item.noWet = false;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.createTile = ModContent.TileType<Tiles.FurnitureAbyss.AbyssTorch>();
            item.flame = false;
            item.value = 500;
        }

        public override void HoldItem(Player player)
        {
            if (Main.rand.Next(player.itemAnimation > 0 ? 40 : 80) == 0)
            {
                // TODO -- This dust was an invalid dust. Replaced with a random dust.
                Dust.NewDust(new Vector2(player.itemLocation.X + 16f * player.direction, player.itemLocation.Y - 14f * player.gravDir), 4, 4, 135);
            }
            Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);
            Lighting.AddLight(position, 1f, 1f, 1f);
        }

        public override void PostUpdate()
        {
            if (!item.wet)
            {
                Lighting.AddLight((int)((item.position.X + item.width / 2) / 16f), (int)((item.position.Y + item.height / 2) / 16f), 1f, 1f, 1f);
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Torch, 3);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>());
            recipe.SetResult(this, 3);
            recipe.AddRecipe();
        }
    }
}
