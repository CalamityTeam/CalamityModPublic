using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    [LegacyName("ShadecrystalTome")]
    public class ShadecrystalBarrage : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";

        internal const float ShootSpeed = 2f;
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.Frostburn2];
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.damage = 24;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 40;
            Item.useTime = 7;
            Item.useAnimation = 14;
            Item.reuseDelay = 49;
            Item.useLimitPerAnimation = 3;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item9;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ShadecrystalProjectile>();
            Item.shootSpeed = ShootSpeed;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int projAmt = 6;
            float maxSpread = ShootSpeed * 0.25f;
            Vector2 cachedVelocity = velocity;
            Vector2 newPosition = position + velocity.SafeNormalize(Vector2.UnitY) * 20f;
            for (int index = 0; index < projAmt; index++)
            {
                velocity += new Vector2(Main.rand.NextFloat(-maxSpread, maxSpread), Main.rand.NextFloat(-maxSpread, maxSpread));
                Projectile.NewProjectile(source, newPosition, velocity, type, damage, knockback, player.whoAmI, 0f, MathHelper.Lerp(1.04f, 1.09f, index / (float)(projAmt - 1)));
                velocity = cachedVelocity;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CrystalStorm).
                AddIngredient<CryonicBar>(6).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
