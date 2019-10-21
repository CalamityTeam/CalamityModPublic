using CalamityMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class CosmicDischarge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Discharge");
            Tooltip.SetDefault("Legendary Drop\n" +
                "Striking an enemy with the whip causes glacial explosions and grants the player the cosmic freeze buff\n" +
                "This buff gives the player increased life regen while standing still and freezes enemies near the player\n" +
                "Revengeance drop");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.damage = 1000;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.autoReuse = true;
            item.melee = true;
            item.useAnimation = 15;
            item.useTime = 15;
            item.useStyle = 5;
            item.knockBack = 0.5f;
            item.UseSound = SoundID.Item122;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.shootSpeed = 24f;
            item.shoot = ModContent.ProjectileType<CosmicDischargeFlail>();
            item.Calamity().postMoonLordRarity = 17;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float ai3 = (Main.rand.NextFloat() - 0.75f) * 0.7853982f; //0.5
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, ai3);
            return false;
        }
    }
}
