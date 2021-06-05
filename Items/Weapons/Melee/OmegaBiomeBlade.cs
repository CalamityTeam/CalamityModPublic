using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class OmegaBiomeBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Omega Biome Blade");
            Tooltip.SetDefault("Fires different homing projectiles based on what biome you're in\n" +
                "Projectiles also change based on moon events");
        }

        public override void SetDefaults()
        {
            item.width = 62;
            item.damage = 130;
            item.melee = true;
            item.useAnimation = 18;
            item.useTime = 18;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 8;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 62;
			item.value = CalamityGlobalItem.Rarity11BuyPrice;
			item.rare = ItemRarityID.Purple;
			item.shoot = ModContent.ProjectileType<OmegaBiomeOrb>();
            item.shootSpeed = 15f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int projectiles = 0; projectiles < 3; projectiles++)
            {
				float SpeedX = speedX + Main.rand.Next(-40, 41) * 0.05f;
				float SpeedY = speedY + Main.rand.Next(-40, 41) * 0.05f;
				Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<TrueBiomeBlade>());
            recipe.AddIngredient(ModContent.ItemType<CoreofCalamity>());
            recipe.AddIngredient(ModContent.ItemType<BarofLife>(), 3);
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 3);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
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
