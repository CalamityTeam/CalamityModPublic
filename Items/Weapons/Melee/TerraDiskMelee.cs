using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TerraDiskMelee : ModItem
    {
        public static int BaseDamage = 100;
        public static float Speed = 12f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terra Disk");
            Tooltip.SetDefault(@"Throws a disk that has a chance to generate several disks if enemies are near it
A max of three disks can be active at a time");
        }

        public override void SetDefaults()
        {
            item.width = 46;
            item.height = 46;
            item.damage = BaseDamage;
            item.knockBack = 4f;
            item.useAnimation = 16;
            item.useTime = 16;
            item.autoReuse = true;
            item.noMelee = true;
            item.noUseGraphic = true;

            item.useStyle = 1;
            item.UseSound = SoundID.Item1;

            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;

            item.melee = true;
            item.shoot = ModContent.ProjectileType<TerraDiskProjectile>();
            item.shootSpeed = Speed;
        }

        public override bool CanUseItem(Player player)
        {
			if (player.ownedProjectileCounts[item.shoot] >= 3)
			{
				return false;
			}
			else
			{
				return true;
			}
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            Main.projectile[proj].Calamity().forceMelee = true;
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.SetResult(this);
            recipe.AddIngredient(ModContent.ItemType<SeashellBoomerangMelee>());
            recipe.AddIngredient(ModContent.ItemType<Equanimity>());
            recipe.AddIngredient(ItemID.ThornChakram);
            recipe.AddIngredient(ModContent.ItemType<LivingShard>(), 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.AddRecipe();
        }
    }
}
