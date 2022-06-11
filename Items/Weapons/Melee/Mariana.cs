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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 90;
            Item.width = 54;
            Item.height = 62;
            Item.scale = 1.5f;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 24;
            Item.useTurn = true;
            Item.knockBack = 6.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(0, 60, 0, 0);
            Item.rare = ItemRarityID.Lime;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            var source = player.GetSource_ItemUse(Item);
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
                int projDamage = player.CalcIntDamage<MeleeDamageClass>(Item.damage);
                Projectile.NewProjectile(source, target.Center.X, target.Center.Y, value15.X, value15.Y, ModContent.ProjectileType<MarianaProjectile>(), projDamage, knockback, player.whoAmI, 0f, 0f);
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
            var source = player.GetSource_ItemUse(Item);
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
                int projDamage = player.CalcIntDamage<MeleeDamageClass>(Item.damage);
                Projectile.NewProjectile(source, target.Center.X, target.Center.Y, value15.X, value15.Y, ModContent.ProjectileType<MarianaProjectile>(), projDamage, Item.knockBack, player.whoAmI, 0f, 0f);
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

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ChlorophyteClaymore).
                AddIngredient(ItemID.Coral, 3).
                AddIngredient(ItemID.Starfish, 3).
                AddIngredient(ItemID.Seashell, 3).
                AddIngredient<DepthCells>(10).
                AddIngredient<Lumenyl>(10).
                AddIngredient<Tenebris>(5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
