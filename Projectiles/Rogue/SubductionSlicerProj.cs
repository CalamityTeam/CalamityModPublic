using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityMod.Projectiles.Rogue
{
    public class SubductionSlicerProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/SubductionSlicer";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Subduction Slicer");
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = 3;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 600;
            Projectile.alpha = 55;
            AIType = ProjectileID.WoodenBoomerang;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.25f, 0.15f, 0f);
            if (Main.rand.NextBool(5))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, Main.rand.NextBool(3) ? 16 : 127, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            Vector2 goreVec = new Vector2(Projectile.position.X + Projectile.width / 2 + Projectile.velocity.X, Projectile.position.Y + Projectile.height / 2 + Projectile.velocity.Y);
            if (Main.rand.NextBool(8) && Main.netMode != NetmodeID.Server)
            {
                int smoke = Gore.NewGore(Projectile.GetSource_FromAI(), goreVec, default, Main.rand.Next(375, 378), 0.75f);
                Main.gore[smoke].behindTiles = true;
            }
            if (Projectile.localAI[0] > 0f)
                Projectile.localAI[0]--;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 6;
            target.AddBuff(BuffID.OnFire, 240);
            OnHitEffects(target.Center);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 240);
            OnHitEffects(target.Center);
        }

        private void OnHitEffects(Vector2 position)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, Vector2.Zero, ModContent.ProjectileType<FuckYou>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
                if (proj.WithinBounds(Main.maxProjectiles))
                    Main.projectile[proj].Calamity().forceRogue = true;
                if (Projectile.Calamity().stealthStrike && Projectile.localAI[0] <= 0f)
                {
                    Point result;
                    if (WorldUtils.Find(Projectile.Top.ToTileCoordinates(), Searches.Chain((GenSearch)new Searches.Down(80), (GenCondition)new Conditions.IsSolid()), out result))
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), result.ToVector2() * 16f, Vector2.Zero, ModContent.ProjectileType<SubductionFlameburst>(), (int)(Projectile.damage * 1.5f), 2f, Projectile.owner, 1f);
                        Projectile.localAI[0] = 30f;
                    }
                }
            }
        }
    }
}
