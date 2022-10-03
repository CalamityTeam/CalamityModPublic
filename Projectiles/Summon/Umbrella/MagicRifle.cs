using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon.Umbrella
{
    public class MagicRifle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rifle");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 0f;
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            //Set player namespace
            Player player = Main.player[Projectile.owner];

			if (player.Calamity().magicHat)
			{
				Projectile.timeLeft = 2;
			}

			const float outwardPosition = 180f;
			Projectile.Center = player.Center + Projectile.ai[0].ToRotationVector2() * outwardPosition;
			Projectile.ai[0] -= MathHelper.ToRadians(4f);

            float homingRange = MagicHat.Range;
            Vector2 targetVec = Projectile.position;
            bool foundTarget = false;
            //If targeting something, prioritize that enemy
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float extraDist = (npc.width / 2) + (npc.height / 2);
                    //Calculate distance between target and the projectile to know if it's too far or not
                    float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
                    if (!foundTarget && targetDist < (homingRange + extraDist))
                    {
                        homingRange = targetDist;
                        targetVec = npc.Center;
                        foundTarget = true;
                    }
                }
            }
            if (!foundTarget)
            {
                for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
                {
                    NPC npc = Main.npc[npcIndex];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float extraDist = (npc.width / 2) + (npc.height / 2);
                        //Calculate distance between target and the projectile to know if it's too far or not
                        float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
                        if (!foundTarget && targetDist < (homingRange + extraDist))
                        {
                            homingRange = targetDist;
                            targetVec = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }

            //Update rotation
            if (foundTarget)
            {
                Projectile.spriteDirection = Projectile.direction = ((targetVec.X - Projectile.Center.X) > 0).ToDirectionInt();
                Projectile.rotation = Projectile.rotation.AngleTowards(Projectile.AngleTo(targetVec) + (Projectile.spriteDirection == 1 ? MathHelper.ToRadians(45) : MathHelper.ToRadians(135)), 0.1f);
            }
			else
			{
				Projectile.rotation = Projectile.ai[0] + MathHelper.PiOver4;
			}

            //Increment attack cooldown
            if (Projectile.ai[1] > 0f)
            {
                Projectile.ai[1] += 1f;
            }
            //Set the minion to be ready for attack
            if (Projectile.ai[1] > 45f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }

            //Return if on attack cooldown, has no target
            if (Projectile.ai[1] != 0f || !foundTarget)
                return;

            //Shoot a bullet
            if (Main.myPlayer == Projectile.owner)
            {
                float projSpeed = 6f;
                int projType = ModContent.ProjectileType<MagicBullet>();
                if (Main.rand.NextBool(6))
                {
                    SoundEngine.PlaySound(SoundID.Item20 with { Volume = SoundID.Item20.Volume * 0.1f}, Projectile.position);
                }
                Projectile.ai[1] += 1f;
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 velocity = targetVec - Projectile.Center;
                    velocity.Normalize();
                    velocity *= projSpeed;
                    SoundEngine.PlaySound(SoundID.Item40, Projectile.position);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, projType, Projectile.damage, 0f, Projectile.owner);
                    Projectile.netUpdate = true;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(148, 0, 211, Projectile.alpha);

        public override bool? CanDamage() => false;

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 dspeed = new Vector2(Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-7f, 7f));
                int dust = Dust.NewDust(Projectile.Center, 1, 1, 66, dspeed.X, dspeed.Y, 160, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 0.75f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
