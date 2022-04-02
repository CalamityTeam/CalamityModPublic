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
    public class PulsePistol : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pulse Pistol");
            Tooltip.SetDefault("Fires a pulse that arcs to a new target on enemy hits");
        }

        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = item.Calamity();

            item.width = 62;
            item.height = 22;
            item.magic = true;
            item.damage = 26;
            item.knockBack = 0f;
            item.useTime = item.useAnimation = 20;
            item.autoReuse = true;

            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PulseRifleFire");
            item.noMelee = true;

            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = ItemRarityID.Red;
            modItem.customRarity = CalamityRarity.DraedonRust;

            item.shoot = ModContent.ProjectileType<PulseRifleShot>();
            item.shootSpeed = 5.2f; // This may seem low but the shot has 10 extra updates.

            modItem.UsesCharge = true;
            modItem.MaxCharge = 50f;
            modItem.ChargePerUse = 0.05f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<PulsePistolShot>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(10f, 0f);

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 1);

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
