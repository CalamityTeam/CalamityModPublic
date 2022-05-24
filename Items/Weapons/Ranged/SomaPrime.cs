using Terraria.DataStructures;
using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class SomaPrime : ModItem
    {
        private static readonly float XYInaccuracy = 0.32f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soma Prime");
            Tooltip.SetDefault(@"This weapon can supercrit if its crit chance is over 100%
All bullets fired inflict Shred, a stacking bleed debuff
Shred deals 75 DPS per stack and scales with your ranged stats
Damage ticks of Shred can also critically strike or supercrit
Replaces standard bullets with High Velocity Bullets
80% chance to not consume ammo");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 130;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 94;
            Item.height = 34;
            Item.useTime = Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.UseSound = SoundID.Item40;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.BulletHighVelocity;
            Item.shootSpeed = 9f;
            Item.useAmmo = AmmoID.Bullet;

            Item.value = CalamityGlobalItem.Rarity16BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.HotPink;
            Item.Calamity().devItem = true;
            Item.Calamity().canFirePointBlankShots = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 30;

        public override Vector2? HoldoutOffset() => new Vector2(-25, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (type == ProjectileID.Bullet) {
                type = ProjectileID.BulletHighVelocity;
                damage += 4; // in 1.4, HVBs deal 11 damage and Musket Balls deal 7
            }

            velocity.X += Main.rand.NextFloat(-XYInaccuracy, XYInaccuracy);
            velocity.Y += Main.rand.NextFloat(-XYInaccuracy, XYInaccuracy);
            Vector2 vel = velocity;
            Projectile shot = Projectile.NewProjectileDirect(source, position, vel, type, damage, knockback, player.whoAmI);
            CalamityGlobalProjectile cgp = shot.Calamity();
            cgp.canSupercrit = true;
            cgp.appliesSomaShred = true;
            return false;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextFloat() > 0.8f;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<P90>().
                AddIngredient<Minigun>().
                AddIngredient<ShadowspecBar>(5).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
