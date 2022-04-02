using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.DataStructures
{
    public class DeadMinionProperties
    {
        public int Type;
        public int OriginalDamage;
        public float RequiredMinionSlots;
        public float OriginalKnockback;
        public float[] AIValues;
        public virtual bool DisallowMultipleEntries => false;
        public DeadMinionProperties(int type, float requiredMinionSlots, int originalDamage, float originalKnockback, float[] aiValues)
        {
            Type = type;
            RequiredMinionSlots = requiredMinionSlots;
            OriginalDamage = originalDamage;
            AIValues = aiValues;
            OriginalKnockback = originalKnockback;
        }

        public virtual void SummonCopy(int ownerIndex)
        {
            if (Main.myPlayer != ownerIndex)
                return;

            Player owner = Main.player[ownerIndex];
            int copy = Projectile.NewProjectile(owner.Center, Vector2.Zero, Type, OriginalDamage, OriginalKnockback, ownerIndex, AIValues[0], AIValues[1]);

            if (Main.projectile.IndexInRange(copy))
            {
                Main.projectile[copy].Calamity().RequiresManualResurrection = true;

                // Set the amount of slots as needed for the minion.
                // This is specifically done for logarithmic minions and should have no negative affect on anything else.
                Main.projectile[copy].minionSlots = RequiredMinionSlots;
            }
        }
    }

    public class DeadEndoHydraProperties : DeadMinionProperties
    {
        public int HeadCount;
        public override bool DisallowMultipleEntries => true;
        public DeadEndoHydraProperties(int headCount, int originalDamage, float originalKnockback)
            : base(ModContent.ProjectileType<EndoHydraBody>(), 0f, originalDamage, originalKnockback, null)
        {
            HeadCount = headCount;

            // The minion slot decalaration is explicitly layed out in this child constructor instead of in the parent
            // parameter for extra clarity.
            RequiredMinionSlots = HeadCount;
        }

        public override void SummonCopy(int ownerIndex)
        {
            if (Main.myPlayer != ownerIndex || HeadCount <= 0)
                return;

            Player owner = Main.player[ownerIndex];
            int body = Projectile.NewProjectile(owner.Center, Vector2.Zero, ModContent.ProjectileType<EndoHydraBody>(), OriginalDamage, OriginalKnockback, owner.whoAmI);

            // If there was not enough space for even the body to be created, don't both trying to create even more projectiles needlessly.
            if (!Main.projectile.IndexInRange(body))
                return;

            while (HeadCount > 0)
            {
                Projectile.NewProjectile(owner.Center, Main.rand.NextVector2Unit(), ModContent.ProjectileType<EndoHydraHead>(), OriginalDamage, OriginalKnockback, owner.whoAmI, body);
                HeadCount--;
            }
        }
    }

    public class DeadEndoCooperProperties : DeadMinionProperties
    {
        public int AttackMode;
        public override bool DisallowMultipleEntries => true;
        public DeadEndoCooperProperties(int attackMode, float requiredMinionSlots, int originalDamage, float originalKnockback)
            : base(ModContent.ProjectileType<EndoHydraBody>(), requiredMinionSlots, originalDamage, originalKnockback, null)
        {
            AttackMode = attackMode;
        }

        public override void SummonCopy(int ownerIndex)
        {
            if (Main.myPlayer != ownerIndex)
                return;

            Player owner = Main.player[ownerIndex];
            Endogenesis.SummonEndoCooper(AttackMode, Main.MouseWorld, OriginalDamage, OriginalKnockback, owner, out int bodyIndex, out int limbsIndex);
            Main.projectile[bodyIndex].Calamity().RequiresManualResurrection = true;
            Main.projectile[limbsIndex].Calamity().RequiresManualResurrection = true;
        }
    }
}
