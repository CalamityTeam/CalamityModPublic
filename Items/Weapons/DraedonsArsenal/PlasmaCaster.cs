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
    // still awkward that the item called Plasma Rifle is the same class and exact same tier as this item
    public class PlasmaCaster : ModItem
    {
        public const int BaseDamage = 705;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plasma Caster");
            Tooltip.SetDefault("Industrial tool used to fuse metal together with super-heated plasma\n" +
                "Right click for turbo mode");
        }

        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = item.Calamity();

            item.width = 62;
            item.height = 30;
            item.magic = true;
            item.damage = BaseDamage;
            item.knockBack = 7f;
            item.useTime = 45;
            item.useAnimation = 45;
            item.autoReuse = true;
            item.mana = 24;

            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaCasterFire");
            item.noMelee = true;

            item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            item.rare = ItemRarityID.Red;
            modItem.customRarity = CalamityRarity.DraedonRust;

            item.shoot = ModContent.ProjectileType<PlasmaCasterShot>();
            item.shootSpeed = 5f;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 190f;
            modItem.ChargePerUse = 0.32f;
            modItem.ChargePerAltUse = 0.12f; // turbo mode is more energy inefficient
        }

        public override bool AltFunctionUse(Player player) => true;

        public override float UseTimeMultiplier    (Player player)
        {
            if (player.altFunctionUse == 2)
                return 3f;
            return 1f;
        }

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.altFunctionUse == 2)
                mult /= 3f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 velocity = new Vector2(speedX, speedY);
            if (velocity.Length() > 5f)
            {
                velocity.Normalize();
                velocity *= 5f;
            }

            float SpeedX = velocity.X + Main.rand.Next(-3, 4) * 0.05f;
            float SpeedY = velocity.Y + Main.rand.Next(-3, 4) * 0.05f;
            float damageMult = 1f;
            float kbMult = 1f;
            if (player.altFunctionUse == 2)
            {
                SpeedX = velocity.X + Main.rand.Next(-15, 16) * 0.05f;
                SpeedY = velocity.Y + Main.rand.Next(-15, 16) * 0.05f;
                damageMult = 0.3333f;
                kbMult = 3f / 7f;
            }

            Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<PlasmaCasterShot>(), (int)(damage * damageMult), knockBack * kbMult, player.whoAmI, 0f, 0f);
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
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
