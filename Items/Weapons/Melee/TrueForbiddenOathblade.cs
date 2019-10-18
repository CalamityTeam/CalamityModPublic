using Microsoft.Xna.Framework;
using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TrueForbiddenOathblade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("True Forbidden Oathblade");
            Tooltip.SetDefault("Fires a spread of demonic scythes and critical hits cause shadowflame explosions");
        }

        public override void SetDefaults()
        {
            item.width = 78;
            item.damage = 200;
            item.melee = true;
            item.useAnimation = 23;
            item.useTime = 23;
            item.useTurn = true;
            item.useStyle = 1;
            item.knockBack = 7.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 78;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.shoot = ModContent.ProjectileType<Oathblade>();
            item.shootSpeed = 3f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int numProj = 2;
            float rotation = MathHelper.ToRadians(4);
            for (int i = 0; i < numProj + 1; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage / 2, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "ForbiddenOathblade");
            recipe.AddIngredient(null, "CalamityDust", 3);
            recipe.AddIngredient(null, "LivingShard", 3);
            recipe.AddIngredient(ItemID.BrokenHeroSword);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 173);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.ShadowFlame, 300);
            target.AddBuff(BuffID.OnFire, 300);
            if (crit)
            {
                target.AddBuff(BuffID.ShadowFlame, 900);
                target.AddBuff(BuffID.OnFire, 900);
                player.ApplyDamageToNPC(target, damage * 3, 0f, 0, false);
                float num50 = 1.7f;
                float num51 = 0.8f;
                float num52 = 2f;
                Vector2 value3 = (target.rotation - 1.57079637f).ToRotationVector2();
                Vector2 value4 = value3 * target.velocity.Length();
                Main.PlaySound(SoundID.Item14, target.position);
                int num3;
                for (int num53 = 0; num53 < 40; num53 = num3 + 1)
                {
                    int num54 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 173, 0f, 0f, 200, default, num50);
                    Main.dust[num54].position = target.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)target.width / 2f;
                    Main.dust[num54].noGravity = true;
                    Dust dust = Main.dust[num54];
                    dust.velocity.Y -= 4.5f;
                    dust.velocity *= 3f;
                    dust = Main.dust[num54];
                    dust.velocity += value4 * Main.rand.NextFloat();
                    num54 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 173, 0f, 0f, 100, default, num51);
                    Main.dust[num54].position = target.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)target.width / 2f;
                    dust = Main.dust[num54];
                    dust.velocity.Y -= 3f;
                    dust.velocity *= 2f;
                    Main.dust[num54].noGravity = true;
                    Main.dust[num54].fadeIn = 1f;
                    Main.dust[num54].color = Color.Crimson * 0.5f;
                    dust = Main.dust[num54];
                    dust.velocity += value4 * Main.rand.NextFloat();
                    num3 = num53;
                }
                for (int num55 = 0; num55 < 20; num55 = num3 + 1)
                {
                    int num56 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 173, 0f, 0f, 0, default, num52);
                    Main.dust[num56].position = target.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)target.velocity.ToRotation(), default) * (float)target.width / 3f;
                    Main.dust[num56].noGravity = true;
                    Dust dust = Main.dust[num56];
                    dust.velocity.Y -= 1.5f;
                    dust.velocity *= 0.5f;
                    dust = Main.dust[num56];
                    dust.velocity += value4 * (0.6f + 0.6f * Main.rand.NextFloat());
                    num3 = num55;
                }
            }
        }
    }
}
