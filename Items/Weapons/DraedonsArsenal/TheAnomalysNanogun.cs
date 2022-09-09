using Terraria.DataStructures;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Projectiles.DraedonsArsenal;
using System.Collections.Generic;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.CustomRecipes;
using System;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class TheAnomalysNanogun : ModItem
    {
        public static readonly SoundStyle PlasmaChargeSFX = new("CalamityMod/Sounds/Item/AnomalysNanogunPlasmaCharge");
        public static readonly SoundStyle MPFBShotSFX = new("CalamityMod/Sounds/Item/AnomalysNanogunMPFBShot");
        public static readonly SoundStyle PlasmaShotSFX = new("CalamityMod/Sounds/Item/AnomalysNanogunPlasmaShot");
        public bool PlasmaChargeSelected = true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Anomaly's Nanogun");
            Tooltip.SetDefault("'Welcome to the party, pal'\n" +
                "Left click to charge up 5 rapid-fire plasma beams\n" +
                "Right click to launch 3 fission bombs");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = Item.Calamity();

            SetPlasmaDefaults();
            Item.width = 102;
            Item.height = 44;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<AnomalysNanogunHoldout>();
            Item.UseSound = null;

            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
            modItem.customRarity = CalamityRarity.DarkBlue;
            modItem.donorItem = true;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 250f;
            modItem.ChargePerUse = 0.2f;
            modItem.ChargePerAltUse = 0.125f;
        }

        private void SetPlasmaDefaults()
        {
            Item.damage = 500;
            Item.knockBack = 4.5f;
            Item.useTime = Item.useAnimation = AnomalysNanogunHoldout.PlasmaFireTimer;
            Item.shootSpeed = 5f;
        }

        private void SetMPFBDefaults()
        {
            Item.damage = 250;
            Item.knockBack = 5.5f;
            Item.useTime = Item.useAnimation = AnomalysNanogunHoldout.MPFBFireTimer;
            Item.shootSpeed = 13f;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 4);
        public override Vector2? HoldoutOffset() => new(-30f, 0f);
        public override bool AltFunctionUse(Player player) => true;
        public override void UseItemFrame(Player player)
        {
            // Thank you Mr. IbanPlay (CoralSprout.cs)
            // Calculate the dirction in which the players arms should be pointing at.
            float armPointingDirection = player.itemRotation;
            if (player.direction < 0)
                armPointingDirection += MathHelper.Pi;

            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, armPointingDirection - MathHelper.PiOver2);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armPointingDirection - MathHelper.PiOver2);
        }

        // Stats are set before firing
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 rotationVector = velocity.SafeNormalize(Vector2.UnitX);
            position = player.MountedCenter + rotationVector * AnomalysNanogunHoldout.GunLength;
            float damageRatio = (float)damage / Item.damage;

            // Right click is the MPBF Devastator
            if (player.altFunctionUse == 2)
            {
                // Correct the stats if left click was previously used
                if (!PlasmaChargeSelected)
                    return;

                SetMPFBDefaults();
                PlasmaChargeSelected = false;
            }

            // Left click is the plasma beams
            else
            {
                // Correct the stats if right click was previously used
                if (PlasmaChargeSelected)
                    return;

                SetPlasmaDefaults();
                PlasmaChargeSelected = true;
            }

            velocity = rotationVector * Item.shootSpeed;
            knockback = Item.knockBack;
            damage = (int)(damageRatio * Item.damage);

            player.itemAnimation = Item.useAnimation;
            player.itemTime = Item.useTime;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // The shooting mode is stored inside the holdout projectile
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, player.altFunctionUse, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HeavyLaserRifle>()
                .AddIngredient<CosmiliteBar>(8)
                .AddIngredient<MysteriousCircuitry>(16)
                .AddIngredient<DubiousPlating>(16)
                .AddIngredient<AscendantSpiritEssence>(2)
                .AddCondition(ArsenalTierGatedRecipe.ConstructRecipeCondition(4, out Predicate<Recipe> condition), condition)
                .AddTile<CosmicAnvil>()
                .Register();
        }
    }
}
