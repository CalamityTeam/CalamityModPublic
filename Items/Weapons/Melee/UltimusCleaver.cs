using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class UltimusCleaver : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.OnFire3];
        }
        public override void SetDefaults()
        {
            Item.width = 72;
            Item.height = 62;
            Item.damage = 130;
            Item.DamageType = DamageClass.Melee;
            Item.useTurn = true;
            Item.rare = ItemRarityID.Yellow;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 8f;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<UltimusCleaverDust>(); // Dummy argument to ensure it doesn't get set to true melee
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => false;

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 360);
            SoundEngine.PlaySound(SoundID.Item14, target.Center);
            int onHitDamage = player.CalcIntDamage<MeleeDamageClass>(Item.damage);
            Projectile blast = Projectile.NewProjectileDirect(Item.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), onHitDamage, 0f, player.whoAmI, target.whoAmI);
            blast.DamageType = Item.DamageType;

            Vector2 dustRotation = (target.rotation - MathHelper.PiOver2).ToRotationVector2();
            Vector2 dustVelocity = dustRotation * target.velocity.Length();
            for (int i = 0; i < 40; i++)
            {
                Vector2 mainDustPos = target.Center + Main.rand.NextVector2Circular(target.width / 2, target.width / 2);
                Dust swingDust = Dust.NewDustPerfect(mainDustPos, DustID.InfernoFork, dustVelocity * Main.rand.NextFloat() - Vector2.UnitY * 18f, 200, Scale: 1.7f);
                swingDust.noGravity = true;

                Dust swingDust2 = Dust.NewDustPerfect(mainDustPos, DustID.InfernoFork, dustVelocity * Main.rand.NextFloat() - Vector2.UnitY * 12f, 100, Color.Crimson * 0.5f, 0.8f);
                swingDust2.noGravity = true;
                swingDust2.fadeIn = 1f;
            }
            for (int j = 0; j < 20; j++)
            {
                Vector2 dustPos = target.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(target.velocity.ToRotation()) * (target.width / 3f);
                Dust moreDust = Dust.NewDustPerfect(dustPos, DustID.InfernoFork, dustVelocity * (0.6f + 0.6f * Main.rand.NextFloat()) - Vector2.UnitY * 3f, Scale: 2f);
                moreDust.noGravity = true;
            }
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (player.whoAmI == Main.myPlayer && !player.Calamity().bladeArmEnchant)
            {
                if (player.itemAnimation == (int)(player.itemAnimationMax * 0.1) ||
                    player.itemAnimation == (int)(player.itemAnimationMax * 0.3) ||
                    player.itemAnimation == (int)(player.itemAnimationMax * 0.5) ||
                    player.itemAnimation == (int)(player.itemAnimationMax * 0.7) ||
                    player.itemAnimation == (int)(player.itemAnimationMax * 0.9))
                {
                    float sparkXVel = 0f;
                    float sparkYVel = 0f;
                    float sparkXSpawn = 0f;
                    float sparkYSpawn = 0f;
                    
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.9))
                    {
                        sparkYVel = -10.5f;
                        if (player.direction == -1)
                            sparkXSpawn = -8f;
                    }
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.7))
                    {
                        sparkYVel = -9f;
                        sparkXVel = 3f;
                        sparkXSpawn = player.direction == -1 ? 20f : 26f;
                    }
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.5))
                    {
                        sparkYVel = -6f;
                        sparkXVel = 6f;
                    }
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.3))
                    {
                        sparkXVel = 9f;
                        sparkYVel = -3f;
                        sparkXSpawn = -4f;
                        sparkYSpawn = -20f;
                    }
                    if (player.itemAnimation == (int)(player.itemAnimationMax * 0.1))
                    {
                        sparkXVel = 10.5f;
                        sparkYSpawn = 6f;
                    }

                    sparkXVel *= player.direction;
                    sparkYVel *= player.gravDir;
                    sparkXSpawn *= player.direction;
                    sparkYSpawn *= player.gravDir;
                    var source = player.GetSource_ItemUse(Item);
                    int damage = (int)player.GetTotalDamage<MeleeDamageClass>().ApplyTo(Item.damage * 0.1f);
                    Projectile.NewProjectile(source, hitbox.X + hitbox.Width / 2 + sparkXSpawn, hitbox.Y + hitbox.Height / 2 + sparkYSpawn, sparkXVel, sparkYVel, ModContent.ProjectileType<UltimusCleaverDust>(), damage, 0f, player.whoAmI);
                }
            }
        }
    }
}
