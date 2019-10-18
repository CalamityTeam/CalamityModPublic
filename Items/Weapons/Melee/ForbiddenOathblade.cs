using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Melee
{
    public class ForbiddenOathblade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Forbidden Oathblade");
            Tooltip.SetDefault("Fires a demonic scythe and critical hits cause shadowflame explosions");
        }

        public override void SetDefaults()
        {
            item.width = 76;
            item.damage = 64;
            item.melee = true;
            item.useAnimation = 25;
            item.useTime = 25;
            item.useTurn = true;
            item.useStyle = 1;
            item.knockBack = 6.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 76;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.shoot = ModContent.ProjectileType<Oathblade>();
            item.shootSpeed = 3f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BladecrestOathsword");
            recipe.AddIngredient(null, "OldLordOathsword");
            recipe.AddIngredient(ItemID.SoulofFright, 5);
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
            target.AddBuff(BuffID.ShadowFlame, 240);
            target.AddBuff(BuffID.OnFire, 240);
            if (crit)
            {
                target.AddBuff(BuffID.ShadowFlame, 720);
                target.AddBuff(BuffID.OnFire, 720);
                player.ApplyDamageToNPC(target, damage * 2, 0f, 0, false);
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
