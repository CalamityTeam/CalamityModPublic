using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
	public class Greentide : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greentide");
            Tooltip.SetDefault("Summons teeth from the sky on hit");
        }

        public override void SetDefaults()
        {
            item.damage = 95;
            item.melee = true;
            item.width = 62;
            item.height = 62;
            item.useTime = 24;
            item.useAnimation = 24;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 7;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.shootSpeed = 18f;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (target.type == NPCID.TargetDummy)
            {
                return;
            }
            if (player.whoAmI == Main.myPlayer)
                SpawnTeeth(player, knockback);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (player.whoAmI == Main.myPlayer)
                SpawnTeeth(player, item.knockBack);
        }

        private void SpawnTeeth(Player player, float knockBack)
        {
			CalamityUtils.ProjectileToMouse(player, 4, item.shootSpeed, 0f, 180f, ModContent.ProjectileType<GreenWater>(), (int)(item.damage * player.MeleeDamage()), knockBack, player.whoAmI, true);
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            int randomDust = Main.rand.Next(2);
            if (randomDust == 0)
            {
                randomDust = 33;
            }
            else
            {
                randomDust = 89;
            }
            if (Main.rand.NextBool(4))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, randomDust);
            }
        }
    }
}
