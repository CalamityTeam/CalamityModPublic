using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace CalamityMod.Items.Weapons.Magic
{
    public class PrimordialAncient : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";

        public static float BuffDamageReductionBoost = 0.08f;
        public static float BuffDamageBoost = 0.15f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(BuffDamageReductionBoost.ToPercent(), BuffDamageBoost.ToPercent());

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 56;
            Item.damage = 3825;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 104;
            Item.useTime = Item.useAnimation = 72;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 14;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.UseSound = new SoundStyle("CalamityMod/Sounds/Item/MagicRockSound") with { Volume = 0.4f, Pitch = -0.1f };
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PrimordialAncientProjectile>();
            Item.shootSpeed = 8f;
            Item.rare = ModContent.RarityType<CosmicPurple>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.velocity.Length() <= 16)
                player.velocity += -velocity.SafeNormalize(Vector2.UnitX) * 7f;

            bool MaxMana = player.statMana >= (player.statManaMax2 - ((int)(Item.mana * player.manaCost))) && !player.HasBuff(BuffID.ManaSickness);
            for (int i = -2; i <= 2; i++)
            {
                Vector2 vel = velocity.RotatedBy(0.1f * i) * MathHelper.Lerp(3 - Math.Abs(i), 1, 0.7f);
                Projectile dust = Projectile.NewProjectileDirect(source, position, vel, type, damage, knockback, player.whoAmI, 0f, i == 0 ? 1 : 0, MaxMana ? 1f : 0f);
                dust.localAI[0] = i * 0.45f;
            }

            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PrimordialEarth>().
                AddIngredient<CosmiliteBar>(8).
                AddIngredient<EndothermicEnergy>(20).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
