using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class TeslaCannon : ModItem
    {
        private int BaseDamage = 1050;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tesla Cannon");
            Tooltip.SetDefault("Lightweight energy cannon that blasts an intense electrical beam that explodes\n" +
                "Beams can arc to nearby targets");
        }

        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = item.Calamity();

            item.width = 78;
            item.height = 28;
            item.magic = true;
            item.damage = BaseDamage;
            item.knockBack = 10f;
            item.useTime = 90;
            item.useAnimation = 90;
            item.autoReuse = true;
            item.mana = 30;

            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/TeslaCannonFire");
            item.noMelee = true;

            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.rare = ItemRarityID.Purple;
            modItem.customRarity = CalamityRarity.DraedonRust;

            item.shoot = ModContent.ProjectileType<TeslaCannonShot>();
            item.shootSpeed = 5f;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 250f;
            modItem.ChargePerUse = 0.9f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 velocity = new Vector2(speedX, speedY);
            if (velocity.Length() > 5f)
            {
                velocity.Normalize();
                velocity *= 5f;
            }

            float SpeedX = velocity.X + (float)Main.rand.Next(-1, 2) * 0.02f;
            float SpeedY = velocity.Y + (float)Main.rand.Next(-1, 2) * 0.02f;

            Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<TeslaCannonShot>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-20, 0);

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 5);

        public override void AddRecipes()
        {
            ArsenalTierGatedRecipe recipe = new ArsenalTierGatedRecipe(mod, 5);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 25);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 15);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 8);
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 2);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
