using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class HydraulicVoltCrasher : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hydraulic Volt Crasher");
            Tooltip.SetDefault("Good for both stamping metal plates and instantly fusing them, as well as crushing enemies.\n" +
			"An electrically charged jackhammer which shocks all nearby foes on hit");
        }

        public override void SetDefaults()
        {
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useAnimation = 16;
            item.useTime = 4;
            item.shootSpeed = 46f;
            item.knockBack = 12f;
            item.width = 56;
            item.height = 24;
            item.damage = 70;
            item.hammer = 230;
            item.UseSound = SoundID.Item23;

            item.shoot = ModContent.ProjectileType<HydraulicVoltCrasherProjectile>();
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.DraedonRust;

            item.noMelee = true;
            item.noUseGraphic = true;
            item.melee = true;
            item.channel = true;

            item.Calamity().Chargeable = true;
            item.Calamity().ChargeMax = 85;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 8);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 12);
            recipe.AddIngredient(ItemID.HallowedBar, 10);
            recipe.AddIngredient(ItemID.SoulofSight, 20);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
