using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Providence
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
            item.damage = 400;
            item.melee = true;
            item.useAnimation = 22;
            item.useStyle = 1;
            item.useTime = 22;
            item.useTurn = true;
            item.knockBack = 7.75f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 80;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.shootSpeed = 10f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 12;
		}

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            float spread = 45f * 0.0174f;
            double startAngle = Math.Atan2(item.shootSpeed, item.shootSpeed) - spread / 2;
            double deltaAngle = spread / 8f;
            double offsetAngle;
            int i;
            for (i = 0; i < 4; i++)
            {
                offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i;
                Projectile.NewProjectile(target.Center.X, target.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), mod.ProjectileType("HolyColliderHolyFire"), (int)((double)((float)item.damage * player.meleeDamage) * 0.3), knockback, Main.myPlayer);
                Projectile.NewProjectile(target.Center.X, target.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), mod.ProjectileType("HolyColliderHolyFire"), (int)((double)((float)item.damage * player.meleeDamage) * 0.3), knockback, Main.myPlayer);
            }
        }
    }
}
