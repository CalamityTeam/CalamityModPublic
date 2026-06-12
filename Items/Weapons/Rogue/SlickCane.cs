using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class SlickCane : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 36;
            Item.damage = 50;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.useTime = Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.DD2_GhastlyGlaivePierce;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(gold: 35); // Sold by Bandit
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<SlickCaneProjectile>();
            Item.shootSpeed = 16f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float ai0 = Main.rand.NextFloat() * Item.shootSpeed * 1.5f * (float)player.direction;
            int projectileIndex = Projectile.NewProjectile(source, position.X, position.Y+100, velocity.X*2f, velocity.Y*2f, type, damage, knockback, player.whoAmI, ai0, 0f);
            if (projectileIndex.WithinBounds(Main.maxProjectiles))
                Main.projectile[projectileIndex].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }
    }
}
