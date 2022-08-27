using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Sounds;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class GaussPistol : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Gauss Pistol");
            Tooltip.SetDefault("A simple pistol that utilizes magic power; a weapon for the more magically adept\n" +
            "Fires a devastating high velocity blast with extreme knockback");
        }

        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = Item.Calamity();

            Item.width = 40;
            Item.height = 22;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 6;
            Item.damage = 110;
            Item.knockBack = 11f;
            Item.useTime = Item.useAnimation = 20;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = CommonCalamitySounds.GaussWeaponFire;
            Item.noMelee = true;

            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Red;
            modItem.customRarity = CalamityRarity.DraedonRust;

            Item.shoot = ModContent.ProjectileType<GaussPistolShot>();
            Item.shootSpeed = 14f;

            modItem.MaxCharge = 85f;
            modItem.UsesCharge = true;
            modItem.ChargePerUse = 0.05f;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 2);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(12).
                AddIngredient<DubiousPlating>(8).
                AddRecipeGroup("AnyMythrilBar", 10).
                AddIngredient(ItemID.SoulofMight, 20).
                AddCondition(ArsenalTierGatedRecipe.ConstructRecipeCondition(2, out Predicate<Recipe> condition), condition).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
