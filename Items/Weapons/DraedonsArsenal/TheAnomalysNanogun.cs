using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class TheAnomalysNanogun : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.DraedonsArsenal";
        public static readonly SoundStyle PlasmaChargeSFX = new("CalamityMod/Sounds/Item/AnomalysNanogunPlasmaCharge");
        public static readonly SoundStyle MPFBShotSFX = new("CalamityMod/Sounds/Item/AnomalysNanogunMPFBShot");
        public static readonly SoundStyle PlasmaShotSFX = new("CalamityMod/Sounds/Item/AnomalysNanogunPlasmaShot");
        public bool PlasmaChargeSelected = true;

        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = Item.Calamity();

            Item.damage = 1850;
            Item.knockBack = 4.5f;
            Item.useTime = Item.useAnimation = AnomalysNanogunHoldout.PlasmaFireTimer;
            Item.shootSpeed = 5f;
            Item.width = 102;
            Item.height = 44;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<AnomalysNanogunHoldout>();
            Item.UseSound = null;

            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();
            modItem.donorItem = true;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 250f;
            modItem.ChargePerUse = 0.8f;
            modItem.ChargePerAltUse = 0.375f;
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

        // Adjust based on right or left click
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 rotationVector = velocity.SafeNormalize(Vector2.UnitX);
            position = player.MountedCenter + rotationVector * AnomalysNanogunHoldout.GunLength;
            velocity = rotationVector * 5f;

            // Right click is the MPBF Devastator
            if (player.altFunctionUse == 2)
            {
                damage = (int)(damage * 0.5f);
                knockback *= 0.8f;
                velocity = rotationVector * 13f;
            }
        }

        // The clicks have different use times
        public override float UseSpeedMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return (float)AnomalysNanogunHoldout.PlasmaFireTimer / AnomalysNanogunHoldout.MPFBFireTimer;

            return 1f;
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
                .AddIngredient<MysteriousCircuitry>(20)
                .AddIngredient<DubiousPlating>(20)
                .AddIngredient<CosmiliteBar>(8)
                .AddIngredient<AscendantSpiritEssence>(2)
                .AddCondition(ArsenalTierGatedRecipe.ConstructRecipeCondition(5, out Func<bool> condition), condition)
                .AddTile<CosmicAnvil>()
                .Register();
        }
    }
}
