using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class InfernalKris : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
                       Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 38;
            Item.damage = 24;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.consumable = true;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 18;
            Item.knockBack = 1f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(0, 0, 5, 0);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<InfernalKrisProjectile>();
            Item.shootSpeed = 15f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI, 0f, 1f);
                if (p.WithinBounds(Main.maxProjectiles))
                    Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(100).
                AddIngredient(ItemID.HellstoneBar).
                AddIngredient(ItemID.Obsidian, 2).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
