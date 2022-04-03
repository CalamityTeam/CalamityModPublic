using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class TaserHook : ModProjectile
    {
        public enum TaserAIState
        {
            Firing,
            Electrocuting,
            ReelingBack
        }

        public TaserAIState AIState
        {
            get => (TaserAIState)(int)Projectile.ai[0];
            set => Projectile.ai[0] = (int)value;
        }

        public float Time
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public int ElectrocutionTarget
        {
            get => (int)Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }

        public const float ReelbackSpeed = 15f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Taser");
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.tileCollide = true;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
        }

        public override void AI()
        {
            Time++;
            Player player = Main.player[Projectile.owner];
            switch (AIState)
            {
                case TaserAIState.Firing:
                    float distanceFromPlayer = Projectile.Distance(player.Center);
                    if (distanceFromPlayer > 600f || Time >= 90f)
                        GoToAIState(TaserAIState.ReelingBack);
                    break;
                case TaserAIState.Electrocuting:
                    // Reel back to the player if the target has been killed or a second has passed.
                    if (!Main.npc[ElectrocutionTarget].active || Time >= 60f)
                    {
                        GoToAIState(TaserAIState.ReelingBack);
                        return;
                    }

                    Projectile.Center = Main.npc[ElectrocutionTarget].Center;
                    break;
                case TaserAIState.ReelingBack:
                    // Kill the gun and the hook if the hook has returned to the gun.
                    if (Projectile.Hitbox.Intersects(player.Hitbox))
                    {
                        Projectile.Kill();
                        return;
                    }
                    Projectile.tileCollide = false;
                    Projectile.velocity = Projectile.SafeDirectionTo(player.Center) * ReelbackSpeed;
                    break;
            }

            Projectile.rotation = Projectile.AngleFrom(player.Center);

            ManipulatePlayerItemValues(player);
        }


        public void ManipulatePlayerItemValues(Player player)
        {
            player.ChangeDir((player.Center.X - Projectile.Center.X < 0).ToDirectionInt());
            player.itemRotation = CalamityUtils.WrapAngle90Degrees(Projectile.rotation);
            player.itemTime = 4;
            player.itemAnimation = 4;
        }

        public void GoToAIState(TaserAIState newAIState)
        {
            // Don't waste the resources changing the AI state if the projectile is already in said state.
            if (AIState == newAIState)
                return;

            Projectile.penetrate = -1;
            AIState = newAIState;
            Projectile.netUpdate = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D texture = ModContent.Request<Texture2D>(Texture);
            Utils.DrawLine(spriteBatch, player.MountedCenter, Projectile.Center, Color.Cyan, Color.White, 4f);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 120);

            if (AIState == TaserAIState.Firing)
            {
                if (!Main.dedServ)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        float angle = MathHelper.TwoPi / 50f * i + Utils.InverseLerp(90f, 150f, Time, true) * MathHelper.ToRadians(1080f);
                        Dust dust = Dust.NewDustPerfect(target.Center + angle.ToRotationVector2() * 10f, 226);
                        dust.velocity = Vector2.Zero;
                        if (Main.rand.NextBool(6))
                            dust.velocity = target.SafeDirectionTo(dust.position) * 4.5f;

                        dust.noGravity = true;
                    }
                }
                ElectrocutionTarget = target.whoAmI;
                Time = 0f;
                GoToAIState(TaserAIState.Electrocuting);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            GoToAIState(TaserAIState.ReelingBack);
            return false;
        }
    }
}
