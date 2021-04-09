using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class FlakToxicannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flak Toxicannon");
            Tooltip.SetDefault("Fires angled shots in the direction of the cursor\n" +
                               "Can only be shot in a cone direction above the player\n" +
                               "High IQ required");
        }
        public override void SetDefaults()
        {
            item.damage = 50;
            item.ranged = true;
            item.width = 88;
            item.height = 28;
            item.useTime = item.useAnimation = 16;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTurn = false;
            item.noMelee = true;
            item.knockBack = 7f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.UseSound = SoundID.Item109;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<ToxicannonShot>();
            item.shootSpeed = 9f;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float angle = new Vector2(speedX, speedY).ToRotation() + MathHelper.PiOver2;
            if (angle <= -MathHelper.PiOver4 || angle >= MathHelper.PiOver4)
                return false;
            angle -= MathHelper.PiOver2;

            Vector2 velocity = angle.ToRotationVector2() * (float)Math.Sqrt(speedX * speedX + speedY * speedY) * new Vector2(1f, 2f);

            Projectile.NewProjectile(position, velocity, ModContent.ProjectileType<ToxicannonShot>(), damage, knockBack, player.whoAmI);
            return false;
        }
    }
}
