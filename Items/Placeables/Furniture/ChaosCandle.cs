using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Potions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class ChaosCandle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chaos Candle");
            Tooltip.SetDefault("The mere presence of this candle enrages surrounding enemies drastically");
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
            item.useStyle = 1;
            item.consumable = true;
            item.value = 500;
            item.createTile = ModContent.TileType<Tiles.Furniture.ChaosCandle>();
            item.flame = true;
            item.holdStyle = 1;
        }

        public override void HoldItem(Player player)
        {
            player.Calamity().chaosCandle = true;
            if (Main.rand.Next(player.itemAnimation > 0 ? 10 : 20) == 0)
            {
                Dust.NewDust(new Vector2(player.itemLocation.X + 12f * player.direction, player.itemLocation.Y - 10f * player.gravDir), 4, 4, 235);
            }
            player.itemLocation.Y += 8;
            Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);
            Lighting.AddLight(position, 0.85f, 0.25f, 0.25f);
        }

        public override void PostUpdate()
        {
            Lighting.AddLight((int)((item.position.X + item.width / 2) / 16f), (int)((item.position.Y + item.height / 2) / 16f), 0.85f, 0.25f, 0.25f);
        }

        public override void AutoLightSelect(ref bool dryTorch, ref bool wetTorch, ref bool glowstick)
        {
            wetTorch = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.WaterCandle, 3);
            recipe.AddIngredient(ItemID.SoulofNight, 3);
            recipe.AddIngredient(ModContent.ItemType<CoreofChaos>(), 2);
            recipe.AddIngredient(ModContent.ItemType<ZergPotion>());
            recipe.SetResult(this);
            recipe.AddTile(18);
            recipe.AddRecipe();
        }
    }
}
