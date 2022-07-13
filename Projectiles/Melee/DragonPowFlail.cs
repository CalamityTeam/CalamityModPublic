using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Melee
{
    public class DragonPowFlail : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon Pow");
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;

            // Only hits enemies once, but doesn't give them iframes so sparks and explosions can hit.
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            Projectile.alpha = 255;
        }

        // ai[0] = flail's current behavior state
        // 0 = flail is heading outwards
        // 1 = flail is coming back (Any other value also makes the flail return)
        // ai[1] = tick counter used to spawn projectiles and control transparency
        public override void AI()
        {
            DrawOffsetX = 0;
            DrawOriginOffsetY = -11;
            DrawOriginOffsetX = 0;

            Player owner = Main.player[Projectile.owner];

            // Update facing so it's always directly away from player
            Vector2 posDiff = owner.Center - Projectile.Center;
            Projectile.rotation = posDiff.ToRotation();

            // Destroy immediately if the owner is dead
            if (owner.dead)
            {
                Projectile.Kill();
                return;
            }

            // Make the owner face the correct direction for the flail
            if (posDiff.X < 0f)
            {
                owner.ChangeDir(1);
                Projectile.direction = 1;
                Projectile.spriteDirection = 1;
                Projectile.rotation += MathHelper.Pi; // keep dragon head facing up
            }
            else
            {
                owner.ChangeDir(-1);
                Projectile.direction = -1;
                Projectile.spriteDirection = -1;
            }

            // Keep the owner's hand out for as long as necessary and face it the right way
            owner.itemRotation = (-1f * posDiff * (float)Projectile.direction).ToRotation();
            owner.itemAnimation = 6;
            owner.itemTime = 6;

            // If the flail is far enough out and still "heading out", then swap it to be returning
            float dist = posDiff.Length();
            if (Projectile.ai[0] == 0f && dist > 800f)
                Projectile.ai[0] = 1f;

            // Flail returning behavior
            if (Projectile.ai[0] != 0f)
            {
                // If the flail gets way too far away, just destroy it
                if (dist > 1500f)
                {
                    Projectile.Kill();
                    return;
                }

                // Set velocity to be directly towards the player
                Projectile.velocity = posDiff.SafeNormalize(Vector2.Zero) * DragonPow.ReturnSpeed;

                // Once the flail is close enough to the player, destroy it
                if (posDiff.Length() < DragonPow.ReturnSpeed)
                {
                    Projectile.Kill();
                    return;
                }
            }

            // Creates a flurry of dust
            int numDust = 5;
            for (int i = 0; i < numDust; ++i)
            {
                int dustType = Main.rand.NextBool(3) ? 246 : 244;
                float scale = 0.8f + Main.rand.NextFloat(0.6f);
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].scale = scale;
                Main.dust[idx].velocity *= 2f;
                Main.dust[idx].velocity += Projectile.velocity * 0.3f;
            }

            // Increment the frame counter
            ++Projectile.ai[1];

            // If the flail has been out for at least 5 frames, make it visible
            Projectile.alpha = (Projectile.ai[1] > 5f) ? 0 : 255;

            if (Projectile.ai[1] % 4 == 0)
            {
                int type = ModContent.ProjectileType<DraconicSpark>();
                int damage = Projectile.damage / 8;
                float kb = 3f;
                float speed = DragonPow.SparkSpeed;
                Vector2 vel = new Vector2(speed, 0f).RotatedByRandom(MathHelper.TwoPi);
                vel += Projectile.velocity * 0.05f;
                float sparkVariety = Main.rand.Next(3);

                if (Projectile.owner == Main.myPlayer)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, type, damage, kb, Projectile.owner, sparkVariety, 0f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Only perform hit effects after reaching approximately 40% of the way out.
            if (Main.player[Projectile.owner].WithinRange(target.Center, 345f))
                return;

            // Inflicts Daybroken, Abyssal Flames and Holy Flames for 3 seconds on-hit
            target.AddBuff(BuffID.Daybreak, 180);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);

            Projectile.ai[0] = 1f;
            Projectile.netUpdate = true;

            PetalStorm(target.Center);
            Waterfalls(target.Center);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            // Only perform hit effects after reaching approximately 55% of the way out.
            if (Projectile.WithinRange(Main.player[Projectile.owner].Center, 345f))
                return;

            // Inflicts Abyssal Flames and Holy Flames for 8 seconds on-hit
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);

            Projectile.ai[0] = 1f;
            Projectile.netUpdate = true;

            PetalStorm(target.Center);
            Waterfalls(target.Center);
        }

        // Spawns a storm of flower petals on-hit.
        private void PetalStorm(Vector2 targetPos)
        {
            SoundEngine.PlaySound(SoundID.Item105, targetPos);

            int type = ProjectileID.FlowerPetal;
            int numPetals = 12;
            var source = Projectile.GetSource_FromThis();
            int petalDamage = Projectile.damage / 8;
            float petalKB = 0f;
            for (int i = 0; i < numPetals; ++i)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                    Projectile petal = CalamityUtils.ProjectileBarrage(source, Projectile.Center, targetPos, Main.rand.NextBool(), 1000f, 1400f, 80f, 900f, Main.rand.NextFloat(DragonPow.MinPetalSpeed, DragonPow.MaxPetalSpeed), type, petalDamage, petalKB, Projectile.owner);
                    if (petal.whoAmI.WithinBounds(Main.maxProjectiles))
                    {
                        petal.DamageType = DamageClass.Melee;
                        petal.rotation = angle;
                        petal.usesLocalNPCImmunity = true;
                        petal.localNPCHitCooldown = -1;
                    }
                }
            }
        }

        private void Waterfalls(Vector2 targetPos)
        {
            int type = ModContent.ProjectileType<Waterfall>();
            int numWaterfalls = 12;
            int waterfallDamage = Projectile.damage / 6;
            float waterfallKB = 0f;
            for (int i = 0; i < numWaterfalls; ++i)
            {
                float startOffsetX = Main.rand.NextFloat(-120f, 120f);
                float startOffsetY = Main.rand.NextFloat(-740f, -700f);
                Vector2 startPos = new Vector2(targetPos.X + startOffsetX, targetPos.Y + startOffsetY);
                float fallSpeed = Main.rand.NextFloat(DragonPow.MinWaterfallSpeed, DragonPow.MaxWaterfallSpeed);
                Vector2 fallVec = new Vector2(0f, fallSpeed);
                fallVec = fallVec.RotatedBy(Main.rand.NextFloat(-0.08f, 0.08f));

                if (Projectile.owner == Main.myPlayer)
                {
                    int idx = Projectile.NewProjectile(Projectile.GetSource_FromThis(), startPos, fallVec, type, waterfallDamage, waterfallKB, Projectile.owner, 0f, 0f);
                    Main.projectile[idx].timeLeft = 100;
                    Main.projectile[idx].tileCollide = false;
                }
            }
        }

        // PreDraw draws the flail's chain (underneath it, so it doesn't look weird)
        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];
            Vector2 mountedCenter = owner.MountedCenter;
            Color transparent = Color.Transparent;
            Texture2D chainTex = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Chains/DragonPowChain").Value;
            Vector2 chainDrawPos = Projectile.Center;

            Rectangle? sourceRectangle = null;
            Vector2 origin = new Vector2(chainTex.Width * 0.5f, chainTex.Height * 0.5f);

            Vector2 posDiff = mountedCenter - chainDrawPos;
            float rot = (float)Math.Atan2(posDiff.Y, posDiff.X) - MathHelper.PiOver2;

            // If going right: Flip chain segments over so they face upwards
            if (posDiff.X < 0f)
                rot += MathHelper.Pi;

            // Just don't draw anything if there's NaNs to prevent crashes
            bool keepDrawing = true;
            if (chainDrawPos.HasNaNs() || posDiff.HasNaNs())
                keepDrawing = false;

            // Draw chain segments until you run out of space
            while (keepDrawing)
            {
                // Stop drawing when closer than 1 chain link away.
                if (posDiff.Length() < chainTex.Height + 1f)
                {
                    break;
                }

                Vector2 chainDirection = posDiff.SafeNormalize(Vector2.Zero);
                chainDrawPos += chainDirection * chainTex.Height;
                posDiff = mountedCenter - chainDrawPos;
                Color colorAtLoc = Lighting.GetColor((int)chainDrawPos.X / 16, (int)chainDrawPos.Y / 16);
                Main.spriteBatch.Draw(chainTex, chainDrawPos - Main.screenPosition, sourceRectangle, colorAtLoc, rot, origin, 1f, SpriteEffects.None, 0);
            }
            return true;
        }
    }
}
