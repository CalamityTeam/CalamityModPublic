using CalamityMod.Items.Materials;
using CalamityMod.Projectiles;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class SomaPrime : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        private static readonly float XYInaccuracy = 0.32f;

        public static int AmmoSavedPercent = 80;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(AmmoSavedPercent);

        public override void SetDefaults()
        {
            Item.width = 94;
            Item.height = 34;
            Item.damage = 705;
            Item.DamageType = DamageClass.Ranged;
            Item.crit = 26;
            Item.useAnimation = Item.useTime = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.UseSound = SoundID.Item40;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.BulletHighVelocity;
            Item.shootSpeed = 9f;
            Item.useAmmo = AmmoID.Bullet;

            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-25, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (type == ProjectileID.Bullet)
            {
                type = ProjectileID.BulletHighVelocity;
                damage += 4; // in 1.4, HVBs deal 11 damage and Musket Balls deal 7
            }

            velocity.X += Main.rand.NextFloat(-XYInaccuracy, XYInaccuracy);
            velocity.Y += Main.rand.NextFloat(-XYInaccuracy, XYInaccuracy);
            Vector2 vel = velocity;
            Projectile shot = Projectile.NewProjectileDirect(source, position, vel, type, damage, knockback, player.whoAmI);

            // Set all projectiles fired from Soma Prime to have 3x base crit multiplier and to have supercrits enabled.
            // They also are able to apply one stack of Shred.
            CalamityGlobalProjectile cgp = shot.Calamity();
            cgp.supercritHits = -1;
            cgp.bonusCritDamage += 1f;
            cgp.appliesSomaShred = true;
            return false;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.Next(100) >= AmmoSavedPercent;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Kingsbane>().
                AddIngredient(ItemID.VenusMagnum).
                AddIngredient<ShadowspecBar>(5).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
