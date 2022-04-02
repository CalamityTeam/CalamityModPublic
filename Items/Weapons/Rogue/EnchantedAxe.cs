using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class EnchantedAxe : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Enchanted Axe");
            Tooltip.SetDefault("Throws a high velocity axe that returns to you after travelling a short distance or hitting a wall\n" +
                               "At the furthest point from the player, a magical axe that travels through walls will be cast towards the nearest enemy\n" +
                               "Stealth strikes make the axe fly further, throwing out many other magical axes in all directions");
        }

        public override void SafeSetDefaults()
        {
            item.width = 40;
            item.damage = 20;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 15;
            item.knockBack = 1f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 36;
            item.maxStack = 1;
            item.value = 1000;
            item.rare = ItemRarityID.Orange;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.shoot = ModContent.ProjectileType<EnchantedAxeProj>();
            item.shootSpeed = 30f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 1f);
                if (p.WithinBounds(Main.maxProjectiles))
                    Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<IronFrancisca>(), 100);
            recipe.AddIngredient(ItemID.FallenStar, 5);
            recipe.AddIngredient(ModContent.ItemType<VictoryShard>(), 10);
            recipe.AddIngredient(ItemID.Bone, 30);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<LeadTomahawk>(), 100);
            recipe.AddIngredient(ItemID.FallenStar, 5);
            recipe.AddIngredient(ModContent.ItemType<VictoryShard>(), 10);
            recipe.AddIngredient(ItemID.Bone, 30);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}
