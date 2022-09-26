using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Pets
{
    public class FurtasticDuoBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Furtastic Duo");
            Description.SetDefault("They just did! The Furtastic Duo will accompany you!");
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            player.Calamity().kendra = true;
            player.Calamity().bearPet = true;

            // Spawn both component pets individually
            if (player.whoAmI == Main.myPlayer)
            {
                List<int> pets = new List<int> { ModContent.ProjectileType<Bear>(), ModContent.ProjectileType<KendraPet>() };
                foreach (int petProjID in pets)
                    if (player.ownedProjectileCounts[petProjID] <= 0)
                        Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, petProjID, 0, 0f, player.whoAmI);
            }
        }
    }
}
