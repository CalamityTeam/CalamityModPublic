using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.CalamityCustomThrowingDamage.Patreon
{
    public class DeepSeaDumbbell : CalamityDamageItem
    {
        private int BaseDamage = 900;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deep Sea Dumbbell");
            Tooltip.SetDefault("Throws a dumbbell that bounces and flings weights with each bounce\n" +
                "Right click to flex with it");
        }

        public override void SafeSetDefaults()
        {
            item.width = 38;
            item.damage = BaseDamage;
            item.crit -= 2;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 25;
            item.useStyle = 1;
            item.useTime = 25;
            item.knockBack = 16f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 24;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.shoot = mod.ProjectileType("DeepSeaDumbbell1");
            item.shootSpeed = 20f;
            item.Calamity().rogue = true;
            item.Calamity().postMoonLordRarity = 21;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.useStyle = 4;
                item.useAnimation = 45;
                item.useTime = 45;
                item.noMelee = false;
                item.noUseGraphic = false;
                item.autoReuse = false;
                item.damage = BaseDamage * 25;
                item.UseSound = SoundID.Item1;
            }
            else
            {
                item.useStyle = 1;
                item.useAnimation = 25;
                item.useTime = 25;
                item.noMelee = true;
                item.noUseGraphic = true;
                item.autoReuse = true;
                item.damage = BaseDamage;
                item.UseSound = SoundID.Item1;
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
                return false;

            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, (int)((float)BaseDamage * player.Calamity().throwingDamage), knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
