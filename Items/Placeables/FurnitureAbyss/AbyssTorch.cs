using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.FurnitureAbyss
{
    public class AbyssTorch : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 100;
            Tooltip.SetDefault("Can be placed in water");
        }

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 12;
            Item.maxStack = 99;
            Item.holdStyle = 1;
            Item.noWet = false;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.FurnitureAbyss.AbyssTorch>();
            Item.flame = true;
            Item.value = 500;
        }

        public override void HoldItem(Player player)
        {
            if (Main.rand.Next(player.itemAnimation > 0 ? 10 : 20) == 0)
            {
                Dust.NewDust(new Vector2(player.itemLocation.X + 16f * player.direction, player.itemLocation.Y - 14f * player.gravDir), 4, 4, 180);
            }
            Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);
            Lighting.AddLight(position, 1f, 1f, 1f);
        }

        public override void PostUpdate()
        {
            Lighting.AddLight((int)((Item.position.X + Item.width / 2) / 16f), (int)((Item.position.Y + Item.height / 2) / 16f), 1f, 1f, 1f);
        }

        public override void AutoLightSelect(ref bool dryTorch, ref bool wetTorch, ref bool glowstick)
        {
            wetTorch = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(3).AddIngredient(ItemID.Torch, 3).AddIngredient(ModContent.ItemType<Lumenite>()).Register();
        }
    }
}
