using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class ShinobiBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shinobi Blade");
            Tooltip.SetDefault("Throws a fast blade that spawns healing orbs when it kills an enemy\n" +
                "Stealth strikes cause 3 blades to be thrown at once");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 42;
            Item.damage = 15;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 15;
            Item.knockBack = 1f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<ShinobiBladeProjectile>();
            Item.shootSpeed = 20f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                float spreadAngle = MathHelper.ToRadians(7.5f);
                Vector2 direction = velocity;
                Vector2 baseDirection = direction.RotatedBy(-spreadAngle);

                for (int i = 0; i < 3; i++)
                {
                    Vector2 currentDirection = baseDirection.RotatedBy(spreadAngle * i);

                    int p = Projectile.NewProjectile(source, position, currentDirection, type, damage, knockback, player.whoAmI);
                    if (p.WithinBounds(Main.maxProjectiles))
                        Main.projectile[p].Calamity().stealthStrike = true;
                }
                return false;
            }
            return true;
        }
    }
}
