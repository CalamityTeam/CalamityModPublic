using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class WebBall : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Web Ball");
            Tooltip.SetDefault(@"Throws a web-covered ball that covers enemies in cobwebs to slow them down
Stealth strikes slow enemies down longer");
        }

        public override void SafeSetDefaults()
        {
            item.width = 20;
            item.damage = 8;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.maxStack = 999;
            item.consumable = true;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 20;
            item.knockBack = 3f;
            item.UseSound = SoundID.Item1;
            item.height = 18;
            item.value = Item.buyPrice(0, 0, 0, 30);
            item.rare = ItemRarityID.White;
            item.shoot = ModContent.ProjectileType<WebBallBol>();
            item.shootSpeed = 6.5f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int proj = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
                if (proj.WithinBounds(Main.maxProjectiles))
                    Main.projectile[proj].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Cobweb, 5);
            recipe.SetResult(this, 20);
            recipe.AddRecipe();
        }
    }
}
