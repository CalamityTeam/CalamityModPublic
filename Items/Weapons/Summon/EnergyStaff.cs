using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class EnergyStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Energy Staff");
            Tooltip.SetDefault("Summons a profaned energy turret to fight for you");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 170;
            item.summon = true;
            item.sentry = true;
            item.mana = 25;
            item.width = 66;
            item.height = 68;
            item.useTime = 38;
            item.useAnimation = 38;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<ProfanedEnergy>();
            item.Calamity().postMoonLordRarity = 12;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            position = Main.MouseWorld;
            speedX = 0;
            speedY = 0;
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
            player.UpdateMaxTurrets();
            return false;
        }
    }
}
