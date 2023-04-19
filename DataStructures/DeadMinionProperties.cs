using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.DataStructures
{
    public class DeadMinionProperties
    {
        public int Type;
        public int Damage;
        public int OriginalDamage;
        public float RequiredMinionSlots;
        public float OriginalKnockback;
        public float[] AIValues;

        // summoners shine stuff
        internal int SummonersShine_SourceItem;
        internal int SummonersShine_Crit;
        internal float SummonersShine_MinionAS;
        internal float SummonersShine_PrefixMinionPower;

        public virtual bool DisallowMultipleEntries => false;
        public DeadMinionProperties(int type, float requiredMinionSlots, int originalDamage, int damage, float originalKnockback, float[] aiValues)
        {
            Type = type;
            RequiredMinionSlots = requiredMinionSlots;
            Damage = damage;
            AIValues = aiValues;
            OriginalKnockback = originalKnockback;
            OriginalDamage = originalDamage;
        }

        public virtual void SummonCopy(int ownerIndex)
        {
            if (Main.myPlayer != ownerIndex)
                return;

            Player owner = Main.player[ownerIndex];

            // Ozzatron 19APR2023: If Summoner's Shine is loaded, use a Call from its API to respawn minions
            IEntitySource source = new EntitySource_Misc("1");
            Mod summonersShine = CalamityMod.Instance.summonersShine;
            if (summonersShine is not null)
                source = (IEntitySource)summonersShine.Call(10, 21, SummonersShine_SourceItem, SummonersShine_MinionAS, SummonersShine_Crit, SummonersShine_PrefixMinionPower);
            int copy = Projectile.NewProjectile(source, owner.Center, Vector2.Zero, Type, Damage, OriginalKnockback, ownerIndex, AIValues[0], AIValues[1]);

            if (Main.projectile.IndexInRange(copy))
            {
                Main.projectile[copy].Calamity().RequiresManualResurrection = true;
                Main.projectile[copy].originalDamage = OriginalDamage;

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
        public DeadEndoHydraProperties(int headCount, int originalDamage, int damage, float originalKnockback)
            : base(ModContent.ProjectileType<EndoHydraBody>(), 0f, originalDamage, damage, originalKnockback, null)
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

            // Ozzatron 19APR2023: If Summoner's Shine is loaded, use a Call from its API to respawn minions
            IEntitySource source = new EntitySource_Misc("1");
            Mod summonersShine = CalamityMod.Instance.summonersShine;
            if (summonersShine is not null)
                source = (IEntitySource)summonersShine.Call(10, 21, SummonersShine_SourceItem, SummonersShine_MinionAS, SummonersShine_Crit, SummonersShine_PrefixMinionPower);
            int body = Projectile.NewProjectile(source, owner.Center, Vector2.Zero, ModContent.ProjectileType<EndoHydraBody>(), Damage, OriginalKnockback, owner.whoAmI);

            // If there was not enough space for even the body to be created, don't both trying to create even more projectiles needlessly.
            if (!Main.projectile.IndexInRange(body))
                return;

            Main.projectile[body].originalDamage = OriginalDamage;
            while (HeadCount > 0)
            {
                int head = Projectile.NewProjectile(new EntitySource_Misc("1"), owner.Center, Main.rand.NextVector2Unit(), ModContent.ProjectileType<EndoHydraHead>(), Damage, OriginalKnockback, owner.whoAmI, body);
                Main.projectile[head].originalDamage = OriginalDamage;
                HeadCount--;
            }
        }
    }

    public class DeadEndoCooperProperties : DeadMinionProperties
    {
        public int AttackMode;
        public override bool DisallowMultipleEntries => true;
        public DeadEndoCooperProperties(int attackMode, float requiredMinionSlots, int originalDamage, int damage, float originalKnockback)
            : base(ModContent.ProjectileType<EndoHydraBody>(), requiredMinionSlots, originalDamage, damage, originalKnockback, null)
        {
            AttackMode = attackMode;
        }

        public override void SummonCopy(int ownerIndex)
        {
            if (Main.myPlayer != ownerIndex)
                return;

            Player owner = Main.player[ownerIndex];

            // Ozzatron 19APR2023: If Summoner's Shine is loaded, use a Call from its API to respawn minions
            IEntitySource source = new EntitySource_Misc("1");
            Mod summonersShine = CalamityMod.Instance.summonersShine;
            if (summonersShine is not null)
                source = (IEntitySource)summonersShine.Call(10, 21, SummonersShine_SourceItem, SummonersShine_MinionAS, SummonersShine_Crit, SummonersShine_PrefixMinionPower);
            Endogenesis.SummonEndoCooper(source, AttackMode, Main.MouseWorld, Damage, OriginalDamage, OriginalKnockback, owner, out int bodyIndex, out int limbsIndex);
            Main.projectile[bodyIndex].Calamity().RequiresManualResurrection = true;
            Main.projectile[limbsIndex].Calamity().RequiresManualResurrection = true;
        }
    }
}
