using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class BiomeBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Biome Blade");
            Tooltip.SetDefault("Fires different projectiles based on what biome you're in");
        }

        public override void SetDefaults()
        {
            item.width = 42;
            item.damage = 38;
            item.melee = true;
            item.useAnimation = 30;
            item.useTime = 30;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 42;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.shoot = ModContent.ProjectileType<BiomeOrb>();
            item.shootSpeed = 12f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.WoodenSword);
            recipe.AddIngredient(ItemID.DirtBlock, 20);
            recipe.AddIngredient(ItemID.SandBlock, 20);
            recipe.AddIngredient(ItemID.IceBlock, 20); //intentionally not any ice
            recipe.AddRecipeGroup("AnyEvilBlock", 20);
            recipe.AddIngredient(ItemID.GlowingMushroom, 20);
            recipe.AddIngredient(ItemID.Marble, 20);
            recipe.AddIngredient(ItemID.Granite, 20);
            recipe.AddIngredient(ItemID.Hellstone, 20);
            recipe.AddIngredient(ItemID.Coral, 20);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 0);
            }
        }
    }
}
