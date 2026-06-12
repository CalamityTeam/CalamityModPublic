using CalamityMod.Projectiles.Summon;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class OasisElementalBuff : BaseSummonBuff
    {
        protected override int MinionProjectileType => ModContent.ProjectileType<SandElementalHealer>();

        protected override ref bool MinionBool => ref BuffModdedOwner.oasisEleBuff;
    }
}
