using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Typeless
{
    public class Aestheticus : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aestheticus");
            Tooltip.SetDefault("Fires crystals that explode and slow enemies down\n" +
                "This weapon scales with all your damage stats at once");
        }

        public override void SetDefaults()
        {
            item.width = 58;
            item.damage = 8;
            item.useAnimation = 25;
            item.useTime = 25;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 3f;
            item.UseSound = SoundID.Item109;
            item.autoReuse = true;
            item.height = 58;

            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = ItemRarityID.Orange;
            item.Calamity().donorItem = true;

            item.shoot = ModContent.ProjectileType<CursorProj>();
            item.shootSpeed = 5f;
        }

        // Aestheticus scales off of all damage types simultaneously (meaning it scales 5x from universal damage boosts).
        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
        {
            float formula = 5f * (player.allDamage - 1f);
            formula += player.meleeDamage - 1f;
            formula += player.rangedDamage - 1f;
            formula += player.magicDamage - 1f;
            formula += player.minionDamage - 1f;
            formula += player.Calamity().throwingDamage - 1f;
            add += formula;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HellstoneBar, 10);
            recipe.AddIngredient(ItemID.MeteoriteBar, 10);
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 5);
            recipe.AddIngredient(ItemID.Glass, 20);
            recipe.AddIngredient(ItemID.Gel, 15);
            recipe.AddIngredient(ItemID.FallenStar, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Vaporfied>(), 180);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Vaporfied>(), 180);
        }
    }
}
