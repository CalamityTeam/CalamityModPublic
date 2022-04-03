using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class GoldenEagle : ModItem
    {
        private const float Spread = 0.0425f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Golden Eagle");
            Tooltip.SetDefault("Fires 5 bullets at once");
        }

        public override void SetDefaults()
        {
            Item.damage = 85;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.width = 46;
            Item.height = 30;
            Item.useTime = 19;
            Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3f;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item41;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 20f;
            Item.useAmmo = AmmoID.Bullet;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 velocity = new Vector2(speedX, speedY);

            // Fire extra bullets to the left and right
            for (int i = 0; i < 2; i++)
            {
                Projectile.NewProjectile(position, velocity.RotatedBy(-Spread * (i + 1)), type, damage, knockBack, player.whoAmI);
                Projectile.NewProjectile(position, velocity.RotatedBy(+Spread * (i + 1)), type, damage, knockBack, player.whoAmI);
            }

            return true;
        }
    }
}
