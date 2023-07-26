using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    // still awkward that the item called Plasma Rifle is the same class and exact same tier as this item
    public class PlasmaCaster : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.DraedonsArsenal";
        public static readonly SoundStyle FireSound = new("CalamityMod/Sounds/Item/PlasmaCasterFire");

        public const int BaseDamage = 705;
        public override void SetStaticDefaults()
        {
           
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = Item.Calamity();

            Item.width = 62;
            Item.height = 30;
            Item.DamageType = DamageClass.Magic;
            Item.damage = BaseDamage;
            Item.knockBack = 7f;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.autoReuse = true;
            Item.mana = 24;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = FireSound;
            Item.noMelee = true;

            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();

            Item.shoot = ModContent.ProjectileType<PlasmaCasterShot>();
            Item.shootSpeed = 5f;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 190f;
            modItem.ChargePerUse = 0.32f;
            modItem.ChargePerAltUse = 0.12f; // turbo mode is more energy inefficient
        }

        public override bool AltFunctionUse(Player player) => true;

        public override float UseSpeedMultiplier(Player player)
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

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
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

            Projectile.NewProjectile(source, position, new Vector2(SpeedX, SpeedY), ModContent.ProjectileType<PlasmaCasterShot>(), (int)(damage * damageMult), knockback * kbMult, player.whoAmI, 0f, 0f);
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 4);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(18).
                AddIngredient<DubiousPlating>(12).
                AddIngredient<UelibloomBar>(8).
                AddIngredient(ItemID.LunarBar, 4).
                AddCondition(ArsenalTierGatedRecipe.ConstructRecipeCondition(4, out Func<bool> condition), condition).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
