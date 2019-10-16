using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class PearlGod : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pearl God");
            Tooltip.SetDefault("Your life is mine...");
        }

        public override void SetDefaults()
        {
            item.damage = 40;
            item.ranged = true;
            item.width = 50;
            item.height = 32;
            item.useTime = 12;
            item.useAnimation = 12;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 3f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.UseSound = SoundID.Item41;
            item.autoReuse = true;
            item.shootSpeed = 24f;
            item.shoot = ModContent.ProjectileType<ShockblastRound>();
            item.useAmmo = 97;
            item.Calamity().postMoonLordRarity = 22;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int index = 0; index < 2; ++index)
            {
                float speedMult = (float)(index + 1) * 0.15f;
                Projectile.NewProjectile(position.X, position.Y, speedX * speedMult, speedY * speedMult, ModContent.ProjectileType<FrostsparkBullet>(), (int)((double)damage * 0.75), knockBack, player.whoAmI, 0f, 0f);
            }
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<ShockblastRound>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
