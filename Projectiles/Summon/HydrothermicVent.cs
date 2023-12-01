using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class HydrothermicVent : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public int dust = 3;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 34;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            bool isMinion = Projectile.type == ModContent.ProjectileType<HydrothermicVent>();
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (!modPlayer.chaosSpirit)
            {
                Projectile.active = false;
                return;
            }
            if (isMinion)
            {
                if (player.dead)
                {
                    modPlayer.cSpirit = false;
                }
                if (modPlayer.cSpirit)
                {
                    Projectile.timeLeft = 2;
                }
            }
            dust--;
            if (dust >= 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 16f), Projectile.width, Projectile.height - 16, Main.rand.NextBool(3) ? 16 : 127, 0f, 0f, 0, default, 1f);
                    Main.dust[dust].velocity *= 2f;
                    Main.dust[dust].scale *= 1.15f;
                }
            }
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 1f / 255f, (255 - Projectile.alpha) * 0.35f / 255f, (255 - Projectile.alpha) * 0f / 255f);
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 9)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
            bool reversedGravity = player.gravDir == -1f;
            Projectile.position.X = player.Center.X - (float)(Projectile.width / 2);
            Projectile.position.Y = player.Center.Y - (float)(Projectile.height / 2) + player.gfxOffY - 60f;
            if (reversedGravity)
            {
                Projectile.position.Y += 120f;
                Projectile.rotation = MathHelper.Pi;
            }
            else
            {
                Projectile.rotation = 0f;
            }
            Projectile.position.X = (float)(int)Projectile.position.X;
            Projectile.position.Y = (float)(int)Projectile.position.Y;

            if (Projectile.owner == Main.myPlayer)
            {
                if (Projectile.ai[0] != 0f)
                {
                    Projectile.ai[0] -= 1f;
                    return;
                }
                bool foundTarget = false;
                Vector2 targetVec = Projectile.Center;
                Vector2 half = new Vector2(0.5f);
                float range = 1000f;
                int targetIndex = -1;
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        Vector2 sizeCheck = npc.position + npc.Size * half;
                        float npcDist = Vector2.Distance(sizeCheck, targetVec);
                        if (npcDist < range && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                        {
                            range = npcDist;
                            targetVec = sizeCheck;
                            foundTarget = true;
                            targetIndex = npc.whoAmI;
                        }
                    }
                }
                if (!foundTarget)
                {
                    for (int k = 0; k < Main.maxNPCs; k++)
                    {
                        NPC npc = Main.npc[k];
                        if (npc.CanBeChasedBy(Projectile, false))
                        {
                            Vector2 sizeCheck = npc.position + npc.Size * half;
                            float npcDist = Vector2.Distance(sizeCheck, targetVec);
                            if (npcDist < range && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                            {
                                range = npcDist;
                                targetVec = sizeCheck;
                                foundTarget = true;
                                targetIndex = k;
                            }
                        }
                    }
                }
                float yAdjust = player.gravDir == -1f ? 0f : 10f;
                if (foundTarget && targetIndex != -1)
                {
                    int projectileType = ModContent.ProjectileType<VolcanicFireballSummon>();
                    //If the target is above the minion, fire directly at it at double speed
                    if (reversedGravity ? (Main.npc[targetIndex].Bottom.Y > Projectile.Top.Y) : (Main.npc[targetIndex].Bottom.Y < Projectile.Top.Y))
                    {
                        Vector2 source = new Vector2(Projectile.Center.X - 4f, Projectile.Center.Y - yAdjust);
                        float speed = Main.rand.Next(14, 19); //modify the speed the projectile are shot.  Lower number = slower projectile.
                        Vector2 velocity = targetVec - Projectile.Center;
                        float targetDist = velocity.Length();
                        targetDist = speed / targetDist;
                        velocity.X *= targetDist;
                        velocity.Y *= targetDist;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), source, velocity, projectileType, Projectile.damage, 5f, Projectile.owner, 0f, 0f);
                        SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
                        Projectile.ai[0] = 10f;
                    }
                    //Otherwise, fire away like a volcano
                    else
                    {
                        int amount = Main.rand.Next(2, 4); //2 to 3
                        for (int i = 0; i < amount; i++)
                        {
                            Vector2 velocity = new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-10f, -7f));
                            if (reversedGravity)
                                velocity.Y *= -1f;
                            Projectile flame = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.oldPosition + Projectile.Size * 0.5f, velocity, projectileType, Projectile.damage, Projectile.knockBack, Projectile.owner);
                            flame.aiStyle = ProjAIStyleID.Arrow;
                        }
                        SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
                        Projectile.ai[0] = 20f;
                    }
                }

                //doesn't look good
                /*Vector2 goreVec = new Vector2(projectile.Center.X, projectile.Center.Y - yAdjust - 10f);
                Vector2 goreVec = new Vector2(projectile.Center.X + projectile.velocity.X, projectile.Center.Y + projectile.velocity.Y);
                if (Main.rand.NextBool(8))
                {
                    int smoke = Gore.NewGore(goreVec, default, Main.rand.Next(375, 378), 0.5f);
                    Main.gore[smoke].behindTiles = true;
                }*/
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 200);

        public override bool? CanDamage() => false;
    }
}
