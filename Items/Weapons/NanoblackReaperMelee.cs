using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class NanoblackReaperMelee : ModItem
    {
        public static int BaseDamage = 800;
        public static float Knockback = 9f;
        public static float Speed = 16f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nanoblack Reaper");
			Tooltip.SetDefault("Unleashes a storm of nanoblack energy blades\nBlades target bosses whenever possible\n'She smothered them in Her hatred'");
        }

        public override void SetDefaults()
        {
            item.width = 78;
			item.height = 64;
            item.melee = true;
            item.damage = BaseDamage;
            item.knockBack = Knockback;
            item.useTime = 6;
            item.useAnimation = 6;
            item.autoReuse = true;
            item.noMelee = true;
            item.noUseGraphic = true;

            item.useStyle = 1;
            item.UseSound = SoundID.Item18;

            item.rare = 10;
            item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 16;
            item.value = Item.buyPrice(5, 0, 0, 0);
            
            item.shoot = mod.ProjectileType("NanoblackMainMelee");
            item.shootSpeed = Speed;
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddTile(null, "DraedonsForge");
            r.AddIngredient(null, "GhoulishGouger");
            r.AddIngredient(null, "SoulHarvester");
            r.AddIngredient(null, "EssenceFlayer");
            r.AddIngredient(null, "ShadowspecBar", 5);
            r.AddIngredient(null, "EndothermicEnergy", 40);
            r.AddIngredient(null, "DarkPlasma", 10);
            r.AddIngredient(ItemID.Nanites, 400);
            r.AddRecipe();
        }
    }
}
