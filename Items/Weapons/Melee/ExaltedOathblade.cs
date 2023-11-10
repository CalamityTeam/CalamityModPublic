using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("TrueForbiddenOathblade")]
    public class ExaltedOathblade : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 88;
            Item.height = 88;
            Item.damage = 150;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 23;
            Item.useTime = 23;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<ForbiddenOathbladeProjectile>();
            Item.shootSpeed = 3f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int index = 8;
            for (int i = -index; i <= index; i += index)
            {
                Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.ToRadians(i));
                Projectile.NewProjectile(source, position, perturbedSpeed, type, damage / 2, knockback, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 173);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 150);
            target.AddBuff(BuffID.OnFire, 300);
            if (hit.Crit)
            {
                target.AddBuff(BuffID.ShadowFlame, 450);
                target.AddBuff(BuffID.OnFire, 900);
                player.ApplyDamageToNPC(target, player.GetWeaponDamage(player.ActiveItem()) * 2, 0f, 0, false);
                float firstDustScale = 1.7f;
                float secondDustScale = 0.8f;
                float thirdDustScale = 2f;
                Vector2 dustRotation = (target.rotation - MathHelper.PiOver2).ToRotationVector2();
                Vector2 dustVelocity = dustRotation * target.velocity.Length();
                SoundEngine.PlaySound(SoundID.Item14, target.Center);
                for (int i = 0; i < 40; i++)
                {
                    int swingDust = Dust.NewDust(target.position, target.width, target.height, 173, 0f, 0f, 200, default, firstDustScale);
                    Dust dust = Main.dust[swingDust];
                    dust.position = target.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * (float)target.width / 2f;
                    dust.noGravity = true;
                    dust.velocity.Y -= 4.5f;
                    dust.velocity *= 3f;
                    dust.velocity += dustVelocity * Main.rand.NextFloat();
                    swingDust = Dust.NewDust(target.position, target.width, target.height, 173, 0f, 0f, 100, default, secondDustScale);
                    dust.position = target.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * (float)target.width / 2f;
                    dust.velocity.Y -= 3f;
                    dust.velocity *= 2f;
                    dust.noGravity = true;
                    dust.fadeIn = 1f;
                    dust.color = Color.Crimson * 0.5f;
                    dust.velocity += dustVelocity * Main.rand.NextFloat();
                }
                for (int j = 0; j < 20; j++)
                {
                    int swingDust2 = Dust.NewDust(target.position, target.width, target.height, 173, 0f, 0f, 0, default, thirdDustScale);
                    Dust dust = Main.dust[swingDust2];
                    dust.position = target.Center + Vector2.UnitX.RotatedByRandom(Math.PI).RotatedBy((double)target.velocity.ToRotation(), default) * (float)target.width / 3f;
                    dust.noGravity = true;
                    dust.velocity.Y -= 1.5f;
                    dust.velocity *= 0.5f;
                    dust.velocity += dustVelocity * (0.6f + 0.6f * Main.rand.NextFloat());
                }
            }
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<Shadowflame>(), 450);
            target.AddBuff(BuffID.OnFire, 900);
            SoundEngine.PlaySound(SoundID.Item14, target.Center);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ForbiddenOathblade>().
                AddIngredient(ItemID.BrokenHeroSword).
                AddIngredient<AshesofCalamity>(8).
                AddIngredient<InfectedArmorPlating>(8).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
