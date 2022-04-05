using Terraria.DataStructures;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

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
            Item.width = 46;
            Item.damage = 70;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 21;
            Item.knockBack = 5.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.height = 60;
            Item.scale = 1.25f;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
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
            OnHitEffects(player, target.life <= 0, target.Center, target.width, target.height, knockback);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            OnHitEffects(player, target.statLife <= 0, target.Center, target.width, target.height, Item.knockBack);
        }

        private void OnHitEffects(Player player, bool health, Vector2 targetPos, int targetWidth, int targetHeight, float kBack)
        {
            if (health)
            {
                var source = player.GetProjectileSource_Item(Item);
                SoundEngine.PlaySound(SoundID.Item74, targetPos);
                targetPos.X += (float)(targetWidth / 2);
                targetPos.Y += (float)(targetHeight / 2);
                targetPos.X -= (float)(targetWidth / 2);
                targetPos.Y -= (float)(targetHeight / 2);
                for (int i = 0; i < 15; i++)
                {
                    int idx = Dust.NewDust(targetPos, targetWidth, targetHeight, 5, 0f, 0f, 100, default, 2f);
                    Main.dust[idx].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[idx].scale = 0.5f;
                        Main.dust[idx].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int i = 0; i < 25; i++)
                {
                    int idx = Dust.NewDust(targetPos, targetWidth, targetHeight, 5, 0f, 0f, 100, default, 3f);
                    Main.dust[idx].noGravity = true;
                    Main.dust[idx].velocity *= 5f;
                    idx = Dust.NewDust(targetPos, targetWidth, targetHeight, 5, 0f, 0f, 100, default, 2f);
                    Main.dust[idx].velocity *= 2f;
                }
                int bloodAmt = Main.rand.Next(4, 6);
                for (int i = 0; i < bloodAmt; i++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(source, targetPos, velocity, ModContent.ProjectileType<Blood>(), (int)(Item.damage * (player.allDamage + player.GetDamage(DamageClass.Melee) - 1f)), kBack, player.whoAmI, 0f, 0f);
                }
            }
        }
    }
}
