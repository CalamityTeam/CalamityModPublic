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
    public class GaussRifle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gauss Rifle");
            Tooltip.SetDefault("A large and slow weapon, the concussive force of its projectiles do well in clearing large groups\n" +
            "Fires a devastating high velocity blast with absurd knockback");
        }

        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = item.Calamity();

            item.width = 112;
            item.height = 36;
            item.ranged = true;
            item.damage = 150;
            item.knockBack = 30f;
            item.useTime = item.useAnimation = 32;
            item.autoReuse = true;

            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/GaussWeaponFire");
            item.noMelee = true;

            item.value = CalamityGlobalItem.Rarity8BuyPrice;
            item.rare = ItemRarityID.Red;
            modItem.customRarity = CalamityRarity.DraedonRust;

            item.shoot = ModContent.ProjectileType<GaussRifleBlast>();
            item.shootSpeed = 27f;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 135f;
            modItem.ChargePerUse = 0.1125f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<GaussRifleBlast>(), damage, knockBack, player.whoAmI);
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 3);

        public override void AddRecipes()
        {
            ArsenalTierGatedRecipe recipe = new ArsenalTierGatedRecipe(mod, 3);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 18);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 12);
            recipe.AddIngredient(ModContent.ItemType<BarofLife>(), 5);
            recipe.AddIngredient(ModContent.ItemType<InfectedArmorPlating>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
