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
            get => (TaserAIState)(int)projectile.ai[0];
            set => projectile.ai[0] = (int)value;
        }

        public float Time
        {
            get => projectile.ai[1];
            set => projectile.ai[1] = value;
        }

        public int ElectrocutionTarget
        {
            get => (int)projectile.localAI[0];
            set => projectile.localAI[0] = value;
        }

        public const float ReelbackSpeed = 15f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Taser");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.penetrate = 2;
            projectile.tileCollide = true;
            projectile.ownerHitCheck = true;
            projectile.ranged = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 6;
        }

        public override void AI()
        {
            Time++;
            Player player = Main.player[projectile.owner];
            switch (AIState)
            {
                case TaserAIState.Firing:
                    float distanceFromPlayer = projectile.Distance(player.Center);
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

                    projectile.Center = Main.npc[ElectrocutionTarget].Center;
                    break;
                case TaserAIState.ReelingBack:
                    // Kill the gun and the hook if the hook has returned to the gun.
                    if (projectile.Hitbox.Intersects(player.Hitbox))
                    {
                        projectile.Kill();
                        return;
                    }
                    projectile.tileCollide = false;
                    projectile.velocity = projectile.SafeDirectionTo(player.Center) * ReelbackSpeed;
                    break;
            }

            projectile.rotation = projectile.AngleFrom(player.Center);

            ManipulatePlayerItemValues(player);
        }


        public void ManipulatePlayerItemValues(Player player)
        {
            player.ChangeDir((player.Center.X - projectile.Center.X < 0).ToDirectionInt());
            player.itemRotation = CalamityUtils.WrapAngle90Degrees(projectile.rotation);
            player.itemTime = 4;
            player.itemAnimation = 4;
        }

        public void GoToAIState(TaserAIState newAIState)
        {
            // Don't waste the resources changing the AI state if the projectile is already in said state.
            if (AIState == newAIState)
                return;

            projectile.penetrate = -1;
            AIState = newAIState;
            projectile.netUpdate = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Player player = Main.player[projectile.owner];
            Texture2D texture = ModContent.GetTexture(Texture);
            Utils.DrawLine(spriteBatch, player.MountedCenter, projectile.Center, Color.Cyan, Color.White, 4f);
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation, texture.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0f);
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
