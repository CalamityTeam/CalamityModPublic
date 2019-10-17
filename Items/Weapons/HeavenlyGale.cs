using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class HeavenlyGale : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heavenly Gale");
            Tooltip.SetDefault("Fires a barrage of 5 random exo arrows\n" +
                "Green exo arrows explode into a tornado on death\n" +
                "Blue exo arrows cause a second group of arrows to fire on enemy hits\n" +
                "Orange exo arrows cause explosions on death\n" +
                "Teal exo arrows ignore enemy immunity frames");
        }

        public override void SetDefaults()
        {
            item.damage = 585;
            item.ranged = true;
            item.width = 44;
            item.height = 58;
            item.useTime = 9;
            item.useAnimation = 18;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 4f;
            item.value = Item.buyPrice(2, 50, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = 1;
            item.shootSpeed = 17f;
            item.useAmmo = 40;
            item.Calamity().postMoonLordRarity = 15;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num117 = 0.314159274f;
            int num118 = 5;
            Vector2 vector7 = new Vector2(speedX, speedY);
            vector7.Normalize();
            vector7 *= 40f;
            bool flag11 = Collision.CanHit(vector2, 0, 0, vector2 + vector7, 0, 0);
            for (int num119 = 0; num119 < num118; num119++)
            {
                float num120 = (float)num119 - ((float)num118 - 1f) / 2f;
                Vector2 value9 = vector7.RotatedBy((double)(num117 * num120), default);
                if (!flag11)
                {
                    value9 -= vector7;
                }
                switch (Main.rand.Next(10))
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                        type = mod.ProjectileType("TealExoArrow");
                        break;
                    case 4:
                    case 5:
                    case 6:
                        type = mod.ProjectileType("OrangeExoArrow");
                        break;
                    case 7:
                    case 8:
                        type = mod.ProjectileType("BlueExoArrow");
                        break;
                    case 9:
                        type = mod.ProjectileType("GreenExoArrow");
                        break;
                }
                Projectile.NewProjectile(vector2.X + value9.X, vector2.Y + value9.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "Alluvion");
            recipe.AddIngredient(null, "AstrealDefeat");
            recipe.AddIngredient(null, "ClockworkBow");
            recipe.AddIngredient(null, "FlarewingBow");
            recipe.AddIngredient(null, "Phangasm");
            recipe.AddIngredient(null, "PlanetaryAnnihilation");
            recipe.AddIngredient(null, "TheBallista");
            recipe.AddIngredient(null, "NightmareFuel", 5);
            recipe.AddIngredient(null, "EndothermicEnergy", 5);
            recipe.AddIngredient(null, "CosmiliteBar", 5);
            recipe.AddIngredient(null, "DarksunFragment", 5);
            recipe.AddIngredient(null, "HellcasterFragment", 3);
            recipe.AddIngredient(null, "Phantoplasm", 5);
            recipe.AddIngredient(null, "AuricOre", 25);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
