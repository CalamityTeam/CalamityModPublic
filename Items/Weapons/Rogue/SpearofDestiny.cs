using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class SpearofDestiny : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spear of Destiny");
            Tooltip.SetDefault("Throws three spears with the outer two having homing capabilities");
        }

        public override void SafeSetDefaults()
        {
            item.width = 52;
            item.damage = 25;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 20;
            item.useStyle = 1;
            item.useTime = 20;
            item.knockBack = 2f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 52;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.shoot = ModContent.ProjectileType<SpearofDestinyProjectile>();
            item.shootSpeed = 20f;
            item.Calamity().rogue = true;
            item.Calamity().customRarity = CalamityRarity.RareVariant;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int numProj = 2;
            float rotation = MathHelper.ToRadians(3);
            for (int i = 0; i < numProj + 1; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, i == 1 ? type : ModContent.ProjectileType<SpearofDestinyProjectile>(), damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }
    }
}
