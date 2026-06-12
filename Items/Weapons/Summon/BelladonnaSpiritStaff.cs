using CalamityMod.Buffs.Summon;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class BelladonnaSpiritStaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        #region Other stats for easy modification

        public const float EnemyDistanceDetection = 1200f; // In pixels.

        public const float FireRate = 75f;  // In frames. 60 frames = 1 second.

        public const float PetalTimeBeforeTargetting = 60f;  // In frames. 60 frames = 1 second.

        public const float PetalVelocity = 20f;

        public const float PetalGravityStrenght = 0.2f;

        #endregion
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.Poisoned];
        }
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 42;
            Item.damage = 22;
            Item.knockBack = 1f;
            Item.mana = 10;

            Item.buffType = ModContent.BuffType<BelladonnaSpiritBuff>();
            Item.shoot = ModContent.ProjectileType<BelladonnaSpirit>();
            Item.useAnimation = Item.useTime = 36;

            Item.DamageType = DamageClass.Summon;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item44;
            Item.rare = ItemRarityID.Orange;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.noMelee = true;
            Item.autoReuse = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            var minion = Projectile.NewProjectileDirect(source, player.ClampedMouseWorld(), Vector2.Zero, type, damage, knockback, player.whoAmI);
            minion.originalDamage = Item.damage;
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Vine, 4).
                AddIngredient(ItemID.JungleSpores, 5).
                AddIngredient(ItemID.Stinger, 8).
                AddIngredient(ItemID.RichMahogany, 25).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
