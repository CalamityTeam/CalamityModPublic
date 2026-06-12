using CalamityMod.CalPlayer;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class VoidConcentrationBuff : BaseSummonBuff
    {
        protected override ref bool MinionBool => ref BuffModdedOwner.VoidConcentrationStaff;
        protected override int MinionProjectileType => ModContent.ProjectileType<VoidConcentrationMinion>();
    }
}
