using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DesertScourge
{
    public class AquaticDischarge : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Aquatic Discharge");
			Tooltip.SetDefault("Enemies release electric sparks on death");
		}

		public override void SetDefaults()
		{
			item.useStyle = 3;
			item.useTurn = false;
			item.useAnimation = 12;
			item.useTime = 12;
			item.width = 32;
			item.height = 32;
			item.damage = 23;
			item.melee = true;
			item.knockBack = 5.5f;
			item.UseSound = SoundID.Item1;
			item.useTurn = true;
			item.autoReuse = true;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
		}

	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(5) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 226);
	        }
	    }

	    public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
	    	if (target.life <= 0)
	    	{
	    		Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, mod.ProjectileType("Spark"), (int)((float)item.damage * player.meleeDamage), knockback, Main.myPlayer);
	    	}
		}
	}
}
