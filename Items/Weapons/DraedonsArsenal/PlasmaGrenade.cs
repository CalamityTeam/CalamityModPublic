using System;
using System.Collections.Generic;
using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class PlasmaGrenade : ModItem
    {
        public static readonly SoundStyle ExplosionSound = new("CalamityMod/Sounds/Item/PlasmaGrenadeExplosion");

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plasma Grenade");
            Tooltip.SetDefault("Each grenade contains a heavily condensed and heated unit of plasma. Use with care\n" +
                               "Throws a grenade that explodes into plasma on collision\n" +
                               "Stealth strikes explode violently on collision into a vaporizing blast");
        }

        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = Item.Calamity();

            Item.width = 22;
            Item.height = 28;
            Item.damage = 1378;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.consumable = false;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = Item.useTime = 27;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            Item.value = Item.sellPrice(silver: 10);
            Item.rare = ItemRarityID.Purple;
            modItem.customRarity = CalamityRarity.DraedonRust;

            Item.shoot = ModContent.ProjectileType<PlasmaGrenadeProjectile>();
            Item.shootSpeed = 14f;
            Item.DamageType = RogueDamageClass.Instance;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 250f;
            modItem.ChargePerUse = 0.5f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
                damage = (int)(damage * 1.05);

            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 5);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(5).
                AddIngredient<DubiousPlating>(5).
                AddIngredient<CosmiliteBar>().
                AddIngredient<AscendantSpiritEssence>().
                AddCondition(ArsenalTierGatedRecipe.ConstructRecipeCondition(5, out Predicate<Recipe> condition), condition).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
