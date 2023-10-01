using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Carnage : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.damage = 130;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 21;
            Item.knockBack = 5.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.height = 54;
            Item.scale = 1.25f;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            // Spawn occasional blood dust
            if (Main.rand.NextBool(3))
                Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, DustID.Blood);
        }

        // Carnage's on-hits only occur on valid enemies. Specifically won't trigger on statues.
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life > 0 || !target.IsAnEnemy(false))
                return;
            OnHitEffects(player, target, hit.Knockback);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            if (target.statLife > 0)
                return;
            OnHitEffects(player, target, Item.knockBack);
        }

        private void OnHitEffects(Player player, Entity target, float kb)
        {
            var source = player.GetSource_ItemUse(Item);

            // Play sound
            SoundEngine.PlaySound(SoundID.Item74, target.Center);

            // Dust loop 1
            for (int i = 0; i < 15; i++)
            {
                int idx = Dust.NewDust(target.position, target.width, target.height, DustID.Blood, 0f, 0f, 100, default, 2f);
                Main.dust[idx].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[idx].scale = 0.5f;
                    Main.dust[idx].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }

            // Dust loop 2
            for (int i = 0; i < 25; i++)
            {
                int idx = Dust.NewDust(target.position, target.width, target.height, DustID.Blood, 0f, 0f, 100, default, 3f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 5f;
                idx = Dust.NewDust(target.position, target.width, target.height, DustID.Blood, 0f, 0f, 100, default, 2f);
                Main.dust[idx].velocity *= 2f;
            }

            // 6-8 blood projectiles spawned on kill
            int bloodAmt = Main.rand.Next(6, 9);
            int bloodDamage = player.CalcIntDamage<MeleeDamageClass>(0.3f * Item.damage);
            for (int i = 0; i < bloodAmt; i++)
            {
                Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                Projectile.NewProjectile(source, target.Center, velocity, ModContent.ProjectileType<Blood>(), bloodDamage, kb, player.whoAmI, 0f, 0f);
            }
        }
    }
}
