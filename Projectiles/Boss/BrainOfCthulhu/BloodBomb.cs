using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss.BrainOfCthulhu;

public class BloodBomb : ModNPC, ILocalizedModType
{
    public new string LocalizationCategory => "NPCs";

    internal static Asset<Texture2D> BloodBombYellow;
    internal static Texture2D BloodBombGlow;

    public override void SetStaticDefaults()
    {
        NPCID.Sets.ProjectileNPC[Type] = true;
        BloodBombYellow = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/BrainOfCthulhu/BloodBomb2");
        this.HideFromBestiary();
    }

    internal static Texture2D GetBombGlow()
    {
        if (BloodBombGlow == null)
        {
            var tex = new Texture2D(Main.graphics.GraphicsDevice, BloodBombYellow.Value.Width, BloodBombYellow.Value.Height);

            var BaseArray = new Color[tex.Width * tex.Height];
            var ColorArray = new Color[tex.Width * tex.Height];
            BloodBombYellow.Value.GetData(BaseArray);
            for (var i = 0; i < BaseArray.Length; i++)
            {
                ColorArray[i] = new Color(255, 255, 255) * (((float)BaseArray[i].A) / 255f);
            }
            tex.SetData(ColorArray);
            BloodBombGlow = tex;
        }

        return BloodBombGlow;
    }

    public override void SetDefaults()
    {
        NPC.width = 30;
        NPC.height = 30;
        NPC.noGravity = true;
        NPC.damage = 10;
        NPC.defense = 5;
        NPC.lifeMax = 60;
        NPC.Calamity().canBreakPlayerDefense = true;
        NPC.lavaImmune = true;
        NPC.aiStyle = -1;
        AIType = -1;
        NPC.value = 0;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCHit1 with { Pitch = 0.5f };
        NPC.knockBackResist = 0f;
        NPC.chaseable = false;
        NPC.noTileCollide = true;
        NPC.ShowNameOnHover = false;
    }

    public override void OnSpawn(IEntitySource source)
    {
        if (source is EntitySource_Parent { Entity: NPC n })
            NPC.ai[1] = n.whoAmI;
        else
            NPC.ai[1] = -1;

        Vector2 goal = Main.player[0].Center - NPC.Center;

        float vyi = 6;
        float vyisq = 36;
        float g = 0.2f;

        float vyf = -MathF.Sqrt(vyisq + 2 * g * goal.Y);
        float t = Math.Abs((vyf - vyi) / g);

        float vxi = goal.X / t;

        NPC.velocity = new Vector2(float.IsNaN(vxi) ? (goal.X > 0 ? 10 : -10) : vxi, -vyi);

        for (int i = 0; i < 3; i++)
        {
            BloodParticle p = new(NPC.Center, NPC.velocity.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 6f, MathHelper.Pi / 6f)) * Main.rand.NextFloat(2f, 3f), 32, 1f, Color.Red);
            GeneralParticleHandler.SpawnParticle(p);
        }
        BloodParticle2 p2 = new(NPC.Center, NPC.velocity * 2.5f, 16, 0.5f, Color.Red);
        GeneralParticleHandler.SpawnParticle(p2);
    }

    public override void AI()
    {
        if (NPC.ai[0] > 1200)
            NPC.active = false;

        NPC.velocity.Y += 0.2f;

        NPC.rotation += Math.Sign(NPC.velocity.X) * (NPC.velocity.Length()) * 0.01f;

        if (NPC.ai[0] % 3 == 0)
        {
            float deathRatio = 1 - (NPC.life / NPC.lifeMax);
            float time = Main.GlobalTimeWrappedHourly * (10f + (10f * deathRatio));
            float lerp = MathF.Sin(time) / 2f + 0.5f;
            Color color = Color.Lerp(Color.Red, Color.Yellow, lerp);

            BloodParticle p = new(NPC.Center + Main.rand.NextVector2Circular(NPC.width, NPC.height), (-NPC.velocity).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 6f, MathHelper.Pi / 6f)) * Main.rand.NextFloat(0.25f, 0.75f), Main.rand.Next(10, 17), Main.rand.NextFloat(0.5f, 1f), color);
            GeneralParticleHandler.SpawnParticle(p);
        }

        NPC.ai[0]++;
    }

    public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
    {
        if (modifiers.DamageType == TrueMeleeDamageClass.Instance)
        {
            modifiers.FinalDamage *= 1.5f;
            return;
        }

        float dist = player.DistanceSQ(NPC.Center);
        dist = MathHelper.Clamp(dist - 10000, 0, 160000);
        if (dist > 160000)
            modifiers.FinalDamage *= 0;
        else
            modifiers.FinalDamage *= MathHelper.Lerp(1f, 0f, dist / 160000f);
    }

    public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
    {
        if (modifiers.DamageType == TrueMeleeDamageClass.Instance)
        {
            modifiers.FinalDamage *= 1.5f;
            return;
        }

        if (projectile.owner == -1)
        {
            modifiers.FinalDamage *= 0;
            return;
        }

        float dist = Main.player[projectile.owner].DistanceSQ(NPC.Center);
        dist = MathHelper.Clamp(dist - 10000, 0, 160000);
        if (dist > 160000f)
            modifiers.FinalDamage *= 0;
        else
            modifiers.FinalDamage *= MathHelper.Lerp(1f, 0f, dist / 160000f);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if(NPC.life <= 0)
        {
            if (NPC.ai[1] == -1)
                NPC.ai[1] = NPC.Center.ClosestNPCAt(1200, true, true).whoAmI;
            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.DirectionTo(Main.npc[(int)NPC.ai[1]].Center) * 16f, ModContent.ProjectileType<BloodBombRTS>(), 40, 1f, -1, NPC.ai[1]);
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        float deathRatio = 1 - (NPC.life / NPC.lifeMax);
        float time = Main.GlobalTimeWrappedHourly * (10f + (10f * deathRatio));
        float lerp = MathF.Sin(time) / 2f + 0.5f;

        Texture2D baseTex = TextureAssets.Npc[Type].Value;
        Texture2D overlayTex = BloodBombYellow.Value;

        Vector2 drawPos = NPC.Center - Main.screenPosition + Main.rand.NextVector2CircularEdge((2 * deathRatio), (2 * deathRatio));
        float scale = 1.5f + (MathF.Sin(time) * 0.25f);

        if (Main.LocalPlayer.HasBuff(BuffID.Hunter))
            spriteBatch.Draw(GetBombGlow(), drawPos, null, Color.OrangeRed, NPC.rotation, BloodBombGlow.Size() * 0.5f, scale + 0.15f, 0, 0);

        spriteBatch.Draw(baseTex, drawPos, null, Color.White, NPC.rotation, baseTex.Size() * 0.5f, scale, 0, 0);
        spriteBatch.Draw(overlayTex, drawPos, null, Color.White * lerp, NPC.rotation, overlayTex.Size() * 0.5f, scale, 0, 0);
        return false;
    }
}

