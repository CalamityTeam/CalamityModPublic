using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Items.Weapons
{
    public class GalaxySmasherRogue : CalamityDamageItem
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
            item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 14;
            item.value = Item.buyPrice(1, 80, 0, 0);

            item.shoot = mod.ProjectileType("GalaxySmasherHammerRogue");
            item.shootSpeed = Speed;
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
