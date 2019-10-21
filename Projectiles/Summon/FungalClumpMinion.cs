using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Healing;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class FungalClumpMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fungal Clump");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.minionSlots = 0f;
            projectile.aiStyle = 54;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            aiType = 317;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            bool flag64 = projectile.type == ModContent.ProjectileType<FungalClumpMinion>();
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (!modPlayer.fungalClump)
            {
                projectile.active = false;
                return;
            }
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.fClump = false;
                }
                if (modPlayer.fClump)
                {
                    projectile.timeLeft = 2;
                }
            }
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = Main.player[projectile.owner].minionDamage;
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int num226 = 36;
                for (int num227 = 0; num227 < num226; num227++)
                {
                    Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                    Vector2 vector7 = vector6 - projectile.Center;
                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 56, vector7.X * 1.5f, vector7.Y * 1.5f, 100, default, 1.4f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].noLight = true;
                    Main.dust[num228].velocity = vector7;
                }
                projectile.localAI[0] += 1f;
            }
            if (Main.player[projectile.owner].minionDamage != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    Main.player[projectile.owner].minionDamage);
                projectile.damage = damage2;
            }
            if (Main.rand.NextBool(16))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 56, projectile.velocity.X * 0.05f, projectile.velocity.Y * 0.05f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.type == NPCID.TargetDummy || !target.canGhostHeal)
            {
                return;
            }
            float num = (float)damage * 0.25f;
            if ((int)num == 0)
            {
                return;
            }
            if (Main.player[Main.myPlayer].lifeSteal <= 0f)
            {
                return;
            }
            Main.player[Main.myPlayer].lifeSteal -= num;
            int num2 = projectile.owner;
            Projectile.NewProjectile(target.position.X, target.position.Y, 0f, 0f, ModContent.ProjectileType<FungalHeal>(), 0, 0f, projectile.owner, (float)num2, num);
        }
    }
}
