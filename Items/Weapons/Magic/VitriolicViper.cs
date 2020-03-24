using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class VitriolicViper : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vitriolic Viper");
            Tooltip.SetDefault("Releases a volley of venomous fangs and spit");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 150;
            item.magic = true;
            item.mana = 15;
            item.width = 60;
            item.height = 62;
            item.useTime = item.useAnimation = 16;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.Calamity().postMoonLordRarity = 13;
            item.rare = 10;
            item.UseSound = SoundID.Item46;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<VitriolicViperSpit>();
            item.shootSpeed = 15f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int i = -4; i <= 4; i += 1)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(i));
                Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X * 1.65f, perturbedSpeed.Y * 1.65f, type, (int)(damage * 0.7f), knockBack * 0.7f, player.whoAmI, 0f, 0f);
            }
            for (int j = -2; j <= 2; j += 2)
            {
                Vector2 perturbedSpeed2 = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(j));
                Projectile.NewProjectile(position.X, position.Y, perturbedSpeed2.X, perturbedSpeed2.Y, ModContent.ProjectileType<VitriolicViperFang>(), damage, knockBack, player.whoAmI, 0f, 0f);
            }
            /*Vector2 velocity = new Vector2(speedX, speedY);
            float spitAngleMax = MathHelper.ToRadians(6f + Main.rand.NextFloat(-2f, 2f));
            float fangAngleMax = MathHelper.ToRadians(3f + Main.rand.NextFloat(-1f, 1f));
            for (int i = 0; i < 9; i++)
            {
                float angle = MathHelper.Lerp(i / 9f, -spitAngleMax, spitAngleMax);
                Projectile.NewProjectile(position, velocity.RotatedBy(angle) * 1.65f, type, (int)(damage * 0.7f), knockBack, player.whoAmI);
            }
            for (int i = 0; i < 5; i++)
            {
                float angle = MathHelper.Lerp(i / 5f, -fangAngleMax, fangAngleMax);
                Projectile.NewProjectile(position, velocity.RotatedBy(angle), 
                    ModContent.ProjectileType<VitriolicViperFang>(), damage, knockBack, player.whoAmI);
            }*/
            return false;
        }
    }
}
