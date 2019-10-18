using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Melee
{
    public class YateveoBloom : ModItem
    {
        public static int BaseDamage = 30;
        public static float ShootSpeed = 12f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yateveo Bloom");
            Tooltip.SetDefault("A synthesis of jungle flora\n" +
                "Throws a powerful rose flail\n" +
                "Right click to stab with a flower spear");
        }

        public override void SetDefaults()
        {
            item.width = 42;
            item.height = 62;
            item.damage = BaseDamage;
            item.knockBack = 5f;
            item.useAnimation = 22;
            item.useTime = 22;

            item.noUseGraphic = true;
            item.melee = true;
            item.noMelee = true;
            item.channel = true;
            item.autoReuse = false;
            item.useTurn = true;

            item.useStyle = 5;
            item.UseSound = SoundID.Item1;

            item.rare = 2;
            item.Calamity().postMoonLordRarity = 21;
            item.value = Item.buyPrice(0, 2, 0, 0);

            item.shoot = ModContent.ProjectileType<Projectiles.YateveoBloom>();
            item.shootSpeed = ShootSpeed;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.damage = 20;
                item.channel = false;
                item.useAnimation = 33;
                item.useTime = 33;
                item.shootSpeed = 4.5f;
            }
            else
            {
                item.damage = BaseDamage;
                item.channel = true;
                item.useAnimation = 22;
                item.useTime = 22;
                item.shootSpeed = ShootSpeed;
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<YateveoBloomSpear>(), damage, knockBack, player.whoAmI, 0f, 0f);
            else
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<Projectiles.YateveoBloom>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.RichMahogany, 15);
            recipe.AddIngredient(ItemID.JungleSpores, 12);
            recipe.AddIngredient(ItemID.Stinger, 4);
            recipe.AddIngredient(ItemID.Vine, 2);
            recipe.AddIngredient(ItemID.JungleRose);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
