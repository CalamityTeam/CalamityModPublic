using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class CosmicKunai : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Kunai");
            Tooltip.SetDefault("Fires a stream of short-range kunai\n" +
                "Stealth strikes spawn 5 Cosmic Scythes which home and explode");
        }

        public override void SafeSetDefaults()
        {
            item.width = 26;
            item.damage = 150;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useTime = 2;
            item.useAnimation = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 5f;
            item.UseSound = SoundID.Item109;
            item.autoReuse = true;
            item.height = 48;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<CosmicKunaiProj>();
            item.shootSpeed = 28f;
            item.Calamity().rogue = true;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f);
            if (player.Calamity().StealthStrikeAvailable() && player.ownedProjectileCounts[ModContent.ProjectileType<CosmicScythe>()] < 10)
            {
				Main.projectile[stealth].Calamity().stealthStrike = true;
                Main.PlaySound(SoundID.Item73, player.position);
                for (float i = 0; i < 5; i++)
                {
                    float angle = MathHelper.TwoPi / 5f * i;
                    Projectile.NewProjectile(player.Center, angle.ToRotationVector2() * 8f, ModContent.ProjectileType<CosmicScythe>(), (int)(damage * 0.8f), knockBack, player.whoAmI, angle, 0f);
                }
            }
            return false;
        }
    }
}
