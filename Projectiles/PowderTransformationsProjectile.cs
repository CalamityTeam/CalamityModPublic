using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.NormalNPCs.HorribleHog;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class PowderTransformationsProjectile : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return entity.type == ProjectileID.VilePowder || entity.type == ProjectileID.ViciousPowder || entity.type == ProjectileID.PurificationPowder;
        }

        public override void AI(Projectile projectile)
        {
            // Transform certain NPCs into different variants when contact is made with Vile/Vicious Powder or Purification Powder.
            // Currently only used for Piggy, Divine Swine and Horrible Hog.

            bool isPurePowder = projectile.type == ProjectileID.PurificationPowder;
            bool isEvilPowder = projectile.type == ProjectileID.VilePowder || projectile.type == ProjectileID.ViciousPowder;

            foreach (NPC npc in Main.ActiveNPCs)
            {
                bool targetIsPiggy = npc.type == ModContent.NPCType<Piggy>() || npc.type == ModContent.NPCType<PiggyGold>();
                bool targetIsDivineSwine = npc.type == ModContent.NPCType<DivineSwine>();
                bool targetIsHorribleHog = npc.type == ModContent.NPCType<HorribleHog>();

                if (!targetIsPiggy && !targetIsDivineSwine && !targetIsHorribleHog)
                    continue;

                if (projectile.Hitbox.Intersects(npc.Hitbox))
                {
                    if (targetIsPiggy && npc.ModNPC<Piggy>().TryTransformingIntoVariant())
                    {
                        if (isEvilPowder)
                            npc.ModNPC<Piggy>().TransformIntoVariant(ModContent.NPCType<HorribleHog>());
                        else if (isPurePowder)
                            npc.ModNPC<Piggy>().TransformIntoVariant(ModContent.NPCType<DivineSwine>());
                    }

                    if (targetIsDivineSwine && isEvilPowder && npc.ModNPC<DivineSwine>().TryTransformingIntoPiggy())
                        npc.ModNPC<DivineSwine>().TransformIntoPiggy();

                    if (targetIsHorribleHog && isPurePowder && npc.ModNPC<HorribleHog>().TryTransformingIntoPiggy())
                        npc.ModNPC<HorribleHog>().TransformIntoPiggy();
                }
            }
        }
    }
}
