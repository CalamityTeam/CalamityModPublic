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
                "Enemies release homing dark energy on hit");
        }

        public override void SetDefaults()
        {
            item.useStyle = ItemUseStyleID.Stabbing;
            item.useTurn = false;
            item.useAnimation = 15;
            item.useTime = 15;
            item.width = 28;
            item.height = 34;
            item.damage = 60;
            item.melee = true;
            item.knockBack = 6.5f;
            item.UseSound = SoundID.Item1;
            item.useTurn = true;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<LeechingDagger>());
            recipe.AddIngredient(ModContent.ItemType<AncientShiv>());
            recipe.AddIngredient(ModContent.ItemType<SporeKnife>());
            recipe.AddIngredient(ModContent.ItemType<FlameburstShortsword>());
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BloodyRupture>());
            recipe.AddIngredient(ModContent.ItemType<AncientShiv>());
            recipe.AddIngredient(ModContent.ItemType<SporeKnife>());
            recipe.AddIngredient(ModContent.ItemType<FlameburstShortsword>());
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 14);
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<NightStabber>(), (int)(item.damage * (player.allDamage + player.meleeDamage - 1f)), knockback, Main.myPlayer);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<NightStabber>(), (int)(item.damage * (player.allDamage + player.meleeDamage - 1f)), item.knockBack, Main.myPlayer);
        }
    }
}
