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
    public class StarSwallowerContainmentUnit : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star Swallower Containment Unit");
            Tooltip.SetDefault("Small novelties created to easily transport and fire plasma, strangely popular with humans\n" +
            "Summons a biomechanical frog that vomits plasma onto enemies");
        }

        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = item.Calamity();

            item.shootSpeed = 10f;
            item.damage = 24;
            item.mana = 10;
            item.width = 18;
            item.height = 28;
            item.useTime = item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.noMelee = true;
            item.knockBack = 2.25f;
            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = ItemRarityID.Red;
            modItem.customRarity = CalamityRarity.DraedonRust;
            item.UseSound = SoundID.Item15;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<StarSwallowerSummon>();
            item.shootSpeed = 10f;
            item.summon = true;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 50f;
            modItem.ChargePerUse = 0.8f;
            modItem.ChargePerAltUse = 0f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Point mouseTileCoords = Main.MouseWorld.ToTileCoordinates();
            if (!CalamityUtils.ParanoidTileRetrieval(mouseTileCoords.X, mouseTileCoords.Y).active())
            {
                Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 1);

        public override void AddRecipes()
        {
            ArsenalTierGatedRecipe recipe = new ArsenalTierGatedRecipe(mod, 1);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 8);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 4);
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 4);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 7);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
