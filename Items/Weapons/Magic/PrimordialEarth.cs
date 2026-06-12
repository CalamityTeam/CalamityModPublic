using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class PrimordialEarth : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";

        public static int BuffDefenseBoost = 4;
        public static float BuffDamageBoost = 0.12f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(BuffDefenseBoost, BuffDamageBoost.ToPercent());

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 42;
            Item.damage = 205;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 48;
            Item.useTime = Item.useAnimation = 68;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 10;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = new SoundStyle("CalamityMod/Sounds/Item/MagicRockSound") with { Volume = 0.4f, Pitch = -0.1f };
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PrimordialEarthProjectile>();
            Item.shootSpeed = 4.5f;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.velocity.Length() <= 14)
                player.velocity += -velocity.SafeNormalize(Vector2.UnitX) * 6f;

            Vector2 staticSpeed = Utils.DirectionTo(player.Center, player.Calamity().mouseWorld) * Utils.Distance(player.Center, player.ClampedMouseWorld()) * 0.008f;
            bool MaxMana = player.statMana >= (player.statManaMax2 - ((int)(Item.mana * player.manaCost))) && !player.HasBuff(BuffID.ManaSickness);
            float rotation = 0.4f;
            Projectile.NewProjectile(source, position, staticSpeed.RotatedBy(-rotation), type, damage / 2, knockback, player.whoAmI, 0f, 1f, MaxMana ? 1f : 0f);
            Projectile.NewProjectile(source, position, staticSpeed.RotatedBy(rotation), type, damage / 2, knockback, player.whoAmI, 0f, 0f, MaxMana ? 1f : 0f);

            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DeathValleyDuster>().
                AddIngredient(ItemID.Amber, 5).
                AddIngredient(ItemID.Ectoplasm, 5).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
