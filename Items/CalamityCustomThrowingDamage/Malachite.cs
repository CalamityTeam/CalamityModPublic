using Terraria;
using Terraria.ID;

namespace CalamityMod.Items
{
    public class Malachite : CalamityDamageItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Malachite");
            Tooltip.SetDefault("Legendary Drop\n" +
                "Throws a stream of kunai that stick to enemies and explode\n" +
                "Right click to throw a single kunai that pierces, after piercing an enemy it emits a massive explosion on the next enemy hit\n" +
                "Revengeance drop");
        }

        public override void SafeSetDefaults()
        {
            item.width = 26;
            item.damage = 62;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useTime = 10;
            item.useAnimation = 10;
            item.useStyle = 1;
            item.knockBack = 1.25f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 58;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.shoot = ModContent.ProjectileType<Malachite>();
            item.shootSpeed = 10f;
            item.Calamity().rogue = true;
            item.Calamity().postMoonLordRarity = 17;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.useTime = 10;
                item.useAnimation = 10;
                item.UseSound = SoundID.Item109;
            }
            else
            {
                item.useTime = 5;
                item.useAnimation = 5;
                item.UseSound = SoundID.Item1;
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<MalachiteBolt>(), (int)((double)damage * 1.75), knockBack, player.whoAmI, 0f, 0f);
            }
            else
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<Malachite>(), damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }
    }
}
