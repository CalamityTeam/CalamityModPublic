using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class NastyCholla : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nasty Cholla");
            Tooltip.SetDefault(@"Throws a spiky ball that sticks to everything
Stealth strikes throw five at once");
        }

        public override void SafeSetDefaults()
        {
            item.width = 20;
            item.damage = 20;
            item.noMelee = true;
            item.noUseGraphic = true;
			item.maxStack = 999;
			item.consumable = true;
            item.useAnimation = 18;
            item.useStyle = 1;
            item.useTime = 20;
            item.knockBack = 3f;
            item.UseSound = SoundID.Item1;
            item.height = 18;
            item.value = Item.buyPrice(0, 0, 0, 50);
            item.rare = 0;
            item.shoot = ModContent.ProjectileType<NastyChollaBol>();
            item.shootSpeed = 8f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int spread = 3;
                for (int i = 0; i < 5; i++)
                {
                    Vector2 perturbedspeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(spread));
                    int proj = Projectile.NewProjectile(position.X, position.Y, perturbedspeed.X, perturbedspeed.Y, type, item.damage, item.knockBack, player.whoAmI, 0f, 0f);
                    Main.projectile[proj].Calamity().stealthStrike = true;
                    spread -= 3;
                }
                return false;
            }
            return true;
        }

        /*public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Cactus, 2);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this, 15);
            recipe.AddRecipe();
        }*/
    }
}
