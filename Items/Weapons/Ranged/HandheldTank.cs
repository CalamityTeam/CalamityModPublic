using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class HandheldTank : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public static readonly SoundStyle UseSound = new("CalamityMod/Sounds/Item/TankCannon") { PitchVariance = 0.5f };

        public override void SetDefaults()
        {
            Item.width = 110;
            Item.height = 46;
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 940;
            Item.knockBack = 16f;
            Item.useTime = 71;
            Item.useAnimation = 71;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = UseSound;
            Item.noMelee = true;

            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.Calamity().donorItem = true;

            Item.shoot = ModContent.ProjectileType<HandheldTankShell>();
            Item.shootSpeed = 6f;
            Item.useAmmo = AmmoID.Rocket;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 15;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position+player.DirectionTo(Main.MouseWorld)*48f, velocity, ModContent.ProjectileType<HandheldTankShell>(), damage, knockback, player.whoAmI, 0f, 0f);
            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-33, 0);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Shroomer>().
                AddRecipeGroup("IronBar", 50).
                AddIngredient<DivineGeode>(5).
                AddIngredient(ItemID.TigerSkin).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
