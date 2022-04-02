using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.DraedonsArsenal;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class Taser : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Taser");
            Tooltip.SetDefault("A slow, simple electric weapon, meant only for low ranking guards\n" +
            "Shoots a hook that attaches to enemies and electrocutes them before returning");
        }

        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = item.Calamity();

            item.width = 50;
            item.height = 26;
            item.ranged = true;
            item.damage = 22;
            item.knockBack = 0f;
            item.useTime = item.useAnimation = 28;
            item.autoReuse = true;

            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaBolt");
            item.noMelee = true;

            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = ItemRarityID.Red;
            modItem.customRarity = CalamityRarity.DraedonRust;

            item.shoot = ModContent.ProjectileType<TaserHook>();
            item.shootSpeed = 15f;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 50f;
            modItem.ChargePerUse = 0.05f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 1);

        public override void AddRecipes()
        {
            ArsenalTierGatedRecipe recipe = new ArsenalTierGatedRecipe(mod, 1);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 7);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 5);
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 4);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 7);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
