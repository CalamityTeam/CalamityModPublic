using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Melee
{
    public class UltimusCleaver : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public override void SetDefaults()
        {
            Item.damage = 130;
            Item.DamageType = DamageClass.Melee;
            Item.useTurn = true;
            Item.rare = ItemRarityID.Yellow;
            Item.width = 72;
            Item.height = 62;
            Item.scale = 1.5f;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 8f;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 360);
            int onHitDamage = player.CalcIntDamage<MeleeDamageClass>(Item.damage);
            player.ApplyDamageToNPC(target, onHitDamage, 0f, 0, false);
            float firstDustScale = 1.7f;
            float secondDustScale = 0.8f;
            float thirdDustScale = 2f;
            Vector2 dustRotation = (target.rotation - 1.57079637f).ToRotationVector2();
            Vector2 dustVelocity = dustRotation * target.velocity.Length();
            SoundEngine.PlaySound(SoundID.Item14, target.Center);
            int increment;
            for (int i = 0; i < 40; i = increment + 1)
            {
                int swingDust = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 174, 0f, 0f, 200, default, firstDustScale);
                Dust dust = Main.dust[swingDust];
                dust.position = target.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)target.width / 2f;
                dust.noGravity = true;
                dust.velocity.Y -= 6f;
                dust.velocity *= 3f;
                dust.velocity += dustVelocity * Main.rand.NextFloat();
                swingDust = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 174, 0f, 0f, 100, default, secondDustScale);
                dust.position = target.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)target.width / 2f;
                dust.velocity.Y -= 6f;
                dust.velocity *= 2f;
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.color = Color.Crimson * 0.5f;
                dust.velocity += dustVelocity * Main.rand.NextFloat();
                increment = i;
            }
            for (int j = 0; j < 20; j = increment + 1)
            {
                int swingDust2 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 174, 0f, 0f, 0, default, thirdDustScale);
                Dust dust = Main.dust[swingDust2];
                dust.position = target.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)target.velocity.ToRotation(), default) * (float)target.width / 3f;
                dust.noGravity = true;
                dust.velocity.Y -= 6f;
                dust.velocity *= 0.5f;
                dust.velocity += dustVelocity * (0.6f + 0.6f * Main.rand.NextFloat());
                increment = j;
            }
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.itemAnimation == (int)(player.itemAnimationMax * 0.1) ||
                    player.itemAnimation == (int)(player.itemAnimationMax * 0.3) ||
                    player.itemAnimation == (int)(player.itemAnimationMax * 0.5) ||
                    player.itemAnimation == (int)(player.itemAnimationMax * 0.7) ||
                    player.itemAnimation == (int)(player.itemAnimationMax * 0.9))
                {
                    float sparkYVel = 0f;
                    float sparkXVel = 0f;
                    float sparkYSpawn = 0f;
                    float sparkXSpawn = 0f;
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.9))
                    {
                        sparkYVel = -7f;
                    }
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.7))
                    {
                        sparkYVel = -6f;
                        sparkXVel = 2f;
                    }
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.5))
                    {
                        sparkYVel = -4f;
                        sparkXVel = 4f;
                    }
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.3))
                    {
                        sparkYVel = -2f;
                        sparkXVel = 6f;
                    }
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.1))
                    {
                        sparkXVel = 7f;
                    }
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.7))
                    {
                        sparkXSpawn = 26f;
                    }
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.3))
                    {
                        sparkXSpawn -= 4f;
                        sparkYSpawn -= 20f;
                    }
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.1))
                    {
                        sparkYSpawn += 6f;
                    }
                    if (player.direction == -1)
                    {
                        if (player.itemAnimation == (int)(player.itemAnimationMax * 0.9))
                        {
                            sparkXSpawn -= 8f;
                        }
                        if (player.itemAnimation == (int)(player.itemAnimationMax * 0.7))
                        {
                            sparkXSpawn -= 6f;
                        }
                    }
                    sparkYVel *= 1.5f;
                    sparkXVel *= 1.5f;
                    sparkXSpawn *= (float)player.direction;
                    sparkYSpawn *= player.gravDir;
                    var source = player.GetSource_ItemUse(Item);
                    int damage = (int)player.GetTotalDamage<MeleeDamageClass>().ApplyTo(Item.damage * 0.1f);
                    Projectile.NewProjectile(source, (float)(hitbox.X + hitbox.Width / 2) + sparkXSpawn, (float)(hitbox.Y + hitbox.Height / 2) + sparkYSpawn,
                        (float)player.direction * sparkXVel, sparkYVel * player.gravDir, ModContent.ProjectileType<UltimusCleaverDust>(), damage, 0f, player.whoAmI, 0f, 0f);
                }
            }
        }
    }
}
