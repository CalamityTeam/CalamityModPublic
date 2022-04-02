using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class EpidemicShredder : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Epidemic Shredder");
            Tooltip.SetDefault("Contrary to its name, it will probably cause an epidemic if used incorrectly\n" +
                               "Throws a plagued boomerang that releases plague seekers when it hits tiles or enemies\n" +
                               "Stealth strikes cause the boomerang to release plague seekers constantly as it travels");
        }

        public override void SafeSetDefaults()
        {
            item.width = 34;
            item.height = 34;
            item.damage = 80;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 16;
            item.useTime = 16;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 4.5f;
            item.UseSound = SoundID.Item1;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.shoot = ModContent.ProjectileType<EpidemicShredderProjectile>();
            item.shootSpeed = 18f;
            item.Calamity().rogue = true;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 velocity = new Vector2(speedX, speedY);
            float strikeValue = player.Calamity().StealthStrikeAvailable().ToInt(); //0 if false, 1 if true
            int projectileIndex = Projectile.NewProjectile(position, velocity, ModContent.ProjectileType<EpidemicShredderProjectile>(), damage, knockBack, player.whoAmI, ai1: strikeValue);
            if (player.Calamity().StealthStrikeAvailable() && projectileIndex.WithinBounds(Main.maxProjectiles))
            {
                Main.projectile[projectileIndex].Calamity().stealthStrike = strikeValue == 1f;
            }
            return false;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.ChlorophyteBar, 20);
            recipe.AddIngredient(ItemID.Nanites, 150);
            recipe.AddIngredient(ModContent.ItemType<PlagueCellCluster>(), 15);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
