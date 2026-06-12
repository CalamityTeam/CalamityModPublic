using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.DataStructures;
using CalamityMod.Utilities.Daybreak;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss.BrainOfCthulhu;

public class TelekineticEnemyGrab : ModProjectile, ILocalizedModType
{
    public new string LocalizationCategory => "Projectiles.Boss";
    public override string Texture => "CalamityMod/Particles/BloomRing";

    BezierCurve curve = null;
    int throwSign = 0;
    Vector2 throwPos = Vector2.Zero;
    Vector2 holdPos;
    int MyTarget = -1;

    ref float Time => ref Projectile.ai[0];
    ref float StunTime => ref Projectile.ai[1];

    private static Dictionary<int, Texture2D> EnemyGlowTextures = [];
    int enemyID { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = (float)value; }

    public override void SetDefaults()
    {
        Projectile.width = 48;
        Projectile.height = 48;
        Projectile.penetrate = -1;
        Projectile.Opacity = 1f;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 1200;
        Projectile.damage = 10;
        Projectile.hostile = true;
        if(NPC.crimsonBoss != -1)
            MyTarget = Main.npc[NPC.crimsonBoss].target;
        Projectile.Calamity().DealsDefenseDamage = true;
    }

    public override void OnSpawn(IEntitySource source)
    {
        holdPos = new Vector2(Projectile.Center.X, Main.npc[NPC.crimsonBoss].Center.Y - 128) - Main.npc[NPC.crimsonBoss].Center;

        Projectile.rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);

        StunTime = -1;

        enemyID = Main.rand.Next(0, 3);
        int[] enemyIDs = [NPCID.FaceMonster, NPCID.Crimera, NPCID.BloodCrawler];
        enemyID = enemyIDs[enemyID];

        MyTarget = Main.npc[NPC.crimsonBoss].target;

