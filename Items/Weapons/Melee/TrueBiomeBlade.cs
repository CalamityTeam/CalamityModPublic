using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TrueBiomeBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("True Biome Blade");
            Tooltip.SetDefault("Fires different projectiles based on what biome you're in");
        }

        public override void SetDefaults()
        {
            item.width = 54;
            item.damage = 160;
            item.melee = true;
            item.useAnimation = 21;
            item.useTime = 21;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 7.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 54;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.shoot = ModContent.ProjectileType<TrueBiomeOrb>();
            item.shootSpeed = 12f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BiomeBlade>());
            recipe.AddIngredient(ModContent.ItemType<LivingShard>(), 5);
            recipe.AddIngredient(ItemID.Ectoplasm, 5);
            recipe.AddIngredient(ModContent.ItemType<DepthCells>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Tenebris>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
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
