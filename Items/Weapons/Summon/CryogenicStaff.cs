using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class CryogenicStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cryogenic Staff");
            Tooltip.SetDefault(@"Summons an animated ice construct to protect you
Fire rate and range increase the longer it targets an enemy");
        }

        public override void SetDefaults()
        {
            item.damage = 50;
            item.mana = 10;
            item.summon = true;
            item.sentry = true;
            item.width = 40;
            item.height = 40;
            item.useTime = item.useAnimation = 25;
            item.useStyle = 1;
            item.noMelee = true;
            item.knockBack = 4f;
            item.value = Item.buyPrice(0, 48, 0, 0);
            item.rare = 6;
            item.UseSound = SoundID.Item78;
            item.shoot = ModContent.ProjectileType<IceSentry>();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                position = Main.MouseWorld;
                speedX = 0;
                speedY = 0;
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
                player.UpdateMaxTurrets();
            }
            return false;
        }
    }
}
