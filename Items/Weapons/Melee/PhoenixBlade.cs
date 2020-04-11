using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Healing;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class PhoenixBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phoenix Blade");
            Tooltip.SetDefault("Enemies explode and emit healing flames on death");
        }

        public override void SetDefaults()
        {
            item.width = 106;
            item.damage = 120;
            item.melee = true;
            item.useAnimation = 27;
            item.useStyle = 1;
            item.useTime = 27;
            item.useTurn = true;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 106;
            item.value = Item.buyPrice(0, 48, 0, 0);
            item.rare = 6;
            item.shootSpeed = 12f;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (target.life <= 0)
            {
                int boom = Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<FuckYou>(), (int)(item.damage * (player.allDamage + player.meleeDamage - 1f)), knockback, player.whoAmI, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
                Main.projectile[boom].Calamity().forceMelee = true;
                float randomSpeedX = (float)Main.rand.Next(5);
                float randomSpeedY = (float)Main.rand.Next(3, 7);
                Projectile.NewProjectile(target.Center.X, target.Center.Y, -randomSpeedX, -randomSpeedY, ModContent.ProjectileType<PhoenixHeal>(), (int)(item.damage * player.MeleeDamage()), knockback, player.whoAmI);
                Projectile.NewProjectile(target.Center.X, target.Center.Y, randomSpeedX, -randomSpeedY, ModContent.ProjectileType<PhoenixHeal>(), (int)(item.damage * player.MeleeDamage()), knockback, player.whoAmI);
            }
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (target.statLife <= 0)
            {
                int boom = Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<FuckYou>(), (int)(item.damage * (player.allDamage + player.meleeDamage - 1f)), item.knockBack, player.whoAmI, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
                Main.projectile[boom].Calamity().forceMelee = true;
                float randomSpeedX = (float)Main.rand.Next(5);
                float randomSpeedY = (float)Main.rand.Next(3, 7);
                Projectile.NewProjectile(target.Center.X, target.Center.Y, -randomSpeedX, -randomSpeedY, ModContent.ProjectileType<PhoenixHeal>(), (int)(item.damage * (player.allDamage + player.meleeDamage - 1f)), item.knockBack, player.whoAmI);
                Projectile.NewProjectile(target.Center.X, target.Center.Y, randomSpeedX, -randomSpeedY, ModContent.ProjectileType<PhoenixHeal>(), (int)(item.damage * (player.allDamage + player.meleeDamage - 1f)), item.knockBack, player.whoAmI);
            }
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 244);
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BreakerBlade);
            recipe.AddIngredient(ItemID.HellstoneBar, 10);
            recipe.AddIngredient(ModContent.ItemType<EssenceofCinder>());
            recipe.AddIngredient(ItemID.SoulofMight, 3);
            recipe.AddIngredient(ItemID.SoulofSight, 3);
            recipe.AddIngredient(ItemID.SoulofFright, 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
