using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class StarSwallowerContainmentUnit : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.DraedonsArsenal";
        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = Item.Calamity();

            Item.shootSpeed = 10f;
            Item.damage = 24;
            Item.mana = 10;
            Item.width = 18;
            Item.height = 28;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 2.25f;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item15;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<StarSwallowerSummon>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 50f;
            modItem.ChargePerUse = 0.8f;
            modItem.ChargePerAltUse = 0f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Point mouseTileCoords = Main.MouseWorld.ToTileCoordinates();
            if (!CalamityUtils.ParanoidTileRetrieval(mouseTileCoords.X, mouseTileCoords.Y).HasTile)
            {
                int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;
            }
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 1);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(8).
                AddIngredient<DubiousPlating>(4).
                AddIngredient<AerialiteBar>(4).
                AddIngredient<SeaPrism>(7).
                AddCondition(ArsenalTierGatedRecipe.ConstructRecipeCondition(1, out Func<bool> condition), condition).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
