using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class GaussDagger : ModItem
    {
        public const int HitsRequiredForFlux = 2;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gauss Dagger");
            Tooltip.SetDefault("Slicing foes, it causes a flux of energy to form on the area tearing at them with turbulent forces\n" +
            "Repeat strikes envelop foes in magnetic flux");
        }
        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = item.Calamity();

            item.damage = 25;
            item.melee = true;
            item.width = 26;
            item.height = 26;
            item.scale = 1.5f;
            item.useTime = 24;
            item.useAnimation = 24;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.knockBack = 7f;

            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = ItemRarityID.Red;
            modItem.customRarity = CalamityRarity.DraedonRust;

            item.UseSound = SoundID.Item1;
            item.autoReuse = true;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 50f;
            modItem.ChargePerUse = 0.05f;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 1);

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            target.Calamity().GaussFluxTimer += 50;
            if (target.Calamity().GaussFluxTimer >= 30 * HitsRequiredForFlux)
            {
                target.Calamity().GaussFluxTimer = 0;
                if (player.whoAmI == Main.myPlayer)
                {
                    if (crit)
                        damage /= 2;

                    Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<GaussFlux>(), damage, 0f, player.whoAmI, 0f, target.whoAmI);
                }
            }
        }

        public override void AddRecipes()
        {
            ArsenalTierGatedRecipe recipe = new ArsenalTierGatedRecipe(mod, 1);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 5);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 7);
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 4);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 7);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
