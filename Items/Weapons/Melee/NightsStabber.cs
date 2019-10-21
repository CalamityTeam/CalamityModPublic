using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class NightsStabber : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Night's Stabber");
            Tooltip.SetDefault("Don't underestimate the power of stabby knives\n" +
                "Enemies release homing dark energy on death");
        }

        public override void SetDefaults()
        {
            item.useStyle = 3;
            item.useTurn = false;
            item.useAnimation = 15;
            item.useTime = 15;
            item.width = 30;
            item.height = 30;
            item.damage = 52;
            item.melee = true;
            item.knockBack = 6.5f;
            item.UseSound = SoundID.Item1;
            item.useTurn = true;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AncientShiv>());
            recipe.AddIngredient(ModContent.ItemType<SporeKnife>());
            recipe.AddIngredient(ModContent.ItemType<FlameburstShortsword>());
            recipe.AddIngredient(ModContent.ItemType<LeechingDagger>());
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AncientShiv>());
            recipe.AddIngredient(ModContent.ItemType<SporeKnife>());
            recipe.AddIngredient(ModContent.ItemType<FlameburstShortsword>());
            recipe.AddIngredient(ModContent.ItemType<BloodyRupture>());
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 14);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (target.life <= 0)
            {
                for (int i = 0; i <= 2; i++)
                {
                    Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<NightStabber>(), (int)((float)item.damage * player.meleeDamage), knockback, Main.myPlayer);
                }
            }
        }
    }
}
