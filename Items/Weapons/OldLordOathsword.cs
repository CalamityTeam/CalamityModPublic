using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class OldLordOathsword : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Old Lord Oathsword");
			Tooltip.SetDefault("A relic of the ancient underworld\n" +
				"Critical hits cause lava explosions");
		}

		public override void SetDefaults()
		{
			item.damage = 34;
			item.width = 78;
			item.height = 78;
			item.melee = true;
			item.useAnimation = 24;
			item.useStyle = 1;
			item.useTime = 24;
			item.useTurn = true;
			item.knockBack = 7f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
		}

		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
		{
			if (crit)
			{
				target.AddBuff(BuffID.OnFire, 600, false);
				player.ApplyDamageToNPC(target, damage, 0f, 0, false);
				float num50 = 1.7f;
				float num51 = 0.8f;
				float num52 = 2f;
				Vector2 value3 = (target.rotation - 1.57079637f).ToRotationVector2();
				Vector2 value4 = value3 * target.velocity.Length();
				Main.PlaySound(SoundID.Item14, target.position);
				int num3;
				for (int num53 = 0; num53 < 40; num53 = num3 + 1)
				{
					int num54 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 127, 0f, 0f, 200, default, num50);
					Main.dust[num54].position = target.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)target.width / 2f;
					Main.dust[num54].noGravity = true;
					Dust dust = Main.dust[num54];
					dust.velocity *= 3f;
					dust = Main.dust[num54];
					dust.velocity += value4 * Main.rand.NextFloat();
					num54 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 127, 0f, 0f, 100, default, num51);
					Main.dust[num54].position = target.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)target.width / 2f;
					dust = Main.dust[num54];
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
					int num56 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 127, 0f, 0f, 0, default, num52);
					Main.dust[num56].position = target.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)target.velocity.ToRotation(), default) * (float)target.width / 3f;
					Main.dust[num56].noGravity = true;
					Dust dust = Main.dust[num56];
					dust.velocity *= 0.5f;
					dust = Main.dust[num56];
					dust.velocity += value4 * (0.6f + 0.6f * Main.rand.NextFloat());
					num3 = num55;
				}
			}
		}
	}
}
