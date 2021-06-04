using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Carnage : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Carnage");
            Tooltip.SetDefault("Enemies explode into homing blood on death");
        }

        public override void SetDefaults()
        {
            item.width = 46;
            item.damage = 55;
            item.melee = true;
            item.useAnimation = 21;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 21;
            item.knockBack = 5.25f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.useTurn = true;
            item.height = 60;
            item.value = CalamityGlobalItem.Rarity3BuyPrice;
			item.rare = ItemRarityID.Orange;
			item.Calamity().challengeDrop = true;
		}

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 5);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (target.life <= 0)
            {
                Main.PlaySound(SoundID.Item74, (int)target.position.X, (int)target.position.Y);
                target.position.X += (float)(target.width / 2);
                target.position.Y += (float)(target.height / 2);
                target.position.X -= (float)(target.width / 2);
                target.position.Y -= (float)(target.height / 2);
                for (int num621 = 0; num621 < 15; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 5, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 25; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 5, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 5, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
                int num251 = Main.rand.Next(4, 6);
                for (int num252 = 0; num252 < num251; num252++)
                {
                    Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    while (value15.X == 0f && value15.Y == 0f)
                    {
                        value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    }
                    value15.Normalize();
                    value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
                    Projectile.NewProjectile(target.Center.X, target.Center.Y, value15.X, value15.Y, ModContent.ProjectileType<Blood>(), (int)(item.damage * (player.allDamage + player.meleeDamage - 1f)), knockback, player.whoAmI, 0f, 0f);
                }
            }
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (target.statLife <= 0)
            {
                Main.PlaySound(SoundID.Item74, (int)target.position.X, (int)target.position.Y);
                target.position.X += (float)(target.width / 2);
                target.position.Y += (float)(target.height / 2);
                target.position.X -= (float)(target.width / 2);
                target.position.Y -= (float)(target.height / 2);
                for (int num621 = 0; num621 < 15; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 5, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 25; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 5, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 5, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
                int num251 = Main.rand.Next(4, 6);
                for (int num252 = 0; num252 < num251; num252++)
                {
                    Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    while (value15.X == 0f && value15.Y == 0f)
                    {
                        value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    }
                    value15.Normalize();
                    value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
                    Projectile.NewProjectile(target.Center.X, target.Center.Y, value15.X, value15.Y, ModContent.ProjectileType<Blood>(), (int)(item.damage * (player.allDamage + player.meleeDamage - 1f)), item.knockBack, player.whoAmI, 0f, 0f);
                }
            }
        }
    }
}
