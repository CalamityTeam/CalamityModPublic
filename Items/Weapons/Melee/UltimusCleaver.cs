using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Projectiles.Melee;

namespace CalamityMod.Items.Weapons.Melee
{
    public class UltimusCleaver : ModItem
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ultimus Cleaver");
            Tooltip.SetDefault("Launches damaging homing sparks");
        }

        public override void SetDefaults()
        {
            item.damage = 300;
            item.melee = true;
            item.rare = 8;
            item.width = 72;
            item.height = 62;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 1;
            item.knockBack = 8f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.autoReuse = true;
            item.UseSound = SoundID.Item1;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.1) ||
                    player.itemAnimation == (int)((double)player.itemAnimationMax * 0.3) ||
                    player.itemAnimation == (int)((double)player.itemAnimationMax * 0.5) ||
                    player.itemAnimation == (int)((double)player.itemAnimationMax * 0.7) ||
                    player.itemAnimation == (int)((double)player.itemAnimationMax * 0.9))
                {
                    float num339 = 0f;
                    float num340 = 0f;
                    float num341 = 0f;
                    float num342 = 0f;
                    if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.9))
                    {
                        num339 = -7f;
                    }
                    if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.7))
                    {
                        num339 = -6f;
                        num340 = 2f;
                    }
                    if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.5))
                    {
                        num339 = -4f;
                        num340 = 4f;
                    }
                    if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.3))
                    {
                        num339 = -2f;
                        num340 = 6f;
                    }
                    if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.1))
                    {
                        num340 = 7f;
                    }
                    if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.7))
                    {
                        num342 = 26f;
                    }
                    if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.3))
                    {
                        num342 -= 4f;
                        num341 -= 20f;
                    }
                    if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.1))
                    {
                        num341 += 6f;
                    }
                    if (player.direction == -1)
                    {
                        if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.9))
                        {
                            num342 -= 8f;
                        }
                        if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.7))
                        {
                            num342 -= 6f;
                        }
                    }
                    num339 *= 1.5f;
                    num340 *= 1.5f;
                    num342 *= (float)player.direction;
                    num341 *= player.gravDir;
                    Projectile.NewProjectile((float)(hitbox.X + hitbox.Width / 2) + num342, (float)(hitbox.Y + hitbox.Height / 2) + num341,
                        (float)player.direction * num340, num339 * player.gravDir, ModContent.ProjectileType<UltimusCleaverDust>(), (int)((float)item.damage * 0.1f * player.meleeDamage), 0f, player.whoAmI, 0f, 0f);
                }
            }
        }
    }
}
