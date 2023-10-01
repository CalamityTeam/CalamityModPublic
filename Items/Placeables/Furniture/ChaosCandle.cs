using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Potions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class ChaosCandle : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 20;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = 500;
            Item.createTile = ModContent.TileType<Tiles.Furniture.ChaosCandle>();
            Item.flame = true;
            Item.holdStyle = 1;
        }

        public override void HoldItem(Player player)
        {
            player.Calamity().chaosCandle = true;
            if (Main.rand.Next(player.itemAnimation > 0 ? 10 : 20) == 0)
            {
                Dust.NewDust(new Vector2(player.itemLocation.X + 12f * player.direction, player.itemLocation.Y - 10f * player.gravDir), 4, 4, (int)CalamityDusts.Brimstone);
            }
            player.itemLocation.Y += 8;
            Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);
            Lighting.AddLight(position, 0.85f, 0.25f, 0.25f);
        }

        public override void PostUpdate()
        {
            Lighting.AddLight((int)((Item.position.X + Item.width / 2) / 16f), (int)((Item.position.Y + Item.height / 2) / 16f), 0.85f, 0.25f, 0.25f);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.WaterCandle, 3).
                AddIngredient(ItemID.SoulofNight, 3).
                AddIngredient<CoreofHavoc>(2).
                AddIngredient<ZergPotion>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
