using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class StormDragoon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Dragoon");
            Tooltip.SetDefault("Fires a spray of bullets\n" +
            "90% chance to not consume ammo");
        }

        public override void SetDefaults()
        {
            item.damage = 62;
            item.ranged = true;
            item.width = 74;
            item.height = 34;
            item.useTime = 2;
            item.reuseDelay = 10;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3.25f;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.UseSound = SoundID.Item31;
            item.autoReuse = true;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 18f;
            item.useAmmo = AmmoID.Bullet;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-25, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float SpeedX = speedX + (float)Main.rand.Next(-40, 41) * 0.05f;
            float SpeedY = speedY + (float)Main.rand.Next(-40, 41) * 0.05f;
            Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override bool ConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) < 90)
                return false;
            return true;
        }
    }
}
