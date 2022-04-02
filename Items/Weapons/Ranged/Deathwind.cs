using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Deathwind : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deathwind");
            Tooltip.SetDefault("Fires a spread of 4 arrows\n" +
                "Wooden arrows are converted into nebula arrows");
        }

        public override void SetDefaults()
        {
            item.damage = 248;
            item.ranged = true;
            item.width = 40;
            item.height = 82;
            item.useTime = 14;
            item.useAnimation = 14;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<DWArrow>();
            item.shootSpeed = 20f;
            item.useAmmo = AmmoID.Arrow;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.GetTexture("CalamityMod/Items/Weapons/Ranged/DeathwindGlow"));
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int index = 0; index < 4; ++index)
            {
                float SpeedX = speedX + Main.rand.Next(-20, 21) * 0.05f;
                float SpeedY = speedY + Main.rand.Next(-20, 21) * 0.05f;
                if (type == ProjectileID.WoodenArrowFriendly)
                {
                    Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<DWArrow>(), damage, knockBack, player.whoAmI);
                }
                else
                {
                    int num121 = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, (int)(damage * 0.8), knockBack, player.whoAmI);
                    Main.projectile[num121].noDropItem = true;
                }
            }
            return false;
        }
    }
}
