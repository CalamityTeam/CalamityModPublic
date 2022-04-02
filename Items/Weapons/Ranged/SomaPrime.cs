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
        }

        public override void SetDefaults()
        {
            item.damage = 130;
            item.ranged = true;
            item.width = 94;
            item.height = 34;
            item.useTime = item.useAnimation = 5;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2f;
            item.UseSound = SoundID.Item40;
            item.autoReuse = true;
            item.shoot = ProjectileID.BulletHighVelocity;
            item.shootSpeed = 9f;
            item.useAmmo = AmmoID.Bullet;

            item.value = CalamityGlobalItem.Rarity16BuyPrice;
            item.Calamity().customRarity = CalamityRarity.HotPink;
            item.Calamity().devItem = true;
            item.Calamity().canFirePointBlankShots = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 30;

        public override Vector2? HoldoutOffset() => new Vector2(-25, 0);

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<P90>());
            recipe.AddIngredient(ModContent.ItemType<Minigun>());
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (type == ProjectileID.Bullet) {
                type = ProjectileID.BulletHighVelocity;
                damage += 4; // in 1.4, HVBs deal 11 damage and Musket Balls deal 7
            }

            speedX += Main.rand.NextFloat(-XYInaccuracy, XYInaccuracy);
            speedY += Main.rand.NextFloat(-XYInaccuracy, XYInaccuracy);
            Vector2 vel = new Vector2(speedX, speedY);
            Projectile shot = Projectile.NewProjectileDirect(position, vel, type, damage, knockBack, player.whoAmI);
            CalamityGlobalProjectile cgp = shot.Calamity();
            cgp.canSupercrit = true;
            cgp.appliesSomaShred = true;
            return false;
        }

        public override bool ConsumeAmmo(Player player) => Main.rand.NextFloat() > 0.8f;
    }
}
