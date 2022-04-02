using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class ShinobiBlade : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shinobi Blade");
            Tooltip.SetDefault("Throws a fast blade that spawns healing orbs when it kills an enemy\n" +
                "Stealth strikes cause 3 blades to be thrown at once");
        }

        public override void SafeSetDefaults()
        {
            item.width = 16;
            item.height = 42;
            item.damage = 15;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 15;
            item.knockBack = 1f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.maxStack = 1;
            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = ItemRarityID.Orange;
            item.shoot = ModContent.ProjectileType<ShinobiBladeProjectile>();
            item.shootSpeed = 20f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                float spreadAngle = MathHelper.ToRadians(7.5f);
                Vector2 direction = new Vector2(speedX, speedY);
                Vector2 baseDirection = direction.RotatedBy(-spreadAngle);

                for (int i = 0; i < 3; i++)
                {
                    Vector2 currentDirection = baseDirection.RotatedBy(spreadAngle * i);

                    int p = Projectile.NewProjectile(position, currentDirection, type, damage, knockBack, player.whoAmI);
                    if (p.WithinBounds(Main.maxProjectiles))
                        Main.projectile[p].Calamity().stealthStrike = true;
                }
                return false;
            }
            return true;
        }
    }
}
