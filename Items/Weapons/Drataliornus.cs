using Microsoft.Xna.Framework;
using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items
{
    public class Drataliornus : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Drataliornus");
            Tooltip.SetDefault(@"Fires an escalating stream of fireballs.
Fireballs rain meteors, leave dragon dust trails, and launch additional bolts at max speed.
Taking damage while firing the stream will interrupt it and reduce your wing flight time.
Right click to fire two devastating barrages of five empowered fireballs.
'Just don't get hit'");
        }

        public override void SetDefaults()
        {
            item.damage = 620;
            item.knockBack = 1f;
            item.shootSpeed = 18f;
            item.useStyle = 5;
            item.useAnimation = 10;
            item.useTime = 10;
            item.reuseDelay = 0;
            item.width = 64;
            item.height = 84;
            item.UseSound = SoundID.Item5;
            item.shoot = ModContent.ProjectileType<Projectiles.Drataliornus>();
            item.value = Item.buyPrice(5, 0, 0, 0);
            item.rare = 10;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.ranged = true;
            item.channel = true;
            item.useTurn = false;
            item.useAmmo = AmmoID.Arrow;
            item.autoReuse = true;
            item.Calamity().postMoonLordRarity = 16;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.useAnimation = 24;
                item.useTime = 12;
                item.reuseDelay = 48;
                item.noUseGraphic = false;
            }
            else
            {
                item.useAnimation = 9;
                item.useTime = 9;
                item.reuseDelay = 0;
                item.noUseGraphic = true;
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2) //tsunami
            {
                const float num3 = 0.471238898f;
                const int num4 = 5;
                Vector2 spinningpoint = new Vector2(speedX, speedY);
                spinningpoint.Normalize();
                spinningpoint *= 36f;
                for (int index1 = 0; index1 < num4; ++index1)
                {
                    float num8 = index1 - (num4 - 1) / 2;
                    Vector2 vector2_5 = spinningpoint.RotatedBy(num3 * num8, new Vector2());
                    Projectile.NewProjectile(position.X + vector2_5.X, position.Y + vector2_5.Y, speedX, speedY, ModContent.ProjectileType<DrataliornusFlame>(), damage, knockBack, player.whoAmI, 1f, 0f);
                }
            }
            else
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<Projectiles.Drataliornus>(), 0, 0f, player.whoAmI);
            }

            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(4f, 0f);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BlossomFlux");
            recipe.AddIngredient(null, "DaemonsFlame");
            recipe.AddIngredient(null, "Deathwind");
            recipe.AddIngredient(null, "HeavenlyGale");
            recipe.AddIngredient(null, "DragonsBreath", 2);
            recipe.AddIngredient(null, "ChickenCannon", 2);
            recipe.AddIngredient(null, "AngryChickenStaff", 2);
            recipe.AddIngredient(null, "YharimsGift", 3);
            recipe.AddIngredient(null, "AuricOre", 40);
            recipe.AddIngredient(null, "EffulgentFeather", 60);
            recipe.AddIngredient(null, "DarksunFragment", 30);
            recipe.AddIngredient(null, "NightmareFuel", 30);
            recipe.AddIngredient(null, "EndothermicEnergy", 30);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