public class BloodBombRTS : ModProjectile, ILocalizedModType
{
    public new string LocalizationCategory => "Projectiles.Boss";
    public override string Texture => "CalamityMod/Projectiles/Boss/BrainOfCthulhu/BloodBomb";
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 8;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 30;
        Projectile.height = 30;
        Projectile.penetrate = 1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.damage = 10;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 360;
    }

    public override void AI()
    {
        NPC npc = Main.npc[(int)Projectile.ai[0]];
        if(npc.active && !npc.dontTakeDamage)
            Projectile.velocity = Projectile.DirectionTo(npc.Center) * 16f;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.SetCrit();
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);

        CustomPulse explosion1 = new(Projectile.Center, Vector2.Zero, Color.OrangeRed, "CalamityMod/Particles/FlameExplosion", Vector2.One, Main.rand.NextFloat(0f, MathHelper.Pi), 0f, 0.1f, 24);
        GeneralParticleHandler.SpawnParticle(explosion1);

        CustomPulse explosion2 = new(Projectile.Center, Vector2.Zero, Color.Red, "CalamityMod/Particles/FlameExplosion", Vector2.One, Main.rand.NextFloat(0f, MathHelper.Pi), 0f, 0.075f, 24);
        GeneralParticleHandler.SpawnParticle(explosion2);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Projectile[Type].Value;
        Texture2D overlayTex = BloodBomb.BloodBombYellow.Value;


        for (int i = 0; i < (CalamityClientConfig.Instance.Afterimages ? Projectile.oldPos.Length : 1); ++i)
        {
            float time = (Main.GlobalTimeWrappedHourly * 30) - (i / 2f);
            float lerp = MathF.Sin(time) / 2f + 0.5f;
            Color color = Color.White;
            float afterimageRot = Projectile.oldRot[i];
            Vector2 drawPos = Projectile.oldPos[i] + (Projectile.Size / 2f) - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            if (i != 0)
                color *= 0.75f;

            // DO NOT REMOVE THESE "UNNECESSARY" FLOAT CASTS. THIS WILL BREAK THE AFTERIMAGES.
            float interpolant = ((float)(Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length);
            Main.spriteBatch.Draw(tex, drawPos, null, color * (1 - lerp), afterimageRot, tex.Size() * 0.5f, (Projectile.scale + MathF.Sin(time) / 2f + 0.5f) * interpolant, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(overlayTex, drawPos, null, color * lerp, afterimageRot, overlayTex.Size() * 0.5f, (Projectile.scale + MathF.Sin(time) / 2f + 0.5f) * interpolant, SpriteEffects.None, 0f);

        }
        return false;
    }
}
