using Terraria.DataStructures;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Malevolence : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Malevolence");
            Tooltip.SetDefault("Fires two arrows at once\n" +
                "Converts wooden arrows into plague arrows that explode into bees on death");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 56;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 36;
            Item.height = 58;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item97;
            Item.autoReuse = true;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<PlagueArrow>();
            Item.useAmmo = AmmoID.Arrow;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 2; i++)
            {
                float SpeedX = velocity.X + Main.rand.Next(-20, 21) * 0.05f;
                float SpeedY = velocity.Y + Main.rand.Next(-20, 21) * 0.05f;

                if (CalamityUtils.CheckWoodenAmmo(type, player))
                    Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<PlagueArrow>(), damage, knockback, player.whoAmI);
                else
                {
                    int proj = Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, damage, knockback, player.whoAmI);
                    Main.projectile[proj].noDropItem = true;
                }
            }
            return false;
        }
    }
}
