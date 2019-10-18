using Microsoft.Xna.Framework;
using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class BloodsoakedCrasher : RogueWeapon //This weapon has been coded by Achilles|Termi|Ben
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodsoaked Crasher");
            Tooltip.SetDefault("Slows down when hitting an enemy. Speeds up otherwise\n" +
            "Heals on enemy hits\n" +
            "Stealth strikes spawn homing blood on enemy hits");
        }

        public override void SafeSetDefaults()
        {
            item.width = 66;
            item.damage = 300;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 18;
            item.useStyle = 1;
            item.useTime = 18;
            item.knockBack = 3f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 64;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.Calamity().postMoonLordRarity = 13;
            item.shoot = ModContent.ProjectileType<BloodsoakedCrashax>();
            item.shootSpeed = 15f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int p = Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<BloodsoakedCrashax>(), damage, knockBack, player.whoAmI, 0f, 1f);
                Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod); //post-Prov rogue weapon
            recipe.AddIngredient(ModContent.ItemType<CrushsawCrasher>(), 1);
            recipe.AddIngredient(ModContent.ItemType<BloodstoneCore>(), 12);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
