using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.BrimstoneCragCatches
{
    public class DragoonDrizzlefish : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragoon Drizzlefish");
            Tooltip.SetDefault(@"Fires an inaccurate spread of fireballs
The brimstone sac appears to contain fuel
Revenge is a dish best served flaming hot");
            Item.staff[item.type] = true; //so it doesn't look weird af when holding it
        }

        public override void SetDefaults()
        {
            item.damage = 20;
            item.ranged = true;
            item.width = 36;
            item.height = 30;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.UseSound = SoundID.Item20;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<DrizzlefishFireball>();
            item.shootSpeed = 11f;
        }

        public override Vector2? HoldoutOrigin() //so it looks normal when holding
        {
            return new Vector2(10, 10);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 rotated = new Vector2(speedX, speedY);
            rotated = rotated.RotatedByRandom(MathHelper.ToRadians(10f));
            speedX = rotated.X;
            speedY = rotated.Y;
            int shotType = ModContent.ProjectileType<DrizzlefishFireball>();
            if (Main.rand.NextBool(2))
            {
                shotType = ModContent.ProjectileType<DrizzlefishFire>();
            }
            else
            {
                shotType = ModContent.ProjectileType<DrizzlefishFireball>();
            }
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, shotType, damage, knockBack, player.whoAmI, 0f, Main.rand.Next(2));
            return false;
        }
    }
}
