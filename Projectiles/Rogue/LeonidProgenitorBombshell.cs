using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class LeonidProgenitorBombshell : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/LeonidProgenitor";

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            int randomDust = Utils.SelectRandom(Main.rand, new int[]
            {
                ModContent.DustType<AstralOrange>(),
                ModContent.DustType<AstralBlue>()
            });
            int astral = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDust, 0f, 0f, 100, CalamityUtils.ColorSwap(LeonidProgenitor.blueColor, LeonidProgenitor.purpleColor, 1f), 0.8f);
            Main.dust[astral].noGravity = true;
            Main.dust[astral].velocity *= 0f;

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 10f)
            {
                Projectile.ai[0] = 10f;
                if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X * 0.97f;
                    if (Projectile.velocity.X > -0.01f && Projectile.velocity.X < 0.01f)
                    {
                        Projectile.velocity.X = 0f;
                        Projectile.netUpdate = true;
                    }
                }
                Projectile.velocity.Y += 0.2f;
            }
            Projectile.rotation += Projectile.velocity.X * 0.1f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Rogue/LeonidProgenitorGlow").Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item62, Projectile.position);
            if (Main.myPlayer == Projectile.owner)
            {
                int flash = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Flash>(), Projectile.damage, 0f, Projectile.owner, 0f, 1f);
                if (flash.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[flash].DamageType = RogueDamageClass.Instance;
                    Main.projectile[flash].usesLocalNPCImmunity = true;
                    Main.projectile[flash].localNPCHitCooldown = 10;
                }

                Vector2 pos = new Vector2(Projectile.Center.X + Projectile.width * 0.5f + Main.rand.Next(-201, 201), Main.screenPosition.Y - 600f - Main.rand.Next(50));
                Vector2 velocity = (Projectile.Center - pos) / 40f;
                int dmg = Projectile.damage / 2;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, velocity, ModContent.ProjectileType<LeonidCometBig>(), dmg, Projectile.knockBack, Projectile.owner, 0f, 0.5f + (float)Main.rand.NextDouble() * 0.3f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240);
            StealthStrikeEffect();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240);
            StealthStrikeEffect();
        }

        private void StealthStrikeEffect()
        {
            if (!Projectile.Calamity().stealthStrike || Main.myPlayer != Projectile.owner)
                return;

            Vector2 spinningpoint = new Vector2(0f, 6f);
            float radian45 = MathHelper.ToRadians(45f);
            int cometAmt = 5;
            float cometSpread = -(radian45 * 2f) / (cometAmt - 1f);
            for (int projIndex = 0; projIndex < cometAmt; ++projIndex)
            {
                int index2 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, spinningpoint.RotatedBy((double)radian45 + (double)cometSpread * (double)projIndex, new Vector2()), ModContent.ProjectileType<LeonidCometSmall>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, -1f);
                Projectile proj = Main.projectile[index2];
                for (int index3 = 0; index3 < Projectile.localNPCImmunity.Length; ++index3)
                {
                    proj.localNPCImmunity[index3] = Projectile.localNPCImmunity[index3];
                }
            }
        }
    }
}
