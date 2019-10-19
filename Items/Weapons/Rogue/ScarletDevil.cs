using Terraria;
using Terraria.ModLoader; using CalamityMod.Items.Materials;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class ScarletDevil : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scarlet Devil");
            Tooltip.SetDefault("Throws an ultra high velocity spear, which creates more projectiles that home in\n" +
            "The spear creates a Scarlet Blast upon hitting an enemy\n" +
            "Stealth strikes grant you lifesteal\n" +
            "'Divine Spear \"Spear the Gungnir\"'");
        }

        public override void SafeSetDefaults()
        {
            item.width = 94;
            item.height = 94;
            item.damage = 40000;
            item.crit += 20;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 60;
            item.useStyle = 1;
            item.useTime = 60;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item60;
            item.autoReuse = true;
            item.value = Item.buyPrice(5, 0, 0, 0);
            item.rare = 8;
            item.shoot = ModContent.ProjectileType<ScarletDevilProjectile>();
            item.shootSpeed = 30f;
            item.Calamity().rogue = true;
            item.Calamity().postMoonLordRarity = 16;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ProfanedTrident>());
            recipe.AddIngredient(ModContent.ItemType<BloodstoneCore>(), 15);
            recipe.AddIngredient(ItemID.SoulofNight, 15);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