        Projectile.netUpdate = true;
    }

    public override void AI()
    {
        if (NPC.crimsonBoss == -1)
        {
            Projectile.active = false;
            return;
        }

        if (Time <= 90)
            Projectile.hostile = false;
        else
            Projectile.hostile = true;

        bool throwing = Time > 180;
        float throwTime = Time - 180;
        if (throwing)
        {
            if (throwSign == 0)
                throwSign = Math.Sign(Projectile.Center.X - Main.npc[NPC.crimsonBoss].Center.X) * -Math.Sign(Main.player[MyTarget].Center.Y - Main.npc[NPC.crimsonBoss].Center.Y);
        }
        bool thrown = throwTime > 90;

        if (!thrown)
        {
            Vector2 startPoint = Main.npc[NPC.crimsonBoss].Center;// Main.npc[NPC.crimsonBoss].Center;
            Vector2 endPoint = Projectile.Center;

            if (StunTime == -1)
            {
                if (!throwing)
                {
                    if (Time >= 150)
                        Projectile.velocity = ((holdPos + Main.npc[NPC.crimsonBoss].Center) - Projectile.Center) / 30f;
                    else if (Time != 0)
                    {

                        if (Time <= 90)
                        {
                            if (Time == 90)
                                Projectile.velocity = Vector2.UnitY * -24f;
                            if (Time % 30 == 0)
                                Projectile.velocity = Vector2.UnitY * -16f;
                            else
                                Projectile.velocity *= 0.33f;
                        }
                        else
                            Projectile.velocity *= 0.966f;
                    }
                }
                else
                {
                    if (throwTime <= 30)
                        throwPos = (Projectile.Center + Projectile.velocity) - (Main.npc[NPC.crimsonBoss].Center + Main.npc[NPC.crimsonBoss].velocity);
                    Vector2 target = Main.player[MyTarget].Center;
                    Vector2 throwDir = (target - (throwPos + (Main.npc[NPC.crimsonBoss].Center + Main.npc[NPC.crimsonBoss].velocity))).SafeNormalize(Vector2.UnitY);

                    if (throwTime >= 30 && throwTime <= 90)
                    {
                        if (throwTime <= 60f)
                        {
                            Projectile.Center = Vector2.Lerp(throwPos, throwPos - throwDir * 56f, CalamityUtils.SineInOutEasing((throwTime - 30) / 30f, 1)) + (Main.npc[NPC.crimsonBoss].Center + Main.npc[NPC.crimsonBoss].velocity);
                            Projectile.velocity = Vector2.Zero;
                        }
                        else
                        {
                            Projectile.velocity += throwDir * 0.9f;
                            if (throwTime == 90)
                            {
                                float dist = Projectile.Center.Distance(target) / 2f;
                                dist /= Projectile.velocity.Length();
                                Projectile.velocity.Y -= dist * 0.6f;
                                Projectile.tileCollide = true;
                            }
                        }
                    }
                }
            }

            Vector2 direction = endPoint - startPoint;
            float distance = Vector2.Distance(startPoint, endPoint);

            float lerp = CalamityUtils.SineInOutEasing(MathHelper.Clamp((throwTime) / 60f, 0f, 1f), 1);
            float xMult = MathHelper.Lerp(-Math.Clamp(direction.X / 256f, -1, 1), throwSign, lerp);
            float yMult = Math.Clamp(direction.Y / 256f, -1, 1);
            float curveIntensity = Math.Clamp(distance / 420f, 0f, 0.66f) * xMult * yMult;
            //Main.NewText("Completion: " + lerp + ", Intensity: " + curveIntensity);
            Vector2 perpindicular = direction.RotatedBy(MathHelper.PiOver2);

            Vector2 controlPoint1 = startPoint + (direction * 0.25f) + (perpindicular * curveIntensity);
            Vector2 controlPoint2 = startPoint + (direction * 0.75f) + (perpindicular * curveIntensity);

            curve = new BezierCurve(startPoint, controlPoint1, controlPoint2, endPoint);
        }
        else
        {
            Projectile.velocity.Y += 0.6f;
        }

        if (StunTime >= 0)
        {
            if (StunTime < 30f)
                StunTime++;
            else
            {
                Projectile.velocity.Y += 0.6f;
                Projectile.tileCollide = true;
            }
        }
        else
        {
            if (Time > 90)
                Projectile.rotation += (Projectile.velocity.X / 100f) - (Math.Sign(Main.npc[NPC.crimsonBoss].Center.X - Projectile.Center.X) * 0.025f);
            Time++;
        }
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(throwSign);

        writer.WriteVector2(throwPos);

        writer.WriteVector2(holdPos);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        throwSign = reader.ReadInt32();

        throwPos = reader.ReadVector2();

        holdPos = reader.ReadVector2();
    }

    public override bool OnTileCollide(Vector2 oldVelocity) => true;

    public override void OnKill(int timeLeft)
    {
        Vector2 velocity = Projectile.velocity.RotatedBy(MathHelper.Pi) / 8f;
        Vector2 pos = Projectile.Center + Projectile.velocity;

        switch (enemyID)
        {
            case NPCID.FaceMonster:
                Gore.NewGore(Projectile.GetSource_Death(), pos - (Projectile.velocity / 2f), velocity.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)), 237);

                for (int i = 0; i < 24; i++)
                    Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(16, 32), DustID.Blood, Scale: Main.rand.NextFloat(1, 2));

                SoundEngine.PlaySound(SoundID.NPCDeath1 with { Volume = 0.25f }, pos);
                break;
            case NPCID.Crimera:
                Gore.NewGore(Projectile.GetSource_Death(), pos - (Projectile.velocity / 2f), velocity.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)), 223);
                Gore.NewGore(Projectile.GetSource_Death(), pos - (Projectile.velocity / 2f), velocity.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2)), 224);

                for (int i = 0; i < 24; i++)
                    Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(16, 32), DustID.Blood, Scale: Main.rand.NextFloat(1, 2));

                SoundEngine.PlaySound(SoundID.NPCDeath1 with { Volume = 0.25f }, pos);
                break;
            case NPCID.BloodCrawler:
                Gore.NewGore(Projectile.GetSource_Death(), pos - (Projectile.velocity / 2f), velocity.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)), 351);
                Gore.NewGore(Projectile.GetSource_Death(), pos - (Projectile.velocity / 2f), velocity.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2)), 352);
                Gore.NewGore(Projectile.GetSource_Death(), pos - (Projectile.velocity / 2f), velocity.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2)), 353);

                for (int i = 0; i < 24; i++)
                    Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(16, 32), DustID.Blood, Scale: Main.rand.NextFloat(1, 2));

                SoundEngine.PlaySound(SoundID.NPCDeath1 with { Volume = 0.25f }, pos);
                break;
            default:
                Gore.NewGore(Projectile.GetSource_Death(), pos - (Projectile.velocity / 2f), velocity.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)), 42);
                Gore.NewGore(Projectile.GetSource_Death(), pos - (Projectile.velocity / 2f), velocity.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2)), 43);
                Gore.NewGore(Projectile.GetSource_Death(), pos - (Projectile.velocity / 2f), velocity.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2)), 44);

                SoundEngine.PlaySound(SoundID.NPCDeath2 with { Volume = 0.175f }, pos);
                break;
        }
    }

    bool evenRed = false;
    public override bool PreDraw(ref Color lightColor)
    {
        //Handles getting the glow textures
        if (EnemyGlowTextures.Count == 0 || !EnemyGlowTextures.ContainsKey(enemyID))
        {
            EnemyGlowTextures.Clear();
            int[] enemyIDs = [NPCID.FaceMonster, NPCID.Crimera, NPCID.BloodCrawler];
            foreach (int id in enemyIDs)
            {
                Main.instance.LoadNPC(id);
                var baseTex = TextureAssets.Npc[id];
                var glow = new Texture2D(Main.graphics.GraphicsDevice, baseTex.Value.Width, baseTex.Value.Height);

                var BaseArray = new Color[glow.Width * glow.Height];
                var ColorArray = new Color[glow.Width * glow.Height];
                baseTex.Value.GetData(BaseArray);
                for (var i = 0; i < BaseArray.Length; i++)
                {
                    if (BaseArray[i].A != 0)
                        ColorArray[i] = new Color(255, 255, 255);
                }
                glow.SetData(ColorArray);

                EnemyGlowTextures.Add(id, glow);
            }
        }

        int type = enemyID;
        int frameCount;
        int wrapFrame = -1;
        int startFrame = 0;
        float mult = 0.75f;

        switch (type)
        {
            case NPCID.FaceMonster:
                frameCount = 16;
                startFrame = 2;
                wrapFrame = 16;
                break;
            case NPCID.Crimera:
                frameCount = 2;
                mult = 0.25f;
                break;
            case NPCID.BloodCrawler:
                frameCount = 5;
                mult = 0.25f;
                break;
            default:
                frameCount = 15;
                type = NPCID.Skeleton;
                startFrame = 1;
                break;
        }

        if (wrapFrame == -1)
            wrapFrame = frameCount;

        Texture2D tex = TextureAssets.Npc[type].Value;

        int currentFrame = ((int)((1200 - Projectile.timeLeft) * mult)) % (wrapFrame - startFrame);

        Rectangle frame = tex.Frame(1, frameCount, 0, startFrame + currentFrame);

        float throwTime = Time - 180;

        float opacity = 1f;
        if (Time < 10)
            opacity = Time / 10f;

        if (throwTime <= 90)
        {
            int pCount = 12;

            float wrapTime = 60;
            float wrappedTime = MathHelper.Clamp((Time % (wrapTime + 1)) / wrapTime, 0f, 1f);
            if (wrappedTime == 0)
                evenRed = !evenRed;

            float glowOpacity = MathHelper.Clamp(1 - ((throwTime - 75) / 15f), 0f, 1f);
            if (StunTime >= 0)
                glowOpacity = 1 - (StunTime / 30f);
            if (Time < 15)
                glowOpacity = Time / 15f;

            if (curve != null)
            {
                Main.spriteBatch.End(out var snapshot);
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                Texture2D ring = TextureAssets.Projectile[Type].Value;

                List<Vector2> points = curve.GetPoints(pCount);
                for (int i = 1; i < pCount; i++)
                {
                    float scale1 = MathHelper.Lerp(0.1f, 1f, i / (float)(pCount - 1));
                    float scale2 = MathHelper.Lerp(0.1f, 1f, (i + 1) / (float)(pCount - 1));
                    float scale = MathHelper.Lerp(scale1, scale2, wrappedTime);

                    float rot1 = (points[i] - points[i - 1]).ToRotation();

                    float rot2;
                    if (i == pCount - 1)
                        rot2 = points[i].ToRotation();
                    else
                        rot2 = (points[i + 1] - points[i]).ToRotation();

                    float rot;
                    if (i == pCount - 1)
                        rot = rot1;
                    else
                        rot = rot1.AngleLerp(rot2, wrappedTime);

                    Vector2 pos;
                    if (i == pCount - 1)
                        pos = Vector2.Lerp(points[i], points[i] + rot.ToRotationVector2() * Vector2.Distance(points[^1], points[^2]), wrappedTime);
                    else
                        pos = Vector2.Lerp(points[i], points[i + 1], wrappedTime);

                    Color color;
                    if (evenRed)
                        color = (i % 2 == 0 ? Color.Red : Color.Magenta) * 0.666f;
                    else
                        color = (i % 2 == 0 ? Color.Magenta : Color.Red) * 0.666f;

                    if (i == pCount - 1)
                        color *= 1 - wrappedTime;
                    else if (i == 1)
                        color *= wrappedTime;

                    Main.spriteBatch.Draw(ring, pos + Projectile.velocity - Main.screenPosition, null, color * glowOpacity, rot, ring.Size() * 0.5f, new Vector2(0.5f, 1f) * scale, 0, 0);
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(snapshot);
            }

            Vector2[] offsets = [Vector2.UnitX * 2, Vector2.UnitX * -2, Vector2.UnitY * 2, Vector2.UnitY * -2];
            for (int i = 0; i < 4; i++)
                Main.EntitySpriteDraw(EnemyGlowTextures[enemyID], Projectile.Center - Main.screenPosition + offsets[i].RotatedBy(Projectile.rotation), frame, Color.Lerp(Color.Red, Color.Magenta, ((float)Math.Sin((Main.GlobalTimeWrappedHourly * 5f)) / 2f + 0.5f)) * glowOpacity, Projectile.rotation, frame.Size() * 0.5f, 1f, 0);
        }

        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, lightColor * opacity, Projectile.rotation, frame.Size() * 0.5f, 1f, 0);

        return false;
    }
}

