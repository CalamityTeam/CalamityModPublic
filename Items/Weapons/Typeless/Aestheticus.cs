using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
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
            Item.width = 58;
            Item.damage = 8;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item109;
            Item.autoReuse = true;
            Item.height = 58;

            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.Calamity().donorItem = true;

            Item.shoot = ModContent.ProjectileType<CursorProj>();
            Item.shootSpeed = 5f;
        }

        // Aestheticus scales off of all damage types simultaneously (meaning it scales 5x from universal damage boosts).
        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
        {
            float formula = 5f * (player.allDamage - 1f);
            formula += player.GetDamage(DamageClass.Melee) - 1f;
            formula += player.GetDamage(DamageClass.Ranged) - 1f;
            formula += player.GetDamage(DamageClass.Magic) - 1f;
            formula += player.GetDamage(DamageClass.Summon) - 1f;
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
            CreateRecipe(1).AddIngredient(ItemID.HellstoneBar, 10).AddIngredient(ItemID.MeteoriteBar, 10).AddIngredient(ModContent.ItemType<AerialiteBar>(), 5).AddIngredient(ModContent.ItemType<SeaPrism>(), 10).AddIngredient(ItemID.Glass, 20).AddIngredient(ItemID.Gel, 15).AddIngredient(ItemID.FallenStar, 5).AddTile(TileID.Anvils).Register();
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Vaporfied>(), 120);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Vaporfied>(), 120);
        }
    }
}
