using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Hellborn : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hellborn");
            Tooltip.SetDefault("Fires a spread of 3 bullets\n" +
                "Converts musket balls into explosive bullets\n" +
                "Enemies that touch the gun while it's being fired trigger a massive explosion\n" +
                "After the explosion, this gun gains a massive boost to damage, fire rate and knockback for 10 seconds\n" +
                "These stat bonuses slowly decay over time");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 7;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 66;
            Item.height = 34;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Bullet;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override float UseTimeMultiplier(Player player) => 1f + (player.Calamity().hellbornBoost * (1f / 600f));

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage) => damage *= 1f + player.Calamity().hellbornBoost * (1f / 600f);

        public override void ModifyWeaponKnockback(Player player, ref StatModifier knockback) => knockback *= 1f + (player.Calamity().hellbornBoost * (1f / 600f));

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int index = 0; index < 3; index++)
            {
                float SpeedX = velocity.X + Main.rand.Next(-15, 16) * 0.05f;
                float SpeedY = velocity.Y + Main.rand.Next(-15, 16) * 0.05f;

                if (type == ProjectileID.Bullet)
                {
                    int bullet = Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, ProjectileID.ExplosiveBullet, damage, knockback, player.whoAmI);
                    Main.projectile[bullet].usesLocalNPCImmunity = true;
                    Main.projectile[bullet].localNPCHitCooldown = 10;
                }
                else
                    Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            player.Calamity().hellbornBoost = 600;
            damage *= 10;
            player.ApplyDamageToNPC(target, (int)(Item.damage * (player.GetDamage<GenericDamageClass>().Additive + player.GetDamage<RangedDamageClass>().Additive - 1f)), 0f, 0, false);
            float num50 = 3.4f;
            float num51 = 1.6f;
            float num52 = 4f;
            Vector2 value3 = (target.rotation - MathHelper.PiOver2).ToRotationVector2();
            Vector2 value4 = value3 * target.velocity.Length();
            SoundEngine.PlaySound(SoundID.Item14, target.position);
            for (int num53 = 0; num53 < 80; num53++)
            {
                int num54 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 174, 0f, 0f, 200, default, num50);
                Dust dust = Main.dust[num54];
                dust.position = target.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * target.width / 2f;
                dust.noGravity = true;
                dust.velocity.Y -= 6f;
                dust.velocity *= 3f;
                dust.velocity += value4 * Main.rand.NextFloat();
                num54 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 174, 0f, 0f, 100, default, num51);
                dust.position = target.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * target.width / 2f;
                dust.velocity.Y -= 6f;
                dust.velocity *= 2f;
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.color = Color.Crimson * 0.5f;
                dust.velocity += value4 * Main.rand.NextFloat();
            }
            for (int num55 = 0; num55 < 40; num55++)
            {
                int num56 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 174, 0f, 0f, 0, default, num52);
                Dust dust = Main.dust[num56];
                dust.position = target.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(target.velocity.ToRotation()) * target.width / 3f;
                dust.noGravity = true;
                dust.velocity.Y -= 6f;
                dust.velocity *= 0.5f;
                dust.velocity += value4 * (0.6f + 0.6f * Main.rand.NextFloat());
            }
        }

        public override void ModifyHitPvp(Player player, Player target, ref int damage, ref bool crit)
        {
            damage *= 10;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 360);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 360);
        }
    }
}
