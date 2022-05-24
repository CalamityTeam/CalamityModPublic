using Terraria.DataStructures;
using CalamityMod.NPCs.Providence;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Weapons.Melee
{
    public class ColdheartIcicle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Coldheart Icicle");
            Tooltip.SetDefault("Drains a percentage of enemy health on hit\nCannot inflict critical hits");
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.damage = 1;
            Item.DamageType = DamageClass.Melee;
            Item.width = Item.height = 24;
            Item.useTime = 27;
            Item.useAnimation = 27;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.UseSound = SoundID.Item1;
            Item.useTurn = true;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
        }

        public override void ModifyHitPvp(Player player, Player target, ref int damage, ref bool crit)
        {
            damage = target.statLifeMax2 * 2 / 100;
            target.statDefense = 0;
            target.endurance = 0f;
            crit = false;
        }

        // LATER -- Providence specifically is immune to Coldheart Icicle. There is probably a better way to do this
        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            damage = 1;
            crit = false;
            if (target.type != NPCID.TargetDummy && target.type != ModContent.NPCType<Providence>())
                target.life -= target.lifeMax * 2 / 100;
            target.checkDead();
        }
    }
}
