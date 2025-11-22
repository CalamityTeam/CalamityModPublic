using System.IO;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class WildfireBloomHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<WildfireBloom>();
        public override string Texture => "CalamityMod/Projectiles/Ranged/WildfireBloomHoldout";
        public override Vector2 GunTipPosition => base.GunTipPosition - Vector2.UnitX.RotatedBy(Projectile.rotation) * 17f - (Vector2.UnitY.RotatedBy(Projectile.rotation) * 7f * Projectile.spriteDirection * Owner.gravDir);
        public override float MaxOffsetLengthFromArm => 20f;
        public override float OffsetXUpwards => -5f;
        public override float BaseOffsetY => -5f;
        public override float OffsetYDownwards => 5f;

        public ref float ShotCooldown => ref Projectile.ai[0];
        public ref float ShotsFired => ref Projectile.ai[1];
        public ref float ShootTimer => ref Projectile.ai[2];
        public int FireBlobs { get; set; }

        public override void KillHoldoutLogic()
        {
            base.KillHoldoutLogic();
            if (ShotsFired >= 16)
                Projectile.Kill();
        }

        public override void HoldoutAI()
        {
            ShootTimer++;
            if (ShootTimer >= 60)
            {
                if (ShotCooldown == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item34, Projectile.Center);
                    Owner.PickAmmo(Owner.ActiveItem(), out _, out float shootSpeed, out int damage, out float knockback, out _, Main.rand.NextBool(2));
                    if (Main.myPlayer == Projectile.owner)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, (Projectile.velocity * 9).RotatedByRandom(0.08f), ModContent.ProjectileType<WildfireBloomFire>(), damage, knockback, Projectile.owner);
                    ShotsFired++;
                    ShotCooldown = HeldItem.useTime;
                    if (FireBlobs == 0 && Main.myPlayer == Projectile.owner)
                    {
                        float randAngle = Main.rand.NextFloat(8f, 15f);
                        Vector2 newVel = (Projectile.velocity * 9).RotatedBy(MathHelper.ToRadians(randAngle)) * 2f;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, newVel, ModContent.ProjectileType<WildfireBloomFlare>(), damage, knockback, Projectile.owner);
                        newVel = (Projectile.velocity * 9).RotatedBy(MathHelper.ToRadians(-randAngle)) * 2f;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, newVel, ModContent.ProjectileType<WildfireBloomFlare>(), damage, knockback, Projectile.owner);
                        FireBlobs = 3;
                    }
                    else
                        FireBlobs--;
                }
                else
                    ShotCooldown--;
            }
            else
            {
                ShotsFired = 0;
                for (int i = 0; i < 2; i++)
                {
                    float rotMulti = Main.rand.NextFloat(0.3f, 1f);
                    Dust dust2 = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(5) ? 135 : 107);
                    dust2.noGravity = true;
                    dust2.velocity = new Vector2(0, -2).RotatedByRandom(rotMulti * 0.3f) * (Main.rand.NextFloat(1f, 2.9f) - rotMulti);
                    dust2.scale = Main.rand.NextFloat(1.2f, 1.8f) * (ShootTimer * 0.015f);
                }
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            SoundEngine.PlaySound(SoundID.Item73 with { Volume = 0.7f }, Projectile.Center);
        }

        public override void SendExtraAIHoldout(BinaryWriter writer) => writer.Write(FireBlobs);

        public override void ReceiveExtraAIHoldout(BinaryReader reader) => FireBlobs = reader.ReadInt32();
    }
}
