using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Mariana : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mariana");
            Tooltip.SetDefault("Tropical and deadly\n" +
                "Enemies explode into water orbs on hit");
        }

        public override void SetDefaults()
        {
            item.damage = 90;
            item.width = 54;
            item.height = 62;
            item.scale = 1.5f;
            item.melee = true;
            item.useAnimation = 24;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 24;
            item.useTurn = true;
            item.knockBack = 6.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = ItemRarityID.Lime;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.ChlorophyteClaymore);
            recipe.AddIngredient(ItemID.Coral, 3);
            recipe.AddIngredient(ItemID.Starfish, 3);
            recipe.AddIngredient(ItemID.Seashell, 3);
            recipe.AddIngredient(ModContent.ItemType<DepthCells>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Tenebris>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            int num251 = Main.rand.Next(2, 4);
            for (int num252 = 0; num252 < num251; num252++)
            {
                Vector2 value15 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                while (value15.X == 0f && value15.Y == 0f)
                {
                    value15 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                }
                value15.Normalize();
                value15 *= Main.rand.Next(70, 101) * 0.1f;
                Projectile.NewProjectile(target.Center.X, target.Center.Y, value15.X, value15.Y, ModContent.ProjectileType<MarianaProjectile>(), (int)(item.damage * (player.allDamage + player.meleeDamage - 1f)), knockback, player.whoAmI, 0f, 0f);
            }
            for (int num621 = 0; num621 < 30; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 59, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 50; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 59, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 59, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            int num251 = Main.rand.Next(2, 4);
            for (int num252 = 0; num252 < num251; num252++)
            {
                Vector2 value15 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                while (value15.X == 0f && value15.Y == 0f)
                {
                    value15 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                }
                value15.Normalize();
                value15 *= Main.rand.Next(70, 101) * 0.1f;
                Projectile.NewProjectile(target.Center.X, target.Center.Y, value15.X, value15.Y, ModContent.ProjectileType<MarianaProjectile>(), (int)(item.damage * (player.allDamage + player.meleeDamage - 1f)), item.knockBack, player.whoAmI, 0f, 0f);
            }
            for (int num621 = 0; num621 < 30; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 59, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 50; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 59, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 59, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 59);
            }
        }
    }
}
