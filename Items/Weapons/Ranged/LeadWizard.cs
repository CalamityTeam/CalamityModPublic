using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class LeadWizard : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lead Wizard");
            Tooltip.SetDefault("Something's not right...\n" +
                "33% chance to not consume ammo");
        }

        public override void SetDefaults()
        {
            item.damage = 70;
            item.crit += 30;
            item.ranged = true;
            item.width = 66;
            item.height = 34;
            item.useTime = 2;
            item.reuseDelay = 10;
            item.useAnimation = 6;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.UseSound = SoundID.Item31;
            item.autoReuse = true;
            item.shoot = 10;
            item.shootSpeed = 20f;
            item.useAmmo = 97;
            item.Calamity().postMoonLordRarity = 22;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float rotation = MathHelper.ToRadians(6);
            for (int i = 0; i < 2; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i == 1 ? 0 : 2));
                Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, 242, damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override bool ConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) < 33)
                return false;
            return true;
        }
    }
}
