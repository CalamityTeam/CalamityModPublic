using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class BrinyBaron : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Briny Baron");
            Tooltip.SetDefault("Legendary Drop\n" +
                "Striking an enemy with the blade causes a briny typhoon to appear\n" +
                "Right click to fire a razorwind aqua blade\n" +
                "Revengeance drop");
        }

        public override void SetDefaults()
        {
            item.width = 62;
            item.damage = 80;
            item.melee = true;
            item.useAnimation = 20;
            item.useTime = 20;
            item.useTurn = true;
            item.useStyle = 1;
            item.knockBack = 4f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 62;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.shootSpeed = 4f;
            item.Calamity().postMoonLordRarity = 17;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.noMelee = true;
                item.noUseGraphic = true;
                item.useTime = 15;
                item.useAnimation = 15;
                item.UseSound = SoundID.Item84;
                item.shoot = ModContent.ProjectileType<Razorwind>();
            }
            else
            {
                item.noMelee = false;
                item.noUseGraphic = false;
                item.useTime = 20;
                item.useAnimation = 20;
                item.UseSound = SoundID.Item1;
                item.shoot = 0;
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<Razorwind>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 187, 0f, 0f, 100, new Color(53, Main.DiscoG, 255));
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<BrinyTyphoonBubble>(), (int)((float)item.damage * 0.5f * player.meleeDamage), knockback, player.whoAmI);
        }
    }
}
