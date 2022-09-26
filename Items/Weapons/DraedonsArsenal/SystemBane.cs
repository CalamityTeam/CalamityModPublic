using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.DraedonsArsenal;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class SystemBane : RogueWeapon
    {
        public const int MaxDeployedProjectiles = 5;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("System Bane");
            Tooltip.SetDefault("Can be used to quickly send out an electromagnetic blast\n" +
                               "Hurls an unstable device which sticks to the ground and shocks nearby enemies with lightning\n" +
                               "Stealth strikes make the device emit a large, damaging EMP field");
        }

        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = Item.Calamity();

            Item.damage = 45;
            Item.DamageType = RogueDamageClass.Instance;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.width = 42;
            Item.height = 36;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ModContent.RarityType<DarkOrange>();
            Item.UseSound = SoundID.Item1;

            Item.shootSpeed = 16f;
            Item.shoot = ModContent.ProjectileType<SystemBaneProjectile>();

            modItem.UsesCharge = true;
            modItem.MaxCharge = 135f;
            modItem.ChargePerUse = 0.085f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < MaxDeployedProjectiles;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 3);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(20).
                AddIngredient<DubiousPlating>(10).
                AddIngredient<InfectedArmorPlating>(10).
                AddIngredient<LifeAlloy>(5).
                AddCondition(ArsenalTierGatedRecipe.ConstructRecipeCondition(3, out Predicate<Recipe> condition), condition).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
