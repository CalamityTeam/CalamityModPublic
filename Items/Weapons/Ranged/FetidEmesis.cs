using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class FetidEmesis : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fetid Emesis");
            Tooltip.SetDefault("40% chance to not consume ammo\n" +
            "Has a chance to release rotten chunks instead of bullets");
        }

        public override void SetDefaults()
        {
            Item.damage = 129;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 76;
            Item.height = 46;
            Item.useTime = Item.useAnimation = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Bullet;

            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool ConsumeAmmo(Player player) => Main.rand.Next(100) > 40;

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (Main.rand.NextBool(8))
            {
                Projectile.NewProjectile(position, new Vector2(speedX, speedY) * 0.8f,
                    ModContent.ProjectileType<EmesisGore>(), damage, knockBack, player.whoAmI);
                for (int i = 0; i < 5; i++)
                {
                    Dust dust = Dust.NewDustDirect(position, 10, 10, 27);
                    dust.velocity = Vector2.Normalize(new Vector2(speedX, speedY)).RotatedByRandom(MathHelper.ToRadians(15f));
                    dust.noGravity = true;
                }
                if (player.Calamity().soundCooldown <= 0)
                {
                    // WoF vomit sound.
                    SoundEngine.PlaySound(SoundID.NPCKilled, (int)position.X, (int)position.Y, 13, 0.5f, 0f);
                    player.Calamity().soundCooldown = 120;
                }
                return false;
            }
            return true;
        }
    }
}
