using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
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
    public class PulseTurretRemote : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pulse Turret Remote");
            Tooltip.SetDefault("A device used to defend against the weaker, less cognizant rogue creations of Draedon\n" +
                               "Summons a pulse turret which eradicates nearby foes with focused energy blasts\n" +
                               "Only one pulse turret may exist at a time");
        }

        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = Item.Calamity();

            Item.width = 28;
            Item.height = 26;
            Item.DamageType = DamageClass.Summon;
            Item.sentry = true;
            Item.damage = 100;
            Item.knockBack = 0f;
            Item.mana = 10;
            Item.useTime = Item.useAnimation = 24;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item15;
            Item.noMelee = true;

            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Red;
            modItem.customRarity = CalamityRarity.DraedonRust;

            Item.shoot = ModContent.ProjectileType<PulseTurret>();
            Item.shootSpeed = 1f;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 135f;
            modItem.ChargePerUse = 1f;
            modItem.ChargePerAltUse = 0f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            CalamityUtils.OnlyOneSentry(player, type);
            Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
            player.UpdateMaxTurrets();
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 3);

        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 12).
                AddIngredient(ModContent.ItemType<DubiousPlating>(), 18).
                AddIngredient(ModContent.ItemType<BarofLife>(), 5).
                AddCondition(ArsenalTierGatedRecipe.ConstructRecipeCondition(3, out Predicate<Recipe> condition), condition).
                AddIngredient(ModContent.ItemType<InfectedArmorPlating>(), 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
