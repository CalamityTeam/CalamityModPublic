using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class FlameburstShortsword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flameburst Shortsword");
            Tooltip.SetDefault("Enemies explode on hit");
        }

        public override void SetDefaults()
        {
            item.useStyle = ItemUseStyleID.Stabbing;
            item.useTurn = false;
            item.useAnimation = 15;
            item.useTime = 15;
            item.width = 30;
            item.height = 30;
            item.damage = 35;
            item.melee = true;
            item.knockBack = 5.5f;
            item.UseSound = SoundID.Item1;
            item.useTurn = true;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HellstoneBar, 7);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 6);
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            int boom = Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<FuckYou>(), (int)(item.damage * 0.75f * player.MeleeDamage()), knockback, player.whoAmI, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
            if (boom.WithinBounds(Main.maxProjectiles))
                Main.projectile[boom].Calamity().forceMelee = true;
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            int boom = Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<FuckYou>(), (int)(item.damage * 0.75f * player.MeleeDamage()), item.knockBack, player.whoAmI, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
            if (boom.WithinBounds(Main.maxProjectiles))
                Main.projectile[boom].Calamity().forceMelee = true;
        }
    }
}
