using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Items.Weapons.Magic
{
    public class GhastlyVisage : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ghastly Visage");
            Tooltip.SetDefault("Fires homing ghast energy that explodes");
        }

        public override void SetDefaults()
        {
            item.damage = 92;
            item.magic = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.mana = 20;
            item.width = 78;
            item.height = 70;
            item.useTime = 27;
            item.useAnimation = 27;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.shootSpeed = 9f;
            item.shoot = ModContent.ProjectileType<GhastlyVisageProj>();
            item.Calamity().postMoonLordRarity = 13;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<Projectiles.GhastlyVisage>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
