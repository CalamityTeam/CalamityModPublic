using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class GalvanizingGlaive : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galvanizing Glaive");
            Tooltip.SetDefault("Its use as a tool is to quickly separate a single object into two\n" +
                "That is also its use as a weapon\n" +
                "Swings a spear which envelops struck foes in an energy field\n" +
                "When done swinging, the spear discharges an extra pulse of energy");
        }

        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = item.Calamity();

            item.width = 56;
            item.height = 52;
            item.damage = 100;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.melee = true;
            item.useAnimation = 21;
            item.useTime = 21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 9f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;

            item.rare = ItemRarityID.Red;
            modItem.customRarity = CalamityRarity.DraedonRust;
            item.value = CalamityGlobalItem.Rarity8BuyPrice;

            item.shoot = ModContent.ProjectileType<GalvanizingGlaiveProjectile>();
            item.shootSpeed = 21f;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 135f;
            modItem.ChargePerUse = 0.075f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 3);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float swingOffset = Main.rand.NextFloat(0.5f, 1f) * item.shootSpeed * 1.6f * player.direction;
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, swingOffset);
            return false;
        }

        public override void AddRecipes()
        {
            ArsenalTierGatedRecipe recipe = new ArsenalTierGatedRecipe(mod, 3);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 12);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 18);
            recipe.AddIngredient(ModContent.ItemType<BarofLife>(), 5);
            recipe.AddIngredient(ModContent.ItemType<InfectedArmorPlating>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
