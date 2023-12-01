using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.NPCs.NormalNPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Items.Weapons.Rogue;

namespace CalamityMod.Projectiles.Rogue
{
    public class SearedPanProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        internal enum SearedPanTypes
        {
            VenLocket = 0,
            Normal = 1,
            Golden = 2,
            StealthStrike = 3
        }

        internal SearedPanTypes PanType
        {
            get => (SearedPanTypes)(int)Projectile.ai[0];
            set => Projectile.ai[0] = (int)value;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 420;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 5, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Don't increment the Seared Pan counter when hitting dummies
             //&& target.type != ModContent.NPCType<SuperDummyNPC>() this is here just in case its needed again
            bool dummy = target.type != NPCID.TargetDummy;
            OnHitEffects(target.whoAmI, target.life, dummy);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Bleeding, 300);
            OnHitEffects(-1, target.statLife, true);
        }

        private void OnHitEffects(int targetIndex, int health, bool specialEffects)
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            // Don't spawn fireballs or increment the special effects counter if you can't even stealth strike
            bool playerCanStealthStrike = modPlayer.wearingRogueArmor && modPlayer.rogueStealthMax > 0;
            if (!playerCanStealthStrike)
                return;

            // Increment the seared pan counter. Pans summoned via the Venerated Locket shouldn't increment the counter.
            // See CalamityPlayerMiscEffects.cs for code that resets the counter after 40 frames
            modPlayer.searedPanTimer = 0;
            if (specialEffects && PanType != SearedPanTypes.VenLocket)
                modPlayer.searedPanCounter++;

            // Pans summoned via the Venerated Locket have zero special effects
            if (PanType == SearedPanTypes.StealthStrike)
            {
                modPlayer.searedPanCounter = 0;
                // Stealth strikes spawn four golden sparks on hit
                for (int t = 0; t < 4; t++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<PanSpark>(), (int)(Projectile.damage * 0.2), 0f, Projectile.owner);
                }
                SoundEngine.PlaySound(SearedPan.SmashSound, Projectile.position);
                // Stealth strikes also cause any existing fireballs to home in on their targets
                FireballStuff(true);

                if (!specialEffects)
                    return;
                // Stealth strikes then summon five fireballs to circle the hit enemy, they will home in on their own after a second.
                for (int t = 0; t < 5; t++)
                {
                    int i = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<NiceCock>(), (int)(Projectile.damage * 0.1), 0f, Projectile.owner, 0f, targetIndex);
                    Main.projectile[i].ModProjectile<NiceCock>().Timer = 61;
                }
                FireballPositions(targetIndex);
            }
            else if (PanType == SearedPanTypes.Golden)
            {
                modPlayer.searedPanCounter = 0;
                SoundEngine.PlaySound(SearedPan.SmashSound, Projectile.position);
                // Golden pans simply cause all fireballs to home in on their targets
                FireballStuff(true);
            }
            else if (PanType == SearedPanTypes.Normal && targetIndex != -1 && health > 0 && specialEffects)
            {
                // Summon three fireballs to circle the hit enemy
                for (int t = 0; t < 3; t++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<NiceCock>(), (int)(Projectile.damage * 0.1), 0f, Projectile.owner, 0f, targetIndex);
                }
                FireballPositions(targetIndex);
            }
        }

        private void FireballStuff(bool activate)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (!Main.projectile[i].active || Main.projectile[i].owner != Projectile.owner)
                        continue;
                    if (Main.projectile[i].ModProjectile is NiceCock)
                    {
                        if (!activate)
                            Main.projectile[i].Kill();
                        else
                        {
                            Main.projectile[i].ModProjectile<NiceCock>().homing = true;
                            Main.projectile[i].extraUpdates += 2;
                        }
                    }
                }
            }
        }

        private void FireballPositions(int targetIndex)
        {
            int fireballCount = 0;
            // Count how many fireballs exist already around the given target
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                // Keep the loop as short as possible
                if (!Main.projectile[i].active || Main.projectile[i].owner != Projectile.owner || !Main.projectile[i].CountsAsClass<ThrowingDamageClass>() || targetIndex != (int)Main.projectile[i].ai[1])
                    continue;
                if (Main.projectile[i].ModProjectile is NiceCock)
                {
                    if (Main.projectile[i].ModProjectile<NiceCock>().homing)
                        continue;
                    fireballCount++;
                }
            }
            // Adjust the angle of the existing fireballs around a target
            float angleVariance = MathHelper.TwoPi / fireballCount;
            float angle = 0f;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (!Main.projectile[i].active || Main.projectile[i].owner != Projectile.owner || !Main.projectile[i].CountsAsClass<ThrowingDamageClass>() || targetIndex != (int)Main.projectile[i].ai[1])
                    continue;
                if (Main.projectile[i].ModProjectile is NiceCock)
                {
                    if (Main.projectile[i].ModProjectile<NiceCock>().homing)
                        continue;
                    Main.projectile[i].ai[0] = angle;
                    Main.projectile[i].netUpdate = true;
                    angle += angleVariance;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Kill all fireballs if you miss a stealth strike or golden pan
            if (PanType == SearedPanTypes.Golden || PanType == SearedPanTypes.StealthStrike)
                FireballStuff(false);
            return true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            // Stealth strikes and golden pans are golden colored
            if (PanType == SearedPanTypes.Golden || PanType == SearedPanTypes.StealthStrike)
                return new Color(255, 222, 0);
            return null;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
