using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod
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

    // For my own sanity and yours, only a base worm with the bare amount of segments is summoned for these two cunts.
    // Not doing this results in some freaky shit, much akin to the old mechworm bugs, where segments die or attach to nonsensical things.
    // However, in this instance, these bugs were appearing in singleplayer as well, for unknown reasons.

	#region The Stupid Fucking Segmented Projectiles
	public class DeadMechwormProperties : DeadMinionProperties
    {
        public override bool DisallowMultipleEntries => true;
        public DeadMechwormProperties(int originalDamage, float originalKnockback)
            : base(ModContent.ProjectileType<MechwormHead>(), 0f, originalDamage, originalKnockback, null)
        {
            // The minion slot decalaration is explicitly layed out in this child constructor instead of in the parent
            // parameter for extra clarity.
            RequiredMinionSlots = 1f;
        }

        public override void SummonCopy(int ownerIndex)
        {
            if (Main.myPlayer != ownerIndex)
                return;

            Player owner = Main.player[ownerIndex];
            StaffoftheMechworm.SummonBaseMechworm(OriginalDamage, OriginalKnockback, owner, out int tailIndex);
        }
    }

    public class DeadStardustDragonProperties : DeadMinionProperties
    {
        public override bool DisallowMultipleEntries => true;
        public DeadStardustDragonProperties(int originalDamage, float originalKnockback)
            : base(ProjectileID.StardustDragon1, 0f, originalDamage, originalKnockback, null)
        {
            // The minion slot decalaration is explicitly layed out in this child constructor instead of in the parent
            // parameter for extra clarity.
            RequiredMinionSlots = 1f;
        }

        public static void SummonBaseDragon(int damage, float knockback, Player owner, out int tailIndex)
        {
            tailIndex = -1;
            if (Main.myPlayer != owner.whoAmI)
                return;

            int curr = Projectile.NewProjectile(owner.Center, Vector2.Zero, ProjectileID.StardustDragon1, damage, knockback, owner.whoAmI);
            curr = Projectile.NewProjectile(owner.Center, Vector2.Zero, ProjectileID.StardustDragon2, damage, knockback, owner.whoAmI, curr);
            int prev = curr;
            curr = Projectile.NewProjectile(owner.Center, Vector2.Zero, ProjectileID.StardustDragon3, damage, knockback, owner.whoAmI, curr);
            Main.projectile[prev].localAI[1] = curr;
            prev = curr;
            curr = Projectile.NewProjectile(owner.Center, Vector2.Zero, ProjectileID.StardustDragon4, damage, knockback, owner.whoAmI, curr);
            Main.projectile[prev].localAI[1] = curr;

            tailIndex = curr;
        }

        public override void SummonCopy(int ownerIndex)
        {
            if (Main.myPlayer != ownerIndex)
                return;

            Player owner = Main.player[ownerIndex];
            SummonBaseDragon(OriginalDamage, OriginalKnockback, owner, out int tailIndex);
        }
    }

	#endregion

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
