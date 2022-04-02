using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class DeepWounder : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deep Wounder");
            Tooltip.SetDefault("Throws an abyssal hatchet that inflicts Armor Crunch and Marked for Death to the enemies it hits\n" +
                "Stealth strikes cause the hatchet to be thrown faster and trail water, inflicting Crush Depth in addition to the other debuffs");
        }

        public override void SafeSetDefaults()
        {
            item.width = 52;
            item.damage = 106;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 23;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 23;
            item.knockBack = 3f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 48;
            item.maxStack = 1;
            item.rare = ItemRarityID.Lime;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.shoot = ModContent.ProjectileType<DeepWounderProjectile>();
            item.shootSpeed = 14f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                float stealthSpeedMult = 1.5f;
                Vector2 velocity = new Vector2(speedX, speedY);
                velocity.Normalize();
                velocity *= item.shootSpeed * stealthSpeedMult;

                int p = Projectile.NewProjectile(position, velocity, type, damage, knockBack, player.whoAmI, 0f, 1f);
                if (p.WithinBounds(Main.maxProjectiles))
                    Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}
