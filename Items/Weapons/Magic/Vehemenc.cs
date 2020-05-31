using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
	public class Vehemenc : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vehemence");
            Tooltip.SetDefault("Casts an intense energy blast\n" +
                               "Does far more damage the more HP an enemy has left\n" +
                               "Max damage is capped at 1,000,000\n" +
                               "If an enemy has full HP it will inflict several long-lasting debuffs\n" +
                               "Revengeance drop");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 7;
            item.magic = true;
            item.mana = 590;
            item.width = 44;
            item.height = 44;
            item.useTime = 50;
            item.useAnimation = 50;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5.75f;
            item.value = Item.buyPrice(1, 0, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item73;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<Vehemence>();
            item.shootSpeed = 16f;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(25, 25);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<Vehemence>(), damage, knockBack, player.whoAmI, 0f, 0f);
            player.AddBuff(BuffID.ManaSickness, 600, true);
            return false;
        }
    }
}
