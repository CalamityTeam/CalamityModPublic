using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 60;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 60;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item60;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<ScarletDevilProjectile>();
            item.shootSpeed = 30f;
            item.Calamity().rogue = true;

            item.value = CalamityGlobalItem.Rarity16BuyPrice;
            item.Calamity().customRarity = CalamityRarity.ItemSpecific;
            item.Calamity().devItem = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 20;

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
