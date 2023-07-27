using CalamityMod.NPCs.Crags;
using CalamityMod.NPCs.Other;
using CalamityMod.Particles;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class AlicornonaStick : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Misc";
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 50;
            Item.holdStyle = 6;
            Item.value = 0;
            Item.rare = ModContent.RarityType<CalamityRed>();
        }

        public override void HoldItem(Player player)
        {
            int offset = player.direction == 1 ? 5 : -Item.width - 5;
            Rectangle itemRect = new Rectangle((int)player.Center.X + offset, (int)player.position.Y - 10, Item.width, Item.height);
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc != null && !npc.dontTakeDamage && npc.type != ModContent.NPCType<THELORDE>())
                {
                    if (itemRect.Intersects(npc.getRect()))
                    {
                        int damage = npc.SimpleStrikeNPC((int)(npc.lifeMax / 200), player.direction, true);
                        SoundEngine.PlaySound(Projectiles.Summon.CnidarianJellyfishOnTheString.SlapSound with { Volume = 2, MaxInstances = 200 }, npc.Center);
                        if (Main.netMode != NetmodeID.Server)
                        {
                            BloodShed(itemRect, npc, damage, player);
                        }
                    }
                }
            }
        }

        public void BloodShed(Rectangle hitBox, NPC target, int damage, Player player)
        {
            // this is violence but exaggerated
            float damageInterpolant = Utils.GetLerpValue(950f, 2000f, damage, true);

            Vector2 impactPoint = Vector2.Lerp(hitBox.Center.ToVector2(), target.Center, 0.65f);
            Vector2 bloodSpawnPosition = target.Center + Main.rand.NextVector2Circular(target.width, target.height) * 0.04f;
            Vector2 splatterDirection = (new Vector2(bloodSpawnPosition.X * player.direction, bloodSpawnPosition.Y)).SafeNormalize(Vector2.UnitY);

            // Emit blood if the target is organic.
            if (target.Organic())
            {
                for (int i = 0; i < 16; i++)
                {
                    int bloodLifetime = Main.rand.Next(22, 36);
                    float bloodScale = Main.rand.NextFloat(0.6f, 0.8f);
                    Color bloodColor = Color.Lerp(Color.Red, Color.DarkRed, Main.rand.NextFloat());
                    bloodColor = Color.Lerp(bloodColor, new Color(51, 22, 94), Main.rand.NextFloat(0.65f));

                    if (Main.rand.NextBool(20))
                        bloodScale *= 2f;

                    Vector2 bloodVelocity = splatterDirection.RotatedByRandom(0.81f) * Main.rand.NextFloat(11f, 23f);
                    bloodVelocity.Y -= 12f;
                    BloodParticle blood = new BloodParticle(bloodSpawnPosition, bloodVelocity, bloodLifetime, bloodScale, bloodColor);
                    GeneralParticleHandler.SpawnParticle(blood);
                }
                for (int i = 0; i < 9; i++)
                {
                    float bloodScale = Main.rand.NextFloat(0.2f, 0.33f);
                    Color bloodColor = Color.Lerp(Color.Red, Color.DarkRed, Main.rand.NextFloat(0.5f, 1f));
                    Vector2 bloodVelocity = splatterDirection.RotatedByRandom(0.9f) * Main.rand.NextFloat(9f, 14.5f);
                    BloodParticle2 blood = new BloodParticle2(bloodSpawnPosition, bloodVelocity, 20, bloodScale, bloodColor);
                    GeneralParticleHandler.SpawnParticle(blood);
                }
            }

            // Emit sparks if the target is not organic.
            else
            {
                for (int i = 0; i < 16; i++)
                {
                    int sparkLifetime = Main.rand.Next(22, 36);
                    float sparkScale = Main.rand.NextFloat(0.8f, 1f) + damageInterpolant * 0.85f;
                    Color sparkColor = Color.Lerp(Color.Silver, Color.Gold, Main.rand.NextFloat(0.7f));
                    sparkColor = Color.Lerp(sparkColor, Color.Orange, Main.rand.NextFloat());

                    if (Main.rand.NextBool(10))
                        sparkScale *= 2f;

                    Vector2 sparkVelocity = splatterDirection.RotatedByRandom(0.6f) * Main.rand.NextFloat(12f, 25f);
                    sparkVelocity.Y -= 6f;
                    SparkParticle spark = new SparkParticle(impactPoint, sparkVelocity, true, sparkLifetime, sparkScale, sparkColor);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
        }
    }
}
