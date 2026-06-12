using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class PalladiumJavelinProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/PalladiumJavelin";

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.aiStyle = ProjAIStyleID.StickProjectile;
            Projectile.timeLeft = 600;
            Projectile.MaxUpdates = 2;
            AIType = ProjectileID.BoneJavelin;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.localNPCHitCooldown = 15 * Projectile.MaxUpdates;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            //Stealth strike behavior
            if (!Projectile.Calamity().stealthStrike || Projectile.owner != Main.myPlayer || Projectile.ai[2] > 0)
                return;
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] == 12f)
            {
                Vector2 vector2 = new Vector2(20f, 20f);
                for (int index1 = 0; index1 < 10; ++index1)
                {
                    int index2 = Dust.NewDust(Projectile.Center - vector2 / 2f, (int)vector2.X, (int)vector2.Y, DustID.GemTopaz, 0f, 0f, 100, new Color(), 1.5f);
                    Dust dust = Main.dust[index2];
                    dust.velocity *= 1.4f;
                }
                for (int index1 = 0; index1 < 5; ++index1)
                {
                    Dust.NewDust(Projectile.Center - vector2 / 2f, (int)vector2.X, (int)vector2.Y, DustID.Palladium, 0f, 0f, 0, default, 1f);
                }

                int javelin = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.ToRadians(3f)), Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, ai2: 1);
                int javelin2 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.ToRadians(-3f)), Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, ai2: 1);
                if (javelin.WithinBounds(Main.maxProjectiles))
                    Main.projectile[javelin].Calamity().stealthStrike = true;
                if (javelin2.WithinBounds(Main.maxProjectiles))
                    Main.projectile[javelin2].Calamity().stealthStrike = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => OnHitEffects();
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => OnHitEffects();

        public void OnHitEffects()
        {
            if (Projectile.owner == Main.myPlayer && Projectile.Calamity().stealthStrike)
                Main.player[Projectile.owner].AddBuff(BuffID.RapidHealing, 300);
        }
    }
}
