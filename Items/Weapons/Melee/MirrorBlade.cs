using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class MirrorBlade : ModItem
    {
        private int baseDamage = 295;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mirror Blade");
            Tooltip.SetDefault("The amount of contact damage an enemy does is added to this weapons' damage\n" +
                "You must hit an enemy with the blade to trigger this effect");
        }

        public override void SetDefaults()
        {
            item.width = 52;
            item.damage = baseDamage;
            item.melee = true;
            item.useAnimation = 12;
            item.useTime = 12;
            item.useTurn = true;
            item.useStyle = 1;
            item.knockBack = 7f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 62;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.shootSpeed = 9f;
            item.shoot = ModContent.ProjectileType<MirrorBlast>();
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            int conDamage = target.damage + baseDamage;
            if (conDamage < baseDamage)
            {
                conDamage = baseDamage;
            }
            if (conDamage > 750)
            {
                conDamage = 750;
            }
            item.damage = conDamage;
        }
    }
}
