using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Magic
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
            Item.damage = 59;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 7;
            Item.width = 36;
            Item.height = 40;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTurn = false;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = Item.buyPrice(0, 80, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item9;
            Item.scale = 0.9f;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Icicle>();
            Item.shootSpeed = 15f;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (Main.rand.Next(4) != 0)
            {
                Vector2 speed = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-15, 16)));
                speed.Normalize();
                speed *= 15f;
                speed.Y -= Math.Abs(speed.X) * 0.2f;
                int p = Projectile.NewProjectile(position, speed, ModContent.ProjectileType<FrostShardFriendly>(), damage, knockBack, player.whoAmI);
                if (p.WithinBounds(Main.maxProjectiles))
                    Main.projectile[p].Calamity().forceMagic = true;
            }
            if (Main.rand.NextBool(4))
            {
                SoundEngine.PlaySound(SoundID.Item1, position);
                Projectile.NewProjectile(position.X, position.Y, speedX * 1.2f, speedY * 1.2f, ModContent.ProjectileType<Snowball>(), damage, knockBack * 2f, player.whoAmI);
            }
            speedX += Main.rand.Next(-40, 41) * 0.05f;
            speedY += Main.rand.Next(-40, 41) * 0.05f;
            return true;
        }
    }
}
