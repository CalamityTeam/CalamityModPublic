using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class RealmRavager : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Realm Ravager");
            Tooltip.SetDefault("Shoots a burst of explosive bullets");
        }

        public override void SetDefaults()
        {
            item.damage = 50;
            item.ranged = true;
            item.width = 76;
            item.height = 32;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 4f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.UseSound = SoundID.Item38;
            item.autoReuse = true;
            item.shootSpeed = 30f;
            item.shoot = ModContent.ProjectileType<RealmRavagerBullet>();
            item.useAmmo = 97;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int index = 0; index < 5; ++index)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-75, 76) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-75, 76) * 0.05f;
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<RealmRavagerBullet>(), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            }
            return false;
        }
    }
}
