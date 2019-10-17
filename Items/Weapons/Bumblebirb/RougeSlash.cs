using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class RougeSlash : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rouge Slash");
            Tooltip.SetDefault("Fires a wave of 3 rouge air slashes");
        }

        public override void SetDefaults()
        {
            item.damage = 90;
            item.magic = true;
            item.mana = 30;
            item.width = 28;
            item.height = 32;
            item.useTime = 19;
            item.useAnimation = 19;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 7.5f;
            item.UseSound = SoundID.Item91;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<Projectiles.RougeSlashLarge>();
            item.shootSpeed = 24f;
            item.Calamity().postMoonLordRarity = 12;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<Projectiles.RougeSlashLarge>(), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            Projectile.NewProjectile(position.X, position.Y, speedX * 0.8f, speedY * 0.8f, ModContent.ProjectileType<Projectiles.RougeSlashMedium>(), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            Projectile.NewProjectile(position.X, position.Y, speedX * 0.6f, speedY * 0.6f, ModContent.ProjectileType<Projectiles.RougeSlashSmall>(), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            return false;
        }
    }
}
