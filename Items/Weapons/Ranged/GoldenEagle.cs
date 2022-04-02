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
            item.damage = 85;
            item.ranged = true;
            item.noMelee = true;
            item.width = 46;
            item.height = 30;
            item.useTime = 19;
            item.useAnimation = 19;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 3f;
            item.value = CalamityGlobalItem.Rarity11BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.UseSound = SoundID.Item41;
            item.autoReuse = true;
            item.shoot = ProjectileID.Bullet;
            item.shootSpeed = 20f;
            item.useAmmo = AmmoID.Bullet;
            item.Calamity().canFirePointBlankShots = true;
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
