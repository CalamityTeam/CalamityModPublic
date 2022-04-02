using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class HydraulicVoltCrasher : ModItem
    {
        // This is the amount of charge consumed every frame the holdout projectile is summoned, i.e. the weapon is in use.
        public const float HoldoutChargeUse = 0.002f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hydraulic Volt Crasher");
            Tooltip.SetDefault("Good for both stamping metal plates and instantly fusing them, as well as crushing enemies\n" +
            "An electrically charged jackhammer which shocks all nearby foes on hit");
        }

        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = item.Calamity();

            item.damage = 65;
            item.knockBack = 12f;
            item.useTime = 4;
            item.useAnimation = 16;
            item.hammer = 100;

            item.useStyle = ItemUseStyleID.HoldingOut;
            item.shootSpeed = 46f;
            item.width = 56;
            item.height = 24;
            item.UseSound = SoundID.Item23;

            item.shoot = ModContent.ProjectileType<HydraulicVoltCrasherProjectile>();
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = ItemRarityID.Red;
            modItem.customRarity = CalamityRarity.DraedonRust;

            item.noMelee = true;
            item.noUseGraphic = true;
            item.melee = true;
            item.channel = true;

            modItem.trueMelee = true;
            modItem.UsesCharge = true;
            modItem.MaxCharge = 85f;
            modItem.ChargePerUse = 0f; // This weapon is a holdout. Charge is consumed by the holdout projectile.
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0 && item.Calamity().Charge > 0;

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 2);

        public override void AddRecipes()
        {
            ArsenalTierGatedRecipe recipe = new ArsenalTierGatedRecipe(mod, 2);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 8);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 12);
            recipe.AddRecipeGroup("AnyMythrilBar", 10);
            recipe.AddIngredient(ItemID.SoulofSight, 20);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
