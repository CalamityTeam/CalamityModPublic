using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class Shellfish : ModProjectile
    {
        private int playerStill = 0;
        private bool fly = false;
        private bool spawnDust = true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shellfish");
            Main.projFrames[projectile.type] = 2;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 24;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.minionSlots = 2;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = false;
            return true;
        }

        public override void AI()
        {
            if (spawnDust)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = Main.player[projectile.owner].minionDamage;
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int num501 = 20;
                for (int num502 = 0; num502 < num501; num502++)
                {
                    int num503 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 16f), projectile.width, projectile.height - 16, 33, 0f, 0f, 0, default, 1f);
                    Main.dust[num503].velocity *= 2f;
                    Main.dust[num503].scale *= 1.15f;
                }
                spawnDust = false;
            }
            bool flag64 = projectile.type == ModContent.ProjectileType<Shellfish>();
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            player.AddBuff(ModContent.BuffType<Shellfish>(), 3600);
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.shellfish = false;
                }
                if (modPlayer.shellfish)
                {
                    projectile.timeLeft = 2;
                }
            }
            projectile.frameCounter++;
            if (projectile.frameCounter > 3)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 1)
            {
                projectile.frame = 0;
            }
            if (projectile.ai[0] == 0f)
            {
                projectile.damage = 70;
                if (Main.player[projectile.owner].minionDamage != projectile.Calamity().spawnedPlayerMinionDamageValue)
                {
                    int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                        projectile.Calamity().spawnedPlayerMinionDamageValue *
                        Main.player[projectile.owner].minionDamage);
                    projectile.damage = damage2;
                }
                float[] var0 = projectile.ai;
                int var1 = 1;
                float num73 = var0[var1];
                var0[var1] = num73 + 1f;
                Vector2 vector46 = projectile.position;
                if (!fly)
                {
                    projectile.tileCollide = true;
                    Vector2 center2 = projectile.Center;
                    Vector2 vector48 = player.Center - center2;
                    float playerDistance = vector48.Length();
                    if (projectile.velocity.Y == 0f && (projectile.velocity.X != 0f || playerDistance > 200f))
                    {
                        switch (Main.rand.Next(1, 3))
                        {

                            case 1:
                                projectile.velocity.Y -= 5f;
                                break;

                            case 2:
                                projectile.velocity.Y -= 7.5f;
                                break;

                            case 3:
                                projectile.velocity.Y -= 10f;
                                break;
                        }
                    }
                    projectile.velocity.Y += 0.3f;
                    float num633 = 1000f;
                    bool chaseNPC = false;
                    float npcPositionX = 0f;
                    for (int num645 = 0; num645 < 200; num645++)
                    {
                        NPC npcTarget = Main.npc[num645];
                        if (npcTarget.CanBeChasedBy(projectile, false))
                        {
                            float num646 = Vector2.Distance(npcTarget.Center, projectile.Center);
                            if ((Vector2.Distance(projectile.Center, vector46) > num646 && num646 < num633) || !chaseNPC)
                            {
                                num633 = num646;
                                vector46 = npcTarget.Center;
                                npcPositionX = npcTarget.position.X;
                                chaseNPC = true;
                            }
                        }
                    }
                    if (chaseNPC)
                    {
                        if (npcPositionX - projectile.position.X > 0f)
                        {
                            switch (Main.rand.Next(1, 2))
                            {

                                case 1:
                                    projectile.velocity.X += 0.15f;
                                    break;

                                case 2:
                                    projectile.velocity.X += 0.20f;
                                    break;
                            }

                            if (projectile.velocity.X > 8f)
                            {
                                projectile.velocity.X = 8f;
                            }
                        }
                        else
                        {
                            switch (Main.rand.Next(1, 2))
                            {

                                case 1:
                                    projectile.velocity.X -= 0.15f;
                                    break;

                                case 2:
                                    projectile.velocity.X -= 0.20f;
                                    break;
                            }

                            if (projectile.velocity.X < -8f)
                            {
                                projectile.velocity.X = -8f;
                            }
                        }
                    }
                    else
                    {
                        if (playerDistance > 800f)
                        {
                            fly = true;
                            projectile.velocity.X = 0f;
                            projectile.velocity.Y = 0f;
                            projectile.tileCollide = false;
                        }
                        if (playerDistance > 200f)
                        {
                            if (player.position.X - projectile.position.X > 0f)
                            {
                                switch (Main.rand.Next(1, 3))
                                {
                                    case 1:
                                        projectile.velocity.X += 0.05f;
                                        break;

                                    case 2:
                                        projectile.velocity.X += 0.10f;
                                        break;

                                    case 3:
                                        projectile.velocity.X += 0.15f;
                                        break;
                                }

                                if (projectile.velocity.X > 6f)
                                {
                                    projectile.velocity.X = 6f;
                                }
                            }
                            else
                            {
                                switch (Main.rand.Next(1, 3))
                                {
                                    case 1:
                                        projectile.velocity.X -= 0.05f;
                                        break;

                                    case 2:
                                        projectile.velocity.X -= 0.10f;
                                        break;

                                    case 3:
                                        projectile.velocity.X -= 0.15f;
                                        break;
                                }

                                if (projectile.velocity.X < -6f)
                                {
                                    projectile.velocity.X = -6f;
                                }
                            }
                        }
                        if (playerDistance < 200f)
                        {
                            if (projectile.velocity.X != 0f)
                            {
                                if (projectile.velocity.X > 0.5f)
                                {
                                    switch (Main.rand.Next(1, 3))
                                    {
                                        case 1:
                                            projectile.velocity.X -= 0.05f;
                                            break;

                                        case 2:
                                            projectile.velocity.X -= 0.10f;
                                            break;

                                        case 3:
                                            projectile.velocity.X -= 0.15f;
                                            break;
                                    }
                                }
                                else if (projectile.velocity.X < -0.5f)
                                {
                                    switch (Main.rand.Next(1, 3))
                                    {
                                        case 1:
                                            projectile.velocity.X += 0.05f;
                                            break;

                                        case 2:
                                            projectile.velocity.X += 0.10f;
                                            break;

                                        case 3:
                                            projectile.velocity.X += 0.15f;
                                            break;
                                    }
                                }
                                else if (projectile.velocity.X < 0.5f && projectile.velocity.X > -0.5f)
                                {
                                    projectile.velocity.X = 0f;
                                }
                            }
                        }
                    }
                }
                else if (fly)
                {
                    Vector2 center2 = projectile.Center;
                    Vector2 vector48 = player.Center - center2 + new Vector2(0f, 0f);
                    float playerDistance = vector48.Length();
                    vector48.Normalize();
                    vector48 *= 14f;
                    projectile.velocity = (projectile.velocity * 40f + vector48) / 41f;

                    projectile.rotation = projectile.velocity.X * 0.03f;
                    if (playerDistance > 1500f)
                    {
                        projectile.position.X = Main.player[projectile.owner].Center.X - (float)(projectile.width / 2);
                        projectile.position.Y = Main.player[projectile.owner].Center.Y - (float)(projectile.height / 2);
                        projectile.netUpdate = true;
                    }
                    if (playerDistance < 100f)
                    {
                        if (player.velocity.Y == 0f)
                        {
                            ++playerStill;
                        }
                        else
                        {
                            playerStill = 0;
                        }
                        if (playerStill > 30 && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                        {
                            fly = false;
                            projectile.tileCollide = true;
                            projectile.rotation = 0;
                            projectile.velocity.X *= 0.30f;
                            projectile.velocity.Y *= 0.30f;
                        }
                    }
                }
                if ((double)projectile.velocity.X > 0.25)
                {
                    projectile.spriteDirection = -1;
                }
                else if ((double)projectile.velocity.X < -0.25)
                {
                    projectile.spriteDirection = 1;
                }
            }
            if (projectile.ai[0] == 1f)
            {
                projectile.rotation = 0;
                projectile.tileCollide = false;
                int num988 = 10;
                bool flag54 = false;
                bool flag55 = false;
                float[] var0 = projectile.localAI;
                int var1 = 0;
                float num73 = var0[var1];
                var0[var1] = num73 + 1f;
                if (projectile.localAI[0] % 30f == 0f)
                {
                    flag55 = true;
                }
                int num989 = (int)projectile.ai[1];
                if (projectile.localAI[0] >= (float)(60000 * num988)) //tryna make it stay on there "forever" without glitching
                {
                    flag54 = true;
                }
                else if (num989 < 0 || num989 >= 200)
                {
                    flag54 = true;
                }
                else if (Main.npc[num989].active && !Main.npc[num989].dontTakeDamage && Main.npc[num989].defense < 9999)
                {
                    projectile.Center = Main.npc[num989].Center - projectile.velocity * 2f;
                    projectile.gfxOffY = Main.npc[num989].gfxOffY;
                    if (flag55)
                    {
                        Main.npc[num989].HitEffect(0, 1.0);
                    }
                }
                else
                {
                    flag54 = true;
                }
                if (flag54)
                {
                    projectile.ai[0] = 0f;
                    projectile.velocity.X = 0f;
                    projectile.velocity.Y = 0f;
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Rectangle myRect = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);
            if (projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 200; i++)
                {
                    if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && Main.npc[i].defense < 9999 &&
                        ((projectile.friendly && (!Main.npc[i].friendly || projectile.type == 318 || (Main.npc[i].type == 22 && projectile.owner < 255 && Main.player[projectile.owner].killGuide) || (Main.npc[i].type == 54 && projectile.owner < 255 && Main.player[projectile.owner].killClothier))) ||
                        (projectile.hostile && Main.npc[i].friendly && !Main.npc[i].dontTakeDamageFromHostiles)) && (projectile.owner < 0 || Main.npc[i].immune[projectile.owner] == 0 || projectile.maxPenetrate == 1))
                    {
                        if (Main.npc[i].noTileCollide || !projectile.ownerHitCheck || projectile.CanHit(Main.npc[i]))
                        {
                            bool flag3;
                            if (Main.npc[i].type == 414)
                            {
                                Rectangle rect = Main.npc[i].getRect();
                                int num5 = 8;
                                rect.X -= num5;
                                rect.Y -= num5;
                                rect.Width += num5 * 2;
                                rect.Height += num5 * 2;
                                flag3 = projectile.Colliding(myRect, rect);
                            }
                            else
                            {
                                flag3 = projectile.Colliding(myRect, Main.npc[i].getRect());
                            }
                            if (flag3)
                            {
                                if (Main.npc[i].reflectingProjectiles && projectile.CanReflect())
                                {
                                    Main.npc[i].ReflectProjectile(projectile.whoAmI);
                                    return;
                                }
                                projectile.ai[0] = 1f;
                                projectile.ai[1] = (float)i;
                                projectile.velocity = (Main.npc[i].Center - projectile.Center) * 0.75f;
                                projectile.netUpdate = true;
                                projectile.StatusNPC(i);
                                projectile.damage = 0;
                                int num28 = 10;
                                Point[] array2 = new Point[num28];
                                int num29 = 0;
                                for (int l = 0; l < 1000; l++)
                                {
                                    if (l != projectile.whoAmI && Main.projectile[l].active && Main.projectile[l].owner == Main.myPlayer && Main.projectile[l].type == projectile.type && Main.projectile[l].ai[0] == 1f && Main.projectile[l].ai[1] == (float)i)
                                    {
                                        array2[num29++] = new Point(l, Main.projectile[l].timeLeft);
                                        if (num29 >= array2.Length)
                                        {
                                            break;
                                        }
                                    }
                                }
                                if (num29 >= array2.Length)
                                {
                                    int num30 = 0;
                                    for (int m = 1; m < array2.Length; m++)
                                    {
                                        if (array2[m].Y < array2[num30].Y)
                                        {
                                            num30 = m;
                                        }
                                    }
                                    Main.projectile[array2[num30].X].Kill();
                                }
                            }
                        }
                    }
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.penetrate == 0)
            {
                projectile.Kill();
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.buffImmune[ModContent.BuffType<ShellfishEating>()] = false;
            if (target.type == ModContent.NPCType<CeaselessVoid>() || target.type == ModContent.NPCType<EidolonWyrmHeadHuge>())
            {
                target.buffImmune[ModContent.BuffType<ShellfishEating>()] = true;
            }
            target.AddBuff(ModContent.BuffType<ShellfishEating>(), 600000);
        }
    }
}
