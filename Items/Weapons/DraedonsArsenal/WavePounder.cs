using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
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
    public class WavePounder : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wave Pounder");
            Tooltip.SetDefault("It utilizes its power to send heavy shockwaves throughout the area, causing agonizing internal damage\n" +
                               "Throws a bomb which explodes into a forceful shockwave\n" +
                               "Stealth strikes emit absurdly powerful shockwaves");
        }

        public override void SafeSetDefaults()
        {
            CalamityGlobalItem modItem = Item.Calamity();

            Item.damage = 75;
            modItem.rogue = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.width = 26;
            Item.height = 44;
            Item.useTime = 56;
            Item.useAnimation = 56;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 0f;

            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ItemRarityID.Red;

            modItem.customRarity = CalamityRarity.DraedonRust;
            Item.UseSound = SoundID.Item1;

            Item.shootSpeed = 16f;
            Item.shoot = ModContent.ProjectileType<WavePounderProjectile>();

            modItem.UsesCharge = true;
            modItem.MaxCharge = 190f;
            modItem.ChargePerUse = 0.5f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 4);

        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 12).
                AddIngredient(ModContent.ItemType<DubiousPlating>(), 18).
                AddIngredient(ModContent.ItemType<UeliaceBar>(), 8).
                AddIngredient(ItemID.LunarBar, 4).
                AddCondition(ArsenalTierGatedRecipe.ConstructRecipeCondition(4, out Predicate<Recipe> condition), condition).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
