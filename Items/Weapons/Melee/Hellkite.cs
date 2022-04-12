using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Hellkite : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hellkite");
            Tooltip.SetDefault("Contains the power of an ancient drake\n" +
                "Summons flame geyser explosions on enemy hits");
        }

        public override void SetDefaults()
        {
            Item.width = 84;
            Item.damage = 180;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 30;
            Item.useTurn = true;
            Item.knockBack = 8f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 84;
            Item.value = Item.buyPrice(0, 60, 0, 0);
            Item.rare = ItemRarityID.Lime;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 174);
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
            player.ApplyDamageToNPC(target, (int)(Item.damage * (player.GetDamage<GenericDamageClass>().Base + player.GetDamage(DamageClass.Melee).Base - 1f)), 0f, 0, false);
            float num50 = 1.7f;
            float num51 = 0.8f;
            float num52 = 2f;
            Vector2 value3 = (target.rotation - 1.57079637f).ToRotationVector2();
            Vector2 value4 = value3 * target.velocity.Length();
            SoundEngine.PlaySound(SoundID.Item14, target.position);
            int num3;
            for (int num53 = 0; num53 < 40; num53 = num3 + 1)
            {
                int num54 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 174, 0f, 0f, 200, default, num50);
                Dust dust = Main.dust[num54];
                dust.position = target.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)target.width / 2f;
                dust.noGravity = true;
                dust.velocity.Y -= 6f;
                dust.velocity *= 3f;
                dust.velocity += value4 * Main.rand.NextFloat();
                num54 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 174, 0f, 0f, 100, default, num51);
                dust.position = target.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)target.width / 2f;
                dust.velocity.Y -= 6f;
                dust.velocity *= 2f;
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.color = Color.Crimson * 0.5f;
                dust.velocity += value4 * Main.rand.NextFloat();
                num3 = num53;
            }
            for (int num55 = 0; num55 < 20; num55 = num3 + 1)
            {
                int num56 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 174, 0f, 0f, 0, default, num52);
                Dust dust = Main.dust[num56];
                dust.position = target.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)target.velocity.ToRotation(), default) * (float)target.width / 3f;
                dust.noGravity = true;
                dust.velocity.Y -= 6f;
                dust.velocity *= 0.5f;
                dust.velocity += value4 * (0.6f + 0.6f * Main.rand.NextFloat());
                num3 = num55;
            }
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
            SoundEngine.PlaySound(SoundID.Item14, target.position);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FieryGreatsword).
                AddIngredient<DraedonBar>(8).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
