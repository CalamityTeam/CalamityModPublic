using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.ILEditing;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.Melee.Yoyos;
using MonoMod;
using MonoMod.Cil;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.VanillaProjectileOverrides
{
    public class CounterweightEffects : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        bool runInitialization = false;
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            //All counterweights fall within this range.
            if (entity.type >= ProjectileID.BlackCounterweight && entity.type <= ProjectileID.YellowCounterweight)
                return true;
            return false;
        }

        public override bool PreAI(Projectile projectile)
        {
            var player = Main.player[projectile.owner];
            int id = player.HeldItem.type;

            if (!runInitialization)
            {
                if (id == ModContent.ItemType<Pandemic>())
                    projectile.MaxUpdates++;
                runInitialization = true;
            }
            return true;
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var player = Main.player[projectile.owner];
            int id = player.HeldItem.type;
            if ( id == ModContent.ItemType<Pandemic>())
                target.AddBuff(ModContent.BuffType<Plague>(), 300);
            if (id == ModContent.ItemType<TheObliterator>())
            {
                foreach (var item in Main.ActiveProjectiles)
                {
                    if (item.owner == player.whoAmI && item.ModProjectile is ObliteratorYoyo o) 
                    {
                        o.DashQueue++;
                        item.netUpdate = true;
                    }
                }
            }
        }
    }
}
