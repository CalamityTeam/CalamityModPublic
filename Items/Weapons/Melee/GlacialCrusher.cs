using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class GlacialCrusher : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glacial Crusher");
            Tooltip.SetDefault("Fires very slow frost projectiles that gain strength as they travel and freeze enemies\n" +
                "Enemies are frozen for longer the further the projectile travels\n" +
                "True melee strikes cause tremendous damage to frozen enemies\n" +
                "Enemies that cannot be frozen take increased damage");
        }

        public override void SetDefaults()
        {
            item.width = 60;
            item.damage = 65;
            item.melee = true;
            item.useAnimation = 27;
            item.useTime = 27;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 58;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.shoot = ModContent.ProjectileType<Iceberg>();
            item.shootSpeed = 5f;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 67);
            }
        }

        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            if (target.buffImmune[ModContent.BuffType<GlacialState>()])
            {
                damage *= 2;
                knockBack *= 2f;
            }
            else if (target.Calamity().gState > 0)
            {
                damage *= 3;
                knockBack *= 3f;
            }
        }
    }
}
