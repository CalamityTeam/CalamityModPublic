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
            CalamityGlobalItem modItem = Item.Calamity();

            Item.width = 50;
            Item.height = 26;
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 22;
            Item.knockBack = 0f;
            Item.useTime = Item.useAnimation = 28;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = Mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaBolt");
            Item.noMelee = true;

            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Red;
            modItem.customRarity = CalamityRarity.DraedonRust;

            Item.shoot = ModContent.ProjectileType<TaserHook>();
            Item.shootSpeed = 15f;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 50f;
            modItem.ChargePerUse = 0.05f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 1);

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 7).AddIngredient(ModContent.ItemType<DubiousPlating>(), 5).AddIngredient(ModContent.ItemType<AerialiteBar>(), 4).AddIngredient(ModContent.ItemType<SeaPrism>(), 7).AddTile(TileID.Anvils).Register();
        }
    }
}
