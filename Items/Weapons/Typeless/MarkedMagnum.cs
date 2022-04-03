using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Typeless
{
    public class MarkedMagnum : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Marked Magnum");
            Tooltip.SetDefault("Shots reduce enemy protection\n" +
                "This weapon scales with all your damage stats at once");
        }

        public override void SetDefaults()
        {
            Item.damage = 4;
            Item.width = 54;
            Item.height = 20;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item33;
            Item.autoReuse = false;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<MarkRound>();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        // Marked Magnum scales off of all damage types simultaneously (meaning it scales 5x from universal damage boosts).
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
            CreateRecipe(1).AddIngredient(ItemID.HellstoneBar, 7).AddIngredient(ItemID.Obsidian, 15).AddIngredient(ItemID.GlowingMushroom, 15).AddTile(TileID.Anvils).Register();
        }
    }
}
