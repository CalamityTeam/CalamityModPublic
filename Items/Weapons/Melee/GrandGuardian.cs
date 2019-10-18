using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Melee
{
    public class GrandGuardian : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grand Guardian");
            Tooltip.SetDefault("Has a chance to lower enemy defense by 15 when striking them\n" +
                       "If enemy defense is 0 or below your attacks will heal you\n" +
                       "Striking enemies causes a large explosion\n" +
                       "Striking enemies that have under half life will make you release rainbow bolts\n" +
                       "Enemies spawn healing orbs on death");
        }

        public override void SetDefaults()
        {
            item.width = 124;
            item.damage = 150;
            item.melee = true;
            item.useAnimation = 22;
            item.useStyle = 1;
            item.useTime = 22;
            item.useTurn = true;
            item.knockBack = 8.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 124;
            item.value = Item.buyPrice(1, 0, 0, 0);
            item.rare = 10;
            item.shootSpeed = 12f;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.NextBool(5))
            {
                target.defense -= 15;
            }
            if (target.defense <= 0 && target.canGhostHeal)
            {
                player.statLife += 4;
                player.HealEffect(4);
            }
            Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<RainbowBoom>(), (int)((float)item.damage * player.meleeDamage * 0.5f), 0f, player.whoAmI);
            if (target.life <= (target.lifeMax * 0.5f))
            {
                float randomSpeedX = (float)Main.rand.Next(9);
                float randomSpeedY = (float)Main.rand.Next(6, 15);
                Projectile.NewProjectile(player.Center.X, player.Center.Y, -randomSpeedX, -randomSpeedY, ModContent.ProjectileType<RainBolt>(), (int)((float)item.damage * player.meleeDamage * 0.75f), knockback, player.whoAmI);
                Projectile.NewProjectile(player.Center.X, player.Center.Y, randomSpeedX, -randomSpeedY, ModContent.ProjectileType<RainBolt>(), (int)((float)item.damage * player.meleeDamage * 0.75f), knockback, player.whoAmI);
                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -randomSpeedY, ModContent.ProjectileType<RainBolt>(), (int)((float)item.damage * player.meleeDamage * 0.75f), knockback, player.whoAmI);
            }
            if (target.life <= 0)
            {
                float randomSpeedX = (float)Main.rand.Next(9);
                float randomSpeedY = (float)Main.rand.Next(6, 15);
                Projectile.NewProjectile(target.Center.X, target.Center.Y, -randomSpeedX, -randomSpeedY, ModContent.ProjectileType<RainHeal>(), 0, 0f, player.whoAmI);
                Projectile.NewProjectile(target.Center.X, target.Center.Y, randomSpeedX, -randomSpeedY, ModContent.ProjectileType<RainHeal>(), 0, 0f, player.whoAmI);
                Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, -randomSpeedY, ModContent.ProjectileType<RainHeal>(), 0, 0f, player.whoAmI);
            }
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 66, 0f, 0f, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
                Main.dust[dust].noGravity = true;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "MajesticGuard");
            recipe.AddIngredient(null, "BarofLife", 10);
            recipe.AddIngredient(null, "GalacticaSingularity", 3);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
