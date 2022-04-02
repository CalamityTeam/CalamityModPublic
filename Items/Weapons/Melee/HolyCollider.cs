using CalamityMod.Projectiles.Melee;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class HolyCollider : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Collider");
            Tooltip.SetDefault("Striking enemies will cause them to explode into holy fire");
        }

        public override void SetDefaults()
        {
            item.width = 94;
            item.height = 80;
            item.scale = 1.5f;
            item.damage = 270;
            item.melee = true;
            item.useAnimation = 22;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 22;
            item.useTurn = true;
            item.knockBack = 7.75f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.shootSpeed = 10f;

            item.rare = ItemRarityID.Purple;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            Main.PlaySound(SoundID.Item14, target.position);
            float spread = 45f * 0.0174f;
            double startAngle = Math.Atan2(item.shootSpeed, item.shootSpeed) - spread / 2;
            double deltaAngle = spread / 8f;
            double offsetAngle;
            int i;
            for (i = 0; i < 4; i++)
            {
                offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                Projectile.NewProjectile(target.Center.X, target.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<HolyColliderHolyFire>(), (int)(item.damage * (player.allDamage + player.meleeDamage - 1f) * 0.3), knockback, Main.myPlayer);
                Projectile.NewProjectile(target.Center.X, target.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<HolyColliderHolyFire>(), (int)(item.damage * (player.allDamage + player.meleeDamage - 1f) * 0.3), knockback, Main.myPlayer);
            }
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            Main.PlaySound(SoundID.Item14, target.position);
            float spread = 45f * 0.0174f;
            double startAngle = Math.Atan2(item.shootSpeed, item.shootSpeed) - spread / 2;
            double deltaAngle = spread / 8f;
            double offsetAngle;
            int i;
            for (i = 0; i < 4; i++)
            {
                offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                Projectile.NewProjectile(target.Center.X, target.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<HolyColliderHolyFire>(), (int)(item.damage * (player.allDamage + player.meleeDamage - 1f) * 0.3), item.knockBack, Main.myPlayer);
                Projectile.NewProjectile(target.Center.X, target.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<HolyColliderHolyFire>(), (int)(item.damage * (player.allDamage + player.meleeDamage - 1f) * 0.3), item.knockBack, Main.myPlayer);
            }
        }
    }
}
