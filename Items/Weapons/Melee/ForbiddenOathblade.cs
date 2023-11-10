using CalamityMod.Projectiles.Melee;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Melee
{
    public class ForbiddenOathblade : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 74;
            Item.height = 74;
            Item.scale = 1.5f;
            Item.damage = 110;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 55;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<ForbiddenOathbladeProjectile>();
            Item.shootSpeed = 10f;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 173);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 120);
            target.AddBuff(BuffID.OnFire, 240);
            if (hit.Crit)
            {
                target.AddBuff(BuffID.ShadowFlame, 360);
                target.AddBuff(BuffID.OnFire, 720);
                int onHitDamage = player.CalcIntDamage<MeleeDamageClass>(2 * Item.damage);
                player.ApplyDamageToNPC(target, onHitDamage, 0f, 0, false);
                float firstDustScale = 1.7f;
                float secondDustScale = 0.8f;
                float thirdDustScale = 2f;
                Vector2 dustRotation = (target.rotation - 1.57079637f).ToRotationVector2();
                Vector2 dustVelocity = dustRotation * target.velocity.Length();
                SoundEngine.PlaySound(SoundID.Item14, target.Center);
                int dustIncr;
                for (int i = 0; i < 40; i = dustIncr + 1)
                {
                    int swingDust = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 173, 0f, 0f, 200, default, firstDustScale);
                    Dust dust = Main.dust[swingDust];
                    dust.position = target.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)target.width / 2f;
                    dust.noGravity = true;
                    dust.velocity.Y -= 4.5f;
                    dust.velocity *= 3f;
                    dust.velocity += dustVelocity * Main.rand.NextFloat();
                    swingDust = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 173, 0f, 0f, 100, default, secondDustScale);
                    dust.position = target.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)target.width / 2f;
                    dust.velocity.Y -= 3f;
                    dust.velocity *= 2f;
                    dust.noGravity = true;
                    dust.fadeIn = 1f;
                    dust.color = Color.Crimson * 0.5f;
                    dust.velocity += dustVelocity * Main.rand.NextFloat();
                    dustIncr = i;
                }
                for (int j = 0; j < 20; j = dustIncr + 1)
                {
                    int swingDust2 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 173, 0f, 0f, 0, default, thirdDustScale);
                    Dust dust = Main.dust[swingDust2];
                    dust.position = target.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)target.velocity.ToRotation(), default) * (float)target.width / 3f;
                    dust.noGravity = true;
                    dust.velocity.Y -= 1.5f;
                    dust.velocity *= 0.5f;
                    dust.velocity += dustVelocity * (0.6f + 0.6f * Main.rand.NextFloat());
                    dustIncr = j;
                }
            }
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<Shadowflame>(), 360);
            target.AddBuff(BuffID.OnFire, 720);
            SoundEngine.PlaySound(SoundID.Item14, target.Center);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BladecrestOathsword>().
                AddIngredient<OldLordClaymore>().
                AddIngredient(ItemID.SoulofFright, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
