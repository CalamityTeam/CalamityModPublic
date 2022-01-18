using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Mycoroot : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mycoroot");
            Tooltip.SetDefault("Fires a stream of short-range fungal roots\n" +
                "Stealth strikes spawn an explosion of fungi spores");
        }

        public override void SafeSetDefaults()
        {
            item.width = 32;
            item.damage = 10;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useTime = 5;
            item.useAnimation = 5;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 1.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 32;
            item.rare = ItemRarityID.Green;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.shoot = ModContent.ProjectileType<MycorootProj>();
            item.shootSpeed = 20f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f);
            if (player.Calamity().StealthStrikeAvailable() && player.ownedProjectileCounts[ModContent.ProjectileType<ShroomerangSpore>()] < 20 && stealth.WithinBounds(Main.maxProjectiles))
            {
                Main.projectile[stealth].Calamity().stealthStrike = true;
                for (float i = 0; i < Main.rand.Next(7,11); i++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(50f, 10f, 50f, 0.01f);
                    int spore = Projectile.NewProjectile(player.Center, velocity, ModContent.ProjectileType<ShroomerangSpore>(), (int)(damage * 0.5f), knockBack, player.whoAmI);
                    if (spore.WithinBounds(Main.maxProjectiles))
                        Main.projectile[spore].ai[1] = 1f;
                }
            }
            return false;
        }
    }
}
