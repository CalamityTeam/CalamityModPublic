using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
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
            Item.damage = 740;
            Item.crit = 15;
            Item.knockBack = 16f;
            Item.useTime = Item.useAnimation = 74;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = UseSound;
            Item.noMelee = true;

            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.Calamity().donorItem = true;

            Item.shoot = ModContent.ProjectileType<HandheldTankShell>();
            Item.shootSpeed = 6f;
            Item.useAmmo = AmmoID.Rocket;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position += velocity.SafeNormalize(Vector2.UnitX) * 48f;
            type = Item.shoot;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-33, 0);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.RocketLauncher).
                AddRecipeGroup("IronBar", 50).
                AddIngredient<DivineGeode>(5).
                AddIngredient(ItemID.TigerSkin).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
