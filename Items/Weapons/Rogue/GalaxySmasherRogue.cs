using Microsoft.Xna.Framework;
using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items
{
    public class GalaxySmasherRogue : RogueWeapon
    {
        public static int BaseDamage = 855;
        public static float Speed = 18f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galaxy Smasher");
            Tooltip.SetDefault("Explodes and summons death lasers on enemy hits");
        }

        public override void SafeSetDefaults()
        {
            item.width = 86;
            item.height = 72;
            item.damage = BaseDamage;
            item.knockBack = 9f;
            item.useAnimation = 13;
            item.useTime = 13;
            item.autoReuse = true;
            item.noMelee = true;
            item.noUseGraphic = true;

            item.useStyle = 1;
            item.UseSound = SoundID.Item1;

            item.rare = 10;
            item.Calamity().postMoonLordRarity = 14;
            item.value = Item.buyPrice(1, 80, 0, 0);

            item.Calamity().rogue = true;
            item.shoot = ModContent.ProjectileType<GalaxySmasherHammer>();
            item.shootSpeed = Speed;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            Main.projectile[proj].Calamity().forceRogue = true;
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddTile(null, "DraedonsForge");
            r.AddIngredient(null, "StellarContemptRogue");
            r.AddIngredient(null, "CosmiliteBar", 10);
            r.AddIngredient(null, "NightmareFuel", 10);
            r.AddIngredient(null, "EndothermicEnergy", 10);
            r.AddRecipe();
        }
    }
}
