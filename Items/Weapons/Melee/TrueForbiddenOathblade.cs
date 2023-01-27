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
    public class TrueForbiddenOathblade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("True Forbidden Oathblade");
            Tooltip.SetDefault("Fires a spread of demonic scythes and critical hits cause shadowflame explosions");
            SacrificeTotal = 1;
        }

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

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.ShadowFlame, 150);
            target.AddBuff(BuffID.OnFire, 300);
            if (crit)
            {
                target.AddBuff(BuffID.ShadowFlame, 450);
                target.AddBuff(BuffID.OnFire, 900);
                player.ApplyDamageToNPC(target, player.GetWeaponDamage(player.ActiveItem()) * 2, 0f, 0, false);
                float num50 = 1.7f;
                float num51 = 0.8f;
                float num52 = 2f;
                Vector2 value3 = (target.rotation - MathHelper.PiOver2).ToRotationVector2();
                Vector2 value4 = value3 * target.velocity.Length();
                SoundEngine.PlaySound(SoundID.Item14, target.Center);
                for (int num53 = 0; num53 < 40; num53++)
                {
                    int num54 = Dust.NewDust(target.position, target.width, target.height, 173, 0f, 0f, 200, default, num50);
                    Dust dust = Main.dust[num54];
                    dust.position = target.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * (float)target.width / 2f;
                    dust.noGravity = true;
                    dust.velocity.Y -= 4.5f;
                    dust.velocity *= 3f;
                    dust.velocity += value4 * Main.rand.NextFloat();
                    num54 = Dust.NewDust(target.position, target.width, target.height, 173, 0f, 0f, 100, default, num51);
                    dust.position = target.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * (float)Main.rand.NextDouble() * (float)target.width / 2f;
                    dust.velocity.Y -= 3f;
                    dust.velocity *= 2f;
                    dust.noGravity = true;
                    dust.fadeIn = 1f;
                    dust.color = Color.Crimson * 0.5f;
                    dust.velocity += value4 * Main.rand.NextFloat();
                }
                for (int num55 = 0; num55 < 20; num55++)
                {
                    int num56 = Dust.NewDust(target.position, target.width, target.height, 173, 0f, 0f, 0, default, num52);
                    Dust dust = Main.dust[num56];
                    dust.position = target.Center + Vector2.UnitX.RotatedByRandom(Math.PI).RotatedBy((double)target.velocity.ToRotation(), default) * (float)target.width / 3f;
                    dust.noGravity = true;
                    dust.velocity.Y -= 1.5f;
                    dust.velocity *= 0.5f;
                    dust.velocity += value4 * (0.6f + 0.6f * Main.rand.NextFloat());
                }
            }
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Shadowflame>(), 150);
            target.AddBuff(BuffID.OnFire, 300);
            if (crit)
            {
                target.AddBuff(ModContent.BuffType<Shadowflame>(), 450);
                target.AddBuff(BuffID.OnFire, 900);
                SoundEngine.PlaySound(SoundID.Item14, target.Center);
            }
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
