using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using CalamityMod.Rarities;
using System;
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
            SacrificeTotal = 1;
            DisplayName.SetDefault("Hydraulic Volt Crasher");
            Tooltip.SetDefault("Good for both stamping metal plates and instantly fusing them, as well as crushing enemies\n" +
            "An electrically charged jackhammer which shocks all nearby foes on hit");
        }

        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = Item.Calamity();

            Item.damage = 65;
            Item.knockBack = 12f;
            Item.useTime = 4;
            Item.useAnimation = 16;
            Item.hammer = 100;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shootSpeed = 46f;
            Item.width = 56;
            Item.height = 24;
            Item.UseSound = SoundID.Item23;

            Item.shoot = ModContent.ProjectileType<HydraulicVoltCrasherProjectile>();
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ModContent.RarityType<DarkOrange>();

            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.DamageType = TrueMeleeNoSpeedDamageClass.Instance;
            Item.channel = true;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 85f;
            modItem.ChargePerUse = 0f; // This weapon is a holdout. Charge is consumed by the holdout projectile.
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0 && Item.Calamity().Charge > 0;

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 2);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(8).
                AddIngredient<DubiousPlating>(12).
                AddRecipeGroup("AnyMythrilBar", 10).
                AddIngredient(ItemID.SoulofSight, 20).
                AddCondition(ArsenalTierGatedRecipe.ConstructRecipeCondition(2, out Predicate<Recipe> condition), condition).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
