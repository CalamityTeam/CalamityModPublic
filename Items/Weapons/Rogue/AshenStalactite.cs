using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class AshenStalactite : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ashen Stalactite");
            Tooltip.SetDefault("Throws a fast, small stalactite that crumbles to dust after travelling a short distance\n" +
                "Stealth strikes cause a larger, more damaging stalagmite to be thrown which travels slower and further before crumbling to damaging dust");
        }

        public override void SafeSetDefaults()
        {
            item.width = 36;
            item.height = 34;
            item.damage = 37;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 20;
            item.knockBack = 1f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.maxStack = 1;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.shoot = ModContent.ProjectileType<AshenStalactiteProj>();
            item.shootSpeed = 15f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int stealthType = ModContent.ProjectileType<AshenStalagmiteProj>();
                float stealthSpeedMult = 0.6f;
                float stealthDamageMult = 1.15f;
                float stealthKnockbackMult = 2.5f;
                int p = Projectile.NewProjectile(position.X, position.Y, speedX * stealthSpeedMult, speedY * stealthSpeedMult, stealthType, (int)(damage * stealthDamageMult), (int)(knockBack * stealthKnockbackMult), player.whoAmI, 0f, 1f);
                if (p.WithinBounds(Main.maxProjectiles))
                    Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}
