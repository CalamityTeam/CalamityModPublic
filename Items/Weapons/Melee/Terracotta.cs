using Terraria.DataStructures;
using CalamityMod.Projectiles.Healing;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Terracotta : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terra-cotta");
            Tooltip.SetDefault("Causes enemies to erupt into healing projectiles on death\n" +
                "Enemies explode on hit");
        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 62;
            Item.scale = 1.5f;
            Item.damage = 110;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(0, 80, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.shootSpeed = 5f;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            var source = player.GetProjectileSource_Item(Item);
            if (target.life <= 0)
            {
                float randomSpeedX = Main.rand.Next(3);
                float randomSpeedY = Main.rand.Next(3, 5);
                Projectile.NewProjectile(source, target.Center.X, target.Center.Y, -randomSpeedX, -randomSpeedY, ModContent.ProjectileType<TerracottaProj>(), 0, 0f, player.whoAmI, player.whoAmI);
                Projectile.NewProjectile(source, target.Center.X, target.Center.Y, randomSpeedX, -randomSpeedY, ModContent.ProjectileType<TerracottaProj>(), 0, 0f, player.whoAmI, player.whoAmI);
                Projectile.NewProjectile(source, target.Center.X, target.Center.Y, 0f, -randomSpeedY, ModContent.ProjectileType<TerracottaProj>(), 0, 0f, player.whoAmI, player.whoAmI);
            }
            Projectile.NewProjectile(source, target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<TerracottaExplosion>(), (int)(Item.damage * (player.GetDamage<GenericDamageClass>().Additive + player.GetDamage(DamageClass.Melee) - 1f)), knockback, player.whoAmI);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            var source = player.GetProjectileSource_Item(Item);
            if (target.statLife <= 0)
            {
                float randomSpeedX = Main.rand.Next(3);
                float randomSpeedY = Main.rand.Next(3, 5);
                Projectile.NewProjectile(source, target.Center.X, target.Center.Y, -randomSpeedX, -randomSpeedY, ModContent.ProjectileType<TerracottaProj>(), 0, 0f, player.whoAmI, player.whoAmI);
                Projectile.NewProjectile(source, target.Center.X, target.Center.Y, randomSpeedX, -randomSpeedY, ModContent.ProjectileType<TerracottaProj>(), 0, 0f, player.whoAmI, player.whoAmI);
                Projectile.NewProjectile(source, target.Center.X, target.Center.Y, 0f, -randomSpeedY, ModContent.ProjectileType<TerracottaProj>(), 0, 0f, player.whoAmI, player.whoAmI);
            }
            Projectile.NewProjectile(source, target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<TerracottaExplosion>(), (int)(Item.damage * (player.GetDamage<GenericDamageClass>().Additive + player.GetDamage(DamageClass.Melee) - 1f)), Item.knockBack, player.whoAmI);
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 246);
        }
    }
}
