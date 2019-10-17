using Microsoft.Xna.Framework;
using System;
using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items
{
    public class WintersFury : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Winter's Fury");
            Tooltip.SetDefault("The pages are freezing to the touch");
        }
        public override void SetDefaults()
        {
            item.damage = 40;
            item.magic = true;
            item.mana = 7;
            item.width = 28;
            item.height = 30;
            item.useTime = 9;
            item.useAnimation = 9;
            item.useStyle = 5;
            item.useTurn = false;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.UseSound = SoundID.Item9;
            item.scale = 0.9f;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<Icicle>();
            item.shootSpeed = 12f;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (Main.rand.Next(3) != 0)
            {
                Vector2 speed = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-15, 16)));
                speed.Normalize();
                speed *= 15f;
                speed.Y -= Math.Abs(speed.X) * 0.2f;
                int p = Projectile.NewProjectile(position, speed, ModContent.ProjectileType<FrostShardFriendly>(), damage, knockBack, player.whoAmI);
                Main.projectile[p].minion = false;
                Main.projectile[p].magic = true;
            }
            if (Main.rand.NextBool(3))
            {
                Main.PlaySound(SoundID.Item1, position);
                Projectile.NewProjectile(position.X, position.Y, speedX * 1.2f, speedY * 1.2f, ModContent.ProjectileType<Snowball>(), damage, knockBack * 2f, player.whoAmI);
            }
            speedX += Main.rand.Next(-40, 41) * 0.05f;
            speedY += Main.rand.Next(-40, 41) * 0.05f;
            return true;
        }
    }
}
