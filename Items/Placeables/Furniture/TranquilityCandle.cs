using CalamityMod.Items.Materials;
using CalamityMod.Items.Potions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class TranquilityCandle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tranquility Candle");
            Tooltip.SetDefault("The mere presence of this candle calms surrounding enemies drastically");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 20;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.value = 500;
            item.createTile = ModContent.TileType<Tiles.Furniture.TranquilityCandle>();
            item.flame = true;
            item.holdStyle = 1;
        }

        public override void HoldItem(Player player)
        {
            player.Calamity().tranquilityCandle = true;
            if (Main.rand.Next(player.itemAnimation > 0 ? 10 : 20) == 0)
            {
                Dust.NewDust(new Vector2(player.itemLocation.X + 10f * player.direction, player.itemLocation.Y - 12f * player.gravDir), 4, 4, 62);
            }
            player.itemLocation.Y += 8;
            Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);
            Lighting.AddLight(position, 0.55f, 0.85f, 1f);
        }

        public override void PostUpdate()
        {
            Lighting.AddLight((int)((item.position.X + item.width / 2) / 16f), (int)((item.position.Y + item.height / 2) / 16f), 1f, 0.55f, 1f);
        }

        public override void AutoLightSelect(ref bool dryTorch, ref bool wetTorch, ref bool glowstick)
        {
            wetTorch = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.PeaceCandle, 3);
            recipe.AddIngredient(ItemID.SoulofLight, 3);
            recipe.AddIngredient(ModContent.ItemType<CoreofEleum>(), 2);
            recipe.AddIngredient(ModContent.ItemType<ZenPotion>());
            recipe.SetResult(this);
            recipe.AddTile(TileID.WorkBenches);
            recipe.AddRecipe();
        }
    }
}
