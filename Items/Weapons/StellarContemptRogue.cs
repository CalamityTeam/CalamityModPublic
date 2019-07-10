using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Items.Weapons
{
    public class StellarContemptRogue : CalamityDamageItem
    {
        public static int BaseDamage = 575;
        public static float Speed = 18f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stellar Contempt");
			Tooltip.SetDefault("Lunar flares rain down on enemy hits");
        }

        public override void SafeSetDefaults()
        {
            item.width = 66;
			item.height = 64;
            item.damage = BaseDamage;
            item.knockBack = 9f;
            item.useTime = 13;
            item.useAnimation = 13;
            item.autoReuse = true;
            item.noMelee = true;
            item.noUseGraphic = true;

            item.useStyle = 1;
            item.UseSound = SoundID.Item1;

            item.rare = 10;
            item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 12;
            item.value = Item.buyPrice(1, 20, 0, 0);
            
            item.shoot = mod.ProjectileType("StellarContemptHammerRogue");
            item.shootSpeed = Speed;
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddIngredient(null, "TruePaladinsHammer");
            r.AddIngredient(ItemID.LunarBar, 10);
            r.AddIngredient(ItemID.FragmentSolar, 10);
            r.AddIngredient(ItemID.FragmentNebula, 10);
            r.AddRecipe();
        }
    }
}
