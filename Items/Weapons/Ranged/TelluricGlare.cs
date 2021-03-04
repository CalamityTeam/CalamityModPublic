using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class TelluricGlare : ModItem
    {
        public const int RayCount = 4;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Telluric Glare");
            Tooltip.SetDefault($"Fires a burst of {RayCount} solar rays outward");
        }

        public override void SetDefaults()
        {
            item.damage = 150;
            item.ranged = true;
            item.width = 54;
            item.height = 92;
            item.useTime = item.useAnimation = 16;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4f;
			item.value = CalamityGlobalItem.Rarity12BuyPrice;
			item.rare = ItemRarityID.Purple;
			item.Calamity().customRarity = CalamityRarity.Turquoise;
			item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<TelluricGlareProj>();
            item.shootSpeed = 18f;
            item.useAmmo = AmmoID.Arrow;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 velocity = new Vector2(speedX, speedY).SafeNormalize(Vector2.UnitX * player.direction) * 30f;
            bool willHitTiles = Collision.CanHit(position, 0, 0, position + velocity, 0, 0);
            for (int i = 0; i < RayCount; i++)
            {
                Vector2 offset = velocity.RotatedBy(MathHelper.Pi * (i - (RayCount - 1f) / 2f) * 0.18f, default);
                
                // Go back if the spawn position and offset spawn position have an obstacle to prevent arrows from appearing
                // behind walls the player cannot access.
                if (!willHitTiles)
                    offset -= velocity;

                Projectile.NewProjectile(position + offset, new Vector2(speedX, speedY), item.shoot, damage, knockBack, player.whoAmI);
            }

            return false;
        }
    }
}
