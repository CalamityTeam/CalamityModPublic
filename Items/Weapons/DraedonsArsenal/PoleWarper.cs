using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class PoleWarper : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.DraedonsArsenal";
        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = Item.Calamity();

            Item.shootSpeed = 10f;
            Item.damage = 310;
            Item.mana = 12;
            Item.width = 38;
            Item.height = 24;
            Item.useTime = Item.useAnimation = 9;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 8f;

            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();

            Item.UseSound = SoundID.Item15;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PoleWarperSummon>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 250f;
            modItem.ChargePerUse = 1.25f;
            modItem.ChargePerAltUse = 0f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack)
        {
            Projectile north = Projectile.NewProjectileDirect(source, Main.MouseWorld + Vector2.UnitY * 30f, Vector2.Zero, type, damage, knockBack, player.whoAmI);
            Projectile south = Projectile.NewProjectileDirect(source, Main.MouseWorld - Vector2.UnitY * 30f, Vector2.Zero, type, damage, knockBack, player.whoAmI);
            north.originalDamage = Item.damage;
            south.originalDamage = Item.damage;
            north.ai[1] = 1f;
            south.ai[1] = 0f;

            float magnetCount = 0f;

            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].type == type && Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI)
                {
                    magnetCount++;
                }
            }

            // Adjust the offset of all existing magnets such that they form a psuedo-circle.
            // This offset is used when determining where a magnet should move to relative to its true destination (such as the player or an enemy).
            int magnetIndex = 0;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].type == type && Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI)
                {
                    ((PoleWarperSummon)Main.projectile[i].ModProjectile).Time = 0f;
                    ((PoleWarperSummon)Main.projectile[i].ModProjectile).AngularOffset = MathHelper.TwoPi * magnetIndex / magnetCount;
                    magnetIndex++;
                }
            }

            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 5);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(25).
                AddIngredient<DubiousPlating>(15).
                AddIngredient<CosmiliteBar>(8).
                AddIngredient<AscendantSpiritEssence>(2).
                AddCondition(ArsenalTierGatedRecipe.ConstructRecipeCondition(5, out Func<bool> condition), condition).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
