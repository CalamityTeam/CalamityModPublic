using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class TeslaCannon : ModItem
    {
        private int BaseDamage = 1050;

        public static readonly SoundStyle FireSound = new("CalamityMod/Sounds/Item/TeslaCannonFire");
        
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Tesla Cannon");
            Tooltip.SetDefault("Lightweight energy cannon that blasts an intense electrical beam that explodes\n" +
                "Beams can arc to nearby targets");
        }

        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = Item.Calamity();

            Item.width = 78;
            Item.height = 28;
            Item.DamageType = DamageClass.Magic;
            Item.damage = BaseDamage;
            Item.knockBack = 10f;
            Item.useTime = 90;
            Item.useAnimation = 90;
            Item.autoReuse = true;
            Item.mana = 30;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = FireSound;
            Item.noMelee = true;

            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.rare = ItemRarityID.Purple;
            modItem.customRarity = CalamityRarity.DraedonRust;

            Item.shoot = ModContent.ProjectileType<TeslaCannonShot>();
            Item.shootSpeed = 5f;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 250f;
            modItem.ChargePerUse = 0.9f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (velocity.Length() > 5f)
            {
                velocity.Normalize();
                velocity *= 5f;
            }

            float SpeedX = velocity.X + (float)Main.rand.Next(-1, 2) * 0.02f;
            float SpeedY = velocity.Y + (float)Main.rand.Next(-1, 2) * 0.02f;

            Projectile.NewProjectile(source, position, new Vector2(SpeedX, SpeedY), ModContent.ProjectileType<TeslaCannonShot>(), damage, knockback, player.whoAmI, 0f, 0f);
            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-20, 0);

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 5);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(25).
                AddIngredient<DubiousPlating>(15).
                AddIngredient<CosmiliteBar>(8).
                AddIngredient<AscendantSpiritEssence>(2).
                AddCondition(ArsenalTierGatedRecipe.ConstructRecipeCondition(5, out Predicate<Recipe> condition), condition).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
