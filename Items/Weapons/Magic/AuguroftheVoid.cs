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
    [LegacyName("AuguroftheElements")]
    public class AuguroftheVoid : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<ElementalMix>()];
        }
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.damage = 131;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 14;
            Item.useTime = 2;
            Item.useAnimation = 10;
            Item.reuseDelay = 5;
            Item.useLimitPerAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5.5f;
            Item.value = CalamityGlobalItem.RarityPurpleBuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item103;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AugurTentacle>();
            Item.shootSpeed = 30f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 spreadVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(18f)) * Main.rand.NextFloat(0.8f, 1.2f);
            float tentacleYDirection = Main.rand.NextFloat(0.01f, 0.05f);
            if (Main.rand.NextBool())
                tentacleYDirection *= -1f;
            float tentacleXDirection = Main.rand.NextFloat(0.01f, 0.05f);
            if (Main.rand.NextBool())
                tentacleXDirection *= -1f;

            Projectile.NewProjectile(source, position, spreadVelocity, type, damage, knockback, Main.myPlayer, tentacleXDirection, tentacleYDirection);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ShadowFlameHexDoll).
                AddIngredient<EldritchTome>().
                AddIngredient(ItemID.LunarBar, 5).
                AddIngredient<LifeAlloy>(5).
                AddIngredient(ItemID.FragmentNebula, 5).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
