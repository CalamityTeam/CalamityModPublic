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
    public class SnakeEyes : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Snake Eyes");
            Tooltip.SetDefault("Surveillance drones equipped with a strong electric field which can be directed at enemies\n" +
            "Summons a mechanical watcher that zaps and flies around enemies");
        }

        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = item.Calamity();

            item.shootSpeed = 10f;
            item.damage = 36;
            item.mana = 12;
            item.width = 38;
            item.height = 24;
            item.useTime = item.useAnimation = 14;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.noMelee = true;
            item.knockBack = 3f;

            item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            item.rare = ItemRarityID.Red;
            modItem.customRarity = CalamityRarity.DraedonRust;

            item.UseSound = SoundID.Item15;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SnakeEyesSummon>();
            item.shootSpeed = 10f;
            item.summon = true;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 190f;
            modItem.ChargePerUse = 1f;
            modItem.ChargePerAltUse = 0f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI);
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 4);

        public override void AddRecipes()
        {
            ArsenalTierGatedRecipe recipe = new ArsenalTierGatedRecipe(mod, 4);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 18);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 12);
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 8);
            recipe.AddIngredient(ItemID.LunarBar, 4);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
